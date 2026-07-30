using System.Collections.Generic;

using UnityEngine;

using Humanizer;

using static Logger;

public static class InventoryItem
{
    /// <summary>
    /// Inventory item
    /// </summary>
    public static List<GameObject> Inventory { get; private set; } = new();

    /// <summary>
    /// Set Inventory item
    /// </summary>
    /// <param name="item">The GameObject</param>
    public static void AddInventory(GameObject item) => Inventory.Add(item);

    /// <summary>
    /// Does Inventory contain item?
    /// </summary>
    /// <returns>
    /// True or false
    /// </returns>
    public static bool HasInventory() => Inventory != null;

    /// <summary>
    /// Clear Inventory item
    /// </summary>
    public static void ClearInventory() => Inventory = null;

    /// <summary>
    /// Remove item from Inventory by name
    /// </summary>
    /// <param name="item">The GameObject name</param>
    public static void RemoveInventory(string name) => Inventory.RemoveAll(item => item.name == name);

    /// <summary>
    /// Writes Inventory count to debug console
    /// </summary>
    public static void ShowInventoryCount() => LogInfo($"Inventory: {"item".ToQuantity(Inventory.Count)}");
}
