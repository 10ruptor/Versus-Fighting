using System;
using System.Collections;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    float size;
    private bool isActive = false;
    private SphereCollider collider;

    public void Initialize(float size)
    {
        this.size = size;
        collider.radius = size;
    }

    private void Awake()
    {
        isActive = true;
        collider = GetComponent<SphereCollider>();
        
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
}
