using UnityEngine;
using System;
public class CharacterAnimatorController : MonoBehaviour
{
    
    [SerializeField] private Animator animator;
    private int Velocity = Animator.StringToHash("Velocity");
    
    public void AnimationTransition(string StateName)
    {
        if (!animator.HasState(0, Animator.StringToHash(StateName))) { return; }
        animator.CrossFade(StateName, 0.1f);
    }

    public void UpdateVelocityAnimation(float velocity)
    {
        animator.SetFloat(Velocity, Math.Abs(velocity) );
    }
    
    public void VisualOrientationUpdate(PlayerGameplay.Orientation orientation)
    {
        switch (orientation)
        {
            case PlayerGameplay.Orientation.Left:
                transform.rotation = Quaternion.LookRotation(Vector3.back, Vector3.up);
                break;
            case PlayerGameplay.Orientation.Right:
                transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
                break;
        }
    }
    
}
