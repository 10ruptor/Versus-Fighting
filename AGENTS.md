# Versus Fighting - AI Agents Guide

## Project Overview

**Versus Fighting** is a 2D/3D fighting game prototype built with **Unity 6 (6000.4.3f1)** using **C# 9.0**. The project focuses on character movement mechanics with a state-machine-based player controller and scriptable object configuration system.

- **Target Platform**: Windows (Standalone)
- **Input System**: Unity's new InputSystem (not Legacy Input)
- **Physics**: 3D Rigidbody with gravity manipulation
- **Architecture**: State Machine pattern for player gameplay

## Critical Architecture Patterns

### 1. State Machine-Based Player Controller

**Core Flow**: `PlayerGameplay` → `PlayerStateMachine` → Active `PlayerState`

Key classes:
- **`PlayerGameplay.cs`** (160 lines): Master controller managing input, physics, and state transitions. Maintains all player-facing properties (IsGrounded, JumpRequested, etc.)
- **`PlayerStateMachine.cs`**: Minimal state manager. Calls `Enter()` / `Exit()` hooks on state changes
- **`PlayerState.cs`** (abstract): Base class with virtual `Enter()`, `Exit()`, `Update()`, `FixedUpdate()`

**States in hierarchy** (`Assets/Scripts/Player/PlayerStates/`):
- `PlayerIdleState`: Zero horizontal velocity; transitions to Move/Jump
- `PlayerMoveState`: Applies horizontal movement; plays "Run" animator bool
- `PlayerJumpState`: Manages vertical physics via `JumpController`; handles double-jump logic

**Why this matters**: All gameplay logic flows through state methods. Adding features requires extending states, not modifying PlayerGameplay directly.

### 2. Scriptable Object Configuration (CharacterStatsSO)

**File**: `Assets/Scripts/Scriptable Objects/CharacterStatsSO.cs`

```csharp
[CreateAssetMenu(fileName = "CharacterStats", menuName = "Versus Fighting/Character Stats")]
```

Stores all character tuning in EditorGUI-friendly fields:
- Movement: `moveSpeed`
- Jump: `jumpHeight`, `jumpAscentDuration`, `maxAddJumpCount`
- Descent: `weight` (gravity multiplier), `fastFallAccelerationMultiplier`

**Instances**: `Assets/Scripts/Scriptable Objects/` contains `DefaultCharacterStats.asset` and `ExperimentalCharacterStats 1.asset`

**Pattern**: All numeric constants should go here. Never hardcode values in scripts.

### 3. Jump Physics System

**File**: `Assets/Scripts/Player/Jump/JumpController.cs` (86 lines)

Two-phase approach:
- **Ascent**: Disables gravity, applies eased velocity using `easeOut` curve (reaches 0 at peak)
- **Descent**: Re-enables gravity, multiplied by `weight` stat, with optional `fastFallAccelerationMultiplier`

Key methods:
- `Begin()`: Starts jump, records start height, disables gravity
- `ApplyVerticalPhysics(isFastFallHeld)`: Per-frame velocity calculation
- `ConsumeJump()` / `ResetJumpCount()`: Track multi-jump availability

**Important**: Jump logic is **not** in PlayerJumpState—it's delegated to JumpController for reusability.

### 4. Input Handling (PlayerInputController + Unity InputSystem)

**Architecture**: Input management is **decoupled** from gameplay logic:
- **`PlayerInputController.cs`**: Handles ALL input logic (caching actions, reading values, managing frame-based flags)
- **`PlayerGameplay.cs`**: Delegates to `PlayerInputController` via public properties

**Setup**:
- `PlayerInput` + `PlayerInputController` components required on Player GameObject
- Action Map: "Player" containing actions "Move", "Jump", "FastFall"
- `PlayerInputController` initializes and caches actions in `Start()`

**Key Properties** exposed by `PlayerInputController`:
- `MoveInput` (Vector2): Current move direction
- `HasMoveInput` (bool): True if horizontal input exceeds 0.01f threshold
- `JumpRequested` (bool): True if jump was requested this frame
- `JumpPressedThisFrame` (bool): True if jump action performed this frame
- `IsFastFallHeld` (bool): True while fast-fall is held

**Pattern**: 
- Input read in `Update()`, jump request flag cleared in `FixedUpdate()`
- `PlayerGameplay` accesses input via properties, not direct access to InputActions
- Null-checks on `inputController` prevent null reference exceptions

**Threshold**: Horizontal move input threshold hardcoded at `0.01f` in `PlayerInputController`

### 5. Visual Feedback Systems

**Orientation** (`VisualOrientationController.cs`):
- Updates character visual rotation based on velocity direction
- Called every frame in `PlayerGameplay.OrientationCheck()`
- Left: `Quaternion.LookRotation(Vector3.back)` | Right: `Quaternion.LookRotation(Vector3.forward)`

**Animation** (`PlayerMoveState.cs`):
- Sets animator bool "Run" on enter/exit
- `JumpState` does not set jump animation parameter yet (AnimatorController.cs is stub)

**Grounding Detection**:
- Collision callbacks (OnCollisionEnter/Stay/Exit) track stage contact count
- Stage identified by "Stage" tag
- Flag: `IsGrounded` only true when `stageContactCount > 0`

## Development Workflows

### Adding a New Player State

1. Create class in `Assets/Scripts/Player/PlayerStates/`
2. Inherit from `PlayerState`
3. Override needed lifecycle methods (typically `FixedUpdate` for logic)
4. Call `PlayerGameplay.StateMachine.ChangeState(new YourNewState(this))`

Example: `PlayerAirState` for landing recovery would override `FixedUpdate` to check landing conditions.

### Modifying Jump Feel

Adjust in `CharacterStatsSO` asset file (Inspector), never in code:
- `jumpHeight`: World units reached during ascent
- `jumpAscentDuration`: Time (seconds) to reach peak with velocity reaching 0
- `maxAddJumpCount`: Available additional jumps (1 = double-jump)
- `weight`: Gravity multiplier during descent (> 1 = heavier)

### Debugging Player State

`PlayerGameplay` exposes `currentStateName` (SerializeField) in Inspector—shows active state name in real-time.

## Project Structure

```
Assets/Scripts/
├── Player/
│   ├── PlayerGameplay.cs           # Master controller (gameplay + physics)
│   ├── PlayerInputController.cs    # Input handling (NEW - decoupled from gameplay)
│   ├── PlayerStates/
│   │   ├── PlayerState.cs          # Abstract base
│   │   ├── PlayerStateMachine.cs
│   │   ├── PlayerIdleState.cs
│   │   ├── PlayerMoveState.cs
│   │   └── PlayerJumpState.cs
│   ├── Jump/
│   │   └── JumpController.cs       # Physics system
│   └── VisualRoot/
│       ├── VisualOrientationController.cs
│       └── AnimatorController.cs   # (currently stub)
└── Scriptable Objects/
    ├── CharacterStatsSO.cs
    ├── DefaultCharacterStats.asset
    └── ExperimentalCharacterStats 1.asset
```

**Scenes**: Single playable scene at `Assets/Scenes/SampleScene.unity`

## Key Dependencies & Modules

- **UnityEngine**: Core
- **UnityEngine.InputSystem**: Input handling (not Legacy)
- **UnityEditor.Animations**: Animator references
- **Physics 3D**: Rigidbody, Collider-based grounding

## Common Pitfalls

1. **Modifying jump via code**: Edit CharacterStatsSO asset instead—instances are pre-configured
2. **Forgetting Stage tag**: Colliders must have "Stage" tag or grounding fails
3. **State transitions mid-state**: Call `StateMachine.ChangeState()` only in FixedUpdate/Update, not in Enter/Exit
4. **Input persistence**: Input actions are consumed/cleared per frame—re-read each Update
5. **Animator parameter typing**: Expect bool parameters ("Run"); int/float will require AnimatorController updates

## Testing Checklist

- [ ] Idle state stops all horizontal movement
- [ ] Move input (> 0.01 threshold) triggers MoveState and "Run" animation
- [ ] Jump request (pressed this frame) initiates JumpState and disables gravity
- [ ] Double-jump available if `maxAddJumpCount > 1` and not grounded
- [ ] Fast-fall multiplies descent acceleration only during descent phase
- [ ] Orientation flips character model based on velocity direction
- [ ] Grounding resets jump count (contact with "Stage" tag)



