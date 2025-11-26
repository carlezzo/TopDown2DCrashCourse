using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace QuestSystem
{
    /// <summary>
    /// Gerenciador global do sistema de quests (Singleton com DontDestroyOnLoad).
    /// Gerencia quest ativa, progresso, save/load automático e dispara eventos para UI.
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        [Header("Events")]
        public UnityEvent<Quest> OnQuestStarted;
        public UnityEvent<Quest> OnQuestCompleted;
        public UnityEvent<Quest> OnQuestFailed;
        public UnityEvent<Quest, QuestObjective> OnObjectiveUpdated;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;

        // Estado atual
        private Quest activeQuest;
        private List<string> completedQuestIDs = new List<string>();

        // Persistência
        private string SavePath => Application.persistentDataPath + "/quest_progress.json";

        void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                // LoadQuestProgress();

                if (enableDebugLogs)
                    Debug.Log("[QuestManager] Initialized and quest progress loaded.");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #region Quest Management

        /// <summary>
        /// Inicia uma nova quest
        /// </summary>
        public void StartQuest(Quest quest)
        {
            if (quest == null)
            {
                Debug.LogError("[QuestManager] Tentativa de iniciar quest nula!");
                return;
            }

            if (activeQuest != null)
            {
                Debug.LogWarning($"[QuestManager] Já existe uma quest ativa: {activeQuest.questName}. Completando automaticamente...");
                CompleteCurrentQuest();
            }

            // Verifica prerequisites
            if (!CanStartQuest(quest))
            {
                Debug.LogWarning($"[QuestManager] Quest {quest.questName} não pode ser iniciada. Prerequisites não atendidos.");
                return;
            }

            // Reseta progresso antes de iniciar
            quest.ResetProgress();

            activeQuest = quest;
            OnQuestStarted?.Invoke(quest);
            SaveQuestProgress();

            if (enableDebugLogs)
                Debug.Log($"[QuestManager] Quest iniciada: {quest.questName}");
        }

        /// <summary>
        /// Completa a quest atual
        /// </summary>
        public void CompleteCurrentQuest()
        {
            if (activeQuest == null)
            {
                Debug.LogWarning("[QuestManager] Nenhuma quest ativa para completar.");
                return;
            }

            // Adiciona à lista de completadas
            if (!completedQuestIDs.Contains(activeQuest.questID))
            {
                completedQuestIDs.Add(activeQuest.questID);
            }

            // Concede recompensas
            GrantRewards(activeQuest.reward);

            Quest completedQuest = activeQuest;
            activeQuest = null;

            OnQuestCompleted?.Invoke(completedQuest);
            SaveQuestProgress();

            if (enableDebugLogs)
                Debug.Log($"[QuestManager] Quest completada: {completedQuest.questName}");
        }

        /// <summary>
        /// Atualiza o progresso de um objetivo específico
        /// </summary>
        public void UpdateObjectiveProgress(string objectiveID, int amount = 1)
        {
            if (activeQuest == null)
            {
                Debug.LogWarning("[QuestManager] Nenhuma quest ativa para atualizar objetivo.");
                return;
            }

            var objective = activeQuest.objectives.Find(obj => obj.objectiveID == objectiveID);

            if (objective == null)
            {
                Debug.LogWarning($"[QuestManager] Objetivo {objectiveID} não encontrado na quest {activeQuest.questName}");
                return;
            }

            // Se já estava completo, não atualiza
            if (objective.IsCompleted)
                return;

            objective.IncrementProgress(amount);
            OnObjectiveUpdated?.Invoke(activeQuest, objective);
            SaveQuestProgress();

            if (enableDebugLogs)
                Debug.Log($"[QuestManager] Objetivo atualizado: {objective.description} ({objective.currentAmount}/{objective.requiredAmount})");

            // Verifica se a quest foi completada
            if (activeQuest.AreAllObjectivesCompleted())
            {
                CompleteCurrentQuest();
            }
        }

        /// <summary>
        /// Verifica se uma quest pode ser iniciada (prerequisites)
        /// </summary>
        public bool CanStartQuest(Quest quest)
        {
            if (quest == null)
                return false;

            foreach (string prereqID in quest.prerequisiteQuestIDs)
            {
                if (!HasCompletedQuest(prereqID))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Verifica se uma quest já foi completada
        /// </summary>
        public bool HasCompletedQuest(string questID)
        {
            return completedQuestIDs.Contains(questID);
        }

        /// <summary>
        /// Retorna a quest atualmente ativa
        /// </summary>
        public Quest GetActiveQuest()
        {
            return activeQuest;
        }

        #endregion

        #region Rewards

        /// <summary>
        /// Concede as recompensas da quest ao jogador
        /// </summary>
        private void GrantRewards(QuestReward reward)
        {
            if (reward == null || !reward.HasRewards)
                return;

            // Adiciona itens ao inventário
            foreach (var itemReward in reward.items)
            {
                if (itemReward.item != null)
                {
                    InventoryManager.Instance?.AddItem(itemReward.item, itemReward.quantity);

                    if (enableDebugLogs)
                        Debug.Log($"[QuestManager] Recompensa concedida: {itemReward.item.itemName} x{itemReward.quantity}");
                }
            }

            // TODO: Implementar sistema de XP e moeda no futuro
            if (reward.experiencePoints > 0)
            {
                if (enableDebugLogs)
                    Debug.Log($"[QuestManager] XP concedido: {reward.experiencePoints} (sistema não implementado ainda)");
            }

            if (reward.currency > 0)
            {
                if (enableDebugLogs)
                    Debug.Log($"[QuestManager] Moeda concedida: {reward.currency} (sistema não implementado ainda)");
            }
        }

        #endregion

        #region Save/Load System

        /// <summary>
        /// Salva o progresso das quests em JSON
        /// </summary>
        public void SaveQuestProgress()
        {
            QuestSaveData data = new QuestSaveData();

            // Salva quest ativa
            data.activeQuestID = activeQuest?.questID ?? "";

            // Salva quests completadas
            data.completedQuestIDs = new List<string>(completedQuestIDs);

            // Salva progresso dos objetivos
            if (activeQuest != null)
            {
                foreach (var objective in activeQuest.objectives)
                {
                    data.objectiveProgress.Add(new ObjectiveProgress
                    {
                        objectiveID = objective.objectiveID,
                        currentAmount = objective.currentAmount
                    });
                }
            }

            // Serializa e salva
            try
            {
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(SavePath, json);

                if (enableDebugLogs)
                    Debug.Log($"[QuestManager] Progresso salvo em: {SavePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[QuestManager] Erro ao salvar progresso: {e.Message}");
            }
        }

        /// <summary>
        /// Carrega o progresso das quests do JSON
        /// </summary>
        public void LoadQuestProgress()
        {
            if (!File.Exists(SavePath))
            {
                if (enableDebugLogs)
                    Debug.Log("[QuestManager] Nenhum arquivo de save encontrado. Começando do zero.");
                return;
            }

            try
            {
                string json = File.ReadAllText(SavePath);
                QuestSaveData data = JsonUtility.FromJson<QuestSaveData>(json);

                // Restaura quests completadas
                completedQuestIDs = data.completedQuestIDs ?? new List<string>();

                // Restaura quest ativa
                if (!string.IsNullOrEmpty(data.activeQuestID))
                {
                    Quest questToRestore = Resources.Load<Quest>($"Quests/{data.activeQuestID}");

                    if (questToRestore != null)
                    {
                        // Restaura progresso dos objetivos
                        foreach (var savedProgress in data.objectiveProgress)
                        {
                            var objective = questToRestore.objectives.Find(
                                obj => obj.objectiveID == savedProgress.objectiveID
                            );

                            if (objective != null)
                            {
                                objective.currentAmount = savedProgress.currentAmount;
                            }
                        }

                        activeQuest = questToRestore;
                        OnQuestStarted?.Invoke(activeQuest);

                        if (enableDebugLogs)
                            Debug.Log($"[QuestManager] Quest restaurada: {questToRestore.questName}");
                    }
                    else
                    {
                        Debug.LogError($"[QuestManager] Quest {data.activeQuestID} não encontrada em Resources/Quests/");
                    }
                }

                if (enableDebugLogs)
                    Debug.Log($"[QuestManager] Progresso carregado. Quests completadas: {completedQuestIDs.Count}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[QuestManager] Erro ao carregar progresso: {e.Message}");
            }
        }

        #endregion

        #region Auto-Save Triggers

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                SaveQuestProgress();
        }

        void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
                SaveQuestProgress();
        }

        void OnDestroy()
        {
            if (Instance == this)
                SaveQuestProgress();
        }

        #endregion

        #region Debug Menu

        [ContextMenu("Clear Quest Progress")]
        private void ClearQuestProgress()
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log("[QuestManager] Quest progress cleared!");
            }

            activeQuest = null;
            completedQuestIDs.Clear();
        }

        [ContextMenu("Print Save Path")]
        private void PrintSavePath()
        {
            Debug.Log($"[QuestManager] Save Path: {SavePath}");
        }

        #endregion
    }
}
