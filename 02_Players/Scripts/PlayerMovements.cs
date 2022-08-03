using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour {

	// Private variables
	private float forwardSpeed = 6f;
	private float backwardSpeed = 4f;
	private float playerSpeed;
	
	private Animator playerAnim;
	private Vector3 movePosition;
	private PlayerHandler playerHandler;


	// ====================================================================================
	// Start / Update
	// ====================================================================================
	private void Start() {
		playerAnim = GetComponent<Animator>();
		playerHandler = GetComponent<PlayerHandler>();
	}

	private void Update() {
		transform.Translate(movePosition *playerSpeed *Time.deltaTime);
	}
	

	// =========================================================================================================
	// Public Methods
	// =========================================================================================================
	public void MoveLeft() {

		movePosition = new Vector3(-1f, 0);
		playerHandler.movePosX = -1f;
		SetMoveAnim(backwardSpeed, "Left", "Backward");
		SetMoveAnim(forwardSpeed, "Right", "Forward");
	}

	public void MoveRight() {

		movePosition = new Vector3(1f, 0);
		playerHandler.movePosX = 1f;
		SetMoveAnim(forwardSpeed, "Left", "Forward");
		SetMoveAnim(backwardSpeed, "Right", "Backward");
	}

	public void StopMove() {

		movePosition = new Vector3(0, 0);
		playerHandler.movePosX = 0;
		playerHandler.isWalking = false;
		playerHandler.IdleAnim();
	}


	// =========================================================================================================
	// Private Methods
	// =========================================================================================================
	private void SetMoveAnim(float speed, string side, string animName) {

		if(side == playerHandler.characterSide) {
			playerSpeed = speed;
			playerHandler.moveSpeed = speed;
			playerHandler.isWalking = true;
			playerHandler.SetPlayerAnim("Walk", animName);
		}
	}
}
