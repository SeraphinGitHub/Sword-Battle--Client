using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour {

	[Header("MoveSpeed")]
	public float forwardSpeed = 8f;
	public float backwardSpeed = 4f;

	// Private variables
	private float moveSpeed;
	private string playerSide;
	private GameObject gameHandler;
	
	private Animator playerAnim;
	private Rigidbody2D rb;
	private Vector3 movement;


	// ====================================================================================
	// Start
	// ====================================================================================
	private void Start() {
		playerAnim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
				
		gameHandler = GameObject.Find("_GameHandler");
		playerSide = gameHandler.GetComponent<GameHandler>().playerSide;

		if(playerSide == "Left") playerAnim.SetBool("isPlayer_Left", true);
		if(playerSide == "Right") playerAnim.SetBool("isPlayer_Right", true);
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

		if(playerSide == "Left") moveSpeed = backwardSpeed;
		if(playerSide == "Right") moveSpeed = forwardSpeed;
	}

	public void MoveRight() {
		movement = new Vector3(1f, 0);
		
		playerAnim.SetFloat("walk", 1f);
		// playerAnim.SetBool("moveRight", true);

		if(playerSide == "Left") moveSpeed = forwardSpeed;
		if(playerSide == "Right") moveSpeed = backwardSpeed;
	}

	public void StopMove() {

		playerAnim.SetFloat("walk", 0);
		movement = new Vector3(0, 0);
	}
}
