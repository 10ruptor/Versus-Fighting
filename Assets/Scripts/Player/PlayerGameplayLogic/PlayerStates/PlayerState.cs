using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState
{
    /// <summary>
    /// ID of a state. 
    /// </summary>
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

    /// <summary>
    /// Need to be hardcoded for each state
    /// </summary>
    public abstract StateType State { get; }

    /// <summary>
    /// Character dependant parameter, binded by StateMachine
    /// </summary>
    public StateParametersSO Parameters { get; private set; }

    protected readonly PlayerStateMachine stateMachine;
    protected readonly PlayerGameplay playerGameplay;
    private List<StateTransition>  transitions = new List<StateTransition>();
    protected virtual string StateAnimationName => null;

    protected PlayerState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        this.playerGameplay = stateMachine.PlayerGameplay;
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
            playerGameplay.Character.CharacterAnimatorController.AnimationTransition(StateAnimationName);
        }
    }

    public abstract void RegisterTransition();

    /// <summary>
    ///called on any Character change : parameter belongs to Character
    /// </summary>
    public void BindCharacterParameters(StateParametersSO parameters)
    {
        Parameters = parameters;
    }

    public virtual void Enter()
    {
        Debug.Log("Enter state : " + this);
        PlayStateAnimation();
    }
    
    public virtual void Exit() { Debug.Log("Exit state : " + this); }

    public virtual void Update() { }
    
    public virtual void FixedUpdate() { }
}
