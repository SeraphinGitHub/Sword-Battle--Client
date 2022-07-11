using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour {

   private SocketIOUnity socket;

   [Header("Server Options")]
   public int FPS = 10;
   public string URL = "http://localhost:3000/";

   [Header("Player GameObject")]
   public GameObject Player;

   private float FrameRate() { return Mathf.Floor(1f/FPS *1000)/1000; }
   private float posX() { return Mathf.Floor(Player.transform.position.x *10) /10; }
   private bool isAttacking() { return Player.GetComponent<PlayerAttack>().isAttacking; }

   // **********************************
   public string playerName;
   public string battleName;
   private List<string> createBattleData = new List<string>();
   // **********************************


   // Sync
   public void StartSync(string methodName) {
      InvokeRepeating(methodName, 0, FrameRate());
   }

   public void StopSync() {
      CancelInvoke();
   }


   // Menu Methods
   public void SetPlayerName() {
      string playerName = GameObject.Find("PlayerNameField").GetComponent<InputField>().text;
   }

   public void SetBattleName() {
      string playerName = GameObject.Find("BattleNameField").GetComponent<InputField>().text;
   }

   // Create Battle / Init Socket IO
	public void CreateBattle() {

      // Init Socket IO
      socket = new SocketIOUnity(URL, new SocketIOOptions {
         Query = new Dictionary<string, string> {
            {"token", "UNITY" }
         },
         EIO = 4,
         Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
      });

      socket.JsonSerializer = new NewtonsoftJsonSerializer();
      socket.Connect();
      
      // Socket listener
      socket.On("Get", (data) => Debug.Log(data));

      // Data to send
      string playerName = GameObject.Find("PlayerNameField").GetComponent<InputField>().text;
      string battleName = GameObject.Find("BattleNameField").GetComponent<InputField>().text;
      createBattleData.Add(playerName);
      createBattleData.Add(battleName);
      
      // Fix latency
      StartCoroutine(sendCreateBattle());
   }

   IEnumerator sendCreateBattle() {
      yield return new WaitForSeconds(0.5f);
      socket.Emit("createBattle", createBattleData);
      createBattleData.Clear();
   }

   public void FindBattle() {
      socket.Emit("findBattle", "FindBattle");
   }

   public void JoinBattle() {
      socket.Emit("joinBattle", "JoinBattle");
   }

   public void LeaveBattle() {
      socket.Disconnect();
   }


   // Player Methods
	public void MoveLeft() {
      socket.Emit("Left", posX());
   }

   public void MoveRight() {
      socket.Emit("Right", posX());
   }

   public void AttackingStrike() {
      socket.Emit("AttackingStrike", isAttacking());
   }

   public void AttackingEstoc() {
      socket.Emit("AttackingEstoc", isAttacking());
   }
}