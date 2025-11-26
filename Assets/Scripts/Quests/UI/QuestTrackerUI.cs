using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace QuestSystem
{
    /// <summary>
    /// UI HUD que exibe a quest ativa e seus objetivos em tempo real no canto da tela.
    /// Instancia ObjectiveDisplay prefabs dinamicamente e atualiza via eventos do QuestManager.
    /// </summary>
    public class QuestTrackerUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject trackerPanel;
        [SerializeField] private TextMeshProUGUI questNameText;
        [SerializeField] private Transform objectivesContainer;
        [SerializeField] private ObjectiveDisplayUI objectivePrefab;

        [Header("Settings")]
        [SerializeField] private bool hideWhenNoQuest = true;

        [Header("References")]
        [SerializeField] private QuestManager questManager;

        // Cache dos displays de objetivos
        private List<ObjectiveDisplayUI> objectiveDisplays = new List<ObjectiveDisplayUI>();

        void Awake()
        {
            // Auto-reference com fallback
            questManager ??= FindFirstObjectByType<QuestManager>();

            if (questManager == null)
                Debug.LogError("[QuestTrackerUI] QuestManager não encontrado na cena!");

            if (trackerPanel == null)
                trackerPanel = gameObject;

            if (objectivesContainer == null)
            {
                Debug.LogError("[QuestTrackerUI] ObjectivesContainer não configurado!");
            }

            // Subscribe para eventos do QuestManager no Awake (executa antes de qualquer Start())
            if (questManager != null)
            {
                questManager.OnQuestStarted.AddListener(OnQuestStarted);
                questManager.OnQuestCompleted.AddListener(OnQuestCompleted);
                questManager.OnObjectiveUpdated.AddListener(OnObjectiveUpdated);
            }
            else if (QuestManager.Instance != null)
            {
                questManager = QuestManager.Instance;
                questManager.OnQuestStarted.AddListener(OnQuestStarted);
                questManager.OnQuestCompleted.AddListener(OnQuestCompleted);
                questManager.OnObjectiveUpdated.AddListener(OnObjectiveUpdated);
            }
        }

        void OnEnable()
        {
            // Atualiza UI com quest ativa (se houver)
            RefreshUI();
        }

        void OnDisable()
        {
            // Unsubscribe para evitar memory leaks
            if (questManager != null)
            {
                questManager.OnQuestStarted.RemoveListener(OnQuestStarted);
                questManager.OnQuestCompleted.RemoveListener(OnQuestCompleted);
                questManager.OnObjectiveUpdated.RemoveListener(OnObjectiveUpdated);
            }
        }

        #region Event Handlers

        private void OnQuestStarted(Quest quest)
        {
            RefreshUI();
        }

        private void OnQuestCompleted(Quest quest)
        {
            // Esconde tracker quando quest é completada
            if (hideWhenNoQuest)
            {
                // trackerPanel.SetActive(false);
            }
        }

        private void OnObjectiveUpdated(Quest quest, QuestObjective objective)
        {
            // Atualiza apenas o objetivo específico que mudou
            UpdateObjectiveDisplay(objective);
        }

        #endregion

        #region UI Update

        /// <summary>
        /// Atualiza toda a UI do tracker
        /// </summary>
        private void RefreshUI()
        {
            Quest activeQuest = questManager?.GetActiveQuest();

            if (activeQuest == null)
            {
                // Nenhuma quest ativa - NÃO desabilitar o panel aqui
                // Isso causava race condition quando chamado no OnEnable antes da quest ser iniciada
                // O panel só deve ser desabilitado no OnQuestCompleted
                return;
            }

            // Mostra tracker
            trackerPanel.SetActive(true);

            // Atualiza nome da quest
            if (questNameText != null)
            {
                questNameText.text = activeQuest.questName;
            }

            // Limpa displays antigos
            ClearObjectiveDisplays();

            // Cria displays para cada objetivo
            foreach (var objective in activeQuest.objectives)
            {
                CreateObjectiveDisplay(objective);
            }
        }

        /// <summary>
        /// Cria um display UI para um objetivo
        /// </summary>
        private void CreateObjectiveDisplay(QuestObjective objective)
        {
            if (objectivePrefab == null || objectivesContainer == null)
            {
                Debug.LogError("[QuestTrackerUI] ObjectivePrefab ou ObjectivesContainer não configurado!");
                return;
            }

            ObjectiveDisplayUI display = Instantiate(objectivePrefab, objectivesContainer);
            display.UpdateDisplay(objective);
            objectiveDisplays.Add(display);
        }

        /// <summary>
        /// Atualiza o display de um objetivo específico
        /// </summary>
        private void UpdateObjectiveDisplay(QuestObjective objective)
        {
            Quest activeQuest = questManager?.GetActiveQuest();
            if (activeQuest == null)
                return;

            // Encontra o índice do objetivo na quest
            int index = activeQuest.objectives.IndexOf(objective);

            if (index >= 0 && index < objectiveDisplays.Count)
            {
                objectiveDisplays[index].UpdateDisplay(objective);
            }
        }

        /// <summary>
        /// Limpa todos os displays de objetivos
        /// </summary>
        private void ClearObjectiveDisplays()
        {
            foreach (var display in objectiveDisplays)
            {
                if (display != null)
                    Destroy(display.gameObject);
            }

            objectiveDisplays.Clear();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Força atualização manual da UI (útil para debug)
        /// </summary>
        public void ForceRefresh()
        {
            RefreshUI();
        }

        /// <summary>
        /// Mostra/esconde o tracker manualmente
        /// </summary>
        public void SetVisible(bool visible)
        {
            if (trackerPanel != null)
                trackerPanel.SetActive(visible);
        }

        #endregion
    }
}
