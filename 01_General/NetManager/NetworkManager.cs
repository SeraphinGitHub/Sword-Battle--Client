using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;

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
   

   // Start
   void Start() {

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
   }


   // Methods
   public void StartSync(string methodName) {
      InvokeRepeating(methodName, 0, FrameRate());
   }

   public void StopSync() {
      CancelInvoke();
   }

	public void MoveLeft() {
      socket.Emit("Left", posX());
   }

   public void MoveRight() {
      socket.Emit("Right", posX());
   }

   public void Attacking() {
      socket.Emit("Attacking", isAttacking());
   }
}
