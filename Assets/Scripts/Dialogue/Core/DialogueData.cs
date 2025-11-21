using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Header("Dialogue Info")]
    public string dialogueID;
    public string npcName;
    
    [Header("Dialogue Nodes")]
    public DialogueNode[] nodes;
    
    [Header("Repeatable")]
    public bool isRepeatable = true;
    public DialogueData repeatDialogue; // Diálogo alternativo após primeira conversa
    
    public DialogueNode GetNode(int index)
    {
        if (index >= 0 && index < nodes.Length)
            return nodes[index];
        return null;
    }
    
    public DialogueNode GetFirstNode()
    {
        return GetNode(0);
    }
    
    public bool HasNextNode(int currentIndex)
    {
        return currentIndex + 1 < nodes.Length;
    }
}