# Tutorial: Configurando Pickups Avançados

## 🎯 Objetivo
Aprender técnicas avançadas para configurar diferentes tipos de pickups, desde drops dinâmicos até sistemas de loot complexos.

---

## 📋 Pré-requisitos
- ✅ [Tutorial: Criando Novos Itens](CreatingNewItems.md) completado
- ✅ Compreensão básica do ItemPickup
- ✅ Sistema de inventário funcionando

---

## 🎮 Cenários de Pickup

### **1. 🎯 Static World Pickups (Itens Estáticos)**
### **2. ⚔️ Enemy Loot Drops (Drops de Inimigos)**  
### **3. 🏺 Breakable Object Loot (Objetos Quebráveis)**
### **4. 🎁 Treasure Chests (Baús de Tesouro)**
### **5. 🌿 Harvestable Resources (Recursos Coletáveis)**

---

## 1. 🎯 Static World Pickups

### **Cenário:** Itens fixos espalhados pelo mapa

**Casos de uso:**
- Minerais em cavernas
- Plantas medicinais na floresta
- Moedas escondidas

### **Setup Básico**
```yaml
GameObject: "MinerioFerro_001"
├── SpriteRenderer (ferro_sprite.png)
├── CircleCollider2D (IsTrigger = true)
├── ItemPickup
│   ├── Item = MinerioDeFerro.asset
│   ├── Quantity = 1
│   ├── Pickup Mode = Trigger
│   └── Destroy On Pickup = true
└── AudioSource (pickup_sound.wav)
```

### **Variações Avançadas**

#### **A) Respawn após tempo**
```csharp
public class RespawnablePickup : MonoBehaviour
{
    public float respawnTime = 300f; // 5 minutos
    public GameObject pickupPrefab;
    
    private Vector3 originalPosition;
    
    void Start()
    {
        originalPosition = transform.position;
    }
    
    public void OnItemPickedUp()
    {
        StartCoroutine(RespawnAfterTime());
    }
    
    private IEnumerator RespawnAfterTime()
    {
        yield return new WaitForSeconds(respawnTime);
        
        GameObject newPickup = Instantiate(pickupPrefab, originalPosition, Quaternion.identity);
        RespawnablePickup respawnable = newPickup.GetComponent<RespawnablePickup>();
        if (respawnable != null)
        {
            respawnable.pickupPrefab = pickupPrefab;
        }
    }
}
```

#### **B) Quantidade aleatória**
```csharp
public class RandomQuantityPickup : MonoBehaviour
{
    void Start()
    {
        ItemPickup pickup = GetComponent<ItemPickup>();
        if (pickup != null)
        {
            pickup.quantity = Random.Range(1, 5); // 1-4 itens
        }
    }
}
```

---

## 2. ⚔️ Enemy Loot Drops

### **Cenário:** Inimigos dropam loot ao morrer

### **Setup no Enemy Script**
```csharp
public class Enemy : MonoBehaviour
{
    [Header("Loot Settings")]
    public LootTable lootTable;
    public GameObject lootDropPrefab; // Prefab genérico para drops
    public int minDrops = 0;
    public int maxDrops = 3;
    public float dropRadius = 2f;
    
    void OnDeath()
    {
        DropLoot();
        // ... resto da lógica de morte
    }
    
    void DropLoot()
    {
        int dropCount = Random.Range(minDrops, maxDrops + 1);
        
        for (int i = 0; i < dropCount; i++)
        {
            LootItem loot = lootTable.GetRandomLoot();
            if (loot != null && loot.item != null)
            {
                SpawnLootDrop(loot);
            }
        }
    }
    
    void SpawnLootDrop(LootItem loot)
    {
        // Posição aleatória ao redor do inimigo
        Vector3 dropPosition = transform.position + 
                              Random.insideUnitSphere * dropRadius;
        dropPosition.z = 0; // Manter em 2D
        
        GameObject drop = Instantiate(lootDropPrefab, dropPosition, Quaternion.identity);
        
        ItemPickup pickup = drop.GetComponent<ItemPickup>();
        if (pickup != null)
        {
            pickup.item = loot.item;
            pickup.quantity = Random.Range(loot.minQuantity, loot.maxQuantity + 1);
            
            // Aplicar sprite do item automaticamente
            SpriteRenderer sr = drop.GetComponent<SpriteRenderer>();
            if (sr != null && loot.item.icon != null)
            {
                sr.sprite = loot.item.icon;
            }
        }
    }
}

[System.Serializable]
public class LootItem
{
    public Item item;
    public int minQuantity = 1;
    public int maxQuantity = 1;
    [Range(0f, 100f)]
    public float dropChance = 50f;
}
```

### **Loot Table ScriptableObject**
```csharp
[CreateAssetMenu(fileName = "NewLootTable", menuName = "Inventory/Loot Table")]
public class LootTable : ScriptableObject
{
    public LootItem[] possibleLoot;
    
    public LootItem GetRandomLoot()
    {
        List<LootItem> validLoot = new List<LootItem>();
        
        foreach (LootItem loot in possibleLoot)
        {
            if (Random.Range(0f, 100f) <= loot.dropChance)
            {
                validLoot.Add(loot);
            }
        }
        
        if (validLoot.Count > 0)
        {
            return validLoot[Random.Range(0, validLoot.Count)];
        }
        
        return null;
    }
}
```

---

## 3. 🏺 Breakable Objects

### **Cenário:** Caixotes, barris, pedras que dropam loot

### **BreakableObject Script**
```csharp
public class BreakableObject : MonoBehaviour
{
    [Header("Breakable Settings")]
    public int health = 1;
    public LootTable lootTable;
    public GameObject lootDropPrefab;
    public GameObject breakEffect; // Particle effect
    
    [Header("Audio")]
    public AudioClip breakSound;
    
    public void TakeDamage(int damage)
    {
        health -= damage;
        
        if (health <= 0)
        {
            Break();
        }
    }
    
    void Break()
    {
        DropLoot();
        PlayBreakEffect();
        PlayBreakSound();
        Destroy(gameObject);
    }
    
    void DropLoot()
    {
        if (lootTable == null || lootDropPrefab == null) return;
        
        LootItem loot = lootTable.GetRandomLoot();
        if (loot != null)
        {
            GameObject drop = Instantiate(lootDropPrefab, transform.position, Quaternion.identity);
            
            ItemPickup pickup = drop.GetComponent<ItemPickup>();
            if (pickup != null)
            {
                pickup.item = loot.item;
                pickup.quantity = Random.Range(loot.minQuantity, loot.maxQuantity + 1);
            }
        }
    }
    
    void PlayBreakEffect()
    {
        if (breakEffect != null)
        {
            Instantiate(breakEffect, transform.position, Quaternion.identity);
        }
    }
    
    void PlayBreakSound()
    {
        if (breakSound != null)
        {
            AudioSource.PlayClipAtPoint(breakSound, transform.position);
        }
    }
}
```

---

## 4. 🎁 Treasure Chests

### **Cenário:** Baús que contêm múltiplos itens valiosos

### **TreasureChest Script**
```csharp
public class TreasureChest : MonoBehaviour
{
    [Header("Chest Settings")]
    public bool isOpened = false;
    public LootTable lootTable;
    public GameObject lootDropPrefab;
    public int guaranteedDrops = 3;
    public float openRadius = 2f;
    
    [Header("Visual")]
    public Sprite closedSprite;
    public Sprite openedSprite;
    public ParticleSystem openEffect;
    
    [Header("Audio")]
    public AudioClip openSound;
    
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        
        if (spriteRenderer != null && closedSprite != null)
        {
            spriteRenderer.sprite = closedSprite;
        }
    }
    
    void Update()
    {
        if (!isOpened)
        {
            CheckForPlayer();
        }
    }
    
    void CheckForPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance <= openRadius && Input.GetKeyDown(KeyCode.E))
            {
                OpenChest();
            }
        }
    }
    
    public void OpenChest()
    {
        if (isOpened) return;
        
        isOpened = true;
        
        // Mudar sprite
        if (spriteRenderer != null && openedSprite != null)
        {
            spriteRenderer.sprite = openedSprite;
        }
        
        // Efeitos
        PlayOpenEffect();
        PlayOpenSound();
        
        // Dropar loot
        DropTreasure();
    }
    
    void DropTreasure()
    {
        for (int i = 0; i < guaranteedDrops; i++)
        {
            LootItem loot = lootTable.GetRandomLoot();
            if (loot != null)
            {
                Vector3 dropPos = transform.position + Random.insideUnitSphere * 1.5f;
                dropPos.z = 0;
                
                GameObject drop = Instantiate(lootDropPrefab, dropPos, Quaternion.identity);
                
                ItemPickup pickup = drop.GetComponent<ItemPickup>();
                if (pickup != null)
                {
                    pickup.item = loot.item;
                    pickup.quantity = Random.Range(loot.minQuantity, loot.maxQuantity + 1);
                }
            }
        }
    }
    
    void PlayOpenEffect()
    {
        if (openEffect != null)
        {
            openEffect.Play();
        }
    }
    
    void PlayOpenSound()
    {
        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, openRadius);
    }
}
```

---

## 5. 🌿 Harvestable Resources

### **Cenário:** Recursos que regeneram após tempo (árvores, plantas)

### **HarvestableResource Script**
```csharp
public class HarvestableResource : MonoBehaviour
{
    [Header("Harvest Settings")]
    public Item harvestedItem;
    public int baseYield = 3;
    public int maxYield = 5;
    public float harvestTime = 2f; // Tempo para colher
    public float regrowTime = 180f; // 3 minutos
    
    [Header("States")]
    public bool isHarvestable = true;
    public bool isHarvesting = false;
    
    [Header("Visual")]
    public Sprite fullSprite;
    public Sprite emptySprite;
    
    private SpriteRenderer spriteRenderer;
    private float harvestProgress = 0f;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisuals();
    }
    
    void Update()
    {
        if (isHarvesting)
        {
            harvestProgress += Time.deltaTime;
            
            if (harvestProgress >= harvestTime)
            {
                CompleteHarvest();
            }
        }
        
        CheckForPlayer();
    }
    
    void CheckForPlayer()
    {
        if (!isHarvestable || isHarvesting) return;
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance <= 1.5f)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartHarvest();
                }
            }
        }
    }
    
    void StartHarvest()
    {
        isHarvesting = true;
        harvestProgress = 0f;
        Debug.Log("Iniciando coleta...");
    }
    
    void CompleteHarvest()
    {
        isHarvesting = false;
        isHarvestable = false;
        harvestProgress = 0f;
        
        // Calcular yield
        int yield = Random.Range(baseYield, maxYield + 1);
        
        // Spawn items
        for (int i = 0; i < yield; i++)
        {
            SpawnHarvestedItem();
        }
        
        UpdateVisuals();
        StartCoroutine(RegrowAfterTime());
        
        Debug.Log($"Coletado {yield} {harvestedItem.itemName}(s)!");
    }
    
    void SpawnHarvestedItem()
    {
        Vector3 dropPos = transform.position + Random.insideUnitSphere * 1f;
        dropPos.z = 0;
        
        // Criar pickup temporário
        GameObject pickup = new GameObject($"{harvestedItem.itemName}_Pickup");
        pickup.transform.position = dropPos;
        
        SpriteRenderer sr = pickup.AddComponent<SpriteRenderer>();
        sr.sprite = harvestedItem.icon;
        sr.sortingOrder = 10;
        
        CircleCollider2D col = pickup.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.5f;
        
        ItemPickup itemPickup = pickup.AddComponent<ItemPickup>();
        itemPickup.item = harvestedItem;
        itemPickup.quantity = 1;
        itemPickup.pickupMode = PickupMode.Trigger;
    }
    
    IEnumerator RegrowAfterTime()
    {
        yield return new WaitForSeconds(regrowTime);
        
        isHarvestable = true;
        UpdateVisuals();
        
        Debug.Log($"{harvestedItem.itemName} resource regenerated!");
    }
    
    void UpdateVisuals()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = isHarvestable ? fullSprite : emptySprite;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = isHarvestable ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
        
        if (isHarvesting)
        {
            Gizmos.color = Color.yellow;
            float progress = harvestProgress / harvestTime;
            Gizmos.DrawWireSphere(transform.position, 1.5f * progress);
        }
    }
}
```

---

## 🔧 Configurações Avançadas

### **🎚️ Drop Physics**
```csharp
public class PhysicsPickup : MonoBehaviour
{
    void Start()
    {
        // Adicionar física para drops mais realistas
        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
        rb.drag = 2f;
        
        // Força inicial aleatória
        Vector2 randomForce = Random.insideUnitCircle * 5f;
        rb.AddForce(randomForce, ForceMode2D.Impulse);
        
        // Parar física após pouco tempo
        StartCoroutine(DisablePhysicsAfterTime(2f));
    }
    
    IEnumerator DisablePhysicsAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }
    }
}
```

### **✨ Rarity-Based Effects**
```csharp
public class RarityEffects : MonoBehaviour
{
    void Start()
    {
        ItemPickup pickup = GetComponent<ItemPickup>();
        if (pickup != null && pickup.item != null)
        {
            ApplyRarityEffects(pickup.item.rarity);
        }
    }
    
    void ApplyRarityEffects(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Common:
                // Sem efeitos especiais
                break;
                
            case ItemRarity.Uncommon:
                // Bobbing mais rápido
                ItemPickup pickup = GetComponent<ItemPickup>();
                pickup.bobSpeed = 3f;
                break;
                
            case ItemRarity.Rare:
                // Efeito de brilho
                AddGlowEffect(Color.blue);
                break;
                
            case ItemRarity.Epic:
                // Efeito de partículas
                AddParticleEffect();
                AddGlowEffect(Color.purple);
                break;
                
            case ItemRarity.Legendary:
                // Múltiplos efeitos
                AddParticleEffect();
                AddGlowEffect(Color.gold);
                AddPulseEffect();
                break;
        }
    }
    
    void AddGlowEffect(Color color)
    {
        // Implementar efeito de brilho
    }
    
    void AddParticleEffect()
    {
        // Adicionar sistema de partículas
    }
    
    void AddPulseEffect()
    {
        // Efeito de pulsação na escala
    }
}
```

---

## 🚨 Otimização e Performance

### **📊 Object Pooling para Drops**
```csharp
public class PickupPool : MonoBehaviour
{
    public static PickupPool Instance;
    
    [Header("Pool Settings")]
    public GameObject pickupPrefab;
    public int poolSize = 50;
    
    private Queue<GameObject> pooledPickups = new Queue<GameObject>();
    
    void Awake()
    {
        Instance = this;
        InitializePool();
    }
    
    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject pickup = Instantiate(pickupPrefab);
            pickup.SetActive(false);
            pooledPickups.Enqueue(pickup);
        }
    }
    
    public GameObject GetPooledPickup()
    {
        if (pooledPickups.Count > 0)
        {
            GameObject pickup = pooledPickups.Dequeue();
            pickup.SetActive(true);
            return pickup;
        }
        
        // Se pool vazio, criar novo
        return Instantiate(pickupPrefab);
    }
    
    public void ReturnToPool(GameObject pickup)
    {
        pickup.SetActive(false);
        pooledPickups.Enqueue(pickup);
    }
}
```

### **⚡ Culling de Pickups**
```csharp
public class PickupCuller : MonoBehaviour
{
    public float cullDistance = 50f;
    
    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            
            if (distance > cullDistance)
            {
                // Remover pickup muito distante
                gameObject.SetActive(false);
            }
        }
    }
}
```

---

## 📋 Checklist de Configuração

Para cada tipo de pickup:

### **🎯 Static Pickups**
- [ ] ✅ Sprite configurado
- [ ] ✅ Collider como trigger
- [ ] ✅ ItemPickup configurado
- [ ] ✅ Respawn logic (se necessário)

### **⚔️ Enemy Drops**
- [ ] ✅ LootTable criada e configurada
- [ ] ✅ Enemy.OnDeath() chama DropLoot()
- [ ] ✅ Prefab genérico de drop configurado
- [ ] ✅ Physics/positioning correto

### **🏺 Breakable Objects**
- [ ] ✅ Health system implementado
- [ ] ✅ Damage detection configurado
- [ ] ✅ Break effects funcionando
- [ ] ✅ Loot spawning correto

### **🎁 Treasure Chests**
- [ ] ✅ Interaction system (E key)
- [ ] ✅ Visual feedback (sprite change)
- [ ] ✅ Multiple drops configurados
- [ ] ✅ One-time use garantido

### **🌿 Harvestable Resources**
- [ ] ✅ Harvest timing implementado
- [ ] ✅ Visual feedback durante harvest
- [ ] ✅ Regrow timer funcionando
- [ ] ✅ Yield balanceado

---

## 📖 Próximos Passos

1. **[Integrando com Inimigos](IntegratingWithEnemies.md)** - Sistemas avançados de loot
2. **Sistema de UI** - Mostrar progresso de harvest
3. **Sistema de Equipamentos** - Usar ferramentas para harvest
4. **Sistema de Crafting** - Usar materiais coletados

---

**Última atualização:** 2024-11-10  
**Dificuldade:** ⭐⭐⭐⭐ Avançado  
**Tempo estimado:** 2-4 horas para implementar todos os sistemas