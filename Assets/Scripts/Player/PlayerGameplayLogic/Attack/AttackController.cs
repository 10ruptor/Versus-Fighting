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

    private void Awake()
    {
        playerGameplay = GetComponent<PlayerGameplay>();
    }
    
    
    public void ResolveGroundAttack()
    {
        if(playerGameplay.PlayerInputController.HasWalkInput)
        {
            currentAttack = playerGameplay.Character.attackLookup[AttackTypes.SideTilt];
        }
        else if(playerGameplay.PlayerInputController.HasUpMoveInput)
        {
            currentAttack = playerGameplay.Character.attackLookup[AttackTypes.UpTilt];
        }
        else
        {
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
        playerGameplay.Character.CharacterAnimatorController.AnimationTransition(currentAttack.AnimationTrigger);
    }
    public void EndAttack()
    {
        Debug.Log("Attack ended.");
        IsAttacking = false;
        currentAttack = null;
    }
    
    public void ActivateHitbox()
    {
        playerGameplay.Character.HitboxManager.ActivateHitbox(currentAttack.AttackType);
    }
    public void DeactivateHitbox()
    {
        playerGameplay.Character.HitboxManager.DeactivateHitbox(currentAttack.AttackType);
    }
}
