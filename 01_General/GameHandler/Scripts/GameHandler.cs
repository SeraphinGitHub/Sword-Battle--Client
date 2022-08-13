using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;
using System;

public class GameHandler : MonoBehaviour {
   
   [Header("**Server Options**")]
   // ********** Dev **********
      private int FPS = 10;
      private string serverURL = "http://localhost:3000/";
      private int endBattleCD = 3; // seconds
      private int randomizeDelay = 1; // seconds
   // ********** Dev **********


   // ********** Build **********
      // private int FPS = 40;
      // private string serverURL = "http://sword-battle.herokuapp.com/";
      // private int endBattleCD = 3; // seconds
      // private int randomizeDelay = 1; // seconds
   // ********** Build **********


   [Header("**Attached Components**")]
   public Animator loadingGearsAnimator;
   public GameObject joinBtnPrefab;
   public GameObject connectingText;
   public GameObject connectedText;
   public RectTransform scrollContent;
	public Transform[] healthSpritesArray = new Transform[2];

   [Header("**BattleList Options**")]
   public int firstBattleOffsetY = 20;
   public int battleOffsetY = 155;
   public int battleCountBeforeScroll = 5;

   [Header("**Attached Canvas**")]
   public GameObject loadingScreen;
   public GameObject mainMenu;
   public GameObject findBattleMenu;
   public GameObject optionsMenu;
   public GameObject optionsClickable;
   public GameObject optionsClicked;
   public GameObject battleUI;
   public GameObject messageUI;

   [Header("**Attached InputFields**")]
   public InputField playerNameField;
   public InputField battleNameField;

   [Header("**Attatched TexMeshPro**")]
   public TextMeshProUGUI serverMessageTMP;
   public TextMeshProUGUI countDownTMP;
   public TextMeshProUGUI battleNameTMP;
   public TextMeshProUGUI leftPlayerNameTMP;
   public TextMeshProUGUI rightPlayerNameTMP;


   // Hidden Public Variables
   [HideInInspector] public SocketIOUnity socket;
   [HideInInspector] public List<string> playerProps;
   [HideInInspector] public List<string> enemyProps;

   [HideInInspector] public int[] statesIndexArray = new int[3];
   [HideInInspector] public float frameRate;
   [HideInInspector] public float showPayerUIDelay;
   [HideInInspector] public string battleID;
   [HideInInspector] public string swordColor;


   // Private Variables
   private int baseEndBattleCD;
   private bool isConnected;
   private bool isBattleOnGoing;
   private bool hasNewBattle;
   private float toggleGearDelay = 0.75f;

   private string createdBattleName;
   private string joinedBattleName;
   private string playerSide;
   private string playerName;
   private string enemySide;
   private string enemyName;

   private List<string> existBattle_IDList = new List<string>();
   private List<string> newBattles_IDList = new List<string>();

   private CreateBattleClass newBattle;
   private GameRandomize gameRandomize;
   private PlayerHandler enemyHandler;

   private PlayerHandler localHandler;
   private PlayerMovements localMovements;
   private PlayerAttack localAttack;
   private PlayerProtect localProtect;  


   // ====================================================================================
   // Transfert Data Classes
   // ====================================================================================
   [System.Serializable]
   class PlayerClass {

      public string name;
      public string side;
      public string hairStyle;
      public string hairColor;
      public string tabardColor;
      public string swordColor;

      public PlayerClass(
      string name,
      string side,
      string hairStyle,
      string hairColor,
      string tabardColor,
      string swordColor) {

         this.name = name;
         this.side = side;
         this.hairStyle = hairStyle;
         this.hairColor = hairColor;
         this.tabardColor = tabardColor;
         this.swordColor = swordColor;
      }
   }

   [System.Serializable]
   class CreateBattleClass {

      public string battleName;
      public PlayerClass player;

      public CreateBattleClass(string battleName, PlayerClass player) {
         this.battleName = battleName;
         this.player = player;
      }
   }

   [System.Serializable]
   class FoundBattleClass {

      public string id;
      public string name;

      public FoundBattleClass(string id, string name) {
         this.id = id;
         this.name = name;
      }
   } 
   
   [System.Serializable]
   class LeavingPlayerClass {
      
      public bool isHostPlayer;
      public bool isJoinPlayer;

      public LeavingPlayerClass(bool isHostPlayer, bool isJoinPlayer) {
         this.isHostPlayer = isHostPlayer;
         this.isJoinPlayer = isJoinPlayer;
      }
   }

   // Server SyncPack
   [System.Serializable]
   class SyncPackClass {
      
      public int[] statesIndexArray;
      public float movePosX;
      public float playerSpeed;
      public bool isAttacking;
      public bool isProtecting;

      public SyncPackClass(
      int[] statesIndexArray,
      float movePosX,
      float playerSpeed,
      bool isAttacking,
      bool isProtecting) {

         this.statesIndexArray = statesIndexArray;
         this.movePosX = movePosX;
         this.playerSpeed = playerSpeed;
         this.isAttacking = isAttacking;
         this.isProtecting = isProtecting;
      }
   }


   // ====================================================================================
   // Awake() / Start()
   // ====================================================================================
   private void Awake() {

      // Init Socket IO
      socket = new SocketIOUnity(serverURL, new SocketIOOptions {
         Query = new Dictionary<string, string> {
            {"token", "UNITY" }
         },
         EIO = 4,
         Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
      });
      socket.JsonSerializer = new NewtonsoftJsonSerializer();
      SocketIO_Connect();

      // Show UI components
      loadingScreen.SetActive(true);
      connectingText.SetActive(true);
      optionsClickable.SetActive(true);
      
      // Hide UI components
      connectedText.SetActive(false);
      findBattleMenu.SetActive(false);
      optionsClicked.SetActive(false);
      messageUI.SetActive(false);

      SwitchToMainMenu();

      gameRandomize = GetComponent<GameRandomize>();
   }

   private void Start() {

      StartCoroutine(ShowLoadingGear());

      // Init Variables
      playerName = playerNameField.text;
      createdBattleName = battleNameField.text;
      baseEndBattleCD = endBattleCD;
      frameRate = Mathf.Floor(1f/FPS *1000)/1000;
      showPayerUIDelay = randomizeDelay +0.5f;


      // ===================================
      // Socket Listening Events
      // ===================================
      socket.OnAnyInUnityThread((channel, response) => {
         
         // Connection
         if(channel == "connected") {
            ConnectedToSocketIO();
         }

         // Create Battle
         if(channel == "battleCreated") {
            battleID = response.GetValue().GetRawText();
         }
         
         // Find Battle
         if(channel == "battleFound") {
            var battlesArray = response.GetValue<FoundBattleClass[]>();
            SetBattleList(battlesArray);
         }
         
         // Join Battle
         if(channel == "joinBattleAccepted") {
            var enemyPlayer = response.GetValue<PlayerClass>();
            JoinBattleAccepted(enemyPlayer);
         }

         if(channel == "battleJoined") {
            InitBattle();
            StartCoroutine(BattleCreated(joinedBattleName));
         }

         if(channel == "enemyJoined") {
            var enemyPlayer = response.GetValue<PlayerClass>();
            InitEnemyPlayer(enemyPlayer);
         }

         // Leave Battle
         if(channel == "battleEnded") {
            var leavingPlayer = response.GetValue<LeavingPlayerClass>();
            BattleEnded(leavingPlayer);
         }

         // Receive Sync
         if(channel == "ReceiveServerSync") {
            var syncPack = response.GetValue<SyncPackClass>();
            ReceiveServerSync(syncPack);
         }
      });
   }
   

   // ====================================================================================
   // Public Methods
   // ====================================================================================
   public void SocketIO_Connect() {
      socket.Connect();
   }

   public void SocketIO_Disconnect() {
      socket.Disconnect();
   }

   public void StopSync() {
      CancelInvoke();
   }

   public void UpdatePlayerName() {
      playerName = playerNameField.text;
   }

   public void UpdateBattleName() {
      createdBattleName = battleNameField.text;
   }

   public void CreateBattle() {

      if(!isBattleOnGoing
      && playerName != ""
      && createdBattleName != "") {

         InitBattle();

         playerProps = gameRandomize.RunRandomize(new List<string>());
         playerSide = playerProps[0];
         swordColor = playerProps[4];

         // Set hostPlayer
         PlayerClass hostPlayer = new PlayerClass(
            playerName,
            playerProps[0],
            playerProps[1],
            playerProps[2],
            playerProps[3],
            playerProps[4]
         );

         // Set battle
         newBattle = new CreateBattleClass(
            createdBattleName,
            hostPlayer
         );

         StartCoroutine(BattleCreated(createdBattleName));

         if(!isConnected && !hasNewBattle) hasNewBattle = true;
         if(isConnected) socket.Emit("createBattle", newBattle);
      }

      // ************************
      else if(playerName == "") Debug.Log("Need to have playerName");
      else if(createdBattleName == "") Debug.Log("Need to have battleName");
      // ************************
   }

   public void FindBattle() {
      SetInterval("SearchBattleList", 0.7f); // seconds
   }

   public void JoinBattleRequest(string battleID, string battleName) {
      // Used in > JoinBattleHandler.cs

      joinedBattleName = battleName;
      socket.Emit("joinBattleRequest", battleID);
   }

   public void LeaveBattle() {

      isBattleOnGoing = false;
      gameRandomize.DestroyAllPlayers();
      enemyProps.Clear();
      ResetCountDown();

      socket.Emit("EndBattle", battleID);
      battleID = "";

      SwitchToMainMenu();
   }

   public void QuitApplication() {

      SocketIO_Disconnect();
      isConnected = false;
      Application.Quit();
   }


   // ====================================================================================
   // Private Methods
   // ====================================================================================
   private void ConnectedToSocketIO() {
      
      isConnected = true;
      if(hasNewBattle) socket.Emit("createBattle", newBattle);

      connectingText.SetActive(false);
      connectedText.SetActive(true);

      StartCoroutine(HideLoadingGear());
   }  

   private void SetBattleText(string name) {
      battleNameTMP.text = name;
   }

   private void SetNameText(string side, string name) {
      if(side == "Left") leftPlayerNameTMP.text = name;
      if(side == "Right") rightPlayerNameTMP.text = name;
   }

   private void InitBattle() {

      SetBattleText("");
      SetNameText("Left", "");
      SetNameText("Right", "");

      isBattleOnGoing = true;
      SwitchToBattle();
   }

   private void SetInterval(string methodName, float refreshRate) {
      InvokeRepeating(methodName, 0, refreshRate);
   }

   private void CountDown() {
      countDownTMP.text = endBattleCD.ToString();
      endBattleCD--;

      if(endBattleCD  < 0) {
         if(gameRandomize.localPlayer) Destroy(gameRandomize.localPlayer);
         if(!mainMenu.activeSelf) SwitchToMainMenu();

         ResetCountDown();
      }
   }

   private void ResetCountDown() {
      endBattleCD = baseEndBattleCD;
      CancelInvoke();
   }

   private void SwitchToBattle() {

      mainMenu.SetActive(false);
      findBattleMenu.SetActive(false);
      optionsMenu.SetActive(false);
      optionsClickable.SetActive(true);
      optionsClicked.SetActive(false);
      battleUI.SetActive(true);
      messageUI.SetActive(true);
   }

   private void SwitchToMainMenu() {

      mainMenu.SetActive(true);
      optionsMenu.SetActive(false);
      battleUI.SetActive(false);
      serverMessageTMP.gameObject.SetActive(false);
      countDownTMP.gameObject.SetActive(false);
   }

   private void SearchBattleList() {
      if(isConnected) socket.Emit("findBattle");
   }

   private void SetBattleList(FoundBattleClass[] battlesArray) {

      newBattles_IDList.Clear();

      // ========================
      // Add new Battles
      // ========================
      foreach (var battle in battlesArray) {
         newBattles_IDList.Add(battle.id);

         // If new battle not rendered already
         if(!existBattle_IDList.Contains(battle.id)) {
            int existBattleCount = existBattle_IDList.Count;
            float joinBtn_PosY = -firstBattleOffsetY -battleOffsetY *existBattleCount;

            GameObject joinBtn = Instantiate(joinBtnPrefab, new Vector3(0, joinBtn_PosY), Quaternion.identity);
            joinBtn.transform.SetParent(scrollContent, false);

            joinBtn.transform.Find("BattleID").gameObject.GetComponent<TMP_Text>().text = battle.id;
            joinBtn.transform.Find("BattleName").gameObject.GetComponent<TMP_Text>().text = battle.name;

            // Extend ScorllContent height
            if(existBattleCount >= battleCountBeforeScroll) {
               scrollContent.sizeDelta = new Vector2(0, scrollContent.sizeDelta.y + battleOffsetY);
            }
            existBattle_IDList.Add(battle.id);
         }
      }
      

      // ========================
      // Remove old Battles
      // ========================
      int renderedBattleCount = scrollContent.childCount;

      // For all rendered battle
      for(int i = 0; i < renderedBattleCount; i++) {
         Transform joinBtn = scrollContent.GetChild(i);
         string BattleID = joinBtn.Find("BattleID").GetComponent<TMP_Text>().text;            
         
         // If rendered battle doesn't exist anymore 
         if(!newBattles_IDList.Contains(BattleID)) {
            Destroy(joinBtn.gameObject);
            existBattle_IDList.Remove(BattleID);

            // Move up other rendered battle after the destroyed one
            for(int j = 0; j < renderedBattleCount -i; j++) {
               RectTransform OtherjoinBtn = scrollContent.GetChild(j +i).GetComponent<RectTransform>();
               float OtherjoinBtn_PosY = OtherjoinBtn.anchoredPosition.y + battleOffsetY;
               OtherjoinBtn.anchoredPosition = new Vector2(0, OtherjoinBtn_PosY);
            }

            // Shorten ScorllContent height
            int existBattleCount = existBattle_IDList.Count;
            if(existBattleCount >= battleCountBeforeScroll) {
               scrollContent.sizeDelta = new Vector2(0, scrollContent.sizeDelta.y - battleOffsetY);
            }
         }
      }
   }

   private void JoinBattleAccepted(PlayerClass enemyPlayer) {
      
      // Set enemy player (Host player)
      enemyName = enemyPlayer.name;
      enemySide = enemyPlayer.side;

      enemyProps = new List<string>() {
         enemyPlayer.side,
         enemyPlayer.hairStyle,
         enemyPlayer.hairColor,
         enemyPlayer.tabardColor,
         enemyPlayer.swordColor
      };
      
      // Randomize joinPlayer, out of hostPlayer props
      playerProps = gameRandomize.RunRandomize(enemyProps);
      playerSide = playerProps[0];
      swordColor = playerProps[4];

      // Set joinPlayer
      PlayerClass joinPlayerProps = new PlayerClass(
         playerName,
         playerProps[0],
         playerProps[1],
         playerProps[2],
         playerProps[3],
         playerProps[4]
      );

      // On received event ("battleJoined") ==> Coroutine BattleCreated();
      socket.Emit("joinBattle", joinPlayerProps);
   }

   private void InitEnemyPlayer(PlayerClass enemyPlayer) {

      serverMessageTMP.gameObject.SetActive(false);

      List<string> joinPropsList = new List<string>() {
         enemyPlayer.side,
         enemyPlayer.hairStyle,
         enemyPlayer.hairColor,
         enemyPlayer.tabardColor,
         enemyPlayer.swordColor,
      };      
      
      enemySide = enemyPlayer.side;

      // Instantiate enemy player
      gameRandomize.InstantiatePlayer(joinPropsList, false);
      enemyHandler = gameRandomize.enemyPlayer.GetComponent<PlayerHandler>();
      SetNameText(enemyPlayer.side, enemyPlayer.name);

      // Start server sync
      StartSync("ServerSync");
   }
   
   private void BattleEnded(LeavingPlayerClass leavingPlayer) {

      if(leavingPlayer.isHostPlayer) {
         Destroy(gameRandomize.enemyPlayer);

         isBattleOnGoing = false;         
         enemyProps.Clear();

         serverMessageTMP.text = "Host player left battle !";     
         serverMessageTMP.gameObject.SetActive(true);
         countDownTMP.gameObject.SetActive(true);
         SetNameText(enemySide, "");

         SetInterval("CountDown", 1f);
      }

      if(leavingPlayer.isJoinPlayer) {
         Destroy(gameRandomize.enemyPlayer);

         serverMessageTMP.text = "Join player left battle !";
         serverMessageTMP.gameObject.SetActive(true);
         SetNameText(enemySide, "");
      }
   }


   // ====================================================================================
   // Coroutines
   // ====================================================================================
   IEnumerator BattleCreated(string battleName) {
      yield return new WaitForSeconds(randomizeDelay);

      if(isBattleOnGoing) {

         SetBattleText(battleName);
         SetNameText(playerSide, playerName);

         // Instantiate LocalPlayer
         gameRandomize.InstantiatePlayer(playerProps, true);
         InitEvents(gameRandomize.localPlayer);

         // Instantiate EnemyPlayer if exists
         if(enemyProps.Count != 0) {

            gameRandomize.InstantiatePlayer(enemyProps, false);
            enemyHandler = gameRandomize.enemyPlayer.GetComponent<PlayerHandler>();
            SetNameText(enemySide, enemyName);
         }

         // Start server sync
         if(gameRandomize.enemyPlayer) StartSync("ServerSync");
      }
      else yield break;
   }

   IEnumerator ShowLoadingGear() {

      yield return new WaitForSeconds(toggleGearDelay);
      loadingGearsAnimator.SetTrigger("connect");
   }

   IEnumerator HideLoadingGear() {

      loadingGearsAnimator.SetTrigger("hide");
      yield return new WaitForSeconds(toggleGearDelay);
      loadingScreen.SetActive(false);
   }


   // ====================================================================================
   // Events
   // ====================================================================================
   private event EventHandler tr_moveLeft;
   private event EventHandler tr_moveRight;
	private event EventHandler tr_stopMove;
	private event EventHandler tr_strikeAttack;
	private event EventHandler tr_estocAttack;
	private event EventHandler tr_protect;
	private event EventHandler tr_stopProtect;
   
   private void InitEvents(GameObject localPlayer) {

      // Init player Scripts
      localHandler = localPlayer.GetComponent<PlayerHandler>();
      localMovements = localPlayer.GetComponent<PlayerMovements>();
      localAttack = localPlayer.GetComponent<PlayerAttack>();
      localProtect = localPlayer.GetComponent<PlayerProtect>();
      
      // Init Triggers
      tr_moveLeft = localMovements.Ev_MoveLeft;
      tr_moveRight = localMovements.Ev_MoveRight;
      tr_stopMove = localMovements.Ev_StopMove;
      tr_strikeAttack = localAttack.Ev_StrikeAttack;
      tr_estocAttack = localAttack.Ev_EstocAttack;
      tr_protect = localProtect.Ev_Protect;
      tr_stopProtect = localProtect.Ev_StopProtect;
   }
   
   public void TriggerEvents(string trigger) {

      if(trigger == "MoveLeft" && tr_moveLeft != null) tr_moveLeft(this, EventArgs.Empty);
      if(trigger == "MoveRight" && tr_moveRight != null) tr_moveRight(this, EventArgs.Empty);
      if(trigger == "StopMove" && tr_stopMove != null) tr_stopMove(this, EventArgs.Empty);
      if(trigger == "Strike" && tr_strikeAttack != null) tr_strikeAttack(this, EventArgs.Empty);
      if(trigger == "Estoc" && tr_estocAttack != null) tr_estocAttack(this, EventArgs.Empty);
      if(trigger == "Protect" && tr_protect != null) tr_protect(this, EventArgs.Empty);
      if(trigger == "StopProtect" && tr_stopProtect != null) tr_stopProtect(this, EventArgs.Empty);
   }


   // ====================================================================================
   // Server Sync Methods
   // ====================================================================================
   public void StartSync(string methodName) {
      InvokeRepeating(methodName, 0, frameRate);
   }

   // Emit Sync
   public void ServerSync() {
      if(gameRandomize.localPlayer && gameRandomize.enemyPlayer) {
         
         SyncPackClass syncPack = new SyncPackClass(
            statesIndexArray,
            localHandler.movePosX,
            localHandler.playerSpeed,
            localHandler.isAttacking,
            localHandler.isProtecting
         );
         
         socket.Emit("ServerSync", syncPack);
      }
   }

   // Receive Sync
   private void ReceiveServerSync(SyncPackClass syncPack) {
      if(gameRandomize.enemyPlayer) {

         // Movements
         enemyHandler.EnemyMovements(syncPack.movePosX, syncPack.playerSpeed);

         // Animations
         enemyHandler.SetEnemyAnim(
            syncPack.statesIndexArray,
            syncPack.isAttacking,
            syncPack.isProtecting
         );
      }
   }
}