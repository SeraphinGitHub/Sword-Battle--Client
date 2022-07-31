using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour {

	// Private variables
	private float forwardSpeed = 6f;
	private float backwardSpeed = 4f;
	private float moveSpeed;
	
	private Animator playerAnim;
	private Rigidbody2D rb;
	private Vector3 movement;

	private PlayerHandler playerHandler;


	// ====================================================================================
	// Start
	// ====================================================================================
	private void Start() {
		playerAnim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		playerHandler = GetComponent<PlayerHandler>();

		playerHandler.SetAnim("Idle", "Idle");
	}


	// ====================================================================================
	// Fixed Update
	// ====================================================================================
	private void FixedUpdate() {
		rb.MovePosition(transform.position +movement *moveSpeed *Time.fixedDeltaTime);
	}
	

	// =========================================================================================================
	// Public Methods
	// =========================================================================================================
	public void MoveLeft() {

		movement = new Vector3(-1f, 0);
		SetMoveAnim(backwardSpeed, "Left", "Backward");
		SetMoveAnim(forwardSpeed, "Right", "Forward");
	}

	public void MoveRight() {

		movement = new Vector3(1f, 0);
		SetMoveAnim(forwardSpeed, "Left", "Forward");
		SetMoveAnim(backwardSpeed, "Right", "Backward");
	}

	public void StopMove() {

		playerHandler.SetAnim("Idle", "Idle");
		movement = new Vector3(0, 0);
	}


	// =========================================================================================================
	// Private Methods
	// =========================================================================================================
	private void SetMoveAnim(float speed, string side, string animName) {

		if(playerHandler.characterSide == side) {
			moveSpeed = speed;
			playerHandler.SetAnim("Walk", animName);
		}
	}
}
