using UnityEngine;
using UnityEngine.Events;

public class NPCController : MonoBehaviour
{
    [Header("NPC Settings")]
    public string npcName = "NPC";
    public DialogueData dialogueData;
    public float interactionRange = 2f;
    public InteractionMode interactionMode = InteractionMode.Manual;
    
    [Header("Visual Feedback")]
    public GameObject interactionIndicator;
    public bool enableIndicatorBobbing = true;
    public float bobbingSpeed = 2f;
    public float bobbingHeight = 0.2f;
    
    [Header("Audio")]
    public AudioClip interactionSound;
    
    [Header("Events")]
    public UnityEvent OnPlayerEnterRange;
    public UnityEvent OnPlayerExitRange;
    public UnityEvent OnInteractionStart;
    public UnityEvent OnInteractionEnd;
    
    private bool playerInRange = false;
    private GameObject player;
    private AudioSource audioSource;
    private Vector3 indicatorStartPosition;
    private bool hasInteracted = false;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Configurar indicador de interação
        if (interactionIndicator != null)
        {
            indicatorStartPosition = interactionIndicator.transform.localPosition;
            interactionIndicator.SetActive(false);
        }
        
        // Buscar player
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError($"[NPCController] Player not found! Make sure Player GameObject has 'Player' tag.");
        }
    }
    
    void Update()
    {
        if (player == null) return;
        
        CheckPlayerProximity();
        
        // Animar indicador se ativo
        if (interactionIndicator != null && interactionIndicator.activeInHierarchy && enableIndicatorBobbing)
        {
            AnimateIndicator();
        }
    }
    
    void CheckPlayerProximity()
    {
        float distance = Vector2.Distance(transform.position, player.transform.position);
        bool wasInRange = playerInRange;
        playerInRange = distance <= interactionRange;
        
        // Player entrou no range
        if (playerInRange && !wasInRange)
        {
            OnPlayerEnterRange?.Invoke();
            ShowInteractionIndicator();
            
            // Se modo for automático, iniciar diálogo imediatamente
            if (interactionMode == InteractionMode.Automatic)
            {
                StartDialogue();
            }
        }
        // Player saiu do range
        else if (!playerInRange && wasInRange)
        {
            OnPlayerExitRange?.Invoke();
            HideInteractionIndicator();
        }
    }
    
    void ShowInteractionIndicator()
    {
        if (interactionIndicator != null)
        {
            interactionIndicator.SetActive(true);
        }
        
        // Notificar UI para mostrar botão de interação (se modo for manual)
        if (interactionMode == InteractionMode.Manual)
        {
            DialogueManager.Instance?.ShowInteractionPrompt(this);
            
            // Notificar PlayerController que este NPC está próximo
            PlayerController playerController = player?.GetComponent<PlayerController>();
            playerController?.SetNearbyNPC(this);
        }
    }
    
    void HideInteractionIndicator()
    {
        if (interactionIndicator != null)
        {
            interactionIndicator.SetActive(false);
        }
        
        // Esconder botão de interação
        if (interactionMode == InteractionMode.Manual)
        {
            DialogueManager.Instance?.HideInteractionPrompt();
            
            // Notificar PlayerController que este NPC não está mais próximo
            PlayerController playerController = player?.GetComponent<PlayerController>();
            playerController?.ClearNearbyNPC(this);
        }
    }
    
    void AnimateIndicator()
    {
        float newY = indicatorStartPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;
        interactionIndicator.transform.localPosition = new Vector3(
            indicatorStartPosition.x,
            newY,
            indicatorStartPosition.z
        );
    }
    
    public void StartDialogue()
    {
        if (dialogueData == null)
        {
            Debug.LogWarning($"[NPCController] No dialogue data assigned to {npcName}!");
            return;
        }
        
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("[NPCController] DialogueManager not found in scene!");
            return;
        }
        
        // Tocar som de interação
        if (interactionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(interactionSound);
        }
        
        OnInteractionStart?.Invoke();
        
        // Determinar qual diálogo usar (primeira vez ou repetição)
        DialogueData currentDialogue = GetCurrentDialogue();
        
        DialogueManager.Instance.StartDialogue(currentDialogue, this);
        
        // Marcar como interagido
        if (!hasInteracted)
        {
            hasInteracted = true;
        }
    }
    
    DialogueData GetCurrentDialogue()
    {
        // Se já interagiu e tem diálogo alternativo, usar ele
        if (hasInteracted && !dialogueData.isRepeatable && dialogueData.repeatDialogue != null)
        {
            return dialogueData.repeatDialogue;
        }
        
        return dialogueData;
    }
    
    public void OnDialogueEnd()
    {
        OnInteractionEnd?.Invoke();
    }
    
    // Método público para ser chamado por botão UI
    public void TryInteract()
    {
        if (playerInRange)
        {
            StartDialogue();
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Mostrar range de interação
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
        
        // Mostrar indicação visual do tipo de interação
        Gizmos.color = interactionMode == InteractionMode.Automatic ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange * 0.8f);
    }
}

public enum InteractionMode
{
    Manual,     // Requer botão/input para interagir
    Automatic   // Inicia diálogo automaticamente por proximidade
}