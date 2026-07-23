using UnityEngine;

using DotNetEnv;

internal class DotEnv : MonoBehaviour
{
    /// <summary>
    /// Load Environment variables
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#pragma warning disable IDE0051
    private static void LoadEnv()
    {
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);

        Env.Load();

        Globals.Load();
    }
}
