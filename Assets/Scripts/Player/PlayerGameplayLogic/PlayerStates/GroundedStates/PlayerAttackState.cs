using UnityEngine;
using System.Collections;

public class PlayerAttackState : PlayerState
{
    protected override string StateAnimationName => "Attack";

    public override void RegisterTransition()
    {
        AddTransition(() => !stateMachine.PlayerGameplay.AttackController.IsAttacking, stateMachine.stateLibrary[StateType.Idle]);
    }

    public override void OnEnable()
    {
        OnEnable();
        stateMachine.PlayerGameplay.PlayerInputController.ConsumeAttackBuffer();
        stateMachine.PlayerGameplay.AttackController.ResolveGroundAttack();
        stateMachine.PlayerGameplay.AttackController.StartAttack();
    }
    

    public override void Update()
    {
        CheckTransitions();
    }
    
}




