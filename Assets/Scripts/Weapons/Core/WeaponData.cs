using UnityEngine;

/// <summary>
/// Enum para tipos de armas disponíveis no jogo.
/// </summary>
public enum WeaponType
{
    None,
    Staff,
    Sword,
    Wand,
    SpellBook
}

/// <summary>
/// ScriptableObject que armazena dados configuráveis de armas.
/// Permite criar diferentes variações de armas sem código adicional.
/// </summary>
[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Info")]
    [Tooltip("Nome da arma exibido no jogo")]
    public string weaponName;

    [Tooltip("Tipo da arma (determina qual prefab usar)")]
    public WeaponType weaponType;

    [Tooltip("Ícone da arma para UI")]
    public Sprite weaponIcon;

    [Header("Combat Stats")]
    [Tooltip("Dano causado por ataque")]
    public int damage = 3;

    [Tooltip("Alcance do ataque em unidades")]
    public float attackRange = 1f;

    [Tooltip("Velocidade de ataque (multiplicador)")]
    public float attackSpeed = 1f;

    [Tooltip("Tempo de cooldown entre ataques em segundos")]
    public float cooldown = 0.5f;

    [Header("Visual & Animation")]
    [Tooltip("Animator Controller usado pela arma")]
    public RuntimeAnimatorController animatorController;

    [Tooltip("Offset de posição para o ataque (lado direito)")]
    public Vector2 attackOffset = Vector2.right * 0.15f;

    [Header("Item Integration")]
    [Tooltip("Referência ao Item ScriptableObject (sistema de inventário)")]
    public Item itemReference;
}
