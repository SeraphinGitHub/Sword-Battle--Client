using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProtect : MonoBehaviour {

   // Private Variables
	private PlayerHandler playerHandler;
   private float protectAnimDelay = 0.25f;


   // ====================================================================================
	// Start
	// ====================================================================================
	private void Start() {
		playerHandler = GetComponent<PlayerHandler>();
	}


   // ====================================================================================
   // Public Methods
   // ====================================================================================
	public void Protect() {
      if(!playerHandler.isProtecting && !playerHandler.isAttacking) {

         playerHandler.isProtecting = true;
         playerHandler.SetPlayerAnim("Protect", "Defend");
         StartCoroutine(ProtectTimeOut());
      }
	}

   public void StopProtect() {
      if(playerHandler.isProtecting) {

         playerHandler.isProtecting = false;
		   playerHandler.IdleAnim();
      }
	}


   // ====================================================================================
   // Coroutines
   // ====================================================================================
   IEnumerator ProtectTimeOut() {
      yield return new WaitForSeconds(protectAnimDelay);
      
      if(playerHandler.isProtecting) playerHandler.SetPlayerAnim("Protect", "Protected");
      else {
   		playerHandler.IdleAnim();
         yield break;
      }
   }
}
