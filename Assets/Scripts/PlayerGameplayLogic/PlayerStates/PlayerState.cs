using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState
{
    protected readonly PlayerGameplay playerGameplay;
    private List<StateTransition>  transitions = new List<StateTransition>();
    protected virtual string StateAnimationName => null;

    protected PlayerState(PlayerGameplay playerGameplay)
    {
        this.playerGameplay = playerGameplay;
    }

    protected void CheckTransitions()
    {
        foreach (StateTransition transition in transitions)
        {
            if (transition.Condition())
            {
                playerGameplay.StateMachine.ChangeState(transition.TargetState);
                return;
            }
        }
    }

    protected void AddTransition(Func<bool> condition, PlayerState targetState)
    {
        transitions.Add(new StateTransition(condition, targetState));
    }

    private void PlayAnimation()
    {
        if (StateAnimationName != null)
        {
            playerGameplay.Character.CharacterAnimatorController.AnimationTransition(StateAnimationName);
        }
    }

    public abstract void RegisterTransition();

    public virtual void Enter()
    {
        Debug.Log("Enter state : " + this);
        PlayAnimation();
    }
    
    public virtual void Exit() { Debug.Log("Exit state : " + this); }

    public virtual void Update() { }
    
    public virtual void FixedUpdate() { }
}
