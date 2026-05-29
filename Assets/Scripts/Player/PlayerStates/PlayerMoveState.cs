using UnityEngine;
using UnityEditor.Animations;
public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerGameplay playerGameplay) : base(playerGameplay) { }

    public override void FixedUpdate()
    {
        PlayerGameplay.ApplyHorizontalMovement();

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
        PlayerGameplay.CharacterAnimatorController.UpdateRunAnimation(false);
    }
}
