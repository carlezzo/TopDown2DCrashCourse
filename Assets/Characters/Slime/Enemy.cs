using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 3;

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
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
        animator.SetTrigger("defeated");
    }

    public void RemoveEnemy()
    {
        Destroy(gameObject);
    }
}
