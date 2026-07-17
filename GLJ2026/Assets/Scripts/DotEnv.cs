using UnityEngine;

using DotNetEnv;

public class DotEnv : MonoBehaviour
{
    /// <summary>
    /// Load Environment variables
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void LoadEnv()
    {
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);

        Env.Load();

        Globals.Load();
    }
}
