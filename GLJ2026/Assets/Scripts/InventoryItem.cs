using UnityEngine;

public static class InventoryItem
{
    /// <summary>
    /// Inventory item
    /// </summary>
    public static GameObject Inventory { get; private set; }

    /// <summary>
    /// Set Inventory item
    /// </summary>
    /// <param name="item">The GameObject</param>
    public static void SetInventory(GameObject item) => Inventory = item;

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

}
