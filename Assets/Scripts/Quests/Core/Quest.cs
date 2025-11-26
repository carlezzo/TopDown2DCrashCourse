using System.Collections.Generic;
using UnityEngine;

namespace QuestSystem
{
    /// <summary>
    /// ScriptableObject que define uma quest completa (nome, objetivos, recompensas, prerequisites).
    /// Criado em Assets/Resources/Quests/ para permitir persistência via Resources.Load().
    /// </summary>
    [CreateAssetMenu(fileName = "New Quest", menuName = "Quest System/Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        [Header("Quest Identity")]
        [Tooltip("ID único da quest (ex: quest_001)")]
        public string questID;

        [Tooltip("Nome da quest exibido ao jogador")]
        public string questName;

        [TextArea(3, 6)]
        [Tooltip("Descrição da quest")]
        public string description;

        [Header("Objectives")]
        [Tooltip("Lista de objetivos que devem ser completados")]
        public List<QuestObjective> objectives = new List<QuestObjective>();

        [Header("Rewards")]
        [Tooltip("Recompensas ao completar a quest")]
        public QuestReward reward;

        [Header("Requirements")]
        [Tooltip("IDs de quests que devem ser completadas antes desta")]
        public List<string> prerequisiteQuestIDs = new List<string>();

        /// <summary>
        /// Verifica se todos os objetivos foram completados
        /// </summary>
        public bool AreAllObjectivesCompleted()
        {
            if (objectives.Count == 0)
                return false;

            foreach (var objective in objectives)
            {
                if (!objective.IsCompleted)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Reseta o progresso de todos os objetivos
        /// </summary>
        public void ResetProgress()
        {
            foreach (var objective in objectives)
            {
                objective.ResetProgress();
            }
        }

        /// <summary>
        /// Retorna o progresso geral da quest (0.0 a 1.0)
        /// </summary>
        public float GetOverallProgress()
        {
            if (objectives.Count == 0)
                return 0f;

            int completedCount = 0;
            foreach (var objective in objectives)
            {
                if (objective.IsCompleted)
                    completedCount++;
            }

            return (float)completedCount / objectives.Count;
        }
    }
}
