using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private CharacterStatData characterStatData;
    public CharacterStatData CharacterStatData => characterStatData;
    
    [Header("Visuals")]
    [SerializeField] CharacterAnimatorController characterAnimatorController;
    [SerializeField] VFXManager vfxManager;
    public VFXManager VFXManager => vfxManager;
    public CharacterAnimatorController CharacterAnimatorController => characterAnimatorController;
}
