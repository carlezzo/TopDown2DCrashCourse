using UnityEngine;

namespace QuestSystem
{
    /// <summary>
    /// Rastreia objetivos do tipo CollectItem automaticamente via InventoryManager.OnItemAdded.
    /// Compara item coletado com targetID do objetivo e atualiza progresso no QuestManager.
    /// </summary>
    public class CollectItemObjectiveTracker : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool enableDebugLogs = true;

        void Start()
        {
            // Subscribe para o evento de item adicionado ao inventário
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnItemAdded.AddListener(OnItemCollected);
            }
            else
            {
                Debug.LogError("[CollectItemObjectiveTracker] InventoryManager não encontrado! Verifique se existe na cena.");
            }
        }

        void OnDestroy()
        {
            // Unsubscribe para evitar memory leaks
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnItemAdded.RemoveListener(OnItemCollected);
            }
        }

        /// <summary>
        /// Callback chamado quando um item é adicionado ao inventário
        /// </summary>
        private void OnItemCollected(Item item, int quantity)
        {
            if (item == null)
                return;

            // Verifica se há uma quest ativa
            Quest activeQuest = QuestManager.Instance?.GetActiveQuest();
            if (activeQuest == null)
                return;

            // Procura por objetivos do tipo CollectItem que correspondam ao item coletado
            foreach (var objective in activeQuest.objectives)
            {
                // Verifica se é objetivo de coleta e se o targetID bate com o nome do item
                if (objective.type == ObjectiveType.CollectItem &&
                    objective.targetID == item.itemName &&
                    !objective.IsCompleted)
                {
                    // Atualiza o progresso via QuestManager
                    QuestManager.Instance.UpdateObjectiveProgress(objective.objectiveID, quantity);

                    if (enableDebugLogs)
                    {
                        Debug.Log($"[CollectItemObjectiveTracker] Item '{item.itemName}' coletado. " +
                                  $"Progresso: {objective.currentAmount}/{objective.requiredAmount}");
                    }
                }
            }
        }
    }
}
