using System;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] AttackStatSO attackStats;
    [SerializeField] float size;
    public AttackStatSO AttackStats => attackStats;
    
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, size);
    }
    
}
