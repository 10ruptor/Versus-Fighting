using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Outil editeur de generation des Hurtbox sur les bones d'un SkinnedMeshRenderer.
///
/// Toute la logique vit ici, dans un dossier Editor : elle est donc exclue du build et
/// n'ajoute rien au runtime. Le composant HurtboxAutoGenerator ne porte que la configuration.
///
/// Principe : les os sont partages en trois categories par HurtboxAutoGenerator.
///  - ecartes : ni Hurtbox, ni couverture (cape, helpers) ;
///  - porteurs : ils recoivent une Hurtbox ;
///  - absorbes : tous les autres, couverts par le porteur le plus proche au dessus d'eux.
/// La Hurtbox d'un porteur est orientee et dimensionnee pour englober ses os absorbes : celle
/// de la main couvre les doigts, celle du bassin couvre le bas du buste. Le mode EveryBone ne
/// fait que declarer tous les os porteurs, sans autre changement d'algorithme.
/// </summary>
[CustomEditor(typeof(HurtboxAutoGenerator))]
public class HurtboxAutoGeneratorEditor : Editor
{
    private const string GenerateUndoLabel = "Generation des Hurtbox";
    private const string RemoveUndoLabel = "Suppression des Hurtbox generees";

    /// <summary>
    /// Ce qu'une Hurtbox doit couvrir, exprime dans le repere de son os porteur.
    /// </summary>
    private struct BoneCoverage
    {
        /// <summary>Direction (normalisee) le long de laquelle la Hurtbox s'etend.</summary>
        public Vector3 LocalAxis;

        /// <summary>Portee le long de cet axe, marge de bout de chaine comprise.</summary>
        public float Length;

        /// <summary>Ecart maximal des os absorbes a l'axe : rayon minimal pour les englober.</summary>
        public float LateralExtent;

        /// <summary>Aucun os a couvrir sous ce porteur : il recoit la taille par defaut.</summary>
        public bool IsIsolated;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HurtboxAutoGenerator generator = (HurtboxAutoGenerator)target;
        SkinnedMeshRenderer renderer = generator.ResolveRenderer();

        EditorGUILayout.Space();

        bool canGenerate = DrawValidation(generator, renderer);

        using (new EditorGUI.DisabledScope(!canGenerate))
        {
            if (GUILayout.Button("Generer les Hurtbox sur chaque bone", GUILayout.Height(28f)))
                Generate(generator, renderer);
        }

        if (GUILayout.Button("Supprimer les Hurtbox generees"))
            RemoveGenerated(generator);
    }

    #region Inspecteur

    /// <summary>
    /// Diagnostics affiches dans l'Inspecteur. Ils remplacent des Debug.LogError leves une fois
    /// le bouton clique : l'utilisateur voit ce qui manque avant d'essayer.
    /// </summary>
    private static bool DrawValidation(HurtboxAutoGenerator generator, SkinnedMeshRenderer renderer)
    {
        bool canGenerate = true;

        if (renderer == null)
        {
            EditorGUILayout.HelpBox(
                "Aucun SkinnedMeshRenderer trouve. Renseignez le champ, ou posez ce composant sur " +
                "un parent du modele skinne.", MessageType.Error);
            canGenerate = false;
        }
        else if (renderer.bones == null || renderer.bones.Length == 0)
        {
            EditorGUILayout.HelpBox(
                "Le SkinnedMeshRenderer n'expose aucun bone : le modele n'est probablement pas rigge.",
                MessageType.Error);
            canGenerate = false;
        }
        else
        {
            // Compte des porteurs : c'est le nombre de Hurtbox qui seront generees, et le seul
            // moyen de voir tout de suite qu'une liste d'os porteurs ne colle pas au rig.
            int carrierCount = CountCarrierBones(generator, renderer);

            EditorGUILayout.LabelField(
                "Hurtbox a generer",
                $"{carrierCount} (sur {renderer.bones.Length} os)");

            if (carrierCount == 0)
            {
                EditorGUILayout.HelpBox(
                    "Aucun os porteur reconnu : les noms de la liste ne correspondent a aucun os " +
                    "de ce rig. Ouvrez la hierarchie du modele pour relever les noms reels.",
                    MessageType.Error);
                canGenerate = false;
            }
        }

        Hurtbox prefab = generator.HurtboxPrefab;

        if (prefab == null)
        {
            EditorGUILayout.HelpBox("Aucun prefab de Hurtbox assigne.", MessageType.Error);
            return false;
        }

        if (!EditorUtility.IsPersistent(prefab))
        {
            EditorGUILayout.HelpBox(
                "Le champ ne pointe pas vers un prefab (asset) : le lien au prefab ne pourra pas " +
                "etre conserve sur les objets generes.", MessageType.Warning);
        }
        else if (prefab.transform.parent != null)
        {
            EditorGUILayout.HelpBox(
                "La Hurtbox assignee n'est pas la racine de son prefab. Assignez la racine pour " +
                "instancier le prefab complet.", MessageType.Warning);
        }

        Collider prefabCollider = prefab.GetComponent<Collider>();

        if (prefabCollider == null)
        {
            EditorGUILayout.HelpBox(
                "Le prefab de Hurtbox n'a pas de Collider : rien a dimensionner.", MessageType.Error);
            canGenerate = false;
        }
        else
        {
            // Hurtbox.OnTriggerEnter est le seul point d'entree des coups : sans trigger, les
            // hurtbox generees seront muettes.
            if (!prefabCollider.isTrigger)
            {
                EditorGUILayout.HelpBox(
                    "Le Collider du prefab n'est pas en Is Trigger : les Hurtbox ne detecteront " +
                    "aucune Hitbox.", MessageType.Warning);
            }

            if (!(prefabCollider is CapsuleCollider) && !(prefabCollider is BoxCollider) && !(prefabCollider is SphereCollider))
            {
                EditorGUILayout.HelpBox(
                    $"Collider de type {prefabCollider.GetType().Name} non gere : il sera instancie " +
                    "tel quel, sans redimensionnement.", MessageType.Warning);
            }
        }

        return canGenerate;
    }

    private static int CountCarrierBones(HurtboxAutoGenerator generator, SkinnedMeshRenderer renderer)
    {
        int count = 0;

        foreach (Transform bone in renderer.bones)
        {
            if (IsCarrier(generator, bone))
                count++;
        }

        return count;
    }

    #endregion

    #region Generation

    private static void Generate(HurtboxAutoGenerator generator, SkinnedMeshRenderer renderer)
    {
        Transform[] bones = renderer.bones;

        // La generation ne considere que les os reellement skinnes : les helpers, IK et autres
        // transforms intermediaires du rig n'ont pas de peau attachee, donc pas de volume a couvrir.
        HashSet<Transform> boneSet = new HashSet<Transform>();
        HashSet<Transform> carrierSet = new HashSet<Transform>();

        foreach (Transform bone in bones)
        {
            if (bone == null)
                continue;

            boneSet.Add(bone);

            if (IsCarrier(generator, bone))
                carrierSet.Add(bone);
        }

        Undo.SetCurrentGroupName(GenerateUndoLabel);
        int undoGroup = Undo.GetCurrentGroup();

        int createdCount = 0;
        int alreadyPresentCount = 0;
        List<string> isolatedCarriers = new List<string>();

        foreach (Transform carrier in bones)
        {
            if (carrier == null || !carrierSet.Contains(carrier))
                continue;

            if (HasHurtbox(carrier, boneSet))
            {
                alreadyPresentCount++;
                continue;
            }

            BoneCoverage coverage = ComputeCoverage(generator, carrier, boneSet, carrierSet);

            if (coverage.IsIsolated)
                isolatedCarriers.Add(carrier.name);

            if (CreateHurtbox(generator, carrier, coverage))
                createdCount++;
        }

        // Tout est regroupe sous une seule entree d'historique : un Ctrl+Z annule la generation
        // entiere, pas une hurtbox a la fois.
        Undo.CollapseUndoOperations(undoGroup);

        Debug.Log($"[HurtboxAutoGenerator] {createdCount} Hurtbox generees sur {generator.name} " +
                  $"({carrierSet.Count} os porteurs sur {boneSet.Count} os, " +
                  $"{alreadyPresentCount} deja equipes).", generator);

        // Un porteur isole passe inapercu dans la Scene view : c'est une petite boule a la
        // taille par defaut la ou on attendait un membre. Le cas est legitime (bout de chaine)
        // mais il trahit aussi une liste d'os ecartes trop large, d'ou l'avertissement nomme.
        if (isolatedCarriers.Count > 0)
        {
            Debug.LogWarning(
                $"[HurtboxAutoGenerator] {isolatedCarriers.Count} os porteurs n'avaient aucun os a " +
                $"couvrir et ont recu la taille par defaut ({generator.LeafBoneSize}) : " +
                string.Join(", ", isolatedCarriers) + ".", generator);
        }
    }

    private static bool IsCarrier(HurtboxAutoGenerator generator, Transform bone)
    {
        if (bone == null)
            return false;

        // Un os ecarte n'est jamais porteur, quel que soit le mode : c'est ce qui permet de
        // rendre une partie du personnage intouchable sans toucher a la liste des porteurs.
        return !generator.IsBoneIgnored(bone.name) && generator.IsCarrierBone(bone.name);
    }

    /// <summary>
    /// Un os est considere comme deja equipe s'il porte une Hurtbox, ou si l'un de ses enfants
    /// directs en porte une. Les enfants qui sont eux-memes des os sont exclus du test : leur
    /// propre hurtbox leur appartient et ne doit pas faire passer le parent pour equipe.
    /// </summary>
    private static bool HasHurtbox(Transform bone, HashSet<Transform> boneSet)
    {
        if (bone.GetComponent<Hurtbox>() != null)
            return true;

        for (int i = 0; i < bone.childCount; i++)
        {
            Transform child = bone.GetChild(i);

            if (boneSet.Contains(child))
                continue;

            if (child.GetComponent<Hurtbox>() != null)
                return true;
        }

        return false;
    }

    private static bool CreateHurtbox(HurtboxAutoGenerator generator, Transform carrier, BoneCoverage coverage)
    {
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(generator.HurtboxPrefab.gameObject, carrier);

        if (instance == null)
        {
            Debug.LogError($"[HurtboxAutoGenerator] Instanciation du prefab impossible pour l'os {carrier.name}.", carrier);
            return false;
        }

        Undo.RegisterCreatedObjectUndo(instance, GenerateUndoLabel);

        instance.name = HurtboxAutoGenerator.GeneratedNamePrefix + carrier.name;

        Transform instanceTransform = instance.transform;
        instanceTransform.localPosition = Vector3.zero;
        // L'axe Y local suit la zone couverte : c'est aussi l'axe par defaut d'une
        // CapsuleCollider, donc orientation et dimensionnement parlent du meme repere.
        instanceTransform.localRotation = coverage.IsIsolated
            ? Quaternion.identity
            : Quaternion.FromToRotation(Vector3.up, coverage.LocalAxis);
        // L'echelle est neutralisee pour que les valeurs calculees plus bas soient celles
        // reellement visibles dans l'Inspecteur du collider.
        instanceTransform.localScale = Vector3.one;

        ResizeCollider(generator, instance, coverage);

        return true;
    }

    /// <summary>
    /// Zone a couvrir pour un os porteur.
    ///
    /// On descend dans la hierarchie sous l'os en classant ce qu'on rencontre :
    ///  - os ecarte : sous-arbre abandonne, il ne doit rien influencer ;
    ///  - os porteur : frontiere. Il marque la fin de notre zone (c'est lui qui couvrira la
    ///    suite de la chaine), on ne descend pas plus bas ;
    ///  - os absorbe : il fait partie de notre zone ;
    ///  - bout de chaine non skinne (HeadTop_End, LeftHandIndex4...) : compte comme couvert si
    ///    useEndTransforms est actif, car il marque la fin reelle de la chair ;
    ///  - autre transform non skinne : simple relais, on le traverse sans le compter.
    ///
    /// L'axe vise l'os absorbe le plus lointain quand il en existe (la main pointe donc vers le
    /// bout des doigts), sinon la frontiere la plus proche (le bras pointe vers le coude, sans
    /// deborder sur l'avant-bras). La longueur est ensuite la projection maximale sur cet axe :
    /// une frontiere situee dans le prolongement de la zone l'etire, une frontiere partant dans
    /// une autre direction ne compte pas.
    ///
    /// Tout est calcule dans le repere local du porteur : les dimensions du collider y sont
    /// directement exprimables, quelle que soit l'echelle heritee du modele (un FBX importe a
    /// 0.01 ne fausse donc pas les tailles).
    /// </summary>
    private static BoneCoverage ComputeCoverage(HurtboxAutoGenerator generator, Transform carrier,
        HashSet<Transform> boneSet, HashSet<Transform> carrierSet)
    {
        // Points a couvrir : os absorbes et, le cas echeant, bouts de chaine non skinnes.
        List<Vector3> coveredPoints = new List<Vector3>();
        List<Vector3> boundaryPoints = new List<Vector3>();
        bool hasEndTransforms = false;

        Queue<Transform> pending = new Queue<Transform>();
        EnqueueChildren(carrier, pending);

        while (pending.Count > 0)
        {
            Transform current = pending.Dequeue();

            if (generator.IsBoneIgnored(current.name))
                continue;

            if (boneSet.Contains(current))
            {
                Vector3 localPoint = carrier.InverseTransformPoint(current.position);

                if (carrierSet.Contains(current))
                {
                    boundaryPoints.Add(localPoint);
                    continue;
                }

                coveredPoints.Add(localPoint);
            }
            else if (generator.UseEndTransforms && current.childCount == 0)
            {
                coveredPoints.Add(carrier.InverseTransformPoint(current.position));
                hasEndTransforms = true;
            }

            EnqueueChildren(current, pending);
        }

        BoneCoverage coverage = new BoneCoverage();

        // L'axe vise le point couvert le plus lointain ; a defaut, la frontiere la plus proche.
        Vector3 reference = coveredPoints.Count > 0
            ? FindFarthest(coveredPoints)
            : FindNearest(boundaryPoints);

        if (reference.sqrMagnitude <= Mathf.Epsilon)
        {
            // Os isole : bout de chaine, ou porteur dont tous les enfants sont ecartes.
            coverage.IsIsolated = true;
            return coverage;
        }

        coverage.LocalAxis = reference.normalized;
        float coveredReach = MaxProjection(coveredPoints, coverage.LocalAxis);
        float boundaryReach = MaxProjection(boundaryPoints, coverage.LocalAxis);

        coverage.Length = Mathf.Max(coveredReach, boundaryReach);

        // Le dernier os d'une chaine n'est pas le bout de la chair : la derniere phalange
        // s'arrete avant le bout du doigt. On prolonge donc d'une demi-taille par defaut, mais
        // seulement quand la zone s'arrete vraiment la : ni si le rig fournit ses propres
        // marqueurs de fin (la portee est deja juste), ni si une frontiere porte deja plus loin.
        if (!hasEndTransforms && coveredPoints.Count > 0 && coveredReach >= boundaryReach)
            coverage.Length += generator.LeafBoneSize * 0.5f;

        // Les points couverts doivent etre englobes : ils n'ont pas de hurtbox a eux. Les frontieres
        // comptent elles aussi, mais en largeur seulement : c'est l'ecartement des bras et des
        // cuisses qui donne au buste et au bassin leur vraie epaisseur, qu'aucun ratio applique
        // a une longueur d'os ne saurait deviner.
        coverage.LateralExtent = Mathf.Max(
            ComputeLateralExtent(coveredPoints, coverage.LocalAxis),
            ComputeLateralExtent(boundaryPoints, coverage.LocalAxis));

        return coverage;
    }

    private static void EnqueueChildren(Transform parent, Queue<Transform> pending)
    {
        for (int i = 0; i < parent.childCount; i++)
            pending.Enqueue(parent.GetChild(i));
    }

    private static Vector3 FindFarthest(List<Vector3> points)
    {
        Vector3 farthest = Vector3.zero;
        float farthestDistance = 0f;

        foreach (Vector3 point in points)
        {
            float distance = point.sqrMagnitude;

            if (distance > farthestDistance)
            {
                farthestDistance = distance;
                farthest = point;
            }
        }

        return farthest;
    }

    private static Vector3 FindNearest(List<Vector3> points)
    {
        Vector3 nearest = Vector3.zero;
        float nearestDistance = float.MaxValue;

        foreach (Vector3 point in points)
        {
            float distance = point.sqrMagnitude;

            if (distance > Mathf.Epsilon && distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = point;
            }
        }

        return nearest;
    }

    /// <summary>
    /// Portee maximale le long de l'axe. Les points situes derriere le pivot donnent une
    /// projection negative et ne comptent pas : ils appartiennent a une autre branche du rig.
    /// </summary>
    private static float MaxProjection(List<Vector3> points, Vector3 axis)
    {
        float maxProjection = 0f;

        foreach (Vector3 point in points)
            maxProjection = Mathf.Max(maxProjection, Vector3.Dot(point, axis));

        return maxProjection;
    }

    /// <summary>
    /// Distance maximale des points a l'axe : c'est l'epaisseur minimale sous laquelle la
    /// hurtbox ne les couvrirait pas (doigts ecartes, orteils, largeur d'epaules).
    /// </summary>
    private static float ComputeLateralExtent(List<Vector3> points, Vector3 axis)
    {
        float extent = 0f;

        foreach (Vector3 point in points)
        {
            Vector3 projection = axis * Vector3.Dot(point, axis);
            extent = Mathf.Max(extent, (point - projection).magnitude);
        }

        return extent;
    }

    /// <summary>
    /// Dimensionnement du collider instancie. Le volume est centre sur le milieu de la zone
    /// couverte, sauf pour un os isole qui n'a pas de longueur : il recoit alors la taille par
    /// defaut, centree sur son pivot.
    /// </summary>
    private static void ResizeCollider(HurtboxAutoGenerator generator, GameObject instance, BoneCoverage coverage)
    {
        Collider collider = instance.GetComponent<Collider>();

        if (collider == null)
            return;

        float radius;
        float length;

        if (coverage.IsIsolated)
        {
            radius = Mathf.Max(generator.MinRadius, generator.LeafBoneSize * 0.5f);
            length = Mathf.Max(generator.LeafBoneSize, radius * 2f);
        }
        else
        {
            // Le ratio donne l'epaisseur "naturelle" de l'os ; l'ecart lateral des os absorbes
            // l'emporte quand la zone s'elargit (une main est plus large que longue).
            radius = Mathf.Max(generator.MinRadius, coverage.Length * generator.RadiusRatio);
            radius = Mathf.Max(radius, coverage.LateralExtent);

            // Une capsule ne peut pas etre plus courte que ses deux hemispheres : on borne la
            // hauteur pour eviter un volume degenere quand lengthRatio est tres faible.
            length = Mathf.Max(coverage.Length * generator.LengthRatio, radius * 2f);
        }

        Vector3 center = coverage.IsIsolated ? Vector3.zero : Vector3.up * (coverage.Length * 0.5f);

        switch (collider)
        {
            case CapsuleCollider capsule:
                capsule.direction = 1; // Axe Y : celui aligne sur la zone couverte par CreateHurtbox.
                capsule.radius = radius;
                capsule.height = length;
                capsule.center = center;
                break;

            case BoxCollider box:
                box.size = new Vector3(radius * 2f, length, radius * 2f);
                box.center = center;
                break;

            case SphereCollider sphere:
                sphere.radius = radius;
                sphere.center = center;
                break;

            default:
                // Type non gere (MeshCollider, etc.) : l'instance reste telle que le prefab la
                // definit, l'avertissement a deja ete affiche dans l'Inspecteur.
                break;
        }
    }

    #endregion

    #region Suppression

    private static void RemoveGenerated(HurtboxAutoGenerator generator)
    {
        List<GameObject> generatedHurtboxes = new List<GameObject>();

        foreach (Hurtbox hurtbox in generator.GetComponentsInChildren<Hurtbox>(includeInactive: true))
        {
            // Filtre par prefixe : les hurtbox posees a la main (corps, arme...) survivent.
            if (hurtbox.name.StartsWith(HurtboxAutoGenerator.GeneratedNamePrefix))
                generatedHurtboxes.Add(hurtbox.gameObject);
        }

        if (generatedHurtboxes.Count == 0)
        {
            Debug.Log($"[HurtboxAutoGenerator] Aucune Hurtbox generee a supprimer sur {generator.name}.", generator);
            return;
        }

        bool confirmed = EditorUtility.DisplayDialog(
            "Supprimer les Hurtbox generees",
            $"{generatedHurtboxes.Count} Hurtbox nommees \"{HurtboxAutoGenerator.GeneratedNamePrefix}...\" " +
            "vont etre supprimees. Les Hurtbox posees a la main sont conservees.",
            "Supprimer",
            "Annuler");

        if (!confirmed)
            return;

        Undo.SetCurrentGroupName(RemoveUndoLabel);
        int undoGroup = Undo.GetCurrentGroup();

        foreach (GameObject hurtboxObject in generatedHurtboxes)
            Undo.DestroyObjectImmediate(hurtboxObject);

        Undo.CollapseUndoOperations(undoGroup);

        Debug.Log($"[HurtboxAutoGenerator] {generatedHurtboxes.Count} Hurtbox supprimees sur {generator.name}.", generator);
    }

    #endregion
}
