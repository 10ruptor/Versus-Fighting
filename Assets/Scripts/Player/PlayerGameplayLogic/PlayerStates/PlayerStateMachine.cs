using System;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStateMachine : MonoBehaviour
{
    public PlayerState CurrentState { get; private set; }
    readonly PlayerGameplay playerGameplay;
    public PlayerGameplay PlayerGameplay => playerGameplay;
    
    public Dictionary<PlayerState.StateType, PlayerState> stateLibrary = new Dictionary<PlayerState.StateType, PlayerState>();
    public List<PlayerState> states = new List<PlayerState>(); //For debug to be deleted
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

    public PlayerStateMachine(PlayerGameplay playerGameplay)
    {
        this.playerGameplay = playerGameplay;
    }

    public void ChangeState(PlayerState newState)
    {
        CurrentState.enabled = false;
        Debug.Log("Changing state : " + newState);
        CurrentState = newState;
        CurrentState.enabled = true;
        playerGameplay.SetCurrentStateName(CurrentState?.GetType().Name);
    }
}
