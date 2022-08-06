using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

   // Private Variables
	private PlayerHandler pH;
   private float attackAnimDelay = 0.4f;


	// public Transform attackPoint;

   // public int strikeDamages = 20;
   // public int estocDamages = 20;

	// public float attackRange = 2.2f;
	// public float attackAnimDuration = 1.5f;
	// public float startAttackTime = 0.3f;
	// public float timeBetweenAttack = 3.0f;
	// private float endAttackTime;
	

   // ====================================================================================
	// Start
	// ====================================================================================
	private void Start() {
		pH = GetComponent<PlayerHandler>();
	}


   // ====================================================================================
   // Public Methods
   // ====================================================================================
	public void Attack(string attackType) {
      if(!pH.isProtecting && !pH.isAttacking) {
            
         pH.isAttacking = true;
         pH.SetPlayerAnim("Sword", attackType);
         StartCoroutine(AttackTimeOut());
      }
	}


   // ====================================================================================
   // Coroutines
   // ====================================================================================
   IEnumerator AttackTimeOut() {
      yield return new WaitForSeconds(attackAnimDelay);

      if(pH.isAttacking) pH.isAttacking = false;
      if(!pH.isWalking) pH.SetPlayerAnim("Sword", "Idle");
   }
}
