
using UnityEngine;
using System.Collections;


public class PlayerAttackState : PlayerState 
{
    private const float CancelThreshold = 0.3f; // Temps avant de pouvoir annuler l'attaque
    
    public PlayerAttackState(PlayerGameplay playerGameplay) : base(playerGameplay) { }
	

    public override void Enter()
    {
        PlayerGameplay.AttackController.StartAttack();
    }

    public override void Update()
    {
        if (!PlayerGameplay.AttackController.IsAttacking)
        {
            PlayerGameplay.StateMachine.ChangeState(new PlayerIdleState(PlayerGameplay));
        }
    }

    public override void Exit()
    {
        //Logic Done in Attack controller
    }
}




