using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour {
	
	// Public Hidden Variables
	[HideInInspector] public float health;

	// Private Variables
	private float healthMax = 1000f;
	private int coeff = 100;

	private GameHandler gameHandler;
	private PlayerHandler pH;
	private Transform[] healthSpritesArray;
	

	// ===========================================================================================================
	// Start
	// ===========================================================================================================
	private void Start() {
      gameHandler = GameObject.Find("_GameHandler").GetComponent<GameHandler>();
		pH = GetComponent<PlayerHandler>();

		healthSpritesArray = gameHandler.healthSpritesArray;
		health = healthMax;
		HealthBarChange();
	}
	
	
	// ===========================================================================================================
	// Public Methods
	// ===========================================================================================================
	public void GetDamage(float damagesValue) {

		if(health > 0) health -= damagesValue;
		HealthBarChange();
	}
	

	// ===========================================================================================================
	// Private Methods
	// ===========================================================================================================
	private float healthPercent() {
		return Mathf.Floor(health / healthMax *coeff) /coeff;
	}

	private void HealthBarChange() {

		healthSpritesArray[pH.sideIndex].localScale = new Vector3(healthPercent(), 1, 1);
		if(healthPercent() <= 0 ) healthSpritesArray[pH.sideIndex].localScale = new Vector3(0, 1, 1);
	}
}
