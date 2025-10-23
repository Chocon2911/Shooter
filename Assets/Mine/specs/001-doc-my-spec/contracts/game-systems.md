# Game Systems Contracts
**Feature**: 3D Loot-and-Fight Mobile/PC Game
**Date**: 2025-09-27

This document defines the interfaces and contracts for all core game systems.

## Core System Interfaces

### IInventorySystem
**Purpose**: Manage player's carried items and weapons

**Contract**:
```csharp
public interface IInventorySystem
{
    // Items
    bool AddItem(ItemData item);          // Returns: true if added, false if null
    bool RemoveItem(ItemData item);       // Returns: true if removed, false if not found
    bool HasItem(ItemData item);          // Returns: true if item exists in inventory
    int GetItemCount(ItemData item);      // Returns: count of item (0 if not found)
    List<ItemData> GetAllItems();         // Returns: copy of items list

    // Weapons
    bool AddWeapon(WeaponData weapon);    // Returns: true if added, false if null
    bool RemoveWeapon(WeaponData weapon); // Returns: true if removed, false if not found
    bool HasWeapon(WeaponData weapon);    // Returns: true if weapon exists
    List<WeaponData> GetAllWeapons();     // Returns: copy of weapons list

    // Events
    event Action<ItemData> OnItemAdded;
    event Action<ItemData> OnItemRemoved;
    event Action<WeaponData> OnWeaponAdded;
    event Action<WeaponData> OnWeaponRemoved;
}
```

**Behavioral Guarantees**:
- AddItem/AddWeapon never throws, returns false for null
- Unlimited capacity (lists grow dynamically)
- Remove operations are idempotent (removing non-existent item returns false, no error)
- Get operations return defensive copies (callers can't modify internal state)
- Events fire after state change completes

**Test Cases**:
- Add null item → returns false
- Add valid item → returns true, HasItem true, GetItemCount = 1
- Remove existing item → returns true, HasItem false
- Remove non-existent item → returns false
- Add 1000 items → all added, no capacity error

---

### ICombatSystem
**Purpose**: Handle combat calculations and damage application

**Contract**:
```csharp
public interface ICombatSystem
{
    void Attack(IDamageable attacker, IDamageable target, WeaponData weapon);
    float CalculateDamage(WeaponData weapon);  // Returns: weapon.ActualDamage
    void TakeDamage(IDamageable target, float damage);

    event Action<IDamageable, IDamageable, float> OnAttack; // (attacker, target, damage)
    event Action<IDamageable> OnDeath;
}
```

**IDamageable Interface**:
```csharp
public interface IDamageable
{
    float CurrentHealth { get; set; }
    float MaxHealth { get; }
    bool IsDead { get; }
    void Die();
}
```

**Behavioral Guarantees**:
- CalculateDamage uses formula: baseDamage * (1 + 0.25 * upgradeLevel)
- TakeDamage reduces CurrentHealth, clamps to 0
- If CurrentHealth <= 0 → calls Die(), fires OnDeath event
- Attack combines CalculateDamage and TakeDamage
- All operations handle null checks (no throws)

**Test Cases**:
- Attack with +0 weapon (10 damage) → target takes 10 damage
- Attack with +1 weapon (10 base) → target takes 12.5 damage
- Attack with +2 weapon (10 base) → target takes 15 damage
- TakeDamage(100) on 50 health entity → health = 0, Die() called
- TakeDamage on dead entity → no-op

---

### ISaveSystem
**Purpose**: Persist and restore game state

**Contract**:
```csharp
public interface ISaveSystem
{
    void Save();                    // Serializes current game state to PlayerPrefs
    bool Load();                    // Returns: true if load successful, false if no save or corrupt
    bool HasSaveData();             // Returns: true if save file exists
    void DeleteSave();              // Removes save file
    bool CanSave();                 // Returns: false during active challenge, true otherwise

    event Action OnSaveCompleted;
    event Action OnLoadCompleted;
}
```

**SaveData Structure**:
```csharp
[Serializable]
public class SaveData
{
    public PlayerData player;
    public InventoryData inventory;
    public StorageData storage;
    public ProgressData progress;
    public string lastSaveTime;  // ISO 8601 format
    public int saveVersion;      // Current: 1
}
```

**Behavioral Guarantees**:
- Save() serializes to JSON, stores in PlayerPrefs key "GameSaveData_Slot1"
- Load() validates saveVersion, discards if mismatch
- CanSave() returns false if ChallengeSystem.IsActive == true
- Load() resets challenge state if one was active
- Save/Load operations are synchronous (< 1 second on mobile)

**Test Cases**:
- Save then Load → data matches
- Load without save → returns false
- Save during challenge → CanSave false, Save() no-op
- Save with version 1, load with version 2 → returns false, no load
- Corrupt JSON in PlayerPrefs → Load() returns false, doesn't crash

---

### IShopSystem
**Purpose**: Buy/sell item transactions

**Contract**:
```csharp
public interface IShopSystem
{
    bool CanBuy(ItemData item, int playerGold);
    bool CanBuy(WeaponData weapon, int playerGold);
    bool Buy(ItemData item, IInventorySystem inventory, ref int playerGold);
    bool Buy(WeaponData weapon, IInventorySystem inventory, ref int playerGold);
    int GetBuyPrice(ItemData item);     // Returns: item.baseValue * 2
    int GetBuyPrice(WeaponData weapon); // Returns: weapon.baseValue * 2
    int GetSellPrice(ItemData item);    // Returns: item.baseValue
    int GetSellPrice(WeaponData weapon);// Returns: weapon.baseValue
    void Sell(ItemData item, IInventorySystem inventory, ref int playerGold);
    void Sell(WeaponData weapon, IInventorySystem inventory, ref int playerGold);

    event Action<object, int> OnItemBought;  // (item, price)
    event Action<object, int> OnItemSold;    // (item, price)
}
```

**Behavioral Guarantees**:
- Buy price = baseValue * 2
- Sell price = baseValue (50% value)
- CanBuy checks: playerGold >= GetBuyPrice
- Buy decrements gold, adds to inventory atomically
- Sell increments gold, removes from inventory atomically
- Buy/Sell return false if insufficient gold / item not found
- All transactions fire events after completion

**Test Cases**:
- CanBuy with sufficient gold → true
- CanBuy with insufficient gold → false
- Buy item with 100 gold, cost 50 → gold = 50, item in inventory
- Buy with insufficient gold → returns false, no state change
- Sell item worth 25 → gold += 25, item removed
- Sell non-existent item → returns false, no state change

---

### IUpgradeSystem
**Purpose**: Upgrade items and weapons

**Contract**:
```csharp
public interface IUpgradeSystem
{
    bool CanUpgrade(WeaponData weapon, int playerGold);
    bool CanUpgrade(ItemData item, int playerGold);
    int GetUpgradeCost(WeaponData weapon);  // Returns: baseValue * (2 ^ currentLevel)
    int GetUpgradeCost(ItemData item);      // Returns: baseValue * (2 ^ currentLevel)
    bool Upgrade(WeaponData weapon, ref int playerGold);
    bool Upgrade(ItemData item, ref int playerGold);

    event Action<object> OnUpgradeSuccess;
    event Action<object> OnUpgradeFailed;
}
```

**Behavioral Guarantees**:
- Upgrade cost formula: baseValue * (2 ^ currentLevel)
- Max upgrade level: 2 (+0, +1, +2)
- CanUpgrade checks: level < 2 AND gold >= GetUpgradeCost
- Upgrade increments upgradeLevel, decrements gold atomically
- Upgrade on max level item → returns false
- Upgrade with insufficient gold → returns false

**Test Cases**:
- Weapon +0 with baseValue 100 → cost = 100, gold -= 100, level = 1
- Weapon +1 with baseValue 100 → cost = 200, gold -= 200, level = 2
- Weapon +2 → CanUpgrade false
- Upgrade with insufficient gold → returns false, no change
- Upgrade cost never negative

---

### IChallengeSystem
**Purpose**: Orchestrate timed wave-based challenges

**Contract**:
```csharp
public interface IChallengeSystem
{
    void StartChallenge(ChallengeData challengeData);
    void UpdateChallenge(float deltaTime);  // Called per frame by ChallengeManager
    void CompleteStage();                    // Called when all stage monsters dead
    void EndChallenge(bool success);         // Called on completion or failure
    void CancelChallenge();                  // Called when player dies

    bool IsActive { get; }
    float TimeRemaining { get; }
    int CurrentStage { get; }
    int TotalStages { get; }
    ChallengeData CurrentChallengeData { get; }

    event Action<ChallengeData> OnChallengeStarted;
    event Action<int> OnStageCompleted;      // stage index
    event Action<bool> OnChallengeEnded;     // success flag
}
```

**Behavioral Guarantees**:
- StartChallenge resets timer, stage index, locks map transitions
- UpdateChallenge decrements TimeRemaining by deltaTime
- TimeRemaining <= 0 → calls EndChallenge(false) automatically
- CompleteStage advances CurrentStage, spawns next wave
- CurrentStage == TotalStages → calls EndChallenge(true)
- EndChallenge unlocks map transitions, awards rewards if success
- CancelChallenge (on player death) → EndChallenge(false), no rewards

**Test Cases**:
- Start challenge with 5 stages → CurrentStage = 0, TotalStages = 5, IsActive = true
- Update challenge, TimeRemaining = 0 → EndChallenge(false) called
- Complete all stages before timer → EndChallenge(true), rewards awarded
- Player dies during challenge → CancelChallenge, EndChallenge(false), no rewards
- Multiple CompleteStage calls → each increments CurrentStage
- EndChallenge → IsActive = false, TimeRemaining = 0

---

### IMapSystem
**Purpose**: Handle map loading and transitions

**Contract**:
```csharp
public interface IMapSystem
{
    void LoadMap(string mapId);
    bool CanTransition(string targetMapId);
    MapData GetCurrentMap();
    List<MapData> GetConnectedMaps();
    void LockTransitions();    // Prevents transitions (during challenges)
    void UnlockTransitions();  // Re-enables transitions

    event Action<MapData> OnMapLoading;
    event Action<MapData> OnMapLoaded;
}
```

**Behavioral Guarantees**:
- LoadMap loads Unity scene asynchronously
- CanTransition checks: !IsLocked AND targetMapId in currentMap.connectedMapIds
- LoadMap updates PlayerData.currentMapId
- LockTransitions sets internal flag, CanTransition returns false
- UnlockTransitions clears flag
- GetConnectedMaps returns MapData objects from connectedMapIds

**Test Cases**:
- LoadMap valid ID → scene loads, OnMapLoaded fires
- LoadMap invalid ID → logs error, no crash
- CanTransition to connected map → returns true
- CanTransition to non-connected map → returns false
- LockTransitions → CanTransition returns false for all maps
- UnlockTransitions → CanTransition works normally

---

## Integration Points

### Combat + Inventory Integration
```
Player kills monster with loot drop
  → CombatSystem.OnDeath event fires
  → MonsterAI.Die() checks MonsterData.itemDrops
  → Rolls loot table
  → Calls InventorySystem.AddItem(dropped item)
  → InventorySystem.OnItemAdded event fires
  → UI updates
```

### Challenge + Map Integration
```
ChallengeSystem.StartChallenge
  → Calls MapSystem.LockTransitions()

ChallengeSystem.EndChallenge
  → Calls MapSystem.UnlockTransitions()
```

### Save + All Systems Integration
```
SaveSystem.Save()
  → Queries PlayerData from GameManager
  → Queries InventoryData from InventorySystem
  → Queries StorageData from StorageSystem
  → Queries ProgressData from ChallengeSystem (completed challenges)
  → Serializes to JSON

SaveSystem.Load()
  → Deserializes JSON to SaveData
  → Restores PlayerData to GameManager
  → Restores InventoryData to InventorySystem
  → Restores StorageData to StorageSystem
  → If challenge was active: Calls ChallengeSystem.CancelChallenge()
```

---

## Error Handling

### Null Safety
- All public methods handle null inputs gracefully (return false/default, never throw)
- All collections initialized in constructor (never null)

### Invalid State
- Upgrade at max level → returns false
- Buy with insufficient gold → returns false
- Save during challenge → CanSave() returns false, Save() no-op
- Load corrupt data → returns false, logs error

### Concurrency
- All systems are single-threaded (Unity main thread)
- No async operations during gameplay (only scene loading)

---

## Performance Contracts

- InventorySystem operations: O(1) add, O(n) remove (n < 1000)
- CombatSystem.CalculateDamage: O(1), no allocations
- SaveSystem.Save/Load: < 1 second on mobile
- ChallengeSystem.UpdateChallenge: < 0.1ms per frame
- MapSystem.LoadMap: Async, non-blocking