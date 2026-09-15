using System;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStateMachine : MonoBehaviour
{
    public PlayerState CurrentState { get; private set; }
    private PlayerGameplay playerGameplay;
    public PlayerGameplay PlayerGameplay => playerGameplay;
    
    public Dictionary<PlayerState.StateType, PlayerState> stateLibrary = new Dictionary<PlayerState.StateType, PlayerState>();
    
    [SerializeField] private PlayerState.StateType startingState;
    [SerializeField] private List<PlayerState> states = new List<PlayerState>(); 
    
    
    private void Awake()
    {
        foreach (PlayerState state in GetComponentsInChildren<PlayerState>())
        {
            states.Add(state);
            if(!stateLibrary.TryAdd(state.State, state))
            {
                Debug.LogError($"Duplicate state name {state.State}");
            }
        }
    }

    public void Initialize(PlayerGameplay playerGameplay)
    {
        this.playerGameplay = playerGameplay;
        ChangeState(stateLibrary[startingState]);
    }

    public PlayerStateMachine(PlayerGameplay playerGameplay)
    {
        this.playerGameplay = playerGameplay;
    }

    public void ChangeState(PlayerState newState)
    {
        if(CurrentState != null) CurrentState.enabled = false;
        Debug.Log("Changing state : " + newState);
        CurrentState = newState;
        CurrentState.enabled = true;
        playerGameplay.SetCurrentStateName(CurrentState?.GetType().Name);
    }
}
