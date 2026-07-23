using System.Linq;

using UnityEngine;

using static Logger;

public class Init : MonoBehaviour
{
    private const string BLOCK_OBJECT = "BlockObject";

    /// <summary>
    /// Run before scene loads
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#pragma warning disable IDE0051
    private static void Load() => Loot.Load();

#pragma warning disable IDE0051
    private void Awake() => DisableBlockObject();

    /// <summary>
    /// Hide box in hand
    /// </summary>
    private void DisableBlockObject()
    {
        GameObject obj = GameObject.FindWithTag(BLOCK_OBJECT);

        if (!obj)
        {
            LogError($"Could not find {BLOCK_OBJECT} tag");
            return;
        }

        obj.SetActive(false);
    }
}
