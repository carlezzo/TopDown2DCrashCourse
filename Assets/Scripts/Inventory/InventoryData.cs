using System.Collections.Generic;

[System.Serializable]
public class InventoryData
{
    public List<ItemData> items = new List<ItemData>();

    public InventoryData()
    {
        items = new List<ItemData>();
    }

    public static InventoryData CreateMockData()
    {
        var mockData = new InventoryData();
        mockData.items.Add(new ItemData("Coin", 50));
        mockData.items.Add(new ItemData("Sword", 12));
        // mockData.items.Add(new ItemData("HealthPotion", 5));
        // mockData.items.Add(new ItemData("Sword", 1));
        // mockData.items.Add(new ItemData("Wood", 10));
        return mockData;
    }
}

[System.Serializable]
public class ItemData
{
    public string itemName;
    public int quantity;

    public ItemData(string name, int qty)
    {
        itemName = name;
        quantity = qty;
    }
}