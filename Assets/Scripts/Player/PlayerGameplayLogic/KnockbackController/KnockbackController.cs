using UnityEngine;

public class KnockbackController : MonoBehaviour
{
    PlayerGameplay playerGameplay;
    private void Awake()
    {
        playerGameplay = GetComponent<PlayerGameplay>();
    }

    public void Knockback(HitData hitData)
    {
        //Debug.Log("Knock back to apply : " + hitData.Attack.KnockbackPower + " vector : " + hitData.HitPosition );
        playerGameplay.DamageController.AddDamage(hitData.Attack.Damage);
        playerGameplay.PlayerKnockedState.Initialize(hitData);
        playerGameplay.StateMachine.ChangeState(playerGameplay.PlayerKnockedState);
    }

}
