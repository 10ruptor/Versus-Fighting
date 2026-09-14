using UnityEngine;

public class PlayerDashState : PlayerState
{
    private int dashActiveFrames => stateMachine.PlayerGameplay.Character.CharacterStatData.dashDurationFrames;
    private int dashAccelFrames => stateMachine.PlayerGameplay.Character.CharacterStatData.dashAccelerationFrames;
    private int dashDecelFrames => stateMachine.PlayerGameplay.Character.CharacterStatData.dashDecelerationFrames;
    private float dashSpeed => stateMachine.PlayerGameplay.Character.CharacterStatData.dashSpeed;
    private float dashFrameCounter;
    private float dashInputValue;
    private float dashStartSpeed;
    private int dashDirection;
    private bool DashIsOver => dashFrameCounter >= dashActiveFrames + dashDecelFrames;
    protected override string StateAnimationName => "Dash";

    public override void RegisterTransition()
    {
        AddTransition(() => stateMachine.PlayerGameplay.PlayerInputController.HasDashInput && Mathf.Sign(dashInputValue) != Mathf.Sign(stateMachine.PlayerGameplay.PlayerInputController.HorizontalMoveInputValue), stateMachine.stateLibrary[StateType.Dash]);
        AddTransition(() => DashIsOver && stateMachine.PlayerGameplay.PlayerInputController.HasWalkInput,stateMachine.stateLibrary[StateType.Move]);
        AddTransition(() => DashIsOver && stateMachine.PlayerGameplay.IsGrounded && !stateMachine.PlayerGameplay.PlayerInputController.HasWalkInput, stateMachine.stateLibrary[StateType.Idle]);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        dashFrameCounter = 0;
        dashInputValue = stateMachine.PlayerGameplay.PlayerInputController.HorizontalMoveInputValue;
        dashDirection = dashInputValue >= 0f ? 1 : -1;
        dashStartSpeed = stateMachine.PlayerGameplay.Rigidbody.linearVelocity.x;
        stateMachine.PlayerGameplay.Character.VFXManager.PlayDashParticle();
        ApplyDashMovement();
    }

    public override void FixedUpdate()
    {
        dashFrameCounter += 1;

        ApplyDashMovement();
        CheckTransitions();
    }

    private void ApplyDashMovement()
    {
        Vector3 velocity = stateMachine.PlayerGameplay.Rigidbody.linearVelocity;
        float targetSpeed = dashDirection * dashSpeed;

        if (dashFrameCounter < dashAccelFrames)
        {
            float t = dashFrameCounter / dashAccelFrames;
            float easeOut = 1f - (1f - t) * (1f - t);
            velocity.x = Mathf.Lerp(dashStartSpeed, targetSpeed, easeOut);
        }
        else if (dashFrameCounter < dashActiveFrames)
        {
            velocity.x = targetSpeed;
        }
        else
        {
            float slideFrame = dashFrameCounter - dashActiveFrames;
            float t = dashDecelFrames > 0 ? Mathf.Clamp01(slideFrame / dashDecelFrames) : 1f;
            float smoothT = t * t * (3f - 2f * t);

            float slideTargetSpeed = stateMachine.PlayerGameplay.PlayerInputController.HasWalkInput
                ? stateMachine.PlayerGameplay.PlayerInputController.HorizontalMoveInputValue * stateMachine.PlayerGameplay.Character.CharacterStatData.moveSpeed
                : 0f;

            velocity.x = Mathf.Lerp(targetSpeed, slideTargetSpeed, smoothT);
        }

        stateMachine.PlayerGameplay.Rigidbody.linearVelocity = velocity;
    }
}
