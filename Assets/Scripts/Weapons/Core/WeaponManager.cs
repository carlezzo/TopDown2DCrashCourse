using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// Gerencia a equip/unequip de armas e sincronização de animações.
/// Componente central do sistema de armas modular.
/// </summary>
public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Slots")]
    [SerializeField] private Transform weaponSlot;

    [Header("Available Weapons")]
    [Tooltip("Lista de WeaponData disponíveis para equipar")]
    [SerializeField] private List<WeaponData> availableWeapons = new List<WeaponData>();

    [Header("Weapon Prefabs")]
    [Tooltip("Prefab do Staff (tipo Staff)")]
    [SerializeField] private GameObject staffWeaponPrefab;

    [Tooltip("Prefab da Espada (tipo Sword)")]
    [SerializeField] private GameObject swordWeaponPrefab;

    [Header("Events")]
    [Tooltip("Evento disparado quando uma arma é equipada")]
    public UnityEvent<WeaponData> OnWeaponEquipped;

    [Tooltip("Evento disparado quando uma arma é desequipada")]
    public UnityEvent OnWeaponUnequipped;

    private IWeapon currentWeapon;
    private GameObject currentWeaponObject;
    private Animator parentAnimator;

    void Awake()
    {
        // Auto-assign weapon slot se não configurado
        weaponSlot ??= transform;

        // Obter animator do parent (WizardController)
        parentAnimator = GetComponentInParent<Animator>();

        if (parentAnimator == null)
            Debug.LogError("[WeaponManager] Animator não encontrado no parent! O sistema de sincronização não funcionará.");
    }

    /// <summary>
    /// Equipa uma arma baseada em WeaponData.
    /// </summary>
    public void EquipWeapon(WeaponData weaponData)
    {
        // Desequipar arma atual primeiro
        UnequipCurrentWeapon();

        if (weaponData == null || weaponData.weaponType == WeaponType.None)
        {
            Debug.LogWarning("[WeaponManager] WeaponData é null ou tipo None. Nenhuma arma equipada.");
            return;
        }

        // Obter prefab apropriado para o tipo de arma
        GameObject prefab = GetWeaponPrefab(weaponData.weaponType);
        if (prefab == null)
        {
            Debug.LogError($"[WeaponManager] Nenhum prefab configurado para tipo: {weaponData.weaponType}");
            return;
        }

        // Instanciar arma como filho do slot
        currentWeaponObject = Instantiate(prefab, weaponSlot);
        currentWeapon = currentWeaponObject.GetComponent<IWeapon>();

        if (currentWeapon != null)
        {
            currentWeapon.Initialize(weaponData);
            currentWeapon.SetActive(true);

            SyncWeaponAnimatorWithParent();
            OnWeaponEquipped?.Invoke(weaponData);

            Debug.Log($"[WeaponManager] {weaponData.weaponName} equipada com sucesso!");
        }
        else
        {
            Debug.LogError($"[WeaponManager] Prefab {prefab.name} não possui componente IWeapon!");
            Destroy(currentWeaponObject);
            currentWeaponObject = null;
        }
    }

    /// <summary>
    /// Desequipa a arma atual.
    /// </summary>
    public void UnequipCurrentWeapon()
    {
        if (currentWeaponObject != null)
        {
            currentWeapon?.SetActive(false);
            Destroy(currentWeaponObject);
            currentWeapon = null;
            currentWeaponObject = null;

            OnWeaponUnequipped?.Invoke();
            Debug.Log("[WeaponManager] Arma desequipada.");
        }
    }

    /// <summary>
    /// Executa ataque com a arma atual.
    /// Chamado pelo WizardController via animation event.
    /// </summary>
    public void Attack(Vector2 direction, bool flipX)
    {
        currentWeapon?.Attack(direction, flipX);
    }

    /// <summary>
    /// Para o ataque atual.
    /// Chamado pelo WizardController via animation event.
    /// </summary>
    public void StopAttack()
    {
        currentWeapon?.StopAttack();
    }

    /// <summary>
    /// Verifica se há uma arma equipada.
    /// </summary>
    public bool HasWeaponEquipped() => currentWeapon != null && currentWeapon.IsActive();

    /// <summary>
    /// Retorna os dados da arma atual.
    /// </summary>
    public WeaponData GetCurrentWeaponData() => currentWeapon?.GetWeaponData();

    /// <summary>
    /// Sincroniza parâmetros do animator da arma com o animator do corpo.
    /// Chamado continuamente no Update para manter animações em sincronia.
    /// </summary>
    private void SyncWeaponAnimatorWithParent()
    {
        if (currentWeaponObject == null || parentAnimator == null) return;

        Animator weaponAnimator = currentWeaponObject.GetComponent<Animator>();
        if (weaponAnimator == null) return;

        // Copiar parâmetros do animator do corpo para o animator da arma
        weaponAnimator.SetFloat("moveX", parentAnimator.GetFloat("moveX"));
        weaponAnimator.SetFloat("moveY", parentAnimator.GetFloat("moveY"));
        weaponAnimator.SetBool("isMoving", parentAnimator.GetBool("isMoving"));
    }

    /// <summary>
    /// Retorna o prefab apropriado baseado no tipo de arma.
    /// </summary>
    private GameObject GetWeaponPrefab(WeaponType type)
    {
        return type switch
        {
            WeaponType.Staff => staffWeaponPrefab,
            WeaponType.Sword => swordWeaponPrefab,
            _ => null
        };
    }

    void Update()
    {
        // Sincronizar animações a cada frame
        if (HasWeaponEquipped())
        {
            SyncWeaponAnimatorWithParent();

            // Atualizar posição da arma continuamente baseado na direção
            Vector2 currentDirection = new Vector2(
                parentAnimator.GetFloat("moveX"),
                parentAnimator.GetFloat("moveY")
            );

            // Obter flipX do parent (wizard)
            SpriteRenderer parentSprite = GetComponentInParent<SpriteRenderer>();
            bool flipX = parentSprite != null ? parentSprite.flipX : false;

            // Atualizar posição da arma
            currentWeapon?.UpdateDirectionalPosition(currentDirection, flipX);
        }
    }
}
