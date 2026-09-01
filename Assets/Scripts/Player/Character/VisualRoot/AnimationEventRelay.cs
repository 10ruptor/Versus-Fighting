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
		public void ActivateAttackHitbox()
        {
            attackController.ActivateHitbox();
        }
		public void DeactivateAttackHitbox()
        {
            attackController.DeactivateHitbox();
        }
}
