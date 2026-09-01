using System;
using System.Collections;
using UnityEngine;

public class Hitbox : MonoBehaviour
{

    [SerializeField] public AttackTypes AttackType;
    
    private PlayerGameplay owner;
    public PlayerGameplay Owner => owner;
    private Collider hitboxCollider;
    public Collider HitboxCollider => hitboxCollider;
    public AttackDataSO attackData;

    [Header("Debug")]
    [SerializeField] private bool drawGizmo = true;
    [SerializeField] private Color gizmoColor = Color.red;
    

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
        ColliderGizmoDrawer.DrawWireIfActive(this, ResolveCollider(), gizmoColor);
    }
}
