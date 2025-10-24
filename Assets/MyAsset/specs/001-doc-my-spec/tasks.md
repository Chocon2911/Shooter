# Tasks: 3D Loot-and-Fight Mobile/PC Game

**Input**: Design documents from `/mnt/c/Local_D/VMioSys/Game/Shooter/Assets/MyAsset/specs/001-doc-my-spec/`
**Prerequisites**: plan.md, research.md, data-model.md, contracts/, quickstart.md

## Execution Flow (main)
```
1. Load plan.md from feature directory
   → Tech stack: Unity C# 2021.3 LTS, ScriptableObjects, Unity Input System
   → Structure: Script/, Scene/, Asset/, Tests/
2. Load optional design documents:
   → data-model.md: 9 entities (PlayerData, ItemData, WeaponData, MonsterData, etc.)
   → contracts/game-systems.md: 7 system interfaces
   → quickstart.md: 11 integration test scenarios
3. Generate tasks by category:
   → Setup: Unity project structure, packages, test framework
   → Tests: 7 contract test files, 11 integration scenarios
   → Core: 9 ScriptableObject types, 7 system implementations
   → Integration: UI systems, scene setup, input management
   → Polish: Performance optimization, platform builds
4. Apply task rules:
   → ScriptableObject definitions = [P] (different files)
   → System implementations = [P] where independent
   → UI systems = sequential (shared scene dependencies)
   → Tests before implementation (TDD)
5. Number tasks sequentially (T001, T002...)
6. Total estimated tasks: 58
7. Validate task completeness: All contracts tested, all entities defined
8. Return: SUCCESS (tasks ready for execution)
```

## Format: `[ID] [P?] Description`
- **[P]**: Can run in parallel (different files, no dependencies)
- Include exact file paths in descriptions

## Path Conventions
Unity project structure:
- **Scripts**: `Script/[Domain]/ClassName.cs`
- **Tests**: `Tests/EditMode/` or `Tests/PlayMode/`
- **Data**: `Asset/[Type]/FileName.asset` (ScriptableObject instances)
- **Scenes**: `Scene/SceneName.unity`

---

## Phase 3.1: Setup & Infrastructure

### T001: Create project directory structure
**Description**: Create all required directories per plan.md structure
**Files**:
- `Script/Core/`, `Script/Player/`, `Script/Combat/`, `Script/Monsters/`
- `Script/Challenge/`, `Script/Economy/`, `Script/Map/`, `Script/UI/`, `Script/Data/`
- `Tests/EditMode/`, `Tests/PlayMode/`
- `Asset/Items/`, `Asset/Weapons/`, `Asset/Monsters/`, `Asset/Challenges/`

**Validation**: All directories exist

---

### T002: Install Unity packages
**Description**: Add required packages via Unity Package Manager
**Packages**:
- `com.unity.inputsystem` (Unity Input System)
- `com.unity.textmeshpro` (TextMeshPro)
- `com.unity.test-framework` (Unity Test Framework)

**Validation**: Packages appear in Packages/manifest.json

---

### T003 [P]: Configure Unity Test Framework
**Description**: Set up test assembly definitions
**Files**:
- `Tests/EditMode/EditMode.asmdef` (references main assembly)
- `Tests/PlayMode/PlayMode.asmdef` (references main assembly, testables)

**Validation**: Test Runner window detects assemblies

---

## Phase 3.2: ScriptableObject Data Definitions (TDD Foundation)

### T004 [P]: Create ItemData ScriptableObject
**Description**: Define ItemData ScriptableObject structure per data-model.md
**File**: `Script/Data/ItemData.cs`
**Fields**: itemId, itemName, icon, type (enum), rarity (enum), baseValue, upgradeLevel, description, isStackable
**Validation**: CreateAssetMenu attribute, compiles without errors

---

### T005 [P]: Create WeaponData ScriptableObject
**Description**: Define WeaponData ScriptableObject structure per data-model.md
**File**: `Script/Data/WeaponData.cs`
**Fields**: weaponId, weaponName, icon, prefab, type (enum), baseDamage, attackSpeed, attackRange, upgradeLevel, baseValue, description
**Computed Properties**: ActualDamage, UpgradeCost
**Validation**: Compiles, ActualDamage formula correct (baseDamage * (1 + 0.25 * upgradeLevel))

---

### T006 [P]: Create MonsterData ScriptableObject
**Description**: Define MonsterData ScriptableObject structure per data-model.md
**File**: `Script/Data/MonsterData.cs`
**Fields**: monsterId, monsterName, prefab, type (enum: Ambient/Challenge), maxHealth, damage, moveSpeed, attackRange, goldDrop, itemDrops (LootTable)
**Nested Type**: LootTable struct (ItemData item, float dropChance)
**Validation**: Compiles, CreateAssetMenu

---

### T007 [P]: Create ChallengeData ScriptableObject
**Description**: Define ChallengeData ScriptableObject structure per data-model.md
**File**: `Script/Data/ChallengeData.cs`
**Fields**: challengeId, challengeName, timeLimit, stages (List<ChallengeStage>), currencyReward, possibleLootDrops, lootDropChance
**Nested Type**: ChallengeStage struct (MonsterData, monsterCount, spawnDelay)
**Validation**: Compiles, serializable in Inspector

---

### T008 [P]: Create MapData ScriptableObject
**Description**: Define MapData ScriptableObject structure per data-model.md
**File**: `Script/Data/MapData.cs`
**Fields**: mapId, mapName, type (enum: Resting/Fighting), sceneAssetPath, connectedMapIds, spawnPosition, entryPoints (List<Transform>)
**Validation**: Compiles, CreateAssetMenu

---

### T009 [P]: Create SaveData serializable classes
**Description**: Define SaveData, PlayerData, InventoryData, StorageData, ProgressData per data-model.md
**File**: `Script/Core/SaveData.cs`
**Classes**: SaveData, PlayerData, InventoryData, StorageData, ProgressData (all [Serializable])
**Fields**: Per data-model.md specifications
**Validation**: JsonUtility.ToJson(new SaveData()) doesn't throw

---

## Phase 3.3: System Interfaces (Contracts)

### T010 [P]: Create IInventorySystem interface
**Description**: Define IInventorySystem interface per contracts/game-systems.md
**File**: `Script/Core/IInventorySystem.cs`
**Methods**: AddItem, RemoveItem, HasItem, GetItemCount, GetAllItems, AddWeapon, RemoveWeapon, HasWeapon, GetAllWeapons
**Events**: OnItemAdded, OnItemRemoved, OnWeaponAdded, OnWeaponRemoved
**Validation**: Compiles, matches contract

---

### T011 [P]: Create ICombatSystem interface
**Description**: Define ICombatSystem and IDamageable interfaces per contracts/game-systems.md
**File**: `Script/Combat/ICombatSystem.cs`
**Interfaces**: ICombatSystem, IDamageable
**Methods**: Attack, CalculateDamage, TakeDamage
**Events**: OnAttack, OnDeath
**Validation**: Compiles, IDamageable properties defined

---

### T012 [P]: Create ISaveSystem interface
**Description**: Define ISaveSystem interface per contracts/game-systems.md
**File**: `Script/Core/ISaveSystem.cs`
**Methods**: Save, Load, HasSaveData, DeleteSave, CanSave
**Events**: OnSaveCompleted, OnLoadCompleted
**Validation**: Compiles, matches contract

---

### T013 [P]: Create IShopSystem interface
**Description**: Define IShopSystem interface per contracts/game-systems.md
**File**: `Script/Economy/IShopSystem.cs`
**Methods**: CanBuy, Buy (item/weapon), GetBuyPrice, GetSellPrice, Sell
**Events**: OnItemBought, OnItemSold
**Validation**: Compiles, overloads for ItemData and WeaponData

---

### T014 [P]: Create IUpgradeSystem interface
**Description**: Define IUpgradeSystem interface per contracts/game-systems.md
**File**: `Script/Economy/IUpgradeSystem.cs`
**Methods**: CanUpgrade, GetUpgradeCost, Upgrade (item/weapon)
**Events**: OnUpgradeSuccess, OnUpgradeFailed
**Validation**: Compiles, overloads for ItemData and WeaponData

---

### T015 [P]: Create IChallengeSystem interface
**Description**: Define IChallengeSystem interface per contracts/game-systems.md
**File**: `Script/Challenge/IChallengeSystem.cs`
**Methods**: StartChallenge, UpdateChallenge, CompleteStage, EndChallenge, CancelChallenge
**Properties**: IsActive, TimeRemaining, CurrentStage, TotalStages, CurrentChallengeData
**Events**: OnChallengeStarted, OnStageCompleted, OnChallengeEnded
**Validation**: Compiles, all properties readonly

---

### T016 [P]: Create IMapSystem interface
**Description**: Define IMapSystem interface per contracts/game-systems.md
**File**: `Script/Map/IMapSystem.cs`
**Methods**: LoadMap, CanTransition, GetCurrentMap, GetConnectedMaps, LockTransitions, UnlockTransitions
**Events**: OnMapLoading, OnMapLoaded
**Validation**: Compiles, matches contract

---

## Phase 3.4: Contract Tests (MUST FAIL BEFORE IMPLEMENTATION)

### T017 [P]: Write InventorySystem contract tests
**Description**: Create EditMode tests for IInventorySystem per contracts/game-systems.md test cases
**File**: `Tests/EditMode/InventorySystemTests.cs`
**Test Cases**:
- AddItem with null → returns false
- AddItem valid → returns true, HasItem true
- RemoveItem existing → returns true, HasItem false
- Add 1000 items → no capacity error
**Validation**: All tests fail (no implementation yet), compile

---

### T018 [P]: Write CombatSystem contract tests
**Description**: Create EditMode tests for ICombatSystem per contracts/game-systems.md test cases
**File**: `Tests/EditMode/CombatSystemTests.cs`
**Test Cases**:
- CalculateDamage with +0 weapon → correct damage
- CalculateDamage with +1 weapon → 25% increase
- TakeDamage reducing health to 0 → Die() called
- Attack combines damage calculation and application
**Validation**: All tests fail, compile

---

### T019 [P]: Write SaveSystem contract tests
**Description**: Create EditMode tests for ISaveSystem per contracts/game-systems.md test cases
**File**: `Tests/EditMode/SaveSystemTests.cs`
**Test Cases**:
- Save then Load → data matches
- Load without save → returns false
- CanSave during challenge → returns false
- Corrupt JSON → Load returns false, no crash
**Validation**: All tests fail, compile

---

### T020 [P]: Write ShopSystem contract tests
**Description**: Create EditMode tests for IShopSystem per contracts/game-systems.md test cases
**File**: `Tests/EditMode/ShopSystemTests.cs`
**Test Cases**:
- Buy with sufficient gold → gold deducted, item added
- Buy with insufficient gold → returns false
- Sell item → gold increased by baseValue
- Buy price = baseValue * 2
**Validation**: All tests fail, compile

---

### T021 [P]: Write UpgradeSystem contract tests
**Description**: Create EditMode tests for IUpgradeSystem per contracts/game-systems.md test cases
**File**: `Tests/EditMode/UpgradeSystemTests.cs`
**Test Cases**:
- Upgrade +0 weapon → cost = baseValue * 1
- Upgrade +1 weapon → cost = baseValue * 2
- Upgrade +2 weapon → CanUpgrade false
- Upgrade with insufficient gold → returns false
**Validation**: All tests fail, compile

---

### T022 [P]: Write ChallengeSystem contract tests
**Description**: Create EditMode tests for IChallengeSystem per contracts/game-systems.md test cases
**File**: `Tests/EditMode/ChallengeSystemTests.cs`
**Test Cases**:
- StartChallenge → IsActive true, TimeRemaining set
- UpdateChallenge with time expiring → EndChallenge(false) called
- CompleteStage through all stages → EndChallenge(true)
- CancelChallenge → EndChallenge(false)
**Validation**: All tests fail, compile

---

### T023 [P]: Write MapSystem contract tests
**Description**: Create EditMode tests for IMapSystem per contracts/game-systems.md test cases
**File**: `Tests/EditMode/MapSystemTests.cs`
**Test Cases**:
- LoadMap valid ID → map loads
- CanTransition to connected map → returns true
- CanTransition when locked → returns false
- LockTransitions → CanTransition false for all
**Validation**: All tests fail, compile

---

## Phase 3.5: Core System Implementations

### T024 [P]: Implement InventorySystem
**Description**: Create InventorySystem MonoBehaviour implementing IInventorySystem
**File**: `Script/Player/InventorySystem.cs`
**Requirements**:
- Implement all IInventorySystem methods per contracts
- Use List<ItemData> and List<WeaponData> internally
- Fire events after state changes
- Null checks return false (no throws)
**Validation**: T017 tests pass

---

### T025 [P]: Implement CombatSystem
**Description**: Create CombatSystem singleton implementing ICombatSystem
**File**: `Script/Combat/CombatSystem.cs`
**Requirements**:
- CalculateDamage uses formula: baseDamage * (1 + 0.25 * upgradeLevel)
- TakeDamage clamps health to 0, calls Die() if health <= 0
- Attack combines CalculateDamage + TakeDamage
- Fire OnAttack and OnDeath events
**Validation**: T018 tests pass

---

### T026: Implement SaveSystem
**Description**: Create SaveSystem singleton implementing ISaveSystem
**File**: `Script/Core/SaveSystem.cs`
**Requirements**:
- Save serializes SaveData to JSON, stores in PlayerPrefs key "GameSaveData_Slot1"
- Load deserializes, validates saveVersion
- CanSave returns false if ChallengeManager.IsActive
- HasSaveData checks PlayerPrefs.HasKey
**Validation**: T019 tests pass

---

### T027 [P]: Implement ShopSystem
**Description**: Create ShopSystem MonoBehaviour implementing IShopSystem
**File**: `Script/Economy/ShopSystem.cs`
**Requirements**:
- GetBuyPrice returns baseValue * 2
- GetSellPrice returns baseValue
- Buy/Sell modify gold ref parameter atomically
- Fire events after transactions
**Validation**: T020 tests pass

---

### T028 [P]: Implement UpgradeSystem
**Description**: Create UpgradeSystem MonoBehaviour implementing IUpgradeSystem
**File**: `Script/Economy/UpgradeSystem.cs`
**Requirements**:
- GetUpgradeCost formula: baseValue * (2 ^ currentLevel)
- Upgrade increments upgradeLevel (max 2), deducts gold
- CanUpgrade checks level < 2 AND sufficient gold
- Fire OnUpgradeSuccess/OnUpgradeFailed
**Validation**: T021 tests pass

---

### T029: Implement ChallengeManager
**Description**: Create ChallengeManager MonoBehaviour implementing IChallengeSystem
**File**: `Script/Challenge/ChallengeManager.cs`
**Requirements**:
- StartChallenge initializes timer, locks map via IMapSystem
- UpdateChallenge decrements TimeRemaining, auto-fails on expire
- CompleteStage advances stage, spawns next wave
- EndChallenge unlocks map, awards rewards if success
**Validation**: T022 tests pass

---

### T030: Implement MapManager
**Description**: Create MapManager singleton implementing IMapSystem
**File**: `Script/Map/MapManager.cs`
**Requirements**:
- LoadMap uses SceneManager.LoadSceneAsync
- CanTransition checks locked flag and connectedMapIds
- LockTransitions/UnlockTransitions toggle internal flag
- GetConnectedMaps loads MapData from Resources
**Validation**: T023 tests pass

---

### T031: Implement PlayerHealth with IDamageable
**Description**: Create PlayerHealth MonoBehaviour implementing IDamageable
**File**: `Script/Player/PlayerHealth.cs`
**Requirements**:
- CurrentHealth, MaxHealth, IsDead properties
- Die() triggers respawn: gold *= 0.9, LoadMap(resting), health = max
- Integrates with CombatSystem for damage events
**Validation**: Player takes damage, dies, respawns with 10% gold loss

---

### T032: Implement PlayerController with input
**Description**: Create PlayerController MonoBehaviour for movement
**File**: `Script/Player/PlayerController.cs`
**Requirements**:
- Uses Unity Input System for cross-platform input
- WASD/arrow keys on Windows, virtual joystick on Android
- CharacterController-based movement
**Validation**: Player moves in PlayMode

---

### T033: Implement PlayerCombat
**Description**: Create PlayerCombat MonoBehaviour for attack actions
**File**: `Script/Player/PlayerCombat.cs`
**Requirements**:
- Reads equipped weapon from PlayerData
- Mouse click / touch button triggers Attack via ICombatSystem
- Raycast to detect enemy targets
**Validation**: Player attacks monster, damage applied

---

## Phase 3.6: Monster Systems

### T034 [P]: Implement MonsterAI base class
**Description**: Create MonsterAI MonoBehaviour for AI behavior
**File**: `Script/Monsters/MonsterAI.cs`
**Requirements**:
- IDamageable implementation (health, Die)
- State machine: Idle, Chase, Attack
- NavMeshAgent for pathfinding
- Die() drops gold + rolls loot table
**Validation**: Monster chases player, attacks, dies, drops loot

---

### T035 [P]: Implement AmbientMonster subclass
**Description**: Create AmbientMonster extending MonsterAI
**File**: `Script/Monsters/AmbientMonster.cs`
**Requirements**:
- Roams freely in fighting maps
- Uses MonsterData with type = Ambient
**Validation**: Ambient monsters spawn, roam, engage player

---

### T036 [P]: Implement ChallengeMonster subclass
**Description**: Create ChallengeMonster extending MonsterAI
**File**: `Script/Monsters/ChallengeMonster.cs`
**Requirements**:
- Spawned during challenges only
- Uses MonsterData with type = Challenge
- Reports death to ChallengeManager for stage completion
**Validation**: Challenge monsters spawn from entry points during challenge

---

### T037: Implement MonsterSpawner
**Description**: Create MonsterSpawner utility for object pooling
**File**: `Script/Monsters/MonsterSpawner.cs`
**Requirements**:
- Object pool pattern (pre-instantiate monsters, reuse)
- SpawnMonster(MonsterData, position) method
- Used by ChallengeManager and map setup
**Validation**: Spawning 100 monsters has no GC allocations

---

## Phase 3.7: Challenge System Components

### T038: Implement ChallengeObject interactable
**Description**: Create ChallengeObject MonoBehaviour for challenge trigger
**File**: `Script/Challenge/ChallengeObject.cs`
**Requirements**:
- Trigger collider for player proximity
- UI prompt "Press E to start challenge"
- Calls ChallengeManager.StartChallenge on interact
**Validation**: Player interacts, challenge starts

---

### T039 [P]: Implement StageController
**Description**: Create StageController for wave progression
**File**: `Script/Challenge/StageController.cs`
**Requirements**:
- Spawns monsters per ChallengeStage data
- Tracks alive monsters, calls CompleteStage when all dead
- Integrates with MonsterSpawner
**Validation**: Stages progress correctly during challenge

---

### T040 [P]: Implement EntryPoint marker
**Description**: Create EntryPoint MonoBehaviour for spawn locations
**File**: `Script/Challenge/EntryPoint.cs`
**Requirements**:
- Simple MonoBehaviour marking Transform position
- Gizmo visualization in Scene view
- ChallengeManager queries EntryPoints in map
**Validation**: Entry points visible in editor, monsters spawn at them

---

## Phase 3.8: Economy UI Systems

### T041: Implement ShopUI
**Description**: Create ShopUI MonoBehaviour for buy/sell interface
**File**: `Script/UI/ShopUI.cs`
**Requirements**:
- Display available items with buy prices
- Buy button calls ShopSystem.Buy
- Sell tab shows inventory, sell button calls ShopSystem.Sell
- Error messages for insufficient gold
**Validation**: Quickstart Scenario 5 passes (shop transactions)

---

### T042: Implement InventoryUI
**Description**: Create InventoryUI MonoBehaviour for inventory display
**File**: `Script/UI/InventoryUI.cs`
**Requirements**:
- Grid layout showing all items/weapons
- Icons from ItemData/WeaponData
- Select item to view details
- Integrates with other UIs (shop, storage, upgrade)
**Validation**: Inventory updates when items added/removed

---

### T043: Implement StorageUI
**Description**: Create StorageUI MonoBehaviour for storage interface
**File**: `Script/UI/StorageUI.cs`
**Requirements**:
- Split view: inventory left, storage right
- Deposit button moves item to storage
- Withdraw button moves item to inventory
- Global storage (same across resting maps)
**Validation**: Quickstart Scenario 7 passes (storage)

---

### T044: Implement UpgradeUI
**Description**: Create UpgradeUI MonoBehaviour for upgrade interface
**File**: `Script/UI/UpgradeUI.cs`
**Requirements**:
- Select item/weapon, show upgrade cost
- Upgrade button calls UpgradeSystem.Upgrade
- Display stat changes (+25% per tier)
- Disable button at max level (2)
**Validation**: Quickstart Scenario 6 passes (upgrades)

---

### T045: Implement TradingUI
**Description**: Create TradingUI MonoBehaviour for NPC trading
**File**: `Script/UI/TradingUI.cs`
**Requirements**:
- Display trade offers (required items → reward)
- Accept button validates and executes trade
- Error for missing items
**Validation**: Quickstart Scenario 9 passes (trading)

---

## Phase 3.9: Menu & Lifecycle UI

### T046: Implement MainMenu scene
**Description**: Create MainMenu UI for game start
**File**: `Scene/MainMenu.unity`, `Script/UI/MainMenu.cs`
**Requirements**:
- New Game button → loads first resting map
- Load Game button → calls SaveSystem.Load
- Quit button exits application
**Validation**: Quickstart Scenario 1 passes (new game flow)

---

### T047: Implement GameOverScreen
**Description**: Create GameOverScreen UI
**File**: `Script/UI/GameOverScreen.cs`
**Requirements**:
- Displays: time survived, monsters killed, currency lost
- Respawn button → loads resting map
- Main Menu button → loads MainMenu scene
**Validation**: Death triggers GameOver, shows correct stats

---

### T048: Implement PauseMenu with save
**Description**: Create PauseMenu UI with save/load buttons
**File**: `Script/UI/PauseMenu.cs`
**Requirements**:
- ESC key toggles pause (Time.timeScale = 0)
- Save button calls SaveSystem.Save (disabled if !CanSave)
- Resume, Settings, Quit buttons
**Validation**: Can save in resting map, blocked in challenge

---

## Phase 3.10: Input & Platform Adaptation

### T049: Implement InputManager for cross-platform input
**Description**: Create InputManager singleton for input abstraction
**File**: `Script/UI/InputManager.cs`
**Requirements**:
- Unity Input System actions: Move, Attack, Interact, Menu
- Platform detection (Android vs Windows)
- Virtual joystick + buttons for Android
- Keyboard/mouse for Windows
**Validation**: Quickstart Scenario 10 passes (platform input)

---

### T050 [P]: Create virtual controls for Android
**Description**: Create on-screen controls UI for mobile
**File**: `Script/UI/VirtualControls.cs`
**Requirements**:
- Joystick for movement (bottom-left)
- Attack button (bottom-right)
- Interact button (center, appears near interactables)
- Hidden on Windows builds
**Validation**: Touch input works on Android device

---

## Phase 3.11: Scene Setup

### T051: Create RestingMap_01 scene
**Description**: Build first resting map scene
**File**: `Scene/RestingMap_01.unity`
**Requirements**:
- Shop NPC with ShopUI
- Storage chest with StorageUI
- Upgrade station with UpgradeUI
- Trader NPC with TradingUI
- Map transition to FightingMap_01
- Player spawn point
**Validation**: All resting map features accessible

---

### T052: Create FightingMap_01 scene
**Description**: Build first fighting map scene
**File**: `Scene/FightingMap_01.unity`
**Requirements**:
- Ambient monster spawners (3-5 monsters)
- Loot item spawners
- ChallengeObject with linked ChallengeData
- Entry points for challenge spawns (4-6 points)
- Map transition back to RestingMap_01
- NavMesh baked
**Validation**: Ambient monsters roam, challenge activates

---

### T053: Create GameManager persistent object
**Description**: Create GameManager singleton (DontDestroyOnLoad)
**File**: `Script/Core/GameManager.cs`
**Requirements**:
- Holds PlayerData reference
- Initializes systems (SaveSystem, InputManager)
- Handles application lifecycle (pause, quit)
- Persists across scenes
**Validation**: GameManager survives scene transitions

---

## Phase 3.12: Integration Tests (PlayMode)

### T054 [P]: PlayMode test - Combat & Loot flow
**Description**: Automated integration test for Quickstart Scenario 2
**File**: `Tests/PlayMode/CombatLootTests.cs`
**Requirements**:
- Spawn player + monster in test scene
- Simulate attack input
- Assert monster dies, loot drops
- Assert loot collection adds to inventory
**Validation**: Test passes, matches manual scenario

---

### T055 [P]: PlayMode test - Challenge flow
**Description**: Automated integration test for Quickstart Scenario 3
**File**: `Tests/PlayMode/ChallengeFlowTests.cs`
**Requirements**:
- Load fighting map, activate challenge
- Assert map locked, waves spawn
- Simulate killing all monsters OR timer expire
- Assert EndChallenge called with correct success flag
**Validation**: Test passes, both success/failure paths work

---

### T056 [P]: PlayMode test - Death & Respawn
**Description**: Automated integration test for Quickstart Scenario 4
**File**: `Tests/PlayMode/DeathRespawnTests.cs`
**Requirements**:
- Set player gold = 100
- Reduce health to 0
- Assert gold = 90, map loads to resting, health = max
**Validation**: Test passes, 10% penalty applied

---

### T057 [P]: PlayMode test - Save/Load round-trip
**Description**: Automated integration test for Quickstart Scenario 8
**File**: `Tests/PlayMode/SaveLoadTests.cs`
**Requirements**:
- Set specific game state (gold, inventory, position)
- Call SaveSystem.Save
- Reset state
- Call SaveSystem.Load
- Assert all state matches
**Validation**: Test passes, perfect round-trip

---

## Phase 3.13: Polish & Optimization

### T058 [P]: Create test ItemData/WeaponData/MonsterData assets
**Description**: Create example ScriptableObject assets for testing
**Files**:
- `Asset/Items/HealthPotion.asset`, `Asset/Items/IronOre.asset`, `Asset/Items/Gold.asset`
- `Asset/Weapons/Sword.asset`, `Asset/Weapons/Bow.asset`
- `Asset/Monsters/Goblin.asset` (Ambient), `Asset/Monsters/Orc.asset` (Challenge)
- `Asset/Challenges/ForestChallenge.asset`
**Validation**: Assets loadable in editor, no errors

---

### T059: Optimize monster spawning with object pooling
**Description**: Ensure MonsterSpawner uses object pooling correctly
**File**: `Script/Monsters/MonsterSpawner.cs` (refactor)
**Requirements**:
- Pre-instantiate 20 monsters per type
- Reuse inactive monsters instead of Instantiate
- Profile: spawning 100 monsters should show 0 GC alloc in Profiler
**Validation**: Profiler shows no allocations during spawning

---

### T060 [P]: Add occlusion culling to fighting maps
**Description**: Bake occlusion culling for performance
**Files**: `Scene/FightingMap_01.unity`
**Requirements**:
- Bake occlusion data via Window > Rendering > Occlusion Culling
- Mark static objects
- Test: FPS increases when looking at walls (occluding monsters)
**Validation**: Performance profiler shows reduced draw calls

---

### T061: Configure Quality Settings for Android
**Description**: Create platform-specific quality presets
**Files**: Edit > Project Settings > Quality
**Requirements**:
- "High" profile: PC, shadow quality high, texture res full
- "Medium" profile: Android, shadow quality medium, texture res half
- Set default per platform in Build Settings
**Validation**: Android build runs 60 FPS on mid-range device

---

### T062: Run full quickstart manual testing
**Description**: Execute all 11 quickstart scenarios manually
**File**: Follow `specs/001-doc-my-spec/quickstart.md`
**Requirements**:
- Complete Scenarios 1-11 without errors
- Check all pass criteria
- Document any deviations or bugs
**Validation**: All scenarios pass, no critical bugs

---

## Dependencies

### Critical Path
```
Setup (T001-T003)
  → Data Definitions (T004-T009) [P]
  → Interfaces (T010-T016) [P]
  → Contract Tests (T017-T023) [P] ⚠️ MUST FAIL
  → Core Implementations (T024-T033)
    → Monster Systems (T034-T037) [P after T025 CombatSystem]
    → Challenge Components (T038-T040)
    → UI Systems (T041-T048)
    → Input (T049-T050)
    → Scenes (T051-T053)
    → Integration Tests (T054-T057) [P]
    → Polish (T058-T062)
```

### Blocking Dependencies
- T024-T030 implementations BLOCKED until T017-T023 tests fail
- T034-T036 monsters BLOCKED by T025 CombatSystem
- T038-T040 challenge components BLOCKED by T029 ChallengeManager
- T041-T045 UI systems BLOCKED by respective system implementations
- T051-T052 scenes BLOCKED by all core systems
- T054-T057 integration tests BLOCKED by scenes

### Parallel Execution Opportunities
- **Phase 3.2** (T004-T009): All ScriptableObjects independent
- **Phase 3.3** (T010-T016): All interfaces independent
- **Phase 3.4** (T017-T023): All contract tests independent
- **Phase 3.5** (T024-T025, T027-T028): InventorySystem, CombatSystem, ShopSystem, UpgradeSystem independent
- **Phase 3.6** (T034-T036): Monster subclasses independent after MonsterAI base
- **Phase 3.12** (T054-T057): All PlayMode tests independent

---

## Parallel Execution Examples

### Example 1: ScriptableObject Definitions (Phase 3.2)
```bash
# Launch T004-T009 together (7 tasks):
Task: "Create ItemData ScriptableObject in Script/Data/ItemData.cs"
Task: "Create WeaponData ScriptableObject in Script/Data/WeaponData.cs"
Task: "Create MonsterData ScriptableObject in Script/Data/MonsterData.cs"
Task: "Create ChallengeData ScriptableObject in Script/Data/ChallengeData.cs"
Task: "Create MapData ScriptableObject in Script/Data/MapData.cs"
Task: "Create SaveData serializable classes in Script/Core/SaveData.cs"
```

### Example 2: Contract Tests (Phase 3.4)
```bash
# Launch T017-T023 together (7 tasks):
Task: "Write InventorySystem contract tests in Tests/EditMode/InventorySystemTests.cs"
Task: "Write CombatSystem contract tests in Tests/EditMode/CombatSystemTests.cs"
Task: "Write SaveSystem contract tests in Tests/EditMode/SaveSystemTests.cs"
Task: "Write ShopSystem contract tests in Tests/EditMode/ShopSystemTests.cs"
Task: "Write UpgradeSystem contract tests in Tests/EditMode/UpgradeSystemTests.cs"
Task: "Write ChallengeSystem contract tests in Tests/EditMode/ChallengeSystemTests.cs"
Task: "Write MapSystem contract tests in Tests/EditMode/MapSystemTests.cs"
```

### Example 3: Independent System Implementations (Phase 3.5)
```bash
# Launch T024, T025, T027, T028 together (4 tasks):
Task: "Implement InventorySystem in Script/Player/InventorySystem.cs"
Task: "Implement CombatSystem in Script/Combat/CombatSystem.cs"
Task: "Implement ShopSystem in Script/Economy/ShopSystem.cs"
Task: "Implement UpgradeSystem in Script/Economy/UpgradeSystem.cs"
```

---

## Validation Checklist

### Contracts ✓
- [x] All 7 system interfaces have corresponding tests (T017-T023)
- [x] All contracts match data-model.md specifications

### Data Model ✓
- [x] All 9 entities have ScriptableObject/class definitions (T004-T009)
- [x] All entities have fields per data-model.md

### Tests Before Implementation ✓
- [x] Contract tests (T017-T023) come before implementations (T024-T030)
- [x] Tests marked to fail before implementation phase

### Parallel Tasks ✓
- [x] All [P] tasks modify different files
- [x] No [P] task depends on another [P] task in same group

### File Paths ✓
- [x] Every task specifies exact file path
- [x] Paths follow Unity project conventions

### Coverage ✓
- [x] All quickstart scenarios have corresponding implementation tasks
- [x] All user stories from spec.md covered

---

## Notes

- **TDD Enforcement**: T017-T023 tests MUST fail before starting T024-T030 implementations
- **Unity Specifics**: Use CreateAssetMenu, SerializeField, Coroutines, SceneManager
- **Performance**: Profile with Unity Profiler after T059-T060, target 60 FPS Android
- **Commit Strategy**: Commit after each passing test, after each implementation
- **Avoid**: Modifying same file in parallel, implementing before tests fail

---

## Estimated Timeline

- **Phase 3.1-3.2** (T001-T009): 1-2 days (setup + data definitions)
- **Phase 3.3** (T010-T016): 1 day (interfaces)
- **Phase 3.4** (T017-T023): 1-2 days (contract tests)
- **Phase 3.5-3.7** (T024-T040): 4-5 days (core systems)
- **Phase 3.8-3.11** (T041-T053): 3-4 days (UI + scenes)
- **Phase 3.12-3.13** (T054-T062): 2-3 days (integration + polish)

**Total Estimated**: 12-17 days (solo developer) or 5-7 days (with parallel execution by team)

---

## Success Metrics

- [ ] All 62 tasks completed
- [ ] All automated tests pass (EditMode + PlayMode)
- [ ] All 11 quickstart scenarios pass
- [ ] No critical bugs in manual testing
- [ ] Performance: 60 FPS on Android mid-range device
- [ ] Build size < 500MB
- [ ] Save/load < 1 second

**Definition of Done**: All metrics met, game playable on both Windows and Android per specification