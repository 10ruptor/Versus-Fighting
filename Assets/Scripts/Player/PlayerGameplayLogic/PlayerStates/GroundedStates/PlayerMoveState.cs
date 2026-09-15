using UnityEngine;
using UnityEditor.Animations;
public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine) {  }
    public override StateType State => StateType.Move;
    protected override string StateAnimationName => "Move"; 

    private void ApplyHorizontalMovement()
    {
        Vector3 velocity = playerGameplay.Rigidbody.linearVelocity;
        velocity.x = playerGameplay.PlayerInputController.HorizontalMoveInputValue * playerGameplay.Character.CharacterStatData.moveSpeed;
        playerGameplay.Rigidbody.linearVelocity = velocity;
        playerGameplay.Character.CharacterAnimatorController.UpdateVelocityAnimation(playerGameplay.PlayerInputController.HorizontalMoveInputValue);
    }

    public override void RegisterTransition()
    {
        base.RegisterTransition();
        AddTransition(() => playerGameplay.PlayerInputController.AttackBuffered && playerGameplay.IsGrounded, stateMachine.Attack);
        AddTransition(() => !playerGameplay.PlayerInputController.HasWalkInput && playerGameplay.IsGrounded, stateMachine.Idle);
    }
    
    private void CancelHorizontalMovement()
    {
        playerGameplay.Rigidbody.linearVelocity = Vector3.zero;
        playerGameplay.Character.CharacterAnimatorController.UpdateVelocityAnimation(0);
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
