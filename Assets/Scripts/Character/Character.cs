using UnityEngine;
using System.Collections.Generic;
using System.Linq;
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
    
    [Header("Attacks")]
    [SerializeField] private List<AttackData> AttackStatList = new List<AttackData>();
    
    public Dictionary<AttackTypes, AttackData> attackLookup = new Dictionary<AttackTypes, AttackData>();
    
    private void Awake()
    {
        attackLookup = AttackStatList.ToDictionary(x => x.AttackType);
    }
}
