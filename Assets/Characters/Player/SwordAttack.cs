using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private Vector2 attackOffsetRight;
    [SerializeField] private int damage = 3;

    [Header("References")]
    [SerializeField] private Collider2D swordCollider;

    private void Awake()
    {
        swordCollider ??= GetComponent<Collider2D>();

        if (swordCollider == null)
        {
            Debug.LogError("SwordAttack: Collider2D não encontrado! Adicione via Inspector ou garanta que existe no GameObject.", this);
        }
        else
        {
            // Ensure sword collider starts disabled
            swordCollider.enabled = false;
        }
    }

    private void Start()
    {
        attackOffsetRight = transform.localPosition;
    }

    public void AttackRight()
    {
        if (swordCollider != null)
        {
            swordCollider.enabled = true;
            transform.localPosition = attackOffsetRight;
        }
    }

    public void AttackLeft()
    {
        if (swordCollider != null)
        {
            swordCollider.enabled = true;
            transform.localPosition = new Vector3(-attackOffsetRight.x, attackOffsetRight.y);
        }
    }

    public void StopAttack()
    {
        if (swordCollider != null)
        {
            swordCollider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (!other.CompareTag("Enemy"))
        {
            return;
        }

        HealthComponent enemyHealth = other.GetComponent<HealthComponent>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
            Debug.Log($"SwordAttack: Dealt {damage} damage to {other.name}", this);
        }
    }
}
