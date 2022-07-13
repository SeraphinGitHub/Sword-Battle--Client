using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;

public class NetworkManager : MonoBehaviour {

   [Header("Server Options")]
   public int FPS = 10;
   public string URL = "http://localhost:3000/";

   [Header("Attached Objects")]
   public GameObject GameHandler;

   // Private Scripts
   private SocketIOUnity socket;
   private GameHandler gameHandler;

   // Private Variables
   private float FrameRate() { return Mathf.Floor(1f/FPS *1000)/1000; }


   // ====================================================================================
   // Start
   // ====================================================================================
   public void Start() {
      
      gameHandler = GameHandler.GetComponent<GameHandler>();
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
   // // Init Socket IO
   // ====================================================================================
   public void SocketIO_Connect() {

      socket = new SocketIOUnity(URL, new SocketIOOptions {
         Query = new Dictionary<string, string> {
            {"token", "UNITY" }
         },
         EIO = 4,
         Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
      });

      socket.JsonSerializer = new NewtonsoftJsonSerializer();
      socket.Connect();

      // ************************  DEBUG  ************************
      socket.On("Get", (data) => Debug.Log(data));      
      // ************************  DEBUG  ************************
   }

   public void SocketIO_Disconnect() {
      socket.Disconnect();
   }


   // ====================================================================================
   // Menu Methods
   // ====================================================================================
   
   // ================= Create Battle =================
	public void CreateBattle() {
      SocketIO_Connect();
      StartCoroutine(FixLatency());
   }

   IEnumerator FixLatency() {
      CreatedBattle createdBattle = new CreatedBattle(
         gameHandler.playerName(),
         gameHandler.battleName()
      );

      yield return new WaitForSeconds(0.3f);
      socket.Emit("createBattle", createdBattle);
   }

   // Create Battle Class
   [System.Serializable]
   class CreatedBattle {

      public string playerName;
      public string battleName;

      public CreatedBattle(
         string playerName,
         string battleName) {
            
         this.playerName = playerName;
         this.battleName = battleName;
      }
   }


   // ================= Find Battle =================
   public void SearchBattle() {
      socket.Emit("findBattle");
   }

   public void FindBattle() {

      SocketIO_Connect();
      StartSync("SearchBattle");

      socket.On("battleFound", (response) => {
         var battle = response.GetValue<FoundBattle>();
         gameHandler.joinableBattle.Add(new string[] { battle.id, battle.name });
      });
   }

   // Found Battle Class
   [System.Serializable]
   class FoundBattle {

      public string id;
      public string name;

      public FoundBattle(string id, string name) {
         this.id = id;
         this.name = name;
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


   // ================= Quit Application =================
   public void QuitApplication() {
      Application.Quit();
   }


   // ====================================================================================
   // Player Methods
   // ====================================================================================
   public void MoveLeft() {
      socket.Emit("Left", gameHandler.posX());
   }

   public void MoveRight() {
      socket.Emit("Right", gameHandler.posX());
   }

   public void AttackStrike() {
      socket.Emit("AttackStrike", gameHandler.isAttacking());
   }

   public void AttackEstoc() {
      socket.Emit("AttackEstoc", gameHandler.isAttacking());

      // *******************************************
      socket.On("azerty", (response) => {

         var dataObj = response.GetValue<TestData>();

         string aze = dataObj.name;
         int qsd = dataObj.id;
         bool wxc = dataObj.isPos;

         Debug.Log(aze);
         Debug.Log(qsd);
         Debug.Log(wxc);
      });
      // *******************************************
   }


   // *******************************************
   [System.Serializable]
   class TestData {

      public string name;
      public int id;
      public bool isPos;

      public TestData(string name, int id, bool isPos) {
         this.name = name;
         this.id = id;
         this.isPos = isPos;
      }
   }
   // *******************************************
}