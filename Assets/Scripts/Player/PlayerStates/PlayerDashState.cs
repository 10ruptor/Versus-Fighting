using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    private int dashDuration => playerGameplay.Stats.dashDurationFrames;
    private float dashSpeed => playerGameplay.Stats.dashSpeed;
    private float dashFrameCounter;
    public PlayerDashState(PlayerGameplay playerGameplay) : base(playerGameplay) { }

    public override void Enter()
    {
        base.Enter();
        dashFrameCounter = 0;
        playerGameplay.CharacterAnimatorController.UpdateDashAnimation();
        ApplyDashMovement();
    }

    public override void FixedUpdate()
    {
        dashFrameCounter += 1;
        if (dashFrameCounter >= dashDuration)
        {
            if(playerGameplay.IsGrounded && playerGameplay.PlayerInputManager.HasWalkInput)
            {
                playerGameplay.StateMachine.ChangeState(playerGameplay.playerMoveState);
            }
            else if(playerGameplay.IsGrounded && !playerGameplay.PlayerInputManager.HasWalkInput)
            {
                playerGameplay.StateMachine.ChangeState(playerGameplay.playerIdleState);
            }
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
    public override void Exit()
    {
        base.Exit();
        //other logic
    }
}
