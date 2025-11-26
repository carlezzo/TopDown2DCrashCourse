# Quest System - Guia de Configuração

## ✅ Sistema Implementado

Todos os scripts do sistema de quests foram criados com sucesso! Agora você precisa configurar os assets no Unity Editor.

---

## 📋 Passo 1: Criar Item "Wild Berry"

### No Unity Editor:

1. **Navegue até:** `Assets/Resources/Items/`
2. **Click direito** → `Create` → `Inventory` → `Item`
3. **Renomeie para:** `WildBerry`
4. **Configure os campos:**
   ```
   Item Name: Wild Berry
   Description: Fruta silvestre nutritiva encontrada em arbustos. Restaura um pouco de fome.
   Item Type: Consumable
   Is Stackable: ✓ (checked)
   Max Stack Size: 99
   Value: 5
   Rarity: Common
   Icon: [Escolha um sprite de fruta - pode ser placeholder por enquanto]
   ```

5. **Salve** (Ctrl+S)

---

## 📋 Passo 2: Criar Quest "Awakening Hunger"

### No Unity Editor:

1. **Navegue até:** `Assets/Resources/Quests/`
2. **Click direito** → `Create` → `Quest System` → `Quest`
3. **Renomeie para:** `quest_001_awakening_hunger`
4. **Configure os campos:**

   **Quest Identity:**
   ```
   Quest ID: quest_001_awakening_hunger
   Quest Name: Despertar da Fome
   Description:
   Você acorda faminto em seu acampamento primitivo.
   Colete frutas silvestres nos arbustos ao redor para se alimentar
   e recuperar suas forças.
   ```

   **Objectives:** (Click no + para adicionar)
   ```
   [0] Objective:
       Objective ID: collect_berries
       Type: CollectItem
       Description: Coletar Frutas Silvestres
       Target ID: Wild Berry
       Required Amount: 3
       Current Amount: 0
   ```

   **Rewards:**
   ```
   (deixe vazio por enquanto - é quest tutorial)
   Items: (lista vazia)
   Experience Points: 0
   Currency: 0
   ```

   **Requirements:**
   ```
   Prerequisite Quest IDs: (lista vazia - primeira quest)
   ```

5. **Salve** (Ctrl+S)

---

## 📋 Passo 3: Criar Prefab "BerryBushPickup"

### No Unity Editor:

1. **Na Hierarchy**, click direito → `Create Empty`
2. **Renomeie para:** `BerryBushPickup`

3. **Adicione SpriteRenderer:**
   - Click no GameObject → `Add Component` → `Sprite Renderer`
   - **Sprite:** Escolha sprite de arbusto/bush (pode ser placeholder)
   - **Color:** Verde ou marrom
   - **Sorting Layer:** Default
   - **Order in Layer:** 0

4. **Adicione CircleCollider2D:**
   - `Add Component` → `Circle Collider 2D`
   - **Is Trigger:** ✓ (checked)
   - **Radius:** 0.5

5. **Adicione ItemPickup component:**
   - `Add Component` → `Item Pickup` (script já existente)
   - **Item:** Arraste o `WildBerry` ScriptableObject
   - **Pickup Mode:** Trigger
   - **Quantity:** 1
   - **Auto Destroy On Pickup:** ✓ (checked)
   - **Bob Animation:** ✓ (checked - opcional)
   - **Play Sound:** ✓ (se tiver AudioSource)

6. **Salve como Prefab:**
   - Arraste o GameObject da Hierarchy para `Assets/Prefabs/Inventory/`
   - Delete o GameObject da Hierarchy

---

## 📋 Passo 4: Criar UI Prefabs

### 4.1 ObjectiveDisplay Prefab

1. **Na Hierarchy**, click direito no Canvas → `UI` → `Panel`
2. **Renomeie para:** `ObjectiveDisplay`
3. **Configure o Panel:**
   - **Width:** 300
   - **Height:** 40
   - **Image:** Transparente ou semi-transparente (alpha baixo)

4. **Adicione HorizontalLayoutGroup:**
   - `Add Component` → `Horizontal Layout Group`
   - **Padding:** Left 10, Right 10
   - **Spacing:** 10
   - **Child Alignment:** Middle Left
   - **Child Force Expand:** Width ✓, Height ✗

5. **Adicione Checkmark (filho):**
   - Click direito no ObjectiveDisplay → `UI` → `Image`
   - **Renomeie para:** `Checkmark`
   - **Width:** 24, **Height:** 24
   - **Image:** Sprite de checkmark/✓ (ou símbolo qualquer)
   - **Color:** Verde
   - **Active:** ✗ (desabilitado por padrão)

6. **Adicione Text (filho):**
   - Click direito no ObjectiveDisplay → `UI` → `Text - TextMeshPro`
   - **Renomeie para:** `ObjectiveText`
   - **Text:** "Objective description here"
   - **Font Size:** 16-18
   - **Color:** Branco
   - **Alignment:** Left + Middle
   - **Overflow:** Ellipsis ou Wrap

7. **Adicione component ObjectiveDisplayUI:**
   - Selecione o ObjectiveDisplay
   - `Add Component` → `Objective Display UI`
   - **Objective Text:** Arraste o ObjectiveText
   - **Checkmark Icon:** Arraste o Checkmark Image
   - **Incomplete Color:** Branco
   - **Completed Color:** Verde

8. **Salve como Prefab:**
   - Arraste para `Assets/Prefabs/Quests/UI/`
   - Delete da Hierarchy

### 4.2 QuestTrackerPanel Prefab

1. **Na Hierarchy**, click direito no Canvas → `UI` → `Panel`
2. **Renomeie para:** `QuestTrackerPanel`
3. **Configure o Panel:**
   - **Anchor:** Top-Left
   - **Position:** X=20, Y=-20 (canto superior esquerdo)
   - **Width:** 350
   - **Height:** Auto (usar Content Size Fitter)
   - **Image:** Background escuro semi-transparente (alpha ~0.7)
   - **Color:** Preto ou marrom escuro

4. **Adicione VerticalLayoutGroup:**
   - `Add Component` → `Vertical Layout Group`
   - **Padding:** Top 15, Bottom 15, Left 15, Right 15
   - **Spacing:** 10
   - **Child Alignment:** Upper Left
   - **Child Force Expand:** Width ✓, Height ✗

5. **Adicione ContentSizeFitter:**
   - `Add Component` → `Content Size Fitter`
   - **Vertical Fit:** Preferred Size

6. **Adicione Quest Name Text (filho):**
   - Click direito no QuestTrackerPanel → `UI` → `Text - TextMeshPro`
   - **Renomeie para:** `QuestNameText`
   - **Text:** "Quest Name"
   - **Font Size:** 20-22
   - **Font Style:** Bold
   - **Color:** Amarelo ou dourado
   - **Alignment:** Center

7. **Adicione Separator (opcional - filho):**
   - Click direito → `UI` → `Image`
   - **Renomeie para:** `Separator`
   - **Height:** 2
   - **Color:** Cinza

8. **Adicione ObjectivesContainer (filho):**
   - Click direito → `Create Empty`
   - **Renomeie para:** `ObjectivesContainer`
   - **Adicione VerticalLayoutGroup:**
     - **Spacing:** 5
     - **Child Force Expand:** Width ✓, Height ✗

9. **Adicione component QuestTrackerUI:**
   - Selecione o QuestTrackerPanel
   - `Add Component` → `Quest Tracker UI`
   - **Tracker Panel:** Auto-detectado (self)
   - **Quest Name Text:** Arraste o QuestNameText
   - **Objectives Container:** Arraste o ObjectivesContainer
   - **Objective Prefab:** Arraste o ObjectiveDisplay prefab criado antes
   - **Hide When No Quest:** ✓ (checked)

10. **Salve como Prefab:**
    - Arraste para `Assets/Prefabs/Quests/UI/`
    - **NÃO delete da Hierarchy** (vai usar na scene)

---

## 📋 Passo 5: Scene Setup

### 5.1 Adicionar QuestManager

1. **Na Hierarchy**, click direito → `Create Empty`
2. **Renomeie para:** `QuestManager`
3. **Adicione component:**
   - `Add Component` → `Quest Manager` (namespace QuestSystem)
   - **Enable Debug Logs:** ✓ (para testar)

### 5.2 Adicionar CollectItemObjectiveTracker

1. **Na Hierarchy**, click direito → `Create Empty`
2. **Renomeie para:** `QuestTracker`
3. **Adicione component:**
   - `Add Component` → `Collect Item Objective Tracker`
   - **Enable Debug Logs:** ✓

### 5.3 Adicionar QuestTrigger (Quest 1 Auto-Start)

1. **Na Hierarchy**, click direito → `Create Empty`
2. **Renomeie para:** `QuestTrigger_Quest001`
3. **Adicione component:**
   - `Add Component` → `Quest Trigger`
   - **Quest To Start:** Arraste `quest_001_awakening_hunger`
   - **Trigger Type:** Auto
   - **Destroy After Trigger:** ✗ (pode manter para debug)
   - **Enable Debug Logs:** ✓

### 5.4 Spawnar Berry Bushes no Mundo

1. **Localize o prefab:** `Assets/Prefabs/Inventory/BerryBushPickup`
2. **Arraste para a Scene** 5-6 vezes
3. **Posicione ao redor do acampamento do player:**
   - Distribua em um raio de ~5-10 unidades
   - Não muito longe, mas também não todos juntos
   - Sugestão: 2 a esquerda, 2 a direita, 1-2 acima/abaixo

4. **Opcional:** Organize em uma pasta na Hierarchy
   - Crie GameObject vazio chamado `BerryBushes`
   - Arraste os 5-6 bushes para dentro dele

### 5.5 Verificar UI Canvas

1. **Localize o Canvas existente** na Hierarchy
2. **O QuestTrackerPanel já deve estar lá** (criado no Passo 4.2)
3. **Posicione visualmente:**
   - Deve estar no canto superior esquerdo
   - Não sobrepondo outros elementos UI importantes
   - Ajuste position se necessário

---

## 🎮 Passo 6: Testar!

### Teste 1: Quest Inicia Automaticamente
1. **Play** na scene
2. **Verifique Console:**
   - "[QuestManager] Initialized and quest progress loaded."
   - "[QuestTrigger] Quest 'Despertar da Fome' iniciada via Auto!"
   - "[QuestManager] Quest iniciada: Despertar da Fome"
3. **Verifique HUD:**
   - QuestTrackerPanel deve aparecer
   - Mostrar "Despertar da Fome"
   - Mostrar "☐ Coletar Frutas Silvestres: 0/3"

### Teste 2: Coletar Frutas
1. **Mova o player** até um arbusto
2. **Colidir com ele** (trigger automático)
3. **Verifique Console:**
   - "[InventoryManager] Added 1x Wild Berry to inventory"
   - "[CollectItemObjectiveTracker] Item 'Wild Berry' coletado. Progresso: 1/3"
   - "[QuestManager] Objetivo atualizado: Coletar Frutas Silvestres (1/3)"
4. **Verifique HUD:**
   - Objetivo deve atualizar: "☐ Coletar Frutas Silvestres: 1/3"

### Teste 3: Completar Quest
1. **Colete mais 2 frutas** (total 3)
2. **Verifique Console:**
   - "[QuestManager] Objetivo atualizado: Coletar Frutas Silvestres (3/3)"
   - "[QuestManager] Quest completada: Despertar da Fome"
3. **Verifique HUD:**
   - Último objetivo: "✓ Coletar Frutas Silvestres: 3/3" (verde, strikethrough)
   - QuestTrackerPanel deve desaparecer (hideWhenNoQuest = true)

### Teste 4: Save/Load
1. **Colete 2 frutas** (não complete a quest)
2. **Feche o jogo** (ou pause)
3. **Verifique arquivo:** `~/Library/Application Support/.../quest_progress.json`
4. **Reabra o jogo**
5. **Verifique Console:**
   - "[QuestManager] Quest restaurada: Despertar da Fome"
6. **Verifique HUD:**
   - Deve mostrar "☐ Coletar Frutas Silvestres: 2/3"

---

## 🐛 Troubleshooting

### Quest não inicia:
- Verifique se QuestManager está na scene
- Verifique se QuestTrigger tem o quest configurado
- Verifique Console por erros

### Progresso não atualiza:
- Verifique se CollectItemObjectiveTracker está na scene
- Verifique se o targetID no objetivo bate com itemName EXATO
- Verifique se InventoryManager está ativo

### UI não aparece:
- Verifique se QuestTrackerPanel está no Canvas
- Verifique se ObjectivePrefab está configurado
- Verifique se Canvas tem EventSystem

### SaveLoad não funciona:
- Verifique Console por erros de serialização
- Verifique se quest está em `Resources/Quests/`
- Tente menu: QuestManager → Clear Quest Progress

---

## ✅ Próximos Passos

Com Quest 1 funcionando, você pode:

1. **Criar Quest 2:** Seguir mesmo processo com novos objetivos
2. **Implementar QuestSequenceController:** Auto-start de quest 2 após quest 1
3. **Adicionar Quest Log UI:** Menu completo de quests
4. **Adicionar Notificações:** Pop-ups quando quest completa
5. **Expandir tipos de objetivos:** TalkToNPC, DefeatEnemy, etc.

---

**Sistema de Quests MVP Completo!** 🎉
