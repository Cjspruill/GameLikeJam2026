using System.Collections.Generic;

/// <summary>
/// Inventory item
/// </summary>
public class InventoryItem
{
    public string Name { get; set; }
    public int Quantity { get; set; }
}

public static class InventoryItems
{
    /// <summary>
    /// Inventory items
    /// </summary>
    public static List<InventoryItem> Inventory { get; } = new();
}
