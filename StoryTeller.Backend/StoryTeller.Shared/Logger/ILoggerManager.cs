namespace StoryTeller.StoryTeller.Backend.StoryTeller.Shared.Logger
{
    public interface ILoggerManager
    {
        void LogInfo(String message);
        void LogWarn(String message);
        void LogDebug(String messgae);
        void LogError(String message);
    }
}
