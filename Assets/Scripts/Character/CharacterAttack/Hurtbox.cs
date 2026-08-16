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
            if(other.GetComponent<Hitbox>().Owner != owner)
            {
                Debug.Log("Hurtbox: Hit by " + other.name + " of player  : " + owner.PlayerIndex);
                // Here you can add logic to handle the hit, such as reducing health, playing a sound, etc.
            }
        }
    }

}
