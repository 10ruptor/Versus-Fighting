
using UnityEngine;
using System.Collections;


public class PlayerAttackState : PlayerState 
{
    private float elapsedTime = 0f;
    private bool canCancelWithMovement = false; // Permettre le mouvement après une certaine durée
    private const float CancelThreshold = 0.3f; // Temps avant de pouvoir annuler l'attaque
    
    public PlayerAttackState(PlayerGameplay playerGameplay) : base(playerGameplay) { }
	float frameCount = 0;

    public override void Enter()
    {
		PlayerGameplay.CharacterAnimatorController.UpdateAttackAnimation(true);
		frameCount = 0;
        PlayerGameplay.TiltHitbox.enabled = true; // a déplacer dans le attack controller
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
        PlayerGameplay.CharacterAnimatorController.UpdateAttackAnimation(false);
    }
}




