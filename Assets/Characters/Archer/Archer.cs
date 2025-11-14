using UnityEngine;
using System.Collections.Generic;

public class Archer : MonoBehaviour
{
    [Header("Sistema de Detecção")]
    public float detectionRadius = 5f; // Raio para detectar o player
    public float attackRadius = 3f;  // Raio para atacar o player (ranged)
    public float moveSpeed = 0.05f;       // Velocidade de movimento

    [Header("Sistema de Ataque à Distância")]
    public GameObject arrowPrefab;          // Prefab da flecha
    public Transform arrowSpawnPoint;       // Ponto de spawn da flecha
    public float arrowSpeed = 5f;           // Velocidade da flecha
    public int arrowDamage = 1;             // Dano da flecha
    public float arrowLifespan = 3f;        // Tempo de vida da flecha
    public float attackCooldown = 2.5f;     // Tempo entre ataques

    [Header("Sistema de Colisão")]
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;

    // Estados do inimigo
    public enum ArcherState
    {
        Idle,       // Parado, sem detectar player
        Chasing,    // Perseguindo o player
        Attacking   // Atacando o player
    }

    [Header("Debug - Estado Atual")]
    public ArcherState currentState = ArcherState.Idle;

    // Referências
    private Animator animator;
    private Rigidbody2D rb;
    private Transform playerTransform;
    private HealthComponent healthComponent;
    private HealthBarController healthBarController;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    private ElevationState elevationState;
    private ElevationState playerElevationState;

    // Controle de ataque
    private float lastAttackTime;
    private bool canAttack = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        healthComponent = GetComponent<HealthComponent>();
        healthBarController = GetComponentInChildren<HealthBarController>();


        if (healthComponent != null)
        {
            healthComponent.OnDeath.AddListener(Defeated);
        }

        // Encontra o player pela tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerElevationState = player.GetComponent<ElevationState>();

            if (playerElevationState == null)
            {
                Debug.LogWarning("[Archer] Player não possui componente ElevationState! Sistema de elevação não funcionará corretamente.");
            }
        }

        // Obter componente de elevação do archer
        elevationState = GetComponent<ElevationState>();
        if (elevationState == null)
        {
            Debug.LogError("[Archer] ElevationState component not found! Add ElevationState component to Archer.");
        }
        else
        {
            // Subscrever ao evento de mudança de elevação
            elevationState.OnElevationChanged.AddListener(OnElevationChangedHandler);
        }

        // Verificações de referências críticas
        if (arrowPrefab == null)
        {
            Debug.LogError("[Archer] Arrow Prefab not assigned! Assign in Inspector.");
        }

        if (arrowSpawnPoint == null)
        {
            Debug.LogWarning("[Archer] Arrow Spawn Point not assigned! Using archer position as fallback.");
            arrowSpawnPoint = transform;
        }

        // Configurar filtro de movimento baseado na elevação atual
        UpdateMovementFilter();
    }

    private void FixedUpdate()
    {
        // Só atualiza se não estiver derrotado e player existir
        if (healthComponent.GetCurrentHealth() <= 0 || playerTransform == null) return;

        UpdateArcherBehavior();
    }

    // Atualiza o comportamento baseado no estado atual
    void UpdateArcherBehavior()
    {
        switch (currentState)
        {
            case ArcherState.Idle:
                // Verifica se player entrou na área de detecção
                if (CanDetectPlayer())
                {
                    ChangeState(ArcherState.Chasing);
                }
                break;

            case ArcherState.Chasing:
                // Se player saiu da área de detecção, volta para Idle
                if (!CanDetectPlayer())
                {
                    ChangeState(ArcherState.Idle);
                }
                // Se chegou na distância de ataque, para e ataca
                else if (CanAttackPlayer())
                {
                    ChangeState(ArcherState.Attacking);
                }
                else
                {
                    // Move em direção ao player
                    MoveTowardsPlayer();
                }
                break;

            case ArcherState.Attacking:
                // Verifica se ainda pode atacar
                if (CanAttackPlayer())
                {
                    AttackPlayer();
                }
                else
                {
                    // Se player se afastou, volta a perseguir
                    ChangeState(ArcherState.Chasing);
                }
                break;
        }
    }

    /// <summary>
    /// Verifica se o player pode ser detectado baseado em:
    /// 1. Player existe
    /// 2. Distância está dentro do raio de detecção
    /// NOTA: Archer sempre pode detectar o player independente do nível de elevação (cross-layer detection).
    /// </summary>
    private bool CanDetectPlayer()
    {
        if (playerTransform == null || playerElevationState == null || elevationState == null)
        {
            return false;
        }

        // Archer sempre detecta o player, independente da elevação (cross-layer detection)
        // Isso permite que archer na torre atire no player no chão

        // Verificar distância
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        return distanceToPlayer <= detectionRadius;
    }

    /// <summary>
    /// Verifica se o player está dentro do alcance de ataque (ranged)
    /// NOTA: Archer sempre pode atacar o player independente do nível de elevação (cross-layer attack).
    /// </summary>
    private bool CanAttackPlayer()
    {
        if (playerTransform == null || playerElevationState == null || elevationState == null || !canAttack)
        {
            return false;
        }

        // Archer sempre pode atacar, independente da elevação (cross-layer attack)
        // Isso permite que archer na torre atire no player no chão

        // Verificar distância
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        return distanceToPlayer <= attackRadius;
    }

    // Muda o estado do archer
    void ChangeState(ArcherState newState)
    {
        currentState = newState;

        // Atualiza animações baseado no estado
        switch (currentState)
        {
            case ArcherState.Idle:
                animator.SetBool("isMoving", false);
                if (healthBarController != null)
                    healthBarController.Hide();
                break;

            case ArcherState.Chasing:
                animator.SetBool("isMoving", true);
                if (healthBarController != null)
                    healthBarController.Show();
                break;

            case ArcherState.Attacking:
                animator.SetBool("isMoving", false); // Para de se mover ao atacar
                if (healthBarController != null)
                    healthBarController.Show();
                break;
        }
    }

    // Move o archer em direção ao player
    void MoveTowardsPlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        TryMove(direction);
    }

    // Tenta mover o archer na direção especificada, verificando colisões
    private bool TryMove(Vector2 direction)
    {
        if (direction != Vector2.zero)
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

    // Ataca o player atirando uma flecha
    void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown && canAttack)
        {
            lastAttackTime = Time.time;

            // Verifica se tem prefab de flecha configurado
            if (arrowPrefab == null)
            {
                Debug.LogError("[Archer] Cannot shoot arrow - arrowPrefab is null!");
                return;
            }

            // Atira a flecha
            ShootArrow();

            // Inicia cooldown de ataque
            StartCoroutine(AttackCooldownCoroutine());
        }
    }

    /// <summary>
    /// Dispara uma flecha na direção do player
    /// </summary>
    void ShootArrow()
    {
        // Calcula direção para o player
        Vector2 direction = (playerTransform.position - arrowSpawnPoint.position).normalized;

        // Calcula ângulo de rotação para a flecha
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Instancia a flecha com rotação correta
        GameObject arrowObject = Instantiate(
            arrowPrefab,
            arrowSpawnPoint.position,
            Quaternion.Euler(0, 0, angle)
        );

        // Configura as propriedades da flecha
        Arrow arrowScript = arrowObject.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            arrowScript.direction = direction;
            arrowScript.speed = arrowSpeed;
            arrowScript.lifeSpawn = arrowLifespan;
            arrowScript.damage = arrowDamage;
            arrowScript.shooterElevationState = elevationState; // Passa o estado de elevação do archer
        }
        else
        {
            Debug.LogError("[Archer] Arrow prefab does not have Arrow.cs component!");
        }

        Debug.Log($"[Archer] Shot arrow towards player at angle {angle}°");
    }

    // Cooldown do ataque
    System.Collections.IEnumerator AttackCooldownCoroutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void TakeDamage(int damage)
    {
        if (healthComponent != null)
        {
            healthComponent.TakeDamage(damage);
        }
    }

    void Defeated()
    {
        // Para o movimento e muda para estado derrotado
        currentState = ArcherState.Idle;
        animator.SetTrigger("defeated");

        // Esconde a health bar quando derrotado
        if (healthBarController != null)
            healthBarController.Hide();

        // Desabilita o collider para que não continue detectando/atacando
        GetComponent<Collider2D>().enabled = false;
    }

    public void RemoveArcher()
    {
        Destroy(gameObject);
    }

    // Visualiza as áreas de detecção no editor (apenas para debug)
    private void OnDrawGizmosSelected()
    {
        // Área de detecção (amarelo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Área de ataque (vermelho)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        // Ponto de spawn da flecha (verde)
        if (arrowSpawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(arrowSpawnPoint.position, 0.1f);
        }
    }

    /// <summary>
    /// Atualiza o filtro de movimento para colidir apenas com layers apropriados
    /// baseado na elevação atual do archer (ElevationState).
    /// </summary>
    void UpdateMovementFilter()
    {
        if (elevationState == null)
        {
            Debug.LogWarning("[Archer] Cannot update movement filter - ElevationState is null!");
            return;
        }

        // Configurar filtro para usar layer mask
        movementFilter.useLayerMask = true;

        // Obter o layer de colisão apropriado baseado na elevação atual
        int collisionLayer = elevationState.GetCollisionLayer();

        // Criar layer mask que inclui APENAS o layer de colisão da elevação atual
        // Usa bit shift (1 << layer) para criar uma máscara com apenas esse layer ativo
        LayerMask mask = 1 << collisionLayer;

        movementFilter.SetLayerMask(mask);

        Debug.Log($"[Archer] Movement filter atualizado para elevação: {elevationState.CurrentElevation} (Collision Layer: {LayerMask.LayerToName(collisionLayer)}, Mask: {mask.value})");
    }

    /// <summary>
    /// Handler para o evento OnElevationChanged do ElevationState.
    /// Atualiza automaticamente o filtro de colisão quando a elevação muda.
    /// </summary>
    void OnElevationChangedHandler(ElevationState.ElevationLevel previousLevel, ElevationState.ElevationLevel newLevel)
    {
        Debug.Log($"[Archer] Elevação mudou de {previousLevel} para {newLevel}");
        UpdateMovementFilter();
    }

    /// <summary>
    /// Método público para atualizar o filtro quando o archer mudar de elevação.
    /// DEPRECATED: Use o evento OnElevationChanged do ElevationState ao invés deste método.
    /// Mantido para compatibilidade com código legado.
    /// </summary>
    public void OnElevationChanged()
    {
        UpdateMovementFilter();
    }
}
