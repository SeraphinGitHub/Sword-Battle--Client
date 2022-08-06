using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProtect : MonoBehaviour {

   // Private Variables
	private PlayerHandler pH;
   private float protectAnimDelay = 0.125f;


   // ====================================================================================
	// Start
	// ====================================================================================
	private void Start() {
		pH = GetComponent<PlayerHandler>();
	}


   // ====================================================================================
   // Public Methods
   // ====================================================================================
	public void Protect() {
      if(!pH.isProtecting && !pH.isAttacking) {

         pH.isProtecting = true;
         pH.SetPlayerAnim("Shield", "Defend");

         StartCoroutine(ProtectTimeOut());
      }
	}

   public void StopProtect() {
      if(pH.isProtecting) {

         pH.isProtecting = false;
         if(!pH.isWalking) pH.SetPlayerAnim("Shield", "Idle");
         else pH.SetPlayerAnim("Shield", pH.walkDirection);
      }
	}


   // ====================================================================================
   // Coroutines
   // ====================================================================================
   IEnumerator ProtectTimeOut() {
      yield return new WaitForSeconds(protectAnimDelay);
      
      if(pH.isProtecting) pH.SetPlayerAnim("Shield", "Protected");
      else yield break;
   }
}
