using UnityEngine;

// Script simples para testar se o sistema de diálogo está compilando corretamente
// Se este script aparecer no Add Component, significa que o Unity está conseguindo compilar

public class DialogueSystemTest : MonoBehaviour
{
    [Header("System Test")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private NPCController npcController;
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private InteractionPrompt interactionPrompt;
    [SerializeField] private DialogueData testDialogue;
    
    void Start()
    {
        Debug.Log("[DialogueSystemTest] Sistema de diálogo compilado com sucesso!");
        
        // Verificar se os componentes podem ser encontrados
        if (FindFirstObjectByType<DialogueManager>() != null)
            Debug.Log("[DialogueSystemTest] ✓ DialogueManager encontrado na cena");
        else
            Debug.LogWarning("[DialogueSystemTest] ⚠ DialogueManager não encontrado na cena");
            
        // Verificar se scripts aparecem no Add Component
        Debug.Log("[DialogueSystemTest] Scripts que devem aparecer no Add Component:");
        Debug.Log("- DialogueManager (MonoBehaviour)");
        Debug.Log("- NPCController (MonoBehaviour)"); 
        Debug.Log("- DialogueUI (MonoBehaviour)");
        Debug.Log("- InteractionPrompt (MonoBehaviour)");
        Debug.Log("- DialogueSystemTest (MonoBehaviour)");
        Debug.Log("");
        Debug.Log("Scripts que NÃO aparecem (é normal):");
        Debug.Log("- DialogueData (ScriptableObject)");
        Debug.Log("- DialogueNode (Classe de dados)");
        Debug.Log("- DialogueChoice (Classe de dados)");
    }
    
    [ContextMenu("Test Dialogue Creation")]
    void TestDialogueCreation()
    {
        Debug.Log("[DialogueSystemTest] Testando criação de diálogo...");
        
        var testNode = new DialogueNode
        {
            text = "Teste do sistema de diálogo!",
            speakerName = "Sistema",
            hasChoices = false,
            nextNodeIndex = -1
        };
        
        Debug.Log($"[DialogueSystemTest] ✓ DialogueNode criado: {testNode.text}");
        
        var testChoice = new DialogueChoice
        {
            choiceText = "Teste de escolha",
            nextNodeIndex = -1
        };
        
        Debug.Log($"[DialogueSystemTest] ✓ DialogueChoice criado: {testChoice.choiceText}");
        
        Debug.Log("[DialogueSystemTest] ✓ Todas as classes compilaram corretamente!");
    }
}