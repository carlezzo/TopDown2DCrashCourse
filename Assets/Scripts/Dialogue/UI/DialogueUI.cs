using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image characterIcon;
    [SerializeField] private Button continueButton;
    [SerializeField] private GameObject choicesPanel;
    [SerializeField] private Transform choicesContainer;
    [SerializeField] private Button choiceButtonPrefab;

    [Header("Animation Settings")]
    [SerializeField] private float typewriterSpeed = 0.05f;
    [SerializeField] private bool enableTypewriter = true;
    [SerializeField] private AudioClip typingSound;

    [Header("Mobile UI")]
    [SerializeField] private float minButtonSize = 60f;
    [SerializeField] private float buttonSpacing = 10f;

    private AudioSource audioSource;
    private Coroutine typewriterCoroutine;
    private bool isTyping = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Auto-encontrar componentes se não atribuídos
        if (dialoguePanel == null)
            dialoguePanel = transform.Find("DialoguePanel")?.gameObject;

        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);
    }

    void Start()
    {
        // Hide();
    }

    public void Show()
    {
        Debug.Log($"[DialogueUI] Before - Panel: {dialoguePanel?.name}, Active: {dialoguePanel?.activeInHierarchy}");

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        Debug.Log($"[DialogueUI] After - Panel: {dialoguePanel?.name}, Active: {dialoguePanel?.activeInHierarchy}");
    }

    public void Hide()
    {
        Debug.LogWarning(System.Environment.StackTrace); // Mostra quem chamou!

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Parar typewriter se estiver rodando
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
            isTyping = false;
        }
    }

    public void DisplayNode(DialogueNode node)
    {
        if (node == null) return;

        // Exibir nome do falante
        if (speakerNameText != null)
        {
            speakerNameText.text = string.IsNullOrEmpty(node.speakerName) ? "" : node.speakerName;
            speakerNameText.gameObject.SetActive(!string.IsNullOrEmpty(node.speakerName));
        }

        // Exibir ícone do personagem
        if (characterIcon != null)
        {
            if (node.characterIcon != null)
            {
                characterIcon.sprite = node.characterIcon;
                characterIcon.gameObject.SetActive(true);
            }
            else
            {
                characterIcon.gameObject.SetActive(false);
            }
        }

        // Exibir texto com ou sem typewriter
        if (enableTypewriter)
        {
            StartTypewriter(node.text);
        }
        else
        {
            if (dialogueText != null)
                dialogueText.text = node.text;
            SetupContinueButton(node);
        }

        // Configurar escolhas se existirem
        if (node.hasChoices)
        {
            ShowChoices(node.choices);
        }
        else
        {
            HideChoices();
        }
    }

    void StartTypewriter(string text)
    {
        if (typewriterCoroutine != null)
            StopCoroutine(typewriterCoroutine);

        typewriterCoroutine = StartCoroutine(TypewriterEffect(text));
    }

    IEnumerator TypewriterEffect(string text)
    {
        isTyping = true;

        if (dialogueText != null)
            dialogueText.text = "";

        for (int i = 0; i <= text.Length; i++)
        {
            if (dialogueText != null)
                dialogueText.text = text.Substring(0, i);

            // Tocar som de digitação
            if (typingSound != null && audioSource != null && i < text.Length)
            {
                audioSource.PlayOneShot(typingSound);
            }

            yield return new WaitForSeconds(typewriterSpeed);
        }

        isTyping = false;
        SetupContinueButton(null); // Passar null pois já temos o nó atual
    }

    void SetupContinueButton(DialogueNode node)
    {
        if (continueButton == null) return;

        // Mostrar botão apenas se não há escolhas e não está digitando
        bool showButton = !isTyping && (node?.hasChoices != true);
        continueButton.gameObject.SetActive(showButton);
    }

    void ShowChoices(DialogueChoice[] choices)
    {
        HideChoices();

        if (choicesPanel != null)
            choicesPanel.SetActive(true);

        if (choicesContainer == null || choiceButtonPrefab == null)
            return;

        for (int i = 0; i < choices.Length; i++)
        {
            var choice = choices[i];
            var buttonObj = Instantiate(choiceButtonPrefab, choicesContainer).gameObject;
            var button = buttonObj.GetComponent<Button>();
            var text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (text != null)
                text.text = choice.choiceText;

            // Configurar tamanho mínimo para mobile
            var rectTransform = buttonObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, Mathf.Max(minButtonSize, rectTransform.sizeDelta.y));
            }

            // Verificar se escolha está disponível
            bool isAvailable = IsChoiceAvailable(choice);
            button.interactable = isAvailable;

            // Configurar cor para indicar disponibilidade
            var colors = button.colors;
            if (!isAvailable)
            {
                colors.normalColor = Color.gray;
                colors.highlightedColor = Color.gray;
                colors.pressedColor = Color.gray;
                button.colors = colors;
            }

            // Capturar índice para closure
            int choiceIndex = i;
            button.onClick.AddListener(() => OnChoiceSelected(choiceIndex));

            buttonObj.SetActive(true);
        }

        // Esconder botão continuar quando há escolhas
        if (continueButton != null)
            continueButton.gameObject.SetActive(false);
    }

    void HideChoices()
    {
        if (choicesPanel != null)
            choicesPanel.SetActive(false);

        if (choicesContainer != null)
        {
            // Destruir botões de escolha existentes
            foreach (Transform child in choicesContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }

    bool IsChoiceAvailable(DialogueChoice choice)
    {
        if (choice.requiresItem)
        {
            if (InventoryManager.Instance == null)
                return false;

            var requiredItem = Resources.Load<Item>($"Items/{choice.requiredItemName}");
            return requiredItem != null && InventoryManager.Instance.HasItem(requiredItem, choice.requiredItemQuantity);
        }

        return true;
    }

    void OnContinueClicked()
    {
        if (isTyping)
        {
            // Se está digitando, completar imediatamente
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                typewriterCoroutine = null;
            }

            isTyping = false;

            // Mostrar texto completo
            var currentNode = DialogueManager.Instance?.currentDialogue?.GetNode(DialogueManager.Instance.currentNodeIndex);
            if (currentNode != null && dialogueText != null)
            {
                dialogueText.text = currentNode.text;
            }

            SetupContinueButton(currentNode);
        }
        else
        {
            // Avançar para próximo nó
            DialogueManager.Instance?.NextNode();
        }
    }

    void OnChoiceSelected(int choiceIndex)
    {
        DialogueManager.Instance?.SelectChoice(choiceIndex);
    }

    // Método para ser chamado por toque na tela (mobile)
    void Update()
    {
        // Permitir toque para avançar diálogo em mobile
        // if (Input.GetMouseButtonDown(0) && !isTyping)
        // {
        //     // Verificar se o toque foi na área do diálogo e não em botões
        //     Vector2 touchPosition = Input.mousePosition;

        //     // Se não há botões ativos (escolhas), permitir avançar por toque
        //     if (choicesPanel == null || !choicesPanel.activeInHierarchy)
        //     {
        //         OnContinueClicked();
        //     }
        // }
    }
}