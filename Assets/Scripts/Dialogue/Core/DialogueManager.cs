using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private GameObject interactionPrompt;

    [Header("Dialogue State")]
    public DialogueData currentDialogue;
    public int currentNodeIndex = 0;
    public NPCController currentNPC;
    public bool isDialogueActive = false;

    [Header("Settings")]
    public bool pauseGameDuringDialogue = true;
    public bool disablePlayerMovementDuringDialogue = true;

    [Header("Events")]
    public UnityEvent<DialogueData> OnDialogueStart;
    public UnityEvent<DialogueData> OnDialogueEnd;
    public UnityEvent<DialogueNode> OnNodeChanged;
    public UnityEvent<string> OnEventTriggered; // Para ações customizadas

    [Header("Persistence")]
    public List<string> completedDialogues = new List<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Auto-encontrar componentes se não atribuídos
            dialogueUI ??= FindFirstObjectByType<DialogueUI>();

            if (dialogueUI == null)
                Debug.LogError("[DialogueManager] DialogueUI não encontrado! Adicione via Inspector ou garanta que existe na cena.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadDialogueProgress();

        // Esconder UI inicialmente
        dialogueUI?.Hide();
        interactionPrompt?.SetActive(false);
    }

    void Update()
    {
        if (isDialogueActive)
        {
            // HandleDialogueInput();
        }
    }

    void HandleDialogueInput()
    {
        // Input para avançar diálogo (tecla ou toque)
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            // Se há escolhas, não avançar automaticamente
            if (currentDialogue.GetNode(currentNodeIndex)?.hasChoices == true)
                return;

            NextNode();
        }

        // ESC para fechar diálogo
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndDialogue();
        }
    }

    public void StartDialogue(DialogueData dialogue, NPCController npc)
    {
        if (dialogue == null || isDialogueActive)
            return;

        currentDialogue = dialogue;
        currentNPC = npc;
        currentNodeIndex = 0;
        isDialogueActive = true;

        OnDialogueStart?.Invoke(dialogue);

        // Pausar jogo se configurado
        if (pauseGameDuringDialogue && GameManager.Instance != null)
        {
            GameManager.Instance.SetGameState(GameState.Dialogue);
        }

        // Desabilitar movimento do player se configurado
        if (disablePlayerMovementDuringDialogue)
        {
            var playerController = FindFirstObjectByType<PlayerController>();
            playerController?.LockMovement();
        }

        // Esconder prompt de interação
        HideInteractionPrompt();

        // Mostrar UI e primeiro nó
        if (dialogueUI != null)
        {
            dialogueUI.Show();
            ShowCurrentNode();
        }

        Debug.Log($"[DialogueManager] Iniciando diálogo: {dialogue.dialogueID} com {npc.npcName}");
    }

    public void EndDialogue()
    {
        if (!isDialogueActive)
            return;

        isDialogueActive = false;

        OnDialogueEnd?.Invoke(currentDialogue);

        // Marcar diálogo como completo
        if (currentDialogue != null && !completedDialogues.Contains(currentDialogue.dialogueID))
        {
            completedDialogues.Add(currentDialogue.dialogueID);
            SaveDialogueProgress();
        }

        // Resumir jogo
        if (pauseGameDuringDialogue && GameManager.Instance != null)
        {
            GameManager.Instance.SetGameState(GameState.Playing);
        }

        // Reabilitar movimento do player
        if (disablePlayerMovementDuringDialogue)
        {
            var playerController = FindFirstObjectByType<PlayerController>();
            playerController?.UnlockMovement();
        }

        // Esconder UI
        dialogueUI?.Hide();

        // Notificar NPC
        currentNPC?.OnDialogueEnd();

        Debug.Log($"[DialogueManager] Diálogo finalizado: {currentDialogue?.dialogueID}");


        currentDialogue = null;
        currentNPC = null;
        currentNodeIndex = 0;
    }

    public void NextNode()
    {
        if (!isDialogueActive || currentDialogue == null)
            return;

        var currentNode = currentDialogue.GetNode(currentNodeIndex);
        if (currentNode == null)
        {
            EndDialogue();
            return;
        }

        // Executar evento se configurado
        if (currentNode.triggersEvent && !string.IsNullOrEmpty(currentNode.eventName))
        {
            TriggerEvent(currentNode.eventName);
        }

        // Determinar próximo nó
        int nextIndex = currentNode.nextNodeIndex;

        if (nextIndex == -1 || nextIndex >= currentDialogue.nodes.Length)
        {
            // Fim do diálogo
            EndDialogue();
            return;
        }

        currentNodeIndex = nextIndex;
        ShowCurrentNode();
    }

    public void SelectChoice(int choiceIndex)
    {
        if (!isDialogueActive || currentDialogue == null)
            return;

        var currentNode = currentDialogue.GetNode(currentNodeIndex);
        if (currentNode == null || !currentNode.hasChoices)
            return;

        if (choiceIndex < 0 || choiceIndex >= currentNode.choices.Length)
            return;

        var choice = currentNode.choices[choiceIndex];

        // Verificar condições da escolha
        if (choice.requiresItem)
        {
            var requiredItem = Resources.Load<Item>($"Items/{choice.requiredItemName}");
            if (InventoryManager.Instance == null || requiredItem == null ||
                !InventoryManager.Instance.HasItem(requiredItem, choice.requiredItemQuantity))
            {
                Debug.Log("[DialogueManager] Escolha bloqueada - item requerido não encontrado");
                return;
            }
        }

        // Executar efeitos da escolha
        if (choice.givesItem && !string.IsNullOrEmpty(choice.itemToGive))
        {
            var item = Resources.Load<Item>($"Items/{choice.itemToGive}");
            if (item != null && InventoryManager.Instance != null)
            {
                InventoryManager.Instance.AddItem(item, choice.itemQuantity);
                Debug.Log($"[DialogueManager] Item dado: {choice.itemToGive} x{choice.itemQuantity}");
            }
        }

        // Ir para próximo nó baseado na escolha
        if (choice.nextNodeIndex == -1)
        {
            EndDialogue();
        }
        else
        {
            currentNodeIndex = choice.nextNodeIndex;
            ShowCurrentNode();
        }
    }

    void ShowCurrentNode()
    {
        if (currentDialogue == null || dialogueUI == null)
            return;

        var node = currentDialogue.GetNode(currentNodeIndex);
        if (node == null)
        {
            EndDialogue();
            return;
        }

        OnNodeChanged?.Invoke(node);
        dialogueUI.DisplayNode(node);
    }

    void TriggerEvent(string eventName)
    {
        OnEventTriggered?.Invoke(eventName);
        Debug.Log($"[DialogueManager] Evento disparado: {eventName}");
    }

    public void ShowInteractionPrompt(NPCController npc)
    {
        if (interactionPrompt != null && !isDialogueActive)
        {
            // Configurar prompt para este NPC
            var promptComponent = interactionPrompt.GetComponent<InteractionPrompt>();
            if (promptComponent != null)
            {
                promptComponent.SetNPC(npc);
                promptComponent.Show();
            }
            else
            {
                // Fallback para compatibilidade se não tiver o componente
                interactionPrompt.SetActive(true);
            }
        }
    }

    public void HideInteractionPrompt()
    {
        if (interactionPrompt != null)
        {
            var promptComponent = interactionPrompt.GetComponent<InteractionPrompt>();
            if (promptComponent != null)
            {
                promptComponent.Hide();
            }
            else
            {
                // Fallback para compatibilidade se não tiver o componente
                interactionPrompt.SetActive(false);
            }
        }
    }

    public bool HasCompletedDialogue(string dialogueID)
    {
        return completedDialogues.Contains(dialogueID);
    }
    
    /// <summary>
    /// Retorna o GameObject do interaction prompt para acesso por outros scripts.
    /// </summary>
    public GameObject GetInteractionPromptGameObject()
    {
        return interactionPrompt;
    }

    void SaveDialogueProgress()
    {
        string json = JsonUtility.ToJson(new DialogueProgressData { completedDialogues = completedDialogues });
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, "dialogue_progress.json");
        System.IO.File.WriteAllText(filePath, json);
        Debug.Log($"[DialogueManager] Progresso salvo: {filePath}");
    }

    void LoadDialogueProgress()
    {
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, "dialogue_progress.json");

        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);
            var data = JsonUtility.FromJson<DialogueProgressData>(json);
            completedDialogues = data?.completedDialogues ?? new List<string>();
            Debug.Log($"[DialogueManager] Progresso carregado: {completedDialogues.Count} diálogos completos");
        }
        else
        {
            completedDialogues = new List<string>();
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            SaveDialogueProgress();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            SaveDialogueProgress();
    }

    void OnDestroy()
    {
        SaveDialogueProgress();
    }
}

[System.Serializable]
public class DialogueProgressData
{
    public List<string> completedDialogues = new List<string>();
}