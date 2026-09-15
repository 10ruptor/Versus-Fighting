using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState
{
    /// <summary>
    /// Identifiant serialisable d'un etat. Il n'existe que pour permettre a la donnee cote
    /// personnage (StateParametersLibrarySO) de designer un etat depuis l'inspecteur : la
    /// machine, elle, resout ses etats par propriete typee, pas par cet enum.
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
    /// Ecrit en dur dans chaque etat concret, jamais serialise : rien a renseigner dans
    /// l'inspecteur, donc rien qui puisse se desynchroniser, et le compilateur force chaque
    /// nouvel etat a se declarer.
    /// </summary>
    public abstract StateType State { get; }

    /// <summary>
    /// Parametres dependants du personnage, injectes par BindParameters au demarrage.
    /// L'etat les porte mais ne les applique pas : chaque controleur lit ce qui le concerne.
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
    /// Appelee a chaque changement de personnage, pas seulement au demarrage : les parametres
    /// appartiennent au Character, pas au joueur.
    /// </summary>
    public void BindParameters(StateParametersSO parameters)
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
