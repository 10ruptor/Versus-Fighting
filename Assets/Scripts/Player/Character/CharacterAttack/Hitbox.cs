using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Hitbox : MonoBehaviour
{

    [Tooltip("Identifie cette hitbox au sein de l'attaque. C'est la valeur choisie dans " +
             "l'Animation Event pour ouvrir ou fermer cette hitbox en particulier.")]
    [SerializeField] private HitboxSlot slot = HitboxSlot.Primary;
    public HitboxSlot Slot => slot;

    private Attack currentAttack;

    public Attack CurrentAttack => currentAttack;
    public void ReadAttack(Attack attack)
    {
        currentAttack = attack;
    }

    private PlayerGameplay owner; //owner is used to be able to differenciate players when hitting
    public PlayerGameplay Owner => owner;
    
    private Collider hitboxCollider;
    public Collider HitboxCollider => hitboxCollider;
    
    [SerializeField]  AttackDataSO attackData; //used for knockback previsualization only

    [Header("Debug")]
    [SerializeField] private bool drawGizmo = true;
    [SerializeField] private Color gizmoColor = Color.red;

    [Tooltip("Affiche l'angle d'ejection configure dans l'AttackDataSO. Apercu theorique : " +
             "il ne tient pas compte du % de la victime ni du cote reel du contact.")]
    [SerializeField] private bool drawKnockbackPreview = true;
    [SerializeField] private Color knockbackPreviewColor = new Color(1f, 0.55f, 0f);

    [Tooltip("Longueur de l'apercu = vitesse d'ejection x ce facteur.")]
    [SerializeField, Min(0f)] private float knockbackPreviewScale = 0.25f;
    
    public void Initialize(PlayerGameplay owner)
    {
        this.owner = owner; 
    }

    private void Awake()
    {
        hitboxCollider = ResolveCollider();
        hitboxCollider.enabled = false;
    }

    private void OnEnable()
    {
        hitboxCollider.enabled = true;
    }

    private void OnDisable()
    {
        hitboxCollider.enabled = false;
    }

    /// <summary>
    /// Awake ne tourne pas en mode edition : le collider est resolu a la demande pour
    /// que les gizmos fonctionnent aussi bien dans la Scene view qu'en play.
    /// </summary>
    private Collider ResolveCollider()
    {
        if (hitboxCollider == null)
            hitboxCollider = GetComponent<Collider>();

        return hitboxCollider;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmo)
            return;

        // Le gizmo suit strictement l'etat du collider : il n'apparait que pendant la
        // fenetre active de l'attaque, ouverte et fermee par les Animation Events.
        if (!ColliderGizmoDrawer.IsActive(this, ResolveCollider()))
            return;

        ColliderGizmoDrawer.DrawWire(hitboxCollider, gizmoColor);
        DrawKnockbackPreview();
    }

    /// <summary>
    /// Apercu de l'ejection telle qu'elle est configuree dans l'AttackDataSO, lisible
    /// sans lancer le jeu. C'est volontairement une valeur theorique : la vitesse reelle
    /// depend du % de la victime au moment du coup, et le cote est resolu depuis le point
    /// de contact. Le vecteur reellement applique est affiche par la Hurtbox touchee.
    /// </summary>
    private void DrawKnockbackPreview()
    {
        if (!drawKnockbackPreview || attackData == null)
            return;

        float angleRadians = attackData.launchAngle * Mathf.Deg2Rad;
        Vector3 localDirection = new Vector3(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians), 0f);

        // L'angle de l'AttackDataSO est exprime pour un coup porte vers la droite. On le
        // laisse donc dans le repere de la hitbox, qui suit deja le retournement du
        // personnage (VisualRoot pivote de 180 degres a chaque changement d'orientation) :
        // l'apercu se mirroite ainsi tout seul, sans lire l'orientation du joueur.
        Vector3 direction = transform.rotation * localDirection;

        // baseKnockback est la vitesse a 0% de degats : le scaling ne peut pas etre
        // anticipe ici, il depend de la victime.
        float previewSpeed = attackData.baseKnockback * attackData.KnockbackMultiplier;

        ColliderGizmoDrawer.DrawArrow(
            transform.position,
            direction * previewSpeed * knockbackPreviewScale,
            knockbackPreviewColor);
    }
}
