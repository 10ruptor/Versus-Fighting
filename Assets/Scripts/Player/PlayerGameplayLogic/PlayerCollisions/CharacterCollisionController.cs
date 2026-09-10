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

    private Transform characterTop => playerGameplay.Character.CharacterTop;
    private Transform characterBottom => playerGameplay.Character.CharacterBottom;

    private Collider capsule;

    
    private void Awake()
    {
        playerGameplay = GetComponent<PlayerGameplay>();
        capsule = GetComponent<CapsuleCollider>();
    }
    
    private void FitColliderWithPlayerMesh()
    {
        float height = characterTop.position.y - characterBottom.position.y;
        float center = height / 2;
        Vector3 centralPosition = new Vector3(transform.position.x, center, transform.position.z);
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
