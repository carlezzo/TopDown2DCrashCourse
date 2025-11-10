using System.Collections.Generic;

[System.Serializable]
public class InventoryData
{
    public List<ItemData> items = new List<ItemData>();
    
    public InventoryData()
    {
        items = new List<ItemData>();
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