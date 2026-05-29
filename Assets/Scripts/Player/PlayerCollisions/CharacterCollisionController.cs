using UnityEngine;

public class CharacterCollisionController : MonoBehaviour
{
    const string StageTag = "Stage";
    int stageContactCount;
    public bool IsGrounded => stageContactCount > 0;
    
    static bool IsStageCollision(Collision collision)
    {
        return collision.collider != null && collision.gameObject.CompareTag(StageTag);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (IsStageCollision(collision))
            SetGrounded(stageContactCount + 1);
    }

    void OnCollisionExit(Collision collision)
    {
        if (IsStageCollision(collision))
            SetGrounded(stageContactCount - 1);
    }
    

    void SetGrounded(int contactCount)
    {
        stageContactCount = Mathf.Max(0, contactCount);
    }
}
