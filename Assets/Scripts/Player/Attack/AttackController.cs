using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public enum Attacks
    {
        UpTilt,
        SideTilt,
        DownTilt,
        NeutralTilt
    }

    public bool IsAttacking { get; private set; }
    private int elapsedFrames = 0;
    PlayerGameplay playerGameplay;
    public List<AttackEntry> AttackStatList = new List<AttackEntry>();
    private AttackData currentAttack;
    
    private GameObject currentHitboxInstance;
    
    public void SwitchCurrentAttack(Attacks attacks)
    {
        Debug.Log("Switching attack to: " + attacks);
        currentAttack = AttackStatList.Find(entry => entry.AttackType == attacks)?.AttackStat;
    }

    private void Awake()
    {
        playerGameplay = GetComponent<PlayerGameplay>();
    }

    public void StartAttack()
    {
        if (currentAttack == null)        
        {
            Debug.Log("No AttackData found for attack type: " + currentAttack);
            return;
        }
        elapsedFrames = 0;
        IsAttacking = true;
        InstantiateHitbox();
        DeactivateHitbox();
        playerGameplay.CharacterAnimatorController.UpdateAnimation(currentAttack.AnimationTrigger,true);
    }

    public void EndAttack()
    {
        Debug.Log("Attack ended.");
        Destroy(currentHitboxInstance);
        IsAttacking = false;
        currentAttack = null;
        elapsedFrames = 0;
    }

    private void InstantiateHitbox()
    {
        currentHitboxInstance = Instantiate(currentAttack.hitbox, transform.position + currentAttack.hitboxPosition, Quaternion.identity).gameObject;
        currentHitboxInstance.GetComponent<Hitbox>().Init(currentAttack);
    }

    public void ActivateHitbox()
    {
        if (currentHitboxInstance != null)
        {
            currentHitboxInstance.SetActive(true);
        }
    }
    
    public void DeactivateHitbox()
    {
        if (currentHitboxInstance != null)
        {
            currentHitboxInstance.SetActive(false);
        }
    }
}
