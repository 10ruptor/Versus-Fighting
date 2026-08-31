using UnityEngine;

[CreateAssetMenu(fileName = "ElementalAttackDataSO", menuName = "Scriptable Objects/ElementalAttackDataSO")]
public class ElementalAttackDataSO : AttackDataSO
{
        [Header("Element")]
        [SerializeField] private ElementTypes elementType = ElementTypes.Fire;

        [Tooltip("Damage added on top of the base damage when the element lands.")]
        public float ElementalDamage;

        [Tooltip("Multiplies the knockback speed computed from the victim's percent. 1 = unchanged.")]
        public float ElementalKnockbackMultiplier = 1f;

        [Header("Status Effect")]
        [Tooltip("Chance for the status effect to be applied on hit. 0 = never, 1 = always.")]
        [Range(0f, 1f)] public float StatusChance = 1f;

        [Tooltip("Seconds the status effect stays on the victim. 0 = no status effect.")]
        public float StatusDuration;

        [Tooltip("Percent added to the victim every second while the status effect lasts (burn, shock...).")]
        public float StatusDamagePerSecond;

        [Header("Feedback")]
        [Tooltip("Particle prefab played at the hit position when the element lands.")]
        public ParticleSystem HitVFX;

        public override ElementTypes ElementType => elementType;
        public override float TotalDamage => Damage + ElementalDamage;
        public override float KnockbackMultiplier => ElementalKnockbackMultiplier;

        public bool HasStatusEffect => StatusDuration > 0f && StatusChance > 0f;

        // Rolled once per hit: tells whether this hit applies its status effect.
        public bool RollStatusEffect()
        {
                return HasStatusEffect && UnityEngine.Random.value <= StatusChance;
        }
}
