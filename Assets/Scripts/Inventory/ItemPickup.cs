using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Item Pickup Settings")]
    public Item item;
    public int quantity = 1;
    public float pickupRange = 1.5f;
    public PickupMode pickupMode = PickupMode.Trigger;
    
    [Header("Visual Effects")]
    public bool destroyOnPickup = true;
    public float bobSpeed = 2f;
    public float bobHeight = 0.5f;
    public bool enableBobbing = true;
    
    [Header("Audio")]
    public AudioClip pickupSound;
    
    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    
    void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        
        if (item != null && spriteRenderer != null && item.icon != null)
        {
            spriteRenderer.sprite = item.icon;
        }
        
        if (item == null)
        {
            Debug.LogWarning($"ItemPickup on {gameObject.name} has no item assigned!");
        }
    }
    
    void Update()
    {
        if (enableBobbing)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        }
        
        if (pickupMode == PickupMode.Proximity)
        {
            CheckForPlayer();
        }
    }
    
    void CheckForPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance <= pickupRange)
            {
                TryPickup();
            }
        }
    }
    
    public void TryPickup()
    {
        if (item == null)
        {
            Debug.LogWarning("Cannot pickup item: no item assigned!");
            return;
        }
        
        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("Cannot pickup item: InventoryManager not found!");
            return;
        }
        
        bool success = InventoryManager.Instance.AddItem(item, quantity);
        
        if (success)
        {
            Debug.Log($"Picked up {quantity} {item.itemName}(s)!");
            
            if (pickupSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }
            
            if (destroyOnPickup)
            {
                if (pickupSound != null && audioSource != null)
                {
                    audioSource.transform.SetParent(null);
                    Destroy(audioSource.gameObject, pickupSound.length);
                    Destroy(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.Log($"Could not pickup {item.itemName}: inventory full or item not stackable!");
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (pickupMode == PickupMode.Trigger && other.CompareTag("Player"))
        {
            TryPickup();
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (pickupMode == PickupMode.Proximity)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, pickupRange);
        }
    }
}

public enum PickupMode
{
    Trigger,    // Coleta por trigger (precisa de Collider2D)
    Proximity   // Coleta por proximidade (usa pickupRange)
}