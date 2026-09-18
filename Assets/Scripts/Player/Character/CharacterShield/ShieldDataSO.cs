using UnityEngine;

/// <summary>
/// Donnees de gameplay du bouclier. Comme le CharacterStatData, cet asset appartient au
/// Character : chaque Yokai peut avoir un bouclier plus resistant, plus grand, ou dont la
/// fenetre de contre est plus permissive, sans une ligne de code specifique.
/// </summary>
[CreateAssetMenu(fileName = "ShieldData", menuName = "Versus Fighting/Character/Shield Data")]
public class ShieldDataSO : ScriptableObject
{
    [Header("Durabilite")]
    [Tooltip("Durabilite a plein.")]
    public float maxDurability = 50f;

    [Tooltip("Durabilite perdue par seconde tant que le bouclier est leve.")]
    public float durabilityDrainPerSecond = 6f;

    [Tooltip("Durabilite perdue par point de degats bloque.")]
    public float durabilityCostPerDamage = 1f;

    [Tooltip("Durabilite regagnee par seconde, bouclier baisse.")]
    public float durabilityRegenPerSecond = 10f;

    [Tooltip("Delai apres la derniere sollicitation avant que la regeneration ne reprenne.")]
    public float durabilityRegenDelay = 0.5f;

    [Header("Taille")]
    [Tooltip("Taille du bouclier a durabilite nulle, en proportion de sa taille dans le prefab. 1 = le bouclier ne retrecit pas.")]
    [Range(0f, 1f)] public float minSizeRatio = 0.4f;

    [Header("Blocage")]
    [Tooltip("Part des degats de l'attaque encaissee malgre le blocage. 0 = le bouclier protege totalement.")]
    [Range(0f, 1f)] public float chipDamageRatio = 0f;

    [Tooltip("Vitesse du recul applique au defenseur sur un coup bloque (unites/seconde). Volontairement faible : ce n'est pas une ejection, le joueur n'est pas Knocked.")]
    public float blockPushbackSpeed = 3f;

    [Tooltip("Duree du recul, qui sert aussi de blockstun : pendant ce temps le bouclier absorbe sans nouvel effet, ce qui evite qu'une meme attaque soit comptee plusieurs fois.")]
    public float blockPushbackDuration = 0.12f;

    [Header("Contre (counter parry)")]
    [Tooltip("Fenetre, en secondes, entre l'appui bouclier et le HIT pour transformer le blocage en contre. Elle appartient au bouclier, pas au buffer d'input.")]
    public float counterParryWindow = 0.12f;

    [Tooltip("Attaque renvoyee a l'attaquant sur un contre reussi : degats et ejection se configurent comme n'importe quelle attaque.")]
    public AttackDataSO counterAttackData;

    [Tooltip("Durabilite consommee par un contre reussi. 0 = le contre est gratuit.")]
    public float counterDurabilityCost = 0f;

    [Header("Casse")]
    [Tooltip("Duree pendant laquelle le bouclier est inutilisable apres avoir ete casse.")]
    public float brokenDuration = 1.5f;

    [Tooltip("Ratio de durabilite minimum pour pouvoir relever le bouclier : evite de rouvrir un bouclier residuel juste apres une casse.")]
    [Range(0f, 1f)] public float minDurabilityRatioToRaise = 0.2f;

    public float DurabilityRatioOf(float durability)
    {
        return maxDurability > 0f ? Mathf.Clamp01(durability / maxDurability) : 0f;
    }
}
