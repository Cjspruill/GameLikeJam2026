using UnityEngine;

public class Init : MonoBehaviour
{
    /// <summary>
    /// Run before scene loads
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Load()
    {
        Loot.Load();
    }
}
