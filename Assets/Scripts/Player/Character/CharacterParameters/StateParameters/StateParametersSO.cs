using System;
using UnityEngine;

/// <summary>
/// Character dependant gameplay parameter (Two character does not have same mesh) 
/// Resolved when state change
/// </summary>
[CreateAssetMenu(fileName = "StateParameters", menuName = "Versus Fighting/Character/State Parameters")]
public class StateParametersSO : ScriptableObject
{
    /// <summary>
    /// Gameplay Capsule collider absolute
    /// </summary>
    [Serializable]
    public struct ColliderSettings
    {
        public float height;
        public float radius;
        public Vector3 center;

        // To control capsule value (to avoid empty capsule).
        public bool IsValid => height > 0f && radius > 0f;
    }

    [SerializeField]
    ColliderSettings colliderSettingSettings = new ColliderSettings
    {
        height = 2f,
        radius = 0.5f,
        center = Vector3.zero,
    };

    public ColliderSettings ColliderSetting => colliderSettingSettings;
}
