using System.Collections.Generic;
using UnityEngine;

namespace QuestSystem
{
    /// <summary>
    /// Define as recompensas que o jogador recebe ao completar uma quest (itens, XP, moeda).
    /// Serializable para ser usado dentro de Quest ScriptableObjects.
    /// </summary>
    [System.Serializable]
    public class QuestReward
    {
        [Header("Item Rewards")]
        [Tooltip("Itens que serão adicionados ao inventário")]
        public List<ItemReward> items = new List<ItemReward>();

        [Header("Progression Rewards")]
        [Tooltip("Pontos de experiência (para sistema futuro)")]
        public int experiencePoints = 0;

        [Tooltip("Moeda/dinheiro (para sistema futuro)")]
        public int currency = 0;

        /// <summary>
        /// Verifica se a quest tem alguma recompensa
        /// </summary>
        public bool HasRewards => items.Count > 0 || experiencePoints > 0 || currency > 0;
    }

    /// <summary>
    /// Representa um item de recompensa com quantidade
    /// </summary>
    [System.Serializable]
    public class ItemReward
    {
        public Item item;
        public int quantity = 1;
    }
}
