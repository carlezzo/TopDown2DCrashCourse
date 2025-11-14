# Archer Enemy - Sistema de Ataque à Distância

Documentação completa do sistema de inimigo arqueiro que atira flechas no player.

## 📁 Arquivos do Sistema

- **Archer.cs** - Script principal do inimigo arqueiro (state machine + ataque ranged)
- **Arrow.cs** - Script da flecha com detecção de colisão e sistema de dano
- **Archer.prefab** - Prefab configurado do arqueiro
- **Arrow.prefab** - Prefab da flecha

---

## 🎯 Arquitetura

### State Machine (Máquina de Estados)

```
Idle (Parado)
    └─> Detectou Player? → Chasing

Chasing (Perseguindo)
    ├─> Player saiu do raio? → Idle
    ├─> Player entrou no attackRadius? → Attacking
    └─> Caso contrário: Move em direção ao player

Attacking (Atacando)
    ├─> Player ainda no alcance? → Atira flecha
    └─> Player saiu do alcance? → Chasing
```

### Características Principais

- **Detecção por Layer:** Respeita sistema de elevação (layers) - configurável
- **Cross-Layer Attack:** 🆕 Pode atirar entre layers diferentes (archer no Level1 atira no player no Level0)
- **Para ao Atacar:** Archer para de se mover enquanto atira
- **Rotação Automática:** Flecha rotaciona na direção do player
- **Sistema de Cooldown:** Tempo configurável entre ataques
- **Sem Animações:** Implementação básica, fácil de expandir

---

## 🎯 Cross-Layer Attack System (NOVO!)

### O que é?

Sistema que permite o Archer atirar flechas no player **mesmo em layers diferentes**, simulando ataque de elevações diferentes (ex: archer no morro atirando no player no chão).

### Como funciona?

**Archer.cs:**
- Campo `allowCrossLayerAttack` (bool) controla o comportamento
- Quando `true`: ignora verificação de layer em detecção e ataque
- Quando `false`: comportamento original (só ataca no mesmo layer)

**Arrow.cs (Sistema de Colisão Inteligente):**
- Campo `ignoreLayers` (bool) é configurado automaticamente pelo Archer
- **Quando `true` (cross-layer):**
  - Flecha **IGNORA** colliders do próprio layer do shooter
  - Flecha **PARA** em colliders de outros layers
  - Exemplo: Archer Level1 → ignora colliders Level1, para em Level0
- **Quando `false` (same-layer):**
  - Flecha **PARA** em colliders do mesmo layer
  - Flecha **IGNORA** colliders de outros layers

### Casos de Uso

| Cenário | allowCrossLayerAttack | Resultado |
|---------|----------------------|-----------|
| Archer sniper no morro | `true` ✅ | Atira no player abaixo (Level0) |
| Archer patrulheiro | `false` ❌ | Só atira quando no mesmo nível |
| Boss em plataforma alta | `true` ✅ | Ataca de cima para baixo |
| Archer normal no chão | `false` ❌ | Gameplay tradicional |

### Configuração Rápida

**Para archer que atira entre layers:**
```
Archer.cs Inspector:
└─ Allow Cross Layer Attack: ✅ true
```

**Para archer tradicional (mesmo layer apenas):**
```
Archer.cs Inspector:
└─ Allow Cross Layer Attack: ❌ false
```

⚠️ **Importante:** O Arrow prefab NÃO precisa ser modificado - o campo `ignoreLayers` é configurado automaticamente pelo Archer ao disparar.

---

### Sistema de Colisão Inteligente (Como Funciona)

O sistema resolve o problema de **colliders em múltiplos níveis** de elevação:

#### 🎯 Problema Resolvido

**Cenário:** Archer no morro (Level1) com colliders Level1 para impedir saída

**Antes (problema):**
```
Archer Level1 → Atira flecha
└─> Flecha colide com collider Level1 ❌
    └─> Flecha não chega ao player Level0
```

**Depois (solução):**
```
Archer Level1 → Atira flecha (ignoreLayers=true)
├─> Flecha IGNORA colliders Level1 ✅ (mesmo layer do shooter)
└─> Flecha PARA em colliders Level0 ✅ (layer diferente)
    └─> Ou atinge player Level0 ✅
```

#### 🔄 Tabela de Comportamento

| Modo | Shooter Layer | Collider Layer | Comportamento |
|------|---------------|----------------|---------------|
| **Cross-Layer** (`ignoreLayers=true`) | Level1 | Level1 | ✅ Passa através |
| **Cross-Layer** | Level1 | Level0 | ❌ Para |
| **Cross-Layer** | Level0 | Level0 | ✅ Passa através |
| **Cross-Layer** | Level0 | Level1 | ❌ Para |
| **Same-Layer** (`ignoreLayers=false`) | Level1 | Level1 | ❌ Para |
| **Same-Layer** | Level1 | Level0 | ✅ Passa através |
| **Same-Layer** | Level0 | Level0 | ❌ Para |
| **Same-Layer** | Level0 | Level1 | ✅ Passa através |

#### 💡 Exemplo Prático

**Setup da Cena:**
```
Morro (Level1):
├─ TilemapCollider2D (Layer: Level1) - Impede archer sair
├─ Archer (Layer: Level1, allowCrossLayerAttack=true)
└─> Atira flecha para baixo

Chão (Level0):
├─ TilemapCollider2D (Layer: Level0) - Paredes
└─ Player (Layer: Level0)
```

**Fluxo da Flecha:**
1. Archer Level1 dispara (ignoreLayers=true, shooterLayer=Level1)
2. Flecha voa para baixo
3. Toca collider Level1 → **IGNORA** (mesmo layer do shooter)
4. Continua voando
5. Toca collider Level0 → **PARA** (layer diferente) OU
6. Toca Player Level0 → **CAUSA DANO** e destrói

---

## ⚙️ Configuração no Unity

### **Passo 1: Configurar Arrow Prefab**

#### Componentes Obrigatórios

```
Arrow (GameObject/Prefab)
├─ Arrow.cs (Script)
├─ Rigidbody2D
├─ Collider2D (CircleCollider2D ou CapsuleCollider2D)
└─ SpriteRenderer
```

#### Configuração do Rigidbody2D

| Propriedade | Valor |
|-------------|-------|
| Body Type | `Dynamic` |
| Gravity Scale | `0` ⚠️ (sem gravidade) |
| Collision Detection | `Continuous` |
| Constraints | `Freeze Rotation Z` ✅ |

#### Configuração do Collider2D

| Propriedade | Valor |
|-------------|-------|
| Is Trigger | ✅ **true** (MUITO IMPORTANTE!) |
| Radius/Size | `0.05` - `0.1` (pequeno) |

#### Orientação do Sprite

- ⚠️ O sprite da flecha deve apontar para a **DIREITA** (0°) por padrão
- Se estiver errado, rotacione o sprite ou ajuste no código

---

### **Passo 2: Configurar Archer Prefab**

#### Componentes Obrigatórios

```
Archer (GameObject/Prefab)
├─ Archer.cs (Script)
├─ Rigidbody2D
├─ Collider2D (CapsuleCollider2D)
├─ Animator
├─ SpriteRenderer
├─ HealthComponent
├─ HealthBarController
└─ ArrowSpawnPoint (Child GameObject)
    └─ Transform
```

#### Criando ArrowSpawnPoint

1. Criar Child GameObject chamado `"ArrowSpawnPoint"`
2. Posicionar na frente do sprite do Archer
   - Exemplo: Position X = `0.3`, Y = `0`, Z = `0`
3. Este ponto define onde as flechas spawnam

#### Configuração do Archer.cs no Inspector

##### Sistema de Detecção

| Campo | Valor Recomendado | Descrição |
|-------|-------------------|-----------|
| Detection Radius | `5.0` - `6.0` | Raio para detectar o player |
| Attack Radius | `3.0` - `4.0` | Distância para começar a atirar |
| Move Speed | `0.05` | Velocidade de movimento |
| **Allow Cross Layer Attack** 🆕 | `true` | ✅ Permite atirar entre layers diferentes<br>❌ Só atira no mesmo layer |

##### Sistema de Ataque à Distância

| Campo | Valor Recomendado | Descrição |
|-------|-------------------|-----------|
| Arrow Prefab | `Arrow` (prefab) | Arraste o Arrow prefab aqui |
| Arrow Spawn Point | `ArrowSpawnPoint` | Arraste o child GameObject |
| Arrow Speed | `5.0` | Velocidade da flecha |
| Arrow Damage | `1` | Dano por flecha |
| Arrow Lifespan | `3.0` | Segundos antes de desaparecer |
| Attack Cooldown | `2.5` | Tempo entre ataques (segundos) |

##### Sistema de Colisão

| Campo | Valor | Descrição |
|-------|-------|-----------|
| Collision Offset | `0.05` | Offset para detecção de colisão |
| Movement Filter | Configurar layers | Layers com que o archer colide |

---

### **Passo 3: Configurar Layers e Tags**

#### Tags Necessárias

- **Player:** GameObject do player deve ter tag `"Player"` ✅
- **Obstacle:** (Opcional) Objetos que bloqueiam flechas: `"Obstacle"` ou `"Wall"`

#### Layers (Sistema de Elevação)

**Modo Normal (allowCrossLayerAttack = false):**
- Archer e Player devem estar no **mesmo layer** para interagir
- Exemplos: `"Level0"`, `"Level1"`, `"Level2"`
- Flechas herdam o layer de quem as disparou

**Modo Cross-Layer (allowCrossLayerAttack = true):** 🆕
- Archer pode detectar e atirar no player **independente do layer**
- Exemplo: Archer no Level1 (morro) atira no player no Level0 (chão)
- Flechas ignoram verificação de layer ao colidir com o player

#### Physics 2D Collision Matrix

Certifique-se que a Collision Matrix permite:
- Arrow layer ↔️ Player layer
- Arrow layer ↔️ Obstacle layer (se quiser que flechas colidam com paredes)

---

## 🎮 Perfis de Gameplay

### Archer Agressivo (High Pressure)

```
Detection Radius: 7.0
Attack Radius: 5.0
Move Speed: 0.07
Attack Cooldown: 1.5
Arrow Speed: 6.0
Arrow Damage: 2
```

**Uso:** Boss fights, áreas desafiadoras

---

### Archer Balanceado (Recomendado)

```
Detection Radius: 5.0
Attack Radius: 3.0
Move Speed: 0.05
Attack Cooldown: 2.5
Arrow Speed: 5.0
Arrow Damage: 1
```

**Uso:** Gameplay padrão, primeira implementação

---

### Archer Defensivo (Low Pressure)

```
Detection Radius: 4.0
Attack Radius: 2.5
Move Speed: 0.03
Attack Cooldown: 3.5
Arrow Speed: 4.0
Arrow Damage: 1
```

**Uso:** Tutoriais, áreas iniciais

---

## 🔍 Debug e Visualização

### Gizmos no Editor

Quando o Archer está selecionado, você verá:

- 🟡 **Círculo Amarelo:** Raio de detecção (Detection Radius)
- 🔴 **Círculo Vermelho:** Raio de ataque (Attack Radius)
- 🟢 **Esfera Verde:** Ponto de spawn da flecha (Arrow Spawn Point)

### Console Logs

#### Archer.cs

```
[Archer] Movement filter atualizado para layer: Level0 (ID: 8)
→ Sistema iniciado, filtro de movimento configurado

[Archer] Shot arrow towards player at angle 45.5°
→ Flecha disparada com sucesso
```

#### Arrow.cs

```
[Arrow] Hit player for 1 damage!
→ Flecha atingiu o player via HealthComponent

[Arrow] Hit player (via PlayerController) for 1 damage!
→ Flecha atingiu o player via fallback

[Arrow] Hit obstacle, destroying arrow
→ Flecha colidiu com obstáculo e foi destruída
```

---

## 🐛 Troubleshooting

### Problema: Flecha não spawna

**Sintomas:** Archer entra em estado Attacking mas nenhuma flecha aparece

**Soluções:**
- ✅ Verificar se `Arrow Prefab` está atribuído no Inspector
- ✅ Verificar se `Arrow Spawn Point` está atribuído
- ✅ Verificar console para erro: `[Archer] Cannot shoot arrow - arrowPrefab is null!`
- ✅ Verificar se Arrow prefab tem o script `Arrow.cs`

---

### Problema: Flecha não colide com player

**Sintomas:** Flecha passa através do player sem causar dano

**Soluções:**
- ✅ Verificar Arrow Collider2D tem `Is Trigger = true`
- ✅ Verificar Player tem Collider2D (não trigger)
- ✅ Verificar Archer e Player estão no **mesmo layer**
- ✅ Verificar Physics 2D Collision Matrix
- ✅ Verificar Player tem tag `"Player"`
- ✅ Verificar console para logs de colisão

---

### Problema: Rotação da flecha está errada

**Sintomas:** Flecha aponta na direção errada ao voar

**Solução A: Rotacionar o sprite no prefab**
1. Abra o Arrow prefab
2. Rotacione o sprite para apontar DIREITA (0°)
3. Salve o prefab

**Solução B: Ajustar offset no código**

Edite `Archer.cs` linha 279:

```csharp
// Flecha 90° para esquerda? Use:
Quaternion.Euler(0, 0, angle + 90f)

// Flecha 90° para direita? Use:
Quaternion.Euler(0, 0, angle - 90f)

// Flecha invertida (180°)? Use:
Quaternion.Euler(0, 0, angle + 180f)
```

---

### Problema: Archer não para de se mover ao atacar

**Sintomas:** Archer continua perseguindo enquanto atira

**Soluções:**
- ✅ Verificar no código que `ChangeState(ArcherState.Attacking)` define:
  ```csharp
  animator.SetBool("isMoving", false);
  ```
- ✅ Verificar que estado `Attacking` não chama `MoveTowardsPlayer()`
- ✅ Verificar no Animator que `isMoving = false` realmente para a animação

---

### Problema: Archer não detecta o player

**Sintomas:** Archer permanece em Idle mesmo com player próximo

**Soluções:**
- ✅ Verificar Archer e Player estão no **mesmo layer**
- ✅ Aumentar `Detection Radius` para valor maior (ex: 10) para testar
- ✅ Verificar que Player tem tag `"Player"`
- ✅ Verificar `playerTransform` não é null (console log)

---

### Problema: Flecha causa muito/pouco dano

**Solução:**

Ajustar valor de `Arrow Damage` no Inspector do Archer:
- Dano baixo: `1`
- Dano médio: `2-3`
- Dano alto: `5+`

Ou criar variações do prefab com danos diferentes.

---

## 🔧 Customização Avançada

### Adicionar Múltiplos Tipos de Archer

Crie variações do prefab:

```
Archer_Basic (Arrow Damage: 1, Cooldown: 2.5)
Archer_Rapid (Arrow Damage: 1, Cooldown: 1.0)
Archer_Heavy (Arrow Damage: 3, Cooldown: 4.0)
Archer_Sniper (Attack Radius: 8, Arrow Speed: 8)
```

---

### Adicionar Efeitos Visuais

#### Trail da Flecha

1. Adicionar `Trail Renderer` ao Arrow prefab
2. Configurar:
   - Time: `0.3`
   - Width: `0.05` → `0.01`
   - Color: Gradient do amarelo ao transparente

#### Partículas no Impacto

Em `Arrow.cs`, antes de `Destroy(gameObject)`:

```csharp
// Instanciar partícula de impacto
GameObject impact = Instantiate(impactParticlePrefab, transform.position, Quaternion.identity);
Destroy(impact, 1f);
```

---

### Adicionar Som de Ataque

Em `Archer.cs`, método `ShootArrow()`:

```csharp
void ShootArrow()
{
    // ... código existente ...

    // Adicionar som de disparo
    AudioSource audioSource = GetComponent<AudioSource>();
    if (audioSource != null && shootSound != null)
    {
        audioSource.PlayOneShot(shootSound);
    }
}
```

Adicionar campo:
```csharp
[Header("Audio")]
public AudioClip shootSound;
```

---

### Implementar Animação de Ataque

1. Criar animação "archer_attack" no Animator
2. Adicionar Animation Event no frame de disparo
3. Criar método em Archer.cs:

```csharp
// Chamado por Animation Event
public void OnShootAnimationEvent()
{
    ShootArrow();
}
```

4. Modificar `AttackPlayer()` para usar trigger:

```csharp
void AttackPlayer()
{
    if (Time.time - lastAttackTime >= attackCooldown && canAttack)
    {
        lastAttackTime = Time.time;
        animator.SetTrigger("attack"); // Animation chama OnShootAnimationEvent()
        StartCoroutine(AttackCooldownCoroutine());
    }
}
```

---

## 📊 Integração com Sistemas Existentes

### Sistema de Elevação (Layers)

O Archer respeita automaticamente o sistema de elevação do projeto:

- **Detecção:** Só detecta player no mesmo layer ([Archer.cs:169-171](Assets/Characters/Archer/Archer.cs#L169-L171))
- **Ataque:** Só ataca player no mesmo layer ([Archer.cs:194-196](Assets/Characters/Archer/Archer.cs#L194-L196))
- **Flechas:** Só colidem com alvos no mesmo layer ([Arrow.cs:44-49](Assets/Characters/Player/Arrow.cs#L44-L49))

### Sistema de Dano (HealthComponent)

O Arrow.cs usa o sistema unificado de dano:

```csharp
HealthComponent playerHealth = other.GetComponent<HealthComponent>();
if (playerHealth != null)
{
    playerHealth.TakeDamage(damage);
}
```

**Fallback:** Se HealthComponent não existir, tenta `PlayerController.TakeDamage()`

### Sistema de Movimento (Collision Filter)

O Archer usa o mesmo sistema de movimento do Enemy.cs:

- `ContactFilter2D` com layer mask automático
- `rb.Cast()` para detecção de colisão antes de mover
- `rb.MovePosition()` para movimento físico suave

---

## 📝 Checklist de Implementação

### Setup Inicial

- [ ] Arrow.cs modificado com sistema de colisão
- [ ] Archer.cs criado com state machine
- [ ] Arrow prefab criado e configurado
- [ ] Archer prefab criado com todos componentes

### Configuração do Arrow Prefab

- [ ] Rigidbody2D: Dynamic, Gravity = 0, Freeze Rotation
- [ ] Collider2D: IsTrigger = true, tamanho pequeno
- [ ] Sprite apontando para DIREITA (0°)
- [ ] Arrow.cs script adicionado

### Configuração do Archer Prefab

- [ ] Archer.cs script adicionado
- [ ] ArrowSpawnPoint child criado e posicionado
- [ ] Arrow Prefab atribuído no Inspector
- [ ] Arrow Spawn Point atribuído no Inspector
- [ ] Detection Radius configurado (5.0)
- [ ] Attack Radius configurado (3.0)
- [ ] Attack Cooldown configurado (2.5)
- [ ] Arrow Speed configurado (5.0)
- [ ] Arrow Damage configurado (1)
- [ ] Rigidbody2D, Collider2D, Animator presentes
- [ ] HealthComponent presente
- [ ] HealthBarController presente

### Tags e Layers

- [ ] Player tem tag "Player"
- [ ] Archer e Player no mesmo layer
- [ ] Physics 2D Collision Matrix configurada

### Testes

- [ ] Archer detecta player (entra em Chasing)
- [ ] Archer move em direção ao player
- [ ] Archer para ao entrar no Attack Radius
- [ ] Archer dispara flecha na direção correta
- [ ] Flecha rotaciona corretamente
- [ ] Flecha voa na direção do player
- [ ] Flecha causa dano ao colidir
- [ ] Flecha desaparece após colisão
- [ ] Cooldown funciona (tempo entre tiros)
- [ ] Sistema de elevação funciona (layers)

---

## 🎓 Padrões de Código Utilizados

### Component Reference Pattern

```csharp
[SerializeField] private Rigidbody2D rb;

void Awake()
{
    rb ??= GetComponent<Rigidbody2D>();
    if (rb == null)
        Debug.LogError("[Archer] Rigidbody2D not found!");
}
```

**Características:**
- Inspector-first (manual assignment tem prioridade)
- Fallback automático com `??=` (null-coalescing)
- Sempre loga erros para missing components

---

### State Machine Pattern

```csharp
public enum ArcherState { Idle, Chasing, Attacking }

void UpdateArcherBehavior()
{
    switch (currentState)
    {
        case ArcherState.Idle:
            // Lógica do estado Idle
            break;
        case ArcherState.Chasing:
            // Lógica do estado Chasing
            break;
        case ArcherState.Attacking:
            // Lógica do estado Attacking
            break;
    }
}
```

---

### Layer-Based Detection Pattern

```csharp
int archerLayer = gameObject.layer;
int playerLayer = playerTransform.gameObject.layer;

if (archerLayer != playerLayer)
    return false; // Diferentes elevações
```

---

### Attack Cooldown Pattern

```csharp
private float lastAttackTime;
private bool canAttack = true;

void AttackPlayer()
{
    if (Time.time - lastAttackTime >= attackCooldown && canAttack)
    {
        lastAttackTime = Time.time;
        // Executar ataque
        StartCoroutine(AttackCooldownCoroutine());
    }
}

IEnumerator AttackCooldownCoroutine()
{
    canAttack = false;
    yield return new WaitForSeconds(attackCooldown);
    canAttack = true;
}
```

---

## 🚀 Expansões Futuras

### Curto Prazo (Próximas Implementações)

- [ ] Adicionar animação de ataque (archer_attack)
- [ ] Adicionar som de disparo de flecha
- [ ] Adicionar trail renderer na flecha
- [ ] Adicionar partículas de impacto

### Médio Prazo

- [ ] Implementar múltiplos tipos de archer (Rapid, Heavy, Sniper)
- [ ] Adicionar predição de movimento (lead target)
- [ ] Implementar kiting behavior (se afastar quando player fica muito perto)
- [ ] Adicionar flechas especiais (fogo, gelo, veneno)

### Longo Prazo

- [ ] Sistema de formação para múltiplos archers
- [ ] IA avançada (cover-seeking, flanking)
- [ ] Flechas com trajetória em arco (gravity simulation)
- [ ] Boss archer com padrões de ataque especiais

---

## 📚 Referências

### Arquivos Relacionados

- [Enemy.cs](Assets/Characters/Slime/Enemy.cs) - Base para o Archer (melee enemy)
- [HealthComponent.cs](Assets/Characters/HealthBar/HealthComponent.cs) - Sistema de dano
- [PlayerController.cs](Assets/Characters/Player/PlayerController.cs) - Referência de movimento
- [CLAUDE.md](CLAUDE.md) - Documentação geral do projeto

### Conceitos Unity

- [State Machines in Unity](https://docs.unity3d.com/Manual/StateMachineBasics.html)
- [2D Physics - Collision](https://docs.unity3d.com/Manual/CollidersOverview.html)
- [Rigidbody2D](https://docs.unity3d.com/ScriptReference/Rigidbody2D.html)
- [OnTriggerEnter2D](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerEnter2D.html)

---

**Versão:** 1.0
**Última atualização:** 2025-11-14
**Autor:** Implementado via Claude Code
**Status:** ✅ Implementação Básica Completa
