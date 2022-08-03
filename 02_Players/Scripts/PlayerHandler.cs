using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour {
   
   [Header("Player Animators")]
   public Animator playerUIAnimator;
   public Animator[] armAnimators = new Animator[2];
   public Animator[] swordAnimators = new Animator[2];
   public Animator[] shieldAnimators = new Animator[2];
   public Animator[] bodyAnimators = new Animator[2];


   // Public Hidden Variables
   [HideInInspector] public string characterSide;

   [HideInInspector] public float spawnX = 6.5f;
   [HideInInspector] public float spawnY = 0.2f;
   [HideInInspector] public float movePosX;
   [HideInInspector] public float moveSpeed;

   [HideInInspector] public bool isLocalPlayer;
   [HideInInspector] public bool isWalking;
   [HideInInspector] public bool isAttacking;
   [HideInInspector] public bool isProtecting;
   

   // Private Variables
   private string[] sidesArray = new string[] {
      "Left",
      "Right",
   };
   private string[] statesArray;

   private string playerState;
   private string enemyState;
   private int sideIndex;
   private int currentIndex;
   private float enemySpeed;
   
   private GameHandler gameHandler;
   public Vector3 movePosition;


   // ====================================================================================
   // Start / Update
   // ====================================================================================
   private void Start() {
      gameHandler = GameObject.Find("_GameHandler").GetComponent<GameHandler>();
      statesArray = gameHandler.statesArray;
      movePosition = Vector3.zero;
      
      IdleAnim();
   }

   // For enemy player move sync
   private void Update() {
      if(!isLocalPlayer) transform.Translate(movePosition *enemySpeed *Time.deltaTime);
	}


   // ====================================================================================
   // Public Methods
   // ====================================================================================
   public void IdleAnim() {
		SetPlayerAnim("Idle", "Idle");
	}

   public void SetCharacterSide(string side) {

      for(int i = 0; i < sidesArray.Length; i++) {
         if(side == sidesArray[i]) sideIndex = i;
      }
      characterSide = side;
   }

   public void SetSwordColor(string animName) {
      swordAnimators[sideIndex].SetTrigger(animName);
   }

   public void SetPlayerAnim(string behavior, string animName) {

      // Has to match the name of trigger in unity controller
      playerState = characterSide+animName;
      gameHandler.currentState = playerState;

      if(behavior == "Walk" || behavior == "Idle") {
         if(!isAttacking) armAnimators[sideIndex].SetTrigger(playerState);
         if(!isProtecting) shieldAnimators[sideIndex].SetTrigger(playerState);
         bodyAnimators[sideIndex].SetTrigger(playerState);
      }
      if(behavior == "Attack") armAnimators[sideIndex].SetTrigger(playerState);
      if(behavior == "Protect") shieldAnimators[sideIndex].SetTrigger(playerState);
   }

   public void SetEnemyAnim(int newIndex, bool isWalk, bool isAttack, bool isProtect) {

      // 0 => "Idle",
      // 1 => "Forward",
      // 2 => "Backward",
      // 3 => "Estoc",
      // 4 => "Strike",
      // 5 => "Defend",
      // 6 => "Protected",

      if(newIndex != currentIndex) {
         
         currentIndex = newIndex;
         enemyState = characterSide+statesArray[newIndex];

         // Walk or Idle
         if(newIndex == 0 || newIndex == 1 || newIndex == 2) {

            if(!isAttack) armAnimators[sideIndex].SetTrigger(enemyState);
            if(!isProtect) shieldAnimators[sideIndex].SetTrigger(enemyState);
            bodyAnimators[sideIndex].SetTrigger(enemyState);
         }
         
         // Attack
         if(newIndex == 3 || newIndex == 4) armAnimators[sideIndex].SetTrigger(enemyState);
         
         // Protect
         if(newIndex == 5 || newIndex == 6) shieldAnimators[sideIndex].SetTrigger(enemyState);
      }
   }

   public void EnemyMovements(float movePosX, float moveSpeed) {
      		
      enemySpeed = moveSpeed;
      movePosition = new Vector3(movePosX, 0);
   }

}
