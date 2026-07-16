using Humanizer;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Resources = UnityEngine.Resources; // conflicts with Humanizer

public static class Loot
{
    private static List<GameObject> loot;

    /// <summary>
    /// Load all GameObjects from Resources directory
    /// </summary>
    public static void Load()
    {
        loot = Resources.LoadAll<GameObject>(string.Empty).ToList();

        loot.RemoveAll(l => l.name.StartsWith("Debug")); // ! TODO: where do these come from?

        if (Globals.DEBUG)
        {
            Debug.Log($"<color=cyan>Loaded {"loot item".ToQuantity(loot.Count)}</color>");
        }
    }

    /// <summary>
    /// Get random GameObject
    /// </summary>
    /// <returns>Random GameObject</returns>
    public static GameObject GetLoot() => loot[Random.Range(0, loot.Count)];
}
