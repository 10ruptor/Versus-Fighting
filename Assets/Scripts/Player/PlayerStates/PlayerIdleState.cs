using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerGameplay playerGameplay) : base(playerGameplay) { }

    public override void FixedUpdate()
    {
        Vector3 velocity = PlayerGameplay.Rigidbody.linearVelocity;
        velocity.x = 0f;
        PlayerGameplay.Rigidbody.linearVelocity = velocity;

        if (PlayerGameplay.PlayerInputManager.jump && PlayerGameplay.IsGrounded && PlayerGameplay.JumpController.CanJump)
        {
            Debug.Log("Jump Requested");
            PlayerGameplay.StateMachine.ChangeState(new PlayerJumpState(PlayerGameplay));
            return;
        }

        if (PlayerGameplay.PlayerInputManager.HasMoveInput && PlayerGameplay.IsGrounded)
            PlayerGameplay.StateMachine.ChangeState(new PlayerMoveState(PlayerGameplay));
        
        if(PlayerGameplay.PlayerInputManager.attack)
            PlayerGameplay.StateMachine.ChangeState(new PlayerAttackState(PlayerGameplay));
            
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        
    }
}
