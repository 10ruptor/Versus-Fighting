using System;
using UnityEngine;

public class StateTransition
{
    public readonly Func<bool> Condition;
    public readonly PlayerState TargetState;

    public StateTransition(Func<bool> condition, PlayerState targetState)
    {
        Condition = condition;
        TargetState = targetState;
    }
}
