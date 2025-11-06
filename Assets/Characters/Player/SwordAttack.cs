using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public Vector2 attackOffsetRight;

    public Collider2D swordCollider;

    public float damage = 3f;

    void Start()
    {
        // swordCollider = GetComponent<Collider2D>();
        attackOffsetRight = transform.position;
    }

    public void AttackRight()
    {
        swordCollider.enabled = true;
        transform.localPosition = attackOffsetRight;
    }

    public void AttackLeft()
    {
        swordCollider.enabled = true;
        transform.localPosition = new Vector3(-attackOffsetRight.x, attackOffsetRight.y);
    }

    public void StopAttack()
    {
        swordCollider.enabled = false;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") == false)
        {
            return;
        }

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }

}
