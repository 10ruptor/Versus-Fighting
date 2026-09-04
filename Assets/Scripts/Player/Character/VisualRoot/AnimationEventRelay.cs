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
        
        public void BeginJump()
        {
            jumpController.Begin();
        }
        
        public void EndAttack()
        {
            attackController.EndAttack();
        }

        /// <summary>Animation Event : ouvre toutes les hitbox de l'attaque en cours.</summary>
        public void ActivateAttackHitbox()
        {
            attackController.ActivateHitbox();
        }

        /// <summary>Animation Event : ferme toutes les hitbox de l'attaque en cours.</summary>
        public void DeactivateAttackHitbox()
        {
            attackController.DeactivateHitbox();
        }

        // Un Animation Event n'accepte qu'un seul argument : le type d'attaque est deja
        // connu (l'animation jouee est celle de l'attaque en cours), donc l'argument sert a
        // choisir la hitbox. Le parametre est un enum : la fenetre d'animation affiche une
        // liste deroulante des slots au lieu d'un index numerique.
        // Methodes nommees differemment des versions sans argument : Unity resout un
        // Animation Event par nom de methode, une surcharge serait ambigue.

        /// <summary>Animation Event : ouvre la hitbox de l'attaque en cours occupant ce slot.</summary>
        public void ActivateAttackHitboxSlot(HitboxSlot slot)
        {
            attackController.ActivateHitbox(slot);
        }

        /// <summary>Animation Event : ferme la hitbox de l'attaque en cours occupant ce slot.</summary>
        public void DeactivateAttackHitboxSlot(HitboxSlot slot)
        {
            attackController.DeactivateHitbox(slot);
        }
}
