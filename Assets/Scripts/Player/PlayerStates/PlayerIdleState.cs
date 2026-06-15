using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerGameplay playerGameplay) : base(playerGameplay) { }
    private bool playerHasHorizontalMoveInput => PlayerGameplay.PlayerInputManager.HasHorizontalMoveInput;
    private bool playerHasDownMoveInput => PlayerGameplay.PlayerInputManager.HasDownMoveInput;
    private bool playerHasUpMoveInput => PlayerGameplay.PlayerInputManager.HasUpMoveInput;

    public override void FixedUpdate()
    {
        Vector3 velocity = PlayerGameplay.Rigidbody.linearVelocity;
        velocity.x = 0f;
        PlayerGameplay.Rigidbody.linearVelocity = velocity;
        if (playerHasDownMoveInput && PlayerGameplay.IsGrounded)
        {
            PlayerGameplay.StateMachine.ChangeState(new PlayerCrouchState(PlayerGameplay));
            return;
        }
        if (PlayerGameplay.PlayerInputManager.jump && PlayerGameplay.IsGrounded && PlayerGameplay.JumpController.CanJump)
        {
            PlayerGameplay.StateMachine.ChangeState(new PlayerJumpState(PlayerGameplay));
            return;
        }
        
        if (PlayerGameplay.PlayerInputManager.attack && PlayerGameplay.IsGrounded)
        {
            if (playerHasUpMoveInput)
            {
                PlayerGameplay.StateMachine.ChangeState(new PlayerAttackState(PlayerGameplay, AttackController.Attacks.UpTilt));
            }
            else 
            {
                PlayerGameplay.StateMachine.ChangeState(new PlayerAttackState(PlayerGameplay, AttackController.Attacks.NeutralTilt));
            }
            
            return;
        }
        
        if (playerHasHorizontalMoveInput && PlayerGameplay.IsGrounded)
        {
            PlayerGameplay.StateMachine.ChangeState(new PlayerMoveState(PlayerGameplay));
        }
    }

    public override void Enter()
    {
        base.Enter();
        //other logic
    }

    public override void Exit()
    {
        base.Exit();
        //other logic
    }
}
