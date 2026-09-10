using UnityEngine;

/// <summary>
/// Ajuste le CapsuleCollider physique du PlayerGameplay sur le mesh du personnage.
///
/// Le collider reste porte par le PlayerGameplay (c'est lui qui porte le Rigidbody et la
/// presence physique du joueur, independamment du personnage charge) : ce composant ne fait
/// que lire les Renderer du Character instancie en enfant et en deduire les dimensions.
/// Un personnage plus grand ou plus large n'a donc rien a reconfigurer a la main.
///
/// La mesure est prise une fois, a l'instanciation du personnage. Elle n'est volontairement
/// pas reevaluee a chaque frame : le volume physique doit rester stable pendant qu'une
/// animation joue, sinon un coup de pied qui s'allonge deplacerait le personnage.
/// </summary>
[RequireComponent(typeof(CapsuleCollider))]
[DisallowMultipleComponent]
public class BodyColliderAutoFitter : MonoBehaviour
{
    /// <summary>
    /// D'ou vient le rayon de la capsule.
    /// </summary>
    public enum RadiusSource
    {
        /// <summary>Valeur saisie a la main. Le mesh ne pilote alors que la hauteur.</summary>
        Manual,

        /// <summary>Plus petite des deux demi-largeurs X/Z du mesh. C'est le defaut : sur un
        /// mesh en T-pose l'axe X vaut l'envergure des bras, l'axe Z l'epaisseur du buste,
        /// et c'est cette derniere qui donne un corps credible.</summary>
        MeshDepth,

        /// <summary>Plus grande des deux demi-largeurs X/Z. A reserver aux mesh dont la pose
        /// de reference a les bras le long du corps.</summary>
        MeshWidth
    }

    [Header("Declenchement")]
    [Tooltip("Ajuster automatiquement la capsule quand le PlayerGameplay instancie son personnage. " +
             "Decoche = la capsule garde les valeurs saisies dans le prefab.")]
    [SerializeField] private bool fitOnCharacterSpawn = true;

    [Header("Source")]
    [Tooltip("Optionnel : Renderer a mesurer. Si vide, tous les SkinnedMeshRenderer du personnage " +
             "sont pris en compte (a defaut, les MeshRenderer).")]
    [SerializeField] private Renderer explicitRenderer;

    [Header("Hauteur")]
    [Tooltip("Hauteur de la capsule = hauteur du mesh x ce ratio. 1 = exactement la taille du personnage.")]
    [SerializeField, Min(0f)] private float heightRatio = 1f;

    [Tooltip("Ajoute (ou retire, si negatif) des unites a la hauteur obtenue, apres application du ratio.")]
    [SerializeField] private float heightPadding = 0f;

    [Tooltip("Decale le bas de la capsule par rapport au bas du mesh. Negatif = la capsule " +
             "descend sous les pieds, positif = elle s'arrete au dessus.")]
    [SerializeField] private float bottomOffset = 0f;

    [Header("Rayon")]
    [Tooltip("MeshDepth est le defaut sur un rig humanoide : voir l'infobulle de l'enum.")]
    [SerializeField] private RadiusSource radiusSource = RadiusSource.MeshDepth;

    [Tooltip("Rayon = demi-largeur retenue x ce ratio. Ignore en mode Manual.")]
    [SerializeField, Min(0f)] private float radiusRatio = 1f;

    [Tooltip("Rayon utilise en mode Manual.")]
    [SerializeField, Min(0f)] private float manualRadius = 0.35f;

    [Tooltip("Rayon plancher, pour qu'un mesh tres fin ne produise pas une capsule inutilisable.")]
    [SerializeField, Min(0.001f)] private float minRadius = 0.05f;

    [Header("Centrage")]
    [Tooltip("Recentrer la capsule sur le mesh en X/Z. Laisser decoche est plus sur : la boite " +
             "englobante se decale des qu'un membre est tendu, et la capsule suivrait ce decalage.")]
    [SerializeField] private bool alignHorizontally = false;

    private CapsuleCollider capsule;

    /// <summary>
    /// Resolution paresseuse plutot qu'un Awake : l'ajustement doit aussi pouvoir etre declenche
    /// depuis l'Inspector hors Play Mode, ou aucun Awake n'a tourne.
    /// </summary>
    private CapsuleCollider Capsule
    {
        get
        {
            if (capsule == null)
                capsule = GetComponent<CapsuleCollider>();

            return capsule;
        }
    }

    /// <summary>
    /// Appele par PlayerGameplay juste apres l'instanciation du personnage. Le drapeau
    /// fitOnCharacterSpawn est teste ici plutot que chez l'appelant : c'est un reglage de ce
    /// composant, PlayerGameplay n'a pas a le connaitre.
    /// </summary>
    public void OnCharacterSpawned(Character character)
    {
        if (!fitOnCharacterSpawn || character == null)
            return;

        FitTo(character.gameObject);
    }

    /// <summary>
    /// Redimensionne la capsule pour englober les Renderer trouves sous characterRoot.
    /// Sans rien a mesurer, la capsule est laissee telle quelle : mieux vaut garder les valeurs
    /// du prefab qu'une capsule degeneree.
    /// </summary>
    public void FitTo(GameObject characterRoot)
    {
        if (characterRoot == null)
        {
            Debug.LogWarning($"{nameof(BodyColliderAutoFitter)} : aucun personnage a mesurer.", this);
            return;
        }

        if (!TryComputeLocalBounds(characterRoot, out Bounds bounds))
        {
            Debug.LogWarning($"{nameof(BodyColliderAutoFitter)} : aucun Renderer exploitable sous " +
                             $"{characterRoot.name}, la capsule est laissee inchangee.", this);
            return;
        }

#if UNITY_EDITOR
        // Hors Play Mode, sans cela le redimensionnement ne serait ni annulable ni sauvegarde.
        if (!Application.isPlaying)
            UnityEditor.Undo.RecordObject(Capsule, "Ajustement de la capsule du corps");
#endif

        float radius = Mathf.Max(ResolveRadius(bounds), minRadius);

        // Une capsule dont la hauteur est inferieure a son diametre est reduite par Unity a une
        // sphere : on borne nous memes pour que les valeurs de l'Inspector restent lisibles.
        float height = Mathf.Max(bounds.size.y * heightRatio + heightPadding, radius * 2f);

        Capsule.direction = 1; // axe Y
        Capsule.radius = radius;
        Capsule.height = height;
        Capsule.center = new Vector3(
            alignHorizontally ? bounds.center.x : 0f,
            bounds.min.y + bottomOffset + height * 0.5f,
            alignHorizontally ? bounds.center.z : 0f);

#if UNITY_EDITOR
        if (!Application.isPlaying)
            UnityEditor.EditorUtility.SetDirty(Capsule);
#endif
    }

    /// <summary>
    /// Ajuste la capsule sur le personnage deja present dans la hierarchie. Utile depuis
    /// l'Inspector en Play Mode, ou en edition si un Character a ete depose a la main sous le
    /// PlayerGameplay pour regler les ratios sans lancer la scene.
    /// </summary>
    [ContextMenu("Ajuster la capsule sur le personnage")]
    private void FitToCurrentCharacter()
    {
        Character character = GetComponentInChildren<Character>(includeInactive: true);

        if (character == null)
        {
            Debug.LogWarning($"{nameof(BodyColliderAutoFitter)} : aucun Character dans la hierarchie. " +
                             "En edition, deposez le prefab du personnage sous le PlayerGameplay.", this);
            return;
        }

        FitTo(character.gameObject);
    }

    private float ResolveRadius(Bounds bounds)
    {
        switch (radiusSource)
        {
            case RadiusSource.MeshDepth:
                return Mathf.Min(bounds.extents.x, bounds.extents.z) * radiusRatio;

            case RadiusSource.MeshWidth:
                return Mathf.Max(bounds.extents.x, bounds.extents.z) * radiusRatio;

            default:
                return manualRadius;
        }
    }

    /// <summary>
    /// Boite englobante des Renderer, exprimee dans le repere du PlayerGameplay.
    ///
    /// Renderer.bounds est en repere monde : ses huit coins sont ramenes en local puis
    /// re-englobes, ce qui reste correct quelle que soit l'orientation du personnage (le
    /// VisualOrientationController le retourne d'un demi tour selon le sens de marche).
    ///
    /// Sur un SkinnedMeshRenderer, Renderer.bounds derive du volume de culling calcule sur la
    /// pose de reference, pas sur la frame d'animation en cours : la mesure est donc stable
    /// d'une execution a l'autre, ce qui est exactement ce qu'on veut d'un volume physique.
    /// </summary>
    private bool TryComputeLocalBounds(GameObject characterRoot, out Bounds localBounds)
    {
        localBounds = new Bounds();

        Renderer[] renderers = ResolveRenderers(characterRoot);
        bool initialized = false;

        foreach (Renderer renderer in renderers)
        {
            if (renderer == null)
                continue;

            Bounds worldBounds = renderer.bounds;
            Vector3 center = worldBounds.center;
            Vector3 extents = worldBounds.extents;

            for (int corner = 0; corner < 8; corner++)
            {
                Vector3 worldCorner = center + new Vector3(
                    (corner & 1) == 0 ? -extents.x : extents.x,
                    (corner & 2) == 0 ? -extents.y : extents.y,
                    (corner & 4) == 0 ? -extents.z : extents.z);

                Vector3 localCorner = transform.InverseTransformPoint(worldCorner);

                if (!initialized)
                {
                    localBounds = new Bounds(localCorner, Vector3.zero);
                    initialized = true;
                }
                else
                {
                    localBounds.Encapsulate(localCorner);
                }
            }
        }

        return initialized;
    }

    /// <summary>
    /// Le Renderer explicite prime. Sinon on ne retient que les SkinnedMeshRenderer, qui portent
    /// le corps du personnage ; les MeshRenderer ne servent de repli que si le modele n'est pas
    /// skinne, car une arme ou un accessoire rigide fausserait la mesure.
    /// </summary>
    private Renderer[] ResolveRenderers(GameObject characterRoot)
    {
        if (explicitRenderer != null)
            return new[] { explicitRenderer };

        Renderer[] skinned = characterRoot.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);

        if (skinned.Length > 0)
            return skinned;

        return characterRoot.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
    }
}
