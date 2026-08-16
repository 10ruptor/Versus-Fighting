
using UnityEngine;

public enum AttackTypes
{
    UpTilt,
    SideTilt,
    DownTilt,
    NeutralTilt,
        
    Nair,
    Fair,
    Bair,
    Dair
}

[CreateAssetMenu(fileName = "AttackStatSO", menuName = "Versus Fighting/AttackStatSO")]
public class AttackData : ScriptableObject
{
    //public float damage;
    public string AnimationTrigger;
    public AttackTypes AttackType;
}