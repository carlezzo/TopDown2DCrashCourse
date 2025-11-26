using System.Collections;
using UnityEngine;

namespace QuestSystem
{
    /// <summary>
    /// Component para iniciar quests automaticamente (Auto), por colisão (OnEnterZone) ou manual (TriggerQuest()).
    /// Valida prerequisites antes de iniciar a quest configurada.
    /// </summary>
    public class QuestTrigger : MonoBehaviour
    {
        [Header("Quest Configuration")]
        [Tooltip("Quest ScriptableObject que será iniciada")]
        [SerializeField] private Quest questToStart;

        [Header("Trigger Settings")]
        [Tooltip("Como a quest será iniciada")]
        [SerializeField] private TriggerType triggerType = TriggerType.Auto;

        [Tooltip("Destruir GameObject após iniciar quest?")]
        [SerializeField] private bool destroyAfterTrigger = false;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;

        private bool hasTriggered = false;

        void Start()
        {
            if (triggerType == TriggerType.Auto)
            {
                // Delay de 1 frame para garantir que QuestTrackerUI.OnEnable() já fez subscribe aos eventos
                StartCoroutine(DelayedAutoStart());
            }
        }

        /// <summary>
        /// Coroutine para delay de 1 frame antes de iniciar quest automaticamente
        /// </summary>
        private IEnumerator DelayedAutoStart()
        {
            yield return null; // Espera 1 frame
            TryStartQuest();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (triggerType == TriggerType.OnEnterZone && other.CompareTag("Player"))
            {
                TryStartQuest();
            }
        }

        /// <summary>
        /// Tenta iniciar a quest configurada
        /// </summary>
        private void TryStartQuest()
        {
            // Evita trigger múltiplo
            if (hasTriggered)
                return;

            if (questToStart == null)
            {
                Debug.LogError("[QuestTrigger] Quest não configurada!");
                return;
            }

            if (QuestManager.Instance == null)
            {
                Debug.LogError("[QuestTrigger] QuestManager não encontrado na cena!");
                return;
            }

            // Verifica se a quest pode ser iniciada (prerequisites)
            if (!QuestManager.Instance.CanStartQuest(questToStart))
            {
                if (enableDebugLogs)
                {
                    Debug.Log($"[QuestTrigger] Quest '{questToStart.questName}' não pode ser iniciada ainda. " +
                              "Prerequisites não atendidos.");
                }
                return;
            }

            // Verifica se já foi completada
            if (QuestManager.Instance.HasCompletedQuest(questToStart.questID))
            {
                if (enableDebugLogs)
                {
                    Debug.Log($"[QuestTrigger] Quest '{questToStart.questName}' já foi completada anteriormente.");
                }

                if (destroyAfterTrigger)
                    Destroy(gameObject);

                return;
            }

            // Inicia a quest
            QuestManager.Instance.StartQuest(questToStart);
            hasTriggered = true;

            if (enableDebugLogs)
            {
                Debug.Log($"[QuestTrigger] Quest '{questToStart.questName}' iniciada via {triggerType}!");
            }

            // Opcional: destruir o trigger após usar
            if (destroyAfterTrigger)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Método público para iniciar quest manualmente via código/evento
        /// </summary>
        public void TriggerQuest()
        {
            TryStartQuest();
        }
    }

    /// <summary>
    /// Tipos de trigger para iniciar quests
    /// </summary>
    public enum TriggerType
    {
        Auto,           // Inicia automaticamente no Start()
        OnEnterZone,    // Inicia quando player entra no trigger (requer Collider2D)
        Manual          // Inicia via chamada manual do método TriggerQuest()
    }
}
