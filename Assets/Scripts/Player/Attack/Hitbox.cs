using System;
using System.Collections;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    AttackData attackStats;
    [SerializeField] float size;
    private bool isActive = false;

    public void Init(AttackData attackStats)
    {
        this.attackStats = attackStats;
    }

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
    
    /*
    private IEnumerator HitboxCoroutine()
    {
        if (!isActive)
        {
            yield break;
        }

        for (frameCount = 0; frameCount < frameRate; frameCount++)
        {
            yield return new WaitForEndOfFrame();
        }
        enabled = false;
    }*/

}
