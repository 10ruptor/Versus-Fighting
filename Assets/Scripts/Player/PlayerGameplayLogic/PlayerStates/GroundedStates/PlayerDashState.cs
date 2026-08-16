using UnityEngine;

public class PlayerDashState : PlayerState
{
    private int dashDuration => playerGameplay.Character.CharacterStatData.dashDurationFrames;
    private float dashSpeed => playerGameplay.Character.CharacterStatData.dashSpeed;
    private float dashFrameCounter;
    private float dashInputValue;
    private bool DashIsOver => dashFrameCounter >= dashDuration;
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
        playerGameplay.Character.VFXManager.PlayDashParticle();
        ApplyDashMovement();
    }

    public override void FixedUpdate()
    {
        dashFrameCounter += 1;

        CheckTransitions();
        
    }

    private void ApplyDashMovement()
    {
        Vector3 velocity = playerGameplay.Rigidbody.linearVelocity;
        if(playerGameplay.PlayerInputController.HorizontalMoveInputValue > 0)
            velocity.x = dashSpeed;
        else if(playerGameplay.PlayerInputController.HorizontalMoveInputValue < 0)
            velocity.x = -dashSpeed;
        playerGameplay.Rigidbody.linearVelocity = velocity;
    }

}
