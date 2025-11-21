using UnityEngine;

// Script para criar um exemplo de DialogueData automaticamente
// Usado apenas para demonstração - pode ser removido após criar os diálogos via Inspector
[System.Serializable]
public class ExampleDialogue : MonoBehaviour
{
    [Header("Create Example Dialogue")]
    [SerializeField] private bool createExampleDialogue = false;
    
    [ContextMenu("Create Example Dialogue")]
    public void CreateExample()
    {
        if (!createExampleDialogue) return;
        
        // Criar exemplo de DialogueData
        var dialogue = ScriptableObject.CreateInstance<DialogueData>();
        dialogue.dialogueID = "example_villager_001";
        dialogue.npcName = "Aldeão";
        dialogue.isRepeatable = true;
        
        // Criar nós do diálogo
        dialogue.nodes = new DialogueNode[]
        {
            new DialogueNode
            {
                text = "Olá, viajante! Bem-vindo à nossa vila!",
                speakerName = "Aldeão",
                hasChoices = false,
                nextNodeIndex = 1
            },
            new DialogueNode
            {
                text = "O que você gostaria de saber?",
                speakerName = "Aldeão", 
                hasChoices = true,
                choices = new DialogueChoice[]
                {
                    new DialogueChoice
                    {
                        choiceText = "Sobre a vila",
                        nextNodeIndex = 2
                    },
                    new DialogueChoice
                    {
                        choiceText = "Sobre você",
                        nextNodeIndex = 3
                    },
                    new DialogueChoice
                    {
                        choiceText = "Preciso de uma poção",
                        nextNodeIndex = 4,
                        requiresItem = true,
                        requiredItemName = "Gold",
                        requiredItemQuantity = 10
                    },
                    new DialogueChoice
                    {
                        choiceText = "Tchau",
                        nextNodeIndex = -1
                    }
                }
            },
            new DialogueNode
            {
                text = "Nossa vila é muito antiga e pacífica. Vivemos aqui há gerações cultivando a terra.",
                speakerName = "Aldeão",
                hasChoices = false,
                nextNodeIndex = -1
            },
            new DialogueNode
            {
                text = "Sou o prefeito desta vila. Cuido para que todos tenham o que precisam.",
                speakerName = "Aldeão",
                hasChoices = false, 
                nextNodeIndex = -1
            },
            new DialogueNode
            {
                text = "Ah, você tem o dinheiro! Aqui está uma poção de cura. Use com sabedoria!",
                speakerName = "Aldeão",
                hasChoices = false,
                triggersEvent = true,
                eventName = "give_health_potion",
                givesItem = true,
                itemToGive = "HealthPotion",
                itemQuantity = 1,
                nextNodeIndex = -1
            }
        };
        
        // Salvar o ScriptableObject
#if UNITY_EDITOR
        string path = "Assets/Resources/Dialogues/ExampleDialogue.asset";
        UnityEditor.AssetDatabase.CreateAsset(dialogue, path);
        UnityEditor.AssetDatabase.SaveAssets();
        
        Debug.Log($"[ExampleDialogue] Diálogo criado em: {path}");
        Debug.Log("Para usar: 1) Crie um NPC, 2) Adicione NPCController, 3) Assignar este DialogueData");
#endif
    }
    
    [ContextMenu("Create Simple Dialogue")]
    public void CreateSimpleExample()
    {
        // Exemplo mais simples para teste rápido
        var dialogue = ScriptableObject.CreateInstance<DialogueData>();
        dialogue.dialogueID = "simple_test";
        dialogue.npcName = "Test NPC";
        dialogue.isRepeatable = true;
        
        dialogue.nodes = new DialogueNode[]
        {
            new DialogueNode
            {
                text = "Olá! Este é um teste do sistema de diálogo.",
                speakerName = "Test NPC",
                hasChoices = false,
                nextNodeIndex = 1
            },
            new DialogueNode
            {
                text = "O sistema está funcionando perfeitamente!",
                speakerName = "Test NPC",
                hasChoices = false,
                nextNodeIndex = -1
            }
        };
        
#if UNITY_EDITOR
        string path = "Assets/Resources/Dialogues/SimpleTestDialogue.asset";
        UnityEditor.AssetDatabase.CreateAsset(dialogue, path);
        UnityEditor.AssetDatabase.SaveAssets();
        
        Debug.Log($"[ExampleDialogue] Diálogo simples criado em: {path}");
#endif
    }
}