using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    private int dashDuration;
    private float dashSpeed;
    private float dashFrameCounter;
    public PlayerDashState(PlayerGameplay playerGameplay, int dashDuration, float dashSpeed) : base(playerGameplay)
    {
        this.dashDuration = dashDuration;
        this.dashSpeed = dashSpeed;
    }

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
                playerGameplay.StateMachine.ChangeState(new PlayerMoveState(playerGameplay));
            }
            else if(playerGameplay.IsGrounded && !playerGameplay.PlayerInputManager.HasWalkInput)
            {
                playerGameplay.StateMachine.ChangeState(new PlayerIdleState(playerGameplay));
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
