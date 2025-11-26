using System.Collections.Generic;

namespace QuestSystem
{
    /// <summary>
    /// Estrutura de dados para serialização do progresso de quests em JSON (quest ativa, completadas, progresso).
    /// Salvo em Application.persistentDataPath/quest_progress.json.
    /// </summary>
    [System.Serializable]
    public class QuestSaveData
    {
        /// <summary>
        /// ID da quest atualmente ativa
        /// </summary>
        public string activeQuestID = "";

        /// <summary>
        /// Lista de IDs de quests completadas
        /// </summary>
        public List<string> completedQuestIDs = new List<string>();

        /// <summary>
        /// Progresso dos objetivos da quest ativa
        /// </summary>
        public List<ObjectiveProgress> objectiveProgress = new List<ObjectiveProgress>();
    }

    /// <summary>
    /// Representa o progresso de um objetivo individual
    /// </summary>
    [System.Serializable]
    public class ObjectiveProgress
    {
        public string objectiveID;
        public int currentAmount;
    }
}
