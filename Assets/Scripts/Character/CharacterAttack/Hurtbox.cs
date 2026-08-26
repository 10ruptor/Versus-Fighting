using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private PlayerGameplay owner;
    public PlayerGameplay Owner => owner;
    
    public void Initialize(PlayerGameplay owner)
    {
        this.owner = owner;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hitbox")
        {
            if(IsOtherPlayer(other))
            {
                Hitbox hitbox = other.GetComponent<Hitbox>();
                HitData hitData = CreateHit(hitbox, other.ClosestPoint(transform.position));
                owner.KnockbackController.Knockback(hitData);
            }
        }
    }

    private bool IsOtherPlayer(Collider other)
    {
        switch (other.tag)
        {
            case "Hitbox":
                return other.GetComponent<Hitbox>().Owner != this.owner;
            
            case "Hurtbox":
                return other.GetComponent<Hurtbox>().Owner != this.owner;
            
            default:
                return false;
        }
    }
    
    private HitData CreateHit(Hitbox hitbox, Vector3 position)
    {
        AttackDataSO attackData = hitbox.attackData;

        if (attackData == null)
        {
            Debug.LogError("AttackDataSO not found on hitbox.");
            return default;
        }

        return new HitData
        {
            Attacker = hitbox.Owner,
            Attack = attackData,
            HitPosition = position
        };
    }
}
