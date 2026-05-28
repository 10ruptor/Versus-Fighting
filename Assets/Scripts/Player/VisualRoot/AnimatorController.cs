using UnityEngine;
using System;
public class AnimatorController : MonoBehaviour
{
    
    [SerializeField] private Animator animator;
    
    private int Velocity = Animator.StringToHash("Velocity");
    private int Attack = Animator.StringToHash("Attack");
    private int Run = Animator.StringToHash("Run");
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
        animator.SetBool(Attack, attacking);
    }
    
    

}
