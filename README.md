# Versus Fighting

A 2D/3D versus fighting game built in Unity, inspired by Smash-style platform fighters.

## Tech Stack

- Unity with the Input System package
- C# with a state machine-based player controller
- Physics-based movement via `Rigidbody`
- Scriptable Objects (`CharacterStatsSO`) for designer-tunable character stats

## Architecture

The player controller uses a hierarchical state machine (`PlayerIdleState`, `PlayerMoveState`, `PlayerJumpState`, ...) coordinated by `PlayerGameplay`, with physics handled in `FixedUpdate` and input polled in `Update`. See [AGENTS.md](AGENTS.md) for a detailed breakdown of the code patterns and conventions used throughout the project.

## Getting Started

1. Open the project in Unity (see `ProjectSettings/ProjectVersion.txt` for the required editor version).
2. Open `Assets/Scenes/FightScene.unity`.
3. Enter Play mode to test character movement and combat.
