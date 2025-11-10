# Tutorial: Criando Novos Itens

## 🎯 Objetivo
Aprender a criar novos itens no sistema de inventário, desde o ScriptableObject até os prefabs coletáveis.

---

## 📋 Pré-requisitos
- ✅ Unity 2022.3+ instalado
- ✅ Projeto com sistema de inventário configurado
- ✅ Pasta `Assets/Resources/Items/` criada
- ✅ InventoryManager na cena

---

## 🚀 Passo 1: Criando o Item ScriptableObject

### **1.1 Navegar para a pasta correta**
1. No Project window, vá para `Assets/Resources/Items/`
2. ⚠️ **IMPORTANTE:** O item DEVE estar na pasta Resources

### **1.2 Criar o ScriptableObject**
1. **Clique direito** na pasta Items
2. Selecione **Create → Inventory → Item**
3. Renomeie o arquivo para o nome do seu item (ex: `EspadaDeFerro`)

### **1.3 Configurar propriedades básicas**

```yaml
# Exemplo: Espada de Ferro
Item Name: "Espada de Ferro"
Description: "Uma espada básica feita de ferro. +10 de ataque."
Icon: [Selecionar sprite do ícone]
Item Type: Weapon
Is Stackable: false  # Armas não empilham
Max Stack Size: 1
Value: 150
Rarity: Common
```

```yaml
# Exemplo: Poção de Vida
Item Name: "Poção de Vida"
Description: "Restaura 50 HP quando consumida."
Icon: [Selecionar sprite da poção]
Item Type: Consumable
Is Stackable: true   # Poções empilham
Max Stack Size: 10
Value: 25
Rarity: Common
```

```yaml
# Exemplo: Diamante
Item Name: "Diamante"
Description: "Uma pedra preciosa extremamente rara e valiosa."
Icon: [Selecionar sprite do diamante]
Item Type: Material
Is Stackable: true
Max Stack Size: 64
Value: 1000
Rarity: Legendary
```

---

## 🎨 Passo 2: Preparando Assets Visuais

### **2.1 Sprites necessários**
Para cada item você precisa de:
- **Icon (32x32 ou 64x64):** Para UI do inventário
- **World Sprite (opcional):** Como aparece no chão (pode ser maior)

### **2.2 Configuração de sprites**
1. Importe as imagens para o projeto
2. Configure **Texture Type:** Sprite (2D and UI)
3. Configure **Pixels Per Unit:** 32 ou 64 (dependendo do tamanho)
4. Configure **Filter Mode:** Point (no filter) para pixel art

---

## 🔧 Passo 3: Criando Prefab Coletável

### **3.1 Criar GameObject base**
1. **Hierarchy → Create Empty**
2. Renomear para nome do item (ex: "EspadaDeFerro_Pickup")
3. Position: (0, 0, 0)

### **3.2 Adicionar SpriteRenderer**
1. **Add Component → Rendering → Sprite Renderer**
2. Configurar:
   ```yaml
   Sprite: [Sprite do item no mundo]
   Color: White (255, 255, 255, 255)
   Sorting Layer: Default
   Order in Layer: 10  # Para ficar em cima do chão
   ```

### **3.3 Adicionar Collider2D**
1. **Add Component → Physics 2D → Circle Collider 2D** (ou Box Collider 2D)
2. ⚠️ **IMPORTANTE:** Marcar **Is Trigger = true**
3. Ajustar o tamanho para cobrir o sprite

### **3.4 Adicionar ItemPickup**
1. **Add Component → Scripts → Item Pickup**
2. Configurar:
   ```yaml
   Item: [Selecionar o ScriptableObject criado]
   Quantity: 1
   Pickup Range: 1.5
   Pickup Mode: Trigger
   Destroy On Pickup: true
   Enable Bobbing: true
   Bob Speed: 2
   Bob Height: 0.5
   ```

### **3.5 Adicionar AudioSource (Opcional)**
1. **Add Component → Audio → Audio Source**
2. Configurar:
   ```yaml
   Play On Awake: false
   Volume: 0.7
   ```
3. No ItemPickup, configurar **Pickup Sound**

### **3.6 Salvar como Prefab**
1. Arrastar o GameObject da Hierarchy para a pasta `Assets/Prefabs/Inventory/`
2. Deletar o GameObject da Hierarchy

---

## 📝 Passo 4: Testando o Item

### **4.1 Colocar na cena**
1. Arrastar o prefab da pasta Prefabs para a Scene
2. Posicionar próximo ao player

### **4.2 Testar coleta**
1. **Play** no editor
2. Mover o player para tocar o item
3. Verificar console para mensagens de sucesso:
   ```
   "Added 1 EspadaDeFerro(s) to inventory. Total: 1"
   ```

### **4.3 Testar persistência**
1. Coletar alguns itens
2. **Stop** o jogo
3. **Play** novamente
4. Verificar se itens foram carregados

---

## 📚 Exemplos Completos

### **Exemplo A: Mineral Básico**

**1. ScriptableObject (MinerioDeCobre.asset)**
```yaml
Item Name: "Minério de Cobre"
Description: "Metal básico usado em crafting."
Item Type: Material
Is Stackable: true
Max Stack Size: 99
Value: 5
Rarity: Common
```

**2. Prefab (MinerioCobre_Pickup.prefab)**
- SpriteRenderer com sprite de minério
- CircleCollider2D (IsTrigger = true, Radius = 0.4)
- ItemPickup configurado

### **Exemplo B: Arma Rara**

**1. ScriptableObject (EspadaFlamejar.asset)**
```yaml
Item Name: "Espada Flamejante"
Description: "Espada mágica que causa dano de fogo."
Item Type: Weapon
Is Stackable: false
Max Stack Size: 1
Value: 500
Rarity: Epic
```

**2. Prefab (EspadaFlamejar_Pickup.prefab)**
- SpriteRenderer com efeito de fogo
- BoxCollider2D maior
- ItemPickup com som especial
- Particle System para efeito visual

### **Exemplo C: Consumível Stackable**

**1. ScriptableObject (PocaoMana.asset)**
```yaml
Item Name: "Poção de Mana"
Description: "Restaura 30 MP instantaneamente."
Item Type: Consumable
Is Stackable: true
Max Stack Size: 5  # Limit baixo para balanceamento
Value: 40
Rarity: Uncommon
```

---

## 🔄 Workflow para Diferentes Tipos

### **⚔️ Armas**
```yaml
Item Type: Weapon
Is Stackable: false
Max Stack Size: 1
Value: Alto
Rarity: Varia
```

### **🛡️ Armaduras**
```yaml
Item Type: Armor
Is Stackable: false
Max Stack Size: 1
Value: Alto
Rarity: Varia
```

### **🧪 Consumíveis**
```yaml
Item Type: Consumable
Is Stackable: true
Max Stack Size: 5-20
Value: Médio
Rarity: Common-Uncommon
```

### **🏗️ Materiais**
```yaml
Item Type: Material
Is Stackable: true
Max Stack Size: 64-99
Value: Baixo-Médio
Rarity: Common-Rare
```

### **📜 Quest Items**
```yaml
Item Type: Quest
Is Stackable: false
Max Stack Size: 1
Value: 0 (não vendável)
Rarity: Qualquer
```

---

## 🚨 Problemas Comuns

### **❌ Item não aparece no inventário**
**Soluções:**
1. Verificar se está na pasta `Resources/Items/`
2. Verificar se ItemPickup tem referência correta
3. Verificar nome do arquivo (case-sensitive)

### **❌ Sprite não aparece**
**Soluções:**
1. Verificar se sprite está configurado no SpriteRenderer
2. Verificar se texture está importada como Sprite
3. Verificar Order in Layer

### **❌ Colisão não funciona**
**Soluções:**
1. Verificar se Collider2D está como **IsTrigger = true**
2. Verificar se Player tem tag "Player"
3. Verificar se Player tem Collider2D

### **❌ Item empilha quando não deveria**
**Solução:**
1. Configurar `Is Stackable = false` no ScriptableObject

---

## 💡 Dicas Avançadas

### **🎨 Organização de Assets**
```
Art/
├── Items/
│   ├── Icons/          # Sprites 32x32 para UI
│   ├── World/          # Sprites maiores para mundo
│   └── Effects/        # Particle effects para itens raros
```

### **📊 Balanceamento**
- **Materiais comuns:** Value 1-10, Stack 99
- **Materiais raros:** Value 50-200, Stack 64
- **Consumíveis:** Value 20-100, Stack 5-20
- **Equipamentos:** Value 100-1000, Stack 1

### **🔊 Audio Guidelines**
- **Materiais:** Som metálico/cristalino
- **Consumíveis:** Som de vidro/líquido
- **Armas:** Som metálico pesado
- **Duração:** 0.3-0.8 segundos

### **✨ Efeitos Visuais**
- **Common:** Sem efeitos especiais
- **Uncommon:** Bobbing speed aumentado
- **Rare:** Particle system sutil
- **Epic:** Efeitos de luz/brilho
- **Legendary:** Múltiplos efeitos + som especial

---

## 🎯 Checklist Final

Antes de considerar o item completo:

- [ ] ✅ ScriptableObject criado e configurado
- [ ] ✅ Icon sprite atribuído
- [ ] ✅ Propriedades balanceadas (stack, value, rarity)
- [ ] ✅ Prefab criado com todos componentes
- [ ] ✅ Collider configurado como trigger
- [ ] ✅ ItemPickup referencia o ScriptableObject correto
- [ ] ✅ Som configurado (se aplicável)
- [ ] ✅ Teste de coleta funcionando
- [ ] ✅ Teste de persistência funcionando
- [ ] ✅ Sem erros no console
- [ ] ✅ Prefab salvo na pasta correta

---

## 📖 Próximos Passos

Após dominar a criação de itens básicos:

1. **[Tutorial: Configurando Pickups](SettingUpPickups.md)** - Scenarios avançados
2. **[Integrando com Inimigos](IntegratingWithEnemies.md)** - Sistema de loot
3. **Sistema de Equipamentos** - Usar itens criados
4. **Sistema de Crafting** - Combinar materiais

---

**Última atualização:** 2024-11-10  
**Dificuldade:** ⭐⭐⭐ Intermediário  
**Tempo estimado:** 15-30 minutos por item