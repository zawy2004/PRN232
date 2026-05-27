using DemoValidationRouting.Interface;

namespace DemoValidationRouting.Logger
{
    public class LoggerManager : ILoggerManager
    {
        public void LogDebug(string message) => Console.WriteLine($"DEBUG: {message}");
        public void LogError(string message) => Console.WriteLine($"ERROR: {message}");
        public void LogInfo(string message) => Console.WriteLine($"INFO: {message}");
        public void LogWarn(string message) => Console.WriteLine($"WARN: {message}");
    }
}
