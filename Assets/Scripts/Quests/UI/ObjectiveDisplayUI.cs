using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QuestSystem
{
    /// <summary>
    /// Component UI para exibir uma linha individual de objetivo (ex: "☐ Coletar Frutas: 2/3").
    /// Mostra progresso, checkmark quando completo e strikethrough text.
    /// </summary>
    public class ObjectiveDisplayUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI objectiveText;
        [SerializeField] private Image checkmarkIcon;

        [Header("Visual Settings")]
        [SerializeField] private Color incompleteColor = Color.white;
        [SerializeField] private Color completedColor = Color.green;

        void Awake()
        {
            // Auto-reference com fallback
            objectiveText ??= GetComponentInChildren<TextMeshProUGUI>();
            checkmarkIcon ??= transform.Find("Checkmark")?.GetComponent<Image>();

            if (objectiveText == null)
                Debug.LogError("[ObjectiveDisplayUI] TextMeshProUGUI não encontrado!");
        }

        /// <summary>
        /// Atualiza a exibição do objetivo
        /// </summary>
        public void UpdateDisplay(QuestObjective objective)
        {
            if (objective == null)
                return;

            // Atualiza texto com progresso
            if (objective.requiredAmount > 1)
            {
                objectiveText.text = $"{objective.description}: {objective.currentAmount}/{objective.requiredAmount}";
            }
            else
            {
                objectiveText.text = objective.description;
            }

            // Atualiza visual baseado no status
            bool isCompleted = objective.IsCompleted;

            objectiveText.color = isCompleted ? completedColor : incompleteColor;

            // Mostra/esconde checkmark
            if (checkmarkIcon != null)
            {
                checkmarkIcon.gameObject.SetActive(isCompleted);
            }

            // Adiciona strikethrough se completado
            if (isCompleted)
            {
                objectiveText.fontStyle = FontStyles.Strikethrough;
            }
            else
            {
                objectiveText.fontStyle = FontStyles.Normal;
            }
        }

        /// <summary>
        /// Define o texto manualmente (para casos especiais)
        /// </summary>
        public void SetText(string text)
        {
            if (objectiveText != null)
                objectiveText.text = text;
        }

        /// <summary>
        /// Define o estado de completado manualmente
        /// </summary>
        public void SetCompleted(bool completed)
        {
            objectiveText.color = completed ? completedColor : incompleteColor;

            if (checkmarkIcon != null)
                checkmarkIcon.gameObject.SetActive(completed);

            objectiveText.fontStyle = completed ? FontStyles.Strikethrough : FontStyles.Normal;
        }
    }
}
