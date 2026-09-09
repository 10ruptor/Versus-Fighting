using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

public class BufferedAction
{
    public enum BufferedActionType
    {
        Jump,
        Attack
    }
    
    public BufferedActionType actionType;
    private float pressedAt = 0f;

    public BufferedAction(BufferedActionType actionType, float bufferedTime)
    {
        this.actionType = actionType;
        this.pressedAt = bufferedTime;
    }

    public bool bufferedTimeConsumed(float bufferDuration)
    {
        return Time.time - pressedAt > bufferDuration;
    }
}


