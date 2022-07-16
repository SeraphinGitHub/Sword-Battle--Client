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
   private int baseEndBattleCD() { return endBattleCD; }
   private List<string> existBattle_IDList = new List<string>();
   private List<string> newBattles_IDList = new List<string>();

   // Test Var
   private float posX;
   private bool isAttacking;

   // ====================================================================================
   // Awake() / Start()
   // ====================================================================================
   public void Awake() {
      // mainMenu.SetActive(true);
      // findBattleMenu.SetActive(false);
      // optionsMenu.SetActive(false);
      // battleUI.SetActive(false);

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
socket.Connect();

      playerName = playerNameField.text;
      battleName = battleNameField.text;
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
            mainMenu.SetActive(false);
            battleUI.SetActive(true);
            battleNameTMP.text = battleName;
            leftPlayerNameTMP.text = playerName;
            rightPlayerNameTMP.text = "";

            // Debug.Log(response.GetValue());
         }

         // ================= Leave Battle =================
         if(channel == "battleEnded") {
            string serverMessage = response.GetValue().GetRawText();
            serverMessageTMP.text = serverMessage;
            serverMessageTMP.gameObject.SetActive(true);

            if(serverMessage.Contains("Host")) {
               countDownTMP.gameObject.SetActive(true);
               StartCoroutine(CountDown());
               StartCoroutine(DisplayCountDown());
            }

            else {
               mainMenu.SetActive(true);
               battleUI.SetActive(false);
               serverMessageTMP.gameObject.SetActive(false);
               countDownTMP.gameObject.SetActive(false);
            }

            // Debug.Log(response.GetValue());
         }
      });
   }   

   IEnumerator DisplayCountDown() {
      yield return new WaitForSeconds(1);
      countDownTMP.text = endBattleCD.ToString();
      endBattleCD--;
      if(endBattleCD <= 0) endBattleCD = baseEndBattleCD();
   }

   IEnumerator CountDown() {
      yield return new WaitForSeconds(endBattleCD);
      mainMenu.SetActive(true);
      battleUI.SetActive(false);
      serverMessageTMP.gameObject.SetActive(false);
      countDownTMP.gameObject.SetActive(false);
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
   [System.Serializable]
   class CreatedBattleClass {

      public string playerName;
      public string battleName;

      public CreatedBattleClass(string playerName, string battleName) {
         this.playerName = playerName;
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
      RefreshBattleList("SetBattleList");
   }

   public void RefreshBattleList(string methodName) {
      float refreshRate = 0.7f; // seconds
      InvokeRepeating(methodName, 0, refreshRate);
   }

   public void SetBattleList() {
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
   public void JoinBattle() {
      socket.Emit("joinBattle", "JoinBattle");
   }
   

   // ================= Leave Battle =================
   public void LeaveBattle() {
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