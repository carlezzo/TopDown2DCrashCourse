# Tree System Documentation

## Overview

The **Tree System** provides a complete implementation of destructible environment objects (trees) that can be damaged, drop resources, and provide visual/audio feedback. This system demonstrates the component-based architecture used throughout the project and can be extended to other destructible environment objects (rocks, bushes, crates, etc.).

**Key Features:**
- ✅ Health system integration (takes damage from player attacks)
- ✅ Shake animation on damage
- ✅ Particle effects (falling leaves)
- ✅ Audio feedback (hit sounds)
- ✅ Resource dropping (wood items)
- ✅ Event-driven architecture
- ✅ Fully configurable in Inspector

---

## Architecture

### Component Composition

A complete tree GameObject requires the following components:

```
Tree GameObject
├── SpriteRenderer          (visual representation)
├── Collider2D              (collision detection)
├── HealthComponent         (damage system)
├── DropOnDeath             (resource dropping)
├── TreeController          (visual/audio feedback)
└── ParticleSystem          (optional: leaf particles)
    └── LeafParticles (child GameObject)
```

### Component Responsibilities

| Component | Responsibility | Events Used |
|-----------|---------------|-------------|
| **SpriteRenderer** | Displays tree sprite | - |
| **Collider2D** | Detects sword attacks | - |
| **HealthComponent** | Manages health, invokes death | `OnHealthChanged`, `OnDeath` |
| **DropOnDeath** | Spawns wood items on death | Listens: `OnDeath` |
| **TreeController** | Shake animation, audio, particles | Listens: `OnHealthChanged` |
| **ParticleSystem** | Visual effect (leaves falling) | Triggered by TreeController |

### Event Flow

```
Player attacks Tree
        ↓
SwordAttack.OnTriggerEnter2D
        ↓
Detects "Enemy" tag
        ↓
HealthComponent.TakeDamage(3)
        ↓
┌───────┴───────┐
│               │
OnHealthChanged OnDeath (if health <= 0)
│               │
TreeController  DropOnDeath
│               │
├─ Shake        └─ Spawn wood items
├─ Particles
└─ Sound
```

---

## Setup Guide

### Step 1: Create Tree GameObject

1. **Create Empty GameObject:**
   - Right-click in Hierarchy → **Create Empty**
   - Rename to "Tree"

2. **Add Sprite:**
   - Add Component → **Sprite Renderer**
   - Assign tree sprite to **Sprite** field
   - Set **Sorting Layer** appropriately (e.g., "Environment")

3. **Add Collider:**
   - Add Component → **Box Collider 2D** or **Capsule Collider 2D**
   - Adjust size to cover tree trunk (not leaves)
   - **Is Trigger:** ❌ UNCHECKED (tree is a solid obstacle)

4. **Set Tag:**
   - In Inspector header, set **Tag** to **"Enemy"**
   - This allows SwordAttack to detect and damage the tree

---

### Step 2: Add Health System

1. **Add HealthComponent:**
   - Add Component → **Health Component**
   - Configure:
     - **Max Health:** `9` (3 hits × 3 damage = 9)
     - **Health Bar:** Leave empty (trees don't show health bars)

2. **Add DropOnDeath:**
   - Add Component → **Drop On Death**
   - Configure:
     - **Drop Table:** Assign `TreeDropTable` asset (create if needed)
     - **Drop Radius:** `0.5` (wood spreads around tree)

---

### Step 3: Add TreeController

1. **Add TreeController:**
   - Add Component → **Tree Controller**
   - The script automatically finds HealthComponent

2. **Configure Shake Animation:**
   - **Shake Intensity:** `0.1` (how much the tree shakes)
   - **Shake Duration:** `0.2` seconds
   - **Shake Speed:** `30` (oscillation frequency)

3. **Configure Audio (Optional):**
   - **Hit Sound:** Assign an AudioClip (axe chop sound)
   - **Hit Volume:** `0.7`

4. **Configure Particles (Optional):**
   - Leave empty for now (we'll create particles in Step 4)

---

### Step 4: Create Particle System (Optional)

#### A. Create Particle System GameObject

1. **Select Tree GameObject** in Hierarchy
2. Right-click → **Effects** → **Particle System**
3. Rename child to **"LeafParticles"**
4. Position child GameObject:
   - **Transform Position Y:** `1.0` to `2.0` (top of tree)

#### B. Configure Main Module

In **LeafParticles** Inspector:

| Property | Value | Description |
|----------|-------|-------------|
| **Duration** | `0.5` | How long emission lasts |
| **Looping** | ❌ OFF | Only emit once per damage |
| **Start Lifetime** | `1.0` to `2.0` | How long each leaf exists |
| **Start Speed** | `1.0` to `3.0` | Initial velocity |
| **Start Size** | `0.1` to `0.3` | Leaf size |
| **Start Color** | Green/Brown | Leaf color |
| **Gravity Modifier** | `0.5` | Leaves fall gently |
| **Play On Awake** | ❌ OFF | Only play when damaged |
| **Max Particles** | `20` | Maximum leaf count |

#### C. Configure Emission Module

| Property | Value |
|----------|-------|
| **Rate over Time** | `0` (disable continuous emission) |
| **Bursts** | Click **+** to add burst |
| ├─ **Time** | `0` |
| ├─ **Count** | `8` (leaves per hit) |
| ├─ **Cycles** | `1` |
| └─ **Interval** | `0.01` |

#### D. Configure Shape Module

| Property | Value |
|----------|-------|
| **Shape** | `Cone` or `Hemisphere` |
| **Angle** | `25` |
| **Radius** | `0.5` |
| **Position Y** | `1.0` (emit from top) |

#### E. Configure Rotation over Lifetime (Optional)

- ✅ **Enable module**
- **Angular Velocity:** `-200` to `200` (leaves spin while falling)

#### F. Configure Size over Lifetime (Optional)

- ✅ **Enable module**
- **Size:** Curve from `1.0` to `0.5` (leaves shrink as they fall)

#### G. Configure Renderer Module

| Property | Value |
|----------|-------|
| **Render Mode** | `Billboard` |
| **Material** | `Default-Particle` |
| **Texture Sheet Animation** | Optional: Add leaf sprites |

#### H. Link to TreeController

1. Select **Tree** GameObject (parent)
2. In **TreeController** component:
3. Drag **LeafParticles** GameObject to **Damage Particles** field

---

### Step 5: Create Drop Table

If you haven't created a drop table yet:

1. **Create DropTable Asset:**
   - Right-click in `Assets/Resources/DropTables/`
   - **Create** → **Inventory** → **Drop Table**
   - Rename to `TreeDropTable`

2. **Configure DropTable:**
   - Click **+** to add drop entry
   - **Item:** Assign `Wood` item asset
   - **Pickup Prefab:** Assign `WoodItem` prefab
   - **Quantity:** `2` to `3` (wood pieces per tree)

3. **Assign to Tree:**
   - Select Tree GameObject
   - In **DropOnDeath** component
   - Assign `TreeDropTable` to **Drop Table** field

---

## Testing

### Method 1: Play Mode Testing

1. **Enter Play Mode** (press Play button)
2. **Move player** to tree
3. **Attack tree** with sword
4. **Expected results:**
   - ✅ Tree shakes on each hit
   - ✅ Leaves fall (if particles configured)
   - ✅ Sound plays (if audio configured)
   - ✅ After 3 hits, tree is destroyed
   - ✅ Wood items spawn near tree position

### Method 2: Inspector Testing (Without Play)

1. **Select Tree GameObject**
2. **Right-click on TreeController component**
3. **Select:**
   - `Test Shake Animation` - previews shake
   - `Test Hit Sound` - previews audio
4. **Select LeafParticles child**
5. **Click ▶️ button** in Particle Effect section to preview particles

### Common Issues Checklist

| Issue | Solution |
|-------|----------|
| Tree doesn't take damage | ✅ Check Tag is "Enemy"<br>✅ Check HealthComponent is attached<br>✅ Check Collider2D exists |
| No shake animation | ✅ Check TreeController is attached<br>✅ Check Shake Intensity > 0 |
| No particles | ✅ Check Damage Particles field is assigned<br>✅ Check ParticleSystem "Play On Awake" is OFF |
| No wood drops | ✅ Check DropOnDeath has Drop Table assigned<br>✅ Check TreeDropTable has entries |
| Tree blocks player | ✅ Check Collider2D "Is Trigger" is unchecked (correct behavior)<br>✅ Or configure Layer collision matrix |

---

## Configuration Reference

### Quick Setup Values (Copy-Paste Ready)

#### TreeController Settings
```
Shake Animation Settings:
├─ Shake Intensity: 0.1
├─ Shake Duration: 0.2
└─ Shake Speed: 30

Audio Settings:
├─ Hit Sound: [Assign AudioClip]
└─ Hit Volume: 0.7

Particle Effects:
└─ Damage Particles: [Assign LeafParticles]
```

#### Particle System Settings
```
Main Module:
├─ Duration: 0.5
├─ Looping: OFF
├─ Start Lifetime: 1.5
├─ Start Speed: 2.0
├─ Start Size: 0.15
├─ Start Color: RGB(100, 150, 50)
├─ Gravity Modifier: 0.5
└─ Play On Awake: OFF

Emission:
└─ Burst: Time=0, Count=8

Shape:
├─ Shape: Cone
├─ Angle: 25
└─ Radius: 0.5

Rotation over Lifetime:
└─ Angular Velocity: -200 to 200
```

#### HealthComponent Settings
```
Health Settings:
├─ Max Health: 9
└─ Health Bar: [Empty]
```

#### DropOnDeath Settings
```
Drop Settings:
├─ Drop Table: TreeDropTable
└─ Drop Radius: 0.5
```

---

## Extending the System

### Creating Other Destructible Objects

The same pattern works for rocks, bushes, crates, etc.:

1. **Replace TreeController** with generic **DestructibleController** (or keep TreeController)
2. **Change sprites** and particle effects
3. **Adjust DropTable** for different resources (stone, berries, gold)
4. **Modify health values** for different durability

### Adding Advanced Features

#### 1. Multi-Stage Destruction
```csharp
// In TreeController.cs, add:
[Header("Visual Stages")]
[SerializeField] private Sprite intactSprite;
[SerializeField] private Sprite damagedSprite;
[SerializeField] private Sprite crackedSprite;

private void HandleDamage(int currentHealth)
{
    // Update sprite based on health percentage
    float healthPercent = (float)currentHealth / healthComponent.GetMaxHealth();

    if (healthPercent > 0.66f)
        spriteRenderer.sprite = intactSprite;
    else if (healthPercent > 0.33f)
        spriteRenderer.sprite = damagedSprite;
    else
        spriteRenderer.sprite = crackedSprite;

    // Existing shake/audio/particle code...
}
```

#### 2. Stump Replacement
```csharp
// In DropOnDeath.cs, add:
[SerializeField] private GameObject stumpPrefab;

private void HandleDeath()
{
    if (dropTable != null)
        dropTable.SpawnDrops(transform.position, dropRadius);

    // Spawn stump at tree position
    if (stumpPrefab != null)
        Instantiate(stumpPrefab, transform.position, Quaternion.identity);
}
```

#### 3. Tree Regrowth System
```csharp
// New script: TreeRespawn.cs
public class TreeRespawn : MonoBehaviour
{
    [SerializeField] private GameObject treePrefab;
    [SerializeField] private float respawnTime = 300f; // 5 minutes

    void Start()
    {
        StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnTime);
        Instantiate(treePrefab, transform.position, Quaternion.identity);
        Destroy(gameObject); // Destroy stump
    }
}
```

#### 4. Critical Hits (Random Double Damage)
```csharp
// In TreeController.cs:
[Header("Critical Hit Settings")]
[SerializeField] private float criticalChance = 0.15f; // 15%
[SerializeField] private float criticalShakeMultiplier = 2f;
[SerializeField] private ParticleSystem criticalParticles;

private void HandleDamage(int currentHealth)
{
    bool isCritical = Random.value < criticalChance;

    if (isCritical)
    {
        // Enhanced shake for critical hits
        StartCoroutine(ShakeCoroutine(shakeIntensity * criticalShakeMultiplier));
        criticalParticles?.Play();
        Debug.Log("CRITICAL HIT!");
    }
    else
    {
        PlayShakeAnimation();
    }

    // Rest of damage handling...
}
```

---

## Performance Considerations

### Optimization Tips

1. **Object Pooling for Pickups:**
   - If you have many trees, consider pooling wood pickups instead of Instantiate/Destroy

2. **Particle System Optimization:**
   - Keep `Max Particles` low (10-20)
   - Use simple sprites (avoid complex textures)
   - Disable unused modules (Color over Lifetime, Size by Speed, etc.)

3. **Audio Optimization:**
   - Use compressed audio formats (Vorbis for long clips, ADPCM for short)
   - Set `Load Type` to "Compressed in Memory" for sound effects

4. **Collider Optimization:**
   - Use primitive colliders (Box/Capsule) instead of Polygon
   - Keep collider size minimal (just the trunk)

### Profiling

Monitor these in Unity Profiler:

| Metric | Expected Value | Warning Threshold |
|--------|---------------|-------------------|
| **TreeController.Update** | 0ms (doesn't use Update) | N/A |
| **Particle System** | <0.1ms per tree | >1ms |
| **Instantiate (drops)** | <1ms per tree death | >5ms |
| **Audio.PlayOneShot** | <0.1ms | >0.5ms |

---

## API Reference

### TreeController.cs

#### Public Fields (Inspector)

```csharp
[Header("Shake Animation Settings")]
float shakeIntensity = 0.1f;      // Shake magnitude
float shakeDuration = 0.2f;        // Shake duration in seconds
float shakeSpeed = 30f;            // Oscillation frequency

[Header("Audio Settings")]
AudioClip hitSound;                // Sound played on damage
float hitVolume = 0.7f;            // Audio volume [0-1]

[Header("Particle Effects")]
ParticleSystem damageParticles;    // Particle system to play on damage
```

#### Public Methods

```csharp
// None - all functionality is event-driven
```

#### Context Menu Methods (Inspector Only)

```csharp
[ContextMenu("Test Shake Animation")]
void TestShake();                  // Preview shake animation

[ContextMenu("Test Hit Sound")]
void TestSound();                  // Preview hit sound
```

#### Events Subscribed To

```csharp
HealthComponent.OnHealthChanged    // Triggers shake/audio/particles
```

---

## Related Systems

This system integrates with:

- **[Health System](Health/HealthComponent.md)** - Core damage system
- **[Drop System](../DropSystem.md)** - Resource dropping on death
- **[Inventory System](Inventory/)** - Wood pickup and storage
- **[Combat System](Player/SwordAttack.md)** - Player damage dealing

---

## Changelog

### Version 1.0 (Initial Release)
- ✅ Shake animation system
- ✅ Particle effect support
- ✅ Audio feedback
- ✅ Event-driven integration with HealthComponent
- ✅ Inspector testing tools
- ✅ Comprehensive documentation

### Future Enhancements
- 🔲 Multi-stage visual damage (intact → damaged → cracked)
- 🔲 Stump replacement system
- 🔲 Tree regrowth/respawn mechanics
- 🔲 Critical hit effects
- 🔲 Different tree types (oak, pine, palm)
- 🔲 Seasonal variants (spring/fall foliage)

---

## Credits

**System Architecture:** Unity Component-Based Design Pattern
**Event System:** UnityEvents for loose coupling
**Particle System:** Unity Shuriken Particle System
**Documentation:** Following project documentation standards

---

## Support

For issues or questions:
1. Check **Common Issues Checklist** above
2. Review **Testing** section
3. Verify all components are properly configured
4. Check Unity Console for error messages

**Common Questions:**

**Q: Can I use this for other objects besides trees?**
A: Yes! The system works for any destructible environment object. Just replace sprites/particles.

**Q: How do I make trees stronger/weaker?**
A: Adjust `HealthComponent.maxHealth` value (higher = more hits needed).

**Q: Can trees have different drop rates?**
A: Yes! Create different DropTable assets for different tree types.

**Q: How do I disable particles/audio?**
A: Leave the `Damage Particles` and `Hit Sound` fields empty in TreeController.

**Q: Can I use this system with enemies?**
A: Yes! The same pattern works. Enemies already use HealthComponent + DropOnDeath.

---

**Last Updated:** 2025-01-22
**Unity Version:** 2022.3+
**Script Location:** `/Assets/Scripts/Environment/TreeController.cs`
