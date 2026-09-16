using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Onwner of Player states. The only one who knows state
/// Each state is a typed property (stateMachine.Idle)
/// </summary>
public class PlayerStateMachine
{
    readonly PlayerGameplay playerGameplay;
    readonly List<PlayerState> allStates = new List<PlayerState>();

    public PlayerIdleState      Idle      { get; }
    public PlayerMoveState      Move      { get; }
    public PlayerDashState      Dash      { get; }
    public PlayerCrouchState    Crouch    { get; }
    public PlayerAttackState    Attack    { get; }
    public PlayerJumpingState   Jumping   { get; }
    public PlayerLandingState   Landing   { get; }
    public PlayerAirAttackState AirAttack { get; }
    public PlayerKnockedState   Knocked   { get; }

    public PlayerState CurrentState { get; private set; }
    public PlayerGameplay PlayerGameplay => playerGameplay;

    public PlayerStateMachine(PlayerGameplay playerGameplay)
    {

        this.playerGameplay = playerGameplay;

        Idle      = Register(new PlayerIdleState(this));
        Move      = Register(new PlayerMoveState(this));
        Dash      = Register(new PlayerDashState(this));
        Crouch    = Register(new PlayerCrouchState(this));
        Attack    = Register(new PlayerAttackState(this));
        Jumping   = Register(new PlayerJumpingState(this));
        Landing   = Register(new PlayerLandingState(this));
        AirAttack = Register(new PlayerAirAttackState(this));
        Knocked   = Register(new PlayerKnockedState(this));

        RegisterAllStateTransitions();
        AssertStateTypesAreUnique();
    }
    
    //Genericité : allow to use register for any class that inherit from PlayerState
    T Register<T>(T state) where T : PlayerState
    {
        allStates.Add(state);
        return state;
    }

    private void RegisterAllStateTransitions()
    {
        foreach (PlayerState state in allStates)
        {
            state.RegisterTransition();
        }
    }

    // Les parametres appartiennent au Character : rappelable tel quel si le personnage change.
    public void BindStateParameters(StateParametersLibrarySO library)
    {
        if(library == null)
        {
            Debug.LogError($"No library assigned to {playerGameplay.Character}",context:this.playerGameplay.Character);
            return;
        }
        
        library.Initialize();

        foreach (PlayerState state in allStates)
        {
            state.BindCharacterParameters(library.Resolve(state.State));
        }
    }

    // Deux etats declarant le meme StateType rendraient la resolution des parametres ambigue.
    private void AssertStateTypesAreUnique()
    {
        HashSet<PlayerState.StateType> declaredTypes = new HashSet<PlayerState.StateType>();

        foreach (PlayerState state in allStates)
        {
            if (!declaredTypes.Add(state.State))
            {
                Debug.LogError($"{state.GetType().Name} declaring {state.State}, which is already declared for an other state");
            }
        }
    }

    public void Initialize(PlayerState startState)
    {
        ChangeState(startState);
    }

    public void ChangeState(PlayerState newState)
    {
        PlayerState previousState = CurrentState;

        previousState?.Exit();
        Debug.Log("Changing state : " + newState);
        CurrentState = newState;

        // Before Enter : so the state starts with the right collider directly
        ApplyStateParameter(previousState, newState);

        CurrentState?.Enter();
        playerGameplay.SetCurrentStateName(CurrentState?.GetType().Name);
    }

    // Nothing done when previous and next state have same parameter
    private void ApplyStateParameter(PlayerState previousState, PlayerState newState)
    {
        if (newState == null || newState.Parameters == null) return;

        if (previousState != null && previousState.Parameters == newState.Parameters) return;

        playerGameplay.CollisionController.ApplyColliderSettings(newState.Parameters.ColliderSetting);
    }
    
}
