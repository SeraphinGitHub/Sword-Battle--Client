using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SocketIOClient; // Necessary
using SocketIOClient.Newtonsoft.Json; // Necessary


public class ExampleSocketIO : MonoBehaviour {

   private SocketIOUnity socket; // Necessary
   public string URL = "http://localhost:3000/";

   public string string_1;
   public int    int_1;
   public bool   bool_1;

   public string string_2;
   public int    int_2;
   public bool   bool_2;

   // ====================================================================================
   // Connecting
   // ====================================================================================
	public void SocketConnect() {

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
   }


   // ====================================================================================
   // Emiting
   // ====================================================================================
	public void SocketEmit() {

      TestEmitClass testEmitClass = new TestEmitClass(string_1, int_1, bool_1);
      socket.Emit("Emiting", testEmitClass);
   }

   [System.Serializable] // Necessary
   class TestEmitClass {

      public string emitString;
      public int    emitInt;
      public bool   emitBool;

      public TestEmitClass(string emitString, int emitInt, bool emitBool) {

         this.emitString = emitString;
         this.emitInt    = emitInt;
         this.emitBool   = emitBool;
      }
   }


   // ====================================================================================
   // Receiving
   // ====================================================================================
   public void SocketOn() {

      socket.On("Receiving", (response) => {

         var dataObj = response.GetValue<TestReceivedClass>();

         string string_2 = dataObj.receivedString;
         int    int_2    = dataObj.receivedInt;
         bool   bool_2   = dataObj.receivedBool;

         Debug.Log(string_2);
         Debug.Log(int_2);
         Debug.Log(bool_2);
      });
   }

   [System.Serializable] // Necessary
   class TestReceivedClass {

      public string receivedString;
      public int    receivedInt;
      public bool   receivedBool;

      public TestReceivedClass(string receivedString, int receivedInt, bool receivedBool) {

         this.receivedString = receivedString;
         this.receivedInt    = receivedInt;
         this.receivedBool   = receivedBool;
      }
   }
}