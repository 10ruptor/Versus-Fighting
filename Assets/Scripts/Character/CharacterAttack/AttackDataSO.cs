using UnityEngine;

[CreateAssetMenu(fileName = "AttackDataSO", menuName = "Scriptable Objects/AttackDataSO")]
public class AttackDataSO : ScriptableObject
{
        public float Damage;
        public AttackTypes AttackType;

        [Header("Knockback")]
        [Tooltip("Angle d'ejection en degres, exprime pour un coup porte vers la droite (0 = horizontal, 90 = vertical). Mirrore automatiquement selon le cote touche.")]
        [Range(0f, 180f)] public float launchAngle = 45f;

        [Tooltip("Vitesse d'ejection appliquee a 0% de degats (unites/seconde).")]
        public float baseKnockback = 6f;

        [Tooltip("Vitesse d'ejection gagnee par point de % de degats de la victime.")]
        public float knockbackScaling = 0.08f;
}
