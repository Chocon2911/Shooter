# Implementation Plan: 3D Loot-and-Fight Mobile/PC Game

**Branch**: `001-doc-my-spec` | **Date**: 2025-09-27 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/mnt/c/Local_D/VMioSys/Game/Shooter/Assets/MyAsset/specs/001-doc-my-spec/spec.md`

## Execution Flow (/plan command scope)
```
1. Load feature spec from Input path
   → Loaded successfully
2. Fill Technical Context (scan for NEEDS CLARIFICATION)
   → Detected Unity C# 3D game project
   → Project Type: Mobile (Android + Windows PC cross-platform)
   → Structure Decision: Unity game project structure
3. Fill the Constitution Check section based on the constitution document
   → Constitution file is template only - using Unity game development standards
4. Evaluate Constitution Check section below
   → No violations - standard Unity game architecture
   → Update Progress Tracking: Initial Constitution Check
5. Execute Phase 0 → research.md
   → Research Unity best practices for identified areas
6. Execute Phase 1 → contracts, data-model.md, quickstart.md, CLAUDE.md
   → Generate game design documents and data structures
7. Re-evaluate Constitution Check section
   → Verify design follows Unity patterns
   → Update Progress Tracking: Post-Design Constitution Check
8. Plan Phase 2 → Describe task generation approach (DO NOT create tasks.md)
9. STOP - Ready for /tasks command
```

**IMPORTANT**: The /plan command STOPS at step 8. Phases 2-4 are executed by other commands:
- Phase 2: /tasks command creates tasks.md
- Phase 3-4: Implementation execution (manual or via tools)

## Summary
Build a cross-platform 3D action game for Android and Windows featuring:
- Two map types: Safe resting maps (shop/storage/upgrades) and combat fighting maps
- Dual monster system: Ambient monsters roaming freely + stronger challenge-specific waves
- Wave-based challenge system with timed stages and entry point spawning
- Economy system with unlimited inventory, NPC trading, item/weapon upgrades
- Save/load system with death penalty (currency loss, no item loss)
- Platform-adaptive UI for touch (Android) and keyboard/mouse (Windows)

Technical approach: Unity C# game with scriptable object-based data architecture, modular scene management, and cross-platform input abstraction.

## Technical Context
**Language/Version**: C# (Unity 2021.3 LTS or newer)
**Primary Dependencies**: Unity Engine, Unity Input System (new), TextMeshPro, Unity UI
**Storage**: PlayerPrefs for save data, ScriptableObjects for game data definitions, JSON serialization
**Testing**: Unity Test Framework (PlayMode + EditMode tests), NUnit
**Target Platform**: Android (API 24+), Windows PC (x64)
**Project Type**: Mobile cross-platform game (Android + Windows PC)
**Performance Goals**: 60 FPS on mid-range Android devices, <2s scene load times
**Constraints**: Mobile memory budget <500MB, offline single-player only, no external services
**Scale/Scope**: 5-10 maps, 10-20 monster types, 50-100 items/weapons, 10-15 challenge configurations

## Constitution Check
*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

Since the constitution file is a template, applying standard Unity game development principles:

**Unity Architecture Standards**:
- [x] ScriptableObject-based data architecture for items, weapons, monsters, challenges
- [x] MonoBehaviour components for gameplay systems (player controller, inventory, combat)
- [x] Scene-based map organization with persistent managers
- [x] Event-driven communication between systems (UnityEvents/C# events)
- [x] Separation of data (ScriptableObjects) from logic (MonoBehaviours) from presentation (UI)

**Testing Strategy**:
- [x] EditMode tests for data validation and business logic
- [x] PlayMode tests for gameplay systems and user flows
- [x] Manual QA for cross-platform input and UI layouts

**Cross-Platform Requirements**:
- [x] Input abstraction layer for touch vs keyboard/mouse
- [x] UI responsive design for different aspect ratios
- [x] Platform-specific build configurations

**Result**: PASS - Standard Unity game architecture with no unusual complexity

## Project Structure

### Documentation (this feature)
```
specs/001-doc-my-spec/
├── plan.md              # This file (/plan command output)
├── research.md          # Phase 0 output (/plan command)
├── data-model.md        # Phase 1 output (/plan command)
├── quickstart.md        # Phase 1 output (/plan command)
├── contracts/           # Phase 1 output (/plan command)
│   ├── game-systems.md  # Core gameplay system interfaces
│   ├── data-schemas.md  # ScriptableObject structures
│   └── ui-contracts.md  # UI system contracts
└── tasks.md             # Phase 2 output (/tasks command - NOT created by /plan)
```

### Source Code (Unity project structure)
```
Script/
├── Core/                    # Core game systems
│   ├── GameManager.cs       # Main game state manager
│   ├── SaveSystem.cs        # Save/load functionality
│   └── SceneLoader.cs       # Scene management
│
├── Player/                  # Player-related scripts
│   ├── PlayerController.cs  # Movement and input
│   ├── PlayerCombat.cs      # Combat actions
│   ├── PlayerHealth.cs      # Health system
│   └── PlayerInventory.cs   # Inventory management
│
├── Combat/                  # Combat systems
│   ├── Weapon.cs            # Weapon behavior
│   ├── CombatSystem.cs      # Combat calculations
│   └── Damageable.cs        # Health/damage interface
│
├── Monsters/                # Monster/AI systems
│   ├── MonsterAI.cs         # AI behavior base
│   ├── AmbientMonster.cs    # Free-roaming monsters
│   ├── ChallengeMonster.cs  # Challenge-specific monsters
│   └── MonsterSpawner.cs    # Spawning logic
│
├── Challenge/               # Challenge system
│   ├── ChallengeManager.cs  # Challenge orchestration
│   ├── ChallengeObject.cs   # Interactable trigger
│   ├── StageController.cs   # Stage progression
│   └── EntryPoint.cs        # Spawn point marker
│
├── Economy/                 # Economy systems
│   ├── ShopSystem.cs        # Buy/sell logic
│   ├── TradingSystem.cs     # NPC trading
│   ├── UpgradeSystem.cs     # Item/weapon upgrades
│   └── StorageSystem.cs     # Item storage
│
├── Map/                     # Map systems
│   ├── MapTransition.cs     # Map travel
│   ├── RestingMap.cs        # Resting map manager
│   ├── FightingMap.cs       # Fighting map manager
│   └── ItemSpawner.cs       # Loot placement
│
├── UI/                      # UI systems
│   ├── MainMenu.cs          # Start screen
│   ├── GameOverScreen.cs    # Game over UI
│   ├── InventoryUI.cs       # Inventory display
│   ├── ShopUI.cs            # Shop interface
│   ├── StorageUI.cs         # Storage interface
│   ├── UpgradeUI.cs         # Upgrade interface
│   ├── TradingUI.cs         # Trading interface
│   └── InputManager.cs      # Platform input abstraction
│
├── Data/                    # ScriptableObject definitions
│   ├── ItemData.cs          # Item SO structure
│   ├── WeaponData.cs        # Weapon SO structure
│   ├── MonsterData.cs       # Monster SO structure
│   ├── ChallengeData.cs     # Challenge SO structure
│   └── MapData.cs           # Map configuration SO
│
└── System/                  # Existing utility systems
    ├── HuyMonoBehaviour.cs  # Base MonoBehaviour
    ├── IComponent.cs        # Component interface
    └── Util.cs              # Utilities

Scene/
├── MainMenu.unity           # Start screen scene
├── RestingMap_01.unity      # Example resting map
├── FightingMap_01.unity     # Example fighting map
└── GameOver.unity           # Game over scene

Asset/
├── Items/                   # Item ScriptableObject assets
├── Weapons/                 # Weapon ScriptableObject assets
├── Monsters/                # Monster ScriptableObject assets
└── Challenges/              # Challenge ScriptableObject assets

Tests/
├── EditMode/                # Unit tests
│   ├── CombatSystemTests.cs
│   ├── InventoryTests.cs
│   └── SaveSystemTests.cs
└── PlayMode/                # Integration tests
    ├── ChallengeFlowTests.cs
    ├── ShopSystemTests.cs
    └── PlayerDeathTests.cs
```

**Structure Decision**: Unity mobile game structure organized by feature domains. Script/ contains all C# code organized into functional areas. Scene/ contains Unity scenes for different map types. Asset/ contains ScriptableObject data assets. Tests/ contains automated tests.

## Phase 0: Outline & Research
1. **Extract unknowns from Technical Context** above:
   - Challenge time limits and configuration → Research Unity timer patterns
   - Reward types (currency, items, experience) → Research progression systems
   - Upgrade mechanics and tiers → Research RPG upgrade patterns in Unity
   - Storage architecture (per-map vs global) → Research Unity data persistence
   - Currency types (single vs multiple) → Research economy design patterns
   - Character stats beyond health → Research action game stat systems
   - Monster loot drops → Research loot table implementations
   - Save system mechanics (manual/auto, slots) → Research Unity save systems
   - Death penalty percentage → Research game balance best practices
   - Platform performance differences → Research Unity Android optimization

2. **Generate and dispatch research agents**:
   ```
   Task 1: "Research Unity timer and countdown systems for challenge time limits"
   Task 2: "Research game progression and reward systems (currency, items, XP)"
   Task 3: "Research RPG upgrade mechanics and tier systems in Unity"
   Task 4: "Research Unity data persistence patterns for save systems"
   Task 5: "Research game economy design (single vs multi-currency)"
   Task 6: "Research action game character stat systems"
   Task 7: "Research loot table and drop rate implementations"
   Task 8: "Research Unity save/load best practices (PlayerPrefs, JSON, binary)"
   Task 9: "Research game balance for death penalties"
   Task 10: "Research Unity Android optimization techniques"
   ```

3. **Consolidate findings** in `research.md` using format:
   - Decision: [what was chosen]
   - Rationale: [why chosen]
   - Alternatives considered: [what else evaluated]

**Output**: research.md with all NEEDS CLARIFICATION resolved

## Phase 1: Design & Contracts
*Prerequisites: research.md complete*

1. **Extract entities from feature spec** → `data-model.md`:
   - Player Character (health, inventory, currency, position, equipped weapon)
   - Weapon (damage, attack speed, special abilities, upgrade level)
   - Item (type, rarity, value, effects, stackable)
   - Monster (health, damage, speed, loot table, AI type)
   - Challenge (stages, time limit, entry points, rewards, difficulty)
   - Map (type, size, spawn points, interactables)
   - Inventory (items list, unlimited capacity)
   - Storage (items list, persistent across maps)
   - Save Data (player state, inventory, progress, map location)

2. **Generate API contracts** from functional requirements:
   - IInventorySystem: Add/Remove/Has item operations
   - ICombatSystem: Attack, TakeDamage, CalculateDamage
   - ISaveSystem: Save, Load, HasSaveData
   - IShopSystem: Buy, Sell, GetPrice
   - IUpgradeSystem: Upgrade, GetUpgradeCost, CanUpgrade
   - IChallengeSystem: StartChallenge, NextStage, EndChallenge
   - IMapSystem: LoadMap, CanTransition, GetCurrentMap
   - Output to `/contracts/game-systems.md`

3. **Generate contract tests** from contracts:
   - InventorySystemTests.cs (add/remove/unlimited capacity)
   - CombatSystemTests.cs (damage calculation, death handling)
   - SaveSystemTests.cs (save/load round-trip, data integrity)
   - ShopSystemTests.cs (buy/sell transactions, currency changes)
   - Tests must fail (no implementation yet)

4. **Extract test scenarios** from user stories:
   - Scenario 1: Player enters resting map, buys weapon, inventory updates
   - Scenario 2: Player loots item in fighting map, item added to inventory
   - Scenario 3: Player activates challenge, monsters spawn in stages
   - Scenario 4: Player dies in challenge, respawns at resting map with currency loss
   - Scenario 5: Player completes challenge, receives rewards
   - Scenario 6: Player stores items, retrieves them later
   - Scenario 7: Player saves game, closes, reopens, loads successfully
   - Scenario 8: UI adapts correctly on Android vs Windows

5. **Update agent file incrementally** (O(1) operation):
   - Run `.specify/scripts/bash/update-agent-context.sh claude`
   - Add Unity C#, ScriptableObjects, Unity Test Framework
   - Document project structure and key systems
   - Keep under 150 lines for token efficiency

**Output**: data-model.md, /contracts/*, failing tests, quickstart.md, CLAUDE.md

## Phase 2: Task Planning Approach
*This section describes what the /tasks command will do - DO NOT execute during /plan*

**Task Generation Strategy**:
- Load `.specify/templates/tasks-template.md` as base
- Generate tasks from Phase 1 design docs (contracts, data model, quickstart)
- Each contract interface → contract test task [P]
- Each ScriptableObject → data definition task [P]
- Each system (Combat, Inventory, etc.) → implementation task
- Each UI screen → UI implementation task
- Integration tests for user scenarios

**Ordering Strategy**:
- TDD order: Tests before implementation
- Dependency order:
  1. Data layer (ScriptableObjects) [P]
  2. Core systems (Save, Combat, Inventory) [P]
  3. Game systems (Challenge, Shop, Trading) - depends on core
  4. UI layer - depends on game systems
  5. Integration tests - depends on all systems
- Mark [P] for parallel execution (independent systems)

**Estimated Output**: 45-55 numbered, ordered tasks in tasks.md covering:
- 10 tasks for data model (ScriptableObjects)
- 15 tasks for core systems with tests
- 10 tasks for game-specific systems
- 10 tasks for UI implementation
- 5 tasks for scenes and integration
- 5 tasks for platform testing and polish

**IMPORTANT**: This phase is executed by the /tasks command, NOT by /plan

## Phase 3+: Future Implementation
*These phases are beyond the scope of the /plan command*

**Phase 3**: Task execution (/tasks command creates tasks.md)
**Phase 4**: Implementation (execute tasks.md following Unity best practices)
**Phase 5**: Validation (run tests, execute quickstart.md, performance validation on Android and Windows)

## Complexity Tracking
*Fill ONLY if Constitution Check has violations that must be justified*

No constitutional violations identified. Standard Unity game architecture with clear separation of concerns.

## Progress Tracking
*This checklist is updated during execution flow*

**Phase Status**:
- [x] Phase 0: Research complete (/plan command)
- [x] Phase 1: Design complete (/plan command)
- [x] Phase 2: Task planning complete (/plan command - describe approach only)
- [x] Phase 3: Tasks generated (/tasks command) - 62 tasks created
- [ ] Phase 4: Implementation complete
- [ ] Phase 5: Validation passed

**Gate Status**:
- [x] Initial Constitution Check: PASS
- [x] Post-Design Constitution Check: PASS
- [x] All NEEDS CLARIFICATION resolved (14 items resolved in research.md)
- [x] Complexity deviations documented (none)

---
*Based on Unity game development best practices*