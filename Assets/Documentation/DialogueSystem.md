# Sistema de Diálogo com NPCs - Documentação

## Visão Geral

Sistema completo de interação e diálogo com NPCs otimizado para jogos mobile top-down 2D. O sistema utiliza uma abordagem híbrida de **detecção por proximidade** + **confirmação manual** para garantir a melhor experiência mobile.

## Arquitetura

### Componentes Principais

1. **DialogueData** (ScriptableObject) - Armazena conteúdo dos diálogos
2. **NPCController** - Controla comportamento e detecção de proximidade  
3. **DialogueManager** (Singleton) - Gerencia estado global dos diálogos
4. **DialogueUI** - Interface mobile-friendly para exibir diálogos
5. **InteractionPrompt** - Botão de interação posicionado sobre o NPC

### Padrões de Design Utilizados

- **Singleton Pattern**: DialogueManager para acesso global
- **Event-Driven Architecture**: UnityEvents para integração com outros sistemas
- **ScriptableObject Data-Driven**: Diálogos configuráveis via Inspector
- **JSON Persistence**: Progresso salvo automaticamente

## Como Usar

### 1. Criar Diálogo (ScriptableObject)

```
1. Right-click Assets/Resources/Dialogues/
2. Create → Dialogue → Dialogue Data
3. Configure:
   - dialogueID: ID único para persistência
   - npcName: Nome do NPC
   - nodes: Array de nós do diálogo
   - isRepeatable: Se pode repetir após primeira vez
   - repeatDialogue: Diálogo alternativo para repetições
```

### 2. Configurar NPC

```
1. Criar GameObject para NPC
2. Adicionar SpriteRenderer + Collider2D
3. Adicionar NPCController:
   - dialogueData: Assignar DialogueData criado
   - interactionRange: Distância para detecção (padrão: 2f)
   - interactionMode: Manual ou Automatic
   - interactionIndicator: GameObject visual (opcional)
```

### 3. Setup da Cena

**Objetos Obrigatórios na Cena:**
- **DialogueManager** - GameObject vazio com script
- **DialogueUI** - Canvas com interface de diálogo  
- **InteractionPrompt** - Botão flutuante para interação
- **Player** - Com tag \"Player\" para detecção

## Tipos de Interação

### Manual (Recomendado para Mobile)
- Player se aproxima do NPC
- Aparece indicador visual + botão de interação  
- Player toca no botão para iniciar diálogo
- **Melhor UX mobile**: Evita ativações acidentais

### Automático
- Player se aproxima do NPC
- Diálogo inicia automaticamente
- **Melhor para desktop**: Mais fluído

## Estrutura de Diálogos

### DialogueNode
```csharp
- text: Conteúdo do diálogo
- speakerName: Nome do falante  
- characterIcon: Ícone do personagem
- hasChoices: Se tem escolhas para o player
- choices: Array de escolhas possíveis
- triggersEvent: Se dispara evento customizado
- eventName: Nome do evento para integração
- nextNodeIndex: Próximo nó (-1 = fim)
```

### DialogueChoice  
```csharp
- choiceText: Texto da escolha
- nextNodeIndex: Próximo nó desta escolha
- requiresItem: Se requer item específico
- requiredItemName: Nome do item requerido
- givesItem: Se dá item ao player
- itemToGive: Nome do item a ser dado
```

## Recursos Mobile-Friendly

### Interface
- **Touch targets grandes**: Botões ≥60px para fácil toque
- **Typewriter effect**: Texto animado com skip por toque
- **Auto-positioning**: Prompt posicionado automaticamente sobre NPC
- **Screen clamping**: Interface sempre visível na tela

### Performance
- **Proximity detection**: Eficiente para múltiplos NPCs
- **Event-driven**: Apenas processa quando necessário
- **Automatic cleanup**: Destroy de objetos UI dinâmicos

### Input
- **Hybrid input**: Toque na tela + Input System
- **Skip animations**: Toque rápido completa texto
- **Choice selection**: Botões grandes para seleção

## Integração com Sistemas Existentes

### InventoryManager
```csharp
// Verificar se player tem item para escolha
choice.requiresItem + choice.requiredItemName

// Dar item ao player via escolha
choice.givesItem + choice.itemToGive
```

### GameManager
```csharp
// Pausar jogo durante diálogo
GameState.Dialogue

// Desabilitar movimento do player
playerController.LockMovement()
```

### Event System
```csharp
// Eventos disponíveis do DialogueManager
OnDialogueStart: Quando diálogo inicia
OnDialogueEnd: Quando diálogo termina  
OnNodeChanged: Quando muda nó do diálogo
OnEventTriggered: Para ações customizadas

// Eventos do NPCController  
OnPlayerEnterRange: Player entra no alcance
OnPlayerExitRange: Player sai do alcance
OnInteractionStart: Interação iniciada
OnInteractionEnd: Interação finalizada
```

## Persistência

### Progresso dos Diálogos
- **Auto-save**: Salva automaticamente diálogos completados
- **Arquivo**: `Application.persistentDataPath/dialogue_progress.json`
- **Triggers**: App pause, focus loss, manager destruction

### Localização dos Saves
- **macOS**: `~/Library/Application Support/DefaultCompany/TopDown2DCrashCourse/`
- **Windows**: `%USERPROFILE%\\AppData\\LocalLow\\DefaultCompany\\TopDown2DCrashCourse\\`

## Exemplo de Uso Prático

### Diálogo Simples
```
Node 0: \"Olá! Bem-vindo à nossa vila!\"
  → nextNodeIndex: 1

Node 1: \"Espero que tenha uma boa estadia.\"  
  → nextNodeIndex: -1 (fim)
```

### Diálogo com Escolhas
```
Node 0: \"O que você gostaria de saber?\"
  → hasChoices: true
  → Choice 0: \"Sobre a vila\" → nextNodeIndex: 1
  → Choice 1: \"Sobre você\" → nextNodeIndex: 2
  → Choice 2: \"Tchau\" → nextNodeIndex: -1

Node 1: \"Nossa vila é muito antiga...\"
  → nextNodeIndex: -1

Node 2: \"Sou o prefeito desta vila...\"
  → nextNodeIndex: -1
```

### Diálogo com Item  
```
Node 0: \"Você tem uma poção?\"
  → hasChoices: true  
  → Choice 0: \"Sim\" (requiresItem: \"Potion\") → nextNodeIndex: 1
  → Choice 1: \"Não\" → nextNodeIndex: 2

Node 1: \"Obrigado! Aqui está sua recompensa.\"
  → givesItem: \"Gold\"
  → itemQuantity: 50
  → nextNodeIndex: -1
```

## Debugging e Troubleshooting

### Logs do Sistema
- `[NPCController]`: Detecção de proximidade e interações  
- `[DialogueManager]`: Estado dos diálogos e persistência
- `[DialogueUI]`: Exibição e transições da interface

### Problemas Comuns
1. **Player não detectado**: Verificar tag \"Player\" no GameObject
2. **Diálogo não salva**: Verificar se DialogueData está em Resources/
3. **UI não aparece**: Verificar referências no DialogueManager
4. **Botão não funciona**: Verificar se Canvas tem GraphicRaycaster

## Performance Tips

### Otimizações
- Use `FindFirstObjectByType` ao invés de `FindObjectOfType` (Unity 2023+)
- Configure `interactionRange` apropriadamente para evitar checks desnecessários
- Desabilite `enableBobbing` se houver muitos NPCs na cena
- Use object pooling para `choiceButtonPrefab` se houver muitas escolhas

### Métricas Recomendadas
- **Interaction Range**: 1.5-3.0 units para top-down 2D
- **Button Size**: ≥60px para touch targets  
- **Typewriter Speed**: 0.03-0.08s para boa legibilidade
- **Fade Duration**: 0.2-0.4s para transições suaves

Esta documentação cobre todos os aspectos do sistema de diálogo implementado, seguindo as melhores práticas para desenvolvimento mobile Unity.