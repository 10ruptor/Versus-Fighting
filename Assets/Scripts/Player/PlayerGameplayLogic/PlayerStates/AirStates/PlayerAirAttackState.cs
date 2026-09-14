
using System.Collections.Generic;

public class PlayerAirAttackState : PlayerAirState
{
   
    protected override string StateAnimationName => "AirAttack";

    public override void RegisterTransition()
    {
        AddTransition(() => IsLanding && stateMachine.PlayerGameplay.IsGrounded && stateMachine.PlayerGameplay.PlayerInputController.HasWalkInput,stateMachine.stateLibrary[StateType.Move]);
        AddTransition(() => IsLanding  && stateMachine.PlayerGameplay.IsGrounded && !stateMachine.PlayerGameplay.PlayerInputController.HasWalkInput,stateMachine.stateLibrary[StateType.Idle]);
        AddTransition(() => stateMachine.PlayerGameplay.PlayerInputController.JumpBuffered && stateMachine.PlayerGameplay.JumpController.CanJump  && !stateMachine.PlayerGameplay.AttackController.IsAttacking, stateMachine.stateLibrary[StateType.Jumping]);
        AddTransition(() => IsLanding && !stateMachine.PlayerGameplay.IsGrounded && !stateMachine.PlayerGameplay.AttackController.IsAttacking, stateMachine.stateLibrary[StateType.Landing]);
    }

    public override void OnEnable()
    {
        stateMachine.PlayerGameplay.PlayerInputController.ConsumeAttackBuffer();
        stateMachine.PlayerGameplay.AttackController.ResolveAerialAttack();
        stateMachine.PlayerGameplay.AttackController.StartAttack();
    }
    
    public override void OnDisable()
    {
        base.OnDisable();
        stateMachine.PlayerGameplay.AttackController.EndAttack(); // mandatory for case switching to idle ground state while attack is not finished 
    }

    public override void Update()
    {
        base.Update();
        CheckTransitions();
    }
    
    
    
}
