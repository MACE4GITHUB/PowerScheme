using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Updater.Common;

internal abstract class JsonNode;

internal sealed class JsonObject : JsonNode
{
    private readonly Dictionary<string, JsonNode> _properties = new(StringComparer.Ordinal);

    public bool TryGetValue(string name, out JsonNode? value) =>
        _properties.TryGetValue(name, out value);

    public void Add(string name, JsonNode value) =>
        _properties[name] = value;
}

internal sealed class JsonArray : JsonNode
{
    private readonly List<JsonNode> _items = [];

    public IReadOnlyList<JsonNode> Items => _items;

    public void Add(JsonNode item) =>
        _items.Add(item);
}

internal sealed class JsonString(string value) : JsonNode
{
    public string Value { get; } = value;
}

internal sealed class JsonBoolean(bool value) : JsonNode
{
    public bool Value { get; } = value;
}

internal sealed class JsonNumber : JsonNode
{
    public JsonNumber(long value)
    {
        Integer = value;
        Value = value;
    }

    public JsonNumber(double value) =>
        Value = value;

    public double Value { get; }

    public long? Integer { get; }
}

internal sealed class JsonNull : JsonNode
{
    public static JsonNull Instance { get; } = new();

    private JsonNull()
    {
    }
}

internal static class JsonParser
{
    public static JsonNode Parse(string text)
    {
        if (text is null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        var parser = new Parser(text);
        var value = parser.ReadValue();
        parser.SkipWhitespace();

        if (!parser.IsAtEnd)
        {
            throw parser.CreateException("Unexpected trailing characters.");
        }

        return value;
    }

    private sealed class Parser(string text)
    {
        private readonly string _text = text;
        private int _position;

        public bool IsAtEnd => _position >= _text.Length;

        public JsonNode ReadValue()
        {
            SkipWhitespace();

            if (IsAtEnd)
            {
                throw CreateException("Unexpected end of input.");
            }

            return _text[_position] switch
            {
                '{' => ReadObject(),
                '[' => ReadArray(),
                '"' => new JsonString(ReadString()),
                't' => ReadLiteral("true", new JsonBoolean(true)),
                'f' => ReadLiteral("false", new JsonBoolean(false)),
                'n' => ReadLiteral("null", JsonNull.Instance),
                _ => ReadNumber()
            };
        }

        public void SkipWhitespace()
        {
            while (!IsAtEnd && char.IsWhiteSpace(_text[_position]))
            {
                _position++;
            }
        }

        public FormatException CreateException(string message) =>
            new($"{message} Position: {_position}.");

        private JsonNode ReadObject()
        {
            Expect('{');

            var result = new JsonObject();

            if (TryConsume('}'))
            {
                return result;
            }

            while (true)
            {
                SkipWhitespace();

                if (IsAtEnd || _text[_position] != '"')
                {
                    throw CreateException("Expected object property name.");
                }

                var name = ReadString();
                Expect(':');
                result.Add(name, ReadValue());

                if (TryConsume('}'))
                {
                    return result;
                }

                Expect(',');
            }
        }

        private JsonNode ReadArray()
        {
            Expect('[');

            var result = new JsonArray();

            if (TryConsume(']'))
            {
                return result;
            }

            while (true)
            {
                result.Add(ReadValue());

                if (TryConsume(']'))
                {
                    return result;
                }

                Expect(',');
            }
        }

        private string ReadString()
        {
            Expect('"');

            var builder = new StringBuilder();

            while (!IsAtEnd)
            {
                var character = _text[_position++];

                if (character == '"')
                {
                    return builder.ToString();
                }

                if (character != '\\')
                {
                    builder.Append(character);
                    continue;
                }

                if (IsAtEnd)
                {
                    break;
                }

                var escape = _text[_position++];

                switch (escape)
                {
                    case '"':
                        builder.Append('"');
                        break;
                    case '\\':
                        builder.Append('\\');
                        break;
                    case '/':
                        builder.Append('/');
                        break;
                    case 'b':
                        builder.Append('\b');
                        break;
                    case 'f':
                        builder.Append('\f');
                        break;
                    case 'n':
                        builder.Append('\n');
                        break;
                    case 'r':
                        builder.Append('\r');
                        break;
                    case 't':
                        builder.Append('\t');
                        break;
                    case 'u':
                        if (_position + 4 > _text.Length)
                        {
                            throw CreateException("Invalid unicode escape sequence.");
                        }

                        var hex = _text.Substring(_position, 4);

                        if (!ushort.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var code))
                        {
                            throw CreateException("Invalid unicode escape sequence.");
                        }

                        builder.Append((char)code);
                        _position += 4;
                        break;
                    default:
                        throw CreateException($"Invalid escape character '{escape}'.");
                }
            }

            throw CreateException("Unterminated string.");
        }

        private JsonNode ReadNumber()
        {
            var start = _position;

            while (!IsAtEnd)
            {
                var character = _text[_position];

                if ((character >= '0' && character <= '9') || character is '-' or '+' or '.' or 'e' or 'E')
                {
                    _position++;
                    continue;
                }

                break;
            }

            if (_position == start)
            {
                throw CreateException("Unexpected character.");
            }

            var token = _text.Substring(start, _position - start);

            if (long.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out var integer))
            {
                return new JsonNumber(integer);
            }

            if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
            {
                return new JsonNumber(number);
            }

            throw CreateException($"Invalid number '{token}'.");
        }

        private JsonNode ReadLiteral(string literal, JsonNode value)
        {
            if (string.CompareOrdinal(_text, _position, literal, 0, literal.Length) != 0)
            {
                throw CreateException($"Invalid literal, expected '{literal}'.");
            }

            _position += literal.Length;

            return value;
        }

        private void Expect(char expected)
        {
            SkipWhitespace();

            if (IsAtEnd || _text[_position] != expected)
            {
                throw CreateException($"Expected '{expected}'.");
            }

            _position++;
        }

        private bool TryConsume(char expected)
        {
            SkipWhitespace();

            if (IsAtEnd || _text[_position] != expected)
            {
                return false;
            }

            _position++;

            return true;
        }
    }
}
