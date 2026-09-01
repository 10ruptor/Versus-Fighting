using UnityEngine;

/// <summary>
/// Dessin des gizmos de debug pour un Collider, quel que soit son type concret.
/// Utilitaire purement visuel, partage par Hitbox et Hurtbox : il evite de dupliquer
/// le code de dessin dans chaque composant et centralise la regle "on n'affiche que
/// ce qui est reellement actif".
/// Aucune logique de gameplay ici : ce fichier ne fait que du rendu editeur.
/// </summary>
public static class ColliderGizmoDrawer
{
    /// <summary>
    /// Dessine le contour du collider uniquement s'il est actif, c'est-a-dire si le
    /// composant proprietaire est actif ET si le collider lui-meme est actif.
    /// Les deux conditions sont necessaires : une Hurtbox desactivee garde par exemple
    /// son collider en place, mais ne recoit plus de coups.
    /// </summary>
    public static void DrawWireIfActive(Behaviour owner, Collider collider, Color color)
    {
        if (!IsActive(owner, collider))
            return;

        DrawWire(collider, color);
    }

    public static bool IsActive(Behaviour owner, Collider collider)
    {
        if (owner == null || collider == null)
            return false;

        return owner.isActiveAndEnabled && collider.enabled && collider.gameObject.activeInHierarchy;
    }

    public static void DrawWire(Collider collider, Color color)
    {
        if (collider == null)
            return;

        Color previousColor = Gizmos.color;
        Matrix4x4 previousMatrix = Gizmos.matrix;

        Gizmos.color = color;
        // Le repere du collider porte deja position, rotation et echelle : on dessine
        // donc en coordonnees locales, comme les valeurs saisies dans l'Inspecteur.
        Gizmos.matrix = collider.transform.localToWorldMatrix;

        switch (collider)
        {
            case SphereCollider sphere:
                Gizmos.DrawWireSphere(sphere.center, sphere.radius);
                break;

            case BoxCollider box:
                Gizmos.DrawWireCube(box.center, box.size);
                break;

            case CapsuleCollider capsule:
                DrawWireCapsule(capsule);
                break;

            default:
                // Type non gere (MeshCollider, etc.) : on retombe sur la boite englobante.
                // bounds est deja exprime en monde, d'ou le retour au repere identite.
                Gizmos.matrix = Matrix4x4.identity;
                Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
                break;
        }

        Gizmos.color = previousColor;
        Gizmos.matrix = previousMatrix;
    }

    /// <summary>
    /// Point d'interet en coordonnees monde (impact, point de contact...).
    /// </summary>
    public static void DrawWorldPoint(Vector3 worldPosition, float radius, Color color)
    {
        Color previousColor = Gizmos.color;
        Matrix4x4 previousMatrix = Gizmos.matrix;

        Gizmos.color = color;
        Gizmos.matrix = Matrix4x4.identity;

        Gizmos.DrawSphere(worldPosition, radius);

        Gizmos.color = previousColor;
        Gizmos.matrix = previousMatrix;
    }

    /// <summary>
    /// Unity ne fournit pas de DrawWireCapsule : on l'approxime par les deux calottes
    /// et les quatre aretes qui les relient.
    /// </summary>
    static void DrawWireCapsule(CapsuleCollider capsule)
    {
        float radius = capsule.radius;
        // La capsule ne peut pas etre plus courte que ses deux hemispheres : au dela,
        // les centres se confondent et on retombe sur une sphere.
        float halfSpan = Mathf.Max(0f, capsule.height * 0.5f - radius);

        Vector3 axis = capsule.direction switch
        {
            0 => Vector3.right,
            1 => Vector3.up,
            _ => Vector3.forward
        };

        Vector3 top = capsule.center + axis * halfSpan;
        Vector3 bottom = capsule.center - axis * halfSpan;

        Gizmos.DrawWireSphere(top, radius);
        Gizmos.DrawWireSphere(bottom, radius);

        Vector3 firstOffset = capsule.direction == 0 ? Vector3.up : Vector3.right;
        Vector3 secondOffset = Vector3.Cross(axis, firstOffset);

        DrawCapsuleEdge(top, bottom, firstOffset * radius);
        DrawCapsuleEdge(top, bottom, -firstOffset * radius);
        DrawCapsuleEdge(top, bottom, secondOffset * radius);
        DrawCapsuleEdge(top, bottom, -secondOffset * radius);
    }

    static void DrawCapsuleEdge(Vector3 top, Vector3 bottom, Vector3 offset)
    {
        Gizmos.DrawLine(top + offset, bottom + offset);
    }
}
