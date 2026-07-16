using UnityEngine;

public class Init : MonoBehaviour
{
    /// <summary>
    /// Run before scene loads
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Load()
    {
        // only show stacktrace if error
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);

        Loot.Load();
    }
}
