using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Item Info")]
    public string itemName;
    [TextArea(3, 5)]
    public string description;
    public Sprite icon;
    
    [Header("Item Properties")]
    public ItemType itemType;
    public bool isStackable = true;
    public int maxStackSize = 99;
    
    [Header("Item Value")]
    public int value = 1;
    public ItemRarity rarity = ItemRarity.Common;
}

public enum ItemType
{
    Material,
    Consumable,
    Tool,
    Weapon,
    Armor,
    Quest
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}