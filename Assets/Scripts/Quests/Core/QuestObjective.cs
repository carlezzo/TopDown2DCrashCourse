using UnityEngine;

namespace QuestSystem
{
    /// <summary>
    /// Representa um objetivo individual dentro de uma quest (ex: coletar 3 frutas).
    /// Serializable para ser usado dentro de Quest ScriptableObjects.
    /// </summary>
    [System.Serializable]
    public class QuestObjective
    {
        [Header("Objective Info")]
        public string objectiveID;
        public ObjectiveType type;

        [TextArea(2, 4)]
        public string description;

        [Header("Target")]
        [Tooltip("Nome do item, NPC ID, enemy tag, etc.")]
        public string targetID;

        [Header("Progress")]
        public int requiredAmount = 1;
        public int currentAmount = 0;

        /// <summary>
        /// Verifica se o objetivo foi completado
        /// </summary>
        public bool IsCompleted => currentAmount >= requiredAmount;

        /// <summary>
        /// Incrementa o progresso do objetivo
        /// </summary>
        public void IncrementProgress(int amount = 1)
        {
            currentAmount = Mathf.Min(currentAmount + amount, requiredAmount);
        }

        /// <summary>
        /// Reseta o progresso do objetivo
        /// </summary>
        public void ResetProgress()
        {
            currentAmount = 0;
        }
    }

    /// <summary>
    /// Tipos de objetivos suportados pelo sistema
    /// </summary>
    public enum ObjectiveType
    {
        CollectItem,    // Coletar X itens
        TalkToNPC,      // Falar com NPC
        DefeatEnemy,    // Derrotar X inimigos
        ReachLocation,  // Chegar em um local
        ConsumeItem     // Consumir X itens
    }
}
