using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private float attackDuration = 0.5f; // Durée de l'attaque en secondes
    private float elapsedTime = 0f;
    private bool canCancelWithMovement = false; // Permettre le mouvement après une certaine durée
    private const float CancelThreshold = 0.3f; // Temps avant de pouvoir annuler l'attaque

    public PlayerAttackState(PlayerGameplay playerGameplay) : base(playerGameplay) { }

    public override void Enter()
    {
        PlayerGameplay.AnimatorController.UpdateAttackAnimation(true);
    }

    public override void FixedUpdate()
    {
        
    }

    public override void Exit()
    {
        PlayerGameplay.AnimatorController.UpdateAttackAnimation(false);
    }
}




