# ItemPickup - Tutorial Completo

## 📋 Visão Geral

O **ItemPickup** é um componente universal que torna qualquer GameObject coletável pelo jogador. Ele serve como ponte entre objetos no mundo e o sistema de inventário.

## 🎯 Para Que Serve

### Casos de Uso Principais:
- **Itens estáticos** no mapa (minerais, plantas, baús)
- **Drops dinâmicos** de inimigos
- **Loot de objetos quebráveis** (caixotes, pedras)
- **Recompensas de quests**

---

## 🔧 Configuração Básica

### 1. **Componentes Necessários**

Para um item coletável básico, você precisa:

```
GameObject "MeuItem"
├── SpriteRenderer (visual do item)
├── Collider2D (detecção)
├── ItemPickup (script)
└── AudioSource (opcional, para som)
```

### 2. **Setup Passo a Passo**

1. **Criar GameObject**
   - Crie um GameObject vazio
   - Renomeie para o nome do item (ex: "MinerioDeFerro")

2. **Adicionar Visual**
   - Adicione componente `SpriteRenderer`
   - Configure o sprite do item

3. **Adicionar Colisão**
   - Adicione `Collider2D` (CircleCollider2D ou BoxCollider2D)
   - ✅ **IMPORTANTE:** Marque `IsTrigger = true`

4. **Adicionar Script**
   - Adicione o componente `ItemPickup`
   - Configure as propriedades (veja seção abaixo)

---

## ⚙️ Propriedades Detalhadas

### **📦 Item Pickup Settings**

#### `Item item`
- **Tipo:** Item ScriptableObject
- **Descrição:** Referência ao ScriptableObject do item
- **Localização:** Deve estar em `Resources/Items/`
- **Exemplo:** MinerioDeFerro.asset

#### `int quantity`
- **Padrão:** 1
- **Descrição:** Quantidade de itens a ser adicionada ao inventário
- **Exemplo:** 3 (para dar 3 unidades de uma vez)

#### `float pickupRange`
- **Padrão:** 1.5f
- **Descrição:** Distância máxima para coleta automática
- **Usado apenas** em Proximity Mode

#### `PickupMode pickupMode`
- **Padrão:** Trigger
- **Opções:**
  - **Trigger:** Coleta ao tocar (requer Collider2D)
  - **Proximity:** Coleta por proximidade (usa pickupRange)

### **🎨 Visual Effects**

#### `bool destroyOnPickup`
- **Padrão:** true
- **Descrição:** Se true, destrói o GameObject após coleta
- **Se false:** Apenas desativa o GameObject

#### `bool enableBobbing`
- **Padrão:** true
- **Descrição:** Ativa movimento de balanço vertical

#### `float bobSpeed`
- **Padrão:** 2f
- **Descrição:** Velocidade da animação de balanço

#### `float bobHeight`
- **Padrão:** 0.5f
- **Descrição:** Altura máxima do movimento de balanço

### **🔊 Audio**

#### `AudioClip pickupSound`
- **Descrição:** Som reproduzido ao coletar
- **Requer:** AudioSource no GameObject

---

## 📖 Exemplos Práticos

### **Exemplo 1: Minério Básico (Trigger Mode)**

```csharp
// Configuração no Inspector:
// Item = MinerioDeFerro.asset
// Quantity = 1
// Pickup Mode = Trigger
// Destroy On Pickup = true
// Enable Bobbing = true
```

**Componentes:**
- SpriteRenderer (sprite do minério)
- CircleCollider2D (IsTrigger = true, Radius = 0.5)
- ItemPickup (configurado acima)

### **Exemplo 2: Drop de Inimigo (Código)**

```csharp
public class Enemy : MonoBehaviour
{
    public GameObject lootPrefab;
    public Item[] possibleLoot;
    
    void OnDeath()
    {
        // Instancia o loot
        GameObject drop = Instantiate(lootPrefab, transform.position, Quaternion.identity);
        
        // Configura o item aleatório
        ItemPickup pickup = drop.GetComponent<ItemPickup>();
        pickup.item = possibleLoot[Random.Range(0, possibleLoot.Length)];
        pickup.quantity = Random.Range(1, 4);
    }
}
```

### **Exemplo 3: Baú de Tesouro (Proximity Mode)**

```csharp
// Configuração no Inspector:
// Item = Ouro.asset
// Quantity = 50
// Pickup Mode = Proximity
// Pickup Range = 2.0f
// Destroy On Pickup = false (para reutilizar)
```

### **Exemplo 4: Árvore que Dropa Madeira**

```csharp
public class Tree : MonoBehaviour
{
    public GameObject woodDropPrefab;
    public int woodAmount = 3;
    
    public void OnChopped()
    {
        for (int i = 0; i < woodAmount; i++)
        {
            Vector3 randomPos = transform.position + 
                               Random.insideUnitSphere * 2f;
            randomPos.z = 0; // Manter em 2D
            
            GameObject wood = Instantiate(woodDropPrefab, randomPos, Quaternion.identity);
            // ItemPickup já configurado no prefab
        }
    }
}
```

---

## 🎮 Modos de Coleta

### **🎯 Trigger Mode (Recomendado)**

**Vantagens:**
- ✅ Mais eficiente (não usa Update)
- ✅ Detecção precisa
- ✅ Funciona bem com física

**Requisitos:**
- Collider2D com IsTrigger = true
- Player com Collider2D
- Player com tag "Player"

**Quando usar:**
- Itens pequenos
- Drops de combate
- Items de precisão

### **📍 Proximity Mode**

**Vantagens:**
- ✅ Coleta automática por proximidade
- ✅ Funciona sem colliders
- ✅ Visual feedback com Gizmo

**Desvantagens:**
- ❌ Usa Update (menos eficiente)
- ❌ Requer FindGameObjectWithTag

**Quando usar:**
- Itens grandes (baús)
- Coleta automática
- Debugging/visualização

---

## 🔄 Fluxo de Funcionamento

```
1. Player se aproxima/toca no item
   ↓
2. ItemPickup detecta player
   ↓
3. TryPickup() é chamado
   ↓
4. InventoryManager.AddItem() tenta adicionar
   ↓
5a. Sucesso → Item coletado, GameObject destruído/desativado
5b. Falha → Log de erro (inventário cheio, etc.)
```

---

## 🚨 Problemas Comuns

### **❌ Item não é coletado**

**Possíveis causas:**
1. Player não tem tag "Player"
2. Collider2D não está como Trigger
3. Item ScriptableObject não configurado
4. Item não está em Resources/Items/

### **❌ Som não reproduz**

**Soluções:**
1. Adicionar AudioSource ao GameObject
2. Configurar AudioClip na propriedade pickupSound
3. Verificar volume do AudioSource

### **❌ Performance baixa**

**Se usando Proximity Mode:**
- Use apenas quando necessário
- Considere usar Trigger Mode
- Reduza pickupRange se possível

### **❌ Item coletado duplicado**

**Causa:** TryPickup chamado duas vezes
**Solução:** Verifique se não há dois modos ativos simultaneamente

---

## 🎯 Melhores Práticas

### **1. Prefabs Reutilizáveis**
```
Prefabs/Inventory/
├── GenericPickup.prefab    (base configurável)
├── MinerioDeFerro.prefab   (item específico)
├── MinerioOuro.prefab      (item específico)
└── LootDrop.prefab         (para enemies)
```

### **2. Performance**
- Use **Trigger Mode** sempre que possível
- **Object Pooling** para drops frequentes
- **Combine meshes** para itens estáticos

### **3. Organização**
- Mantenha prefabs organizados por categoria
- Use nomes descritivos
- Configure valores padrão sensatos

### **4. Audio**
- Use sons curtos (< 1 segundo)
- Volume entre 0.5-0.8
- Considere variações de pitch

---

## 🔍 Debug e Testes

### **Gizmos**
- Proximity Mode mostra círculo amarelo no Scene view
- Útil para visualizar área de coleta

### **Console Logs**
```
"Added 1 MinerioDeFerro(s) to inventory. Total: 5"
"Could not pickup Madeira: inventory full"
"ItemPickup on Rock has no item assigned!"
```

### **Testes Recomendados**
1. ✅ Coleta básica funciona
2. ✅ Inventário cheio (deve falhar graciosamente)
3. ✅ Item não stackable
4. ✅ Persistência após restart
5. ✅ Audio/visual feedback

---

## 🚀 Integração com Outros Sistemas

### **Com Enemy System**
```csharp
// No Enemy.cs
void OnDeath()
{
    DropLoot();
}

void DropLoot()
{
    if (lootTable.Length > 0)
    {
        GameObject loot = Instantiate(lootPrefab, transform.position, Quaternion.identity);
        ItemPickup pickup = loot.GetComponent<ItemPickup>();
        pickup.item = lootTable[Random.Range(0, lootTable.Length)];
    }
}
```

### **Com Breakable Objects**
```csharp
// No BreakableObject.cs
public void Break()
{
    SpawnLoot();
    Destroy(gameObject);
}
```

### **Com Quest System**
```csharp
// Quando quest é completa
public void OnQuestComplete()
{
    GameObject reward = Instantiate(questRewardPrefab, player.position, Quaternion.identity);
    // ItemPickup configurado no prefab
}
```

---

## 📚 Arquivos Relacionados

- [InventoryManager.md](InventoryManager.md) - Sistema principal de inventário
- [Item.md](Item-ScriptableObject.md) - Criação de itens
- [InventoryOverview.md](InventoryOverview.md) - Visão geral do sistema

---

**Última atualização:** 2024-11-10  
**Versão:** 1.0  
**Compatibilidade:** Unity 2022.3+