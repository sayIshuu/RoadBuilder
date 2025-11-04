# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

---

## Project Overview

**RoadBuilder** is a casual puzzle game where players connect randomly generated road tile blocks to score points by linking grid edges. Built in Unity with a 1-week development timeline by a 4-person team.

**Core Mechanic**: Place tiles on a 5×5 grid to create roads connecting opposite edges. Completed roads are destroyed and award points based on length³ (cubed).

---

## Unity Development

### Building and Running

**Unity Editor**: Open the project in Unity (version info in `ProjectSettings/ProjectVersion.txt`)
- Main play scene: `Assets/Scenes/MainScene.unity`
- Tutorial scene: `Assets/Scenes/TutorialScene.unity`

**Android Build**: Project uses Google Mobile Ads SDK for monetization
- Configure AdMob settings in `Assets/Scripts/AdMob/AdMobConfig.asset`
- Set `useTestIds = true` for testing, `false` for production

### Scene Flow

1. Game starts → `TutorialManager` checks `PlayerPrefs["TutorialCompleted"]`
2. If incomplete → Load `TutorialScene`
3. If complete → Load `MainScene`
4. Restart button → Reload `MainScene`

**Important**: Use `SceneManager.GetActiveScene().name` to detect current scene when implementing scene-specific features (e.g., ads should not show in `TutorialScene`).

---

## Architecture

### Singleton Pattern (DontDestroyOnLoad)

The codebase uses multiple singleton managers that persist across scene reloads:

- **`AdsManager.I`** (`Assets/Scripts/AdMob/AdsManager.cs:9`) - Ad management
- **`SoundManager.Instance`** (`Assets/Scripts/SoundManager.cs:5`) - Audio playback
- **`TurnCounting.Instance`** (`Assets/Scripts/TurnCounting.cs:8`) - Turn/goal tracking
- **`VibrationManager.Instance`** - Haptic feedback
- **`ThemeManager`** - UI theming
- **`LifeManager`** - Life system (currently commented out)

All singletons follow this pattern:
```csharp
public static ClassName Instance { get; private set; }

void Awake() {
    if (Instance != null) { Destroy(gameObject); return; }
    Instance = this;
    DontDestroyOnLoad(gameObject);
}
```

### Game Flow Architecture

**Turn Processing** (`BoardCheck.cs:104-119`):
```
Tile Placement → BoardCheck.Check() → Path Detection (Union-Find) →
→ [Path Found] → GetScore() → Animation → DestroyTiles → ProcessTurn()
→ [No Path] → ProcessTurn()
→ ProcessTurn() → TurnCounting.CheckTurnAndGoal() → Check Game Over
```

**Game Over Conditions** (`BoardCheck.cs:111-118`, `TurnCounting.cs:92-95`):
1. Grid full: `displayedTileCount >= 25`
2. Goal failure: `turnCount >= limitTurn && score < goalScore`

### Core Systems

#### 1. Road Connection Algorithm (Union-Find)

**File**: `Assets/Scripts/BoardCheck.cs:51-102`

Uses Union-Find data structure on a 7×7 grid (5×5 playable + 2-tile border for edge detection):
- Grid coordinates: `[i,j]` where 1 ≤ i,j ≤ 5 are playable, 0 and 6 are edges
- Tile connections encoded as bitmask in `adj[i,j]`: `1=up, 2=right, 4=down, 8=left`
- Tiles merge via `UfMerge()` when adjacent directions match
- Completed roads detected when any edge cell's root connects to another edge

**Key method**: `Check()` - Called after each tile placement to detect completed roads.

#### 2. Tile Generation System

**File**: `Assets/Scripts/TileGenerator.cs`

Generates 3 random tiles with weighted probabilities:
- 95%: Basic tiles (1-6) - straight, corner pieces
- 4.5%: Three-way intersections (7-10)
- 0.5%: Four-way intersection (11)

**Constraint**: Prevents all 3 tiles from being identical (`IsAllSame()` check on line 201-205).

**Tile Type Encoding** (lines 116-170):
- Types map to bitmask values (e.g., `10 = horizontal line`, `5 = vertical line`, `15 = four-way`)
- Colors indicate complexity: white (basic), magenta (three-way), yellow/red (four-way)

#### 3. Scoring System

**File**: `Assets/Scripts/ScoreManager.cs`

- Formula: `score += length³` (`BoardCheck.cs:180,351`)
- Best score persists via `PlayerPrefs["BestScore"]`
- Turn-based goal system: Every 10 turns, player must reach `goalScore` or game ends
- Goal increases exponentially with level progression (`TurnCounting.cs:99-126`)

#### 4. AdMob Integration

**Files**:
- `Assets/Scripts/AdMob/AdsManager.cs` (wrapper)
- `Assets/Scripts/AdMob/AdMobService.cs` (implementation)
- `Assets/Scripts/AdMob/IAdsService.cs` (interface)

**Current Ads**:
- Banner: Auto-loaded and shown on startup (`AdsManager.cs:26-27`)
- Interstitial: Preloaded, shown via `AdsManager.I.TryShowInterstitial(onClosed: Action)`
- Rewarded: Preloaded, shown via `AdsManager.I.TryShowRewarded(onReward: Action, onClosed: Action)`

**Ad Configuration**: `Assets/Scripts/AdMob/AdMobConfig.cs` (ScriptableObject)
- Toggle `useTestIds` for test vs. production ad unit IDs
- Supports Android ad unit IDs (banner, interstitial, rewarded)

**Auto-preloading**: Ads automatically reload after being shown (`AdMobService.cs:106,169`)

---

## Key Interactions

### Tile Placement Flow

1. **Generation**: `TileGenerator` creates 3 tiles in inventory slots
2. **Drag**: `TileDraggable` handles IBeginDrag/IDrag/IEndDrag events
3. **Placement**: `BoardSlot.PlaceTile()` positions tile on grid
4. **Validation**: `OnEndDrag()` checks if placement is valid (empty slot, on board)
5. **State Update**: Updates `BoardCheck.adj[i,j]` with tile's bitmask
6. **Check**: Calls `BoardCheck.Check()` to detect completed roads
7. **Scoring**: If road found, play animation → destroy tiles → add score
8. **Turn Processing**: Increment turn, check goals, potentially trigger game over

### Reroll System

**File**: `Assets/Scripts/RerollButton.cs`

- Starts with 3 reroll charges (`Awake():15`)
- Each reroll regenerates tiles without consuming a turn
- Recharges: +1 charge every 100 turns (`TurnCounting.cs:79-82`)
- UI updates via `rerollCountText` (TextMeshProUGUI)
- Plays forbid sound when depleted (`RerollButton.cs:21-24`)

### Animation System

**Path Completion Animation** (`BoardCheck.cs:294-356`):
- Uses DOTween for tile bounce sequences
- Plays domino-style animation along sorted path
- Supports fast-forward when holding mouse button
- Plays rising pitch SFX via `SfxManager.PlayRisingSfx()`
- Path sorting: BFS from edge to maintain visual flow (`SortPath()` on line 187-254)

---

## Important Patterns and Conventions

### PlayerPrefs Keys

- `"BestScore"` - Highest score achieved
- `"TutorialCompleted"` - 0/1 flag for tutorial status
- `"BGM_VOLUME"` - Background music volume (0-1)
- `"SFX_VOLUME"` - Sound effects volume (0-1)
- `"LastAdShowTime"` - (For new timer-based ad feature, see TASKS.md)

### Audio Management

**All sound playback goes through `SoundManager.Instance`**:
- `PlayDisplaySound()` - Tile placement (randomized)
- `PlayScoreSound()` / `PlayLargeScoreSound()` - Road completion (length-based)
- `PlaySelectSound()` - Button clicks
- `PlayForbidSound()` - Invalid actions
- `PlaySlideSound()` - Tile dragging
- `PlayLevelUpSound()` - Goal achievement

### Static State

**Caution**: Some game state uses static variables:
- `ScoreManager.score` - Current score (static field, line 7)
- `BoardCheck.gameover` - Game over flag (static field, line 14)
- `BoardCheck.adj` - Grid state (static 2D array, line 20)
- `BoardCheck.isAnimating` - Animation lock (static bool, line 23)

These reset when `MainScene` reloads via `Awake()` methods or scene load callbacks.

---

## Task Tracking

**TASKS.md**: Contains detailed implementation plans for new features. When working on tasks:

1. Read `TASKS.md` to understand current work items
2. Mark tasks as in-progress/completed as you work
3. Update checklist items `[ ]` → `[x]` when completed
4. Add notes about implementation decisions or blockers
5. Keep the file as a living document throughout development

**Current tasks**: Two new ad features (timer-based interstitial, rewarded reroll refill) - see TASKS.md for full specifications.

---

## Common Pitfalls

1. **Scene-specific logic**: Always check scene name before features that shouldn't run in `TutorialScene`
   ```csharp
   if (SceneManager.GetActiveScene().name != "MainScene") return;
   ```

2. **Animation blocking**: Check `BoardCheck.isAnimating` before allowing tile placement to prevent state corruption

3. **Singleton initialization order**: Singletons initialize in `Awake()`. Don't access them in other `Awake()` methods - use `Start()` instead

4. **Grid coordinate confusion**:
   - `BoardCheck.adj` uses 7×7 with 1-indexed playable area
   - `boardSlot` array uses 0-indexed 25-element flat array
   - Conversion: `boardSlot[5*y + x - 6]` ↔ `adj[y,x]` where y,x ∈ [1,5]

5. **Ad testing**: Always set `AdMobConfig.useTestIds = true` during development to avoid policy violations

---

## Working with Ads

### Adding New Ad Placements

**Pattern to follow**:
```csharp
// Interstitial (non-rewarded)
AdsManager.I.TryShowInterstitial(onClosed: () => {
    // Code to run after ad closes
});

// Rewarded
AdsManager.I.TryShowRewarded(
    onReward: () => {
        // Grant reward (e.g., add reroll count)
    },
    onClosed: () => {
        // Optional cleanup
    }
);
```

**Best practices**:
- Check `AdsManager.I != null` before calling
- Never show ads in `TutorialScene`
- Use callbacks for sequential logic (e.g., reload scene after interstitial)
- Ads auto-preload after showing, but can manually preload if needed

### Creating New Singletons

If you need a new persistent manager:

```csharp
public class NewManager : MonoBehaviour
{
    public static NewManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-initialize scene-specific references
    }
}
```

---

## Dependencies

**Key Unity Packages**:
- Google Mobile Ads SDK (AdMob)
- TextMeshPro (UI text rendering)
- DOTween (animation tweening)

**External Services**:
- Google AdMob (monetization)

---

## File Organization

```
Assets/
├── Scenes/
│   ├── MainScene.unity          # Primary gameplay
│   └── TutorialScene.unity      # First-time tutorial
├── Scripts/
│   ├── AdMob/                   # Ad integration
│   │   ├── AdsManager.cs        # Singleton wrapper
│   │   ├── AdMobService.cs      # SDK implementation
│   │   ├── IAdsService.cs       # Interface
│   │   └── AdMobConfig.cs       # ScriptableObject config
│   ├── BoardCheck.cs            # Road detection & game flow
│   ├── TileGenerator.cs         # Tile spawning logic
│   ├── TileDraggable.cs         # Drag & drop mechanics
│   ├── RerollButton.cs          # Reroll feature
│   ├── ScoreManager.cs          # Scoring & persistence
│   ├── TurnCounting.cs          # Turn/goal tracking
│   ├── SoundManager.cs          # Audio playback
│   └── Tutorial/                # Tutorial-specific scripts
└── Settings/
    └── UniversalRP.asset        # Rendering pipeline
```

---

## Notes on Korean Comments

Some files contain Korean comments (encoding issues visible as mojibake). Key translations:
- `싱글톤` = Singleton
- `턴` = Turn
- `점수` = Score
- `게임오버` = Game Over

When editing these files, preserve comment locations even if characters appear garbled.
