using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Proprietaire des etats du joueur. C'est la seule classe qui connait la liste concrete
/// des etats : ajouter un etat ne touche donc que ce fichier, jamais PlayerGameplay.
/// Les etats s'y referencent entre eux par propriete typee (stateMachine.Idle), ce qui
/// laisse le compilateur valider chaque transition.
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
        if (library == null)
        {
            Debug.LogError($"Aucune StateParametersLibrarySO sur le Character de {playerGameplay.name} : les etats gardent le collider authore sur le prefab.", playerGameplay);
            return;
        }

        foreach (PlayerState state in allStates)
        {
            state.BindParameters(library.Resolve(state.State));
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
                Debug.LogError($"{state.GetType().Name} declare le StateType {state.State}, deja declare par un autre etat.");
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

        // Avant Enter : l'etat entrant raisonne ainsi sur sa capsule definitive.
        ApplyStateCollider(previousState, newState);

        CurrentState?.Enter();
        playerGameplay.SetCurrentStateName(CurrentState?.GetType().Name);
    }

    // Rien n'est ecrit quand l'etat entrant partage l'asset du precedent : assigner le meme
    // StateParametersSO a Idle, Move et Dash garantit qu'aucun enchainement entre ces trois
    // etats ne touche au collider, donc aucun risque de perdre le sol.
    // Ici et non dans Enter, qu'un etat peut oublier de chaîner.
    private void ApplyStateCollider(PlayerState previousState, PlayerState newState)
    {
        if (newState == null || newState.Parameters == null) return;

        if (previousState != null && previousState.Parameters == newState.Parameters) return;

        playerGameplay.CollisionController.ApplyColliderSettings(newState.Parameters.Collider);
    }
    
    //Genericité : allow to use register for any class that inherit from PlayerState
    T Register<T>(T state) where T : PlayerState
    {
        allStates.Add(state);
        return state;
    }
}
