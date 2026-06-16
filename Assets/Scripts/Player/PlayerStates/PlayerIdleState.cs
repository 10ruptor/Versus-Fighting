using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerGameplay playerGameplay) : base(playerGameplay) { }
    private bool playerHasWalkInput => playerGameplay.PlayerInputManager.HasWalkInput;
    private bool playerHasDashInput => playerGameplay.PlayerInputManager.HasDashInput;
    private bool playerHasDownMoveInput => playerGameplay.PlayerInputManager.HasDownMoveInput;
    private bool playerHasUpMoveInput => playerGameplay.PlayerInputManager.HasUpMoveInput;

    public override void FixedUpdate()
    {
        Vector3 velocity = playerGameplay.Rigidbody.linearVelocity;
        velocity.x = 0f;
        playerGameplay.Rigidbody.linearVelocity = velocity;
        if (playerHasDownMoveInput && playerGameplay.IsGrounded)
        {
            playerGameplay.StateMachine.ChangeState(new PlayerCrouchState(playerGameplay));
            return;
        }
        
        if (playerGameplay.PlayerInputManager.jump && playerGameplay.IsGrounded && playerGameplay.JumpController.CanJump)
        {
            playerGameplay.StateMachine.ChangeState(new PlayerJumpState(playerGameplay));
            return;
        }
        
        if (playerGameplay.PlayerInputManager.attack && playerGameplay.IsGrounded)
        {
            if (playerHasUpMoveInput)
            {
                playerGameplay.StateMachine.ChangeState(new PlayerAttackState(playerGameplay, AttackController.Attacks.UpTilt));
            }
            else 
            {
                playerGameplay.StateMachine.ChangeState(new PlayerAttackState(playerGameplay, AttackController.Attacks.NeutralTilt));
            }
            return;
        }
        
        if (playerHasDashInput && playerGameplay.IsGrounded)
        {
            playerGameplay.StateMachine.ChangeState(new PlayerDashState(playerGameplay,playerGameplay.Stats.dashDurationFrames, playerGameplay.Stats.dashSpeed));
            return;
        }
        
        if (playerHasWalkInput && playerGameplay.IsGrounded)
        {
            playerGameplay.StateMachine.ChangeState(new PlayerMoveState(playerGameplay));
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
