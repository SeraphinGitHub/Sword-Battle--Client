// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PlayerTeam : MonoBehaviour {

//     // Hair Size GameObjects
//     [SerializeField] private GameObject longHairFace = default;
//     [SerializeField] private GameObject shortHairFace = default;
    
//     // Colors
//     [SerializeField] private Color blueHair = default;
//     [SerializeField] private Color greenHair = default;
//     [SerializeField] private Color orangeTabard = default;
//     [SerializeField] private Color violetTabard = default;

//     // Sword Animator
//     [SerializeField] private Animator swordAnim = default;


// 	private int randTeam;
//     private bool leaveGame;

	
//     // Start
//     void Start() {

//         randTeam = GameObject.Find("_NetworkManager_").GetComponent<NetworkSwordBattle>().randTeam;
//         leaveGame = GameObject.Find("_NetworkManager_").GetComponent<NetworkSwordBattle>().leaveGame;

//         if(leaveGame == false) {
            
//             // Set Blue Hair Player
//             if(randTeam == 0 || randTeam == 2) {

//                 if(longHairFace.activeSelf == false) {
//                     longHairFace.SetActive(true);
//                 }

//                 if(shortHairFace.activeSelf == true) {
//                     shortHairFace.SetActive(false);
//                 }
         
//                 GameObject.FindWithTag("BlueHairTag").GetComponent<SpriteRenderer>().color = blueHair;
//                 GameObject.FindWithTag("TabardTag").GetComponent<SpriteRenderer>().color = orangeTabard;
//             }

//             if(randTeam == 0 ) {
//                 swordAnim.SetBool("isSword_Violet_LEFT", true);
//             }

//             if(randTeam == 2 ) {
//                 swordAnim.SetBool("isSword_Violet_RIGHT", true);
//             }

                    
//             // Set Green Hair Player
//             if(randTeam == 1 || randTeam == 3) {

//                 if(longHairFace.activeSelf == true) {
//                     longHairFace.SetActive(false);
//                 }

//                 if(shortHairFace.activeSelf == false) {
//                     shortHairFace.SetActive(true);
//                 }

//                 GameObject.FindWithTag("GreenHairTag").GetComponent<SpriteRenderer>().color = greenHair;
//                 GameObject.FindWithTag("TabardTag").GetComponent<SpriteRenderer>().color = violetTabard;
//             }

//             if(randTeam == 1 ) {
//                 swordAnim.SetBool("isSword_Green_LEFT", true);
//             }

//             if(randTeam == 3 ) {
//                 swordAnim.SetBool("isSword_Green_RIGHT", true);
//             }
//         }
//     }
// }
