using System;
using UnityEngine;

public class CharacterCollisionController : MonoBehaviour
{
    const string StageTag = "Stage";
    const string PlayerTag = "Player";
    const string HitboxTag = "Hitbox";
    const string DeadzoneTag =  "Deadzone";
    
    int stageContactCount;
    public bool IsGrounded => stageContactCount > 0;
    private PlayerGameplay playerGameplay;
    
    private void Awake()
    {
        playerGameplay = GetComponent<PlayerGameplay>();
    }

    static bool IsStageCollision(Collision collision)
    {
        return collision.collider != null && collision.gameObject.CompareTag(StageTag);
    }
    
    static bool IsHitCollision(Collision collision)
    {
        return collision.collider != null && collision.gameObject.CompareTag(HitboxTag);
    }

    static bool IsDeadZoneTrigger(Collider other)
    {
        return other.gameObject.CompareTag(DeadzoneTag);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Entered with: " + collision.gameObject.tag);
        if (IsStageCollision(collision))
            SetGrounded(stageContactCount + 1);
        
        if (IsHitCollision(collision))
        {
            Debug.Log(this.gameObject + " : hit");
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (IsStageCollision(collision))
        {
            SetGrounded(stageContactCount - 1);
        }
        
        if (IsHitCollision(collision))
        {
            Debug.Log(this.gameObject + " : hit");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsDeadZoneTrigger(other))
        {
            playerGameplay.PlayerReswpan();
        }
    }

    void SetGrounded(int contactCount)
    {
        stageContactCount = Mathf.Max(0, contactCount);
    }
    
    
}
