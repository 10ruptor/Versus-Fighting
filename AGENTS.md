# Versus Fighting - AI Agent Guide

## Project Overview

A 2D/3D fighting game in Unity using C# with a **state machine-based player controller** architecture. The project emphasizes clean separation of concerns between input handling, physics simulation, animation, and game logic.

**Key Tech Stack:**
- Unity (with InputSystem package)
- C# with MonoBehaviour components
- Physics-based movement (Rigidbody)
- Scriptable Objects for configuration (CharacterStatsSO)

---

## Architecture Overview

### Core Player System (State Machine Pattern)

The player controller uses a **hierarchical state machine** where each gameplay state encapsulates its own logic:

```
PlayerGameplay (Main coordinator)
  ├─ PlayerStateMachine (State management)
  │   ├─ PlayerIdleState (Zero velocity, can jump/move)
  │   ├─ PlayerMoveState (Horizontal movement)
  │   └─ PlayerJumpState (Vertical & air mechanics)
  ├─ JumpController (Physics calculations)
  ├─ VisualOrientationController (Character facing direction)
  └─ Rigidbody (Physics integration)
```

**Why this structure:** Each state handles its own transitions and physics updates. `PlayerGameplay` acts as a facade that states query for input data and physics properties.

### Input Handling Pattern

The project uses Unity's **InputSystem package** with action maps:

```csharp
// PlayerGameplay discovers actions at runtime:
moveAction = playerInput.actions.FindAction("Move", true);
jumpAction = playerInput.actions.FindAction("Jump", true);
fastFallAction = playerInput.actions.FindAction("FastFall", true);
```

**Important:** 
- Actions are resolved in `Start()`, not `Awake()`
- Input querying happens in `Update()` for consistency
- States access input through public properties on `PlayerGameplay` (e.g., `MoveInput`, `JumpRequested`)

### Configuration via Scriptable Objects

Movement and jump physics are externalized to **CharacterStatsSO**:
- `moveSpeed`: Horizontal velocity
- `jumpHeight`, `jumpAscentDuration`: Jump arc shape
- `weight`, `fastFallAccelerationMultiplier`: Gravity tuning

This allows designer-friendly tweaking without code changes.

---

## Key Code Patterns

### State Machine Transitions

States change when conditions are met:

```csharp
// In PlayerIdleState.FixedUpdate():
if (PlayerGameplay.JumpRequested && PlayerGameplay.IsGrounded && PlayerGameplay.JumpController.CanJump)
{
    PlayerGameplay.StateMachine.ChangeState(new PlayerJumpState(PlayerGameplay));
}
```

**Pattern:** Transitions occur in FixedUpdate (physics-safe). The new state's `Enter()` is called automatically.

### Input Querying vs Callbacks

The codebase **mixes two input patterns**:
1. **Polling** (preferred): `moveAction.ReadValue<Vector2>()`, `jumpAction.WasPerformedThisFrame()`
2. **Callbacks** (fallback): `OnJump(InputValue value)` method

Both update `jumpRequested` flag to ensure input isn't lost between frames.

### Physics Integration

- Velocity is read/written as `rb.linearVelocity`
- Jump physics disables gravity during ascent, re-enables during descent
- Horizontal & vertical velocity components are handled separately for clean air control

---

## Important Conventions

1. **RequireComponent Usage:** Classes that depend on other components declare them:
   ```csharp
   [RequireComponent(typeof(PlayerInput))]
   [RequireComponent(typeof(Rigidbody))]
   ```
   This ensures dependencies are enforced at the GameObject level.

2. **Frame Timing:**
   - Input read in `Update()` (frame-perfect)
   - Physics applied in `FixedUpdate()` (deterministic)
   - State transitions in `FixedUpdate()`

3. **Null Checks:** Input actions can be null; always check before calling methods:
   ```csharp
   public bool JumpPressedThisFrame => jumpAction != null && jumpAction.WasPerformedThisFrame();
   ```

4. **Tag-based Collision:** Ground detection uses "Stage" tag:
   ```csharp
   static bool IsStageCollision(Collision collision)
   {
       return collision.gameObject.CompareTag(StageTag);
   }
   ```

---

## PlayerInputController (In Development)

**Current Status:** Empty skeleton class meant to refactor input logic out of `PlayerGameplay`.

**Planned Responsibilities:**
- Cache and expose InputActions from PlayerInput component
- Handle InputSystem initialization and action map switching
- Provide input polling methods/properties (move, jump, fastFall)
- Decouple input data from physics logic

**Integration Point:** `PlayerGameplay` will query `PlayerInputController` instead of managing `PlayerInput` directly.

---

## Development Workflow

### Testing Player Behavior
1. Modify `CharacterStatsSO` asset in Inspector (no recompile needed)
2. Test jump height, ascent duration, move speed in Play mode
3. Check state transitions in Animator window

### Adding New States
1. Create class inheriting from `PlayerState`
2. Implement `Enter()`, `Exit()`, `Update()`, `FixedUpdate()`
3. Define transition logic in existing states
4. Add transition condition in `FixedUpdate()`

### Debugging
- Use `currentStateName` SerializeField to inspect active state
- Check `IsGrounded` property for grounding issues
- Monitor `JumpController.CurrentPhase` (Ascent/Descent)

---

## File Manifest

| Path | Purpose |
|------|---------|
| `PlayerGameplay.cs` | Main player coordinator & state machine host |
| `PlayerStates/` | State implementations (Idle, Move, Jump) |
| `PlayerInputController.cs` | **[WIP]** Input handling abstraction |
| `Jump/JumpController.cs` | Physics calculations for jump arc |
| `VisualRoot/VisualOrientationController.cs` | Character facing direction |
| `CharacterStatsSO.cs` | Externalized movement/jump tuning |

---

## Quick References

- **RequireComponent:** Enforces GameObject dependencies, retrieved via `GetComponent<T>()`
- **InputAction.WasPerformedThisFrame():** True only during the frame input occurred
- **linearVelocity:** Direct velocity access (replacement for deprecated `.velocity`)
- **Physics.gravity.y:** Global gravity constant (-9.81 by default)
