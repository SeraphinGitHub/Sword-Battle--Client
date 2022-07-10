// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;


// public class QuarterTop : MonoBehaviour {
    
//     // =================================================
// 	// Quarter Top
// 	// =================================================

//     [SerializeField] private GameObject Player = default;
    
//     // Start
//     void Start() {
//         GetComponent<Image>().enabled = false;
//     }

//     void OnTriggerEnter2D(Collider2D col){

//         if(col.tag == "StickTag") {

//             GetComponent<Image>().enabled = true;
//         }
//     }

//     void OnTriggerExit2D(Collider2D col){

//         if(col.tag == "StickTag") {

//             GetComponent<Image>().enabled = false;
//         }
//     }
// }
