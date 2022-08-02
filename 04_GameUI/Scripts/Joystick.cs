// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;


// public class Joystick : MonoBehaviour {
//     [SerializeField] private GameObject Player = default;
//     [SerializeField] private float StickRange = 127.5f;
//     [SerializeField] private GameObject Stick = default;
//     [SerializeField] private GameObject GreyFilter = default;


//     private bool isStickPressed = false;


//     // Update
//     private void Update() {
//         MovementsRange();
//     }


//     // Movements Range
//     private void MovementsRange() {
//         Vector3 offset = Input.mousePosition - transform.position;

// 		if(isStickPressed == true) {
//             Stick.transform.position = transform.position + Vector3.ClampMagnitude(offset, StickRange);
//         }

//         if(isStickPressed == false) {
//             Stick.transform.position = transform.position;
//         }
//     }


//     // On Stick Down
//     public void OnStick_DOWN() {
//         isStickPressed = true;
//     }


//     // On Stick Up
//     public void OnStick_UP() {
//         isStickPressed = false;
//     }


//     private void OnDrawGizmosSelected() {
// 		Gizmos.color = Color.red;
// 		Gizmos.DrawWireSphere(transform.position, StickRange);
// 	}
// }
