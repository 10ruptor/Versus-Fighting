using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Outil editeur de generation des Hurtbox sur les bones d'un SkinnedMeshRenderer.
///
/// Toute la logique vit ici, dans un dossier Editor : elle est donc exclue du build et
/// n'ajoute rien au runtime. Le composant HurtboxAutoGenerator ne porte que la configuration.
///
/// Fonctionnement : pour chaque bone du renderer, on cherche l'os enfant le plus proche pour
/// en deduire la direction et la longueur de l'os, puis on instancie le prefab de Hurtbox
/// (lien au prefab conserve), on l'oriente le long de l'axe os -> enfant et on dimensionne
/// son collider. Les os terminaux (mains, tete, pieds), qui n'ont pas d'enfant, recoivent une
/// taille par defaut.
/// </summary>
[CustomEditor(typeof(HurtboxAutoGenerator))]
public class HurtboxAutoGeneratorEditor : Editor
{
    private const string GenerateUndoLabel = "Generation des Hurtbox";
    private const string RemoveUndoLabel = "Suppression des Hurtbox generees";

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
            EditorGUILayout.LabelField("Bones detectes", renderer.bones.Length.ToString());
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

    #region Generation

    private static void Generate(HurtboxAutoGenerator generator, SkinnedMeshRenderer renderer)
    {
        Transform[] bones = renderer.bones;
        // La generation ne considere que les os reellement skinnes : les helpers, IK et autres
        // transforms intermediaires du rig n'ont pas de peau attachee, donc pas de volume a couvrir.
        HashSet<Transform> boneSet = new HashSet<Transform>(bones);

        Undo.SetCurrentGroupName(GenerateUndoLabel);
        int undoGroup = Undo.GetCurrentGroup();

        int createdCount = 0;
        int ignoredCount = 0;
        int alreadyPresentCount = 0;

        foreach (Transform bone in bones)
        {
            if (bone == null)
                continue;

            if (generator.IsBoneIgnored(bone.name))
            {
                ignoredCount++;
                continue;
            }

            if (HasHurtbox(bone, boneSet))
            {
                alreadyPresentCount++;
                continue;
            }

            if (CreateHurtbox(generator, bone, boneSet))
                createdCount++;
        }

        // Tout est regroupe sous une seule entree d'historique : un Ctrl+Z annule la generation
        // entiere, pas une hurtbox a la fois.
        Undo.CollapseUndoOperations(undoGroup);

        Debug.Log($"[HurtboxAutoGenerator] {createdCount} Hurtbox generees sur {generator.name} " +
                  $"({alreadyPresentCount} os deja equipes, {ignoredCount} ignores).", generator);
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

    private static bool CreateHurtbox(HurtboxAutoGenerator generator, Transform bone, HashSet<Transform> boneSet)
    {
        Transform childBone = FindNearestChildBone(bone, boneSet);

        // Tout est calcule dans le repere local de l'os : les dimensions du collider y sont
        // directement exprimables, quelle que soit l'echelle heritee du modele (un FBX importe
        // a 0.01 ne fausse donc pas les tailles).
        Vector3 localAxis = childBone != null ? bone.InverseTransformPoint(childBone.position) : Vector3.zero;
        float boneLength = localAxis.magnitude;
        bool isLeafBone = boneLength <= Mathf.Epsilon;

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(generator.HurtboxPrefab.gameObject, bone);

        if (instance == null)
        {
            Debug.LogError($"[HurtboxAutoGenerator] Instanciation du prefab impossible pour l'os {bone.name}.", bone);
            return false;
        }

        Undo.RegisterCreatedObjectUndo(instance, GenerateUndoLabel);

        instance.name = HurtboxAutoGenerator.GeneratedNamePrefix + bone.name;

        Transform instanceTransform = instance.transform;
        instanceTransform.localPosition = Vector3.zero;
        // L'axe Y local suit l'os : c'est aussi l'axe par defaut d'une CapsuleCollider, donc
        // orientation et dimensionnement parlent du meme repere.
        instanceTransform.localRotation = isLeafBone
            ? Quaternion.identity
            : Quaternion.FromToRotation(Vector3.up, localAxis.normalized);
        // L'echelle est neutralisee pour que les valeurs calculees plus bas soient celles
        // reellement visibles dans l'Inspecteur du collider.
        instanceTransform.localScale = Vector3.one;

        ResizeCollider(generator, instance, boneLength, isLeafBone);

        return true;
    }

    /// <summary>
    /// Os enfant le plus proche parmi les os du renderer. On descend a travers les transforms
    /// intermediaires non skinnes (pivots, attach points) et on s'arrete des qu'un vrai os est
    /// atteint : les os plus bas dans la chaine sont, eux, couverts par leur propre hurtbox.
    /// </summary>
    private static Transform FindNearestChildBone(Transform bone, HashSet<Transform> boneSet)
    {
        Transform nearestBone = null;
        float nearestDistance = float.MaxValue;

        Queue<Transform> pending = new Queue<Transform>();

        for (int i = 0; i < bone.childCount; i++)
            pending.Enqueue(bone.GetChild(i));

        while (pending.Count > 0)
        {
            Transform current = pending.Dequeue();

            if (boneSet.Contains(current))
            {
                float distance = Vector3.Distance(bone.position, current.position);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestBone = current;
                }

                continue;
            }

            for (int i = 0; i < current.childCount; i++)
                pending.Enqueue(current.GetChild(i));
        }

        return nearestBone;
    }

    /// <summary>
    /// Dimensionnement du collider instancie. Le volume est centre sur le milieu de l'os, sauf
    /// pour un os terminal qui n'a pas de longueur : il recoit alors une taille par defaut
    /// centree sur son pivot.
    /// </summary>
    private static void ResizeCollider(HurtboxAutoGenerator generator, GameObject instance, float boneLength, bool isLeafBone)
    {
        Collider collider = instance.GetComponent<Collider>();

        if (collider == null)
            return;

        float radius = isLeafBone
            ? Mathf.Max(generator.MinRadius, generator.LeafBoneSize * 0.5f)
            : Mathf.Max(generator.MinRadius, boneLength * generator.RadiusRatio);

        // Une capsule ne peut pas etre plus courte que ses deux hemispheres : on borne la
        // hauteur pour eviter un volume degenere quand lengthRatio est tres faible.
        float length = isLeafBone
            ? Mathf.Max(generator.LeafBoneSize, radius * 2f)
            : Mathf.Max(boneLength * generator.LengthRatio, radius * 2f);

        Vector3 center = isLeafBone ? Vector3.zero : Vector3.up * (boneLength * 0.5f);

        switch (collider)
        {
            case CapsuleCollider capsule:
                capsule.direction = 1; // Axe Y : celui aligne sur l'os par CreateHurtbox.
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
