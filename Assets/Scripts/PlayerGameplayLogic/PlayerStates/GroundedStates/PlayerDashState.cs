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
        AddTransition(() => playerGameplay.PlayerInputManager.HasDashInput && Mathf.Sign(dashInputValue) != Mathf.Sign(playerGameplay.PlayerInputManager.HorizontalMoveInputValue), playerGameplay.playerDashState);
        AddTransition(() => DashIsOver && playerGameplay.PlayerInputManager.HasWalkInput, playerGameplay.playerMoveState);
        AddTransition(() => DashIsOver && playerGameplay.IsGrounded && !playerGameplay.PlayerInputManager.HasWalkInput, playerGameplay.playerIdleState);
    }

    public override void Enter()
    {
        base.Enter();
        dashFrameCounter = 0;
        dashInputValue = playerGameplay.PlayerInputManager.HorizontalMoveInputValue;
        playerGameplay.VFXManager.PlayDashParticle();
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
        if(playerGameplay.PlayerInputManager.HorizontalMoveInputValue > 0)
            velocity.x = dashSpeed;
        else if(playerGameplay.PlayerInputManager.HorizontalMoveInputValue < 0)
            velocity.x = -dashSpeed;
        playerGameplay.Rigidbody.linearVelocity = velocity;
    }

}
