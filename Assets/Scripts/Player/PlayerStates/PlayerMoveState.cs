using UnityEngine;
using UnityEditor.Animations;
public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerGameplay playerGameplay) : base(playerGameplay) {  }
    protected override string StateAnimationName => "Move"; 

    private void ApplyHorizontalMovement()
    {
        Vector3 velocity = playerGameplay.Rigidbody.linearVelocity;
        velocity.x = playerGameplay.PlayerInputManager.horizontalMoveInput * playerGameplay.Stats.moveSpeed;
        playerGameplay.Rigidbody.linearVelocity = velocity;
        playerGameplay.CharacterAnimatorController.UpdateVelocityAnimation(playerGameplay.PlayerInputManager.horizontalMoveInput);
    }

    public override void RegisterTransition()
    {
        AddTransition(() => playerGameplay.PlayerInputManager.attack && playerGameplay.IsGrounded, playerGameplay.playerAttackState);
        AddTransition(() => playerGameplay.PlayerInputManager.jump && playerGameplay.IsGrounded && playerGameplay.JumpController.CanJump, playerGameplay.playerJumpState);
        AddTransition(() => !playerGameplay.PlayerInputManager.HasWalkInput && playerGameplay.IsGrounded, playerGameplay.playerIdleState);
    }
    
    private void CancelHorizontalMovement()
    {
        playerGameplay.Rigidbody.linearVelocity = Vector3.zero;
        playerGameplay.CharacterAnimatorController.UpdateVelocityAnimation(0);
    }

    public override void FixedUpdate()
    {
        ApplyHorizontalMovement();
        CheckTransitions();
    }

    public override void Enter()
    {
        base.Enter();
        
    }

    public override void Exit()
    {
        base.Exit();
        CancelHorizontalMovement();
    }
}
