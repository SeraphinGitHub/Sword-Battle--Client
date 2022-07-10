// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class DestroyPlayer : MonoBehaviour {

//     private GameObject PlayerLeft;
//     private GameObject PlayerRight;

//     private void Start() {
//         if(isLocalPlayer) {
//             PlayerLeft = GameObject.Find("Left_Player");
//             PlayerRight = GameObject.Find("Right_Player");
//         }
//     }

//     public void Destroy_Player() {
//         if(isLocalPlayer && PlayerLeft != null) {
//             Destroy(PlayerLeft);
//         }

//         if(isLocalPlayer && PlayerRight != null) {
//             Destroy(PlayerRight);
//         }
//     }
// }
