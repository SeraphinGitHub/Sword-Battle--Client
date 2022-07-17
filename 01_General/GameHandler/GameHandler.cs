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
   public int endBattleCD = 3; //seconds
   public string serverURL = "http://localhost:3000/";

   [Header("**BattleList Options**")]
   public int firstBattleOffsetY = 20;
   public int battleOffsetY = 155;
   public int battleCountBeforeScroll = 5;

   [Header("**Attached Transforms**")]
   public RectTransform scrollContent;

   [Header("**Attached Prefabs**")]
   public GameObject playerPrefab;
   public GameObject joinBtnPrefab;

   [Header("**Attached Canvas**")]
   public GameObject mainMenu;
   public GameObject findBattleMenu;
   public GameObject optionsMenu;
   public GameObject battleUI;

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
   private float frameRate;
   private string playerName;
   private string battleName;
   private string joinedBattleName;
   private int baseEndBattleCD;
   private List<string> existBattle_IDList = new List<string>();
   private List<string> newBattles_IDList = new List<string>();

   // Test Var
   private float posX;
   private bool isAttacking;


   // ====================================================================================
   // Transfert Data Classes
   // ====================================================================================
   [System.Serializable]
   class CreatedBattleClass {

      public string name;
      public string battleName;

      public CreatedBattleClass(string name, string battleName) {
         this.name = name;
         this.battleName = battleName;
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
   class JoinPlayerClass {

      public string battleID;
      public string name;
      public string color;
      public string side;

      public JoinPlayerClass(
      string battleID,
      string name,
      string color,
      string side) {

         this.battleID = battleID;
         this.name = name;
         this.color = color;
         this.side = side;
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


   // ====================================================================================
   // Awake() / Start()
   // ====================================================================================
   public void Awake() {
      mainMenu.SetActive(true);
      findBattleMenu.SetActive(false);
      optionsMenu.SetActive(false);
      battleUI.SetActive(false);

      serverMessageTMP.gameObject.SetActive(false);
      countDownTMP.gameObject.SetActive(false);
   }

   public void Start() {

      // Init Socket IO
      socket = new SocketIOUnity(serverURL, new SocketIOOptions {
         Query = new Dictionary<string, string> {
            {"token", "UNITY" }
         },
         EIO = 4,
         Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
      });
      socket.JsonSerializer = new NewtonsoftJsonSerializer();

      // ************************  DEBUG  ************************
      socket.On("Get", (data) => Debug.Log(data));
      // ************************  DEBUG  ************************

      playerName = playerNameField.text;
      battleName = battleNameField.text;
      baseEndBattleCD = endBattleCD;
      frameRate = Mathf.Floor(1f/FPS *1000)/1000;

      // // Test Var
      // posX = Mathf.Floor(playerPrefab.transform.position.x *10) /10;
      // isAttacking = playerPrefab.GetComponent<PlayerAttack>().isAttacking;


      // ====================================================================================
      // Socket.io Listening
      // ====================================================================================
      socket.OnAnyInUnityThread((channel, response) => {
         
         // ================= Create Battle =================
         if(channel == "battleCreated") {

            battleNameTMP.text = battleName;
            leftPlayerNameTMP.text = playerName;
            rightPlayerNameTMP.text = "";

            SwitchToBattle();
            // Debug.Log(response.GetValue());
         }

         // ================= Join Battle =================
         if(channel == "battleJoined") {
            var enemyPlayer = response.GetValue<JoinPlayerClass>();
            
            battleNameTMP.text = joinedBattleName;
            leftPlayerNameTMP.text = enemyPlayer.name;
            rightPlayerNameTMP.text = playerName;

            SwitchToBattle();
            // Debug.Log(response.GetValue());
         }

         if(channel == "enemyJoined") {
            var enemyPlayer = response.GetValue<JoinPlayerClass>();

            rightPlayerNameTMP.text = enemyPlayer.name;
            serverMessageTMP.gameObject.SetActive(false);

            // Debug.Log(response.GetValue());
         }

         // ================= Leave Battle =================
         if(channel == "battleEnded") {
            var leavingPlayer = response.GetValue<BattleEnd>();

            // HostPlayer leave
            if(leavingPlayer.isHostPlayer) {

               serverMessageTMP.text = "Host player left battle !";              
               serverMessageTMP.gameObject.SetActive(true);
               countDownTMP.gameObject.SetActive(true);
               SetInterval("CountDown", 1f);
            }

            // JoinPlayer leave
            if(leavingPlayer.isJoinPlayer) {

               serverMessageTMP.text = "Join player left battle !";
               rightPlayerNameTMP.text = "";
               serverMessageTMP.gameObject.SetActive(true);
            }

            // Debug.Log(response.GetValue());
         }
      });
   }

   private void CountDown() {
      countDownTMP.text = endBattleCD.ToString();
      endBattleCD--;

      if(endBattleCD  < 0) {
         endBattleCD = baseEndBattleCD;
         if(mainMenu.activeSelf) SwitchToMainMenu();
         CancelInvoke();
      }
   }

   private void SwitchToBattle() {
      mainMenu.SetActive(false);
      findBattleMenu.SetActive(false);
      battleUI.SetActive(true);
   }

   private void SwitchToMainMenu() {
      mainMenu.SetActive(true);
      battleUI.SetActive(false);
      serverMessageTMP.gameObject.SetActive(false);
      countDownTMP.gameObject.SetActive(false);
   }

   private void SetInterval(string methodName, float refreshRate) {
      InvokeRepeating(methodName, 0, refreshRate);
   }

   // ====================================================================================
   // Server Sync Methods (Every Fame)
   // ====================================================================================
   public void StartSync(string methodName) {
      InvokeRepeating(methodName, 0, frameRate);
   }

   public void StopSync() {
      CancelInvoke();
   }


   // ====================================================================================
   // Socket IO Connection
   // ====================================================================================
   public void SocketIO_Connect() {
      socket.Connect();
   }

   public void SocketIO_Disconnect() {
      socket.Disconnect();
   }


   // ====================================================================================
   // Update inputFields OnChange()
   // ====================================================================================
   public void UpdatePlayerName() {
      playerName = playerNameField.text;
   }

   public void UpdateBattleName() {
      battleName = battleNameField.text;
   }

   
   // ====================================================================================
   // Menu Methods
   // ====================================================================================

   // ================= Create Battle =================
	public void CreateBattle() {
      SocketIO_Connect();
      StartCoroutine(SetNewBattle()); // Await for Socket.io to be connected
   }

   IEnumerator SetNewBattle() {
      CreatedBattleClass createdBattle = new CreatedBattleClass(playerName, battleName);
      yield return new WaitForSeconds(0.2f);
      socket.Emit("createBattle", createdBattle);
   }


   // ================= Find Battle =================
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


   // ================= Join Battle =================
   public void JoinBattle(string battleID, string battleName) {

      joinedBattleName = battleName;

      JoinPlayerClass joinPlayerClass = new JoinPlayerClass(
         battleID,
         playerName,
         "Red",
         "Right"
      );

      socket.Emit("joinBattle", joinPlayerClass);
   }


   // ================= Leave Battle =================
   public void LeaveBattle() {
      SwitchToMainMenu();
      socket.Disconnect();
   }


   // ================= Quit Application =================
   public void QuitApplication() {
      SocketIO_Disconnect();
      Application.Quit();
   }


   // ====================================================================================
   // Player Methods
   // ====================================================================================
   // public void MoveLeft() {
   //    socket.Emit("Left", posX);
   // }

   // public void MoveRight() {
   //    socket.Emit("Right", posX);
   // }

   // public void AttackStrike() {
   //    socket.Emit("AttackStrike", isAttacking);
   // }

   // public void AttackEstoc() {
   //    socket.Emit("AttackEstoc", isAttacking);
   // }
}