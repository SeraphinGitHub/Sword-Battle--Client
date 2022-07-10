// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;


// public class QuarterRight : MonoBehaviour {
    
//     // =================================================
// 	// Quarter Right
// 	// =================================================

//     [SerializeField] private GameObject Player = default;
    
//     // Start
//     void Start() {
//         GetComponent<Image>().enabled = false;
//     }

//     void OnTriggerEnter2D(Collider2D col){

//         if(col.tag == "StickTag") {

//             GetComponent<Image>().enabled = true;
//             Player.GetComponent<PlayerMovements>().RightON();
//         }
//     }

//     void OnTriggerExit2D(Collider2D col){

//         if(col.tag == "StickTag") {
            
//             GetComponent<Image>().enabled = false;
//             Player.GetComponent<PlayerMovements>().RightOFF();
//         }
//     }
// }
