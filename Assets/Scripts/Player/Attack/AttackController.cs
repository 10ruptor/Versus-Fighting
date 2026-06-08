using UnityEngine;

public class AttackController : MonoBehaviour
{
    public bool IsAttacking { get; private set; }

    AttackStatSO currentAttack;
    //Hitbox
    //AttackAnimations

    public void StartAttack(/* ajouter stat so*/)
    {
        IsAttacking = true;
    }

    public void EndAttack()
    {
        Debug.Log("Attack ended.");
        IsAttacking = false;
    }
}
