using UnityEngine;
using UnityEditor.Animations;
public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerGameplay playerGameplay) : base(playerGameplay) { }
    private void ApplyHorizontalMovement()
    {
        Vector3 velocity = PlayerGameplay.Rigidbody.linearVelocity;
        velocity.x = PlayerGameplay.PlayerInputManager.horizontalMoveInput.x * PlayerGameplay.Stats.moveSpeed;
        PlayerGameplay.Rigidbody.linearVelocity = velocity;
        PlayerGameplay.CharacterAnimatorController.UpdateVelocityAnimation(velocity.x, PlayerGameplay.Stats.moveSpeed);
    }

    private void CancelHorizontalMovement()
    {
        PlayerGameplay.Rigidbody.linearVelocity = Vector3.zero;
        PlayerGameplay.CharacterAnimatorController.UpdateVelocityAnimation(0, PlayerGameplay.Stats.moveSpeed);
    }

    public override void FixedUpdate()
    {
        ApplyHorizontalMovement();
        if(PlayerGameplay.PlayerInputManager.attack && PlayerGameplay.IsGrounded)
        {
            PlayerGameplay.StateMachine.ChangeState(new PlayerAttackState(PlayerGameplay, AttackController.Attacks.SideTilt));
            return;
        }
        if (PlayerGameplay.PlayerInputManager.jump && PlayerGameplay.IsGrounded && PlayerGameplay.JumpController.CanJump)
        {
            PlayerGameplay.StateMachine.ChangeState(new PlayerJumpState(PlayerGameplay));
            return;
        }

        if (!PlayerGameplay.PlayerInputManager.HasMoveInput && PlayerGameplay.IsGrounded)
            PlayerGameplay.StateMachine.ChangeState(new PlayerIdleState(PlayerGameplay));
    }
    public override void Enter()
    {
        base.Enter();
        PlayerGameplay.CharacterAnimatorController.UpdateRunAnimation(true);
    }

    public override void Exit()
    {
        CancelHorizontalMovement();
        PlayerGameplay.CharacterAnimatorController.UpdateRunAnimation(false);
    }
}
