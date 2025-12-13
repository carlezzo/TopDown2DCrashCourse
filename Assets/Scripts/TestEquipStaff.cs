using UnityEngine;

/// <summary>
/// Script de teste para equipar o staff automaticamente.
/// Adicione este componente ao GameObject do Wizard para testar.
/// </summary>
public class TestEquipStaff : MonoBehaviour
{
    [Header("Weapon to Equip")]
    [Tooltip("Arraste o Staff_Basic WeaponData aqui")]
    public WeaponData staffData;

    [Header("Auto Equip Settings")]
    [Tooltip("Equipar automaticamente quando o jogo iniciar")]
    public bool equipOnStart = true;

    private WizardController wizardController;

    void Start()
    {
        // Obter referência ao WizardController
        wizardController = GetComponent<WizardController>();

        if (wizardController == null)
        {
            Debug.LogError("[TestEquipStaff] WizardController não encontrado! Este script deve estar no mesmo GameObject do Wizard.");
            return;
        }

        // Equipar staff automaticamente se configurado
        if (equipOnStart && staffData != null)
        {
            EquipStaffNow();
        }
    }

    /// <summary>
    /// Equipa o staff (pode ser chamado manualmente para testes).
    /// </summary>
    public void EquipStaffNow()
    {
        if (wizardController != null && staffData != null)
        {
            Debug.Log("[TestEquipStaff] Equipando staff...");
            wizardController.EquipWeapon(staffData);
        }
        else
        {
            Debug.LogWarning("[TestEquipStaff] WizardController ou staffData é null!");
        }
    }

    /// <summary>
    /// Desequipa a arma atual (pode ser chamado manualmente para testes).
    /// </summary>
    public void UnequipWeaponNow()
    {
        if (wizardController != null)
        {
            Debug.Log("[TestEquipStaff] Desequipando arma...");
            wizardController.UnequipWeapon();
        }
    }

    // REMOVIDO: Teclas de atalho não são necessárias para o teste básico
    // Se precisar de teclas de atalho, use o Input System com InputActions
}
