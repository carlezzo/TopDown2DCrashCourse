using UnityEngine;

/// <summary>
/// Componente que dropa itens quando a entidade morre.
/// Requer HealthComponent no mesmo GameObject.
/// </summary>
[RequireComponent(typeof(HealthComponent))]
public class DropOnDeath : MonoBehaviour
{
    [Header("Drop Configuration")]
    [Tooltip("DropTable ScriptableObject que define quais itens serão dropados")]
    public DropTable dropTable;

    [Header("Spawn Settings")]
    [Tooltip("Raio de dispersão dos drops ao redor da posição de morte")]
    [Range(0f, 2f)]
    public float dropRadius = 0.5f;

    [Header("Debug")]
    [Tooltip("Exibir logs detalhados sobre drops")]
    public bool debugMode = false;

    private HealthComponent healthComponent;

    void Awake()
    {
        // Obtém referência ao HealthComponent (RequireComponent garante que existe)
        healthComponent = GetComponent<HealthComponent>();

        if (healthComponent == null)
        {
            Debug.LogError($"[DropOnDeath] {gameObject.name} não possui HealthComponent! Componente necessário para detectar morte.");
        }
    }

    void Start()
    {
        // Subscreve ao evento de morte do HealthComponent
        if (healthComponent != null)
        {
            healthComponent.OnDeath.AddListener(HandleDeath);

            if (debugMode)
            {
                Debug.Log($"[DropOnDeath] {gameObject.name} configurado para dropar itens ao morrer.");
            }
        }
    }

    /// <summary>
    /// Callback executado quando a entidade morre.
    /// Dropa todos os itens configurados no DropTable.
    /// </summary>
    private void HandleDeath()
    {
        if (dropTable == null)
        {
            if (debugMode)
            {
                Debug.LogWarning($"[DropOnDeath] {gameObject.name} morreu mas não possui DropTable configurado.");
            }
            return;
        }

        if (debugMode)
        {
            Debug.Log($"[DropOnDeath] {gameObject.name} morreu! Dropando {dropTable.GetDropCount()} tipo(s) de item(s)...");
        }

        // Chama o DropTable para spawnar os itens
        dropTable.SpawnDrops(transform.position, dropRadius);
    }

    void OnDestroy()
    {
        // Remove listener para evitar memory leaks
        if (healthComponent != null)
        {
            healthComponent.OnDeath.RemoveListener(HandleDeath);
        }
    }

    /// <summary>
    /// Visualiza o raio de dispersão dos drops no editor (Gizmos).
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (dropTable != null && dropTable.GetDropCount() > 0)
        {
            Gizmos.color = new Color(1f, 0.8f, 0f, 0.3f); // Amarelo transparente
            Gizmos.DrawWireSphere(transform.position, dropRadius);

            Gizmos.color = new Color(1f, 0.8f, 0f, 0.1f);
            Gizmos.DrawSphere(transform.position, dropRadius);
        }
    }
}
