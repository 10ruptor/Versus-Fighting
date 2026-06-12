using System;
using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
        AttackController attackController;

        private void Awake()
        {
            attackController = GetComponentInParent<AttackController>();
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
