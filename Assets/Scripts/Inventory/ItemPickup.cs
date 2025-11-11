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
            return;
        }

        if (InventoryManager.Instance == null)
        {
            return;
        }

        bool success = InventoryManager.Instance.AddItem(item, quantity);

        if (success)
        {

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
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        print("OnTriggerEnter2D called with " + other.name);
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