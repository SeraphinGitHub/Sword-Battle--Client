using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerAttackSystem : MonoBehaviour {	
	
	public int workerToUnitsDamage = 20;
	
	// Attack variables =====================================
	[SerializeField] private float attackRange = 2.2f;
	[SerializeField] private Transform attackPoint = default;
	[SerializeField] private float attackAnimDuration = 1.5f;
	[SerializeField] private float startAttackTime = 0.3f;
	[SerializeField] private float timeBetweenAttack = 3.0f;
	private float endAttackTime;
	
	// Team Colors
	[SerializeField] private LayerMask UnitColor = default;
	// ======================================================
	
	[SerializeField] private GameObject front = default;  //Temporary, delete later !
	private Animator enemyAnim;
	
	// Start
	private void Start() {
		endAttackTime = attackAnimDuration - startAttackTime;
		front.SetActive(false);
		enemyAnim = GetComponent<Animator>();
		StartCoroutine(Co_AttackCycle());
	}
	
	IEnumerator Co_AttackCycle() {
		enemyAnim.SetBool("Attack", true);
		yield return new WaitForSeconds(startAttackTime);
		Attack();
		yield return new WaitForSeconds(endAttackTime);
		enemyAnim.SetBool("Attack", false);
		yield return new WaitForSeconds(timeBetweenAttack);
		StartCoroutine(Co_AttackCycle());
	}
	
	
	private void Attack() {
	
		// Damaging Units
		Collider2D[] unitsToDamage = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, UnitColor);
		
		foreach(Collider2D units in unitsToDamage) {
			units.GetComponent<HealthSystem>().Worker_Damage_Worker(workerToUnitsDamage);
		}
	}
	
	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(attackPoint.position, attackRange);
	}
}