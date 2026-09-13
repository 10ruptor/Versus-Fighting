using UnityEngine;
using UnityEditor.Animations;
public class PlayerMoveState : PlayerGroundedState
{
    
    protected override string StateAnimationName => "Move"; 

    private void ApplyHorizontalMovement()
    {
        Vector3 velocity = stateMachine.PlayerGameplay.Rigidbody.linearVelocity;
        velocity.x = stateMachine.PlayerGameplay.PlayerInputController.HorizontalMoveInputValue * stateMachine.PlayerGameplay.Character.CharacterStatData.moveSpeed;
        stateMachine.PlayerGameplay.Rigidbody.linearVelocity = velocity;
        stateMachine.PlayerGameplay.Character.CharacterAnimatorController.UpdateVelocityAnimation(stateMachine.PlayerGameplay.PlayerInputController.HorizontalMoveInputValue);
    }

    public override void RegisterTransition()
    {
        base.RegisterTransition();
        AddTransition(() => stateMachine.PlayerGameplay.PlayerInputController.AttackBuffered && stateMachine.PlayerGameplay.IsGrounded, stateMachine.stateLibrary[StateType.Attack]);
        AddTransition(() => !stateMachine.PlayerGameplay.PlayerInputController.HasWalkInput && stateMachine.PlayerGameplay.IsGrounded, stateMachine.stateLibrary[StateType.Idle]);
    }
    
    private void CancelHorizontalMovement()
    {
        stateMachine.PlayerGameplay.Rigidbody.linearVelocity = Vector3.zero;
        stateMachine.PlayerGameplay.Character.CharacterAnimatorController.UpdateVelocityAnimation(0);
    }
    
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        CheckTransitions();
        ApplyHorizontalMovement();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        CancelHorizontalMovement();
    }
}
