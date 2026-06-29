using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackStatSO", menuName = "Versus Fighting/AttackStatSO")]
public class AttackData : ScriptableObject
{
    //public float damage;
    public string AnimationTrigger;
    public Hitbox hitbox;
    public Vector3 hitboxPosition;
    public float hitboxRadius;
}