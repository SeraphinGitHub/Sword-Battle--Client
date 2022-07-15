using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine.Networking; // ==> UnityWebRequest
using Newtonsoft.Json; // ==> JsonConvert.DeserializeObject
using UnityEngine.UI;
using TMPro;

public class GameHandler : MonoBehaviour {
   
   [Header("**Server Options**")]
   public int FPS = 10;
   public string URL = "http://localhost:3000/";

   [Header("**BattleList Options**")]
   public int firstBattleOffsetY = 20;
   public int battleOffsetY = 155;
   public int battleCountBeforeScroll = 5;

   [Header("**Attached Objects**")]
   public GameObject PlayerPrefab;
   public InputField PlayerNameField;
   public InputField BattleNameField;
   public GameObject JoinBtnPrefab;
   public RectTransform ScrollContent;

   // Private Variables
   private SocketIOUnity socket;
   private float FrameRate() { return Mathf.Floor(1f/FPS *1000)/1000; }
   private string playerName() { return PlayerNameField.text; }
   private string battleName() { return BattleNameField.text; }
   private List<string> existBattle_IDList = new List<string>();
   private List<string> newBattles_IDList = new List<string>();

   // Test Var
   private float posX() { return Mathf.Floor(PlayerPrefab.transform.position.x *10) /10; }
   private bool isAttacking() { return PlayerPrefab.GetComponent<PlayerAttack>().isAttacking; }


   // ====================================================================================
   // Start
   // ====================================================================================
   public void Start() {

      // Init Socket IO
      socket = new SocketIOUnity(URL, new SocketIOOptions {
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
   }


   // ====================================================================================
   // Server Sync Methods (Every Fame)
   // ====================================================================================
   public void StartSync(string methodName) {
      InvokeRepeating(methodName, 0, FrameRate());
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
      StartCoroutine(FixLatency());
   }

   IEnumerator FixLatency() {
      CreatedBattleClass createdBattle = new CreatedBattleClass(
         playerName(),
         battleName()
      );

      yield return new WaitForSeconds(0.3f);
      socket.Emit("createBattle", createdBattle);
   }


   // ================= Find Battle =================
   public void FindBattle() {
      SocketIO_Connect();
      RefreshBattleList("SearchBattle");
   }

   public void RefreshBattleList(string methodName) {
      float refreshRate = 0.7f; // seconds
      InvokeRepeating(methodName, 0, refreshRate);
   }

   public void SearchBattle() {
      StartCoroutine(RenderBattleList());
   }   

   // Web Request (Render list of battles)
   IEnumerator RenderBattleList() {

      UnityWebRequest request = UnityWebRequest.Get(URL + "find-battle");
      yield return request.SendWebRequest();

      if(!request.isNetworkError) {
         var battlesArray = JsonConvert.DeserializeObject<FoundBattleClass[]>(request.downloadHandler.text);
         newBattles_IDList.Clear();

         // ========================
         // Add new Battles
         // ========================
         foreach (var battle in battlesArray) {
            newBattles_IDList.Add(battle.id);

            // If new battle not rendered already
            if(!existBattle_IDList.Contains(battle.id)) {
               int existBattleCount = existBattle_IDList.Count;
               float JoinBtn_PosY = -firstBattleOffsetY -battleOffsetY *existBattleCount;

               GameObject JoinBtn = Instantiate(JoinBtnPrefab, new Vector3(0, JoinBtn_PosY), Quaternion.identity);
               JoinBtn.transform.SetParent(ScrollContent, false);

               JoinBtn.transform.Find("BattleID").gameObject.GetComponent<TMP_Text>().text = battle.id;
               JoinBtn.transform.Find("BattleName").gameObject.GetComponent<TMP_Text>().text = battle.name;

               // Extend ScorllContent height
               if(existBattleCount >= battleCountBeforeScroll) {
                  ScrollContent.sizeDelta = new Vector2(0, ScrollContent.sizeDelta.y + battleOffsetY);
               }
               existBattle_IDList.Add(battle.id);
            }
         }
        

         // ========================
         // Remove old Battles
         // ========================
         int renderedBattleCount = ScrollContent.childCount;

         // For all rendered battle
         for(int i = 0; i < renderedBattleCount; i++) {
            Transform JoinBtn = ScrollContent.GetChild(i);
            string BattleID = JoinBtn.Find("BattleID").GetComponent<TMP_Text>().text;            
            
            // If rendered battle doesn't exist anymore 
            if(!newBattles_IDList.Contains(BattleID)) {
               Destroy(JoinBtn.gameObject);
               existBattle_IDList.Remove(BattleID);

               // Move up other rendered battle after the destroyed one
               for(int j = 0; j < renderedBattleCount -i; j++) {
                  RectTransform OtherJoinBtn = ScrollContent.GetChild(j +i).GetComponent<RectTransform>();
                  float OtherJoinBtn_PosY = OtherJoinBtn.anchoredPosition.y + battleOffsetY;
                  OtherJoinBtn.anchoredPosition = new Vector2(0, OtherJoinBtn_PosY);
               }

               // Shorten ScorllContent height
               int existBattleCount = existBattle_IDList.Count;
               if(existBattleCount >= battleCountBeforeScroll) {
                  ScrollContent.sizeDelta = new Vector2(0, ScrollContent.sizeDelta.y - battleOffsetY);
               }
            }
         }
      }
   }


   // ================= Join Battle =================
   public void JoinBattle() {
      socket.Emit("joinBattle", "JoinBattle");
   }
   

   // ================= Leave Battle =================
   public void LeaveBattle() {
      socket.Disconnect();
   }

   public void BattleEnded() {
      socket.On("battleEnded", (response) => {
         
         Debug.Log(response.GetValue());

         // Display server message
         // CountDown (3s maybe)
         // hide battleUI
         // show main menu
      });
   }


   // ================= Quit Application =================
   public void QuitApplication() {
      SocketIO_Disconnect();
      Application.Quit();
   }


   // ====================================================================================
   // Player Methods
   // ====================================================================================
   public void MoveLeft() {
      socket.Emit("Left", posX());
   }

   public void MoveRight() {
      socket.Emit("Right", posX());
   }

   public void AttackStrike() {
      socket.Emit("AttackStrike", isAttacking());
   }

   public void AttackEstoc() {
      socket.Emit("AttackEstoc", isAttacking());
   }
}