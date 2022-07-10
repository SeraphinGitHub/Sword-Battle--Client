using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System; 	// <<< Needed for Events

public class HealthSystem : MonoBehaviour {
	
	public event EventHandler OnDeath;
	
	[SerializeField] private GameObject destroyOnDeathObj = default;
	[SerializeField] private float deathAnimDuration = default;
	
	// Health values
	[SerializeField] private int healthMax = 100;
	[HideInInspector] public int health;
	
	private float healthPercent() {
		return (float)health / healthMax;
	}	
	
	// Health Bar States Colors
	[SerializeField] private  Color green = default;  //(30f, 220f, 0f, 255f)
	[SerializeField] private  Color yellow = default; //(235f, 235f, 0f, 255f)
	[SerializeField] private  Color orange = default; //(255f, 120f, 0f, 255f)
	[SerializeField] private  Color red = default;	  //(255f, 0f, 0f, 255f)
	
	private Animator anim;
	[SerializeField] private SpriteRenderer lifeSprite = default;
	
	// Start
	private void Start() {
		anim = GetComponent<Animator>();
		health = healthMax;
		OnDeath += Event_OnDeath;
    }
	
	
	// ===========================================================================================================
	// On Death Event
	// ===========================================================================================================
	
	public void Event_OnDeath(object sender, EventArgs e) {
		anim.SetTrigger("Die");
		
		StartCoroutine(Co_DeathTimer());
	}
	
	IEnumerator Co_DeathTimer() {
		yield return new WaitForSeconds(deathAnimDuration);
		Destroy(destroyOnDeathObj);
	}	
	
	
	
	// ===========================================================================================================
	// Deleted
	// ===========================================================================================================
	
	public void Deleted() {
		health = 0;
		LifeColorChange();
		
		if(health <= 0 && OnDeath != null) {
			OnDeath(this, EventArgs.Empty);
		}
		
		/* Debug.Log("Worker Health " + healthPercent() * healthMax); */
	}


	// ===========================================================================================================
	// Damage To Units
	// ===========================================================================================================
	
	// Damage To Workers
	public void Worker_Damage_Worker(int workerToUnitsDamage) {
		health -= workerToUnitsDamage;
		LifeColorChange();
		
		if(health <= 0 && OnDeath != null) {
			OnDeath(this, EventArgs.Empty);
		}
		
		/* Debug.Log("Worker Health " + healthPercent() * healthMax); */
	}
	
	
	// ===========================================================================================================
	// Health Bar change colors on percentage 
	// ===========================================================================================================
	
	private void LifeColorChange() {
		
		// Change the size of the bar depend on health
		transform.Find("HealthBar/Health_Sprites").localScale = new Vector3(healthPercent(), 1);
	
		// Maximu scale life bar
		if(healthPercent() <= 0 ) {	
			transform.Find("HealthBar/Health_Sprites").localScale = new Vector3(0, 1);
		}
		
		// Minimum scale life bar
		if(healthPercent() >= 1 ) {
			transform.Find("HealthBar/Health_Sprites").localScale = new Vector3(1, 1);
		}
		
		// LifeBars Colors		
		if(healthPercent() > 0.7 ) {
			lifeSprite.color = green; //100%
		}

		if(healthPercent() <= 0.7 && healthPercent() > 0.41 ) {
			lifeSprite.color = yellow; //70%
		}
	
		if(healthPercent() <= 0.41 && healthPercent() > 0.21 ) {
			lifeSprite.color = orange; //40%
		}

		if(healthPercent() <= 0.21 ) {
			lifeSprite.color = red; //20%
		}
	}

}