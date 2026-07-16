using UnityEngine;
using DotNetEnv;

public class DotEnv : MonoBehaviour
{
    /// <summary>
    /// Load Environment from .env file
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void LoadEnv()
    {
        Env.Load();

        Globals.Load();
    }
}
