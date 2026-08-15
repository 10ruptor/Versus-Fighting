
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
    public Hitbox hitbox;
    public Vector3 hitboxPosition;
    public float hitboxRadius;
    public AttackTypes AttackType;
}