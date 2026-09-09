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

    public void AddBufferedAction(BufferedAction action)
    {
        actionQueue.Add(action);
        Purge();
    }

    public bool Has(BufferedAction.BufferedActionType actionType)
    {
        return actionQueue.Find(a =>
            a.actionType == actionType && !a.bufferedTimeConsumed(actionsBufferDurations[a.actionType])) != null;
    }

    public void Purge()
    {
        actionQueue.RemoveAll(a => a.bufferedTimeConsumed(actionsBufferDurations[a.actionType]));
    }
    
    public bool TryConsume(BufferedAction.BufferedActionType actionType, out BufferedAction consumed)
    {
        
        out actionQueue.Find(a => a.actionType == actionType && !a.bufferedTimeConsumed(actionsBufferDurations[a.actionType]));
        return true;
    }
    
    private void PrintActionQueue()
    {
        Debug.Log(string.Join(",",actionQueue));
    }
}
