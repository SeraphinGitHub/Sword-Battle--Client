using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;
using UnityEngine.UI;

public class PlayerHandler : MonoBehaviour {

   // Public Variables
   [Header("**Swords Colliders**")]
   public GameObject[] swordColliders  = new GameObject[2];
   
   [Header("**Shields Colliders**")]
   public GameObject[] shieldColliders = new GameObject[2];

   [Header("**Player Animators**")]
   public Animator[] swordAnimators  = new Animator[2];
   public Animator[] armAnimators    = new Animator[2];
   public Animator[] shieldAnimators = new Animator[2];
   public Animator[] bodyAnimators   = new Animator[2];
 

   // Public Hidden Variables
   [HideInInspector] public int damagesValue;
   [HideInInspector] public int sidePos = 0;
   [HideInInspector] public int playerHealth;
   [HideInInspector] public int shieldHealth;

   [HideInInspector] public float spawnX = 6.5f;
   [HideInInspector] public float spawnY = 0.2f;
   [HideInInspector] public float walkDirX;
   [HideInInspector] public float playerSpeed;

   [HideInInspector] public bool isDead;
   [HideInInspector] public bool isFighting;
   [HideInInspector] public bool isLocalPlayer;
   [HideInInspector] public bool isWalking;
   [HideInInspector] public bool isAttacking;
   [HideInInspector] public bool isProtecting;
   [HideInInspector] public bool isDamaging;
   [HideInInspector] public bool isEnemyDamaged;

   [HideInInspector] public string winnerName;
   [HideInInspector] public string walkDirection;
   [HideInInspector] public string characterSide;
   [HideInInspector] public int sideIndex;

   [HideInInspector] public Vector3 playerMovePosition;
   [HideInInspector] public GameHandler gameHandler;


   // Private Variables
   private Animator[][] animatorsArray;
	private Animator[][] shieldSegmentsArray;
	private Image[][]    healthSpritesArray;
	private Image[][]    shieldLifeSpritesArray;
	private Image[][]    shieldFluidSpritesArray;

   private string[] sidesArray      = new string[] {
      "Left",
      "Right",
   };
   private string[] bodyPartArray   = new string[] {
      "Sword",
      "Shield",
      "Body",
   };
   private string[] statesArray     = new string[] {
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

   // Health & Shield
   private int    playerMaxHealth  = 2500;
   private int    shieldMaxHealth;
   private int    shieldSegHealth  = 250;
   private int    initSegmentCount;
   private float  segPopUpTimeOut  = 0.12f; // Lower ==> Faster
   private bool   isPlayerReseted;
   private bool[] isSegLostArray = new bool[] {
      false,
      false,
      false,
      false,
      false,
      false,
   };

   // Health fluidity
   private Image healthIMG;
   private Image fluidHealthIMG;
   private bool  isHealthRefuelable;
   private bool  isHealthDrainable;
   private bool  isHealthUpdatable;
   private float red_HealthFluidSpeed       = 0.23f; // Higher ==> Faster
   private float red_InitHealthFluidSpeed   = 1.2f;  // Higher ==> Faster
   private float green_InitHealthFluidSpeed = 0.9f;  // Higher ==> Faster

   // Shield fluidity
   private Image fluidSegIMG;
   private bool  isShieldDrainable;   
   private float shieldPercent;
   private float segOldFillAmount;
   private float red_ShieldFluidSpeed = 1.8f; // Higher ==> Faster

   // Movements
   private bool  isMergePosX;
   private float enemySpeed;
   private float forwardSpeed  = 11f;
	private float backwardSpeed = 7.5f;

   // Attack
   private int   strikeDamages    = 220;
   private int   estocDamages     = 150;
	private float randDmgCoeff     = 0.25f; // 25% ==> +/- 12.5%
   private float strikeDelay      = 0.15f;
   private float estocDelay       = 0.05f;
   private float attackAnimDelay  = 0.4f;
   private float protectAnimDelay = 0.125f;
   
	private Animator   playerAnim;
   private Vector3    enemyMovePosition;
   private Vector3    enemyPosX;
   private GameObject gameHandlerObj;
   private GameObject optionsMenu;
   private GameObject swordCol;
   private GameObject shieldCol;


   // ===========================================================================================================
   // Start() / Update()
   // ===========================================================================================================
   private void Start() {

      // Get gamHandler script
      gameHandlerObj = GameObject.Find("_GameHandler");
      gameHandler    = gameHandlerObj.GetComponent<GameHandler>();
		playerAnim     = GetComponent<Animator>();

      AnchorGameObject anchor = GetComponent<AnchorGameObject>();
      anchor.enabled = false;

      // Set sword & shield colliders
      swordCol  = swordColliders[sideIndex];
      shieldCol = shieldColliders[sideIndex];
      swordCol.SetActive(false);
      shieldCol.SetActive(false);

      isDead     = false;
      isFighting = true;
      enemyMovePosition = Vector3.zero;

      // Set health & shield bars images arrays
		healthSpritesArray      = gameHandler.healthSpritesArray;
		shieldSegmentsArray     = gameHandler.shieldSegmentsArray;
		shieldLifeSpritesArray  = gameHandler.shieldLifeSpritesArray;
		shieldFluidSpritesArray = gameHandler.shieldFluidSpritesArray;

      // Display health & shield bars
      healthIMG      = healthSpritesArray[sideIndex][0];
		fluidHealthIMG = healthSpritesArray[sideIndex][1];

      // Set health & shield values
      InitPlayerStats();

      // Set player start animation
      SetPlayerAnim("Body", "Idle");
      SetPlayerAnim("Sword", "Idle");
      SetPlayerAnim("Shield", "Idle");
   }

   // Player & Enemy movements sync
   private void Update() {

		if(isLocalPlayer)  transform.Translate(playerMovePosition *playerSpeed *Time.deltaTime);
      if(!isLocalPlayer) transform.Translate(enemyMovePosition  *enemySpeed  *Time.deltaTime);

      if(isMergePosX)    transform.position = Vector3.MoveTowards(transform.position, enemyPosX, enemySpeed *Time.deltaTime);
      
      UpdateBarFluidity();
	}



   // **************************************************************************
   // **********************                              **********************
	//                             CHARACTER METHODS
   // **********************                              **********************
	// **************************************************************************

   // === Public ===
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

   public void ResetPosition() {
      
      transform.position = new Vector3(spawnX *sidePos, spawnY, 0);
   }

   public void ResetPlayer() {

      isPlayerReseted = true;
      ClearBars();
      ResetPosition();

      if(shieldHealth != shieldMaxHealth) LooseAllSegments();

      for(int i = 0; i < isSegLostArray.Length; i++) {
         isSegLostArray[i] = false;
      };

      InitPlayerStats();
   }

   public void ClearBars() {

      // Clear health bars
      playerHealth = 0;
      HealthBarChange();
      
      // Clear shield segments
      shieldHealth = 0;
      ShieldBarChange(); // Loose all segments
   }

   public void PlayerRespawn() {
      
      ResetPlayer();
   
      // ****** Tempory ******
      transform.localScale = new Vector3(10f, 10f, 1f);
      // ****** Tempory ******
      
      isDead     = false;
      isFighting = true;
   }


   // === Private ===
   private void  InitPlayerStats() {

      HealthBarChange(); // Refuel health bar

      playerHealth    = playerMaxHealth;
      shieldMaxHealth = isSegLostArray.Length *shieldSegHealth;      
      shieldHealth    = shieldMaxHealth;

      initSegmentCount = 0;
      ShieldBarChange(); // Refuel all segments
   }

   private int   damageRnG(int baseDamage) {

      int halfDmgRange = (int)Mathf.Round(baseDamage *randDmgCoeff /2);
      int damageMin = baseDamage -halfDmgRange;
      int damageMax = baseDamage +halfDmgRange;

      return Random.Range(damageMin, damageMax);
   }

   private float healthPercent() {
		return Mathf.Floor( (float)playerHealth / (float)playerMaxHealth *100 ) /100;
	}

	private void  HealthBarChange() {

      // Empty healthBar
      if(playerHealth == 0) {
         healthIMG.fillAmount      = 0f;
         fluidHealthIMG.fillAmount = 0f;
         isHealthRefuelable        = true;
      }

      // Update healthBar
      if(isHealthUpdatable) {
		   healthIMG.fillAmount = healthPercent();
         isHealthDrainable    = true;
      }
	}

   private void  ShieldBarChange() {

      for(int i = 0; i < isSegLostArray.Length; i++) {

         Image lifeSegIMG = shieldLifeSpritesArray[sideIndex][i];
         Animator segAnim = shieldSegmentsArray[sideIndex][i];

         int shieldSeg_MinValue = shieldSegHealth *i;
         int shieldSeg_MaxValue = shieldSegHealth *(i + 1);
         
         // Empty segment
         if(shieldHealth <= shieldSeg_MinValue
         && !isPlayerReseted
         && !isSegLostArray[i]) {
            
            isSegLostArray[i] = true;
            lifeSegIMG.fillAmount = 0f;
            segAnim.SetTrigger("SegmentLoose");
            StartCoroutine( DisabledShieldSegment(segAnim) );
         }

         // Full segment
         else if(shieldHealth >= shieldSeg_MaxValue
         && initSegmentCount < isSegLostArray.Length) {

            fluidSegIMG = shieldFluidSpritesArray[sideIndex][i];
            StartCoroutine( SetShieldSegments(segAnim, lifeSegIMG, fluidSegIMG) );
         }

         // Segment between 0% & 100%
         else if(shieldHealth > shieldSeg_MinValue
         && shieldHealth < shieldSeg_MaxValue) {
            
            shieldPercent = (float)(shieldHealth -shieldSeg_MinValue) /shieldSegHealth;
            fluidSegIMG   = shieldFluidSpritesArray[sideIndex][i];
            
            lifeSegIMG.fillAmount = shieldPercent;
            segOldFillAmount      = fluidSegIMG.fillAmount;

            segAnim.SetTrigger("SegmentBlock");
            isShieldDrainable = true;

            if(shieldHealth <= shieldSegHealth) segAnim.SetTrigger("SegmentLast");
         }
      }
	}

   private void  LooseAllSegments() {

      for(int i = 0; i < isSegLostArray.Length; i++) {
         isSegLostArray[i] = true;
         shieldLifeSpritesArray[sideIndex][i].fillAmount = 0f;
         shieldSegmentsArray   [sideIndex][i].SetTrigger("SegmentLoose");
      }
   }

   private void  UpdateBarFluidity() {

      // Reduce shield segment red bar
      if(isShieldDrainable) {
         fluidSegIMG.fillAmount       -= red_ShieldFluidSpeed *Time.deltaTime;
         if(fluidSegIMG.fillAmount    <= shieldPercent) isShieldDrainable = false;
      }

      // Reduce health red bar
      if(isHealthDrainable) {
         fluidHealthIMG.fillAmount    -= red_HealthFluidSpeed *Time.deltaTime;
         if(fluidHealthIMG.fillAmount <= healthPercent()) isHealthDrainable = false;
      }

      // Refuel green & red health bars
      if(isHealthRefuelable) {
         fluidHealthIMG.fillAmount    += red_InitHealthFluidSpeed   *Time.deltaTime;
         healthIMG.fillAmount         += green_InitHealthFluidSpeed *Time.deltaTime;

         if(healthIMG.fillAmount      >= healthPercent()
         && fluidHealthIMG.fillAmount >= healthPercent()) {

            isHealthRefuelable = false;
            isHealthUpdatable  = true;
         }
      }
   }

   private void  PlayerDeath() {

      playerHealth = 0;
      shieldHealth = 0;
      ShieldBarChange(); // Loose all segments

      isDead = true;
      transform.localScale = Vector3.zero;

      gameHandler.ToggleWinnerText(winnerName);
      gameHandler.BattleLoose();
   }



   // **************************************************************************
   // **********************                              **********************
	//                               PLAYER METHODS
   // **********************                              **********************
	// **************************************************************************
   
   // === Events ===
   public void Ev_MoveLeft    (object sender, EventArgs e) {

      if(isFighting) {
         walkDirX = -1f;
         SetMovementsAnim("Left", backwardSpeed, "Backward");
         SetMovementsAnim("Right", forwardSpeed, "Forward");
      }
	}

	public void Ev_MoveRight   (object sender, EventArgs e) {

      if(isFighting) {
         walkDirX = 1f;
         SetMovementsAnim("Left", forwardSpeed, "Forward");
         SetMovementsAnim("Right", backwardSpeed, "Backward");
      }
	}

	public void Ev_StopMove    (object sender, EventArgs e) {

      if(isFighting) {
         isWalking = false;
         walkDirX = 0;
         playerMovePosition = new Vector3(walkDirX, 0);
         BodyPartAnim("Idle");
      }
	}

   public void Ev_StrikeAttack(object sender, EventArgs e) {
      if(isFighting && !isProtecting && !isAttacking) {
         
         SetPlayerAnim("Sword", "Strike");
         StartCoroutine( AttackTimeOut(strikeDelay, strikeDamages) );
      }
	}

   public void Ev_EstocAttack (object sender, EventArgs e) {
      if(isFighting && !isProtecting && !isAttacking) {
         
         SetPlayerAnim("Sword", "Estoc");
         StartCoroutine( AttackTimeOut(estocDelay, estocDamages) );
      }
	}
   
   public void Ev_Protect     (object sender, EventArgs e) {
      if(isFighting && !isProtecting && shieldHealth != 0) {

         isProtecting = true;
         shieldCol.SetActive(true);
         SetPlayerAnim("Shield", "Defend");
         StartCoroutine(ProtectTimeOut());
      }
	}

   public void Ev_StopProtect (object sender, EventArgs e) {
      if(isProtecting) {

         isProtecting = false;
         shieldCol.SetActive(false);
         if(!isWalking) SetPlayerAnim("Shield", "Idle");
         else SetPlayerAnim("Shield", walkDirection);
      }
	}

  
   // === Private ===
   private void SetPlayerAnim(string bodyPart, string animName) {

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

   private void SetMovementsAnim(string side, float speed, string animName) {
		if(characterSide == side) {

			isWalking = true;
			walkDirection = animName;
			playerSpeed = speed;
			playerMovePosition = new Vector3(walkDirX, 0);
			
			BodyPartAnim(animName);
		}
	}

	private void BodyPartAnim(string animName) {
		
		SetPlayerAnim("Body", animName);
		if(!isAttacking) SetPlayerAnim("Sword", animName);
		if(!isProtecting) SetPlayerAnim("Shield", animName);
	}


   // === Coroutines ===
   IEnumerator AttackTimeOut(float damagingDelay, int baseDamage) {

      swordCol.SetActive(true);
      isAttacking  = true;

      yield return new WaitForSeconds(damagingDelay);

      isDamaging   = true;
      damagesValue = damageRnG(baseDamage);

      yield return new WaitForSeconds(attackAnimDelay -damagingDelay);

      damagesValue   = 0;
      isAttacking    = false;
      isDamaging     = false;
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

   IEnumerator DisabledShieldSegment(Animator segAnim){

      yield return new WaitForSeconds(1f);
      segAnim.SetTrigger("SegmentReset");
      
      yield return new WaitForSeconds(0.1f);
      segAnim.gameObject.SetActive(false);
   }

   IEnumerator SetShieldSegments(Animator segAnim, Image lifeSegIMG, Image fluidSegIMG) {

      initSegmentCount++;

      yield return new WaitForSeconds(segPopUpTimeOut *initSegmentCount);

      lifeSegIMG.fillAmount  = 1f;
      fluidSegIMG.fillAmount = 1f;
      segAnim.SetTrigger("SegmentGet");

      if(isPlayerReseted && initSegmentCount == isSegLostArray.Length) isPlayerReseted = false;
   }

   

   // **************************************************************************
   // **********************                              **********************
	//                                ENEMY METHODS
   // **********************                              **********************
	// **************************************************************************

   // === Public ===
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

   public void EnemyMovements(float walkDirX, float newPosX) {
      if(isFighting) {

         // Move Left
         if(walkDirX == -1) {
            isMergePosX = false;

            if(sidePos == -1) enemySpeed = backwardSpeed; // Left  side
            if(sidePos ==  1) enemySpeed = forwardSpeed;  // Right side
         }

         // Move Right
         if(walkDirX ==  1) {
            isMergePosX = false;

            if(sidePos == -1) enemySpeed = forwardSpeed;  // Left  side
            if(sidePos ==  1) enemySpeed = backwardSpeed; // Right side
         }

         if(walkDirX == 0) {
            isMergePosX = true;
            enemyPosX = new Vector3(newPosX, transform.position.y);
         }

         enemyMovePosition = new Vector3(walkDirX, 0);
      }
   }
   
   public void GetDamageHealth(int damagesValue) {
		
      if(playerHealth > damagesValue) playerHealth -= damagesValue;
      else PlayerDeath();
		
      gameHandler.SetDamageDoneTMP(damagesValue);
      HealthBarChange();
	}

   public void GetDamageShield(int damagesValue) {

		if(shieldHealth > damagesValue) shieldHealth -= damagesValue;  
      else {
         int unAbsorbedDamages = damagesValue -shieldHealth;
         GetDamageHealth(unAbsorbedDamages);
         shieldHealth = 0;
      }

      gameHandler.SetDamageBlockedTMP(damagesValue);
      ShieldBarChange(); // Change Enemy shield bar value
	}


   // === Coroutines ===
   public void UpdatePlayerHealth(int newPlayerHealth, int newShieldHealth) {

      // Player Health
      if(playerHealth != newPlayerHealth) {
         int healthDamages = playerHealth -newPlayerHealth;

         gameHandler.SetDamageTakenTMP(healthDamages);
         playerHealth = newPlayerHealth;

         if(playerHealth <= 0) PlayerDeath();

         HealthBarChange();
         Vibration.Vibrate(150);
      }

      // Player Shield
      if(shieldHealth != newShieldHealth) {
         int shieldDamages = shieldHealth -newShieldHealth;

         gameHandler.SetDamageBlockedTMP(shieldDamages);
         shieldHealth = newShieldHealth;

         if(shieldHealth <= 0) gameHandler.TriggerEvents("StopProtect");

         ShieldBarChange(); // Change Player shield bar value
         Vibration.Vibrate(250);
      }
   }

}
