#pragma warning disable 693
#pragma warning disable CA1000
namespace Common
{
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

        /// <summary>
        /// Gets Success Result. The default value IsSuccess = true.
        /// </summary>
        /// <returns></returns>
        public static Result Ok() => new Result(true, null, LevelInfo.Silent);

        /// <summary>
        /// Gets Info Result. The default value IsSuccess = true.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Result Info(string message) => new Result(true, message, LevelInfo.Info);

        /// <summary>
        /// Gets Silent Result. The default value of IsError = true.
        /// </summary>
        /// <returns></returns>
        public static Result Silent() => new Result(false, null, LevelInfo.Silent);

        /// <summary>
        /// Gets Error Result. The default value of IsError = true.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Result Fail(string message) => new Result(false, message, LevelInfo.Error);

        public enum LevelInfo
        {
            Silent,
            Info,
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
        protected Result(bool success, string message, LevelInfo level, T data) : base(success, message, level)
        {
            Data = data;
        }

        /// <summary>
        /// Gets Data.
        /// </summary>
        public T Data { get; }

        /// <summary>
        /// Gets Success Result. The default value IsSuccess = true.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Result<T> Ok<T>(T data) => new Result<T>(true, null, LevelInfo.Silent, data);

        /// <summary>
        /// Gets Info Result. The default value IsSuccess = true.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Result<T> Info<T>(string message, T data) => new Result<T>(true, message, LevelInfo.Info, data);

        /// <summary>
        /// Gets Silent Result. The default value of IsError = true.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Result<T> Silent<T>(string message, T data) => new Result<T>(false, message, LevelInfo.Silent, data);

        /// <summary>
        /// Gets Error Result. The default value IsError = true.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Result<T> Fail<T>(string message) => new Result<T>(false, message, LevelInfo.Error, default);
    }
}
