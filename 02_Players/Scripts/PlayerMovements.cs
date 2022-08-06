using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


	// ******************************************
	private void Update() {
		if(Input.GetKeyDown("q")) MoveLeft();
		if(Input.GetKeyDown("d")) MoveRight();
		if(Input.GetKeyUp("q") || Input.GetKeyUp("d")) StopMove();
	}
	// ******************************************
	


	// =========================================================================================================
	// Public Methods
	// =========================================================================================================
	public void MoveLeft() {

		pH.movePosX = -1f;
		SetMovementsAnim(backwardSpeed, "Left", "Backward");
		SetMovementsAnim(forwardSpeed, "Right", "Forward");
	}

	public void MoveRight() {

		pH.movePosX = 1f;
		SetMovementsAnim(forwardSpeed, "Left", "Forward");
		SetMovementsAnim(backwardSpeed, "Right", "Backward");
	}

	public void StopMove() {

		pH.isWalking = false;
		pH.movePosX = 0;
		pH.playerMovePosition = new Vector3(pH.movePosX, 0);

		pH.SetPlayerAnim("Body", "Idle");
      pH.SetPlayerAnim("Sword", "Idle");
      pH.SetPlayerAnim("Shield", "Idle");
	}


	// =========================================================================================================
	// Private Methods
	// =========================================================================================================
	private void SetMovementsAnim(float speed, string side, string animName) {
		if(pH.characterSide == side) {

			pH.isWalking = true;
			pH.walkDirection = animName;
			pH.playerSpeed = speed;
			pH.playerMovePosition = new Vector3(pH.movePosX, 0);
			
			pH.SetPlayerAnim("Body", animName);
			pH.SetPlayerAnim("Sword", animName);
			pH.SetPlayerAnim("Shield", animName);
		}
	}
}
