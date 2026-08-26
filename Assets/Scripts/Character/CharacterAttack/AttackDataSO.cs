using UnityEngine;

[CreateAssetMenu(fileName = "AttackDataSO", menuName = "Scriptable Objects/AttackDataSO")]
public class AttackDataSO : ScriptableObject
{
        public float Damage;
        public AttackTypes AttackType;
        public float KnockbackPower;
        public Vector3 KnockBackDirection;

        // Overridden by ElementalAttackDataSO, so consumers can read any attack
        // without knowing whether it carries an element.
        public virtual ElementTypes ElementType => ElementTypes.None;
        public bool IsElemental => ElementType != ElementTypes.None;

        public virtual float TotalDamage => Damage;
        public virtual float TotalKnockbackPower => KnockbackPower;
}
