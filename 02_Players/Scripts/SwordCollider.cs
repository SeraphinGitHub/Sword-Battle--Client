using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollider : MonoBehaviour {
	
   // Public Variables
	public GameObject playerPf;


   // Public Hidden Varaiables
   [HideInInspector] public GameObject enemyPlayer;
   [HideInInspector] public PlayerHandler localPH;
   [HideInInspector] public PlayerHandler enemyPH;


   // Private Variables
   private GameHandler gameHandler;


   // ===========================================================================================================
	// Start
	// ===========================================================================================================
   private void Start() {
      gameHandler = GameObject.Find("_GameHandler").GetComponent<GameHandler>();
   }
   

   // ===========================================================================================================
	// Private Methods
	// ===========================================================================================================
   private void OnTriggerEnter2D(Collider2D col) {

      if(localPH.isAttacking && enemyPlayer != null) {
         if(col.gameObject == enemyPlayer
         || col.gameObject == enemyPH.shieldColliders[enemyPH.sideIndex]) {

            DamagingPlayer();
         }
	   }
	}

   private void DamagingPlayer() {

      if(!localPH.isEnemyDamaged) {
         localPH.isEnemyDamaged = true;

         if(!enemyPH.isProtecting) enemyPH.GetDamageHealth(localPH.damagesValue);
         else enemyPH.GetDamageShield(localPH.damagesValue);
         
         // Damage Done
         gameHandler.ToggleDmgText(gameHandler.damageDoneTMP, localPH.damagesValue);
      }
   }

}