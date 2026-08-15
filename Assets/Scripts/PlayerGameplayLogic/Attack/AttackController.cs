using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public bool IsAttacking { get; private set; }
    //private int elapsedFrames = 0; CONTROLLED BY ANIMATOR
    PlayerGameplay playerGameplay;
    private AttackData currentAttack;
    private GameObject currentHitboxInstance;

    private void Awake()
    {
        playerGameplay = GetComponent<PlayerGameplay>();
    }
    
    
    public void ResolveGroundAttack()
    {
        if(playerGameplay.PlayerInputManager.HasWalkInput)
        {
            //currentAttack = playerGameplay.Character.AttackStatList.Find(entry => entry.AttackType == Attacks.SideTilt)?.AttackStat;
            currentAttack = playerGameplay.Character.attackLookup[AttackTypes.SideTilt];
        }
        else if(playerGameplay.PlayerInputManager.HasUpMoveInput)
        {
            //currentAttack = playerGameplay.Character.AttackStatList.Find(entry => entry.AttackType == Attacks.UpTilt)?.AttackStat;
            currentAttack = playerGameplay.Character.attackLookup[AttackTypes.UpTilt];
        }
        else
        {
            //currentAttack = playerGameplay.Character.AttackStatList.Find(entry => entry.AttackType == Attacks.NeutralTilt)?.AttackStat;
            currentAttack = playerGameplay.Character.attackLookup[AttackTypes.NeutralTilt];
        }
    }
    
    public void ResolveAerialAttack()
    {
        //currentAttack = playerGameplay.Character.AttackStatList.Find(entry => entry.AttackType == Attacks.Nair)?.AttackStat;
        currentAttack = playerGameplay.Character.attackLookup[AttackTypes.Nair];
    }

    public void StartAttack()
    {
        if (!currentAttack)        
        {
            Debug.Log("No AttackData found for attack type: " + currentAttack);
            return;
        }
        IsAttacking = true;
        InstantiateHitbox();
        DeactivateHitbox();
        playerGameplay.Character.CharacterAnimatorController.AnimationTransition(currentAttack.AnimationTrigger);
    }
    public void EndAttack()
    {
        Debug.Log("Attack ended.");
        Destroy(currentHitboxInstance);
        IsAttacking = false;
        currentAttack = null;
    }
    private void InstantiateHitbox()
    {
        currentHitboxInstance = Instantiate(currentAttack.hitbox, transform.position + currentAttack.hitboxPosition, Quaternion.identity).gameObject;
        currentHitboxInstance.GetComponent<Hitbox>().Initialize(currentAttack.hitboxRadius);
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
