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

        // Overridden by ElementalAttackDataSO, so consumers can read any attack
        // without knowing whether it carries an element.
        public virtual ElementTypes ElementType => ElementTypes.None;
        public bool IsElemental => ElementType != ElementTypes.None;

        public virtual float TotalDamage => Damage;

        // La puissance d'ejection n'est plus une valeur statique : elle se calcule
        // au moment du coup a partir du % de la victime (voir KnockbackController).
        // Une attaque elementaire module donc le resultat par un multiplicateur,
        // au lieu de surcharger une puissance fixe.
        public virtual float KnockbackMultiplier => 1f;
}
