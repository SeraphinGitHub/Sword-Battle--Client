using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovements : MonoBehaviour {

	// Private variables
	private float forwardSpeed = 9f;
	private float backwardSpeed = 6f;
	
	private Animator playerAnim;
	private PlayerHandler pH;


	// ====================================================================================
	// Start / Update
	// ====================================================================================
	private void Start() {
		pH = GetComponent<PlayerHandler>();
		playerAnim = GetComponent<Animator>();
	}	


	// =========================================================================================================
	// Public Methods
	// =========================================================================================================
	public void Ev_MoveLeft(object sender, EventArgs e) {

		pH.movePosX = -1f;
		SetMovementsAnim("Left", backwardSpeed, "Backward");
		SetMovementsAnim("Right", forwardSpeed, "Forward");
	}

	public void Ev_MoveRight(object sender, EventArgs e) {

		pH.movePosX = 1f;
		SetMovementsAnim("Left", forwardSpeed, "Forward");
		SetMovementsAnim("Right", backwardSpeed, "Backward");
	}

	public void Ev_StopMove(object sender, EventArgs e) {

		pH.isWalking = false;
		pH.movePosX = 0;
		pH.playerMovePosition = new Vector3(pH.movePosX, 0);
		BodyPartAnim("Idle");
	}


	// =========================================================================================================
	// Private Methods
	// =========================================================================================================
	private void SetMovementsAnim(string side, float speed, string animName) {
		if(pH.characterSide == side) {

			pH.isWalking = true;
			pH.walkDirection = animName;
			pH.playerSpeed = speed;
			pH.playerMovePosition = new Vector3(pH.movePosX, 0);
			
			BodyPartAnim(animName);
		}
	}

	private void BodyPartAnim(string animName) {
		
		pH.SetPlayerAnim("Body", animName);
		if(!pH.isAttacking) pH.SetPlayerAnim("Sword", animName);
		if(!pH.isProtecting) pH.SetPlayerAnim("Shield", animName);
	}
}
