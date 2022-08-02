using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

   // Private Variables
	private PlayerHandler playerHandler;
   private float attackAnimDelay = 0.5f;


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
		playerHandler = GetComponent<PlayerHandler>();
	}


   // ====================================================================================
   // Public Methods
   // ====================================================================================
	public void Attack(string typeOfAttack) {
      if(!playerHandler.isAttacking && !playerHandler.isProtecting) {
      
         playerHandler.isAttacking = true;
         playerHandler.SetPlayerAnim("Attack", typeOfAttack);
         StartCoroutine(AttackTimeOut());
      }
	}


   // ====================================================================================
   // Coroutines
   // ====================================================================================
   IEnumerator AttackTimeOut() {
      yield return new WaitForSeconds(attackAnimDelay);
      playerHandler.isAttacking = false;
		playerHandler.IdleAnim();
   }
}
