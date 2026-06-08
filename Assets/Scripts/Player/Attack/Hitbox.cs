using System;
using System.Collections;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] AttackStatSO attackStats;
    [SerializeField] float size;
    public AttackStatSO AttackStats => attackStats;
    private bool isActive = false;

    [SerializeField] private int frameRate;
    private int frameCount;

    private void OnEnable()
    {
        isActive = true;
        frameCount = 0;
        StartCoroutine(HitboxCoroutine());
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
    }

    private void OnDisable()
    {
        isActive = false;
    }

}
