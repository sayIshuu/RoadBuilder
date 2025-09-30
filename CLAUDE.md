# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

RoadBuilder is a casual puzzle game built in Unity where players connect randomly generated road tile blocks to form paths from one edge of the grid to the opposite edge. Developed by a team of 4 Unity developers.

**Unity Version:** 6000.0.36f1

## Building and Running

This is a Unity project that must be opened and run through the Unity Editor:

1. Open the project directory in Unity Hub
2. Ensure Unity version 6000.0.36f1 is installed
3. Open the main scene at [Assets/Scenes/MainScene.unity](Assets/Scenes/MainScene.unity)
4. Click Play in the Unity Editor to run

For platform builds (Android, iOS, PC), use the Unity Editor's Build Settings window (`File > Build Settings`).

## Key Technologies

- **Engine:** Unity 6000.0.36f1
- **Language:** C#
- **Input:** Unity Input System (`com.unity.inputsystem`)
- **Rendering:** Universal Render Pipeline (URP)
- **UI:** Unity UI (UGUI)
- **Tweening:** DOTween
- **Ads:** Google Mobile Ads (AdMob)
- **Backend:** Firebase (App, Firestore/Storage)

## Core Architecture

### Game Board System

The game uses a 5x5 grid where tiles can be placed. The grid is represented internally as a 7x7 adjacency matrix (with borders):

- **[BoardCheck.cs](Assets/Scripts/BoardCheck.cs)**: Core game logic controller
  - Manages the game board state using `adj[7,7]` adjacency matrix
  - Uses Union-Find algorithm to detect completed paths
  - Checks win/lose conditions
  - Handles tile completion animations with DOTween
  - Each tile encodes connections as bitflags: 1=up, 2=right, 4=down, 8=left

- **[BoardSlot.cs](Assets/Scripts/BoardSlot.cs)**: Individual grid slot representation
  - Handles tile placement validation
  - Provides visual feedback on hover

### Tile Generation & Dragging

- **[TileGenerator.cs](Assets/Scripts/TileGenerator.cs)**: Spawns random tiles for player selection
  - Generates 3 tiles at a time in inventory slots
  - Prevents all 3 tiles from being identical
  - Tile probabilities: 95% basic (types 1-6), 4.5% tri-lane (types 7-10), 0.5% quad-lane (type 11)

- **[TileDraggable.cs](Assets/Scripts/TileDraggable.cs)**: Drag-and-drop mechanics
  - Implements Unity's `IBeginDragHandler`, `IDragHandler`, `IEndDragHandler`
  - Validates placement on board slots
  - Triggers board check after placement

### Scoring & Progression

- **[ScoreManager.cs](Assets/Scripts/ScoreManager.cs)**: Score tracking and persistence
  - Score formula: length³ (cube of connected path length)
  - Saves high score using `PlayerPrefs`

- **[TurnCounting.cs](Assets/Scripts/TurnCounting.cs)**: Turn-based progression system
  - Tracks current turn and goal score milestones
  - Updates goal score every 10 turns
  - Provides visual feedback at turn 9 for goal status
  - Manages game-over condition when goal not met

- **[MissionController.cs](Assets/Scripts/MissionController.cs)**: Achievement system
  - Tracks achievements based on path length

### Systems

- **[SoundManager.cs](Assets/Scripts/SoundManager.cs)**: Audio management for BGM
  - Sound assets located in [Assets/Resources/](Assets/Resources/)

- **[SfxManager.cs](Assets/Scripts/SfxManager.cs)**: Sound effects management
  - Chain completion SFX at [Assets/Resources/SFX/](Assets/Resources/SFX/)

- **[ThemeManager.cs](Assets/Scripts/ThemeSetting/ThemeManager.cs)**: Theme system
  - Light/Dark mode toggle
  - Theme state persisted via `PlayerPrefs`
  - Uses `IThemeChangeable` interface for themed UI components

- **[LifeManager.cs](Assets/Scripts/LifeManager.cs)**: Life/energy system
  - Time-based life recovery mechanism
  - Persists life state using `PlayerPrefs`

- **[AdsManager.cs](Assets/Scripts/AdMob/AdsManager.cs)**: AdMob integration
  - Manages banner, interstitial, and rewarded ads
  - Uses [AdMobConfig](Assets/Scripts/AdMob/AdMobConfig.cs) for configuration

## Important Game Logic Details

### Path Detection Algorithm

The `BoardCheck.Check()` method uses Union-Find to detect when tiles form a path connecting grid edges:
1. Merges adjacent connected tiles into sets
2. Checks if any border tile connects to another border tile via the same set
3. If connected, destroys path and awards score

### Tile Encoding

Tiles use bitmask encoding for 4-directional connections:
- `1` (0001): Up connection
- `2` (0010): Right connection
- `4` (0100): Down connection
- `8` (1000): Left connection

Example: `15` (1111) = all directions connected

### Reroll System

- **[RerollButton.cs](Assets/Scripts/RerollButton.cs)**: Tile reroll functionality
  - Allows refreshing tile selection without consuming a turn
  - Recharges 1 use per 100 turns

## Common Workflows

### Adding New Tile Types

1. Add new tile type constant to tile generation probabilities in `TileGenerator.GetRandNum()`
2. Create corresponding sprite/prefab variant
3. Update adjacency encoding logic if needed

### Modifying Scoring

Edit `ScoreManager.AddScore()` and the score calculation in `BoardCheck.GetScore()`

### Adding Sound Effects

1. Place audio file in [Assets/Resources/](Assets/Resources/) or [Assets/Resources/SFX/](Assets/Resources/SFX/)
2. Add reference in `SoundManager` or `SfxManager`
3. Call appropriate play method from game scripts

## Project Structure

```
Assets/
├── Scenes/
│   └── MainScene.unity          # Main game scene
├── Scripts/
│   ├── BoardCheck.cs            # Core game logic
│   ├── TileGenerator.cs         # Tile spawning
│   ├── TileDraggable.cs         # Drag-and-drop
│   ├── ScoreManager.cs          # Scoring system
│   ├── TurnCounting.cs          # Turn management
│   ├── MissionController.cs     # Achievements
│   ├── LifeManager.cs           # Life system
│   ├── ThemeSetting/            # Theme system
│   └── AdMob/                   # Ad integration
├── Prefabs/                     # Reusable game objects
└── Resources/                   # Audio assets
```

## Git Workflow

- Main branch: `main`
- Current feature branch: `Firebase_KDY`
- Recent commits focus on Firebase integration, tile feedback SFX, and code refinements