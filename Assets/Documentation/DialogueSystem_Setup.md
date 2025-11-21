# Setup Rápido - Sistema de Diálogo

## 🚀 Configuração em 5 Passos

### 1. **Adicionar Managers à Cena**
```
1. Criar GameObject vazio → Renomear para "DialogueManager"
2. Adicionar componente DialogueManager
3. Verificar se GameManager já existe na cena (obrigatório)
```

### 2. **Criar UI do Diálogo**
```
1. Canvas → UI → Canvas (se não existir)
2. Criar filho: GameObject → Renomear para "DialoguePanel" 
3. Adicionar componentes ao DialoguePanel:
   - Image (background)
   - DialogueUI script
4. Criar filhos do DialoguePanel:
   - "SpeakerName" → Text (TextMeshPro)
   - "DialogueText" → Text (TextMeshPro) 
   - "ContinueButton" → Button
   - "ChoicesPanel" → GameObject vazio
   - "CharacterIcon" → Image (opcional)
```

### 3. **Criar Prompt de Interação**
```
1. Canvas → Criar filho: "InteractionPrompt"
2. Adicionar Button + InteractionPrompt script
3. Configurar Button com:
   - Text: "Falar" ou ícone
   - Min size: 80x80px (mobile-friendly)
```

### 4. **Conectar Referencias no DialogueManager**
```
DialogueManager Inspector:
- Dialogue UI: Assignar GameObject com DialogueUI
- Interaction Prompt: Assignar GameObject com InteractionPrompt

DialogueUI Inspector:
- Dialogue Panel: Assignar painel principal
- Speaker Name Text: Assignar texto do nome
- Dialogue Text: Assignar texto principal  
- Continue Button: Assignar botão continuar
- Choices Panel: Assignar container de escolhas
- Choice Button Prefab: Criar prefab de botão para escolhas
```

### 5. **Criar Primeiro NPC**
```
1. Criar GameObject para NPC
2. Adicionar SpriteRenderer + sprite
3. Adicionar Collider2D (para proximidade)
4. Adicionar NPCController script:
   - NPC Name: Nome do personagem
   - Interaction Range: 2.0f (recomendado)
   - Interaction Mode: Manual (recomendado para mobile)
```

---

## 📝 Criar Primeiro Diálogo

### Método 1: Via Script Helper
```
1. Adicionar ExampleDialogue script em qualquer GameObject
2. No Inspector: marcar "Create Example Dialogue" 
3. Clicar botão direito no script → "Create Example Dialogue"
4. Arquivo será criado em Assets/Resources/Dialogues/
5. Assignar ao NPCController
```

### Método 2: Via Inspector
```
1. Right-click Assets/Resources/Dialogues/
2. Create → Dialogue → Dialogue Data
3. Configurar no Inspector:
   - Dialogue ID: ID único (ex: "shopkeeper_001")
   - NPC Name: Nome do falante
   - Is Repeatable: true/false
   - Nodes: Array de nós do diálogo
```

---

## ⚙️ Estrutura Mínima Funcional

### Hierarchy da Cena:
```
Scene
├── GameManager ✓ (já existe)
├── DialogueManager ✓ (novo)
├── Canvas
│   ├── DialoguePanel + DialogueUI
│   └── InteractionPrompt + InteractionPrompt
├── Player ✓ (já existe com tag "Player")
└── TestNPC + NPCController
```

### Assets Structure:
```
Assets/
├── Resources/
│   └── Dialogues/
│       └── ExampleDialogue.asset
├── Scripts/
│   └── Dialogue/
│       ├── Core/ (NPCController, DialogueManager, DialogueData)
│       └── UI/ (DialogueUI, InteractionPrompt)
└── Documentation/
    └── DialogueSystem.md
```

---

## 🔧 Troubleshooting

### Problema: "DialogueUI não encontrado!"
**Solução**: Assignar referência no DialogueManager Inspector

### Problema: "Player not found!"  
**Solução**: Verificar tag "Player" no GameObject do player

### Problema: "Button component not found!"
**Solução**: Adicionar Button component ao InteractionPrompt GameObject

### Problema: NPC não detecta proximidade
**Solução**: 
- Verificar Interaction Range no NPCController
- Confirmar que Player tem tag "Player"
- Testar com diferentes valores de range

### Problema: Diálogo não salva progresso
**Solução**:
- Verificar se DialogueData está em Resources/Dialogues/
- Confirmar se dialogueID está preenchido
- Testar com Application.persistentDataPath

---

## 📱 Otimização Mobile

### UI Settings Recomendadas:
- **Button Size**: Mínimo 80x80px
- **Text Size**: 14-18pt para legibilidade  
- **Fade Duration**: 0.3s para transições suaves
- **Typewriter Speed**: 0.05s entre caracteres

### Performance Tips:
- Usar InteractionMode.Manual para evitar checks constantes
- Configurar Interaction Range adequadamente (1.5-3.0f)
- Desabilitar Bobbing se muitos NPCs na cena

---

## 🎮 Controles

### Teclado/Desktop:
- **Space/Enter**: Avançar diálogo
- **ESC**: Fechar diálogo  
- **Mouse Click**: Interagir/Avançar

### Mobile/Touch:
- **Toque no Botão**: Interagir com NPC
- **Toque na Tela**: Avançar diálogo
- **Botões de Escolha**: Selecionar opções

---

## 🔗 Integração com Outros Sistemas

### Com InventoryManager:
```csharp
// Em DialogueChoice:
requiresItem = true;
requiredItemName = "Gold";
givesItem = true; 
itemToGive = "HealthPotion";
```

### Com GameManager:
```csharp
// Pausar jogo durante diálogo
pauseGameDuringDialogue = true;
// Estado GameState.Dialogue ativo
```

### Com Eventos Customizados:
```csharp
// Em DialogueNode:
triggersEvent = true;
eventName = "open_shop";

// Listener no DialogueManager:
OnEventTriggered.AddListener(HandleCustomEvent);
```

Esta configuração garante um sistema funcional e mobile-optimized para diálogos com NPCs!