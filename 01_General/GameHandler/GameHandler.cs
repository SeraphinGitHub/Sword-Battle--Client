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
   public string serverURL = "http://localhost:3000/";
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

   [Header("**Attached InputFields**")]
   public InputField playerNameField;
   public InputField battleNameField;
   
   [Header("**Attached TextMeshPro**")]
   public TextMeshProUGUI serverMessageTMP;
   public TextMeshProUGUI countDownTMP;
   public TextMeshProUGUI battleNameTMP;
   public TextMeshProUGUI leftPlayerNameTMP;
   public TextMeshProUGUI rightPlayerNameTMP;

   // Private Variables
   private SocketIOUnity socket;
   private PlayerRandomize playerRandomize;
   
   private float frameRate;
   private int baseEndBattleCD;
   private bool isBattleOnGoing = false;

   private string battleName;
   private string joinedBattleName;
   private string playerName;
   private string enemyName;
   private string playerSide;
   private string enemySide;
   
   private List<string> existBattle_IDList = new List<string>();
   private List<string> newBattles_IDList = new List<string>();
   private List<string> hostPropsList = new List<string>();



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

      public PlayerClass(
      string name,
      string side,
      string hairStyle,
      string hairColor,
      string tabardColor) {

         this.name = name;
         this.side = side;
         this.hairStyle = hairStyle;
         this.hairColor = hairColor;
         this.tabardColor = tabardColor;
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
   class BattleEnd {
      
      public bool isHostPlayer;
      public bool isJoinPlayer;

      public BattleEnd(bool isHostPlayer, bool isJoinPlayer) {
         this.isHostPlayer = isHostPlayer;
         this.isJoinPlayer = isJoinPlayer;
      }
   }

   // **************************
   [System.Serializable]
   class GetSyncData {
      
      public string data;

      public GetSyncData(string data) {
         this.data = data;
      }
   }
   // **************************


   // ====================================================================================
   // Awake() / Start()
   // ====================================================================================
   private void Awake() {
      mainMenu.SetActive(true);
      findBattleMenu.SetActive(false);
      optionsMenu.SetActive(false);
      battleUI.SetActive(false);
      popUpMessageUI.SetActive(false);

      serverMessageTMP.gameObject.SetActive(false);
      countDownTMP.gameObject.SetActive(false);

      playerRandomize = GetComponent<PlayerRandomize>();
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
      battleName = battleNameField.text;
      baseEndBattleCD = endBattleCD;
      frameRate = Mathf.Floor(1f/FPS *1000)/1000;


      // ===================================
      // Socket Listening Events
      // ===================================
      socket.OnAnyInUnityThread((channel, response) => {
         
         // Create Battle
         if(channel == "battleCreated") {
            StartCoroutine(InitBattle(battleName));

            // Debug.Log(response.GetValue());
         }

         // Join Battle
         if(channel == "joinBattleAccepted") {
            var enemyPlayer = response.GetValue<PlayerClass>();
            JoinBattle(enemyPlayer);
            // Debug.Log(response.GetValue());
         }

         if(channel == "battleJoined") {
            isBattleOnGoing = true;
            StartCoroutine(InitBattle(joinedBattleName));

            // Debug.Log(response.GetValue());
         }

         if(channel == "enemyJoined") {
            var enemyPlayer = response.GetValue<PlayerClass>();

            serverMessageTMP.gameObject.SetActive(false);
            StartCoroutine(InitEnemyPlayer(enemyPlayer));

            // Debug.Log(response.GetValue());
         }

         // Leave Battle
         if(channel == "battleEnded") {
            var leavingPlayer = response.GetValue<BattleEnd>();            

            if(leavingPlayer.isHostPlayer) {

               isBattleOnGoing = false;
               playerRandomize.DestroyOnePlayer("hostPlayer");
               hostPropsList.Clear();

               serverMessageTMP.text = "Host player left battle !";              
               serverMessageTMP.gameObject.SetActive(true);
               countDownTMP.gameObject.SetActive(true);
               
               SetInterval("CountDown", 1f);
            }

            if(leavingPlayer.isJoinPlayer) {

               playerRandomize.DestroyOnePlayer("joinPlayer");
               serverMessageTMP.text = "Join player left battle !";
               BaseSetName(enemySide, "");
               serverMessageTMP.gameObject.SetActive(true);
            }

            // Debug.Log(response.GetValue());
         }


         // **************
         if(channel == "Get_Position") {
            float posX = float.Parse(response.GetValue<GetSyncData>().data);
            Transform enemyTransform = playerRandomize.playersList[1].transform;
            enemyTransform.position = new Vector3(posX, enemyTransform.position.y, 0);
         }
         // **************
      });
   }



   // ====================================================================================
   // Socket Listening Methods
   // ====================================================================================
   IEnumerator InitBattle(string battleName) {

      leftPlayerNameTMP.text = "";
      rightPlayerNameTMP.text = "";
      battleNameTMP.text = battleName;
      SwitchToBattle();

      yield return new WaitForSeconds(randomizeDelay);

      if(isBattleOnGoing) {
         BaseSetName(playerSide, playerName);

         // If hostPlayer already exists
         if(hostPropsList.Count != 0) {
            string enemySide = hostPropsList[0];
            BaseSetName(enemySide, enemyName);

            playerRandomize.isLocalPlayer = false;
            playerRandomize.InstantiatePlayer(hostPropsList);
         }

         playerRandomize.isLocalPlayer = true;
         playerRandomize.InstantiatePlayer(playerRandomize.playerPropsList);
      }
      else yield break;
   }

   IEnumerator InitEnemyPlayer(PlayerClass enemyPlayer) {

      List<string> joinPropsList = new List<string>() {
         enemyPlayer.side,
         enemyPlayer.hairStyle,
         enemyPlayer.hairColor,
         enemyPlayer.tabardColor,
      };

      yield return new WaitForSeconds(randomizeDelay);
      
      playerRandomize.isLocalPlayer = false;
      playerRandomize.InstantiatePlayer(joinPropsList);

      enemySide = enemyPlayer.side;
      BaseSetName(enemySide, enemyPlayer.name);
   }

   private void BaseSetName(string side, string name) {
      if(side == "Left") leftPlayerNameTMP.text = name;
      if(side == "Right") rightPlayerNameTMP.text = name;
   }

   private void CountDown() {
      countDownTMP.text = endBattleCD.ToString();
      endBattleCD--;

      if(endBattleCD  < 0) {
         endBattleCD = baseEndBattleCD;
         playerRandomize.DestroyOnePlayer("joinPlayer");
         
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



   // ====================================================================================
   // General Methods
   // ====================================================================================

   // Server Sync Methods (Every Fame)
   public void StartSync(string methodName) {
      InvokeRepeating(methodName, 0, frameRate);
   }

   public void StopSync() {
      CancelInvoke();
   }

   private void SetInterval(string methodName, float refreshRate) {
      InvokeRepeating(methodName, 0, refreshRate);
   }
   

   // Socket IO Connection
   public void SocketIO_Connect() {
      socket.Connect();
   }

   public void SocketIO_Disconnect() {
      socket.Disconnect();
   }


   // Update inputFields OnChange()
   public void UpdatePlayerName() {
      playerName = playerNameField.text;
   }

   public void UpdateBattleName() {
      battleName = battleNameField.text;
   }

   

   // ====================================================================================
   // Menu Methods
   // ====================================================================================

   // Create Battle
	public void CreateBattle() {
      if(!isBattleOnGoing) {

         isBattleOnGoing = true;
         playerRandomize.RunRandomize();

         playerSide = playerRandomize.playerPropsList[0];
         string playerHair = playerRandomize.playerPropsList[1];
         string playerHairColor = playerRandomize.playerPropsList[2];
         string playerTabardColor = playerRandomize.playerPropsList[3];

         // Set hostPlayer
         PlayerClass hostPlayer = new PlayerClass(
            playerName,
            playerSide,
            playerHair,
            playerHairColor,
            playerTabardColor
         );

         // Set battle
         CreateBattleClass newBattle = new CreateBattleClass(
            battleName,
            hostPlayer
         );

         // Await for Socket.io to be connected
         StartCoroutine(EmitNewBattle(newBattle));
      }
   }

   IEnumerator EmitNewBattle(CreateBattleClass newBattle) {
      
      SocketIO_Connect();
      yield return new WaitForSeconds(0.2f);
      socket.Emit("createBattle", newBattle);
   }


   // Find Battle
   public void FindBattle() {
      SocketIO_Connect();
      SetInterval("SetBattleList", 0.7f); // seconds
   }

   private void SetBattleList() {
      socket.OnUnityThread("findBattle", (response) => {

         var battlesArray = response.GetValue<FoundBattleClass[]>();
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
      });
   }


   // Join Battle
   public void JoinBattleRequest(string battleID, string battleName) {
      // Used in > JoinBattleHandler.cs

      joinedBattleName = battleName;
      socket.Emit("joinBattleRequest", battleID);
   }

   private void JoinBattle(PlayerClass enemyPlayer) {
      
      // Set enemy player (Host player)
      enemyName = enemyPlayer.name;
      hostPropsList.Add(enemyPlayer.side);
      hostPropsList.Add(enemyPlayer.hairStyle);
      hostPropsList.Add(enemyPlayer.hairColor);
      hostPropsList.Add(enemyPlayer.tabardColor);
      
      // Randomize joinPlayer, out of hostPlayer props
      playerRandomize.RemoveRandomizeProps(hostPropsList);
      playerRandomize.RunRandomize();

      playerSide = playerRandomize.playerPropsList[0];
      string playerHairStyle = playerRandomize.playerPropsList[1];
      string playerHairColor = playerRandomize.playerPropsList[2];
      string playerTabardColor = playerRandomize.playerPropsList[3];

      // Set joinPlayer
      PlayerClass joinPlayerProps = new PlayerClass(
         playerName,
         playerSide,
         playerHairStyle,
         playerHairColor,
         playerTabardColor
      );

      socket.Emit("joinBattle", joinPlayerProps);
   }


   // Leave Battle
   public void LeaveBattle() {

      isBattleOnGoing = false;
      playerRandomize.DestroyAllPlayers();
      hostPropsList.Clear();

      SwitchToMainMenu();
      socket.Disconnect();
   }


   // Quit Application
   public void QuitApplication() {
      SocketIO_Disconnect();
      Application.Quit();
   }



   // ====================================================================================
   // Player Methods
   // ====================================================================================
   public void MoveLeft() {
      // **********
      float posX = playerRandomize.playersList[0].transform.position.x;
      // **********
      socket.Emit("Position", posX);
   }

   public void MoveRight() {
      // **********
      float posX = playerRandomize.playersList[0].transform.position.x;
      // **********
      socket.Emit("Position", posX);
   }

   public void AttackStrike() {
      // socket.Emit("AttackStrike", isAttacking);
      Debug.Log("Attack Strike");
   }

   public void AttackEstoc() {
      // socket.Emit("AttackEstoc", isAttacking);
      Debug.Log("Attack Estoc");
   }
}