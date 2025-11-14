using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("Movement Settings")]
    public Rigidbody2D rb;
    public Vector2 direction = Vector2.right;
    public float lifeSpawn = 2;
    public float speed = 1;

    [Header("Damage Settings")]
    public int damage = 1;                              // Dano que a flecha causa
    public ElevationState shooterElevationState;        // Estado de elevação de quem atirou (Archer sempre atira cross-layer)

    [Header("Components")]
    [SerializeField] private Collider2D arrowCollider;

    void Awake()
    {
        // Usa null-coalescing para obter componente automaticamente se não atribuído
        rb ??= GetComponent<Rigidbody2D>();
        arrowCollider ??= GetComponent<Collider2D>();

        // Validações
        if (rb == null)
            Debug.LogError("[Arrow] Rigidbody2D not found! Add Rigidbody2D component.");

        if (arrowCollider == null)
            Debug.LogError("[Arrow] Collider2D not found! Add Collider2D component with IsTrigger = true.");
    }

    void Start()
    {
        // Define a velocidade da flecha
        if (rb != null)
            rb.linearVelocity = direction * speed;

        // Auto-destruição após tempo de vida
        Destroy(gameObject, lifeSpawn);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ========================================
        // PRIORIDADE 1: COLISÃO COM PLAYER
        // ========================================
        if (other.CompareTag("Player"))
        {
            // Archer sempre atira cross-layer (da torre para o chão)
            // Então a flecha SEMPRE atinge o player, independente da elevação

            // Aplica dano ao player
            DamagePlayer(other);

            // Destroi a flecha após atingir o player
            Destroy(gameObject);
            return;
        }

        // ========================================
        // PRIORIDADE 2: COLISÃO COM OBSTÁCULOS
        // ========================================

        // Só processa colliders sólidos (não-trigger)
        if (!other.isTrigger)
        {
            // Obter o layer de colisão do atirador (baseado na elevação)
            if (shooterElevationState == null)
            {
                Debug.LogWarning("[Arrow] shooterElevationState is null! Arrow will not check obstacle collision properly.");
                return;
            }

            int shooterCollisionLayer = shooterElevationState.GetCollisionLayer();

            // MODO CROSS-LAYER ATTACK (Archer sempre atira assim)
            // Regra: Ignora obstáculos do PRÓPRIO nível de elevação, para em outros níveis

            if (other.gameObject.layer == shooterCollisionLayer)
            {
                // Mesmo nível de elevação que o atirador - PASSA ATRAVÉS
                // Exemplo: Archer Level1 → Flecha ignora Collision_Level1
                Debug.Log($"[Arrow] Passing through obstacle on same elevation level ({LayerMask.LayerToName(shooterCollisionLayer)})");
                return;
            }
            else
            {
                // Nível de elevação diferente - PARA A FLECHA
                // Exemplo: Archer Level1 → Flecha para em Collision_Level0
                Debug.Log($"[Arrow] Hit obstacle on different elevation level (layer: {LayerMask.LayerToName(other.gameObject.layer)})");
                Destroy(gameObject);
                return;
            }
        }

        // Colliders tipo trigger são ignorados (portas, zonas, etc)
    }

    /// <summary>
    /// Aplica dano ao player usando HealthComponent ou fallback para PlayerController
    /// </summary>
    private void DamagePlayer(Collider2D playerCollider)
    {
        // Tenta aplicar dano via HealthComponent (preferencial)
        HealthComponent playerHealth = playerCollider.GetComponent<HealthComponent>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            Debug.Log($"[Arrow] Hit player for {damage} damage!");
            return;
        }

        // Fallback: tenta usar PlayerController diretamente
        PlayerController player = playerCollider.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Debug.Log($"[Arrow] Hit player (via PlayerController) for {damage} damage!");
        }
    }
}
