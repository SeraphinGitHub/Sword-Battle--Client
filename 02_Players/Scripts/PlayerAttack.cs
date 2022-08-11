using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerAttack : MonoBehaviour {

   // Public Variables
   [Header("**Strike Attack Options**")]
   public Vector2 strikePoint = new Vector2(0f, 0f);
   public float strikeDamages = 220f;
	public float strikeRange = 2.2f;
	public float strikeDelay = 0.3f;
   
   [Header("**Estoc Attack Options**")]
   public Vector2 estocPoint = new Vector2(0f, 0f);
   public float estocDamages = 150f;
	public float estocRange = 2.2f;
	public float estocDelay = 0.3f;


   // Private Variables
	private PlayerHandler pH;
   private float animDoneDelay = 0.4f;

	private float endAttackTime;
	

   // ====================================================================================
	// Start
	// ====================================================================================
	private void Start() {
		pH = GetComponent<PlayerHandler>();
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
   // Private Methods
   // ====================================================================================
   private void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(strikePoint, strikeRange);

      Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(estocPoint, estocRange);
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

      Collider2D enemyPlayer = Physics2D.OverlapCircle(point, range);
      enemyPlayer.GetComponent<PlayerHealth>().getDamage(damages);
   }
}
