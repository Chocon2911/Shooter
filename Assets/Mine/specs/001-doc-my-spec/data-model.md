# Data Model
**Feature**: 3D Loot-and-Fight Mobile/PC Game
**Date**: 2025-09-27

This document defines all data structures and their relationships for the game systems.

## Core Entities

### PlayerData
**Purpose**: Persistent player state

**Fields**:
- `float currentHealth` - Current HP (0 to maxHealth)
- `float maxHealth` - Maximum HP capacity
- `int gold` - Current currency amount
- `string currentMapId` - ID of current map location
- `Vector3 lastPosition` - Last known position in map
- `WeaponData equippedWeapon` - Currently equipped weapon (nullable)

**Validation Rules**:
- currentHealth >= 0 && currentHealth <= maxHealth
- gold >= 0
- maxHealth > 0

**State Transitions**:
- TakeDamage: currentHealth -= damage, if currentHealth <= 0 → Die state
- Die: Respawn at resting map, gold *= 0.9, currentHealth = maxHealth
- Heal: currentHealth = min(currentHealth + amount, maxHealth)

**Relationships**:
- Has one InventoryData
- Has one equipped WeaponData (optional)
- Located in one MapData

---

### InventoryData
**Purpose**: Player's carried items and weapons

**Fields**:
- `List<ItemData> items` - All carried items (unlimited capacity)
- `List<WeaponData> weapons` - All carried weapons (unlimited capacity)

**Validation Rules**:
- items != null
- weapons != null
- No duplicate references in lists

**Operations**:
- AddItem(ItemData): Append to items list
- RemoveItem(ItemData): Remove from items list
- HasItem(ItemData): Check if exists in items
- Similar for weapons

**Relationships**:
- Owned by one PlayerData
- Contains many ItemData
- Contains many WeaponData

---

### StorageData
**Purpose**: Global persistent storage accessible from resting maps

**Fields**:
- `List<ItemData> storedItems` - Items in storage (unlimited)
- `List<WeaponData> storedWeapons` - Weapons in storage (unlimited)

**Validation Rules**:
- storedItems != null
- storedWeapons != null

**Operations**:
- Deposit(ItemData/WeaponData): Move from inventory to storage
- Withdraw(ItemData/WeaponData): Move from storage to inventory

**Relationships**:
- Global singleton (one per save file)
- Independent of PlayerData location

---

### ItemData (ScriptableObject)
**Purpose**: Defines item properties (consumables, upgrade materials, quest items)

**Fields**:
- `string itemId` - Unique identifier
- `string itemName` - Display name
- `Sprite icon` - UI icon
- `ItemType type` - Enum: Consumable, Material, Quest, Misc
- `ItemRarity rarity` - Enum: Common, Uncommon, Rare, Epic, Legendary
- `int baseValue` - Sell price (buy price = baseValue * 2)
- `int upgradeLevel` - Current upgrade tier (0-2)
- `string description` - Flavor text
- `bool isStackable` - Can multiple exist as single inventory slot (future feature)

**Validation Rules**:
- itemId != null && itemId != ""
- baseValue >= 0
- upgradeLevel >= 0 && upgradeLevel <= 2

**Relationships**:
- Can be in InventoryData
- Can be in StorageData
- Referenced by LootTable
- Can be sold to ShopSystem

---

### WeaponData (ScriptableObject)
**Purpose**: Defines weapon combat properties

**Fields**:
- `string weaponId` - Unique identifier
- `string weaponName` - Display name
- `Sprite icon` - UI icon
- `GameObject prefab` - Visual 3D model
- `WeaponType type` - Enum: Melee, Ranged, Magic
- `float baseDamage` - Base damage amount
- `float attackSpeed` - Attacks per second
- `float attackRange` - Distance in Unity units
- `int upgradeLevel` - Current upgrade tier (0-2)
- `int baseValue` - Sell price (buy price = baseValue * 2)
- `string description` - Flavor text

**Computed Properties**:
- `float ActualDamage` = baseDamage * (1 + 0.25 * upgradeLevel)
- `int UpgradeCost` = baseValue * (2 ^ upgradeLevel)

**Validation Rules**:
- weaponId != null && weaponId != ""
- baseDamage > 0
- attackSpeed > 0
- attackRange > 0
- upgradeLevel >= 0 && upgradeLevel <= 2

**Relationships**:
- Can be equipped by PlayerData
- Can be in InventoryData
- Can be in StorageData
- Can be upgraded by UpgradeSystem

---

### MonsterData (ScriptableObject)
**Purpose**: Defines monster properties and behavior

**Fields**:
- `string monsterId` - Unique identifier
- `string monsterName` - Display name
- `GameObject prefab` - 3D model + animator
- `MonsterType type` - Enum: Ambient, Challenge
- `float maxHealth` - Health pool
- `float damage` - Damage per attack
- `float moveSpeed` - Movement speed
- `float attackRange` - Detection/attack range
- `int goldDrop` - Currency dropped on death
- `LootTable itemDrops` - Possible item drops

**Validation Rules**:
- monsterId != null && monsterId != ""
- maxHealth > 0
- damage >= 0
- moveSpeed > 0
- goldDrop >= 0

**Relationships**:
- Spawned in FightingMap
- Referenced by ChallengeData for wave composition
- Drops ItemData based on LootTable

---

### ChallengeData (ScriptableObject)
**Purpose**: Defines challenge configuration

**Fields**:
- `string challengeId` - Unique identifier
- `string challengeName` - Display name
- `float timeLimit` - Total time in seconds (default 300)
- `List<ChallengeStage> stages` - Sequential stages
- `int currencyReward` - Gold awarded on success
- `List<ItemData> possibleLootDrops` - Possible items on success
- `float lootDropChance` - Chance per item (0.0 to 1.0)

**Nested Type: ChallengeStage**:
- `MonsterData monsterType` - What monster to spawn
- `int monsterCount` - How many to spawn
- `float spawnDelay` - Seconds between spawns

**Validation Rules**:
- challengeId != null && challengeId != ""
- timeLimit > 0
- stages.Count > 0
- currencyReward >= 0
- lootDropChance >= 0 && lootDropChance <= 1

**Relationships**:
- Activated in FightingMap via ChallengeObject
- References MonsterData for wave composition
- References ItemData for loot rewards

---

### MapData (ScriptableObject)
**Purpose**: Defines map configuration and metadata

**Fields**:
- `string mapId` - Unique identifier
- `string mapName` - Display name
- `MapType type` - Enum: Resting, Fighting
- `string sceneAssetPath` - Unity scene path
- `List<string> connectedMapIds` - Maps accessible from this map
- `Vector3 spawnPosition` - Player spawn point
- `List<Transform> entryPoints` - Monster spawn locations (Fighting maps only)

**Validation Rules**:
- mapId != null && mapId != ""
- sceneAssetPath != null && scene exists
- type == Fighting → entryPoints.Count > 0

**Relationships**:
- PlayerData tracks current MapData
- Connected to other MapData via connectedMapIds
- FightingMap contains ChallengeObject

---

### SaveData (Serializable)
**Purpose**: Root save file structure

**Fields**:
- `PlayerData player` - Player state
- `InventoryData inventory` - Player inventory
- `StorageData storage` - Global storage
- `ProgressData progress` - Game progress tracking
- `DateTime lastSaveTime` - When saved
- `int saveVersion` - Data format version

**Nested Type: ProgressData**:
- `List<string> completedChallenges` - Challenge IDs (for achievements, not rewards)
- `int totalMonstersKilled` - Stat tracking
- `int totalGoldEarned` - Stat tracking
- `float totalPlayTime` - Seconds played

**Validation Rules**:
- player != null
- inventory != null
- storage != null
- saveVersion matches current game version

**Serialization**:
- JSON format via JsonUtility
- Stored in PlayerPrefs as string
- Key: "GameSaveData_Slot1"

---

## System Interfaces

### IInventorySystem
```csharp
interface IInventorySystem {
    bool AddItem(ItemData item);
    bool RemoveItem(ItemData item);
    bool HasItem(ItemData item);
    bool AddWeapon(WeaponData weapon);
    bool RemoveWeapon(WeaponData weapon);
    bool HasWeapon(WeaponData weapon);
    int GetItemCount(ItemData item);
}
```

### ICombatSystem
```csharp
interface ICombatSystem {
    void Attack(PlayerCombat attacker, Damageable target);
    float CalculateDamage(WeaponData weapon);
    void TakeDamage(Damageable target, float damage);
}
```

### ISaveSystem
```csharp
interface ISaveSystem {
    void Save();
    bool Load();
    bool HasSaveData();
    void DeleteSave();
    bool CanSave(); // False during active challenge
}
```

### IShopSystem
```csharp
interface IShopSystem {
    bool CanBuy(ItemData item, int gold);
    bool Buy(ItemData item);
    int GetBuyPrice(ItemData item); // baseValue * 2
    int GetSellPrice(ItemData item); // baseValue
    void Sell(ItemData item);
}
```

### IUpgradeSystem
```csharp
interface IUpgradeSystem {
    bool CanUpgrade(WeaponData weapon, int gold);
    int GetUpgradeCost(WeaponData weapon); // baseValue * (2 ^ level)
    bool Upgrade(WeaponData weapon);
}
```

### IChallengeSystem
```csharp
interface IChallengeSystem {
    void StartChallenge(ChallengeData challenge);
    void UpdateChallenge(float deltaTime);
    void CompleteStage();
    void EndChallenge(bool success);
    bool IsActive { get; }
    float TimeRemaining { get; }
    int CurrentStage { get; }
}
```

### IMapSystem
```csharp
interface IMapSystem {
    void LoadMap(string mapId);
    bool CanTransition(string targetMapId);
    MapData GetCurrentMap();
    List<MapData> GetConnectedMaps();
}
```

---

## Data Flow Diagrams

### Player Death Flow
```
PlayerHealth.currentHealth <= 0
  → PlayerHealth.Die()
  → GameManager.OnPlayerDeath()
  → PlayerData.gold *= 0.9
  → PlayerHealth.currentHealth = maxHealth
  → MapSystem.LoadMap(nearestRestingMapId)
  → GameOverScreen.Show(stats)
```

### Challenge Flow
```
Player interacts with ChallengeObject
  → ChallengeSystem.StartChallenge(challengeData)
  → MapSystem.LockTransitions()
  → For each stage:
      → SpawnMonsters(stage.monsterType, stage.monsterCount)
      → Wait for all monsters defeated
      → ChallengeSystem.CompleteStage()
  → If time expires: ChallengeSystem.EndChallenge(false)
  → If all stages cleared: ChallengeSystem.EndChallenge(true)
      → PlayerData.gold += challengeData.currencyReward
      → Roll loot table, add items to inventory
  → MapSystem.UnlockTransitions()
```

### Shop Transaction Flow
```
Player clicks Buy on ItemData
  → ShopSystem.CanBuy(item, player.gold)
  → If true:
      → player.gold -= GetBuyPrice(item)
      → InventorySystem.AddItem(item)
      → ShopUI.RefreshDisplay()
  → If false:
      → ShopUI.ShowError("Insufficient gold")
```

### Save/Load Flow
```
Player clicks Save
  → SaveSystem.CanSave() (false if challenge active)
  → If true:
      → Serialize PlayerData, InventoryData, StorageData, ProgressData
      → Convert to JSON string
      → PlayerPrefs.SetString("GameSaveData_Slot1", json)
      → PlayerPrefs.Save()
  → If false:
      → UI.ShowMessage("Cannot save during challenge")

Player clicks Load
  → SaveSystem.HasSaveData()
  → If true:
      → json = PlayerPrefs.GetString("GameSaveData_Slot1")
      → Deserialize to SaveData
      → Restore PlayerData, InventoryData, StorageData
      → MapSystem.LoadMap(PlayerData.currentMapId)
      → If challenge was active: Reset challenge state
```

---

## Validation & Constraints

### Invariants
- Player currency never negative
- Player health 0 <= current <= max
- Upgrade level 0-2 only
- Inventory/storage lists never null
- All ScriptableObject IDs unique

### Performance Constraints
- Inventory operations O(1) or O(n) where n < 1000
- Monster spawning uses object pooling (avoid instantiate during gameplay)
- Save/load < 1 second on mobile
- UI updates batched, not per-frame

### Data Integrity
- ScriptableObjects are read-only at runtime (only upgrade level modified on copies)
- Save data versioning for future compatibility
- Validate save data on load, discard if corrupted