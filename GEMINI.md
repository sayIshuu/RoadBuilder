# GEMINI.md

## Project Overview

This project is a casual puzzle game named "RoadBuilder," developed using Unity. The objective of the game is to connect paths from one end of the board to the other using randomly generated tile pieces. The game is developed by a team of four Unity developers.

**Core Gameplay Mechanics:**

*   Players are presented with three random tile options.
*   Players drag and drop tiles onto a grid-based board.
*   Connecting a path from one end of the board to the other clears the connected tiles and awards points.
*   The game ends if the board is full or if the player fails to meet the score goal within a certain number of turns.

**Key Technologies:**

*   **Engine:** Unity 6000.0.36f1
*   **Programming Language:** C#
*   **Key Unity Packages:**
    *   `com.unity.inputsystem`: For handling player input.
    *   `com.unity.render-pipelines.universal`: For graphics.
    *   `com.unity.ugui`: For the user interface.

## Building and Running

As a Unity project, "RoadBuilder" is built and run through the Unity Editor.

1.  **Open the project:** Open the `D:\unity\RoadBuilder` directory in the Unity Hub.
2.  **Select the correct Unity version:** Ensure that Unity version `6000.0.36f1` is installed and selected.
3.  **Open the main scene:** In the Project window, navigate to `Assets/Scenes` and open the `MainScene.unity` file.
4.  **Run the game:** Click the "Play" button in the Unity Editor to start the game.

**Build Commands:**

Building the project for different platforms (e.g., Android, iOS, PC) is done through the "Build Settings" window in the Unity Editor (`File > Build Settings...`).

## Development Conventions

*   **Code Style:** The C# code generally follows standard Microsoft C# conventions.
*   **Game Logic:** The core game logic is encapsulated in scripts located in the `Assets/Scripts` directory.
    *   `BoardCheck.cs`: Manages the game board, checks for completed paths, and handles game-over conditions.
    *   `TileGenerator.cs`: Generates the random tiles for the player.
    *   `ScoreManager.cs`: Manages the player's score and high score.
    *   `MissionController.cs`: Manages in-game missions and achievements.
*   **Input:** The game uses Unity's new Input System. Input actions are defined in `Assets/InputSystem_Actions.inputactions`.
*   **UI:** The user interface is built using Unity's UGUI system. Prefabs for UI elements are located in `Assets/Prefabs`.
*   **Art and Sound Assets:** Game assets such as sounds and prefabs are located in the `Assets/Resources` and `Assets/Prefabs` directories, respectively.
