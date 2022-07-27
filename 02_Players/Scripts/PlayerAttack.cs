using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

   // Public Variables
   [Header("Attack Delay")]
   public float attackAnimDelay = 0.35f;

   
   // Private Variables
	private PlayerHandler playerHandler;
	

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
      
      if(!playerHandler.isAttacking) {
         playerHandler.SetAnim("Attack", typeOfAttack);

         playerHandler.isAttacking = true;
         playerHandler.attackType = typeOfAttack;
         StartCoroutine(AttackTimeOut());
      }
	}


   // ====================================================================================
   // Coroutines
   // ====================================================================================
   IEnumerator AttackTimeOut() {
      yield return new WaitForSeconds(attackAnimDelay);

      playerHandler.isAttacking = false;
      playerHandler.attackType = "";
   }
}
