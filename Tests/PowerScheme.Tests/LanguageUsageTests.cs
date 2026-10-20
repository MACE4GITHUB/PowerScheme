using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Languages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PowerScheme.Tests;

[TestClass]
public sealed class LanguageUsageTests
{
    private const BindingFlags DeclaredMembers = BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private static readonly IReadOnlyDictionary<short, OpCode> OpCodesByValue = typeof(OpCodes)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(field => field.FieldType == typeof(OpCode))
        .Select(field => (OpCode)field.GetValue(null)!)
        .ToDictionary(opCode => opCode.Value);

    [TestMethod]
    public void EveryTranslationProperty_IsUsedByProductionCode()
    {
        var used = FindTranslationGetters(ProductionAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .SelectMany(type => type.GetMethods(DeclaredMembers).Cast<MethodBase>()
                .Concat(type.GetConstructors(DeclaredMembers))));
        AssertAllTranslationsUsed(used);
    }

    [TestMethod]
    [DataRow(nameof(Language.UpdateApp))]
    [DataRow(nameof(Language.Quit))]
    public void UsageCheck_ReportsUnusedPropertyName(string propertyName)
    {
        var used = new HashSet<string>(typeof(Language).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(property => property.Name), StringComparer.Ordinal);
        used.Remove(propertyName);

        var failure = Assert.ThrowsExactly<AssertFailedException>(() => AssertAllTranslationsUsed(used));

        StringAssert.Contains(failure.Message, "Unused translation properties: " + propertyName);
    }

    private static void AssertAllTranslationsUsed(ISet<string> used)
    {
        var unused = typeof(Language).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.PropertyType == typeof(string) && !used.Contains(property.Name))
            .Select(property => property.Name)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.AreEqual(0, unused.Length, "Unused translation properties: " + string.Join(", ", unused));
    }

    [TestMethod]
    public void UsageScanner_CountsGetterCallsButNotNamesInStrings()
    {
        var method = typeof(LanguageUsageTests).GetMethod(nameof(TranslationReference), DeclaredMembers)!;

        var used = FindTranslationGetters([method]);

        CollectionAssert.AreEquivalent(new[] { nameof(Language.UpdateApp) }, used.ToArray());
    }

    private static string TranslationReference() =>
        Language.Current.UpdateApp + nameof(Language.Quit);

    private static IEnumerable<Assembly> ProductionAssemblies()
    {
        var pending = new Stack<Assembly>();
        var visited = new HashSet<string>(StringComparer.Ordinal);
        pending.Push(typeof(MenuLookup).Assembly);
        while (pending.Count > 0)
        {
            var assembly = pending.Pop();
            if (!visited.Add(assembly.FullName) || assembly.GlobalAssemblyCache)
            {
                continue;
            }

            if (assembly != typeof(Language).Assembly)
            {
                yield return assembly;
            }

            foreach (var reference in assembly.GetReferencedAssemblies())
            {
                pending.Push(Assembly.Load(reference));
            }
        }
    }

    private static HashSet<string> FindTranslationGetters(IEnumerable<MethodBase> methods)
    {
        var used = new HashSet<string>(StringComparer.Ordinal);
        foreach (var method in methods)
        {
            var bytes = method.GetMethodBody()?.GetILAsByteArray();
            if (bytes == null)
            {
                continue;
            }

            var position = 0;
            while (position < bytes.Length)
            {
                var value = bytes[position++];
                var opCode = OpCodesByValue[value == 0xfe
                    ? unchecked((short)(0xfe00 | bytes[position++]))
                    : (short)value];
                if (opCode.OperandType == OperandType.InlineMethod)
                {
                    var target = method.Module.ResolveMethod(BitConverter.ToInt32(bytes, position),
                        method.DeclaringType?.GetGenericArguments(),
                        method.IsGenericMethod ? method.GetGenericArguments() : null);
                    if (target is MethodInfo { IsStatic: false } getter && getter.DeclaringType == typeof(Language) &&
                        getter.ReturnType == typeof(string) && getter.Name.StartsWith("get_", StringComparison.Ordinal))
                    {
                        used.Add(getter.Name.Substring(4));
                    }
                }

                position += opCode.OperandType switch
                {
                    OperandType.InlineNone => 0,
                    OperandType.ShortInlineBrTarget or OperandType.ShortInlineI or OperandType.ShortInlineVar => 1,
                    OperandType.InlineVar => 2,
                    OperandType.InlineBrTarget or OperandType.InlineField or OperandType.InlineI or
                        OperandType.InlineMethod or OperandType.InlineSig or OperandType.InlineString or
                        OperandType.InlineTok or OperandType.InlineType or OperandType.ShortInlineR => 4,
                    OperandType.InlineI8 or OperandType.InlineR => 8,
                    OperandType.InlineSwitch => 4 + BitConverter.ToInt32(bytes, position) * 4,
                    _ => throw new NotSupportedException($"Unsupported IL operand: {opCode.OperandType}")
                };
            }
        }
        return used;
    }
}