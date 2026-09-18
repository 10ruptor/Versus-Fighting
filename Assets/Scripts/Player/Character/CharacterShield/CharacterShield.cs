using UnityEngine;

/// <summary>
/// Le bouclier tel qu'il existe dans la scene : son visuel, son volume, et sa capacite a
/// intercepter une hitbox. Porte par le GameObject Shield du personnage.
///
/// Il ne decide rien : la durabilite, le verdict sur un coup et la taille a appliquer
/// appartiennent au ShieldController. Ici on ne fait qu'exposer au controleur ce que seul
/// le personnage peut fournir : une geometrie.
///
/// Consequence de cette geometrie : un bouclier entame retrecit reellement, ce qui laisse
/// des zones du personnage a decouvert. La regle n'est ecrite nulle part, elle tombe de la
/// taille du volume.
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class CharacterShield : MonoBehaviour
{
    private PlayerGameplay owner;
    public PlayerGameplay Owner => owner;

    private SphereCollider shieldCollider;

    // Taille du bouclier telle qu'elle est reglee dans le prefab : c'est elle qui sert de
    // reference "durabilite pleine", pour que le designer continue de regler la taille du
    // bouclier a la main, dans la scene.
    private Vector3 fullSizeScale = Vector3.one;
    private bool initialized;

    /// <summary>
    /// Appele par le Character. Tout est resolu ici plutot que dans Awake : le GameObject
    /// du bouclier est inactif au repos, son Awake ne tournerait donc qu'au premier lever,
    /// une fois la taille deja modifiee.
    /// </summary>
    public void Initialize(PlayerGameplay owner)
    {
        this.owner = owner;
        shieldCollider = GetComponent<SphereCollider>();
        fullSizeScale = transform.localScale;
        initialized = true;
    }

    public bool IsRaised => gameObject.activeSelf;

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    /// <summary>Taille du bouclier, en proportion de celle reglee dans le prefab.</summary>
    public void SetSizeRatio(float sizeRatio)
    {
        // Tant que la taille de reference n'a pas ete relevee, on ne touche a rien :
        // redimensionner a partir d'une reference inventee deformerait le bouclier du prefab.
        if (!initialized)
            return;

        transform.localScale = fullSizeScale * Mathf.Max(sizeRatio, 0f);
    }

    /// <summary>True si le point d'impact tombe dans le volume du bouclier a sa taille courante.</summary>
    public bool Covers(Vector3 worldPoint)
    {
        SphereCollider currentCollider = ResolveCollider();

        if (currentCollider == null)
            return false;

        Vector3 center = transform.TransformPoint(currentCollider.center);

        Vector3 lossyScale = transform.lossyScale;
        float uniformScale = Mathf.Max(Mathf.Abs(lossyScale.x), Mathf.Abs(lossyScale.y), Mathf.Abs(lossyScale.z));
        float radius = currentCollider.radius * uniformScale;

        return (worldPoint - center).sqrMagnitude <= radius * radius;
    }

    /// <summary>
    /// Le bouclier deborde du personnage : une hitbox peut l'atteindre sans jamais toucher
    /// une hurtbox. Il doit donc constater le contact lui meme, sinon ces coups la
    /// passeraient simplement au travers sans etre bloques.
    ///
    /// Comme la hurtbox, il ne fait que constater : c'est le HitReceptionController qui
    /// decide ce qu'il advient du coup.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (owner == null || !other.CompareTag("Hitbox"))
            return;

        Hitbox hitbox = other.GetComponent<Hitbox>();

        if (hitbox == null || hitbox.Owner == owner)
            return;

        if (HitData.TryCreate(hitbox, other.ClosestPoint(transform.position), null, out HitData hitData))
            owner.HitReceptionController.ReceiveHit(hitData, contactOnShield: true);
    }

    private SphereCollider ResolveCollider()
    {
        if (shieldCollider == null)
            shieldCollider = GetComponent<SphereCollider>();

        return shieldCollider;
    }
}
