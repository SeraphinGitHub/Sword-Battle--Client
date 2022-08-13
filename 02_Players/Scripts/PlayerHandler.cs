using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PlayerHandler : MonoBehaviour {
   
   // Public Variables
   [Header("**Player Animators**")]
   public Animator[] swordAnimators = new Animator[2];
   public Animator[] armAnimators = new Animator[2];
   public Animator[] shieldAnimators = new Animator[2];
   public Animator[] bodyAnimators = new Animator[2];


   // Public Hidden Variables
   [HideInInspector] public float spawnX = 6.5f;
   [HideInInspector] public float spawnY = 0.2f;
   [HideInInspector] public float movePosX;
   [HideInInspector] public float playerSpeed;
   [HideInInspector] public float damagesValue;

   [HideInInspector] public bool isLocalPlayer;
   [HideInInspector] public bool isWalking;
   [HideInInspector] public bool isAttacking;
   [HideInInspector] public bool isProtecting;
   [HideInInspector] public bool isEnemyDamaged;

   [HideInInspector] public string walkDirection;
   [HideInInspector] public string characterSide;
   [HideInInspector] public int sideIndex;

   [HideInInspector] public Vector3 playerMovePosition;
   [HideInInspector] public GameHandler gameHandler;

   // Private Variables
   private Animator[][] animatorsArray;
   private string[] sidesArray = new string[] {
      "Left",
      "Right",
   };
   private string[] bodyPartArray = new string[] {
      "Sword",
      "Shield",
      "Body",
   };
   private string[] statesArray = new string[] {
      "Idle",
      "Forward",
      "Backward",
      "Estoc",
      "Strike",
      "Defend",
      "Protected",
   };
   private int[] currentStatesArray = new int[] {-1, -1, -1};

   private string playerState;
   private string enemyState;
   private float enemySpeed;
   
   private GameObject optionsMenu;
   private Vector3 enemyMovePosition;

   
   // ====================================================================================
   // Start / Update
   // ====================================================================================
   private void Start() {
      gameHandler = GameObject.Find("_GameHandler").GetComponent<GameHandler>();
      enemyMovePosition = Vector3.zero;
      
      SetPlayerAnim("Body", "Idle");
      SetPlayerAnim("Sword", "Idle");
      SetPlayerAnim("Shield", "Idle");
   }

   // For enemy player move sync
   private void Update() {
		if(isLocalPlayer) transform.Translate(playerMovePosition *playerSpeed *Time.deltaTime);
      if(!isLocalPlayer) transform.Translate(enemyMovePosition *enemySpeed *Time.deltaTime);
	}


   // ====================================================================================
   // Public Methods
   // ====================================================================================
   public void SetCharacterSide(string side) {

      animatorsArray = new Animator[][] {
         armAnimators,
         shieldAnimators,
         bodyAnimators,
      };
      
      characterSide = side;
      sideIndex = System.Array.IndexOf(sidesArray, side);
   }

   public void SetSwordColor(string animName) {
      swordAnimators[sideIndex].SetTrigger(animName);
   }

   public void SetPlayerAnim(string bodyPart, string animName) {

      // ** StateArray **        ** StatesIndexArray **        ** BodyPartArray ** 
      // 0 => "Idle",            0 => 0 to 6,                  0 => "Sword",
      // 1 => "Forward",         1 => 0 to 6,                  1 => "Shield",
      // 2 => "Backward",        2 => 0 to 6,                  2 => "Body",
      // 3 => "Estoc",
      // 4 => "Strike",
      // 5 => "Defend",
      // 6 => "Protected",

      // playerState ==> Has to match the name of trigger in unity controller
      playerState = characterSide+animName;

      for(int i = 0; i < bodyPartArray.Length; i++) {

         if(bodyPart == bodyPartArray[i]) {
            gameHandler.statesIndexArray[i] = System.Array.IndexOf(statesArray, animName);
            animatorsArray[i][sideIndex].SetTrigger(playerState);
         }
      }
   }

   public void SetEnemyAnim(int[] statesIndexArray, bool attack, bool protect) {

      for(int i = 0; i < bodyPartArray.Length; i++) {

         int newStateIndex = statesIndexArray[i];
         int currentStateIndex = currentStatesArray[i];

         if(newStateIndex != currentStateIndex) {

            currentStatesArray[i] = newStateIndex;
            enemyState = characterSide+statesArray[newStateIndex];
            animatorsArray[i][sideIndex].SetTrigger(enemyState);
         }
      }
   }

   public void EnemyMovements(float movePosX, float playerSpeed) {  		
      enemySpeed = playerSpeed;
      enemyMovePosition = new Vector3(movePosX, 0);
   }

}
