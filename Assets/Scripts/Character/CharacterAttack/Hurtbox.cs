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

    [Tooltip("Duree d'affichage d'un point d'impact, en secondes. 0 = une seule frame.")]
    [SerializeField, Min(0f)] private float hitPointLifetime = 1f;

    [SerializeField] private Color hitPointColor = Color.yellow;

    /// <summary>
    /// Impacts recus recemment, conserves uniquement pour l'affichage des gizmos.
    /// La liste n'est alimentee que dans l'editeur (voir RecordHitGizmo) : en build,
    /// l'appel est supprime a la compilation et la liste reste vide.
    /// </summary>
    private readonly List<RecordedHit> recordedHits = new List<RecordedHit>();

    private struct RecordedHit
    {
        public Vector3 Position;
        public float ExpirationTime;
    }
    
    public void Initialize(PlayerGameplay owner)
    {
        this.owner = owner;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hitbox")
        {
            if(IsOtherPlayer(other))
            {
                Hitbox hitbox = other.GetComponent<Hitbox>();
                HitData hitData = CreateHit(hitbox, other.ClosestPoint(transform.position));

                // CreateHit renvoie default quand l'attaque n'est pas configuree :
                // sans ce garde, l'impact serait dessine a l'origine du monde.
                if (hitData.Attack != null)
                    RecordHitGizmo(hitData.HitPosition);

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

    /// <summary>
    /// Memorise un point d'impact pour l'affichage. L'attribut Conditional supprime les
    /// appels a la compilation hors editeur : aucun cout, et surtout aucune liste qui
    /// grossirait indefiniment en build, ou les gizmos ne sont jamais dessines.
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void RecordHitGizmo(Vector3 position)
    {
        PruneExpiredHits();

        recordedHits.Add(new RecordedHit
        {
            Position = position,
            ExpirationTime = Time.time + hitPointLifetime
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

        DrawRecordedHits();
    }

    /// <summary>
    /// Les impacts sont dessines en coordonnees monde, figes la ou le contact a eu lieu :
    /// la victime etant ejectee dans la foulee, un point solidaire du personnage serait
    /// illisible.
    /// </summary>
    private void DrawRecordedHits()
    {
        PruneExpiredHits();

        foreach (RecordedHit hit in recordedHits)
        {
            ColliderGizmoDrawer.DrawWorldPoint(hit.Position, hitPointRadius, hitPointColor);
        }
    }
}
