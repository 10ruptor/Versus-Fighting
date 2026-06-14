using UnityEngine;
using System;
public class CharacterAnimatorController : MonoBehaviour
{
    
    [SerializeField] private Animator animator;
    
    private int Velocity = Animator.StringToHash("Velocity");
    private int Attack = Animator.StringToHash("Attack");
    private int Run = Animator.StringToHash("Run");

    public void UpdateRunAnimation(bool run)
    {
        animator.SetBool(Run,run);
    }

    public void UpdateVelocityAnimation(float velocity, float maxVelocity)
    {
        animator.SetFloat(Velocity, Math.Abs(velocity) / maxVelocity );
    }

    public void UpdateAttackAnimation(bool attacking)
    {
        Debug.Log("Attack animation updated: " + attacking);
        animator.SetBool(Attack, attacking);
    }
    
    public void UpdateAnimation(string trigger, bool state)
    {
        Debug.Log("Attack animation updated: " + trigger);
        animator.SetTrigger(trigger);
    }
    
    public void VisualOrientationUpdate(PlayerGameplay.Orientations orientation)
    {
        switch (orientation)
        {
            case PlayerGameplay.Orientations.Left:
                transform.rotation = Quaternion.LookRotation(Vector3.back, Vector3.up);
                break;
            case PlayerGameplay.Orientations.Right:
                transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
                break;
        }
    }

}
