using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerAttack : MonoBehaviour {

   // Public Variables
   [Header("**Damages Values**")]
   public float strikeDamages = 220f;
   public float estocDamages = 150f;


   // Private Variables
   private float animDoneDelay = 0.4f;
   private GameRandomize gameRand;
   private PlayerHandler pH;
	

   // ====================================================================================
	// Start
	// ====================================================================================
	private void Start() {
		gameRand = GameObject.Find("_GameHandler").GetComponent<GameRandomize>();
		pH = GetComponent<PlayerHandler>();
	}


   // ====================================================================================
   // Public Methods
   // ====================================================================================
	public void Ev_StrikeAttack(object sender, EventArgs e) {
      if(!pH.isProtecting && !pH.isAttacking) {
            
         pH.isAttacking = true;
         pH.SetPlayerAnim("Sword", "Strike");
         pH.damagesValue = strikeDamages;
         StartCoroutine(AttackTimeOut());
      }
	}

   public void Ev_EstocAttack(object sender, EventArgs e) {
      if(!pH.isProtecting && !pH.isAttacking) {
         
         pH.isAttacking = true;
         pH.SetPlayerAnim("Sword", "Estoc");
         pH.damagesValue = estocDamages;
         StartCoroutine(AttackTimeOut());
      }
	}


   // ====================================================================================
   // Coroutines
   // ====================================================================================
   IEnumerator AttackTimeOut() {
      yield return new WaitForSeconds(animDoneDelay);

      if(pH.isEnemyDamaged) pH.isEnemyDamaged = false;
      if(pH.isAttacking) pH.isAttacking = false;
      if(pH.isWalking) pH.SetPlayerAnim("Sword", pH.walkDirection);
      else pH.SetPlayerAnim("Sword", "Idle");
   }
}
