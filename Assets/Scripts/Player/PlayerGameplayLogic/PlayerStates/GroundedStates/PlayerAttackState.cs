
using UnityEngine;
using System.Collections;

public class PlayerAttackState : PlayerState
{
    protected override string StateAnimationName => "Attack";

    public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine) {  }

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
    

    public override void Update()
    {
        CheckTransitions();
    }
    
}




