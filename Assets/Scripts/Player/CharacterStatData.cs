using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "Versus Fighting/Character Stats")]
public class CharacterStatData : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 6f;

    [Header("Jump - Ascent")]
    [Tooltip("Target height reached during the ascent phase (world units).")]
    public float jumpHeight = 2f;

    [Tooltip("Time to reach jumpHeight with ease-out ascent (velocity reaches 0 at the peak).")]
    public float jumpAscentDuration = 0.35f;

    public int maxAddJumpCount = 1;

    [Header("Jump - Descent")]
    [Tooltip("Gravity multiplier while falling. 1 = normal gravity, higher = faster fall.")]
    public float weight = 1f;

    [Header("Fast Fall")]
    [Tooltip("Multiplies descent acceleration while holding FastFall (only during descent).")]
    public float fastFallAccelerationMultiplier = 2f;
}
