using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : MonoBehaviour
{
    public enum StateType
    {
        Idle,
        Dash,
        Move,
        Attack,
        Crouch,
        
        Jumping,
        Landing,
        Knocked,
        AirAttack,
    }

    [SerializeField] protected StateType state;
    public StateType State => state;
    
    protected PlayerStateMachine stateMachine;
    private List<StateTransition>  transitions = new List<StateTransition>();
    protected virtual string StateAnimationName => null;

    protected virtual void Initialize(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        RegisterTransition();
    }

    protected void CheckTransitions()
    {
        foreach (StateTransition transition in transitions)
        {
            if (transition.Condition())
            {
                stateMachine.ChangeState(transition.TargetState);
                return;
            }
        }
    }

    protected void AddTransition(Func<bool> condition, PlayerState targetState)
    {
        transitions.Add(new StateTransition(condition, targetState));
    }

    private void PlayStateAnimation()
    {
        if (StateAnimationName != null)
        {
            stateMachine.PlayerGameplay.Character.CharacterAnimatorController.AnimationTransition(StateAnimationName);
        }
    }

    public abstract void RegisterTransition();

    public virtual void OnEnable()
    {
        Debug.Log("Enter state : " + this);
        PlayStateAnimation();
    }
    
    public virtual void OnDisable() { Debug.Log("Exit state : " + this); }
    
    public virtual void Update() { }
    
    public virtual void FixedUpdate() { }
}
