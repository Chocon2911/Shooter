# Quickstart Guide
**Feature**: 3D Loot-and-Fight Mobile/PC Game
**Date**: 2025-09-27
**Purpose**: End-to-end integration test scenarios to validate feature completion

## Setup Prerequisites

1. **Unity Project Open**: Unity 2021.3 LTS or newer
2. **Scenes Built**: MainMenu, RestingMap_01, FightingMap_01, GameOver
3. **Test Data Created**:
   - At least 3 ItemData ScriptableObjects in `Asset/Items/`
   - At least 2 WeaponData ScriptableObjects in `Asset/Weapons/`
   - At least 2 MonsterData ScriptableObjects (1 Ambient, 1 Challenge) in `Asset/Monsters/`
   - At least 1 ChallengeData ScriptableObject in `Asset/Challenges/`
   - At least 2 MapData ScriptableObjects (1 Resting, 1 Fighting)

4. **Systems Implemented**: All systems from `contracts/game-systems.md`

---

## Scenario 1: New Game Flow (5 min)

**Goal**: Verify player can start new game and reach first fighting map

**Steps**:
1. Launch game in Unity Editor (Play mode)
2. Verify MainMenu scene loads
3. Click "New Game" button
4. **Expected**: Game loads RestingMap_01, player spawns at designated position
5. Verify player can move using WASD/arrow keys
6. Navigate to map transition trigger
7. **Expected**: UI shows "Press E to travel to [FightingMap_01]"
8. Press E
9. **Expected**: FightingMap_01 loads, player spawns correctly

**Pass Criteria**:
- MainMenu → RestingMap → FightingMap transitions work
- Player input responsive
- No errors in console

---

## Scenario 2: Combat & Loot (5 min)

**Goal**: Verify player can fight ambient monsters and collect loot

**Steps**:
1. Continue from Scenario 1 (in FightingMap_01)
2. Equip starting weapon from inventory
3. Approach ambient monster
4. **Expected**: Monster detects player, moves toward player
5. Press Left Click to attack
6. **Expected**: Weapon animation plays, monster takes damage
7. Continue attacking until monster health = 0
8. **Expected**: Monster dies, drops gold particles + possible item
9. Walk over gold
10. **Expected**: Gold added to player inventory, UI updates
11. If item dropped, walk over item
12. **Expected**: Item added to inventory, UI notification appears

**Pass Criteria**:
- Combat damage calculation works
- Monster AI detects and attacks player
- Loot drops on death
- Loot collection adds to inventory

---

## Scenario 3: Challenge System (10 min)

**Goal**: Verify challenge activation, wave progression, and failure conditions

**Steps**:
1. Continue from Scenario 2 (in FightingMap_01)
2. Navigate to challenge object (glowing pedestal/shrine)
3. **Expected**: UI shows "Press E to Start Challenge: [Challenge Name]"
4. Press E
5. **Expected**:
   - Challenge UI appears (timer, wave counter)
   - Map transitions locked
   - First wave spawns from entry points
6. Kill all monsters in wave 1
7. **Expected**: Wave 2 spawns after 2-second delay
8. Repeat for all waves OR let timer expire
9. **If timer expires**:
   - **Expected**: Challenge fails, GameOver screen shows
   - Stats displayed: time survived, monsters killed
10. **If all waves cleared before timer**:
    - **Expected**: Challenge success UI appears
    - Gold reward added
    - Possible loot item added to inventory
    - Map transitions unlocked

**Pass Criteria**:
- Challenge locks map exits
- Waves spawn sequentially
- Timer counts down correctly
- Failure on timeout shows GameOver screen
- Success awards rewards and unlocks map

---

## Scenario 4: Death & Respawn (3 min)

**Goal**: Verify death penalty and respawn mechanics

**Steps**:
1. Note current gold amount (e.g., 100 gold)
2. In FightingMap, take damage from monsters until health = 0
3. **Expected**: GameOver screen appears, shows stats
4. **Expected**: Gold reduced by 10% (e.g., 100 → 90)
5. Click "Respawn" button
6. **Expected**: Player respawns at RestingMap_01
7. **Expected**: Health = maxHealth, inventory intact

**Pass Criteria**:
- Death triggers GameOver screen
- 10% gold penalty applied
- Respawn at resting map with full health
- Inventory items preserved

---

## Scenario 5: Shop System (5 min)

**Goal**: Verify buying and selling items

**Steps**:
1. In RestingMap_01, navigate to shop NPC
2. Press E to open shop UI
3. **Expected**: Shop UI shows available items, prices (baseValue * 2)
4. Note current gold (e.g., 90)
5. Click Buy on item costing 50 gold
6. **Expected**: Gold = 40, item added to inventory
7. Click Buy on item costing 100 gold (insufficient funds)
8. **Expected**: Error message "Insufficient gold", no transaction
9. Click "Sell" tab in shop UI
10. Select item from inventory, click Sell
11. **Expected**: Gold increases by item.baseValue, item removed from inventory

**Pass Criteria**:
- Buy deducts gold (buyPrice = baseValue * 2), adds item
- Insufficient gold blocks purchase
- Sell adds gold (sellPrice = baseValue), removes item

---

## Scenario 6: Upgrade System (5 min)

**Goal**: Verify weapon/item upgrades

**Steps**:
1. In RestingMap_01, navigate to upgrade station NPC
2. Press E to open upgrade UI
3. Select weapon with upgradeLevel = 0, baseValue = 100
4. **Expected**: Upgrade cost shown = 100 gold (baseValue * 2^0)
5. Click Upgrade (assume sufficient gold)
6. **Expected**: Gold -= 100, weapon.upgradeLevel = 1
7. **Expected**: Weapon damage increased by 25% (displayed in UI)
8. Select same weapon again
9. **Expected**: Upgrade cost = 200 (baseValue * 2^1)
10. Click Upgrade
11. **Expected**: Gold -= 200, weapon.upgradeLevel = 2
12. Try to upgrade again
13. **Expected**: Button disabled, message "Max upgrade level"

**Pass Criteria**:
- Upgrade costs calculated correctly (baseValue * 2^level)
- Each upgrade increases level, deducts gold
- Max level (2) blocks further upgrades
- Damage increase applies (verify in combat)

---

## Scenario 7: Storage System (3 min)

**Goal**: Verify global storage deposit/withdraw

**Steps**:
1. In RestingMap_01, navigate to storage chest
2. Press E to open storage UI
3. Select item from inventory, click "Deposit"
4. **Expected**: Item moves from inventory to storage list
5. Close storage UI, travel to different resting map (if multiple exist)
6. Open storage again
7. **Expected**: Previously deposited item still in storage (global persistence)
8. Click "Withdraw" on item
9. **Expected**: Item moves from storage to inventory

**Pass Criteria**:
- Deposit removes from inventory, adds to storage
- Storage accessible from any resting map
- Withdraw adds to inventory, removes from storage
- Unlimited capacity (no errors after depositing 100+ items)

---

## Scenario 8: Save & Load (5 min)

**Goal**: Verify save/load preserves all state

**Steps**:
1. In RestingMap_01, accumulate specific state:
   - Note exact gold amount (e.g., 347)
   - Note inventory contents (list all items)
   - Note weapon upgrade levels
   - Note storage contents
2. Open pause menu, click "Save Game"
3. **Expected**: Save confirmation message appears
4. Close Unity Play mode
5. Re-enter Play mode, click "Load Game" on MainMenu
6. **Expected**:
   - Game loads to RestingMap_01
   - Player position matches last position
   - Gold amount exact match (347)
   - Inventory exactly matches saved state
   - Weapon upgrade levels preserved
   - Storage contents intact

**Pass Criteria**:
- All state preserved: gold, inventory, storage, position, upgrades
- Load works after Unity editor restart
- Save during challenge blocked (CanSave returns false)

---

## Scenario 9: Trading System (3 min)

**Goal**: Verify NPC trading (distinct from shop)

**Steps**:
1. In RestingMap_01, navigate to trader NPC
2. Press E to open trading UI
3. **Expected**: Trader shows specific trade offers (e.g., "3x Iron Ore → 1x Steel Sword")
4. Select trade, click "Accept"
5. **Expected**: Required items removed from inventory, reward item added
6. Try trade without required items
7. **Expected**: Error message "Missing required items"

**Pass Criteria**:
- Trading removes required items, adds reward
- Missing items blocks trade
- Distinct from shop (specific trades, not gold-based)

---

## Scenario 10: Platform Input (Android vs Windows) (5 min)

**Goal**: Verify input adapts correctly per platform

**Steps**:
1. **Windows Test**:
   - In Editor (simulates Windows), verify WASD movement
   - Verify mouse click attacks
   - Verify E key interactions
   - Verify UI navigation with Tab/Enter

2. **Android Test**:
   - Build and deploy to Android device OR use Unity Remote
   - Verify virtual joystick appears for movement
   - Verify attack button appears on screen
   - Verify interaction button appears near interactables
   - Verify UI uses touch input (no keyboard prompts)

**Pass Criteria**:
- Windows: Keyboard/mouse input works
- Android: Touch controls functional, no keyboard prompts in UI
- UI layouts adapt to aspect ratio (16:9 vs 18:9 mobile)

---

## Scenario 11: Complete Challenge Flow (15 min)

**Goal**: Full end-to-end challenge completion

**Steps**:
1. Start from MainMenu
2. New Game → RestingMap_01
3. Open shop, buy best weapon affordable
4. Travel to FightingMap_01
5. Fight ambient monsters to collect gold/loot
6. Return to RestingMap, upgrade weapon once
7. Return to FightingMap, start challenge
8. Complete all waves within time limit
9. **Expected**: Success, rewards granted
10. Return to RestingMap, deposit loot in storage
11. Save game
12. Load game, verify all state preserved

**Pass Criteria**:
- All systems integrate smoothly
- Challenge success awards rewards
- Save/load works across full flow
- No errors or crashes

---

## Automated Test Validation

After manual quickstart, run automated tests:

```bash
# In Unity Test Runner
Run All EditMode Tests  # Data validation, business logic
Run All PlayMode Tests  # System integration
```

**Expected**:
- All tests pass (green)
- No warnings in console
- Performance tests < 1ms per frame

---

## Success Criteria Summary

| Scenario | Systems Tested | Pass/Fail |
|----------|---------------|-----------|
| 1. New Game | Map loading, scene transitions | [ ] |
| 2. Combat & Loot | Combat system, inventory, monster AI | [ ] |
| 3. Challenge | Challenge system, wave spawning, timer | [ ] |
| 4. Death & Respawn | Death penalty, respawn logic | [ ] |
| 5. Shop | Shop system, buy/sell transactions | [ ] |
| 6. Upgrade | Upgrade system, cost calculation | [ ] |
| 7. Storage | Storage system, global persistence | [ ] |
| 8. Save & Load | Save system, data serialization | [ ] |
| 9. Trading | Trading system, item exchanges | [ ] |
| 10. Platform Input | Input abstraction, UI adaptation | [ ] |
| 11. Complete Flow | All systems integrated | [ ] |

**Definition of Done**: All scenarios pass, automated tests green, no critical bugs