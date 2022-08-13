using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollider : MonoBehaviour {
	
   // Public Variables
	public GameObject playerPf;
	

   // Private Variables
   private GameRandomize gameRandomize;
   private PlayerHealth enemyPHealth;
   private PlayerHandler pH;


   // ===========================================================================================================
	// Start
	// ===========================================================================================================
   private void Start() {
      gameRandomize = GameObject.Find("_GameHandler").GetComponent<GameRandomize>();
      pH = playerPf.GetComponent<PlayerHandler>();
   }


   // ===========================================================================================================
	// Private Methods
	// ===========================================================================================================
   private void OnTriggerEnter2D(Collider2D col) {
      
		if(pH.isAttacking  && col.gameObject == gameRandomize.enemyPlayer) {
         enemyPHealth = gameRandomize.enemyPlayer.GetComponent<PlayerHealth>();
         
         if(!pH.isEnemyDamaged) {

            pH.isEnemyDamaged = true;
            enemyPHealth.GetDamage(pH.damagesValue);
         }
	   }
	}
}