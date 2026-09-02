using System.Collections.Generic;
using UnityEngine;

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
    [Tooltip("Prefab de Hurtbox instancie sur chaque bone. Le lien au prefab est conserve.")]
    [SerializeField] private Hurtbox hurtboxPrefab;

    [Tooltip("Optionnel : si vide, le premier SkinnedMeshRenderer trouve dans les enfants est utilise.")]
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    [Header("Dimensionnement")]
    [Tooltip("Rayon de la hurtbox = longueur de l'os x ce ratio.")]
    [SerializeField, Min(0f)] private float radiusRatio = 0.22f;

    [Tooltip("Longueur de la hurtbox = longueur de l'os x ce ratio. 1 = l'os exactement.")]
    [SerializeField, Min(0f)] private float lengthRatio = 1f;

    [Tooltip("Rayon plancher, pour que les tout petits os (doigts, machoire) restent touchables.")]
    [SerializeField, Min(0f)] private float minRadius = 0.02f;

    [Tooltip("Taille utilisee pour les os terminaux (mains, tete, pieds) : ils n'ont pas d'os " +
             "enfant, donc pas de longueur mesurable.")]
    [SerializeField, Min(0f)] private float leafBoneSize = 0.12f;

    [Header("Filtrage")]
    [Tooltip("Noms d'os a ignorer (root, IK, helpers, os de cape...).")]
    [SerializeField] private List<string> ignoredBoneNames = new List<string>();

    [Tooltip("Actif : un os est ignore si son nom contient l'une des entrees. Inactif : le nom " +
             "doit correspondre exactement.")]
    [SerializeField] private bool ignoredNamesUsePartialMatch = true;

    public Hurtbox HurtboxPrefab => hurtboxPrefab;
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
    /// Regle de filtrage des os. Elle vit ici, avec les donnees qu'elle interprete, plutot que
    /// dans l'editeur : changer la semantique du filtre ne demande alors de toucher qu'un fichier.
    /// </summary>
    public bool IsBoneIgnored(string boneName)
    {
        if (string.IsNullOrEmpty(boneName) || ignoredBoneNames == null)
            return false;

        foreach (string ignoredName in ignoredBoneNames)
        {
            if (string.IsNullOrEmpty(ignoredName))
                continue;

            bool matches = ignoredNamesUsePartialMatch
                ? boneName.IndexOf(ignoredName, System.StringComparison.OrdinalIgnoreCase) >= 0
                : string.Equals(boneName, ignoredName, System.StringComparison.OrdinalIgnoreCase);

            if (matches)
                return true;
        }

        return false;
    }
}
