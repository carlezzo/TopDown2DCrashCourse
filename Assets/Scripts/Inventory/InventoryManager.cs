using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Inventory Settings")]
    public int maxInventorySize = 20;

    [Header("Debug")]
    public bool loadMockDataOnStart = false;

    [Header("Events")]
    public UnityEvent<Item, int> OnItemAdded;
    public UnityEvent<Item, int> OnItemRemoved;
    public UnityEvent OnInventoryChanged;

    private Dictionary<Item, int> inventory = new Dictionary<Item, int>();
    private string savePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            savePath = Path.Combine(Application.persistentDataPath, "inventory.json");
            LoadInventory();

            if (loadMockDataOnStart)
            {
                LoadMockData();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool AddItem(Item item, int quantity = 1)
    {
        if (item == null || quantity <= 0)
            return false;

        if (!item.isStackable && inventory.ContainsKey(item))
        {
            return false;
        }


        print($"Adding {quantity} x {item.name} to inventory.");

        if (inventory.ContainsKey(item))
        {
            int newQuantity = inventory[item] + quantity;
            if (newQuantity > item.maxStackSize)
            {
                int addedAmount = item.maxStackSize - inventory[item];
                inventory[item] = item.maxStackSize;

                if (addedAmount > 0)
                {
                    OnItemAdded?.Invoke(item, addedAmount);
                    OnInventoryChanged?.Invoke();
                    SaveInventory();
                }
                return addedAmount > 0;
            }
            else
            {
                inventory[item] = newQuantity;
            }
        }
        else
        {
            if (GetInventoryItemCount() >= maxInventorySize)
            {
                return false;
            }

            inventory[item] = Mathf.Min(quantity, item.maxStackSize);
        }

        OnItemAdded?.Invoke(item, quantity);
        OnInventoryChanged?.Invoke();
        SaveInventory();

        return true;
    }

    public bool RemoveItem(Item item, int quantity = 1)
    {
        if (item == null || quantity <= 0 || !inventory.ContainsKey(item))
            return false;

        if (inventory[item] <= quantity)
        {
            int removedAmount = inventory[item];
            inventory.Remove(item);
            OnItemRemoved?.Invoke(item, removedAmount);
            OnInventoryChanged?.Invoke();
            SaveInventory();
            return true;
        }
        else
        {
            inventory[item] -= quantity;
            OnItemRemoved?.Invoke(item, quantity);
            OnInventoryChanged?.Invoke();
            SaveInventory();
            return true;
        }
    }

    public int GetItemQuantity(Item item)
    {
        return inventory.ContainsKey(item) ? inventory[item] : 0;
    }

    public bool HasItem(Item item, int quantity = 1)
    {
        return GetItemQuantity(item) >= quantity;
    }

    public Dictionary<Item, int> GetInventory()
    {
        return new Dictionary<Item, int>(inventory);
    }

    public int GetInventoryItemCount()
    {
        return inventory.Count;
    }

    public bool IsInventoryFull()
    {
        return GetInventoryItemCount() >= maxInventorySize;
    }

    public void ClearInventory()
    {
        inventory.Clear();
        OnInventoryChanged?.Invoke();
        SaveInventory();
    }

    private void SaveInventory()
    {
        try
        {
            InventoryData data = new InventoryData();

            foreach (var kvp in inventory)
            {
                data.items.Add(new ItemData(kvp.Key.name, kvp.Value));
            }

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, json);

        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save inventory: {e.Message}");
        }
    }

    private void LoadInventory()
    {
        try
        {
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                InventoryData data = JsonUtility.FromJson<InventoryData>(json);

                inventory.Clear();

                foreach (var itemData in data.items)
                {
                    Item item = Resources.Load<Item>("Items/" + itemData.itemName);
                    if (item != null)
                    {
                        inventory[item] = itemData.quantity;
                    }
                    else
                    {
                        Debug.LogWarning($"Could not load item: {itemData.itemName}");
                    }
                }

                OnInventoryChanged?.Invoke();
            }
            else
            {
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load inventory: {e.Message}");
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            SaveInventory();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            SaveInventory();
    }

    void OnDestroy()
    {
        SaveInventory();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            LoadMockData();
        }
    }

    [ContextMenu("Load Mock Data")]
    public void LoadMockData()
    {
        var mockData = InventoryData.CreateMockData();

        print("Loading mock data into inventory...");

        foreach (var itemData in mockData.items)
        {
            print(itemData.itemName + " x" + itemData.quantity);
            Item item = Resources.Load<Item>("Items/" + itemData.itemName);
            if (item != null)
            {
                AddItem(item, itemData.quantity);
            }
            else
            {
                Debug.LogWarning($"Mock item not found: {itemData.itemName}. Create ScriptableObject in Assets/Resources/Items/");
            }
        }

    }
}