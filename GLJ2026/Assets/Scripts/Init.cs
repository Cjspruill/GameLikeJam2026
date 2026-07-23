using UnityEngine;

public class Init : MonoBehaviour
{
    /// <summary>
    /// Run before scene loads
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#pragma warning disable IDE0051
    private static void Load() => Loot.Load();
}
