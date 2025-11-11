using UnityEngine;
using UnityEngine.UI;

public class InventoryButtonUI : MonoBehaviour
{
    [Header("References")]
    public InventoryUI inventoryUI;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError("InventoryButtonUI: No Button component found!");
            return;
        }

        if (inventoryUI == null)
        {
            inventoryUI = FindFirstObjectByType<InventoryUI>();

            if (inventoryUI == null)
            {
                Debug.LogError("InventoryButtonUI: InventoryUI not found! Please assign it in the inspector.");
                return;
            }
        }
    }

    void Start()
    {
        if (button != null && inventoryUI != null)
        {
            button.onClick.AddListener(OpenInventory);
        }
    }

    void OpenInventory()
    {
        if (inventoryUI != null)
        {
            inventoryUI.OpenInventory();
        }
    }

    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OpenInventory);
        }
    }
}