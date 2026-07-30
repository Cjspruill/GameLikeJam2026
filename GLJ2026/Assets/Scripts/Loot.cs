using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Humanizer;

using static Globals;
using static Logger;

using Resources = UnityEngine.Resources; // conflicts with Humanizer

public static class Loot
{
    // * NOTE: Instantiating Lists so that Count won't error
    private static List<GameObject> LootItems { get; set; } = new();
    private static List<GameObject> ScrapItems { get; set; } = new();

    /// <summary>
    /// Load GameObjects from Resources directory
    /// </summary>
    public static void Load()
    {
        LootItems = Resources.LoadAll<GameObject>("Loot").ToList();

        ScrapItems = Resources.LoadAll<GameObject>("Scraps").ToList();

        if (DEBUG)
        {
            LogInfo($"Loaded {"Loot block".ToQuantity(LootItems.Count)} and {"Scrap block".ToQuantity(ScrapItems.Count)}");
        }
    }

    /// <summary>
    /// Get random GameObject
    /// </summary>
    /// <param name="isLoot">Should get Loot or scrap?</param>
    /// <returns>Random GameObject</returns>
    public static GameObject GetLoot(bool isLoot) => isLoot ? LootItems[Random.Range(0, LootItems.Count)] : ScrapItems[Random.Range(0, ScrapItems.Count)]; // 0-(Count-1)

    /// <summary>
    /// Find LootItem by name
    /// </summary>
    /// <param name="name">The object name</param>
    /// <returns>The found GameObject</returns>
    public static GameObject Find(string name) => LootItems.FirstOrDefault(item => item.name == name);
}
