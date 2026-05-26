# Versus Fighting - 2D/3D Fighting Game Prototype

A Unity 6 fighting game prototype focusing on character movement mechanics with state-machine-based player control.

## Quick Start

1. Open the project in **Unity 6 (6000.4.3f1)** or later
2. Load `Assets/Scenes/SampleScene.unity`
3. Press Play to test movement, jumping, and fast-fall mechanics

## Project Architecture

See `AGENTS.md` for detailed AI coding guidance, or review these key systems:

### Core Systems

- **State Machine**: `PlayerGameplay` → `PlayerStateMachine` → `PlayerState` subclasses
- **Input Handling**: Decoupled in `PlayerInputController` (separate from gameplay logic)
- **Jump Physics**: Two-phase system (Ascent with easing, Descent with gravity multiplier)
- **Configuration**: All tuning values in `CharacterStatsSO` assets (Inspector-editable)

### Key Workflows

**Play Testing**: Ensure "Stage" tag on ground colliders; verify grounding in `PlayerGameplay` Inspector

**Jump Tuning**: Edit `DefaultCharacterStats.asset` or `ExperimentalCharacterStats 1.asset`:
- `jumpHeight`: Peak altitude (world units)
- `jumpAscentDuration`: Time to reach peak
- `maxAddJumpCount`: 1 = double-jump, 2 = triple-jump, etc.
- `weight`: Gravity multiplier (> 1 = faster fall)

**Debugging**: `PlayerGameplay.currentStateName` shows active state in Inspector during play

## Input System Setup

- **Action Map**: "Player" with actions "Move", "Jump", "FastFall"
- **Required Components**: `Rigidbody`, `PlayerInput`, `PlayerInputController`
- **Input Threshold**: 0.01f for move input to avoid noise

See `AGENTS.md` → "Input Handling" section for architecture details.

## Development Notes

- **Do not hardcode values**: Use `CharacterStatsSO` for all numeric tuning
- **State transitions**: Only in `Update()` or `FixedUpdate()`, not in `Enter()/Exit()`
- **Ground detection**: Relies on "Stage" tag and collision callbacks
- **Jump system**: Decoupled in `JumpController` for reusability

For more detailed guidance, see `AGENTS.md`.

