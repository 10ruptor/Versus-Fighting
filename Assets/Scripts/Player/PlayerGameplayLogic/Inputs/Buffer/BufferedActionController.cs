using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Memorise brievement les intentions du joueur pour qu'un input emis pendant un etat
/// qui ne l'accepte pas encore ne soit pas perdu.
/// Sens du flux : PlayerInputController ecrit, la machine a etats lit.
/// </summary>
public class BufferedActionController : MonoBehaviour
{

    [Header("buffer Settings")]
    [SerializeField] private BufferInputSettingsSO bufferSettings;
    [SerializeField] private float defaultBufferDuration = 0.1f;

    private readonly Dictionary<BufferedAction.BufferedActionType, float> actionsBufferDurations = new Dictionary<BufferedAction.BufferedActionType, float>();
    private readonly List<BufferedAction> bufferedActions = new List<BufferedAction>();

    private void Awake()
    {
        if (bufferSettings == null)
        {
            Debug.LogError($"BufferedActionController : aucun BufferInputSettingsSO assigne sur {name}, repli sur {defaultBufferDuration}s.", this);
            return;
        }

        foreach (BufferInputSettingsSO.bufferInputSetting setting in bufferSettings.settings)
        {
            if (!actionsBufferDurations.TryAdd(setting.actionType, setting.bufferDuration))
                Debug.LogWarning($"BufferedActionController : duree en double pour {setting.actionType}, la premiere est conservee.", this);
        }

        WarnAboutMissingDurations();
    }
    
    private void WarnAboutMissingDurations()
    {
        foreach (BufferedAction.BufferedActionType actionType in Enum.GetValues(typeof(BufferedAction.BufferedActionType)))
        {
            if (!actionsBufferDurations.ContainsKey(actionType))
                Debug.LogWarning($"BufferedActionController : aucune duree definie pour {actionType} dans {bufferSettings.name}, repli sur {defaultBufferDuration}s.", this);
        }
    }

    /// <summary> return buffer duration of the action if defined in buffer settings, if not return FallbackBufferDuration </summary>
    private float BufferDurationOf(BufferedAction.BufferedActionType actionType)
    {
        return actionsBufferDurations.TryGetValue(actionType, out float duration) ? duration : defaultBufferDuration;
    }
    
    /// <summary> check if a buffered action is expired </summary>
    private bool IsAlive(BufferedAction action) => !action.IsExpired(BufferDurationOf(action.ActionType));

    /// <summary> check if there is not expired buffered input for actionType </summary>
    public bool HasAlive(BufferedAction.BufferedActionType actionType)
    {
        return bufferedActions.Find(action => action.ActionType == actionType && IsAlive(action)) != null;
    }
    
    /// <summary> called in input controller callbacks to register inputs in buffer queue </summary>
    public void AddBufferedAction(BufferedAction.BufferedActionType actionType, float pressedAt)
    {
        bufferedActions.Add(new BufferedAction(actionType, pressedAt));
        Purge();
    }

    /// <summary> to consume all Action of type action type to keep only the intention of the user </summary>
    public void Consume(BufferedAction.BufferedActionType actionType)
    {
        bufferedActions.RemoveAll(action => action.ActionType == actionType);
    }

    /// <summary> clear every buffered inputs</summary>
    public void Clear()
    {
        bufferedActions.Clear();
    }

    /// <summary> purge is not used to clear the buffered input but only to removed expired inputs frequently</summary>
    public void Purge()
    {
        bufferedActions.RemoveAll(action => !IsAlive(action));
    }

    public void PrintActionQueue()
    {
        Debug.Log(string.Join(",", bufferedActions), this);
    }
}
