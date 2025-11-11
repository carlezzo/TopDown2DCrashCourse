using UnityEngine;
using UnityEngine.UI;

public class CloseInventoryButtonUI : MonoBehaviour
{
    [Header("References")]
    public InventoryUI inventoryUI;
    
    private Button button;
    
    void Awake()
    {
        button = GetComponent<Button>();
        
        if (button == null)
        {
            Debug.LogError("CloseInventoryButtonUI: No Button component found!");
            return;
        }
        
        if (inventoryUI == null)
        {
            inventoryUI = FindFirstObjectByType<InventoryUI>();
            
            if (inventoryUI == null)
            {
                Debug.LogError("CloseInventoryButtonUI: InventoryUI not found! Please assign it in the inspector.");
                return;
            }
        }
    }
    
    void Start()
    {
        if (button != null && inventoryUI != null)
        {
            button.onClick.AddListener(CloseInventory);
        }
    }
    
    void CloseInventory()
    {
        if (!CanCloseInventory())
            return;
            
        if (inventoryUI != null)
        {
            inventoryUI.CloseInventory();
        }
    }
    
    private bool CanCloseInventory()
    {
        // Future validation rules can be added here
        // Examples:
        // - Check if player is in combat
        // - Check if items are being dragged
        // - Check for unsaved changes
        // - etc.
        
        return true;
    }
    
    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(CloseInventory);
        }
    }
}