using UnityEngine;

/// <summary>
/// Implementação específica para arma tipo Staff (Cajado).
/// O staff é uma arma corpo-a-corpo usada pelo Wizard.
/// Futuramente pode ser estendido para ataques mágicos e projéteis.
/// </summary>
public class StaffWeapon : WeaponBase
{
    /// <summary>
    /// Toca a animação de ataque do staff baseada na direção.
    /// O animator do staff usa blend trees direcionais sincronizados com o corpo.
    /// </summary>
    protected override void PlayAttackAnimation(Vector2 direction)
    {
        if (weaponAnimator == null)
        {
            Debug.LogWarning("[StaffWeapon] Animator não encontrado! Animação de ataque não tocará.");
            return;
        }

        // Sincronizar direção com o animator para blend trees direcionais
        weaponAnimator.SetFloat("moveX", direction.x);
        weaponAnimator.SetFloat("moveY", direction.y);

        // Disparar trigger de ataque
        weaponAnimator.SetTrigger("attack");

        Debug.Log($"[StaffWeapon] Ataque tocado na direção: ({direction.x:F2}, {direction.y:F2})");
    }

    /// <summary>
    /// Detecta colisão com inimigos e aplica dano.
    /// Usa tag "Enemy" para filtrar apenas inimigos.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificar se colidiu com inimigo
        if (!other.CompareTag("Enemy"))
            return;

        // Obter componente de vida do inimigo
        HealthComponent enemyHealth = other.GetComponent<HealthComponent>();

        if (enemyHealth != null && weaponData != null)
        {
            enemyHealth.TakeDamage(weaponData.damage);
            Debug.Log($"[StaffWeapon] {weaponData.weaponName} causou {weaponData.damage} de dano em {other.name}");
        }
        else
        {
            Debug.LogWarning($"[StaffWeapon] Inimigo {other.name} não possui HealthComponent ou weaponData é null!");
        }
    }
}
