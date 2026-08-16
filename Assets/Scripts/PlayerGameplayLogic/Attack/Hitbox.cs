using System;
using System.Collections;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    float size;
    private bool isActive = false;
    [SerializeField] public AttackTypes attack;
    private Collider collider;
    
    private void Awake()
    {
        isActive = true;
        collider = GetComponent<Collider>();
    }
    
    private void OnDrawGizmos()
    {
        if (!isActive)
        {
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, size);
    }
    public void Initialize(float size)
    {
        this.size = size;
        GetComponent<SphereCollider>().radius = size;
    }

    public void ActivateHitbox()
    {
        collider.enabled = true;
    }
    
    public void DeactivateHitbox()
    {
        collider.enabled = false;
    }

}
