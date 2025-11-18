using UnityEngine;
using System.Collections.Generic;

public class SheepController : MonoBehaviour
{
    [Header("Sistema de Detecção")]
    [Tooltip("Raio de detecção do player - ovelha foge quando player está nesta distância")]
    public float fleeRadius = 3f;

    [Header("Sistema de Movimento")]
    [Tooltip("Velocidade de fuga da ovelha")]
    public float moveSpeed = 2f;

    [Tooltip("Offset para detecção de colisão")]
    public float collisionOffset = 0.05f;

    [Tooltip("Filtro de colisão para movimento")]
    public ContactFilter2D movementFilter;

    // Estados da ovelha
    public enum SheepState
    {
        Idle,       // Parada, sem detectar player
        Fleeing     // Fugindo do player
    }

    [Header("Debug - Estado Atual")]
    public SheepState currentState = SheepState.Idle;

    // Referências
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Transform playerTransform;
    private ElevationState elevationState;
    private ElevationState playerElevationState;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    private void Start()
    {
        // Obter componentes necessários
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Encontrar o player pela tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerElevationState = player.GetComponent<ElevationState>();

            if (playerElevationState == null)
            {
                Debug.LogWarning("[SheepController] Player não possui componente ElevationState! Sistema de elevação não funcionará corretamente.");
            }
        }
        else
        {
            Debug.LogWarning("[SheepController] Player não encontrado! Certifique-se que o GameObject do player possui a tag 'Player'.");
        }

        // Obter componente de elevação da ovelha
        elevationState = GetComponent<ElevationState>();
        if (elevationState == null)
        {
            Debug.LogWarning("[SheepController] ElevationState component not found! Sistema de elevação não funcionará. Adicione ElevationState se necessário.");
        }
        else
        {
            // Subscrever ao evento de mudança de elevação
            elevationState.OnElevationChanged.AddListener(OnElevationChangedHandler);
        }

        // Configurar filtro de movimento baseado na elevação atual
        UpdateMovementFilter();
    }

    private void FixedUpdate()
    {
        // Só atualiza se player existir
        if (playerTransform == null) return;

        UpdateSheepBehavior();
    }

    /// <summary>
    /// Atualiza o comportamento da ovelha baseado no estado atual
    /// </summary>
    void UpdateSheepBehavior()
    {
        switch (currentState)
        {
            case SheepState.Idle:
                // Verifica se player entrou na área de detecção
                if (CanDetectPlayer())
                {
                    ChangeState(SheepState.Fleeing);
                }
                break;

            case SheepState.Fleeing:
                // Se player saiu da área de detecção, volta para Idle
                if (!CanDetectPlayer())
                {
                    ChangeState(SheepState.Idle);
                }
                else
                {
                    // Foge na direção oposta ao player
                    FleeFromPlayer();
                }
                break;
        }
    }

    /// <summary>
    /// Verifica se o player pode ser detectado baseado em:
    /// 1. Player existe
    /// 2. Sheep e player estão no mesmo nível de elevação (se ElevationState existir)
    /// 3. Distância está dentro do raio de detecção
    /// </summary>
    private bool CanDetectPlayer()
    {
        if (playerTransform == null)
        {
            return false;
        }

        // Verificar se estão no mesmo nível de elevação (apenas se ambos tiverem ElevationState)
        if (elevationState != null && playerElevationState != null)
        {
            if (!elevationState.IsOnSameLevel(playerElevationState))
            {
                // Diferentes níveis de elevação - não pode detectar
                return false;
            }
        }

        // Verificar distância
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        return distanceToPlayer <= fleeRadius;
    }

    /// <summary>
    /// Muda o estado da ovelha e atualiza animações
    /// </summary>
    void ChangeState(SheepState newState)
    {
        currentState = newState;

        // Atualiza animações baseado no estado
        if (animator != null)
        {
            switch (currentState)
            {
                case SheepState.Idle:
                    animator.SetBool("isMoving", false);
                    break;

                case SheepState.Fleeing:
                    animator.SetBool("isMoving", true);
                    break;
            }
        }
    }

    /// <summary>
    /// Move a ovelha na direção oposta ao player
    /// </summary>
    void FleeFromPlayer()
    {
        // Calcula direção oposta ao player
        Vector2 directionAwayFromPlayer = (transform.position - playerTransform.position).normalized;

        // Tenta mover na direção de fuga
        bool moved = TryMove(directionAwayFromPlayer);

        // Atualiza direção do sprite baseado no movimento
        if (moved && spriteRenderer != null)
        {
            if (directionAwayFromPlayer.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (directionAwayFromPlayer.x > 0)
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    /// <summary>
    /// Tenta mover a ovelha na direção especificada, verificando colisões
    /// </summary>
    private bool TryMove(Vector2 direction)
    {
        if (direction != Vector2.zero && rb != null)
        {
            // Verifica colisões potenciais
            int count = rb.Cast(
                direction,
                movementFilter,
                castCollisions,
                moveSpeed * Time.fixedDeltaTime + collisionOffset);

            if (count == 0)
            {
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Atualiza o filtro de movimento para colidir apenas com layers apropriados
    /// baseado na elevação atual da ovelha (ElevationState).
    /// </summary>
    void UpdateMovementFilter()
    {
        if (elevationState == null)
        {
            // Se não houver ElevationState, usa configuração padrão do Inspector
            return;
        }

        // Configurar filtro para usar layer mask
        movementFilter.useLayerMask = true;

        // Obter o layer de colisão apropriado baseado na elevação atual
        int collisionLayer = elevationState.GetCollisionLayer();

        // Criar layer mask que inclui APENAS o layer de colisão da elevação atual
        LayerMask mask = 1 << collisionLayer;

        movementFilter.SetLayerMask(mask);

        Debug.Log($"[SheepController] Movement filter atualizado para elevação: {elevationState.CurrentElevation} (Collision Layer: {LayerMask.LayerToName(collisionLayer)}, Mask: {mask.value})");
    }

    /// <summary>
    /// Handler para o evento OnElevationChanged do ElevationState.
    /// Atualiza automaticamente o filtro de colisão quando a elevação muda.
    /// </summary>
    void OnElevationChangedHandler(ElevationState.ElevationLevel previousLevel, ElevationState.ElevationLevel newLevel)
    {
        Debug.Log($"[SheepController] Elevação mudou de {previousLevel} para {newLevel}");
        UpdateMovementFilter();
    }

    /// <summary>
    /// Visualiza a área de detecção no editor Unity (apenas para debug)
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Área de detecção (verde claro)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, fleeRadius);
    }
}
