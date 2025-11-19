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

    void Awake()
    {
        if (inventoryPanel == null)
            inventoryPanel = gameObject;
    }

    void OnEnable()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged.AddListener(UpdateInventoryUI);
        }
    }

    void OnDisable()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged.RemoveListener(UpdateInventoryUI);
        }
    }

    void Start()
    {
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

        GameManager.Instance?.PauseGame();
    }

    public void CloseInventory()
    {
        isInventoryOpen = false;
        inventoryPanel.SetActive(false);

        GameManager.Instance?.ResumeGame();
    }

    void CreateInventorySlots()
    {

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

        // Criar slots dinamicamente se necessário
        while (inventorySlots.Count < inventory.Count)
        {
            GameObject slotObj = Instantiate(inventorySlotPrefab, itemsContainer);
            InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();

            if (slotUI != null)
            {
                inventorySlots.Add(slotUI);
            }
        }

        // Limpar todos os slots
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            inventorySlots[i].ClearSlot();
        }

        // Popular slots com itens
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

}