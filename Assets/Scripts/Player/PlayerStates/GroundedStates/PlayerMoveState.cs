using UnityEngine;
using UnityEditor.Animations;
public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(PlayerGameplay playerGameplay) : base(playerGameplay) {  }
    protected override string StateAnimationName => "Move"; 

    private void ApplyHorizontalMovement()
    {
        Vector3 velocity = playerGameplay.Rigidbody.linearVelocity;
        velocity.x = playerGameplay.PlayerInputManager.HorizontalMoveInputValue * playerGameplay.Stats.moveSpeed;
        playerGameplay.Rigidbody.linearVelocity = velocity;
        playerGameplay.CharacterAnimatorController.UpdateVelocityAnimation(playerGameplay.PlayerInputManager.HorizontalMoveInputValue);
    }

    public override void RegisterTransition()
    {
        base.RegisterTransition();
        AddTransition(() => playerGameplay.PlayerInputManager.Attack && playerGameplay.IsGrounded, playerGameplay.playerAttackState);
        AddTransition(() => !playerGameplay.PlayerInputManager.HasWalkInput && playerGameplay.IsGrounded, playerGameplay.playerIdleState);
    }
    
    private void CancelHorizontalMovement()
    {
        playerGameplay.Rigidbody.linearVelocity = Vector3.zero;
        playerGameplay.CharacterAnimatorController.UpdateVelocityAnimation(0);
    }
    
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        CheckTransitions();
        ApplyHorizontalMovement();
    }

    public override void Exit()
    {
        base.Exit();
        CancelHorizontalMovement();
    }
}
