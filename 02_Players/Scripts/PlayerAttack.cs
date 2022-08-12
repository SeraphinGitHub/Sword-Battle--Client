using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerAttack : MonoBehaviour {

   public LayerMask enemyLayer;

   // Public Variables
   [Header("**Strike Attack Options**")]
   public Transform[] strikePosArray = new Transform[2];
   public float strikeDamages = 220f;
	public float strikeDelay = 0.3f;
   
   [Header("**Estoc Attack Options**")]
   public Transform[] estocPosArray = new Transform[2];
   public float estocDamages = 150f;
	public float estocDelay = 0.3f;


   // Private Variables
   private float animDoneDelay = 0.4f;
   private float strikeRange;
   private float estocRange;

	private Vector2 strikePoint;
	private Vector2 estocPoint;
	
   private GameRandomize gameRandomize;
   private PlayerHandler pH;
	

   // ====================================================================================
	// Start
	// ====================================================================================
	private void Start() {
		gameRandomize = GameObject.Find("_GameHandler").GetComponent<GameRandomize>();
		pH = GetComponent<PlayerHandler>();

      Transform strikePos = strikePosArray[pH.sideIndex];
      strikePoint = new Vector2(strikePos.position.x, strikePos.position.y);
      strikeRange = strikePos.GetComponent<AttackPoint>().attackRange;

      Transform estocPos = estocPosArray[pH.sideIndex];
      estocPoint = new Vector2(estocPos.position.x, estocPos.position.y);
      estocRange = estocPos.GetComponent<AttackPoint>().attackRange;
	}


   // ====================================================================================
   // Public Methods
   // ====================================================================================
	public void Ev_StrikeAttack(object sender, EventArgs e) {
      if(!pH.isProtecting && !pH.isAttacking) {
            
         pH.isAttacking = true;
         pH.SetPlayerAnim("Sword", "Strike");
         StartCoroutine( AttackTimeOut() );
         StartCoroutine( Damaging(strikePoint, strikeDamages, strikeRange, strikeDelay) );
      }
	}

   public void Ev_EstocAttack(object sender, EventArgs e) {
      if(!pH.isProtecting && !pH.isAttacking) {
         
         pH.isAttacking = true;
         pH.SetPlayerAnim("Sword", "Estoc");
         StartCoroutine( AttackTimeOut() );
         StartCoroutine( Damaging(estocPoint, estocDamages, estocRange, estocDelay) );
      }
	}


   // ====================================================================================
   // Coroutines
   // ====================================================================================
   IEnumerator AttackTimeOut() {
      yield return new WaitForSeconds(animDoneDelay);

      if(pH.isAttacking) pH.isAttacking = false;
      if(pH.isWalking) pH.SetPlayerAnim("Sword", pH.walkDirection);
      else pH.SetPlayerAnim("Sword", "Idle");
   }

   IEnumerator Damaging(Vector2 point, float damages, float range, float delay) {
      yield return new WaitForSeconds(delay);

      Collider2D[] enemyPlayer = Physics2D.OverlapCircleAll(point, range, enemyLayer);
      
      foreach(var enemy in enemyPlayer) {
         bool isLocalPlayer = enemy.GetComponent<PlayerHandler>().isLocalPlayer;

         if(enemy && enemy != gameRandomize.localPlayer) {
            enemy.GetComponent<PlayerHealth>().GetDamage(damages);
            Debug.Log("Touch enemy !");
         }
      }


      Debug.Log("Damaging");
   }
}
