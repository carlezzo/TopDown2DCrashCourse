using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject inventoryPanel;
    public Transform itemsContainer;
    public GameObject inventorySlotPrefab;


    private List<InventorySlotUI> inventorySlots = new List<InventorySlotUI>();
    public bool isInventoryOpen = false;


    void Start()
    {
        if (inventoryPanel == null)
            inventoryPanel = gameObject;


        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged.AddListener(UpdateInventoryUI);
        }

        CreateInventorySlots();
        UpdateInventoryUI();
        
        isInventoryOpen = false;
        inventoryPanel.SetActive(false);
    }


    public void OpenInventory()
    {
        isInventoryOpen = true;
        inventoryPanel.SetActive(true);
        UpdateInventoryUI();
    }

    public void CloseInventory()
    {
        isInventoryOpen = false;
        inventoryPanel.SetActive(false);
    }

    // public void ToggleInventory()
    // {
    //     if (isInventoryOpen)
    //     {
    //         CloseInventory();
    //     }
    //     else
    //     {
    //         OpenInventory();
    //     }
    // }

    void CreateInventorySlots()
    {
        // if (inventorySlotPrefab == null || itemsContainer == null)
        //     return;

        for (int i = 0; i < InventoryManager.Instance.GetInventoryItemCount(); i++)
        {
            GameObject slotObj = Instantiate(inventorySlotPrefab, itemsContainer);
            InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();

            if (slotUI != null)
            {
                inventorySlots.Add(slotUI);
            }
        }
    }

    void UpdateInventoryUI()
    {
        if (InventoryManager.Instance == null) return;

        var inventory = InventoryManager.Instance.GetInventory();

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            inventorySlots[i].ClearSlot();
        }

        int slotIndex = 0;
        foreach (var kvp in inventory)
        {
            if (slotIndex < inventorySlots.Count)
            {
                inventorySlots[slotIndex].SetItem(kvp.Key, kvp.Value);
                slotIndex++;
            }
        }
    }

    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged.RemoveListener(UpdateInventoryUI);
        }
    }

}