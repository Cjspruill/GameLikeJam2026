using Humanizer;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Resources = UnityEngine.Resources; // conflicts with Humanizer

public static class Loot
{
    private static List<GameObject> loot;

    static Loot()
    {
        LoadLoot();
    }

    /// <summary>
    /// Load all GameObjects from Resources directory
    /// </summary>
    private static void LoadLoot()
    {
        loot = Resources.LoadAll<GameObject>(string.Empty).ToList();

        if (Globals.DEBUG)
        {
            Debug.Log($"Loaded {"loot item".ToQuantity(loot.Count)}");
        }
    }

    /// <summary>
    /// Get random GameObject
    /// </summary>
    /// <returns>Random GameObject</returns>
    public static GameObject GetLoot()
    {
        return loot[Random.Range(0, loot.Count)];
    }
}
