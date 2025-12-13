using UnityEngine;

/// <summary>
/// Classe base abstrata para todas as armas.
/// Implementa lógica comum compartilhada entre diferentes tipos de armas.
/// Use o padrão Template Method: métodos virtuais podem ser sobrescritos, métodos abstratos DEVEM ser implementados.
/// </summary>
public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    [Header("Weapon References")]
    [SerializeField] protected Collider2D weaponCollider;
    [SerializeField] protected SpriteRenderer weaponSpriteRenderer;
    [SerializeField] protected Animator weaponAnimator;

    [Header("Weapon Configuration")]
    [SerializeField] protected WeaponData weaponData;

    protected Vector2 attackOffsetRight;
    protected bool isActive = false;

    protected virtual void Awake()
    {
        // Usar null-coalescing operator para auto-assign se não configurado no Inspector
        weaponCollider ??= GetComponent<Collider2D>();
        weaponSpriteRenderer ??= GetComponent<SpriteRenderer>();
        weaponAnimator ??= GetComponent<Animator>();

        if (weaponCollider == null)
            Debug.LogError($"[WeaponBase] Collider2D não encontrado em {gameObject.name}! Adicione um Collider2D.");

        if (weaponSpriteRenderer == null)
            Debug.LogError($"[WeaponBase] SpriteRenderer não encontrado em {gameObject.name}! Adicione um SpriteRenderer.");

        if (weaponAnimator == null)
            Debug.LogError($"[WeaponBase] Animator não encontrado em {gameObject.name}! Adicione um Animator.");
    }

    /// <summary>
    /// Inicializa a arma com dados configurados.
    /// </summary>
    public virtual void Initialize(WeaponData data)
    {
        weaponData = data;
        attackOffsetRight = data?.attackOffset ?? Vector2.right * 0.15f;

        if (weaponCollider != null)
            weaponCollider.enabled = false;

        if (weaponAnimator != null && data?.animatorController != null)
            weaponAnimator.runtimeAnimatorController = data.animatorController;

        Debug.Log($"[WeaponBase] {data?.weaponName ?? "Unknown"} inicializada.");
    }

    /// <summary>
    /// Executa o ataque da arma.
    /// </summary>
    public virtual void Attack(Vector2 direction, bool flipX)
    {
        if (!isActive)
        {
            Debug.LogWarning($"[WeaponBase] Tentou atacar com arma inativa: {weaponData?.weaponName}");
            return;
        }

        ActivateHitbox();
        UpdatePosition(direction, flipX);
        PlayAttackAnimation(direction);
    }

    /// <summary>
    /// Para o ataque e desativa a hitbox.
    /// </summary>
    public virtual void StopAttack()
    {
        DeactivateHitbox();
    }

    /// <summary>
    /// Ativa ou desativa a arma visualmente.
    /// </summary>
    public virtual void SetActive(bool active)
    {
        isActive = active;
        gameObject.SetActive(active);

        if (!active && weaponCollider != null)
            weaponCollider.enabled = false;
    }

    /// <summary>
    /// Retorna os dados da arma.
    /// </summary>
    public WeaponData GetWeaponData() => weaponData;

    /// <summary>
    /// Verifica se a arma está ativa.
    /// </summary>
    public bool IsActive() => isActive;

    /// <summary>
    /// Ativa a hitbox da arma (collider).
    /// </summary>
    protected virtual void ActivateHitbox()
    {
        if (weaponCollider != null)
            weaponCollider.enabled = true;
    }

    /// <summary>
    /// Desativa a hitbox da arma (collider).
    /// </summary>
    protected virtual void DeactivateHitbox()
    {
        if (weaponCollider != null)
            weaponCollider.enabled = false;
    }

    /// <summary>
    /// Atualiza a posição da arma baseado na direção e flip.
    /// </summary>
    protected virtual void UpdatePosition(Vector2 direction, bool flipX)
    {
        Vector3 newPosition;

   
            // Movimento horizontal dominante (left/right)
            if (flipX)
            {
                // Olhando para ESQUERDA - staff à esquerda
                newPosition = new Vector3(-attackOffsetRight.x + -1.2f, attackOffsetRight.y, 0);
            }
            else
            {
                // Olhando para DIREITA - staff à direita
                newPosition = new Vector3(attackOffsetRight.x, attackOffsetRight.y, 0);
            }

        transform.localPosition = newPosition;
    }

    /// <summary>
    /// Atualiza a posição da arma baseado na direção (chamado continuamente).
    /// Implementação da interface IWeapon.
    /// </summary>
    public virtual void UpdateDirectionalPosition(Vector2 direction, bool flipX)
    {
        UpdatePosition(direction, flipX);
    }

    /// <summary>
    /// Toca a animação de ataque.
    /// DEVE ser implementado por subclasses específicas.
    /// </summary>
    protected abstract void PlayAttackAnimation(Vector2 direction);
}
