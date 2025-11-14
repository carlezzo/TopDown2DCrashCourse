using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{

    [Header("Sistema de Detecção")]
    public float detectionRadius = 0.50f; // Raio para detectar o player
    public float attackRadius = 1f;  // Raio para atacar o player
    public float moveSpeed = 0.05f;       // Velocidade de movimento

    [Header("Sistema de Ataque")]
    public int attackDamage = 1;    // Dano que o inimigo causa
    public float attackCooldown = 5.5f; // Tempo entre ataques

    [Header("Sistema de Colisão")]
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;

    // Estados do inimigo
    public enum EnemyState
    {
        Idle,       // Parado, sem detectar player
        Chasing,    // Perseguindo o player
        Attacking   // Atacando o player
    }

    [Header("Debug - Estado Atual")]
    public EnemyState currentState = EnemyState.Idle;


    // Referências
    private Animator animator;
    private Rigidbody2D rb;
    private Transform playerTransform;
    private HealthComponent healthComponent;
    private HealthBarController healthBarController;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    // Controle de ataque
    private float lastAttackTime;
    private bool canAttack = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        healthComponent = GetComponent<HealthComponent>();
        healthBarController = GetComponentInChildren<HealthBarController>();

        // Configurar Rigidbody2D para detecção de colisão adequada
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            Debug.LogError("[Enemy] Rigidbody2D component missing! Add one via Inspector.");
        }

        if (healthComponent != null)
        {
            healthComponent.OnDeath.AddListener(Defeated);
        }

        // Encontra o player pela tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // Configurar filtro de movimento baseado no layer atual
        UpdateMovementFilter();
    }

    private void FixedUpdate()
    {
        // Só atualiza se não estiver derrotado e player existir
        if (healthComponent.GetCurrentHealth() <= 0 || playerTransform == null) return;

        UpdateEnemyBehavior();
    }

    // Atualiza o comportamento baseado no estado atual
    void UpdateEnemyBehavior()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                // Verifica se player entrou na área de detecção
                if (CanDetectPlayer())
                {
                    ChangeState(EnemyState.Chasing);
                }
                break;

            case EnemyState.Chasing:
                // Se player saiu da área de detecção, volta para Idle
                if (!CanDetectPlayer())
                {
                    ChangeState(EnemyState.Idle);
                }
                // Se chegou perto o suficiente, ataca
                else if (CanAttackPlayer())
                {
                    ChangeState(EnemyState.Attacking);
                }
                else
                {
                    // Move em direção ao player
                    MoveTowardsPlayer();
                }
                break;

            case EnemyState.Attacking:
                // Verifica se ainda pode atacar
                if (CanAttackPlayer())
                {
                    AttackPlayer();
                }
                else
                {
                    // Se player se afastou, volta a perseguir
                    ChangeState(EnemyState.Chasing);
                }
                break;
        }
    }

    /// <summary>
    /// Verifica se o player pode ser detectado baseado em:
    /// 1. Player existe
    /// 2. Enemy e player estão no mesmo layer (mesmo nível de elevação)
    /// 3. Distância está dentro do raio de detecção
    /// </summary>
    private bool CanDetectPlayer()
    {
        if (playerTransform == null) return false;

        // Verificar se estão no mesmo layer (nível de elevação)
        int enemyLayer = gameObject.layer;
        int playerLayer = playerTransform.gameObject.layer;

        if (enemyLayer != playerLayer)
        {
            // Diferentes níveis de elevação - não pode detectar
            return false;
        }

        // Verificar distância
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        return distanceToPlayer <= detectionRadius;
    }

    /// <summary>
    /// Verifica se o player está dentro do alcance de ataque
    /// </summary>
    private bool CanAttackPlayer()
    {
        if (playerTransform == null || !canAttack) return false;

        // Verificar se estão no mesmo layer
        int enemyLayer = gameObject.layer;
        int playerLayer = playerTransform.gameObject.layer;

        if (enemyLayer != playerLayer)
        {
            return false;
        }

        // Verificar distância
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        return distanceToPlayer <= attackRadius;
    }

    // Muda o estado do inimigo
    void ChangeState(EnemyState newState)
    {
        currentState = newState;

        // Atualiza animações baseado no estado
        switch (currentState)
        {
            case EnemyState.Idle:
                animator.SetBool("isMoving", false);
                if (healthBarController != null)
                    healthBarController.Hide();
                break;

            case EnemyState.Chasing:
                animator.SetBool("isMoving", true);
                if (healthBarController != null)
                    healthBarController.Show();
                break;

            case EnemyState.Attacking:
                animator.SetBool("isMoving", false);
                if (healthBarController != null)
                    healthBarController.Show();
                break;
        }
    }

    // Move o inimigo em direção ao player
    void MoveTowardsPlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        TryMove(direction);
    }

    // Tenta mover o inimigo na direção especificada, verificando colisões
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

    // Ataca o player
    void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            // Aqui seria onde o inimigo causa dano ao player
            // Por enquanto só mostra debug

            // TODO: Implementar dano ao player quando houver sistema de vida do player

            PlayerController player = playerTransform.GetComponent<PlayerController>();
            if (player != null) player.TakeDamage(attackDamage);

            // Inicia cooldown de ataque
            StartCoroutine(AttackCooldownCoroutine());
        }
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
        currentState = EnemyState.Idle;
        animator.SetTrigger("defeated");

        // Esconde a health bar quando derrotado
        if (healthBarController != null)
            healthBarController.Hide();

        // Desabilita o collider para que não continue detectando/atacando
        GetComponent<Collider2D>().enabled = false;
    }

    public void RemoveEnemy()
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
    }

    /// <summary>
    /// Atualiza o filtro de movimento para colidir apenas com layers apropriados
    /// baseado no layer atual do enemy e na Collision Matrix configurada.
    /// </summary>
    void UpdateMovementFilter()
    {
        // Configurar filtro para usar layer mask
        movementFilter.useLayerMask = true;

        // Obter automaticamente quais layers o enemy deve colidir
        // baseado na Collision Matrix (Physics 2D Settings)
        movementFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));

        Debug.Log($"[Enemy] Movement filter atualizado para layer: {LayerMask.LayerToName(gameObject.layer)} (ID: {gameObject.layer})");
    }

    /// <summary>
    /// Método público para atualizar o filtro quando o enemy mudar de elevação.
    /// Pode ser chamado por ElevationZone ou ElevationManager quando há transição de nível.
    /// </summary>
    public void OnElevationChanged()
    {
        UpdateMovementFilter();
    }


}
