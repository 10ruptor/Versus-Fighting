using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State -> Parameter table for one Character
/// PlayerStateMachine doesn't modify collider when switching between two states sharing same Parameter (ex :Idle -> Move -> Dash)
/// </summary>
[CreateAssetMenu(fileName = "StateParametersLibrary", menuName = "Versus Fighting/Character/State Parameters Library")]
public class StateParametersLibrarySO : ScriptableObject
{
    [Serializable]
    private struct StateParameterEntry
    {
        public PlayerState.StateType state;
        public StateParametersSO parameters;
    }

    [Tooltip("Used when State has no parameter assigned")]
    [SerializeField] StateParametersSO defaultParameters;
    [Tooltip("For debug : display list of parameter in inspector at runtime")]
    [SerializeField] StateParameterEntry[] entries = Array.Empty<StateParameterEntry>();

    Dictionary<PlayerState.StateType, StateParametersSO> lookup = new Dictionary<PlayerState.StateType, StateParametersSO>();
    
    public void Initialize()
    {
        lookup.Clear();

        foreach (StateParameterEntry entry in entries)
        {
            if (!lookup.TryAdd(entry.state, entry.parameters))
            {
                Debug.LogError($"{name} : two entries for {entry.state}, only the first one is kept.", this);
            }
        }
    }

    public StateParametersSO Resolve(PlayerState.StateType state)
    {
        if (lookup.TryGetValue(state, out StateParametersSO parameters) && parameters != null)
            return parameters;

        if (defaultParameters == null)
            Debug.LogError($"{name} : no parameter for {state}, no default parameter assigned", this);

        return defaultParameters;
    }
}
