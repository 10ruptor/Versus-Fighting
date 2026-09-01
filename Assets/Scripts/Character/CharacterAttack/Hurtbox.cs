using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private PlayerGameplay owner;
    public PlayerGameplay Owner => owner;

    private Collider hurtboxCollider;

    [Header("Debug")]
    [SerializeField] private bool drawGizmo = true;
    [SerializeField] private Color gizmoColor = Color.cyan;

    [Tooltip("Rayon du point affiche a l'endroit exact du contact.")]
    [SerializeField, Min(0f)] private float hitPointRadius = 0.05f;

    [Tooltip("Duree d'affichage d'un impact, en secondes. 0 = une seule frame.")]
    [SerializeField, Min(0f)] private float hitLifetime = 1f;

    [SerializeField] private Color hitPointColor = Color.yellow;

    [Tooltip("Affiche le vecteur d'ejection reellement applique par le KnockbackController.")]
    [SerializeField] private bool drawLaunchVector = true;
    [SerializeField] private Color launchVectorColor = Color.magenta;

    [Tooltip("Longueur du vecteur = vitesse d'ejection x ce facteur.")]
    [SerializeField, Min(0f)] private float launchVectorScale = 0.25f;

    /// <summary>
    /// Impacts recus recemment, conserves uniquement pour l'affichage des gizmos.
    /// La liste n'est alimentee que dans l'editeur : hors editeur, l'abonnement n'a
    /// jamais lieu (voir SubscribeHitGizmo) et elle reste donc vide.
    /// </summary>
    private readonly List<RecordedHit> recordedHits = new List<RecordedHit>();

    private struct RecordedHit
    {
        public Vector3 Position;
        public Vector3 LaunchVelocity;
        public float ExpirationTime;
    }
    
    public void Initialize(PlayerGameplay owner)
    {
        this.owner = owner;
        SubscribeHitGizmo();
    }

    private void OnDestroy()
    {
        UnsubscribeHitGizmo();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hitbox")
        {
            if(IsOtherPlayer(other))
            {
                Hitbox hitbox = other.GetComponent<Hitbox>();
                HitData hitData = CreateHit(hitbox, other.ClosestPoint(transform.position));
                owner.KnockbackController.Knockback(hitData);
            }
        }
    }

    private bool IsOtherPlayer(Collider other)
    {
        switch (other.tag)
        {
            case "Hitbox":
                return other.GetComponent<Hitbox>().Owner != this.owner;
            
            case "Hurtbox":
                return other.GetComponent<Hurtbox>().Owner != this.owner;
            
            default:
                return false;
        }
    }
    
    private HitData CreateHit(Hitbox hitbox, Vector3 position)
    {
        AttackDataSO attackData = hitbox.attackData;

        if (attackData == null)
        {
            Debug.LogError("AttackDataSO not found on hitbox.");
            return default;
        }

        return new HitData
        {
            Attacker = hitbox.Owner,
            Attack = attackData,
            HitPosition = position
        };
    }

    /// <summary>
    /// Awake ne tourne pas en mode edition : le collider est resolu a la demande pour
    /// que les gizmos fonctionnent aussi bien dans la Scene view qu'en play.
    /// </summary>
    private Collider ResolveCollider()
    {
        if (hurtboxCollider == null)
            hurtboxCollider = GetComponent<Collider>();

        return hurtboxCollider;
    }

    #region Gizmos

    /// <summary>
    /// L'impact est enregistre depuis le resultat du KnockbackController plutot que
    /// depuis OnTriggerEnter : point de contact et vecteur d'ejection arrivent ainsi
    /// ensemble, dans la meme frame, sans jamais produire d'enregistrement incomplet.
    /// L'attribut Conditional supprime les appels a la compilation hors editeur : aucun
    /// abonnement, et donc aucune liste qui grossirait indefiniment en build, ou les
    /// gizmos ne sont jamais dessines.
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void SubscribeHitGizmo()
    {
        if (owner == null || owner.KnockbackController == null)
            return;

        // Desabonnement prealable : Initialize reste ainsi idempotent.
        owner.KnockbackController.KnockbackResolved -= RecordHit;
        owner.KnockbackController.KnockbackResolved += RecordHit;
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void UnsubscribeHitGizmo()
    {
        if (owner == null || owner.KnockbackController == null)
            return;

        owner.KnockbackController.KnockbackResolved -= RecordHit;
    }

    private void RecordHit(HitData hitData, Vector3 launchVelocity)
    {
        PruneExpiredHits();

        recordedHits.Add(new RecordedHit
        {
            Position = hitData.HitPosition,
            LaunchVelocity = launchVelocity,
            ExpirationTime = Time.time + hitLifetime
        });
    }

    private void PruneExpiredHits()
    {
        for (int i = recordedHits.Count - 1; i >= 0; i--)
        {
            if (Time.time > recordedHits[i].ExpirationTime)
                recordedHits.RemoveAt(i);
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmo)
            return;

        // Contour de la hurtbox : visible tant qu'elle est reellement en mesure
        // d'encaisser un coup.
        ColliderGizmoDrawer.DrawWireIfActive(this, ResolveCollider(), gizmoColor);

        PruneExpiredHits();

        foreach (RecordedHit hit in recordedHits)
        {
            DrawHit(hit);
        }
    }

    /// <summary>
    /// Representation d'un impact. Les impacts sont dessines en coordonnees monde, figes
    /// la ou le contact a eu lieu : la victime etant ejectee dans la foulee, un point
    /// solidaire du personnage serait illisible.
    ///
    /// Point d'extension : afficher la trajectoire predite plutot que le seul vecteur ne
    /// demande que de remplacer le trace ci-dessous. Les donnees necessaires (position et
    /// vitesse initiale) sont deja enregistrees, la plomberie n'a pas a changer.
    /// </summary>
    private void DrawHit(RecordedHit hit)
    {
        ColliderGizmoDrawer.DrawWorldPoint(hit.Position, hitPointRadius, hitPointColor);

        if (drawLaunchVector)
            ColliderGizmoDrawer.DrawArrow(hit.Position, hit.LaunchVelocity * launchVectorScale, launchVectorColor);
    }

    #endregion
}
