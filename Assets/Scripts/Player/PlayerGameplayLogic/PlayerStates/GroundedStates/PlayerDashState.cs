using UnityEngine;

public class PlayerDashState : PlayerState
{
    private int dashActiveFrames => playerGameplay.Character.CharacterStatData.dashDurationFrames;
    private int dashAccelFrames => playerGameplay.Character.CharacterStatData.dashAccelerationFrames;
    private int dashDecelFrames => playerGameplay.Character.CharacterStatData.dashDecelerationFrames;
    private float dashSpeed => playerGameplay.Character.CharacterStatData.dashSpeed;
    private float dashFrameCounter;
    private float dashInputValue;
    private float dashStartSpeed;
    private int dashDirection;
    private bool DashIsOver => dashFrameCounter >= dashActiveFrames + dashDecelFrames;
    public PlayerDashState(PlayerGameplay playerGameplay) : base(playerGameplay) { }
    protected override string StateAnimationName => "Dash";

    public override void RegisterTransition()
    {
        AddTransition(() => playerGameplay.PlayerInputController.HasDashInput && Mathf.Sign(dashInputValue) != Mathf.Sign(playerGameplay.PlayerInputController.HorizontalMoveInputValue), playerGameplay.PlayerDashState);
        AddTransition(() => DashIsOver && playerGameplay.PlayerInputController.HasWalkInput, playerGameplay.PlayerMoveState);
        AddTransition(() => DashIsOver && playerGameplay.IsGrounded && !playerGameplay.PlayerInputController.HasWalkInput, playerGameplay.PlayerIdleState);
    }

    public override void Enter()
    {
        base.Enter();
        dashFrameCounter = 0;
        dashInputValue = playerGameplay.PlayerInputController.HorizontalMoveInputValue;
        dashDirection = dashInputValue >= 0f ? 1 : -1;
        dashStartSpeed = playerGameplay.Rigidbody.linearVelocity.x;
        playerGameplay.Character.VFXManager.PlayDashParticle();
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
        Vector3 velocity = playerGameplay.Rigidbody.linearVelocity;
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

            float slideTargetSpeed = playerGameplay.PlayerInputController.HasWalkInput
                ? playerGameplay.PlayerInputController.HorizontalMoveInputValue * playerGameplay.Character.CharacterStatData.moveSpeed
                : 0f;

            velocity.x = Mathf.Lerp(targetSpeed, slideTargetSpeed, smoothT);
        }

        playerGameplay.Rigidbody.linearVelocity = velocity;
    }
}
