
using UnityEngine;
using System.Collections;

public class PlayerAttackState : PlayerState 
{
    
    protected override string StateAnimationName => "Attack";

    public PlayerAttackState(PlayerGameplay playerGameplay) : base(playerGameplay) {  }

    public override void RegisterTransition()
    {
        AddTransition(() => !playerGameplay.AttackController.IsAttacking, playerGameplay.PlayerIdleState);
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




