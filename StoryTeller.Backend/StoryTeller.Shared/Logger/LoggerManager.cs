namespace StoryTeller.StoryTeller.Backend.StoryTeller.Shared.Logger
{
    public class LoggerManager:ILoggerManager
    {
        private readonly ILogger<LoggerManager> _logger;
        public LoggerManager(ILogger<LoggerManager> logger)
        {
            _logger = logger;
        }

        public void LogInfo(String message) => _logger.LogInformation(message);
        public void LogWarn(String message) => _logger.LogWarning(message);
        public void LogDebug(String message) => _logger.LogDebug(message);
        public void LogError(String message) => _logger.LogError(message);
    }
}
