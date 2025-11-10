# Sistema de Inventário - Visão Geral

## 🎯 Arquitetura do Sistema

O sistema de inventário é modular e escalável, composto por 4 componentes principais:

```
📦 SISTEMA DE INVENTÁRIO
├── 🗂️  InventoryManager    (Singleton - Lógica central)
├── 📜  Item               (ScriptableObject - Dados dos itens)  
├── 🏷️  ItemPickup         (MonoBehaviour - Coleta no mundo)
└── 💾  InventoryData      (Classe - Persistência JSON)
```

---

## 🔄 Fluxo de Dados

### **1. Ciclo Completo do Item**
```
[Criar Item SO] → [Configurar Pickup] → [Player Coleta] → [Salvar JSON] → [Carregar no Restart]
```

### **2. Diagrama Detalhado**
```
🎮 Player toca item
    ↓
🏷️  ItemPickup.TryPickup()
    ↓
🗂️  InventoryManager.AddItem()
    ↓
💾 SaveInventory() → JSON no disco
    ↓
📡 OnInventoryChanged.Invoke()
    ↓
🖥️  UI atualizada (futura implementação)
```

---

## 🧩 Componentes Detalhados

### **🗂️ InventoryManager (Singleton)**

**Responsabilidades:**
- ✅ Gerenciar lista de itens em runtime
- ✅ Persistir/carregar dados (JSON)
- ✅ Validar operações (espaço, stack limits)
- ✅ Disparar eventos para UI

**Métodos Principais:**
```csharp
bool AddItem(Item item, int quantity = 1)
bool RemoveItem(Item item, int quantity = 1)
int GetItemQuantity(Item item)
bool HasItem(Item item, int quantity = 1)
Dictionary<Item, int> GetInventory()
void ClearInventory()
```

**Eventos:**
```csharp
UnityEvent<Item, int> OnItemAdded
UnityEvent<Item, int> OnItemRemoved
UnityEvent OnInventoryChanged
```

### **📜 Item (ScriptableObject)**

**Responsabilidades:**
- ✅ Definir propriedades do item
- ✅ Armazenar metadados (nome, descrição, sprite)
- ✅ Configurar comportamento (stackable, max stack)

**Propriedades:**
- `string itemName` - Nome único do item
- `string description` - Descrição para tooltips
- `Sprite icon` - Ícone para UI
- `ItemType itemType` - Categoria (Material, Consumable, etc.)
- `bool isStackable` - Permite empilhamento
- `int maxStackSize` - Máximo por stack
- `int value` - Valor econômico
- `ItemRarity rarity` - Raridade (Common, Rare, etc.)

### **🏷️ ItemPickup (MonoBehaviour)**

**Responsabilidades:**
- ✅ Detectar proximidade/colisão com player
- ✅ Comunicar com InventoryManager
- ✅ Aplicar efeitos visuais/sonoros
- ✅ Gerenciar GameObject após coleta

**Modos de Operação:**
- **Trigger Mode:** Coleta por collider (eficiente)
- **Proximity Mode:** Coleta por distância (flexível)

### **💾 InventoryData (Serialização)**

**Responsabilidades:**
- ✅ Converter inventário runtime → JSON
- ✅ Estrutura serializável para persistência

**Estrutura JSON:**
```json
{
  "items": [
    {"itemName": "MinerioDeFerro", "quantity": 5},
    {"itemName": "MinerioDeOuro", "quantity": 2},
    {"itemName": "Madeira", "quantity": 15}
  ]
}
```

---

## 💾 Sistema de Persistência

### **Localização do Arquivo**
- **Path:** `Application.persistentDataPath/inventory.json`
- **macOS:** `~/Library/Application Support/DefaultCompany/TopDown2DCrashCourse/`
- **Windows:** `%USERPROFILE%\AppData\LocalLow\DefaultCompany\TopDown2DCrashCourse\`

### **Momentos de Save**
- ✅ Ao adicionar item
- ✅ Ao remover item
- ✅ Ao pausar aplicação (`OnApplicationPause`)
- ✅ Ao perder foco (`OnApplicationFocus`)
- ✅ Ao destruir InventoryManager (`OnDestroy`)

### **Carregamento**
- ✅ No `Awake()` do InventoryManager
- ✅ Usa `Resources.Load<Item>("Items/" + itemName)`
- ✅ Reconstrói Dictionary<Item, int> em runtime

---

## 🔧 Configuração e Setup

### **1. Dependências Obrigatórias**
```
Assets/Resources/Items/          ← ScriptableObjects aqui
Assets/Scripts/Inventory/        ← Scripts do sistema
Player com tag "Player"         ← Para detecção
```

### **2. Scene Setup**
```
Scene "GameScene"
├── InventoryManager (GameObject)
│   └── InventoryManager.cs
├── GameManager (GameObject)  
│   └── GameManager.cs
└── Player
    └── Tag = "Player"
```

### **3. Workflow de Criação**
1. **Criar Item:** `Resources/Items/MeuItem.asset`
2. **Criar Prefab:** GameObject + SpriteRenderer + Collider2D + ItemPickup
3. **Configurar:** Assignar Item no ItemPickup
4. **Testar:** Colocar na cena e coletar

---

## 🎮 Integração com Outros Sistemas

### **🤺 Sistema de Combate**
```csharp
// Enemy.cs - Quando inimigo morre
void OnDeath()
{
    DropLoot();
}

void DropLoot()
{
    GameObject loot = Instantiate(lootPrefab, transform.position, Quaternion.identity);
    ItemPickup pickup = loot.GetComponent<ItemPickup>();
    pickup.item = GetRandomLoot();
    pickup.quantity = Random.Range(1, 4);
}
```

### **❤️ Sistema de Saúde**
```csharp
// Usar poção de vida
public void UseHealthPotion()
{
    if (InventoryManager.Instance.HasItem(healthPotionItem))
    {
        InventoryManager.Instance.RemoveItem(healthPotionItem, 1);
        healthComponent.Heal(50);
    }
}
```

### **🎯 Sistema de Quests**
```csharp
// Verificar se quest pode ser completada
public bool CanCompleteQuest()
{
    return InventoryManager.Instance.HasItem(requiredItem, requiredQuantity);
}
```

---

## 📊 Performance e Otimização

### **✅ Boas Práticas Implementadas**
- **Singleton Pattern** - Acesso global eficiente
- **Event-driven UI** - Updates apenas quando necessário  
- **JSON Compacto** - Salva apenas nomes e quantidades
- **Trigger Mode** - Evita Update() desnecessário
- **Dictionary lookup** - O(1) para busca de itens

### **🔍 Monitoramento**
```csharp
// Debug info útil
Debug.Log($"Inventory items: {InventoryManager.Instance.GetInventoryItemCount()}");
Debug.Log($"Save path: {Application.persistentDataPath}");
```

### **⚡ Possíveis Otimizações Futuras**
- **Object Pooling** para ItemPickups frequentes
- **Addressables** em vez de Resources
- **Binary serialization** em vez de JSON
- **Async loading** para inventários grandes

---

## 🚨 Limitações Atuais

### **📋 Conhecidas**
- ❌ Sem UI visual (apenas console logs)
- ❌ Sem sistema de equipamentos
- ❌ Sem ordenação/filtros automáticos
- ❌ Dependente de Resources/ (limitação Unity)
- ❌ Save único (sem múltiplos slots)

### **🔮 Planejadas para Futuro**
- ✨ InventoryUI com drag & drop
- ✨ Sistema de equipamentos
- ✨ Crafting system integration
- ✨ Multiple save slots
- ✨ Cloud save support

---

## 🧪 Testing e Debug

### **🔧 Console Commands (Debug)**
```csharp
// Adicionar no InventoryManager para debug
[ContextMenu("Add Debug Item")]
void AddDebugItem()
{
    Item debugItem = Resources.Load<Item>("Items/MinerioDeFerro");
    AddItem(debugItem, 10);
}

[ContextMenu("Clear Inventory")]
void ClearInventoryDebug()
{
    ClearInventory();
}
```

### **📋 Checklist de Testes**
- [ ] Coleta básica funciona
- [ ] Persistência funciona (restart do jogo)
- [ ] Inventário cheio é tratado corretamente
- [ ] Itens não stackable respeitam limite
- [ ] Console logs são claros
- [ ] Performance não degrada com muitos itens

---

## 📚 Próximos Passos

### **🎯 Prioridade Alta**
1. **InventoryUI** - Interface visual
2. **UseItem system** - Consumir itens
3. **Equipment slots** - Equipar armas/armaduras

### **🔧 Prioridade Média**  
4. **Crafting system** - Criar itens
5. **Better persistence** - Múltiplos saves
6. **Performance optimization** - Object pooling

### **🚀 Prioridade Baixa**
7. **Advanced features** - Auto-sort, filters
8. **Cloud integration** - Save na nuvem
9. **Modding support** - External item loading

---

## 📖 Documentação Relacionada

- [📋 ItemPickup Tutorial](ItemPickup.md) - Tutorial completo do sistema de coleta
- [🗂️ InventoryManager API](InventoryManager.md) - Referência da API
- [📜 Item Creation](Item-ScriptableObject.md) - Como criar novos itens
- [💾 JSON Persistence](JSON-Persistence.md) - Detalhes do sistema de save

---

**Última atualização:** 2024-11-10  
**Versão do Sistema:** 1.0  
**Status:** ✅ Produção Ready