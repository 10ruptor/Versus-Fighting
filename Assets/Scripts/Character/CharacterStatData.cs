using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "Versus Fighting/Character Stats")]
public class CharacterStatData : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float dashSpeed = 7f;
    public int dashDurationFrames = 7;

    [Tooltip("Frames spent easing in from the current speed up to full dash speed (0 = instant).")]
    public int dashAccelerationFrames = 2;

    [Tooltip("Frames spent easing out of dash speed after the active phase, sliding into a stop or into the held walk speed.")]
    public int dashDecelerationFrames = 6;

    [Header("Jump - Ascent")]
    [Tooltip("Target height reached during the ascent phase (world units).")]
    public float jumpHeight = 2f;

    [Tooltip("Time to reach jumpHeight with ease-out ascent (velocity reaches 0 at the peak).")]
    public float jumpAscentDuration = 0.35f;

    public int maxAddJumpCount = 1;

    [Header("Jump - Descent")]
    [Tooltip("Gravity multiplier while falling. 1 = normal gravity, higher = faster fall.")]
    public float weight = 1f;

    [Header("Knockback")]
    [Tooltip("Duree de l'etat Knocked en secondes : temps pendant lequel les actions du joueur sont bloquees apres avoir ete touche. Volontairement independante de l'attaque recue.")]
    public float knockedDuration = 0.5f;

    [Header("Fast Fall")]
    [Tooltip("Multiplies descent acceleration while holding FastFall (only during descent).")]
    public float fastFallAccelerationMultiplier = 2f;
}
