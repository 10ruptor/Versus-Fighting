using UnityEngine;

[CreateAssetMenu(fileName = "AttackDataSO", menuName = "Scriptable Objects/AttackDataSO")]
public class AttackDataSO : ScriptableObject
{
        public float Damage;
        public AttackTypes AttackType;
        public float KnockbackPower;
        public Vector3 KnockBackDirection;
        
}
