using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public Vector2 attackOffsetRight;

    private Collider2D swordCollider;

    void Start()
    {
        swordCollider = GetComponent<Collider2D>();
        attackOffsetRight = transform.position;
    }

    public void AttackRight()
    {
        print("Attack Right");
        swordCollider.enabled = true;
        transform.position = attackOffsetRight;
    }

    public void AttackLeft()
    {
        swordCollider.enabled = true;
        transform.position = new Vector3(-attackOffsetRight.x, attackOffsetRight.y);
    }

    public void StopAttack()
    {
        swordCollider.enabled = false;
    }
}
