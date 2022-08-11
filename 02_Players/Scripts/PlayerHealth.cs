using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour {
	
	// Public Variables
    public Transform[] healthSprites = new Transform[2];

    
    // Private Variables
	private float healthMax = 1000f;
	private float health;
	private int coeff = 100;

	private PlayerHandler pH;
	

	// ===========================================================================================================
	// Start
	// ===========================================================================================================
	private void Start() {
		pH = GetComponent<PlayerHandler>();
		health = healthMax;
    }
	
	
	// ===========================================================================================================
	// Public Methods
	// ===========================================================================================================
    public void getDamage(float damagesValue) {
        health -= damagesValue;
        ResizeHealthBar();
    }


    // ===========================================================================================================
	// Private Methods
	// ===========================================================================================================
    private float healthPercent() {
		return Mathf.Floor(health / healthMax *coeff) /coeff;
    }

    private void ResizeHealthBar() {
        healthSprites[pH.sideIndex].localScale = new Vector3(healthPercent(), 1);
        if(healthPercent() <= 0 ) healthSprites[pH.sideIndex].localScale = new Vector3(0, 1);
	}
}
