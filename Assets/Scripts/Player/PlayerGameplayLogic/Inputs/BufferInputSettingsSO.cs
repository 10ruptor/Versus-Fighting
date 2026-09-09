using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BufferInputSettingsSO", menuName = "Scriptable Objects/BufferInputSettingsSO")]
public class BufferInputSettingsSO : ScriptableObject
{
    [System.Serializable]
    public class bufferInputSetting
    {
        public BufferedAction.InputBufferedAction action;
        public float buffertime;
    }

    public List<bufferInputSetting> settings = new List<bufferInputSetting>();
}
