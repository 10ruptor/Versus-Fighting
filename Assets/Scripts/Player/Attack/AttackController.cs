using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public enum Attacks
    {
        UpTilt,
        DownTilt,
        NeutralTilt
    }

    public bool IsAttacking { get; private set; }
    private int elapsedFrames = 0;
    PlayerGameplay playerGameplay;
    public List<AttackEntry> AttackStatList = new List<AttackEntry>();
    private AttackStatSO currentAttack;
    
    private GameObject currentHitboxInstance;
    
    private void SwitchCurrentAttack(Attacks attacks)
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
        elapsedFrames = 0;
        if (!playerGameplay.PlayerInputManager.HasMoveInput)
        {
            SwitchCurrentAttack(Attacks.NeutralTilt);
        }
        if (playerGameplay.PlayerInputManager.HasMoveInput)
        {
            SwitchCurrentAttack(Attacks.UpTilt);
        }
        IsAttacking = true;
        playerGameplay.CharacterAnimatorController.UpdateAnimation(currentAttack.AnimationTrigger,true);
    }

    private void Update()
    {
        if (!IsAttacking) return;
        
        elapsedFrames++;
        if (elapsedFrames == currentAttack.hitboxActiveFrames[0])
        {
            currentHitboxInstance = Instantiate(currentAttack.hitbox, transform.position + currentAttack.hitboxPosition, Quaternion.identity).gameObject;
            currentHitboxInstance.GetComponent<Hitbox>().Init(currentAttack);
        }
        if (elapsedFrames > currentAttack.hitboxActiveFrames[^1])
        {
            Destroy(currentHitboxInstance);
        }
    }
    public void EndAttack()
    {
        Debug.Log("Attack ended.");
        
        IsAttacking = false;
        playerGameplay.CharacterAnimatorController.UpdateAnimation(currentAttack.AnimationTrigger,false);
        currentAttack = null;
        elapsedFrames = 0;
    }
}
