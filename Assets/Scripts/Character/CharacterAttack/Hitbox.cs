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

    public void Initialize(PlayerGameplay owner)
    {
        this.owner = owner;
    }

    private void Awake()
    {
        hitboxCollider = GetComponent<Collider>();
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

    private void OnDrawGizmos()
    {
        SphereCollider sphere = GetComponent<SphereCollider>();

        if (sphere == null)
            return;

        Gizmos.color = Color.red;

        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawWireSphere(
            sphere.center,
            sphere.radius
        );
    }
}
