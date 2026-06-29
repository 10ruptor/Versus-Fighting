
using UnityEngine;
using System.Collections;

public class PlayerAttackState : PlayerState 
{

    private const float CancelThreshold = 0.3f; // Temps avant de pouvoir annuler l'attaque
    private AttackController.Attacks currentAttack;

    public PlayerAttackState(PlayerGameplay playerGameplay) : base(playerGameplay) {  }

    public override void RegisterTransition()
    {
        AddTransition(() => !playerGameplay.AttackController.IsAttacking, playerGameplay.playerIdleState);
    }

    public override void Enter()
    {
        base.Enter();
        playerGameplay.AttackController.ResolveAttack();
        playerGameplay.AttackController.StartAttack();
    }

    public override void Update()
    {
        CheckTransitions();
    }
    
}




