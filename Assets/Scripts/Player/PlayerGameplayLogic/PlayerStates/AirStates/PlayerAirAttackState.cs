
using System.Collections.Generic;

public class PlayerAirAttackState : PlayerAirState
{
    public PlayerAirAttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    public override StateType State => StateType.AirAttack;
    protected override string StateAnimationName => "AirAttack";

    public override void RegisterTransition()
    {
        AddTransition(() => IsLanding && playerGameplay.IsGrounded && playerGameplay.PlayerInputController.HasWalkInput,stateMachine.Move);
        AddTransition(() => IsLanding  && playerGameplay.IsGrounded && !playerGameplay.PlayerInputController.HasWalkInput,stateMachine.Idle);
        AddTransition(() => playerGameplay.PlayerInputController.JumpBuffered && playerGameplay.JumpController.CanJump  && !playerGameplay.AttackController.IsAttacking, stateMachine.Jumping);
        AddTransition(() => IsLanding && !playerGameplay.IsGrounded && !playerGameplay.AttackController.IsAttacking, stateMachine.Landing);
    }

    public override void Enter()
    {
        playerGameplay.PlayerInputController.ConsumeAttackBuffer();
        playerGameplay.AttackController.ResolveAerialAttack();
        playerGameplay.AttackController.StartAttack();
    }
    
    public override void Exit()
    {
        base.Exit();
        playerGameplay.AttackController.EndAttack(); // mandatory for case switching to idle ground state while attack is not finished 
    }

    public override void Update()
    {
        base.Update();
        CheckTransitions();
    }
    
    
    
}
