# Sistema de Drop de Itens

## Visão Geral

Sistema de drop de itens baseado em **ScriptableObjects** que permite configurar quais itens inimigos dropam ao morrer. Totalmente integrado com o sistema de inventário existente.

## Arquitetura

### Componentes Principais

1. **[DropTable.cs](../Scripts/Inventory/DropTable.cs)** - ScriptableObject
   - Define lista de itens a serem dropados
   - Reutilizável entre múltiplos inimigos
   - Configuração no Inspector

2. **[DropOnDeath.cs](../Scripts/Inventory/DropOnDeath.cs)** - MonoBehaviour
   - Componente adicionado aos inimigos
   - Escuta evento `HealthComponent.OnDeath`
   - Instancia itens ao morrer

### Dependências

- **Requerido:** `HealthComponent` no mesmo GameObject
- **Integração:** `ItemPickup` (sistema existente)
- **Eventos:** `UnityEvent` do HealthComponent

---

## Como Usar

### 1. Criar DropTable Asset

No Unity Editor:

1. Clique com botão direito em `Assets/Resources/DropTables/`
2. Selecione **Create → Inventory → DropTable**
3. Nomeie o asset (ex: `SlimeDropTable`, `SheepDropTable`)

### 2. Configurar Drops

No Inspector do DropTable:

```
Drops:
  - Element 0
    ├─ Item: [Arraste Item ScriptableObject, ex: Coin]
    ├─ Pickup Prefab: [Arraste prefab, ex: CoinItem]
    └─ Quantity: 3
  - Element 1
    ├─ Item: [Arraste Item ScriptableObject, ex: Sword]
    ├─ Pickup Prefab: [Arraste prefab, ex: Sword]
    └─ Quantity: 1
```

**Campos:**
- **Item**: Referência ao Item ScriptableObject (`Assets/Resources/Items/`)
- **Pickup Prefab**: Prefab com componente `ItemPickup` (`Assets/Prefabs/Inventory/`)
- **Quantity**: Quantidade fixa do item (1-99)

### 3. Adicionar Componentes ao Inimigo

**IMPORTANTE:** O GameObject precisa ter **HealthComponent** primeiro!

#### Se o inimigo NÃO tem HealthComponent:

1. Selecione o prefab do inimigo (ex: Sheep)
2. **Add Component → Health Component**
3. Configure:
   - `Max Health`: 30
   - `Starting Health`: 30
   - `OnDeath`: (evento configurado automaticamente)

#### Adicionar DropOnDeath:

1. **Add Component → Drop On Death**
2. Configure no Inspector:
   - **Drop Table**: Arraste o DropTable asset criado
   - **Drop Radius**: 0.5 (raio de dispersão dos itens)
   - **Debug Mode**: ☑️ (para ver logs durante testes)

---

## Exemplos de Configuração

### Exemplo 1: Slime (Drop Simples)

**SlimeDropTable.asset:**
- 3x Coin (sempre dropa 3 moedas)

**Prefab Slime:**
- ✅ HealthComponent (já possui)
- ✅ DropOnDeath
  - Drop Table: `SlimeDropTable`
  - Drop Radius: `0.5`

**Resultado:** Ao morrer, dropa 3 moedas espalhadas em raio de 0.5 unidades.

---

### Exemplo 2: Sheep (Múltiplos Drops)

**SheepDropTable.asset:**
- 1x Sword
- 5x Coin

**Prefab Sheep:**
- ⚠️ NÃO possui HealthComponent (precisa adicionar!)
- Adicionar **HealthComponent**:
  - Max Health: `20`
  - Starting Health: `20`
- Adicionar **DropOnDeath**:
  - Drop Table: `SheepDropTable`
  - Drop Radius: `0.7`

**Resultado:** Ao morrer, dropa 1 espada + 5 moedas espalhadas.

---

### Exemplo 3: Boss (Loot Especial)

**BossDropTable.asset:**
- 1x Legendary Sword (itemType: Weapon, rarity: Legendary)
- 50x Coin
- 10x Health Potion

**Prefab Boss:**
- ✅ HealthComponent (Max Health: 500)
- ✅ DropOnDeath
  - Drop Table: `BossDropTable`
  - Drop Radius: `1.5` (área maior para loot especial)

---

## Fluxo de Funcionamento

```
1. Inimigo recebe dano (SwordAttack → HealthComponent.TakeDamage())
   ↓
2. Health <= 0
   ↓
3. HealthComponent.OnDeath.Invoke()
   ↓
4. DropOnDeath escuta evento → HandleDeath()
   ↓
5. DropTable.SpawnDrops() instancia todos os itens
   ↓
6. Para cada DropEntry:
   - Instantiate(pickupPrefab) em posição aleatória (dropRadius)
   - Configura ItemPickup.item e ItemPickup.quantity
   ↓
7. Player coleta (ItemPickup → InventoryManager.AddItem())
```

---

## Debugging

### Logs Úteis

Ative `Debug Mode` no DropOnDeath para ver:

```
[DropOnDeath] Sheep configurado para dropar itens ao morrer.
[DropOnDeath] Sheep morreu! Dropando 2 tipo(s) de item(s)...
[DropTable] Dropou 1x Sword em (12.3, 5.7, 0)
[DropTable] Dropou 5x Coin em (12.1, 5.9, 0)
```

### Gizmos no Editor

Quando selecionar um inimigo com DropOnDeath:
- **Círculo amarelo**: Visualiza área de dispersão dos drops (dropRadius)

### Checklist de Problemas Comuns

❌ **Drops não aparecem ao matar inimigo:**
- [ ] HealthComponent está presente no GameObject?
- [ ] DropOnDeath está subscrito ao evento OnDeath? (verificar Start())
- [ ] DropTable está atribuído no Inspector?
- [ ] DropTable possui entradas configuradas?

❌ **Erro "Prefab não possui componente ItemPickup":**
- [ ] Pickup Prefab tem script ItemPickup.cs?
- [ ] Prefab está em `Assets/Prefabs/Inventory/`?

❌ **Sheep não dropa nada:**
- [ ] Sheep possui HealthComponent? (componente necessário!)
- [ ] SwordAttack está causando dano no Sheep? (verificar tag "Enemy")

---

## Expansão Futura

O sistema foi projetado para ser facilmente escalável:

### 1. Sistema de Chances (RNG)

Adicionar campo `dropChance` ao `DropEntry`:

```csharp
[Range(0f, 100f)]
public float dropChance = 100f; // % de chance de dropar

// Em DropTable.SpawnDrops():
if (Random.Range(0f, 100f) <= entry.dropChance)
{
    SpawnItem(entry, position, spreadRadius);
}
```

### 2. Quantidade Variável (Min-Max)

```csharp
public int minQuantity = 1;
public int maxQuantity = 5;

// Em SpawnItem():
pickup.quantity = Random.Range(entry.minQuantity, entry.maxQuantity + 1);
```

### 3. Filtro de Raridade

Usar `Item.rarity` para drops especiais:

```csharp
public ItemRarity minimumRarity = ItemRarity.Common;

// Só dropa itens >= raridade mínima
```

### 4. Drop Pooling (Performance)

Implementar Object Pooling para evitar Instantiate/Destroy:

```csharp
// Usar ObjectPool<GameObject> para reutilizar pickups
GameObject droppedItem = DropPool.Get(entry.pickupPrefab);
```

### 5. Eventos de Drop

Adicionar UnityEvent para VFX/SFX:

```csharp
[Header("Events")]
public UnityEvent<Item, int> OnItemDropped;

// Invocar ao dropar
OnItemDropped?.Invoke(entry.item, entry.quantity);
```

---

## Integração com Sistemas Existentes

### InventoryManager
✅ **Automática** via ItemPickup

Quando player coleta drop:
1. ItemPickup.TryPickup()
2. InventoryManager.AddItem(item, quantity)
3. Persistência JSON automática

### HealthComponent
✅ **Evento OnDeath**

DropOnDeath subscreve ao evento:
```csharp
healthComponent.OnDeath.AddListener(HandleDeath);
```

### ElevationState
✅ **Compatível**

ItemPickup já suporta elevação (Ground/Elevated).

---

## Referências de Arquivos

### Scripts
- `/Assets/Scripts/Inventory/DropTable.cs` - ScriptableObject
- `/Assets/Scripts/Inventory/DropOnDeath.cs` - MonoBehaviour
- `/Assets/Scripts/Inventory/ItemPickup.cs` - Sistema de coleta
- `/Assets/Characters/HealthBar/HealthComponent.cs` - Sistema de vida

### Assets
- `/Assets/Resources/DropTables/` - DropTable ScriptableObjects
- `/Assets/Resources/Items/` - Item ScriptableObjects (Coin, Sword)
- `/Assets/Prefabs/Inventory/` - Pickup prefabs (CoinItem, Sword)

### Scenes
- `Assets/Scenes/SampleScene.unity` - Cena principal para testes

---

## Workflow Completo (Passo a Passo)

### Para Criar Novo Inimigo com Drops:

1. **Criar Item ScriptableObjects** (se não existir)
   - Right-click → Create → Inventory → Item
   - Configurar itemName, itemType, rarity, value

2. **Criar Pickup Prefabs** (se não existir)
   - GameObject + SpriteRenderer + Collider2D(Trigger) + ItemPickup
   - Salvar em `Assets/Prefabs/Inventory/`

3. **Criar DropTable Asset**
   - Right-click → Create → Inventory → DropTable
   - Adicionar entradas com Item + Prefab + Quantity
   - Salvar em `Assets/Resources/DropTables/`

4. **Configurar Inimigo Prefab**
   - Adicionar HealthComponent (se não tiver)
   - Adicionar DropOnDeath
   - Atribuir DropTable criado

5. **Testar no Play Mode**
   - Matar inimigo
   - Verificar drops aparecem
   - Coletar itens
   - Confirmar inventário atualizado

---

## Conclusão

O sistema de drops está pronto para uso em MVP e escalável para funcionalidades avançadas. A arquitetura baseada em ScriptableObjects permite:

- ✅ Reutilização de DropTables entre inimigos
- ✅ Configuração visual no Inspector (sem código)
- ✅ Integração automática com InventoryManager
- ✅ Expansão futura sem refatoração

**Próximos Passos:**
1. Criar mais Item ScriptableObjects (Health Potion, Wood, etc)
2. Criar DropTables para diferentes biomas/áreas
3. Balancear quantidades de drops
4. Adicionar HealthComponent ao Sheep prefab no Unity Editor
