using UnityEngine;
using UnityEngine.UI;

public class OpenInventoryButtonUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventoryUI inventoryUI;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        inventoryUI ??= FindFirstObjectByType<InventoryUI>();

        if (button == null)
            Debug.LogError("Button component não encontrado em OpenInventoryButtonUI!");

        if (inventoryUI == null)
            Debug.LogError("InventoryUI não encontrado! Adicione via Inspector ou garanta que existe na cena.");
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