﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour {

	[Header("MoveSpeed")]
	public float forwardSpeed = 8f;
	public float backwardSpeed = 4f;
	public bool isLocalPlayer = true;
	public bool isMovingLeft;
	public bool isMovingRight;

	// Private variables
	private float moveSpeed;
	private Animator playerAnim;
	private Rigidbody2D rb;
	private Vector3 movement;
	private int randTeam = 1; // Temporary set to 1, otherwise no value;


	// ====================================================================================
	// Start
	// ====================================================================================
	private void Start() {
		playerAnim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
				
		if(randTeam <= 1 ) {
			playerAnim.SetBool("isPlayer_Left", true);
		}

		if(randTeam > 1 ) {
			playerAnim.SetBool("isPlayer_Right", true);
		}
		
		if(isLocalPlayer) {
			isMovingLeft = false;
			isMovingRight = false;
		}
	}


	// ====================================================================================
	// Fixed Update
	// ====================================================================================
	private void FixedUpdate() {
		rb.MovePosition(transform.position +movement *moveSpeed *Time.fixedDeltaTime);
	}
	

	// =========================================================================================================
	// Move Left
	// =========================================================================================================
	public void LeftON() {
		if(isLocalPlayer) {

			// Player is on Left Side
			if(randTeam <= 1 ) {
				isMovingLeft = true;

				moveSpeed = backwardSpeed;
				movement = new Vector3( -1f, 0);
			}

			// Player is on Right Side
			if(randTeam > 1 ) {
				isMovingLeft = true;
				
				moveSpeed = forwardSpeed;
				movement = new Vector3( -1f, 0);
			}
		}
	}
	
	// On Move Left Button Exit
	public void LeftOFF() {

		if(isLocalPlayer) {
			isMovingLeft = false;
			movement = new Vector3( 0, 0);
		}
	}


	// =========================================================================================================
	// Move Right
	// =========================================================================================================
	public void RightON() {
		if(isLocalPlayer) {

			// Player is on Left Side
			if(randTeam <= 1 ) {
				isMovingRight = true;

				moveSpeed = forwardSpeed;
				movement = new Vector3( 1f, 0);
			}

			// Player is on Right Side
			if(randTeam > 1 ) {
				isMovingRight = true;
				
				moveSpeed = backwardSpeed;
				movement = new Vector3( 1f, 0);
			}
		}
	}
	
	// On Move Right Button Exit
	public void RightOFF() {

		if(isLocalPlayer) {
			isMovingRight = false;
			movement = new Vector3( 0, 0);
		}
	}
}
