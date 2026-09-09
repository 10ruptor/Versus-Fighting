using UnityEngine;
using System.Collections.Generic;


public class BufferedActionController : MonoBehaviour
{
    [Header("buffer Settings")]
    [SerializeField] private BufferInputSettingsSO bufferSettings;

    public Dictionary<BufferedAction.InputBufferedAction, float> bufferedActionsTimers = new Dictionary<BufferedAction.InputBufferedAction, float>();

    private void Awake()
    {
        foreach (BufferInputSettingsSO.bufferInputSetting setting in bufferSettings.settings)
        {
            if (!bufferedActionsTimers.TryAdd(setting.action, setting.buffertime))
            {
                Debug.Log("value for  key : " + setting.action + "already exists");   
            }
        }
    }

    private Queue<BufferedAction> ActionQueue = new Queue<BufferedAction>();
   

    public void bufferAction(BufferedAction  action)
    {
        ActionQueue.Enqueue(action);
    }
    
    public BufferedAction enqueueBufferAction(BufferedAction action)
    {
        return ActionQueue.Dequeue();
    }

    public void Update()
    {
        PrintActionQueue();
    }

    public void PrintActionQueue()
    {
        Debug.Log(string.Join(",",ActionQueue));
    }
}
