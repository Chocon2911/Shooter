# Research & Technical Decisions
**Feature**: 3D Loot-and-Fight Mobile/PC Game
**Date**: 2025-09-27

This document resolves all NEEDS CLARIFICATION markers from the feature specification with researched decisions.

## 1. Challenge Time Limits

**Decision**: Configurable per-challenge, default 5 minutes (300 seconds)

**Rationale**:
- Unity Coroutines provide simple countdown timer implementation
- Configurable times allow different difficulty levels per map
- 5 minutes balances tension with playability for mobile sessions
- Stored in ChallengeData ScriptableObject for designer control

**Alternatives considered**:
- Fixed time for all challenges - rejected (lacks variety)
- Time per stage instead of total - rejected (too complex for MVP)

**Implementation**: `float timeLimit` field in ChallengeData SO, countdown managed by ChallengeManager

---

## 2. Time Expiration Behavior

**Decision**: Automatic failure when time expires

**Rationale**:
- Clear, predictable outcome for players
- Simpler to implement than triggering final wave
- Motivates strategic play and time management
- Standard pattern in wave-defense games

**Alternatives considered**:
- Trigger emergency final wave - rejected (adds complexity, unclear win condition)
- Extend time with penalty - rejected (undermines time pressure)

**Implementation**: ChallengeManager checks timer, calls EndChallenge(false) on expiration

---

## 3. Reward Types

**Decision**: Multiple reward types - currency (primary), items (loot drops), no experience system for MVP

**Rationale**:
- Currency as guaranteed reward ensures progression
- Random item drops add excitement and variety
- XP system deferred to post-MVP (adds complexity)
- Fits loot-focused game design

**Alternatives considered**:
- Currency only - rejected (too predictable)
- XP/level system - deferred (scope creep for MVP)

**Implementation**:
- ChallengeData contains `int currencyReward` and `ItemData[] possibleLootDrops`
- On success: Add currency + roll loot table

---

## 4. Storage Architecture

**Decision**: Global storage accessible from any resting map, unlimited capacity

**Rationale**:
- Player convenience - no need to return to specific map
- Matches unlimited inventory design (consistency)
- Simpler data model (single storage list)
- Common pattern in action RPGs

**Alternatives considered**:
- Per-map storage - rejected (frustrating UX, complex data management)
- Limited capacity - rejected (conflicts with unlimited inventory principle)

**Implementation**: Single persistent List<ItemData> in SaveData, accessible via StorageSystem

---

## 5. Upgrade Mechanics

**Decision**: Tier-based upgrades (3 tiers: +0, +1, +2), costs currency only, linear stat increases

**Rationale**:
- Simple system: 3 tiers provides meaningful progression without overwhelming
- Currency-only cost keeps economy straightforward (no crafting materials)
- Linear increases (+25% per tier) easy to balance and communicate
- Matches mobile game conventions (Star Upgrade pattern)

**Alternatives considered**:
- Material-based crafting - rejected (adds inventory complexity)
- Unlimited tiers - rejected (balance nightmare)
- Exponential scaling - rejected (hard to balance for casual players)

**Implementation**:
- WeaponData/ItemData has `int upgradeLevel` (0-2)
- UpgradeSystem.GetUpgradeCost(data) returns currency needed for next level
- Base cost * (2 ^ currentLevel) formula

---

## 6. Currency System

**Decision**: Single currency type (Gold)

**Rationale**:
- Simplest economy for MVP
- All sources (combat, selling, rewards) produce one resource
- All sinks (buying, upgrading, death) consume one resource
- Reduces UI complexity and player cognitive load

**Alternatives considered**:
- Dual currency (soft + premium) - rejected (monetization not in scope, adds complexity)
- Multiple currency types - rejected (over-engineering for single-player game)

**Implementation**: `int gold` field in PlayerData, modified by all economy systems

---

## 7. Character Stats

**Decision**: Health only for MVP, stamina deferred

**Rationale**:
- Health sufficient for core combat loop
- Adding stamina (dodge, sprint) increases scope significantly
- Mobile touch controls better suit simpler systems
- Can add post-MVP if needed

**Alternatives considered**:
- Health + Stamina + Energy - rejected (scope creep, complex mobile UI)
- Health + Armor/Defense - deferred (adds complexity to damage calc)

**Implementation**: PlayerHealth component tracks `float currentHealth` and `float maxHealth`

---

## 8. Monster Loot Drops

**Decision**: Monsters drop currency on death, rare chance for items

**Rationale**:
- Currency drops provide consistent reward feedback
- Item drops add excitement but don't block progression
- Loot table system allows designer control per monster type
- Standard action RPG pattern

**Alternatives considered**:
- No ambient monster drops - rejected (lacks motivation to fight freely)
- Items only - rejected (currency progression too slow)

**Implementation**:
- MonsterData has `int goldDrop` and `LootTable itemDrops` (item + % chance)
- On death: Always drop gold, roll itemDrops table

---

## 9. Save System Mechanics

**Decision**: Manual save only (no autosave), single save slot, JSON format

**Rationale**:
- Manual save gives player control (important for challenge deaths)
- Single slot simplifies UI/UX for MVP
- JSON human-readable for debugging, good Unity support
- PlayerPrefs stores save data as JSON string

**Alternatives considered**:
- Autosave - rejected (could save bad states during challenges)
- Multiple slots - deferred (UI complexity, rare use case)
- Binary format - rejected (harder to debug, marginal performance gain)

**Implementation**: SaveSystem serializes PlayerData/InventoryData/ProgressData to JSON, stores in PlayerPrefs

---

## 10. Death Penalty Percentage

**Decision**: 10% currency loss on death

**Rationale**:
- Meaningful but not punishing (keeps game fun)
- Standard in modern action games (Dark Souls ~10-15%, Minecraft 0%, compromise)
- Encourages caution without harsh punishment
- Easy to adjust in testing

**Alternatives considered**:
- 25%+ loss - rejected (too punishing for casual mobile audience)
- 0% loss - rejected (no risk/reward tension)

**Implementation**: On death, PlayerInventory.gold *= 0.9f (truncated to int)

---

## 11. Platform Performance Differences

**Decision**: Full feature parity, mobile has lower graphics settings (texture resolution, shadow quality), target 60 FPS both platforms

**Rationale**:
- Gameplay parity ensures fair experience
- Graphics settings easily adjusted in Unity Quality settings
- 60 FPS achievable on mid-range Android with optimizations (object pooling, LOD, occlusion)
- No feature cuts needed

**Alternatives considered**:
- Reduce monster count on mobile - rejected (changes game balance)
- 30 FPS on mobile - rejected (poor UX for action game)

**Implementation**:
- Quality Settings profiles (High for Windows, Medium for Android)
- Object pooling for monsters/projectiles
- Occlusion culling in map design

---

## 12. Game Over Screen Information

**Decision**: Show: time survived, monsters killed, currency lost, retry/main menu buttons

**Rationale**:
- Provides feedback on player performance
- Currency lost reinforces death consequence
- Clear options for next action (retry or quit)
- Standard pattern for action games

**Alternatives considered**:
- Detailed stats - deferred (UI clutter for mobile)
- Rewards earned before death - rejected (confusing, implies progress saved)

**Implementation**: GameOverScreen UI with text fields populated by ChallengeManager/PlayerHealth data

---

## 13. Save During Challenge

**Decision**: Cannot save during active challenge, save disabled, challenge resets on load

**Rationale**:
- Prevents save-scumming (save before death, reload)
- Challenges are meant to be completed in single session
- Simpler implementation (no mid-challenge state serialization)
- Mobile sessions typically short enough

**Alternatives considered**:
- Allow saving, restore mid-challenge - rejected (complex serialization, enables exploits)
- Auto-fail challenge on load - same outcome as reset

**Implementation**: SaveSystem.CanSave() returns false if ChallengeManager.IsActive, load resets challenge state

---

## 14. Challenge Repeatability

**Decision**: Challenges can be repeated for full rewards

**Rationale**:
- Encourages replayability and farming
- Players can retry for better loot drops
- No need to track completion state
- Common in loot-focused games (Diablo, Warframe)

**Alternatives considered**:
- One-time rewards - rejected (limits replayability, punishes experimentation)
- Reduced rewards on repeat - deferred (adds complexity)

**Implementation**: No completion tracking needed, every clear gives full rewards

---

## Technical Stack Summary

**Unity Version**: 2021.3 LTS (long-term support, stable for mobile)

**Key Packages**:
- Unity Input System (new) - cross-platform input abstraction
- TextMeshPro - better text rendering than legacy UI Text
- Unity UI (uGUI) - standard UI system
- Unity Test Framework - automated testing

**Architecture Patterns**:
- ScriptableObject data architecture - designer-friendly, memory-efficient
- Component-based systems - Unity standard, testable
- Event-driven communication - decoupled systems (C# events/UnityEvents)
- Object pooling - performance optimization for mobile

**Testing Strategy**:
- EditMode tests - data validation, business logic (fast iteration)
- PlayMode tests - gameplay systems, integration (real Unity environment)
- Manual QA - platform-specific testing (touch vs mouse), performance profiling

**Performance Optimizations**:
- Object pooling (monsters, projectiles, UI elements)
- Occlusion culling (hide off-screen geometry)
- LOD (Level of Detail) for distant objects
- Texture atlasing (reduce draw calls)
- Quality Settings profiles per platform

---

## Resolved NEEDS CLARIFICATION Count

All 14 NEEDS CLARIFICATION markers from spec.md have been resolved with concrete, implementable decisions.