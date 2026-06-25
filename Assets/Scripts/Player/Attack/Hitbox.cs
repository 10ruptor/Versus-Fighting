using System;
using System.Collections;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] float size;
    private bool isActive = false;
    private void Awake()
    {
        isActive = true;
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
