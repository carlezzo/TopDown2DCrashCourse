# Problema: Conflito de Colisão em Elevação Multi-Níveis

> **Data:** 2025-11-13
> **Contexto:** Solução para o conflito entre Player passando por trás do morro vs Arqueiro não escapando pelos cantos
> **Projeto:** TopDown2D Crash Course

---

## 📋 Índice

1. [Análise do Problema](#1-análise-do-problema)
2. [Solução 1: Layer-Based Collision (RECOMENDADA)](#2-solução-1-layer-based-collision-recomendada)
3. [Solução 2: Conditional Collision (Script-Based)](#3-solução-2-conditional-collision-script-based)
4. [Solução 3: Visual Trickery (Fake Depth)](#4-solução-3-visual-trickery-fake-depth)
5. [Comparação de Soluções](#5-comparação-de-soluções)
6. [Implementação Detalhada](#6-implementação-detalhada)
7. [Troubleshooting](#7-troubleshooting)

---

## 1. Análise do Problema

### 1.1 Descrição do Cenário

**Setup atual:**
- Montanha com arqueiro no topo (Level1)
- Player no chão (Level0) passando por trás
- Tilemap `CollisionHigh` com colliders APENAS NO MEIO (não nos cantos)

**Visualização:**

```
Vista Top-Down do Morro:

┌─────────────────────────────┐
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░│ ← Cantos SEM collider
│░░  ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓  ░░░░│
│░░  ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓  ░░░░│
│░░  ▓▓▓  🏹 Arqueiro  ▓  ░░░░│ ← CollisionHigh (meio tem collider)
│░░  ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓  ░░░░│
│░░  ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓  ░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░ 🗡️ Player aqui ░░░░░░│ ← Pode passar (sem collider)
└─────────────────────────────┘

Legenda:
▓ = Área com collider (bloqueia movimento)
░ = Área SEM collider (permite passagem)
```

### 1.2 Conflito de Requisitos

**REQUISITO 1: Player passa ATRÁS do morro**
- Solução atual: Remover colliders dos cantos ✅
- Resultado: Player pode passar por trás ✅
- Problema: Arqueiro também pode escapar pelos cantos! ❌

**REQUISITO 2: Arqueiro não deve escapar pelo morro**
- Solução necessária: Adicionar colliders nos cantos ✅
- Resultado: Arqueiro não escapa ✅
- Problema: Player não pode passar por trás! ❌

**Tabela de Conflito:**

| Configuração | Player Passa Atrás? | Arqueiro Preso? | Solução Válida? |
|--------------|---------------------|-----------------|-----------------|
| Cantos SEM collider | ✅ Sim | ❌ Não (escapa) | ❌ Incompleta |
| Cantos COM collider | ❌ Não (bloqueia) | ✅ Sim | ❌ Incompleta |
| **Dual Colliders** | ✅ Sim | ✅ Sim | ✅ **IDEAL** |

### 1.3 Por Que Isso Acontece?

**Root Cause:** Você está tentando usar **UM único collider** para resolver **DOIS problemas diferentes**:

```
Problema A: Limitar movimento de entidades NO MESMO NÍVEL
└─> Arqueiro (Level1) não deve escapar do morro

Problema B: Permitir passagem de entidades em NÍVEL DIFERENTE
└─> Player (Level0) deve passar por trás do morro

CONCLUSÃO: Precisa de 2 colliders diferentes!
```

---

## 2. Solução 1: Layer-Based Collision (RECOMENDADA) ⭐

### 2.1 Conceito

**Ideia Central:** Criar **dois colliders separados** com **layers diferentes**:

1. **TerrainCollider** (Layer: ElevationLevel1)
   - Cobre **ÁREA INTEIRA** do morro (incluindo cantos)
   - Bloqueia entidades do **MESMO nível** (Arqueiro Level1)
   - Usa Collision Matrix do Unity

2. **DepthBoundsCollider** (Layer: ElevationLevel0)
   - Cobre **APENAS O MEIO** do morro (exclui cantos)
   - Bloqueia entidades do **NÍVEL ABAIXO** (Player Level0)
   - Define área de oclusão visual

### 2.2 Diagrama de Funcionamento

```
VISTA LATERAL (conceitual):

     ┌─────────────────────────┐
     │  🏹 Arqueiro (Level1)   │ ← TerrainCollider (completo)
     │    PRESO DENTRO         │   bloqueia Arqueiro
     └─────────────────────────┘
           │ collider │
           │ completo │
     ──────┴──────────┴──────── ← Superfície do morro
           │ collider │
           │ do meio  │ ← DepthBoundsCollider (parcial)
     ──────┴──────────┴────────   bloqueia Player só no centro
     ░░░░░░░░░░░░░░░░░░░░░░░░░ ← Cantos livres
     ░░  🗡️ Player  ░░░░░░░░░░   Player passa aqui!
     ░░  (Level0)   ░░░░░░░░░░
     ═══════════════════════════ ← Chão


VISTA TOP-DOWN (gameplay):

┌─────────────────────────────┐
│█████████████████████████████│ ← TerrainCollider (Layer1)
│█████████████████████████████│   Bloqueia TUDO para Level1
│█████  ╔═══════════╗  ██████│
│█████  ║ 🏹 Arqueiro ║  ██████│
│█████  ║  (Level1)  ║  ██████│ ← DepthBoundsCollider (Layer0)
│█████  ║ PRESO AQUI ║  ██████│   Bloqueia só MEIO para Level0
│█████  ╚═══════════╝  ██████│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░│ ← Cantos livres para Level0
│░░░░░ 🗡️ Player ░░░░░░░░░░░│   Player passa pelos cantos!
└─────────────────────────────┘

Resultado:
✅ Arqueiro bloqueado em TODA área (TerrainCollider)
✅ Player bloqueado SÓ no meio (DepthBoundsCollider)
✅ Player LIVRE nos cantos (sem collider para Layer0)
```

### 2.3 Implementação: Script Automático

```csharp
// MountainCollisionSetup.cs
// Adicionar a qualquer GameObject de montanha/estrutura elevada
using UnityEngine;

/// <summary>
/// Cria automaticamente dois colliders para sistema de elevação:
/// 1. TerrainCollider: Bloqueia entidades do MESMO nível (área completa)
/// 2. DepthBoundsCollider: Bloqueia entidades de NÍVEL ABAIXO (área parcial)
/// </summary>
public class MountainCollisionSetup : MonoBehaviour
{
    [Header("Configuração de Elevação")]
    [Tooltip("Nível desta estrutura (ex: Platform = Level1)")]
    [SerializeField] private ElevationManager.ElevationLevel structureLevel = ElevationManager.ElevationLevel.Platform;

    [Header("Bounds Configuration")]
    [Tooltip("Tamanho TOTAL da estrutura (inclui cantos) - para entidades do MESMO nível")]
    [SerializeField] private Vector2 fullBoundsSize = new Vector2(10f, 8f);

    [Tooltip("Tamanho do CENTRO da estrutura (exclui cantos) - para entidades de NÍVEL ABAIXO")]
    [SerializeField] private Vector2 centerBoundsSize = new Vector2(6f, 5f);

    [Tooltip("Offset vertical do collider central")]
    [SerializeField] private Vector2 centerOffset = Vector2.zero;

    [Header("Advanced Settings")]
    [Tooltip("Auto-setup ao iniciar o jogo?")]
    [SerializeField] private bool autoSetupOnAwake = true;

    [Tooltip("Tipo de collider (Box é mais performático, Polygon é mais preciso)")]
    [SerializeField] private ColliderType colliderType = ColliderType.Box;

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private Color fullBoundsColor = new Color(1f, 0f, 0f, 0.5f);
    [SerializeField] private Color centerBoundsColor = new Color(1f, 1f, 0f, 0.5f);

    private GameObject terrainColliderObj;
    private GameObject depthBoundsColliderObj;

    public enum ColliderType
    {
        Box,
        Polygon
    }

    void Awake()
    {
        if (autoSetupOnAwake)
        {
            SetupColliders();
        }
    }

    /// <summary>
    /// Cria ou reconfigura os colliders. Pode ser chamado via Inspector ou código.
    /// </summary>
    [ContextMenu("Setup Colliders")]
    public void SetupColliders()
    {
        Debug.Log($"[MountainSetup] Configurando colliders para {gameObject.name}...");

        // 1. Setup TerrainCollider (bloqueia entidades do MESMO nível)
        SetupTerrainCollider();

        // 2. Setup DepthBoundsCollider (bloqueia entidades de NÍVEL ABAIXO)
        SetupDepthBoundsCollider();

        Debug.Log($"[MountainSetup] ✅ Colliders configurados para Level{(int)structureLevel}!");
    }

    private void SetupTerrainCollider()
    {
        // Criar ou encontrar GameObject para TerrainCollider
        terrainColliderObj = transform.Find("TerrainCollider_SameLevel")?.gameObject;

        if (terrainColliderObj == null)
        {
            terrainColliderObj = new GameObject("TerrainCollider_SameLevel");
            terrainColliderObj.transform.SetParent(transform);
            terrainColliderObj.transform.localPosition = Vector3.zero;
        }

        // Configurar layer (mesmo nível da estrutura)
        int layerID = ElevationHelper.LevelToLayer(structureLevel);
        terrainColliderObj.layer = layerID;

        // Adicionar collider apropriado
        if (colliderType == ColliderType.Box)
        {
            var boxCollider = terrainColliderObj.GetComponent<BoxCollider2D>();
            if (boxCollider == null)
            {
                // Remover outros colliders se existirem
                DestroyImmediate(terrainColliderObj.GetComponent<PolygonCollider2D>());
                boxCollider = terrainColliderObj.AddComponent<BoxCollider2D>();
            }

            boxCollider.size = fullBoundsSize;
            boxCollider.offset = Vector2.zero;
        }
        else
        {
            // Polygon collider (mais complexo, apenas como exemplo)
            var polyCollider = terrainColliderObj.GetComponent<PolygonCollider2D>();
            if (polyCollider == null)
            {
                DestroyImmediate(terrainColliderObj.GetComponent<BoxCollider2D>());
                polyCollider = terrainColliderObj.AddComponent<PolygonCollider2D>();
            }

            // Criar forma retangular manualmente
            Vector2[] points = new Vector2[4]
            {
                new Vector2(-fullBoundsSize.x/2, -fullBoundsSize.y/2),
                new Vector2(fullBoundsSize.x/2, -fullBoundsSize.y/2),
                new Vector2(fullBoundsSize.x/2, fullBoundsSize.y/2),
                new Vector2(-fullBoundsSize.x/2, fullBoundsSize.y/2)
            };
            polyCollider.points = points;
        }

        Debug.Log($"[MountainSetup] TerrainCollider criado: Layer={LayerMask.LayerToName(layerID)}, Size={fullBoundsSize}");
    }

    private void SetupDepthBoundsCollider()
    {
        // Verificar se há nível abaixo
        int belowLevelInt = (int)structureLevel - 1;
        if (belowLevelInt < 0)
        {
            Debug.LogWarning("[MountainSetup] Estrutura está no Level0, não há nível abaixo. DepthBounds não criado.");
            return;
        }

        ElevationManager.ElevationLevel belowLevel = (ElevationManager.ElevationLevel)belowLevelInt;

        // Criar ou encontrar GameObject para DepthBoundsCollider
        depthBoundsColliderObj = transform.Find("DepthBounds_LevelBelow")?.gameObject;

        if (depthBoundsColliderObj == null)
        {
            depthBoundsColliderObj = new GameObject("DepthBounds_LevelBelow");
            depthBoundsColliderObj.transform.SetParent(transform);
            depthBoundsColliderObj.transform.localPosition = Vector3.zero;
        }

        // Configurar layer (nível ABAIXO da estrutura)
        int layerID = ElevationHelper.LevelToLayer(belowLevel);
        depthBoundsColliderObj.layer = layerID;

        // Adicionar collider (sempre Box para DepthBounds, mais simples)
        var boxCollider = depthBoundsColliderObj.GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            boxCollider = depthBoundsColliderObj.AddComponent<BoxCollider2D>();
        }

        boxCollider.size = centerBoundsSize;
        boxCollider.offset = centerOffset;

        Debug.Log($"[MountainSetup] DepthBoundsCollider criado: Layer={LayerMask.LayerToName(layerID)}, Size={centerBoundsSize}");
    }

    /// <summary>
    /// Remove colliders criados por este script
    /// </summary>
    [ContextMenu("Remove Colliders")]
    public void RemoveColliders()
    {
        if (terrainColliderObj != null)
        {
            DestroyImmediate(terrainColliderObj);
            Debug.Log("[MountainSetup] TerrainCollider removido.");
        }

        if (depthBoundsColliderObj != null)
        {
            DestroyImmediate(depthBoundsColliderObj);
            Debug.Log("[MountainSetup] DepthBoundsCollider removido.");
        }
    }

    void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        // Desenhar full bounds (vermelho/transparente)
        Gizmos.color = fullBoundsColor;
        Gizmos.DrawCube(transform.position, fullBoundsSize);

        // Desenhar center bounds (amarelo/transparente)
        Gizmos.color = centerBoundsColor;
        Gizmos.DrawCube(transform.position + (Vector3)centerOffset, centerBoundsSize);
    }

    void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) return;

        // Wireframes quando selecionado
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, fullBoundsSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + (Vector3)centerOffset, centerBoundsSize);

        #if UNITY_EDITOR
        // Labels
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * (fullBoundsSize.y / 2 + 0.5f),
            $"Full Bounds: {fullBoundsSize}\n" +
            $"Center Bounds: {centerBoundsSize}\n" +
            $"Level: {structureLevel} ({(int)structureLevel})"
        );
        #endif
    }

    #if UNITY_EDITOR
    void OnValidate()
    {
        // Garantir que centerBounds não seja maior que fullBounds
        centerBoundsSize.x = Mathf.Min(centerBoundsSize.x, fullBoundsSize.x);
        centerBoundsSize.y = Mathf.Min(centerBoundsSize.y, fullBoundsSize.y);
    }
    #endif
}
```

### 2.4 Setup Passo-a-Passo no Unity

**PASSO 1: Configurar Collision Matrix**

1. Abrir `Edit → Project Settings → Physics 2D`
2. Scroll até "Layer Collision Matrix"
3. Configurar conforme tabela:

```
                  Level0  Level1  Level2  Level3
ElevationLevel0  │  ✅     ❌      ❌      ❌
ElevationLevel1  │  ❌     ✅      ❌      ❌
ElevationLevel2  │  ❌     ❌      ✅      ❌
ElevationLevel3  │  ❌     ❌      ❌      ✅

Legenda:
✅ = Colide (marcar checkbox)
❌ = Não colide (desmarcar checkbox)

REGRA: Entidades só colidem com estruturas do MESMO nível!
```

**PASSO 2: Adicionar Script ao Morro**

1. Selecionar GameObject do morro no Hierarchy
2. Inspector → `Add Component → Mountain Collision Setup`
3. Configurar campos:
   ```
   Structure Level: Platform (ou Level1)
   Full Bounds Size: (10, 8)    ← ajustar ao tamanho real do seu morro
   Center Bounds Size: (6, 5)   ← cerca de 60-70% do full bounds
   Center Offset: (0, 0)        ← ajustar se necessário
   ```

4. Inspector → Clicar em "Setup Colliders" (ou marcar "Auto Setup On Awake")

**PASSO 3: Verificar Resultado**

1. Scene view deve mostrar:
   - Gizmo vermelho (full bounds) cobrindo área INTEIRA
   - Gizmo amarelo (center bounds) cobrindo APENAS o meio

2. Hierarchy deve ter novos GameObjects filhos:
   ```
   Morro
   ├── Visual (sprite)
   ├── TerrainCollider_SameLevel (Layer: ElevationLevel1)
   └── DepthBounds_LevelBelow (Layer: ElevationLevel0)
   ```

**PASSO 4: Testar**

1. **Teste 1: Arqueiro no topo**
   - Criar Arqueiro com `ElevationEntity` (Starting Level: Platform)
   - Movê-lo pelo topo → deve colidir com TODA área (incluindo cantos) ✅

2. **Teste 2: Player na base**
   - Player com `ElevationEntity` (Starting Level: Ground)
   - Movê-lo pelos cantos → deve passar livremente ✅
   - Movê-lo pelo centro → deve colidir ✅

### 2.5 Ajuste Fino: Tamanhos dos Bounds

**Como determinar os valores ideais:**

```csharp
// Regra geral:
Full Bounds Size = Tamanho visual completo do morro
Center Bounds Size = 60-80% do Full Bounds Size

// Exemplo prático:
Morro visual: 10x8 unidades
├─> Full Bounds: (10, 8)    ← 100% do tamanho
└─> Center Bounds: (6, 5)   ← 60% do tamanho

// Para deixar mais área de passagem nos cantos:
Center Bounds: (5, 4)   ← 50% do tamanho (cantos maiores)

// Para deixar menos área de passagem:
Center Bounds: (8, 6.5) ← 80% do tamanho (cantos menores)
```

**Visualização dos tamanhos:**

```
Full Bounds = 10x8:
┌──────────────────────────────┐
│                              │
│  Center Bounds = 6x5:        │
│  ┌──────────────────┐        │
│  │                  │        │
│  │   🏹 Arqueiro    │        │
│  │                  │        │
│  └──────────────────┘        │
│ ◄────► 2 unidades de         │
│        espaço livre           │
└──────────────────────────────┘

Resultado:
- Player pode passar em áreas de 2 unidades nos cantos
- Arqueiro bloqueado em toda área de 10x8
```

---

## 3. Solução 2: Conditional Collision (Script-Based)

### 3.1 Conceito

**Ideia:** Usar **um único collider completo**, mas desabilitar colisão **via script** para entidades de nível diferente.

**Vantagens:**
- Não precisa criar múltiplos colliders
- Flexível (lógica customizável)

**Desvantagens:**
- Menos performático (CPU em vez de Physics Matrix)
- Mais complexo de debugar

### 3.2 Implementação

```csharp
// SmartCollider.cs - Collider que filtra por elevação
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Collider inteligente que só afeta entidades baseado em critérios de elevação.
/// Útil para estruturas que devem bloquear apenas entidades do mesmo nível.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class SmartCollider : MonoBehaviour
{
    [Header("Configuração de Elevação")]
    [Tooltip("Nível desta estrutura")]
    [SerializeField] private ElevationManager.ElevationLevel myLevel = ElevationManager.ElevationLevel.Platform;

    [Header("Regras de Colisão")]
    [Tooltip("Bloquear entidades do MESMO nível?")]
    [SerializeField] private bool blockSameLevel = true;

    [Tooltip("Bloquear entidades de NÍVEIS DIFERENTES?")]
    [SerializeField] private bool blockDifferentLevels = false;

    [Header("Exceções")]
    [Tooltip("Tags que SEMPRE colidem (ex: 'Projectile')")]
    [SerializeField] private string[] alwaysBlockTags = new string[0];

    [Tooltip("Tags que NUNCA colidem (ex: 'Ghost')")]
    [SerializeField] private string[] neverBlockTags = new string[0];

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    private Collider2D col;
    private HashSet<Collider2D> ignoredColliders = new HashSet<Collider2D>();

    void Awake()
    {
        col = GetComponent<Collider2D>();

        if (col == null)
        {
            Debug.LogError("[SmartCollider] Collider2D não encontrado!");
            enabled = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        ProcessCollision(collision.collider, collision.gameObject);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Re-avaliar a cada frame (caso entidade mude de nível)
        ProcessCollision(collision.collider, collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        ProcessCollision(other, other.gameObject);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        ProcessCollision(other, other.gameObject);
    }

    private void ProcessCollision(Collider2D otherCollider, GameObject otherObject)
    {
        // Verificar exceções de tags
        if (ShouldAlwaysBlock(otherObject))
        {
            EnableCollision(otherCollider);
            return;
        }

        if (ShouldNeverBlock(otherObject))
        {
            DisableCollision(otherCollider);
            return;
        }

        // Lógica baseada em elevação
        if (ShouldIgnoreCollision(otherObject))
        {
            DisableCollision(otherCollider);
        }
        else
        {
            EnableCollision(otherCollider);
        }
    }

    private bool ShouldIgnoreCollision(GameObject other)
    {
        // Verificar se tem ElevationEntity
        var otherEntity = other.GetComponent<ElevationEntity>();
        if (otherEntity == null)
        {
            // Sem ElevationEntity, usar comportamento padrão
            return !blockSameLevel; // Bloqueia se blockSameLevel estiver ativo
        }

        bool sameLevel = otherEntity.CurrentLevel == myLevel;

        // Aplicar regras
        if (sameLevel && blockSameLevel)
        {
            return false; // Não ignorar = bloquear
        }

        if (!sameLevel && blockDifferentLevels)
        {
            return false; // Não ignorar = bloquear
        }

        return true; // Ignorar colisão
    }

    private bool ShouldAlwaysBlock(GameObject obj)
    {
        foreach (var tag in alwaysBlockTags)
        {
            if (obj.CompareTag(tag)) return true;
        }
        return false;
    }

    private bool ShouldNeverBlock(GameObject obj)
    {
        foreach (var tag in neverBlockTags)
        {
            if (obj.CompareTag(tag)) return true;
        }
        return false;
    }

    private void EnableCollision(Collider2D otherCollider)
    {
        if (ignoredColliders.Contains(otherCollider))
        {
            Physics2D.IgnoreCollision(col, otherCollider, false);
            ignoredColliders.Remove(otherCollider);

            if (showDebugLogs)
            {
                Debug.Log($"[SmartCollider] ✅ Habilitando colisão com {otherCollider.name}");
            }
        }
    }

    private void DisableCollision(Collider2D otherCollider)
    {
        if (!ignoredColliders.Contains(otherCollider))
        {
            Physics2D.IgnoreCollision(col, otherCollider, true);
            ignoredColliders.Add(otherCollider);

            if (showDebugLogs)
            {
                Debug.Log($"[SmartCollider] ❌ Desabilitando colisão com {otherCollider.name}");
            }
        }
    }

    void OnDisable()
    {
        // Limpar ignoredColliders ao desabilitar
        foreach (var otherCol in ignoredColliders)
        {
            if (otherCol != null)
            {
                Physics2D.IgnoreCollision(col, otherCol, false);
            }
        }
        ignoredColliders.Clear();
    }
}
```

### 3.3 Setup

**PASSO 1: Adicionar ao Morro**

1. Selecionar GameObject do morro
2. Garantir que tem **um único collider** cobrindo TODA área (incluindo cantos)
3. Inspector → `Add Component → Smart Collider`

**PASSO 2: Configurar**

```
My Level: Platform (Level1)

Regras de Colisão:
✅ Block Same Level: true       ← Bloqueia Arqueiro (Level1)
❌ Block Different Levels: false ← NÃO bloqueia Player (Level0)

Always Block Tags: (vazio)
Never Block Tags: (vazio)

Show Debug Logs: true (para testes)
```

**PASSO 3: Testar**

- Arqueiro Level1 → deve colidir ✅
- Player Level0 → deve passar através ✅

### 3.4 Quando Usar Esta Solução

**Usar se:**
- ✅ Você quer controle fino sobre lógica de colisão
- ✅ Tem casos especiais (tags, exceções)
- ✅ Não quer mexer em Collision Matrix

**NÃO usar se:**
- ❌ Performance é crítica (muitas entidades)
- ❌ Prefere solução nativa do Unity
- ❌ Quer simplicidade

---

## 4. Solução 3: Visual Trickery (Fake Depth) 🎨

### 4.1 Conceito

**Ideia:** O player **não REALMENTE passa por trás**, apenas **PARECE** que passa (sorting order trick).

**Como funciona:**
1. Collider COMPLETO no morro (incluindo cantos)
2. Player colide normalmente
3. Quando player está em Y baixo → sorting order muda → renderiza ATRÁS do morro
4. Ilusão de profundidade sem remover colisões

### 4.2 Implementação

```csharp
// FakeDepthZone.cs - Cria ilusão de profundidade sem afetar colisão
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Cria zona que muda sorting order de entidades baseado em posição Y,
/// criando ilusão de passar por trás de estruturas SEM remover colisão.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class FakeDepthZone : MonoBehaviour
{
    [Header("Configuração de Profundidade")]
    [Tooltip("SpriteRenderer da estrutura que oclui (montanha, prédio, etc)")]
    [SerializeField] private SpriteRenderer occludingStructure;

    [Tooltip("Sorting order quando entidade está ATRÁS (escondida)")]
    [SerializeField] private int behindSortingOrder = 500;

    [Tooltip("Sorting order quando entidade está NA FRENTE (visível)")]
    [SerializeField] private int frontSortingOrder = 2000;

    [Header("Thresholds")]
    [Tooltip("Posição Y relativa onde entidade 'passa por trás' (negativo = abaixo da estrutura)")]
    [SerializeField] private float depthThresholdY = -2f;

    [Tooltip("Suavizar transição de sorting order?")]
    [SerializeField] private bool smoothTransition = true;

    [Tooltip("Velocidade da transição (se smooth ativado)")]
    [SerializeField] private float transitionSpeed = 5f;

    [Header("Filtros")]
    [Tooltip("Apenas afetar entidades com estas tags")]
    [SerializeField] private string[] affectedTags = new string[] { "Player", "Enemy" };

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private bool showDebugLogs = false;

    private Collider2D triggerZone;
    private Dictionary<SpriteRenderer, int> originalSortingOrders = new Dictionary<SpriteRenderer, int>();
    private Dictionary<SpriteRenderer, int> targetSortingOrders = new Dictionary<SpriteRenderer, int>();

    void Awake()
    {
        triggerZone = GetComponent<Collider2D>();

        if (!triggerZone.isTrigger)
        {
            Debug.LogWarning("[FakeDepthZone] Collider deve ser Trigger! Configurando automaticamente...");
            triggerZone.isTrigger = true;
        }

        if (occludingStructure == null)
        {
            occludingStructure = GetComponentInParent<SpriteRenderer>();
            if (occludingStructure == null)
            {
                Debug.LogWarning("[FakeDepthZone] Occluding Structure não definido!");
            }
        }
    }

    void Update()
    {
        if (smoothTransition)
        {
            // Aplicar transições suaves de sorting order
            foreach (var kvp in targetSortingOrders)
            {
                var sr = kvp.Key;
                int targetOrder = kvp.Value;

                if (sr != null && sr.sortingOrder != targetOrder)
                {
                    sr.sortingOrder = Mathf.RoundToInt(
                        Mathf.Lerp(sr.sortingOrder, targetOrder, Time.deltaTime * transitionSpeed)
                    );
                }
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        // Filtrar por tags
        if (!IsAffectedEntity(collision.gameObject)) return;

        var sr = collision.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        // Salvar sorting order original (primeira vez)
        if (!originalSortingOrders.ContainsKey(sr))
        {
            originalSortingOrders[sr] = sr.sortingOrder;
        }

        // Calcular sorting order baseado em posição Y
        float entityY = collision.transform.position.y;
        float structureY = transform.position.y;
        float relativeY = entityY - structureY;

        int newSortingOrder;

        if (relativeY < depthThresholdY)
        {
            // Entidade está ATRÁS (Y baixo) → renderizar atrás
            newSortingOrder = behindSortingOrder;

            if (showDebugLogs)
            {
                Debug.Log($"[FakeDepthZone] {collision.name} ATRÁS (Y={relativeY:F2}) → sorting={newSortingOrder}");
            }
        }
        else
        {
            // Entidade está NA FRENTE (Y alto) → renderizar na frente
            newSortingOrder = frontSortingOrder;

            if (showDebugLogs)
            {
                Debug.Log($"[FakeDepthZone] {collision.name} NA FRENTE (Y={relativeY:F2}) → sorting={newSortingOrder}");
            }
        }

        // Aplicar sorting order
        if (smoothTransition)
        {
            targetSortingOrders[sr] = newSortingOrder;
        }
        else
        {
            sr.sortingOrder = newSortingOrder;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsAffectedEntity(collision.gameObject)) return;

        var sr = collision.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        // Restaurar sorting order original
        if (originalSortingOrders.TryGetValue(sr, out int originalOrder))
        {
            if (smoothTransition)
            {
                targetSortingOrders[sr] = originalOrder;
            }
            else
            {
                sr.sortingOrder = originalOrder;
            }

            originalSortingOrders.Remove(sr);

            if (showDebugLogs)
            {
                Debug.Log($"[FakeDepthZone] {collision.name} saiu → sorting restaurado para {originalOrder}");
            }
        }

        if (targetSortingOrders.ContainsKey(sr))
        {
            targetSortingOrders.Remove(sr);
        }
    }

    private bool IsAffectedEntity(GameObject obj)
    {
        if (affectedTags.Length == 0) return true; // Sem filtro, afeta todos

        foreach (var tag in affectedTags)
        {
            if (obj.CompareTag(tag)) return true;
        }

        return false;
    }

    void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        var collider = GetComponent<BoxCollider2D>();
        if (collider == null) return;

        // Desenhar zona de trigger
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        Gizmos.DrawCube(transform.position, collider.size);

        // Desenhar linha de threshold
        Gizmos.color = Color.cyan;
        float thresholdY = transform.position.y + depthThresholdY;
        Vector3 lineStart = new Vector3(
            transform.position.x - collider.size.x / 2,
            thresholdY,
            transform.position.z
        );
        Vector3 lineEnd = new Vector3(
            transform.position.x + collider.size.x / 2,
            thresholdY,
            transform.position.z
        );
        Gizmos.DrawLine(lineStart, lineEnd);
    }

    void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) return;

        #if UNITY_EDITOR
        var collider = GetComponent<BoxCollider2D>();
        if (collider == null) return;

        float thresholdY = transform.position.y + depthThresholdY;

        // Label com informações
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * (collider.size.y / 2 + 0.5f),
            $"Fake Depth Zone\n" +
            $"Behind Sorting: {behindSortingOrder}\n" +
            $"Front Sorting: {frontSortingOrder}\n" +
            $"Threshold Y: {depthThresholdY:F2} (world: {thresholdY:F2})"
        );

        // Setas indicando direções
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.ArrowHandleCap(
            0,
            new Vector3(transform.position.x, thresholdY - 0.5f, transform.position.z),
            Quaternion.Euler(0, 0, -90),
            1f,
            EventType.Repaint
        );
        UnityEditor.Handles.Label(
            new Vector3(transform.position.x - 1.5f, thresholdY - 0.5f, transform.position.z),
            "BEHIND"
        );

        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.ArrowHandleCap(
            0,
            new Vector3(transform.position.x, thresholdY + 0.5f, transform.position.z),
            Quaternion.Euler(0, 0, 90),
            1f,
            EventType.Repaint
        );
        UnityEditor.Handles.Label(
            new Vector3(transform.position.x - 1.5f, thresholdY + 0.5f, transform.position.z),
            "FRONT"
        );
        #endif
    }
}
```

### 4.3 Setup

**PASSO 1: Preparar o Morro**

1. Morro deve ter collider COMPLETO (incluindo cantos)
2. Criar GameObject filho vazio: `FakeDepthZone`
3. Posicionar na base do morro

**PASSO 2: Configurar FakeDepthZone**

1. Adicionar `Box Collider 2D` ao FakeDepthZone
   - Marcar "Is Trigger" ✅
   - Size: cobrir área onde player pode passar

2. Adicionar `FakeDepthZone.cs`
   - Occluding Structure: (arrastar sprite do morro)
   - Behind Sorting Order: 500 (atrás do morro)
   - Front Sorting Order: 2000 (na frente do morro)
   - Depth Threshold Y: -2 (ajustar conforme necessário)
   - Affected Tags: ["Player"]

**PASSO 3: Testar**

- Player se move para Y baixo → desaparece atrás do morro ✅
- Player se move para Y alto → aparece na frente do morro ✅
- Player AINDA COLIDE com o morro (não passa através!) ✅

### 4.4 Vantagens e Desvantagens

**✅ Vantagens:**
- Simples de implementar
- Não precisa de múltiplos colliders
- Não precisa configurar Collision Matrix
- Efeito visual convincente

**❌ Desvantagens:**
- Player não REALMENTE passa por trás (ainda colide)
- Pode parecer estranho se player tentar "empurrar" o morro
- Não resolve o problema do Arqueiro escapando
- Puramente visual, não é física real

**Quando Usar:**
- ✅ Prototipagem rápida
- ✅ Demonstração visual
- ✅ Áreas onde colisão é desejada mas visual precisa mudar

**Quando NÃO Usar:**
- ❌ Quando precisa de física real (passar através)
- ❌ Quando arqueiro também precisa ser bloqueado
- ❌ Produção (use Solução 1 ou 2)

---

## 5. Comparação de Soluções

### 5.1 Tabela Comparativa

| Critério | Solução 1 (Layers) | Solução 2 (Script) | Solução 3 (Fake) |
|----------|--------------------|--------------------|------------------|
| **Performance** | ⭐⭐⭐⭐⭐ Excelente | ⭐⭐⭐ Boa | ⭐⭐⭐⭐ Muito Boa |
| **Simplicidade** | ⭐⭐⭐⭐ Fácil | ⭐⭐ Moderado | ⭐⭐⭐⭐⭐ Muito Fácil |
| **Escalabilidade** | ⭐⭐⭐⭐⭐ Excelente | ⭐⭐⭐ Boa | ⭐⭐ Limitada |
| **Física Real** | ✅ Sim | ✅ Sim | ❌ Não (visual apenas) |
| **Bloqueia Arqueiro** | ✅ Sim | ✅ Sim | ❌ Não |
| **Player Passa Atrás** | ✅ Sim (física) | ✅ Sim (física) | ⚠️ Sim (visual) |
| **Customização** | ⭐⭐⭐ Boa | ⭐⭐⭐⭐⭐ Excelente | ⭐⭐⭐ Boa |
| **Debug** | ⭐⭐⭐⭐ Fácil | ⭐⭐ Difícil | ⭐⭐⭐⭐ Fácil |
| **Requer Config** | Collision Matrix | Nenhuma | Nenhuma |
| **Múltiplos Colliders** | ✅ Sim (2) | ❌ Não (1) | ❌ Não (1+trigger) |

### 5.2 Recomendações por Caso de Uso

**Para Produção (Jogo Completo):**
- 🥇 **Solução 1 (Layer-Based)** - Mais robusta e performática

**Para Prototipagem Rápida:**
- 🥇 **Solução 3 (Fake Depth)** - Implementação em minutos

**Para Casos Especiais (lógica complexa):**
- 🥇 **Solução 2 (Script-Based)** - Máximo controle

**Para Seu Projeto Atual:**
- 🥇 **Solução 1** - Combina com o sistema de Layers.MD já documentado

### 5.3 Combinações Possíveis

Você pode **combinar soluções** para casos específicos:

**Exemplo: Layers + Fake Depth**
```
• Usar Solução 1 para física (player passa de verdade)
• Adicionar Solução 3 para melhorar visual (sorting order dinâmico)
• Resultado: Melhor dos dois mundos!
```

**Exemplo: Layers + Script para casos especiais**
```
• Usar Solução 1 como base (maioria dos casos)
• Adicionar Solução 2 em estruturas específicas (pontes, portais)
• Resultado: Flexibilidade onde necessário
```

---

## 6. Implementação Detalhada

### 6.1 Checklist de Implementação (Solução 1)

```
□ PREPARAÇÃO
  □ Layers criadas no Project Settings (Layer 6-9)
  □ Collision Matrix configurada
  □ ElevationManager adicionado à cena

□ SCRIPTS
  □ MountainCollisionSetup.cs criado
  □ ElevationHelper.cs disponível (do Layers.MD)

□ MORRO
  □ GameObject do morro preparado
  □ Sprite visual configurado
  □ MountainCollisionSetup.cs adicionado
  □ Full Bounds Size definido
  □ Center Bounds Size definido
  □ "Setup Colliders" executado

□ ENTIDADES
  □ Player tem ElevationEntity (Starting Level: Ground)
  □ Arqueiro tem ElevationEntity (Starting Level: Platform)
  □ Layers corretos configurados

□ TESTES
  □ Player passa pelos cantos ✅
  □ Player colide com o meio ✅
  □ Arqueiro bloqueado em toda área ✅
  □ Sorting order visual correto ✅

□ POLIMENTO
  □ Ajustar tamanhos de bounds conforme necessário
  □ Adicionar OcclusionZone se quiser efeito de esconder
  □ Testar com múltiplos enemies
  □ Performance OK (verificar Profiler)
```

### 6.2 Exemplo Completo de Setup

**Estrutura Final do Morro:**

```
Hierarchy:
└── Mountain_Level1
    ├── Visual (SpriteRenderer)
    │   • Sprite: mountain_sprite
    │   • Sorting Layer: Level1Ground
    │   • Sorting Order: 500
    │
    ├── MountainCollisionSetup.cs
    │   • Structure Level: Platform (Level1)
    │   • Full Bounds: (10, 8)
    │   • Center Bounds: (6, 5)
    │
    ├── TerrainCollider_SameLevel (auto-criado)
    │   • BoxCollider2D
    │   • Layer: ElevationLevel1
    │   • Size: (10, 8)
    │
    ├── DepthBounds_LevelBelow (auto-criado)
    │   • BoxCollider2D
    │   • Layer: ElevationLevel0
    │   • Size: (6, 5)
    │
    └── OcclusionZone_Back (opcional)
        • FakeDepthZone.cs (para efeito visual extra)
```

### 6.3 Código de Teste

```csharp
// ElevationTester.cs - Script para debugar colisões
using UnityEngine;

public class ElevationTester : MonoBehaviour
{
    [Header("Teste")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject archer;
    [SerializeField] private GameObject mountain;

    [ContextMenu("Test Player Collision")]
    void TestPlayerCollision()
    {
        if (player == null || mountain == null)
        {
            Debug.LogError("Player ou Mountain não definidos!");
            return;
        }

        var playerEntity = player.GetComponent<ElevationEntity>();
        if (playerEntity == null)
        {
            Debug.LogError("Player precisa de ElevationEntity!");
            return;
        }

        Debug.Log($"Player Level: {playerEntity.CurrentLevel}");
        Debug.Log($"Player Layer: {LayerMask.LayerToName(player.layer)}");

        // Verificar se colide com cada collider do morro
        var mountainColliders = mountain.GetComponentsInChildren<Collider2D>();
        foreach (var col in mountainColliders)
        {
            bool willCollide = !Physics2D.GetIgnoreLayerCollision(player.layer, col.gameObject.layer);
            Debug.Log($"  Collider '{col.name}' (Layer: {LayerMask.LayerToName(col.gameObject.layer)}) → " +
                      $"{(willCollide ? "✅ COLIDE" : "❌ NÃO COLIDE")}");
        }
    }

    [ContextMenu("Test Archer Collision")]
    void TestArcherCollision()
    {
        if (archer == null || mountain == null)
        {
            Debug.LogError("Archer ou Mountain não definidos!");
            return;
        }

        var archerEntity = archer.GetComponent<ElevationEntity>();
        if (archerEntity == null)
        {
            Debug.LogError("Archer precisa de ElevationEntity!");
            return;
        }

        Debug.Log($"Archer Level: {archerEntity.CurrentLevel}");
        Debug.Log($"Archer Layer: {LayerMask.LayerToName(archer.layer)}");

        var mountainColliders = mountain.GetComponentsInChildren<Collider2D>();
        foreach (var col in mountainColliders)
        {
            bool willCollide = !Physics2D.GetIgnoreLayerCollision(archer.layer, col.gameObject.layer);
            Debug.Log($"  Collider '{col.name}' (Layer: {LayerMask.LayerToName(col.gameObject.layer)}) → " +
                      $"{(willCollide ? "✅ COLIDE" : "❌ NÃO COLIDE")}");
        }
    }
}
```

---

## 7. Troubleshooting

### 7.1 Problemas Comuns

**Problema 1: Player não passa pelos cantos (ainda colide)**

```
Possíveis causas:
❌ Center Bounds muito grande (cobre cantos)
❌ Collision Matrix não configurada
❌ Player no layer errado

Soluções:
✅ Reduzir Center Bounds Size para ~60% do Full Bounds
✅ Verificar Physics 2D Settings → Collision Matrix
✅ Debug: ElevationHelper.DebugElevationInfo(player)
```

**Problema 2: Arqueiro escapa pelos cantos**

```
Possíveis causas:
❌ Full Bounds muito pequeno
❌ Arqueiro no layer errado (Level0 em vez de Level1)
❌ TerrainCollider não criado

Soluções:
✅ Aumentar Full Bounds Size
✅ Verificar Arqueiro.layer == ElevationLevel1
✅ Clicar "Setup Colliders" novamente no MountainCollisionSetup
```

**Problema 3: Colliders não aparecem no Scene view**

```
Possíveis causas:
❌ Gizmos desabilitados
❌ Script não executou Setup

Soluções:
✅ Scene view → Gizmos button (topo direito) → ON
✅ Selecionar morro → Inspector → "Setup Colliders"
✅ Verificar Hierarchy: devem existir filhos "TerrainCollider_SameLevel" e "DepthBounds_LevelBelow"
```

**Problema 4: Performance ruim com muitos morros**

```
Possíveis causas:
❌ Usando Solução 2 (Script-Based) para tudo
❌ Muitos colliders complexos (PolygonCollider2D)

Soluções:
✅ Usar Solução 1 (Layer-Based) - mais performática
✅ Preferir BoxCollider2D em vez de PolygonCollider2D
✅ Usar Tilemap Collider 2D quando possível
```

### 7.2 Debug Tools

**Visualizar Collision Matrix:**

```csharp
// ColisionMatrixDebugger.cs
using UnityEngine;

public class CollisionMatrixDebugger : MonoBehaviour
{
    [ContextMenu("Print Collision Matrix")]
    void PrintCollisionMatrix()
    {
        Debug.Log("=== COLLISION MATRIX ===");

        for (int i = 6; i <= 9; i++) // Elevation layers
        {
            string layerName = LayerMask.LayerToName(i);
            Debug.Log($"\nLayer {i} ({layerName}) colide com:");

            for (int j = 6; j <= 9; j++)
            {
                string otherLayerName = LayerMask.LayerToName(j);
                bool collides = !Physics2D.GetIgnoreLayerCollision(i, j);
                Debug.Log($"  Layer {j} ({otherLayerName}): {(collides ? "✅ SIM" : "❌ NÃO")}");
            }
        }
    }
}
```

**Visualizar Bounds no Play Mode:**

```csharp
// BoundsVisualizer.cs
using UnityEngine;

public class BoundsVisualizer : MonoBehaviour
{
    [SerializeField] private bool showInPlayMode = true;
    [SerializeField] private MountainCollisionSetup mountainSetup;

    void OnDrawGizmos()
    {
        if (!showInPlayMode && Application.isPlaying) return;

        if (mountainSetup != null)
        {
            // Delegar ao MountainCollisionSetup para desenhar
            // (já tem OnDrawGizmos)
        }
    }
}
```

### 7.3 Validação Automática

```csharp
// Adicionar ao MountainCollisionSetup.cs:

[ContextMenu("Validate Setup")]
public void ValidateSetup()
{
    Debug.Log("=== VALIDANDO SETUP ===");

    bool valid = true;

    // 1. Verificar layers existem
    int sameLayerID = ElevationHelper.LevelToLayer(structureLevel);
    if (LayerMask.LayerToName(sameLayerID) == "")
    {
        Debug.LogError($"❌ Layer {sameLayerID} não configurado!");
        valid = false;
    }
    else
    {
        Debug.Log($"✅ Layer {sameLayerID} ({LayerMask.LayerToName(sameLayerID)}) OK");
    }

    // 2. Verificar colliders criados
    var terrain = transform.Find("TerrainCollider_SameLevel");
    if (terrain == null)
    {
        Debug.LogError("❌ TerrainCollider não encontrado! Execute 'Setup Colliders'");
        valid = false;
    }
    else
    {
        Debug.Log("✅ TerrainCollider OK");
    }

    var depth = transform.Find("DepthBounds_LevelBelow");
    if (depth == null && (int)structureLevel > 0)
    {
        Debug.LogError("❌ DepthBoundsCollider não encontrado! Execute 'Setup Colliders'");
        valid = false;
    }
    else if (depth != null)
    {
        Debug.Log("✅ DepthBoundsCollider OK");
    }

    // 3. Verificar Collision Matrix
    if ((int)structureLevel > 0)
    {
        int belowLayer = ElevationHelper.LevelToLayer((ElevationManager.ElevationLevel)((int)structureLevel - 1));
        bool shouldNotCollide = Physics2D.GetIgnoreLayerCollision(sameLayerID, belowLayer);

        if (!shouldNotCollide)
        {
            Debug.LogWarning($"⚠️ Collision Matrix: Layer {sameLayerID} colide com {belowLayer}! Deveria estar DESMARCADO.");
            valid = false;
        }
        else
        {
            Debug.Log("✅ Collision Matrix configurada corretamente");
        }
    }

    // 4. Verificar tamanhos
    if (centerBoundsSize.x >= fullBoundsSize.x || centerBoundsSize.y >= fullBoundsSize.y)
    {
        Debug.LogWarning("⚠️ Center Bounds >= Full Bounds! Cantos muito pequenos.");
    }
    else
    {
        Debug.Log($"✅ Bounds configurados: Full={fullBoundsSize}, Center={centerBoundsSize}");
    }

    if (valid)
    {
        Debug.Log("=== ✅ SETUP VÁLIDO! ===");
    }
    else
    {
        Debug.LogError("=== ❌ SETUP INVÁLIDO! Corrigir erros acima. ===");
    }
}
```

---

## Conclusão

### Resumo das Soluções

1. **Solução 1: Layer-Based Collision** ⭐ **RECOMENDADA**
   - Dois colliders em layers diferentes
   - Unity gerencia colisão via Collision Matrix
   - Mais performática e escalável

2. **Solução 2: Conditional Collision**
   - Collider único com lógica via script
   - Flexível para casos especiais
   - Menos performática

3. **Solução 3: Visual Trickery**
   - Ilusão visual sem física real
   - Rápida para prototipar
   - Não resolve problema do Arqueiro

### Decisão Final

**Para seu projeto, recomendo Solução 1** porque:
- ✅ Resolve ambos os problemas (Player passa, Arqueiro bloqueado)
- ✅ Integra com sistema de Layers.MD já documentado
- ✅ Performática para múltiplos morros/entidades
- ✅ Fácil de debugar e manter

### Próximos Passos

1. Implementar `MountainCollisionSetup.cs`
2. Configurar Collision Matrix
3. Adicionar ao morro existente
4. Testar com Player e Arqueiro
5. Ajustar bounds conforme necessário
6. Expandir para outras estruturas (pontes, plataformas)

---

**Sua abordagem estava no caminho certo!** Você só precisava de **dois colliders** em vez de um. Agora você tem a solução completa! 🚀

---

*Documentação criada em 2025-11-13*
*Complemento ao Layers.MD*
*Para: TopDown2D Crash Course*
