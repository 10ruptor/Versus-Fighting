using UnityEngine;
using System;
public class CharacterAnimatorController : MonoBehaviour
{
    
    [SerializeField] private Animator animator;
    private int verticalMove = Animator.StringToHash("VerticalMove");
    private int Velocity = Animator.StringToHash("Velocity");
    private int Attack = Animator.StringToHash("Attack");
    private int Run = Animator.StringToHash("Run");
    private int Dash = Animator.StringToHash("Dash");

    public void UpdateRunAnimation(bool run)
    {
        animator.SetBool(Run,run);
    }

    public void UpdateVelocityAnimation(float velocity)
    {
        animator.SetFloat(Velocity, Math.Abs(velocity) );
    }
    
    public void UpdateVerticalAnimation(float verticalMove)
    {
        animator.SetFloat(Velocity, verticalMove);
    }
    
    public void UpdateAttackAnimation(string attackTrigger)
    {
        animator.SetTrigger(attackTrigger);
    }

    public void UpdateDashAnimation()
    {
        animator.SetTrigger(Dash);
    }

    public void UpdateCrouchAnimation(bool isCrouching)
    {
        Debug.Log("Crouch animation updated: " + isCrouching);
        animator.SetBool("Crouch", isCrouching);
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
