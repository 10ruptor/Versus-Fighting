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
        // Assigne avant toute construction d'etat : chaque PlayerState lit
        // stateMachine.PlayerGameplay des son constructeur.
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

        // En second temps seulement : une transition reference un etat cible, donc
        // tous les etats doivent exister avant que le premier ne s'enregistre.
        foreach (PlayerState state in allStates)
            state.RegisterTransition();
    }

    /// <summary>
    /// Entree dans l'etat initial. Volontairement separee du constructeur : elle joue
    /// une animation, donc elle attend que le Character instancie soit pleinement pret.
    /// </summary>
    public void Initialize(PlayerState startState)
    {
        ChangeState(startState);
    }

    public void ChangeState(PlayerState newState)
    {
        CurrentState?.Exit();
        Debug.Log("Changing state : " + newState);
        CurrentState = newState;
        CurrentState?.Enter();
        playerGameplay.SetCurrentStateName(CurrentState?.GetType().Name);
    }

    T Register<T>(T state) where T : PlayerState
    {
        allStates.Add(state);
        return state;
    }
}
