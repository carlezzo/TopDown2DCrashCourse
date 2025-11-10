# TopDown 2D Crash Course - Documentação

## 🎮 Visão Geral do Projeto

Este é um jogo 2D Top-Down desenvolvido em Unity com sistemas modulares e escaláveis. O projeto inclui sistemas de inventário, combate, saúde e movimentação.

## 📁 Navegação da Documentação

### 🔧 Setup e Configuração
- [📋 Setup do Projeto](Setup/ProjectSetup.md)
- [📦 Guia de Dependências](Setup/DependenciesGuide.md)

### ⚙️ Sistemas do Jogo

#### 🎒 Sistema de Inventário
- [📖 Visão Geral do Inventário](Systems/Inventory/InventoryOverview.md)
- [🏷️ ItemPickup - Tutorial Completo](Systems/Inventory/ItemPickup.md)
- [🗂️ InventoryManager](Systems/Inventory/InventoryManager.md)
- [📜 Item ScriptableObject](Systems/Inventory/Item-ScriptableObject.md)
- [💾 Persistência JSON](Systems/Inventory/JSON-Persistence.md)

#### ❤️ Sistema de Saúde
- [🏥 Sistema de Saúde](Systems/Health/HealthSystem.md)
- [🩺 HealthComponent](Systems/Health/HealthComponent.md)

#### 🎮 Sistema de Player
- [🕹️ PlayerController](Systems/Player/PlayerController.md)
- [⚔️ SwordAttack](Systems/Player/SwordAttack.md)

#### 👹 Sistema de Inimigos
- [🧌 Enemy System](Systems/Enemies/Enemy.md)

### 📚 Tutoriais Práticos
- [✨ Criando Novos Itens](Tutorials/CreatingNewItems.md)
- [🎯 Configurando Pickups](Tutorials/SettingUpPickups.md)
- [⚔️ Integrando com Inimigos](Tutorials/IntegratingWithEnemies.md)

### 🔍 API Reference
- [📋 Métodos Públicos](API/PublicMethods.md)
- [📡 Sistema de Eventos](API/Events.md)

### 🔧 Troubleshooting
- [❗ Problemas Comuns](Troubleshooting/CommonIssues.md)
- [❓ FAQ](Troubleshooting/FAQ.md)

## 🚀 Quick Start

### 1. **Setup Básico**
1. Abra o projeto no Unity 2022.3+
2. Configure o InventoryManager na cena principal
3. Configure o GameManager para gerenciamento de estados

### 2. **Testando o Inventário**
1. Crie um item ScriptableObject em `Resources/Items/`
2. Crie um GameObject com ItemPickup
3. Execute o jogo e teste a coleta

### 3. **Verificando Persistência**
1. Colete alguns itens
2. Feche o jogo
3. Reabra - os itens devem estar salvos

## 🏗️ Arquitetura do Projeto

```
Assets/
├── Scripts/
│   ├── Inventory/          # Sistema de inventário
│   ├── Managers/           # Gerenciadores globais
│   ├── Characters/         # Player, inimigos, NPCs
│   └── UI/                 # Interfaces do usuário
├── Resources/
│   └── Items/              # ScriptableObjects de itens
├── Prefabs/
│   └── Inventory/          # Prefabs coletáveis
└── Documentation/          # Esta documentação
```

## 🔧 Sistemas Implementados

- ✅ **Inventário com Persistência JSON**
- ✅ **Sistema de Coleta (ItemPickup)**
- ✅ **GameManager (Estados do Jogo)**
- ✅ **Sistema de Saúde**
- ✅ **Controle de Player**
- ✅ **Sistema de Combate Básico**

## 📝 Convenções de Código

- **PascalCase** para classes e métodos públicos
- **camelCase** para variáveis privadas
- **SCREAMING_SNAKE_CASE** para constantes
- **[Header]** para organizar propriedades no Inspector
- **UnityEvent** para sistema de eventos
- **Singleton Pattern** para managers

## 🎯 Próximos Passos

1. **UI do Inventário** - Interface visual para o sistema
2. **Sistema de Equipamentos** - Armas e armaduras
3. **Sistema de Crafting** - Criação de itens
4. **Sistema de Quests** - Missões e objetivos

## 🤝 Contribuindo

Para adicionar novos sistemas:
1. Siga os padrões arquiteturais existentes
2. Documente todas as funcionalidades
3. Crie tutoriais para outros desenvolvedores
4. Teste a integração com sistemas existentes

---

**Última atualização:** $(date +'%Y-%m-%d')  
**Versão:** 1.0.0  
**Unity Version:** 2022.3+