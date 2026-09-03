using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Granularite de la generation : combien de Hurtbox pour combien d'os.
/// </summary>
public enum HurtboxBoneSelectionMode
{
    /// <summary>Une Hurtbox par os skinne. Couverture maximale, mais une soixantaine de
    /// volumes sur un rig humanoide standard.</summary>
    EveryBone,

    /// <summary>Une Hurtbox par os porteur (voir carrierBoneNames). Les os intermediaires
    /// sont absorbes : la Hurtbox du porteur s'etend jusqu'a les englober, celle de la main
    /// couvre donc les doigts. Une dizaine de volumes au lieu d'une soixantaine.</summary>
    CarrierBonesOnly
}

/// <summary>
/// Comment une entree de liste est comparee au nom d'un os.
/// </summary>
public enum BoneNameMatch
{
    /// <summary>Nom exact, un eventuel prefixe de namespace mis a part : l'entree "LeftHand"
    /// reconnait "mixamorig:LeftHand", mais pas "mixamorig:LeftHandIndex1".</summary>
    ExactName,

    /// <summary>Le nom contient l'entree. Pratique pour ecarter toute une famille d'os
    /// ("cape" attrape cape_01, cape_02...), a manier avec prudence pour les porteurs : une
    /// entree "LeftHand" y attraperait aussi chaque phalange.</summary>
    ContainsName
}

/// <summary>
/// Configuration de la generation automatique des Hurtbox sur les bones d'un modele skinne.
///
/// Ce composant ne contient que des donnees : toute la logique de generation vit dans
/// HurtboxAutoGeneratorEditor (Assets/Editor), qui n'est pas compile dans un build. Le
/// composant reste donc inerte au runtime, il ne sert qu'a memoriser les reglages dans le
/// prefab du personnage pour pouvoir relancer la generation plus tard sans tout ressaisir.
///
/// A poser sur le parent du modele 3D (ex. CharacterModel), au dessus du SkinnedMeshRenderer.
/// </summary>
[DisallowMultipleComponent]
public class HurtboxAutoGenerator : MonoBehaviour
{
    /// <summary>
    /// Prefixe des GameObjects generes. Il sert aussi de marqueur au bouton de suppression :
    /// une Hurtbox posee a la main, nommee autrement, ne sera jamais detruite par l'outil.
    /// </summary>
    public const string GeneratedNamePrefix = "Hurtbox_";

    [Header("Sources")]
    [Tooltip("Prefab de Hurtbox instancie sur chaque os retenu. Le lien au prefab est conserve.")]
    [SerializeField] private Hurtbox hurtboxPrefab;

    [Tooltip("Optionnel : si vide, le premier SkinnedMeshRenderer trouve dans les enfants est utilise.")]
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    [Header("Granularite")]
    [Tooltip("EveryBone : une Hurtbox par os. CarrierBonesOnly : une Hurtbox par os porteur, " +
             "qui englobe les os absorbes en dessous de lui.")]
    [SerializeField] private HurtboxBoneSelectionMode selectionMode = HurtboxBoneSelectionMode.CarrierBonesOnly;

    [Tooltip("Os qui recoivent une Hurtbox en mode CarrierBonesOnly. Les valeurs par defaut " +
             "correspondent a un rig humanoide type Mixamo ; adaptez-les a votre rig.")]
    [SerializeField]
    private List<string> carrierBoneNames = new List<string>
    {
        "Hips",
        "Spine2",
        "Head",
        "LeftArm",
        "RightArm",
        "LeftForeArm",
        "RightForeArm",
        "LeftHand",
        "RightHand",
        "LeftUpLeg",
        "RightUpLeg",
        "LeftLeg",
        "RightLeg",
        "LeftFoot",
        "RightFoot"
    };

    [Tooltip("Comparaison des noms de la liste ci-dessus. ExactName est le bon choix par " +
             "defaut : ContainsName ferait de chaque phalange un os porteur.")]
    [SerializeField] private BoneNameMatch carrierNameMatch = BoneNameMatch.ExactName;

    [Tooltip("Compter les bouts de chaine non skinnes (HeadTop_End, LeftHandIndex4...) dans la " +
             "zone couverte. Ils ne portent pas de peau mais marquent ou la chair s'arrete : " +
             "sans eux, la hurtbox de la tete se limite a la base du crane.")]
    [SerializeField] private bool useEndTransforms = true;

    [Header("Dimensionnement")]
    [Tooltip("Rayon de la hurtbox = longueur couverte x ce ratio. Le rayon est de toute facon " +
             "elargi si besoin pour englober les os absorbes.")]
    [SerializeField, Min(0f)] private float radiusRatio = 0.22f;

    [Tooltip("Longueur de la hurtbox = longueur couverte x ce ratio. 1 = exactement la portion " +
             "d'os couverte.")]
    [SerializeField, Min(0f)] private float lengthRatio = 1f;

    [Tooltip("Rayon plancher, pour que les tout petits os restent touchables.")]
    [SerializeField, Min(0f)] private float minRadius = 0.02f;

    [Tooltip("Taille utilisee pour un os isole (aucun os a couvrir sous lui). Sert aussi de " +
             "marge au bout d'une chaine absorbee : la derniere phalange n'est pas le bout du doigt.")]
    [SerializeField, Min(0f)] private float leafBoneSize = 0.12f;

    [Header("Filtrage")]
    [Tooltip("Os totalement ecartes, eux et leurs descendants : ni Hurtbox, ni absorption. " +
             "A reserver a ce qui ne doit pas etre touchable (cape, os IK, helpers).")]
    [SerializeField] private List<string> ignoredBoneNames = new List<string>();

    [Tooltip("Comparaison des noms de la liste ci-dessus. ContainsName permet d'ecarter toute " +
             "une famille d'os d'un coup.")]
    [SerializeField] private BoneNameMatch ignoredNameMatch = BoneNameMatch.ContainsName;

    public Hurtbox HurtboxPrefab => hurtboxPrefab;
    public HurtboxBoneSelectionMode SelectionMode => selectionMode;
    public bool UseEndTransforms => useEndTransforms;
    public float RadiusRatio => radiusRatio;
    public float LengthRatio => lengthRatio;
    public float MinRadius => minRadius;
    public float LeafBoneSize => leafBoneSize;

    /// <summary>
    /// Le renderer explicite prime ; sinon on retombe sur le premier SkinnedMeshRenderer des
    /// enfants, ce qui couvre le cas courant d'un composant pose juste au dessus du modele.
    /// Resolu a la demande : cette methode est appelee depuis l'editeur, hors play mode, ou
    /// aucun Awake n'a tourne.
    /// </summary>
    public SkinnedMeshRenderer ResolveRenderer()
    {
        if (skinnedMeshRenderer != null)
            return skinnedMeshRenderer;

        return GetComponentInChildren<SkinnedMeshRenderer>(includeInactive: true);
    }

    /// <summary>
    /// Os ecarte : il ne recoit pas de Hurtbox et n'est couvert par aucune autre. C'est la
    /// seule maniere de rendre une partie du personnage intouchable.
    /// </summary>
    public bool IsBoneIgnored(string boneName)
    {
        return MatchesAny(boneName, ignoredBoneNames, ignoredNameMatch);
    }

    /// <summary>
    /// Os porteur : il recoit une Hurtbox. Les os ni porteurs ni ecartes sont absorbes par le
    /// porteur le plus proche au dessus d'eux.
    ///
    /// En mode EveryBone tout os non ecarte est porteur, ce qui ramene la generation au cas
    /// "une Hurtbox par os" sans code separe : seule cette regle change entre les deux modes.
    /// </summary>
    public bool IsCarrierBone(string boneName)
    {
        if (selectionMode == HurtboxBoneSelectionMode.EveryBone)
            return true;

        return MatchesAny(boneName, carrierBoneNames, carrierNameMatch);
    }

    /// <summary>
    /// Les regles de filtrage vivent ici, avec les donnees qu'elles interpretent, plutot que
    /// dans l'editeur : changer leur semantique ne demande de toucher qu'un fichier.
    /// </summary>
    private static bool MatchesAny(string boneName, List<string> entries, BoneNameMatch match)
    {
        if (string.IsNullOrEmpty(boneName) || entries == null)
            return false;

        string bone = StripNamespace(boneName);

        foreach (string entry in entries)
        {
            if (string.IsNullOrEmpty(entry))
                continue;

            string candidate = StripNamespace(entry);

            bool matches = match == BoneNameMatch.ContainsName
                ? bone.IndexOf(candidate, System.StringComparison.OrdinalIgnoreCase) >= 0
                : string.Equals(bone, candidate, System.StringComparison.OrdinalIgnoreCase);

            if (matches)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Retire le prefixe de namespace des rigs exportes depuis un DCC ("mixamorig:LeftHand"
    /// devient "LeftHand"), pour que les listes restent lisibles et portables d'un rig a l'autre.
    /// </summary>
    private static string StripNamespace(string boneName)
    {
        int separatorIndex = boneName.LastIndexOf(':');

        return separatorIndex >= 0 ? boneName.Substring(separatorIndex + 1) : boneName;
    }
}
