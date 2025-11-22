using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionPrompt : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button interactButton;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private Image backgroundImage;

    [Header("Settings")]
    [SerializeField] private string defaultPromptText = "Falar";
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private bool autoPosition = true;
    [SerializeField] private Vector2 offsetFromNPC = new Vector2(0, 320);

    [Header("Mobile UI")]
    [SerializeField] private float minButtonSize = 10f;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = Color.yellow;

    private NPCController currentNPC;
    private Camera mainCamera;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    void Awake()
    {
        // Auto-encontrar componentes
        interactButton ??= GetComponent<Button>();
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        canvasGroup ??= gameObject.AddComponent<CanvasGroup>();

        if (interactButton == null)
        {
            Debug.LogError("[InteractionPrompt] Button component not found!");
        }
        else
        {
            // Configurar tamanho mínimo para mobile
            var buttonRect = interactButton.GetComponent<RectTransform>();
            if (buttonRect != null)
            {
                buttonRect.sizeDelta = new Vector2(Mathf.Max(minButtonSize, buttonRect.sizeDelta.x),
                                                  Mathf.Max(minButtonSize, buttonRect.sizeDelta.y));
            }

            interactButton.onClick.AddListener(OnInteractClicked);
        }

        mainCamera = Camera.main;
        mainCamera ??= FindFirstObjectByType<Camera>();
    }

    void Start()
    {
        // Inicializar como invisível
        canvasGroup?.SetAlpha(0);
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (currentNPC != null && autoPosition)
        {
            UpdatePosition();
        }
    }

    public void SetNPC(NPCController npc)
    {
        currentNPC = npc;

        if (currentNPC != null)
        {
            // Configurar texto do prompt baseado no NPC
            string promptTextToShow = string.IsNullOrEmpty(currentNPC.npcName) ?
                defaultPromptText : $"{defaultPromptText} {currentNPC.npcName}";

            if (promptText != null)
                promptText.text = promptTextToShow;

            // Atualizar posição inicial
            if (autoPosition)
                UpdatePosition();
        }
    }

    void UpdatePosition()
    {
        if (currentNPC == null || mainCamera == null || rectTransform == null)
            return;

        // Converter posição do NPC para screen space
        Vector3 npcScreenPos = mainCamera.WorldToScreenPoint(currentNPC.transform.position);

        // Aplicar offset
        npcScreenPos += new Vector3(offsetFromNPC.x, offsetFromNPC.y, 0);

        // Configurar posição do prompt
        rectTransform.position = npcScreenPos;

        // Manter dentro da tela
        ClampToScreen();
    }

    void ClampToScreen()
    {
        if (rectTransform == null) return;

        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;

        var canvasRect = canvas.GetComponent<RectTransform>();
        if (canvasRect == null) return;

        Vector3 pos = rectTransform.localPosition;
        Vector2 canvasSize = canvasRect.sizeDelta;
        Vector2 promptSize = rectTransform.sizeDelta;

        // Manter dentro dos limites do canvas
        pos.x = Mathf.Clamp(pos.x, -canvasSize.x / 2 + promptSize.x / 2, canvasSize.x / 2 - promptSize.x / 2);
        pos.y = Mathf.Clamp(pos.y, -canvasSize.y / 2 + promptSize.y / 2, canvasSize.y / 2 - promptSize.y / 2);

        rectTransform.localPosition = pos;
    }

    void OnInteractClicked()
    {
        if (currentNPC != null)
        {
            // Efeito visual de feedback
            StartCoroutine(ButtonFeedback());

            // Iniciar diálogo
            currentNPC.TryInteract();
        }
    }

    System.Collections.IEnumerator ButtonFeedback()
    {
        if (backgroundImage != null)
        {
            Color originalColor = backgroundImage.color;
            backgroundImage.color = highlightColor;

            yield return new WaitForSeconds(0.1f);

            backgroundImage.color = originalColor;
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);

        if (canvasGroup != null)
        {
            StartCoroutine(FadeIn());
        }
    }

    public void Hide()
    {
        if (canvasGroup != null)
        {
            StartCoroutine(FadeOut());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    System.Collections.IEnumerator FadeIn()
    {
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1;
    }

    System.Collections.IEnumerator FadeOut()
    {
        float elapsedTime = 0;
        float startAlpha = canvasGroup.alpha;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    // Método para ser chamado via Input System
    public void OnInteract()
    {
        if (gameObject.activeInHierarchy && currentNPC != null)
        {
            OnInteractClicked();
        }
    }

    /// <summary>
    /// Método estático para permitir que o PlayerController chame a interação
    /// diretamente quando não há referência ao prompt específico.
    /// </summary>
    public static void TriggerInteraction()
    {
        var dialogueManager = DialogueManager.Instance;
        if (dialogueManager != null)
        {
            var promptGO = dialogueManager.GetInteractionPromptGameObject();
            var prompt = promptGO?.GetComponent<InteractionPrompt>();
            prompt?.OnInteract();
        }
    }

    /// <summary>
    /// Verifica se o prompt está ativo e há um NPC configurado.
    /// </summary>
    public bool IsReadyForInteraction()
    {
        return gameObject.activeInHierarchy && currentNPC != null;
    }
}