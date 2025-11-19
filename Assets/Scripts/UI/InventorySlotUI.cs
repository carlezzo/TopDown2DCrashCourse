using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI Components")]
    public Image itemIcon;
    public TextMeshProUGUI quantityText;
    public Image slotBackground;

    private Item currentItem;
    private int currentQuantity;

    void Awake()
    {
        if (itemIcon == null)
            itemIcon = GetComponentInChildren<Image>();

        if (quantityText == null)
            quantityText = GetComponentInChildren<TextMeshProUGUI>();

        if (slotBackground == null)
            slotBackground = GetComponent<Image>();
    }

    public void SetItem(Item item, int quantity)
    {
        currentItem = item;
        currentQuantity = quantity;

        if (item != null)
        {
            if (itemIcon != null)
            {
                itemIcon.sprite = item.icon;
                itemIcon.enabled = true;
            }

            if (quantityText != null)
            {
                if (quantity > 1)
                {
                    quantityText.text = quantity.ToString();
                    quantityText.enabled = true;
                    quantityText.color = Color.red;
                }
                else
                {
                    quantityText.enabled = false;
                }
            }

            gameObject.SetActive(true);
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        currentQuantity = 0;

        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }

        if (quantityText != null)
        {
            quantityText.text = "";
            quantityText.enabled = false;
        }

        gameObject.SetActive(false);
    }

    public Item GetItem()
    {
        return currentItem;
    }

    public int GetQuantity()
    {
        return currentQuantity;
    }

    public bool IsEmpty()
    {
        return currentItem == null;
    }
}