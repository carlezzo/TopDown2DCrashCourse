using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject que define quais itens um inimigo dropa ao morrer.
/// Pode ser reutilizado entre múltiplos inimigos do mesmo tipo.
/// </summary>
[CreateAssetMenu(fileName = "NewDropTable", menuName = "Inventory/DropTable")]
public class DropTable : ScriptableObject
{
    [System.Serializable]
    public class DropEntry
    {
        [Header("Item Configuration")]
        [Tooltip("Item ScriptableObject que será dropado")]
        public Item item;

        [Tooltip("Prefab do ItemPickup a ser instanciado no mundo")]
        public GameObject pickupPrefab;

        [Header("Quantity")]
        [Tooltip("Quantidade fixa do item a ser dropado")]
        [Range(1, 99)]
        public int quantity = 1;
    }

    [Header("Drop Settings")]
    [Tooltip("Lista de itens que serão dropados (todos são garantidos)")]
    public List<DropEntry> drops = new List<DropEntry>();

    /// <summary>
    /// Instancia todos os drops na posição especificada.
    /// </summary>
    /// <param name="position">Posição onde os drops serão criados</param>
    /// <param name="spreadRadius">Raio de dispersão dos drops</param>
    public void SpawnDrops(Vector3 position, float spreadRadius = 0.5f)
    {
        if (drops == null || drops.Count == 0)
        {
            Debug.LogWarning($"[DropTable] {name} não possui drops configurados!");
            return;
        }

        foreach (var dropEntry in drops)
        {
            if (dropEntry.pickupPrefab == null)
            {
                Debug.LogWarning($"[DropTable] {name} possui drop entry sem prefab configurado!");
                continue;
            }

            SpawnItem(dropEntry, position, spreadRadius);
        }
    }

    /// <summary>
    /// Instancia um único item no mundo.
    /// </summary>
    private void SpawnItem(DropEntry entry, Vector3 position, float radius)
    {
        // Calcula posição aleatória dentro do raio de dispersão
        Vector2 randomOffset = Random.insideUnitCircle * radius;
        Vector3 spawnPosition = position + (Vector3)randomOffset;

        // Instancia o prefab do pickup
        GameObject droppedItem = Instantiate(entry.pickupPrefab, spawnPosition, Quaternion.identity);

        // Configura a quantidade no ItemPickup
        ItemPickup pickup = droppedItem.GetComponent<ItemPickup>();
        if (pickup != null)
        {
            pickup.item = entry.item;
            pickup.quantity = entry.quantity;
        }
        else
        {
            Debug.LogError($"[DropTable] Prefab {entry.pickupPrefab.name} não possui componente ItemPickup!");
        }

        Debug.Log($"[DropTable] Dropou {entry.quantity}x {entry.item?.itemName ?? "Unknown"} em {spawnPosition}");
    }

    /// <summary>
    /// Retorna o número total de itens diferentes que este DropTable dropa.
    /// </summary>
    public int GetDropCount()
    {
        return drops?.Count ?? 0;
    }
}
