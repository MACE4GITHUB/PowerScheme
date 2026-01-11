#pragma warning disable 693
#pragma warning disable CA1000
namespace RegistryManager;

/// <summary>
/// Represents an operation Result.
/// </summary>
public class Result
{
    /// <summary>
    /// Creates Result.
    /// </summary>
    /// <param name="success"></param>
    /// <param name="message"></param>
    /// <param name="level"></param>
    protected Result(bool success, string message, LevelInfo level)
    {
        IsSuccess = success;
        Message = message;
        Level = level;
    }

    /// <summary>
    /// Gets true if the operation is successful, otherwise false.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets true if the operation is erroneous, otherwise false.
    /// </summary>
    public bool IsError => !IsSuccess;

    /// <summary>
    /// Gets a message if the operation is fail. 
    /// </summary>
    public string Message { get; }

    public LevelInfo Level { get; }

    public enum LevelInfo
    {
        Silent,
        Error
    }
}

/// <summary>
/// Represents an operation Result.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Result<T> : Result 
{
    /// <summary>
    /// Creates Result.
    /// </summary>
    /// <param name="success"></param>
    /// <param name="message"></param>
    /// <param name="level"></param>
    /// <param name="data"></param>
    protected Result(bool success, string message, LevelInfo level, T? data) : 
        base(success, message, level)
    {
        Data = data;
    }

    /// <summary>
    /// Gets Data.
    /// </summary>
    public T? Data { get; }

    /// <summary>
    /// Gets Success Result. The default value IsSuccess = true.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public static Result<T> Ok<T>(T data) => 
        new(true, string.Empty, LevelInfo.Silent, data);

    /// <summary>
    /// Gets Error Result. The default value IsError = true.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <returns></returns>
    public static Result<T> Fail<T>(string message) => 
        new(false, message, LevelInfo.Error, default);
}
