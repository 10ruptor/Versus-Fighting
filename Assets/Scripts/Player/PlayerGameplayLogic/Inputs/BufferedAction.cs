using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

public class BufferedAction
{
    public enum InputBufferedAction
    {
        Jump,
        Attack
    }
    
    InputBufferedAction action;
    float bufferedTime = 0f;

    public BufferedAction(InputBufferedAction action, float bufferedTime)
    {
        this.action = action;
        this.bufferedTime = bufferedTime;
    }
}


