using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine.Networking; // ==> UnityWebRequest
using Newtonsoft.Json; // ==> JsonConvert.DeserializeObject
using UnityEngine.UI;
using TMPro;

public class NetworkManager : MonoBehaviour {
   
   [Header("**Server Options**")]
   public int FPS = 10;
   public string URL = "http://localhost:3000/";

   [Header("**Attached Objects**")]
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
   public void FindBattle() {
      SocketIO_Connect();
      RefreshBattleList("SearchBattle");
   }

   public void RefreshBattleList(string methodName) {
      InvokeRepeating(methodName, 0, 1);
   }

   public void SearchBattle() {
      StartCoroutine(RenderBattleList());
   }   

   // Web Request (Render list of battles)
   IEnumerator RenderBattleList() {

      UnityWebRequest request = UnityWebRequest.Get(URL + "find-battle");
      yield return request.SendWebRequest();

      if(!request.isNetworkError) {

         gameHandler.newBattles_IDList.Clear();
         RectTransform ScrollContent = gameHandler.ScrollContent.transform.GetComponent<RectTransform>();

         var battleArray = JsonConvert.DeserializeObject<FoundBattle[]>(request.downloadHandler.text);

         int firstBattleOffsetY = 20;
         int battleOffsetY = 155;
         int battleCountBeforeScroll = 5;

         // Render list of battles
         foreach (var battle in battleArray) {

            gameHandler.newBattles_IDList.Add(battle.id);

            if(!gameHandler.existBattle_IDList.Contains(battle.id)) {
               int battleCount = gameHandler.existBattle_IDList.Count;

               GameObject JoinBtn = Instantiate(gameHandler.JoinBtn, new Vector3(0, -firstBattleOffsetY -battleOffsetY *battleCount), Quaternion.identity);
               JoinBtn.transform.SetParent(gameHandler.ScrollContent.transform, false);

               JoinBtn.transform.Find("BattleID").gameObject.GetComponent<TMP_Text>().text = battle.id;
               JoinBtn.transform.Find("BattleName").gameObject.GetComponent<TMP_Text>().text = battle.name;

               if(battleCount >= battleCountBeforeScroll) {
                  ScrollContent.sizeDelta = new Vector2(0, ScrollContent.sizeDelta.y + battleOffsetY);
               }

               gameHandler.existBattle_IDList.Add(battle.id);
            }
         }

         // foreach (Transform JoinBtn in gameHandler.ScrollContent.transform) {

         //    string BattleID = JoinBtn.Find("BattleID").GetComponent<TMP_Text>().text;            
               
         //    if(!gameHandler.newBattles_IDList.Contains(BattleID)) {
         //       int battleCount = gameHandler.existBattle_IDList.Count;

         //       Debug.Log("Battle id: "+ BattleID +" has been Destroyed !");
         //       Destroy(JoinBtn.gameObject);

         //       if(battleCount >= battleCountBeforeScroll) {
         //          ScrollContent.sizeDelta = new Vector2(0, ScrollContent.sizeDelta.y - battleOffsetY);
         //       }
         //    }
         // }


         int renderedBattleCount = gameHandler.ScrollContent.transform.childCount;
         // Debug.Log(renderedBattleCount);

         for(int i = 0; i < renderedBattleCount; i++) {

            Transform JoinBtn = ScrollContent.GetChild(i);
            string BattleID = JoinBtn.Find("BattleID").GetComponent<TMP_Text>().text;            
               
            if(!gameHandler.newBattles_IDList.Contains(BattleID)) {

               Debug.Log("Battle id: "+ BattleID +" has been Destroyed !");
               Destroy(JoinBtn.gameObject);

               for(int j = 0; j < renderedBattleCount -i; j++) {

                  RectTransform JoinOtherBtn = ScrollContent.GetChild(j).GetComponent<RectTransform>();
                  JoinOtherBtn.anchoredPosition = new Vector2(0, JoinOtherBtn.anchoredPosition.y + battleOffsetY);
               
               }

               int battleCount = gameHandler.existBattle_IDList.Count;

               if(battleCount >= battleCountBeforeScroll) {
                  ScrollContent.sizeDelta = new Vector2(0, ScrollContent.sizeDelta.y - battleOffsetY);
               }
            }
         }
      }
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

   public void BattleEnded() {
      socket.On("battleEnded", (response) => {
         
         Debug.Log(response.GetValue());
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
   }
}