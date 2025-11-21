using UnityEngine;

[System.Serializable]
public class DialogueNode
{
    [Header("Dialogue Content")]
    [TextArea(3, 10)]
    public string text;
    
    [Header("Character")]
    public string speakerName;
    public Sprite characterIcon;
    
    [Header("Choices")]
    public bool hasChoices;
    public DialogueChoice[] choices;
    
    [Header("Actions")]
    public bool triggersEvent;
    public string eventName;
    
    [Header("Effects")]
    public bool givesItem;
    public string itemToGive;
    public int itemQuantity = 1;
    
    [Header("Flow Control")]
    public int nextNodeIndex = -1; // -1 = end dialogue
}

[System.Serializable]
public class DialogueChoice
{
    [TextArea(2, 5)]
    public string choiceText;
    public int nextNodeIndex = -1;
    
    [Header("Conditions")]
    public bool requiresItem;
    public string requiredItemName;
    public int requiredItemQuantity = 1;
    
    [Header("Effects")]
    public bool givesItem;
    public string itemToGive;
    public int itemQuantity = 1;
}