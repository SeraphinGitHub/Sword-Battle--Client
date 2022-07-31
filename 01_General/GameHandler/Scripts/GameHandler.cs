using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;

public class GameHandler : MonoBehaviour {
   
   [Header("**Server Options**")]
   public int FPS = 10;
   // public string serverURL = "http://localhost:3000/";
   public string serverURL = "http://sword-battle.herokuapp.com/";
   public int endBattleCD = 3; // seconds
   public int randomizeDelay = 3; // seconds

   [Header("**Attached Prefabs**")]
   public GameObject joinBtnPrefab;

   [Header("**BattleList Options**")]
   public int firstBattleOffsetY = 20;
   public int battleOffsetY = 155;
   public int battleCountBeforeScroll = 5;

   [Header("**Attached Transforms**")]
   public RectTransform scrollContent;

   [Header("**Attached Canvas**")]
   public GameObject mainMenu;
   public GameObject findBattleMenu;
   public GameObject optionsMenu;
   public GameObject battleUI;
   public GameObject popUpMessageUI;
   public GameObject LoadingScreenUI;
   public GameObject LoadingBar;

   [Header("**Attached InputFields**")]
   public InputField playerNameField;
   public InputField battleNameField;
   
   [Header("**Attached TextMeshPro**")]
   public TextMeshProUGUI serverMessageTMP;
   public TextMeshProUGUI countDownTMP;
   public TextMeshProUGUI battleNameTMP;
   public TextMeshProUGUI leftPlayerNameTMP;
   public TextMeshProUGUI rightPlayerNameTMP;
   public TextMeshProUGUI loadingPercentTMP;


   // Hidden Public Variables
   [HideInInspector] public SocketIOUnity socket;
   [HideInInspector] public List<string> playerProps;
   [HideInInspector] public List<string> enemyProps;

   [HideInInspector] private string playerSide;
   [HideInInspector] private string enemySide;
   [HideInInspector] public string swordColor;


   // Private Variables
   private int baseEndBattleCD;
   private float frameRate;
   private float socketConnectDelay = 3f; // seconds
   private bool isBattleOnGoing = false;

   private string createdBattleName;
   private string joinedBattleName;
   private string playerName;
   private string enemyName;
   private string playerState;

   
   private List<string> existBattle_IDList = new List<string>();
   private List<string> newBattles_IDList = new List<string>();

   private GameRandomize gameRandomize;
   private PlayerHandler localPlayerHandler;

   // Enemy Scripts
   private PlayerHandler enemyPlayerHandler;
   private PlayerMovements enemyPlayerMovements;
   private PlayerAttack enemyPlayerAttack;
   private PlayerProtect enemyPlayerProtect;


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
      
      public float position;
      public string state;
      public bool isAttacking;
      public bool isProtecting;

      public SyncPackClass(
      float position,
      string state,
      bool isAttacking,
      bool isProtecting) {

         this.position = position;
         this.state = state;
         this.isAttacking = isAttacking;
         this.isProtecting = isProtecting;
      }
   }


   // ====================================================================================
   // Awake() / Start()
   // ====================================================================================
   private void Awake() {
      mainMenu.SetActive(true);
      findBattleMenu.SetActive(false);
      optionsMenu.SetActive(false);
      battleUI.SetActive(false);
      popUpMessageUI.SetActive(false);

      // ***** Temporary *****
      LoadingScreenUI.SetActive(false);
      loadingPercentTMP.text = "0";
      // ***** Temporary *****

      serverMessageTMP.gameObject.SetActive(false);
      countDownTMP.gameObject.SetActive(false);

      gameRandomize = GetComponent<GameRandomize>();
   }

   private void Start() {

      // Init Socket IO
      socket = new SocketIOUnity(serverURL, new SocketIOOptions {
         Query = new Dictionary<string, string> {
            {"token", "UNITY" }
         },
         EIO = 4,
         Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
      });
      socket.JsonSerializer = new NewtonsoftJsonSerializer();

      // Init Variables
      playerName = playerNameField.text;
      createdBattleName = battleNameField.text;
      baseEndBattleCD = endBattleCD;
      frameRate = Mathf.Floor(1f/FPS *1000)/1000;


      // ===================================
      // Socket Listening Events
      // ===================================
      socket.OnAnyInUnityThread((channel, response) => {
         
         // Create Battle
         if(channel == "battleCreated") {
            StartCoroutine(BattleCreated(createdBattleName));
         }
         
         // Find Battle
         if(channel == "findBattle") {
            var battlesArray = response.GetValue<FoundBattleClass[]>();
            SetBattleList(battlesArray);
         }

         // Join Battle
         if(channel == "joinBattleAccepted") {
            var enemyPlayer = response.GetValue<PlayerClass>();
            JoinBattleAccepted(enemyPlayer);
         }

         if(channel == "battleJoined") {
            isBattleOnGoing = true;
            StartCoroutine(BattleCreated(joinedBattleName));
         }

         if(channel == "enemyJoined") {
            var enemyPlayer = response.GetValue<PlayerClass>();
            StartCoroutine(InitEnemyPlayer(enemyPlayer));
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

   public void UpdatePlayerName() {
      playerName = playerNameField.text;
   }

   public void UpdateBattleName() {
      createdBattleName = battleNameField.text;
   }

   public void CreateBattle() {

      if(!isBattleOnGoing) {
         isBattleOnGoing = true;

         leftPlayerNameTMP.text = "";
         rightPlayerNameTMP.text = "";
         battleNameTMP.text = "";

         SwitchToBattle();
         ToggleLoadingScreen();

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
         CreateBattleClass newBattle = new CreateBattleClass(
            createdBattleName,
            hostPlayer
         );

         // Await for Socket.io to be connected
         StartCoroutine(EmitNewBattle(newBattle));
      }
   }

   public void FindBattle() {
      SocketIO_Connect();
      SetInterval("SetBattleList", 0.7f); // seconds
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
      
      SwitchToMainMenu();
      socket.Disconnect();
   }

   public void QuitApplication() {
      SocketIO_Disconnect();
      Application.Quit();
   }


   // ====================================================================================
   // Private Methods
   // ====================================================================================
   private void SetNameText(string side, string name) {
      if(side == "Left") leftPlayerNameTMP.text = name;
      if(side == "Right") rightPlayerNameTMP.text = name;
   }

   private void SetInterval(string methodName, float refreshRate) {
      InvokeRepeating(methodName, 0, refreshRate);
   }

   private void CountDown() {
      countDownTMP.text = endBattleCD.ToString();
      endBattleCD--;

      if(endBattleCD  < 0) {
         endBattleCD = baseEndBattleCD;
         Destroy(gameRandomize.localPlayer);
         
         if(!mainMenu.activeSelf) SwitchToMainMenu();
         CancelInvoke();
      }
   }

   private void SwitchToBattle() {
      mainMenu.SetActive(false);
      findBattleMenu.SetActive(false);
      battleUI.SetActive(true);
      popUpMessageUI.SetActive(true);
   }

   private void SwitchToMainMenu() {
      mainMenu.SetActive(true);
      battleUI.SetActive(false);
      serverMessageTMP.gameObject.SetActive(false);
      countDownTMP.gameObject.SetActive(false);
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

   private void BattleEnded(LeavingPlayerClass leavingPlayer) {

      if(leavingPlayer.isHostPlayer) {
         Destroy(gameRandomize.enemyPlayer);

         isBattleOnGoing = false;         
         enemyProps.Clear();

         serverMessageTMP.text = "Host player left battle !";              
         serverMessageTMP.gameObject.SetActive(true);
         countDownTMP.gameObject.SetActive(true);
         
         SetInterval("CountDown", 1f);
      }

      if(leavingPlayer.isJoinPlayer) {
         Destroy(gameRandomize.enemyPlayer);

         serverMessageTMP.text = "Join player left battle !";
         SetNameText(enemySide, "");
         serverMessageTMP.gameObject.SetActive(true);
      }
   }


   // ***** Temporary *****
   private void ToggleLoadingScreen() {

      float maxTimer = 100f; // ==> 100%
      float loadTimer = 0f;
      float refreshRate = socketConnectDelay / maxTimer;

      LoadingBar.transform.localScale = new Vector3(0, 1);
      LoadingScreenUI.SetActive(true);
      StartCoroutine(SetLoadPercentage(maxTimer, loadTimer, refreshRate));
   }

   private float loadBarScaleX(float maxTimer, float loadTimer) {
      return loadTimer / maxTimer;
   }


   // ====================================================================================
   // Coroutines
   // ====================================================================================
   IEnumerator EmitNewBattle(CreateBattleClass newBattle) {
      SocketIO_Connect();
      
      yield return new WaitForSeconds(socketConnectDelay);
      socket.Emit("createBattle", newBattle);
   }

   IEnumerator BattleCreated(string battleName) {
      yield return new WaitForSeconds(randomizeDelay);

      if(isBattleOnGoing) {
         battleNameTMP.text = createdBattleName;
         SetNameText(playerSide, playerName);

         // Instantiate EnemyPlayer if exists
         if(enemyProps.Count != 0) {

            SetNameText(enemySide, enemyName);
            gameRandomize.InstantiatePlayer(enemyProps, false);

            // Init enemy Scripts
            enemyPlayerHandler = gameRandomize.enemyPlayer.GetComponent<PlayerHandler>();
            enemyPlayerMovements = gameRandomize.enemyPlayer.GetComponent<PlayerMovements>();
            enemyPlayerAttack = gameRandomize.enemyPlayer.GetComponent<PlayerAttack>();
            enemyPlayerProtect = gameRandomize.enemyPlayer.GetComponent<PlayerProtect>();
         }

         // Instantiate LocalPlayer
         gameRandomize.InstantiatePlayer(playerProps, true);
         localPlayerHandler = gameRandomize.localPlayer.GetComponent<PlayerHandler>();

         // Start server sync
         if(gameRandomize.enemyPlayer) StartSync("ServerSync");
      }
      else yield break;
   }

   IEnumerator InitEnemyPlayer(PlayerClass enemyPlayer) {

      List<string> joinPropsList = new List<string>() {
         enemyPlayer.side,
         enemyPlayer.hairStyle,
         enemyPlayer.hairColor,
         enemyPlayer.tabardColor,
         enemyPlayer.swordColor,
      };
      
      serverMessageTMP.gameObject.SetActive(false);
      yield return new WaitForSeconds(randomizeDelay);
      
      // Instantiate enemy player
      SetNameText(enemySide, enemyPlayer.name);
      gameRandomize.InstantiatePlayer(joinPropsList, false);

      // Init enemy Scripts
      enemyPlayerHandler = gameRandomize.enemyPlayer.GetComponent<PlayerHandler>();
      enemyPlayerMovements = gameRandomize.enemyPlayer.GetComponent<PlayerMovements>();
      enemyPlayerAttack = gameRandomize.enemyPlayer.GetComponent<PlayerAttack>();
      enemyPlayerProtect = gameRandomize.enemyPlayer.GetComponent<PlayerProtect>();

      // Start server sync
      StartSync("ServerSync");
   }

   IEnumerator SetLoadPercentage(float maxTimer, float loadTimer, float refreshRate) {

      loadingPercentTMP.text = loadTimer.ToString();

      yield return new WaitForSeconds(refreshRate);

      if(loadTimer < maxTimer) {
         loadTimer++;
         LoadingBar.transform.localScale = new Vector3(loadBarScaleX(maxTimer, loadTimer), 1);
         StartCoroutine(SetLoadPercentage(maxTimer, loadTimer, refreshRate));
      }

      else {
         yield return new WaitForSeconds(1f);
         LoadingScreenUI.SetActive(false);
      }
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

         float posX = Mathf.Floor(gameRandomize.localPlayer.transform.position.x *10) /10;
         
         if(playerState != "" && playerState != localPlayerHandler.state) playerState = localPlayerHandler.state;
         else playerState = "";

         SyncPackClass syncPack = new SyncPackClass(
            posX,
            playerState,
            localPlayerHandler.isAttacking,
            localPlayerHandler.isProtecting
         );

         socket.Emit("ServerSync", syncPack);
      }
   }

   // Receive Sync
   private void ReceiveServerSync(SyncPackClass syncPack) {
      if(gameRandomize.enemyPlayer) {

         Transform enemyTransform = gameRandomize.enemyPlayer.transform;
         enemyTransform.position = new Vector3(syncPack.position, enemyTransform.position.y, 0);
         
         if(syncPack.state == "Idle") enemyPlayerMovements.StopMove();
         if(syncPack.state == "LeftForward") enemyPlayerMovements.MoveRight();
         if(syncPack.state == "LeftBackward") enemyPlayerMovements.MoveLeft();
         if(syncPack.state == "RightForward") enemyPlayerMovements.MoveLeft();
         if(syncPack.state == "RightBackward") enemyPlayerMovements.MoveRight();
         if(syncPack.state == "Estoc" || syncPack.state == "Strike") enemyPlayerAttack.Attack(syncPack.state);
         if(syncPack.state == "Protect") enemyPlayerProtect.Protect();
      }
   }
}