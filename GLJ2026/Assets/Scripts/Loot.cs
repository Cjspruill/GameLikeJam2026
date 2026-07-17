using System.Collections.Generic;
using System.Linq;

using Humanizer;

using UnityEngine;

using Resources = UnityEngine.Resources; // conflicts with Humanizer

using static Logger;

public static class Loot
{
    // * NOTE: Instantiating Lists so that Count won't error
    private static List<GameObject> loot = new();
    private static List<GameObject> scraps = new();

    /// <summary>
    /// Load GameObjects from Resources directory
    /// </summary>
    public static void Load()
    {
        // ! TODO: use correct prefabs

        loot = Resources.LoadAll<GameObject>("Loot").ToList();
        loot.RemoveAll(l => l.name.StartsWith("Debug")); // ! TODO: where do these come from?

        scraps = Resources.LoadAll<GameObject>("Scraps").ToList();
        scraps.RemoveAll(l => l.name.StartsWith("Debug")); // ! TODO: where do these come from?

        if (Globals.DEBUG)
        {
            LogInfo($"Loaded {"loot item".ToQuantity(loot.Count)} and {"scrap".ToQuantity(scraps.Count)}");
        }
    }

    /// <summary>
    /// Get random GameObject
    /// </summary>
    /// <param name="isLoot">Should get Loot or scrap?</param>
    /// <returns>Random GameObject</returns>
    public static GameObject GetLoot(bool isLoot) => isLoot ? loot[Random.Range(0, loot.Count)] : scraps[Random.Range(0, scraps.Count)]; // 0-(Count-1)
}
