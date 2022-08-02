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
   [HideInInspector] public float moveSpeed;
   [HideInInspector] public bool isWalking;
   [HideInInspector] public bool isAttacking;
   [HideInInspector] public bool isProtecting;
   
   [HideInInspector] public float localPosX;
   

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
   private Vector3 currentPosition;
   private Vector3 newPosition;
   private Vector3 moveDir;
   private Rigidbody2D rb;

   [HideInInspector] public bool aze;


   // ====================================================================================
   // Start / Fixed Update
   // ====================================================================================
   private void Start() {
      gameHandler = GameObject.Find("_GameHandler").GetComponent<GameHandler>();
      statesArray = gameHandler.statesArray;
      currentPosition = transform.position;
      rb = GetComponent<Rigidbody2D>();
      IdleAnim();

      Debug.Log(characterSide);
   }

   private void FixedUpdate() {

      if(aze) {
         transform.position = new Vector3(localPosX, transform.position.y);
      }
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
         if(behavior == "Walk") bodyAnimators[sideIndex].SetTrigger(playerState);
         else if(!isWalking)bodyAnimators[sideIndex].SetTrigger(playerState);
      }
      if(behavior == "Attack") armAnimators[sideIndex].SetTrigger(playerState);
      if(behavior == "Protect") shieldAnimators[sideIndex].SetTrigger(playerState);
   }

   public void SetEnemyAnim(
   int newIndex,
   bool isWalk,
   bool isAttack,
   bool isProtect) {

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

            Debug.Log(enemyState);

            armAnimators[sideIndex].SetTrigger(enemyState);
            if(!isProtect) shieldAnimators[sideIndex].SetTrigger(enemyState);
            if(newIndex == 1 || newIndex == 2) bodyAnimators[sideIndex].SetTrigger(enemyState);
            else bodyAnimators[sideIndex].SetTrigger(enemyState);
         }
         
         // Attack
         if(newIndex == 3 || newIndex == 4) armAnimators[sideIndex].SetTrigger(enemyState);
         
         // Protect
         if(newIndex == 5 || newIndex == 6) shieldAnimators[sideIndex].SetTrigger(enemyState);
      }
   }

   public void EnemyMovements(float posX, float moveSpeed) {
      
      // if(posX != localPosX) {
         posX = localPosX;
         // enemySpeed = moveSpeed;
         // newPosition = new Vector3(posX, transform.position.y);
         // currentPosition = newPosition;
      // }
   }

}
