using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackController : MonoBehaviour 
{
    public bool IsAttacking { get; private set; }
    //private int elapsedFrames = 0; CONTROLLED BY ANIMATOR
    PlayerGameplay playerGameplay;
    Attack currentAttack;

    private void Awake()
    {
        playerGameplay = GetComponent<PlayerGameplay>();
    }
    
    
    public void ResolveGroundAttack()
    {
        if(playerGameplay.PlayerInputController.HasWalkInput)
        {
            currentAttack = playerGameplay.Character.AttackLibrary.Attacks[AttackTypes.SideTilt];
        }
        else if(playerGameplay.PlayerInputController.HasUpMoveInput)
        {
            currentAttack = playerGameplay.Character.AttackLibrary.Attacks[AttackTypes.UpTilt];
        }
        else
        {
            currentAttack = playerGameplay.Character.AttackLibrary.Attacks[AttackTypes.NeutralTilt];
        }
    }
    
    public void ResolveAerialAttack()
    {
        if(playerGameplay.PlayerInputController.HasUpMoveInput)
        {
            currentAttack = playerGameplay.Character.AttackLibrary.Attacks[AttackTypes.Uair];
        }
        else
        {
            currentAttack = playerGameplay.Character.AttackLibrary.Attacks[AttackTypes.Nair];
        }
    }

    public void StartAttack()
    {
        if (currentAttack is null)        
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
        if (!HasCurrentAttack("ActivateHitbox"))
            return;

        playerGameplay.Character.AttackLibrary.ActivateAttackHitboxAll(currentAttack.AttackType);
    }

    // Le type d'attaque n'a pas a etre passe par l'Animation Event : l'animation jouee est
    // celle de currentAttack. L'argument du event ne sert donc qu'a designer la hitbox
    // voulue parmi celles de cette attaque.
    public void ActivateHitbox(HitboxSlot slot)
    {
        if (!HasCurrentAttack("ActivateHitbox"))
            return;

        playerGameplay.Character.AttackLibrary.ActivateAttackHitboxAtSlot(currentAttack.AttackType, slot);
    }

    public void DeactivateHitbox()
    {
        if (!HasCurrentAttack("DeactivateHitbox"))
            return;

        playerGameplay.Character.AttackLibrary.DeactivateAttackHitboxAll(currentAttack.AttackType);
    }

    public void DeactivateHitbox(HitboxSlot slot)
    {
        if (!HasCurrentAttack("DeactivateHitbox"))
            return;

        playerGameplay.Character.AttackLibrary.DeactivateAttackHitboxAtSlot(currentAttack.AttackType, slot);
    }

    private bool HasCurrentAttack(string context)
    {
        if (currentAttack != null)
            return true;

        Debug.LogWarning($"AttackController: {context} appele hors attaque sur {name}.", this);
        return false;
    }
}
