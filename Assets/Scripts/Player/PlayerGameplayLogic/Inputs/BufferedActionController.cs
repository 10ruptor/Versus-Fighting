using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Experimental.Audio;


public class BufferedActionController : MonoBehaviour
{
    [Header("buffer Settings")]
    [SerializeField] private BufferInputSettingsSO bufferSettings;

    public Dictionary<BufferedAction.BufferedActionType, float> actionsBufferDurations = new Dictionary<BufferedAction.BufferedActionType, float>();
    private List<BufferedAction> actionQueue = new List<BufferedAction>();
    
    private void Awake()
    {
        foreach (BufferInputSettingsSO.bufferInputSetting setting in bufferSettings.settings)
        {
            if (!actionsBufferDurations.TryAdd(setting.actionType, setting.bufferDuration))
            {
                Debug.Log("value for  key : " + setting.actionType + "already exists");   
            }
        }
    }
    
    bool IsAlive(BufferedAction a) => !a.bufferedTimeConsumed(actionsBufferDurations[a.actionType]);
    public bool HasAlive(BufferedAction.BufferedActionType actionType)
    {
        return actionQueue.Find(a => a.actionType == actionType && IsAlive(a)) != null;
    }

    public void Purge()
    {
        actionQueue.RemoveAll(a => a.bufferedTimeConsumed(actionsBufferDurations[a.actionType]));
    }
    
    public void AddBufferedAction(BufferedAction action)
    {
        actionQueue.Add(action);
        Purge();
    }
    
    public void Consume(BufferedAction.BufferedActionType actionType)
    {
       actionQueue.RemoveAll(a => a.actionType == actionType);
    }
    
    private void PrintActionQueue()
    {
        Debug.Log(string.Join(",",actionQueue));
    }
}
