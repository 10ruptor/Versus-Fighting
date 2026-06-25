using UnityEngine;
using UnityEditor.Animations;
public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerGameplay playerGameplay) : base(playerGameplay) { }
    private void ApplyHorizontalMovement()
    {
        Vector3 velocity = playerGameplay.Rigidbody.linearVelocity;
        velocity.x = playerGameplay.PlayerInputManager.horizontalMoveInput * playerGameplay.Stats.moveSpeed;
        playerGameplay.Rigidbody.linearVelocity = velocity;
        playerGameplay.CharacterAnimatorController.UpdateVelocityAnimation(playerGameplay.PlayerInputManager.horizontalMoveInput);
    }

    private void CancelHorizontalMovement()
    {
        playerGameplay.Rigidbody.linearVelocity = Vector3.zero;
        playerGameplay.CharacterAnimatorController.UpdateVelocityAnimation(0);
    }

    public override void FixedUpdate()
    {
        ApplyHorizontalMovement();
        if(playerGameplay.PlayerInputManager.attack && playerGameplay.IsGrounded)
        {
            if (playerGameplay.PlayerInputManager.attack)
            {
                playerGameplay.StateMachine.ChangeState(new PlayerAttackState(playerGameplay));
                return;
            }
        }
        if (playerGameplay.PlayerInputManager.jump && playerGameplay.IsGrounded && playerGameplay.JumpController.CanJump)
        {
            playerGameplay.StateMachine.ChangeState(new PlayerJumpState(playerGameplay));
            return;
        }
        if (!playerGameplay.PlayerInputManager.HasWalkInput && playerGameplay.IsGrounded)
            playerGameplay.StateMachine.ChangeState(new PlayerIdleState(playerGameplay));
    }
    public override void Enter()
    {
        base.Enter();
        playerGameplay.CharacterAnimatorController.UpdateRunAnimation(true);
    }

    public override void Exit()
    {
        CancelHorizontalMovement();
        playerGameplay.CharacterAnimatorController.UpdateRunAnimation(false);
    }
}
