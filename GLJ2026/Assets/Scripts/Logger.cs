using UnityEngine;

public static class Logger
{
    /// <summary>
    /// Log levels
    /// </summary>
    public enum LogLevel
    {
        Verbose,
        Info,
        None
    }

    /// <summary>
    /// Log verbose message to Unity console
    /// </summary>
    /// <param name="message">The verbose message to log</param>
    public static void LogVerbose(string message)
    {
        if (Globals.LOG_LEVEL == LogLevel.Verbose)
        {
            Debug.Log($"<color=cyan>{message}</color>");
        }
    }

    /// <summary>
    /// Log informational message to Unity console
    /// </summary>
    /// <param name="message">The informational message to log</param>
    public static void LogInfo(string message)
    {
        if (Globals.LOG_LEVEL <= LogLevel.Info)
        {
            Debug.Log($"<color=green>{message}</color>");
        }
    }
}
