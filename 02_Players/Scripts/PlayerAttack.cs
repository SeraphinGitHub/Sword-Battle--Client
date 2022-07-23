using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

   // Public Variables
   public float attackAnimDelay = 0.35f;
   
   
   // Public Hidden Variables
   [HideInInspector] public bool isAttacking = false;
   [HideInInspector] public string attackType = "";


   // Private Variables
   private Animator playerAnim;
   

   // ====================================================================================
	// Start
	// ====================================================================================
   private void Start() {
      playerAnim = GetComponent<Animator>();
   }
	

   // ====================================================================================
   // Public Methods
   // ====================================================================================
	public void AttackStrike() {
      if(!isAttacking) {

         isAttacking = true;
         attackType = "strike";
         playerAnim.SetTrigger("attackStrikeLeft");

         StartCoroutine(AttackTimeOut());
      }
	}

   public void AttackEstoc() {
      if(!isAttacking) {

         isAttacking = true;
         attackType = "estoc";

		   playerAnim.SetFloat("attack", 1f);
         // playerAnim.SetTrigger("attackEstocLeft");

         StartCoroutine(AttackTimeOut());
      }
	}


   // ====================================================================================
   // Coroutines
   // ====================================================================================
   IEnumerator AttackTimeOut() {
      yield return new WaitForSeconds(attackAnimDelay);

      isAttacking = false;
      attackType = "";
      playerAnim.SetFloat("attack", 0);

      playerAnim.SetTrigger("idleLeft");
   }
}
