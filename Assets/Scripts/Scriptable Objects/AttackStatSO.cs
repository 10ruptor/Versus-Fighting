using UnityEngine;

[CreateAssetMenu(fileName = "AttackStatSO", menuName = "Scriptable Objects/AttackStatSO")]
public class AttackStatSO : ScriptableObject
{
    public float damage;
    public string AnimationTrigger;
    public Hitbox hitbox;
}
