using System;
using UnityEngine;

public class CharacterCollisionController : MonoBehaviour
{
    const string StageTag = "Stage";
    const string PlayerTag = "Player";
    const string HitboxTag = "Hitbox";
    const string DeadzoneTag =  "Deadzone";
    const string GroundTag = "Ground";
    
    int stageContactCount;
    public bool IsGrounded => stageContactCount > 0;
    private PlayerGameplay playerGameplay;

    private CapsuleCollider capsule;

    
    private void Awake()
    {
        playerGameplay = GetComponent<PlayerGameplay>();
        capsule = GetComponent<CapsuleCollider>();
    }
    
    /// <summary>
    /// Applique la capsule d'un etat. Appelee par PlayerStateMachine aux seuls changements
    /// d'etat : l'ajustement continu frame par frame a ete retire, il faisait varier le
    /// collider en permanence et provoquait des pertes de contact avec le sol.
    /// </summary>
    public void ApplyColliderSettings(StateParametersSO.ColliderSettings settings)
    {
        if (!settings.IsValid)
        {
            Debug.LogError($"ColliderSettings invalide (height={settings.height}, radius={settings.radius}) : capsule inchangee.", this);
            return;
        }

        capsule.height = settings.height;
        capsule.radius = settings.radius;
        capsule.center = settings.center;
    }


    static bool IsStageCollision(Collision collision)
    {
        return collision.collider != null && collision.gameObject.CompareTag(StageTag);
    }
    
    static bool IsGroundCollision(Collision collision)
    {
        return collision.collider != null && collision.gameObject.CompareTag(GroundTag);
    }
    

    static bool IsDeadZoneTrigger(Collider other)
    {
        return other.gameObject.CompareTag(DeadzoneTag);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Entered with: " + collision.gameObject.tag);
        if (IsGroundCollision(collision))
            SetGrounded(stageContactCount + 1);
    }

    void OnCollisionExit(Collision collision)
    {
        if (IsGroundCollision(collision))
        {
            SetGrounded(stageContactCount - 1);
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
