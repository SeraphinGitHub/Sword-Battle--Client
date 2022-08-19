using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PlayerHandler : MonoBehaviour {

   // Public Variables
   [Header("**Swords Colliders**")]
   public GameObject[] swordColliders = new GameObject[2];
   
   [Header("**Shields Colliders**")]
   public GameObject[] shieldColliders = new GameObject[2];

   [Header("**Damages Values**")]
   public float strikeDamages = 220f;
   public float estocDamages = 150f;
   public float strikeDelay = 0.25f;
   public float estocDelay = 0.125f;

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
   [HideInInspector] public float attackDelay;
   [HideInInspector] public float damagesValue;
   [HideInInspector] public float playerHealth;
   [HideInInspector] public float shieldHealth;

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
   private Image[] healthSpritesArray;
	private Image[] shieldSpritesArray;

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

	private int coeff = 100;
   private float playerMaxHealth = 2000f;
   private float shieldMaxHealth = 1000f;
   private float forwardSpeed = 9f;
	private float backwardSpeed = 6f;
   private float enemySpeed;
   private float attackAnimDelay = 0.4f;
   private float protectAnimDelay = 0.125f;
   private float currentDamages = 0f;
   
	private Animator playerAnim;
   private Vector3 enemyMovePosition;
   private GameObject gameHandlerObj;
   private GameObject optionsMenu;
   private GameObject swordCol;
   private GameObject shieldCol;

   
   // ====================================================================================
   // Start / Update
   // ====================================================================================
   private void Start() {
      gameHandlerObj = GameObject.Find("_GameHandler");
      gameHandler = gameHandlerObj.GetComponent<GameHandler>();
		playerAnim = GetComponent<Animator>();

      swordCol = swordColliders[sideIndex];
      shieldCol = shieldColliders[sideIndex];
      swordCol.SetActive(false);
      shieldCol.SetActive(false);

      enemyMovePosition = Vector3.zero;

		healthSpritesArray = gameHandler.healthSpritesArray;
		shieldSpritesArray = gameHandler.shieldSpritesArray;

		playerHealth = playerMaxHealth;
		shieldHealth = shieldMaxHealth;

		HealthBarChange();
      ShieldBarChange();

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

   // Player Anim
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

   // Player Movements
   public void Ev_MoveLeft(object sender, EventArgs e) {

		movePosX = -1f;
		SetMovementsAnim("Left", backwardSpeed, "Backward");
		SetMovementsAnim("Right", forwardSpeed, "Forward");
	}

	public void Ev_MoveRight(object sender, EventArgs e) {

		movePosX = 1f;
		SetMovementsAnim("Left", forwardSpeed, "Forward");
		SetMovementsAnim("Right", backwardSpeed, "Backward");
	}

	public void Ev_StopMove(object sender, EventArgs e) {

		isWalking = false;
		movePosX = 0;
		playerMovePosition = new Vector3(movePosX, 0);
		BodyPartAnim("Idle");
	}

   // Player Attack
   public void Ev_StrikeAttack(object sender, EventArgs e) {
      if(!isProtecting && !isAttacking) {
         
         SetPlayerAnim("Sword", "Strike");
         damagesValue = strikeDamages;
         StartCoroutine( AttackTimeOut(strikeDelay) );
      }
	}

   public void Ev_EstocAttack(object sender, EventArgs e) {
      if(!isProtecting && !isAttacking) {
         
         SetPlayerAnim("Sword", "Estoc");
         damagesValue = estocDamages;
         StartCoroutine( AttackTimeOut(estocDelay) );
      }
	}

   // Player Protect
   public void Ev_Protect(object sender, EventArgs e) {
      if(!isProtecting && !isAttacking) {

         isProtecting = true;
         shieldCol.SetActive(true);
         SetPlayerAnim("Shield", "Defend");
         StartCoroutine(ProtectTimeOut());
      }
	}

   public void Ev_StopProtect(object sender, EventArgs e) {
      if(isProtecting) {

         isProtecting = false;
         shieldCol.SetActive(false);
         if(!isWalking) SetPlayerAnim("Shield", "Idle");
         else SetPlayerAnim("Shield", walkDirection);
      }
	}

   // Player Health / Shield
   public void GetDamageHealth(float damagesValue) {
		
      if(playerHealth >= damagesValue) playerHealth -= damagesValue;
      else playerHealth = 0;
		
      HealthBarChange();
      gameHandler.TriggerEvents("DamageDone");
	}

   public void GetDamageShield(float damagesValue) {

		if(shieldHealth >= damagesValue) shieldHealth -= damagesValue;
      
      else {
         float unAbsorbedDamages = damagesValue -shieldHealth;
         GetDamageHealth(unAbsorbedDamages);
         shieldHealth = 0;

         gameHandler.TriggerEvents("StopProtect");
      }

      ShieldBarChange();
	}


   // Enemy Anim / Movements / Attack
   public void SetEnemyAnim(int[] statesIndexArray) {

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

   public void CheckEnemyAttack(PlayerHandler localPH, float newDamages, int[] statesIndexArray) {

      // ** StateArray **        ** StatesIndexArray **        ** BodyPartArray ** 
      // 3 => "Estoc",           0 => 0 to 6,                  0 => "Sword",
      // 4 => "Strike",

      if(currentDamages != newDamages) {
         currentDamages = newDamages;

         if(statesIndexArray[0] == 3) StartCoroutine(
            DamageEnemy(estocDelay, localPH, newDamages)
         );

         if(statesIndexArray[0] == 4) StartCoroutine(
            DamageEnemy(strikeDelay, localPH, newDamages)
         );      
      }
   }


   // ===========================================================================================================
	// Private Methods
	// ===========================================================================================================
   private void SetMovementsAnim(string side, float speed, string animName) {
		if(characterSide == side) {

			isWalking = true;
			walkDirection = animName;
			playerSpeed = speed;
			playerMovePosition = new Vector3(movePosX, 0);
			
			BodyPartAnim(animName);
		}
	}

	private void BodyPartAnim(string animName) {
		
		SetPlayerAnim("Body", animName);
		if(!isAttacking) SetPlayerAnim("Sword", animName);
		if(!isProtecting) SetPlayerAnim("Shield", animName);
	}

   private float healthPercent() {
		return Mathf.Floor(playerHealth / playerMaxHealth *coeff) /coeff;
	}

	private void HealthBarChange() {
		healthSpritesArray[sideIndex].fillAmount = healthPercent();
		if(healthPercent() <= 0 ) healthSpritesArray[sideIndex].fillAmount = healthPercent();
	}


   // ***************************
   private float shieldPercent() {
		return Mathf.Floor(shieldHealth / shieldMaxHealth *coeff) /coeff;
	}

   private void ShieldBarChange() {
		shieldSpritesArray[sideIndex].fillAmount = shieldPercent();
		if(shieldPercent() <= 0 ) shieldSpritesArray[sideIndex].fillAmount = shieldPercent();
	}
   // ***************************


   // ====================================================================================
   // Coroutines
   // ====================================================================================
   IEnumerator AttackTimeOut(float delay) {

      attackDelay = delay;
      swordCol.SetActive(true);

      yield return new WaitForSeconds(delay);

      isAttacking = true;

      yield return new WaitForSeconds(attackAnimDelay -delay);

      damagesValue = 0f;
      isAttacking = false;
      isEnemyDamaged = false;
      swordCol.SetActive(false);
      
      if(isWalking) SetPlayerAnim("Sword", walkDirection);
      else SetPlayerAnim("Sword", "Idle");
   }

   IEnumerator ProtectTimeOut() {
      yield return new WaitForSeconds(protectAnimDelay);
      
      if(isProtecting) SetPlayerAnim("Shield", "Protected");
      else yield break;
   }

   public IEnumerator DamageEnemy(float attackDelay, PlayerHandler localPH, float newDamages) {
      yield return new WaitForSeconds(attackDelay);

      if(!localPH.isProtecting) localPH.GetDamageHealth(newDamages);
      else localPH.GetDamageShield(newDamages);

      // Damage Taken
      gameHandler.ToggleDmgText(gameHandler.damageTakenTMP, -newDamages);
   }

}
