
using UnityEngine;
using System.Collections;

public class PlayerAttackState : PlayerState
{
    protected override string StateAnimationName => "Attack";

    public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine) {  }
    public override StateType State => StateType.Attack;

    public override void RegisterTransition()
    {
        AddTransition(() => !playerGameplay.AttackController.IsAttacking, stateMachine.Idle);
    }

    public override void Enter()
    {
        base.Enter();
        playerGameplay.PlayerInputController.ConsumeAttackBuffer();
        playerGameplay.AttackController.ResolveGroundAttack();
        playerGameplay.AttackController.StartAttack();
    }

    public override void Exit()
    {
        base.Exit();
        playerGameplay.AttackController.EndAttack();
    }

    public override void Update()
    {
        CheckTransitions();
    }
    
}




