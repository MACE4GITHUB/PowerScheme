using RunAs.Common.Services;

namespace RunAs
{
    class Program
    {
        static void Main(string[] args)
        {
            var program = args[0];
            var arguments = args[1];

            var executorService = new ExecutorService()
            {
                Name = program,
                Arguments = arguments,
                UseExecutor = false,
                IsWait = false,
                IsRemoveFile = false
            };
            executorService.Execute();
        }
    }


}
