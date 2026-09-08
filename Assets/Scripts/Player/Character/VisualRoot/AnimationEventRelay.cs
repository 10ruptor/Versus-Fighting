using System;
using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
        AttackController attackController;
        private JumpController jumpController;
        private void Awake()
        {
            attackController = GetComponentInParent<AttackController>();
            jumpController = GetComponentInParent<JumpController>();
        }
        
        /// <summary>Animation Event : warn the jumpController that load jump animation is over and that jump animation has started.</summary>
        public void BeginJump()
        {
            jumpController.Begin();
        }
        
        /// <summary>Animation Event : warn the attackController that attack animation is over.</summary>
        public void EndAttack()
        {
            attackController.EndAttack();
        }

        /// <summary>Animation Event : ouvre toutes les hitbox de l'attaque en cours.</summary>
        public void ActivateAllAttackHitbox()
        {
            attackController.ActivateHitboxAll();
        }

        /// <summary>Animation Event : ferme toutes les hitbox de l'attaque en cours.</summary>
        public void DeactivateAllAttackHitbox()
        {
            attackController.DeactivateHitboxAll();
        }

        /// <summary>Animation Event : ouvre la hitbox de l'attaque en cours occupant ce slot.</summary>
        public void ActivateAttackHitboxSlot(HitboxSlot slot)
        {
            attackController.ActivateHitboxAtSlot(slot);
        }

        /// <summary>Animation Event : ferme la hitbox de l'attaque en cours occupant ce slot.</summary>
        public void DeactivateAttackHitboxSlot(HitboxSlot slot)
        {
            attackController.DeactivateHitboxAtSlot(slot);
        }
}
