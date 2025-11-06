using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Vida do Inimigo")]
    public float health = 3;

    [Header("Sistema de Detecção")]
    public float detectionRadius = 0.50f; // Raio para detectar o player
    public float attackRadius = 1f;  // Raio para atacar o player
    public float moveSpeed = 0.05f;       // Velocidade de movimento

    [Header("Sistema de Ataque")]
    public float attackDamage = 1f;    // Dano que o inimigo causa
    public float attackCooldown = 5.5f; // Tempo entre ataques

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

    // Controle de ataque
    private float lastAttackTime;
    private bool canAttack = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Encontra o player pela tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player não encontrado! Certifique-se que o player tenha a tag 'Player'");
        }
    }

    private void FixedUpdate()
    {
        // Só atualiza se não estiver derrotado e player existir
        if (health <= 0 || playerTransform == null) return;

        UpdateEnemyBehavior();
    }

    // Atualiza o comportamento baseado no estado atual
    void UpdateEnemyBehavior()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        switch (currentState)
        {
            case EnemyState.Idle:
                // Verifica se player entrou na área de detecção
                if (distanceToPlayer <= detectionRadius)
                {
                    ChangeState(EnemyState.Chasing);
                }
                break;

            case EnemyState.Chasing:
                // Se player saiu da área de detecção, volta para Idle
                if (distanceToPlayer > detectionRadius)
                {
                    ChangeState(EnemyState.Idle);
                }
                // Se chegou perto o suficiente, ataca
                else if (distanceToPlayer <= attackRadius && canAttack)
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
                if (distanceToPlayer <= attackRadius && canAttack)
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

    // Muda o estado do inimigo
    void ChangeState(EnemyState newState)
    {
        currentState = newState;

        // Atualiza animações baseado no estado
        switch (currentState)
        {
            case EnemyState.Idle:
                animator.SetBool("isMoving", false);
                break;

            case EnemyState.Chasing:
                animator.SetBool("isMoving", true);
                break;

            case EnemyState.Attacking:
                animator.SetBool("isMoving", false);
                break;
        }
    }

    // Move o inimigo em direção ao player
    void MoveTowardsPlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    // Ataca o player
    void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            // Aqui seria onde o inimigo causa dano ao player
            // Por enquanto só mostra debug
            Debug.Log($"Inimigo atacou o player causando {attackDamage} de dano!");

            // TODO: Implementar dano ao player quando houver sistema de vida do player
            // PlayerController player = playerTransform.GetComponent<PlayerController>();
            // if (player != null) player.TakeDamage(attackDamage);

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

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Defeated();
        }
    }

    void Defeated()
    {
        // Para o movimento e muda para estado derrotado
        currentState = EnemyState.Idle;
        animator.SetTrigger("defeated");

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
}
