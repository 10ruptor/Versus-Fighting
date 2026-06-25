
using UnityEngine;
using System.Collections;


public class PlayerAttackState : PlayerState 
{

    private const float CancelThreshold = 0.3f; // Temps avant de pouvoir annuler l'attaque
    private AttackController.Attacks currentAttack;

    public PlayerAttackState(PlayerGameplay playerGameplay) : base(playerGameplay) { }
    
    public override void Enter()
    {
        playerGameplay.AttackController.ResolveAttack();
        playerGameplay.AttackController.StartAttack();
    }

    public override void Update()
    {
        if (!playerGameplay.AttackController.IsAttacking)
        {
            playerGameplay.StateMachine.ChangeState(playerGameplay.playerIdleState);
        }
    }

    public override void Exit()
    {
        //Logic Done in Attack controller
    }
}




