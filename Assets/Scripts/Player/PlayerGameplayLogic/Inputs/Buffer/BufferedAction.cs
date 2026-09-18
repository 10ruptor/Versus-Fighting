using UnityEngine;

/// <summary>
/// Une intention de joueur horodatee. Immuable une fois posee : seule sa fraicheur evolue.
/// </summary>
public class BufferedAction
{
    // L'ordre des valeurs est serialise (BufferInputSettingsSO) : toute nouvelle action
    // se rajoute a la fin pour ne pas decaler les durees deja configurees.
    public enum BufferedActionType
    {
        Jump,
        Attack,
        ShieldPressed,
        ShieldReleased
    }

    public BufferedActionType ActionType { get; }
    public float PressedAt { get; }

    public BufferedAction(BufferedActionType actionType, float pressedAt)
    {
        ActionType = actionType;
        PressedAt = pressedAt;
    }

    /// <summary>True if action input is older than buffer window .</summary>
    public bool IsExpired(float bufferDuration) => Time.time - PressedAt > bufferDuration;

    public override string ToString() => $"{ActionType}@{PressedAt:F2}";
}
