using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    private int dashDuration => playerGameplay.Stats.dashDurationFrames;
    private float dashSpeed => playerGameplay.Stats.dashSpeed;
    private float dashFrameCounter;
    public PlayerDashState(PlayerGameplay playerGameplay) : base(playerGameplay) { }
    protected override string StateAnimationName => "Dash";

    public override void RegisterTransition()
    {
        AddTransition(() => playerGameplay.IsGrounded && playerGameplay.PlayerInputManager.HasWalkInput, playerGameplay.playerMoveState);
        AddTransition(() => playerGameplay.IsGrounded && !playerGameplay.PlayerInputManager.HasWalkInput, playerGameplay.playerIdleState);
    }

    public override void Enter()
    {
        base.Enter();
        dashFrameCounter = 0;
        ApplyDashMovement();
    }

    public override void FixedUpdate()
    {
        dashFrameCounter += 1;
        if (dashFrameCounter >= dashDuration)
        {
            CheckTransitions();
        }
    }

    private void ApplyDashMovement()
    {
        Vector3 velocity = playerGameplay.Rigidbody.linearVelocity;
        if(playerGameplay.PlayerInputManager.horizontalMoveInput > 0)
            velocity.x = dashSpeed;
        else if(playerGameplay.PlayerInputManager.horizontalMoveInput < 0)
            velocity.x = -dashSpeed;
        playerGameplay.Rigidbody.linearVelocity = velocity;
    }

}
