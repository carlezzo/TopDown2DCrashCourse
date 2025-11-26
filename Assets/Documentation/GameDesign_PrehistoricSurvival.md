# Prehistoric Survival RPG - Game Design Document

## 🎮 Conceito do Jogo

**Gênero:** Top-down 2D RPG de Sobrevivência
**Inspiração:** The Legend of Zelda + Far Cry Primal
**Setting:** Era Pré-histórica

### Premissa
Um homem das cavernas isolado acorda sozinho em um pequeno acampamento primitivo. Sem comunidade, sem habilidades, sem conhecimento sobre fogo ou armas. Ele deve aprender a sobreviver explorando o mundo, coletando recursos e desenvolvendo habilidades primitivas gradualmente.

---

## 📋 Tutorial Quest System - Progressão Inicial

### **Quest 1: Despertar da Fome**
**Objetivo:** Coletar frutas silvestres (3-5 unidades)
- **Mecânica Ensinada:** Movimento e coleta de recursos
- **Recompensa:** Primeira alimentação, introdução ao sistema de fome
- **Localização:** Ao redor do acampamento inicial
- **Feedback:** "Você se alimentou. A fome diminuiu."

### **Quest 2: Exploração do Território**
**Objetivo:** Explorar a área ao redor e marcar pontos de interesse
- **Mecânica Ensinada:** Navegação no mapa, descoberta de áreas
- **Recompensa:** Revelar mini-mapa, descobrir fonte de água
- **Sugestão:** Encontrar 3 pontos de interesse (árvore frutífera, rio, caverna pequena)

### **Quest 3: Pedras Primitivas**
**Objetivo:** Coletar pedras (5-8 unidades)
- **Mecânica Ensinada:** Recursos craftáveis, primeiro item no inventário
- **Recompensa:** Unlock do sistema de crafting básico
- **Localização:** Margens do rio, áreas rochosas

### **Quest 4: A Primeira Caça**
**Objetivo:** Caçar um animal pequeno (coelho/esquilo)
- **Mecânica Ensinada:** Combate básico (ataque com mãos/pedra)
- **Pré-requisito:** Ter coletado pedras
- **Recompensa:** Carne crua, pele de animal
- **Desafio:** Animal foge se você se aproximar muito rápido

### **Quest 5: Gravetos para o Fogo**
**Objetivo:** Coletar gravetos secos (10 unidades)
- **Mecânica Ensinada:** Recursos de crafting avançado
- **Localização:** Sob árvores, áreas florestais
- **Recompensa:** Preparação para criar fogo

### **Quest 6: Dominando o Fogo** 🔥
**Objetivo:** Criar a primeira fogueira
- **Pré-requisitos:** Pedras + Gravetos
- **Mecânica Ensinada:** Sistema de crafting, fogos permanentes
- **Recompensa:**
  - Unlock: Cozinhar carne
  - Unlock: Área de descanso (save point)
  - Buff: Proteção noturna contra predadores
- **Impacto:** Momento épico - cutscene curta mostrando o fogo aceso

### **Quest 7: Ferramentas Primitivas**
**Objetivo:** Craftar uma lança de madeira
- **Pré-requisitos:** Gravetos + Pedras afiadas (crafted de pedras)
- **Mecânica Ensinada:** Crafting de armas, combate com armas
- **Recompensa:** Aumento de dano, alcance de ataque

### **Quest 8: A Grande Caçada**
**Objetivo:** Caçar um animal de médio porte (javali/cervo)
- **Pré-requisitos:** Ter lança equipada
- **Mecânica Ensinada:** Combate avançado, dodge, timing
- **Recompensa:** Carne abundante, peles de qualidade
- **Desafio:** Animal contra-ataca, mecânica de stamina

---

## ✅ Checklist de Implementação (Priorizada)

### **Fase 1: Fundação dos Sistemas** 🏗️
- [ ] **Sistema de Quests**
  - [ ] QuestManager (Singleton)
  - [ ] Quest ScriptableObject (título, descrição, objetivos, recompensas)
  - [ ] UI de Quest Tracker (ativo/completo)
  - [ ] Sistema de objetivos (coletar X, matar Y, ir para Z)

- [ ] **Sistema de Recursos Coletáveis**
  - [ ] ResourcePickup component (frutas, pedras, gravetos)
  - [ ] Diferentes tipos de recursos (enum ResourceType)
  - [ ] Spawners de recursos no mundo
  - [ ] Feedback visual/sonoro de coleta

- [ ] **Adaptação do Inventário**
  - [ ] Adicionar categoria "Resources" aos Items
  - [ ] Stackable items (quantity system já existe)
  - [ ] UI para mostrar recursos principais (pedra, madeira, comida)

### **Fase 2: Sobrevivência Básica** 🍖
- [ ] **Sistema de Fome**
  - [ ] HungerComponent (diminui ao longo do tempo)
  - [ ] Consumir comida para restaurar
  - [ ] UI de barra de fome
  - [ ] Penalidades se fome chegar a zero

- [ ] **Sistema de Crafting**
  - [ ] CraftingManager
  - [ ] Recipe ScriptableObject (input items → output item)
  - [ ] UI de Crafting Menu
  - [ ] Categorias: Ferramentas, Armas, Consumíveis, Estruturas

- [ ] **Itens Craftáveis Iniciais**
  - [ ] Pedra Afiada (2 pedras)
  - [ ] Lança de Madeira (graveto + pedra afiada)
  - [ ] Fogueira (5 gravetos + 2 pedras)
  - [ ] Carne Cozida (carne crua + fogueira ativa)

### **Fase 3: Mundo e Exploração** 🗺️
- [ ] **Acampamento Base**
  - [ ] Tenda inicial (save point)
  - [ ] Local para fogueira central
  - [ ] Área de crafting
  - [ ] Storage básico (baú)

- [ ] **Mundo Aberto Inicial**
  - [ ] Mini-mapa funcional (Fog of War)
  - [ ] Biomas: Floresta, Rio, Área Rochosa
  - [ ] Points of Interest (marcadores no mapa)
  - [ ] Sistema de descoberta (reveal areas)

- [ ] **NPCs Animais**
  - [ ] Coelho (pequeno, foge, 1 hit kill)
  - [ ] Javali (médio, agressivo, 3-4 hits)
  - [ ] AI básica: Idle → Roam → Flee/Attack

### **Fase 4: Combate e Progressão** ⚔️
- [ ] **Sistema de Armas Primitivas**
  - [ ] WeaponComponent (dano, alcance, stamina cost)
  - [ ] Mãos nuas (dano 5)
  - [ ] Pedra (dano 10, arremessável)
  - [ ] Lança (dano 20, alcance médio)

- [ ] **Melhorias no Combate**
  - [ ] Sistema de Stamina (ataques e dodge consomem)
  - [ ] Dodge roll (iframe curto)
  - [ ] Hit feedback (screen shake, slow motion)
  - [ ] Blood particles/effects

- [ ] **Sistema de Habilidades**
  - [ ] Skill tree simples (Hunting, Crafting, Survival)
  - [ ] XP por completar quests e ações
  - [ ] Level up system
  - [ ] Unlocks: novas receitas, maior capacidade de inventário

### **Fase 5: Polish e Game Feel** ✨
- [ ] **Tutorial Integrado**
  - [ ] Quest markers no mundo
  - [ ] Pop-ups contextuais (primeira vez coletando recurso)
  - [ ] Cutscene: primeiro fogo aceso
  - [ ] Narrator/UI hints

- [ ] **Audio e Feedback**
  - [ ] SFX: passos, coleta, crafting, combate
  - [ ] Música adaptativa (calm → combat)
  - [ ] Ambient sounds (vento, pássaros, rio)

- [ ] **UI/UX**
  - [ ] HUD minimalista (vida, fome, stamina)
  - [ ] Quest log completo (ativo + completadas)
  - [ ] Tutorial tooltips
  - [ ] Pause menu com crafting integrado

---

## 🎯 Sistemas Existentes a Aproveitar

### ✅ Já Implementados (Adaptar)
- **InventoryManager** → Base para recursos e itens craftados
- **HealthComponent** → Player e animais
- **SwordAttack** → Base para sistema de armas
- **PlayerController** → Movimento já funcional
- **GameManager** → Estados de jogo e pause
- **ItemPickup** → Base para ResourcePickup

### 🔧 Novos Sistemas Necessários
- **QuestManager** (novo)
- **CraftingManager** (novo)
- **HungerSystem** (novo)
- **StaminaSystem** (novo)
- **AnimalAI** (novo)
- **SkillTreeManager** (novo)

---

## 📊 Progressão do Jogador (Primeira Hora)

```
Minuto 0-5:   Acordar → Quest 1 (frutas) → Aprender movimento/coleta
Minuto 5-10:  Quest 2 (explorar) → Descobrir mundo, mini-mapa
Minuto 10-15: Quest 3 (pedras) → Introdução ao inventário de recursos
Minuto 15-20: Quest 4 (caçar coelho) → Primeiro combate
Minuto 20-30: Quest 5 (gravetos) + Quest 6 (fogo) → Momento épico!
Minuto 30-45: Quest 7 (craftar lança) → Sistema de crafting completo
Minuto 45-60: Quest 8 (caçar javali) → Primeiro grande desafio
```

**Goal:** Player sente progressão constante, cada quest desbloqueia nova mecânica.

---

## 🎨 Arte e Estética

- **Palette:** Terra, marrom, verde floresta, cinza pedra
- **Player:** Homem das cavernas com pele de animal rudimentar
- **Animais:** Pixel art estilizado, silhuetas reconhecíveis
- **Ambiente:** Tile-based, vegetação densa, elementos 3D fake (perspectiva)
- **UI:** Primitiva, textura de pedra/madeira, ícones desenhados à mão

---

## 🔄 Próximos Passos (Discussão)

1. **Definir escopo MVP:** Até qual quest implementar primeiro?
2. **Priorizar sistema:** Quest, Crafting ou Fome primeiro?
3. **Arte placeholder:** Usar assets gratuitos ou criar sprites básicos?
4. **Balanceamento:** Valores de dano, fome, recursos?
5. **Expansão futura:** Tribos, construção de base, multiplayer?

---

**Documento criado em:** 2025-11-23
**Versão:** 1.0 - Conceito Inicial
