using System.Collections.Generic;

/// <summary>
/// Inventory item
/// </summary>
public class InventoryItem
{
    /// <summary>
    /// Name of inventory item
    /// </summary>
    public string Name { get; }

    public InventoryItem(string name)
    {
        Name = name;
    }
}

public static class InventoryItems
{
    /// <summary>
    /// Inventory items
    /// </summary>
    public static List<InventoryItem> Inventory { get; } = new();
}
