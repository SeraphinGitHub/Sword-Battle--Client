using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour {

	[Header("MoveSpeed")]
	public float forwardSpeed = 8f;
	public float backwardSpeed = 4f;

	// Private variables
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

		if(playerHandler.characterSide == "Left") moveSpeed = backwardSpeed;
		if(playerHandler.characterSide == "Right") moveSpeed = forwardSpeed;
	}

	public void MoveRight() {
		movement = new Vector3(1f, 0);
		
		// playerAnim.SetFloat("walk", 1f);
		// playerAnim.SetBool("moveRight", true);

		if(playerHandler.characterSide == "Left") moveSpeed = forwardSpeed;
		if(playerHandler.characterSide == "Right") moveSpeed = backwardSpeed;
	}

	public void StopMove() {

		// playerAnim.SetFloat("walk", 0);
		movement = new Vector3(0, 0);
	}
}
