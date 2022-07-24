using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

   // Public Variables
   [Header("Attack Delay")]
   public float attackAnimDelay = 0.35f;
      
   
   // Public Hidden Variables
   [HideInInspector] public bool isAttacking = false;
   [HideInInspector] public string attackType = "";

   
   // Private Variables
	private PlayerAnimators playerAnim;
	

   // ====================================================================================
	// Start
	// ====================================================================================
	private void Start() {
		playerAnim = GetComponent<PlayerAnimators>();
	}


   // ====================================================================================
   // Public Methods
   // ====================================================================================
	public void Attack(string type) {
      
      if(!isAttacking) {
         playerAnim.SetAnim("attack", "left"+type, "right"+type);

         isAttacking = true;
         attackType = type;
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
   }
}
