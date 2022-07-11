using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BordersMax : MonoBehaviour {

   private bool isLocalPlayer = true;

   void OnTriggerEnter2D(Collider2D collision) {

      if(isLocalPlayer && collision.gameObject.tag == "Player") {

         if(transform.position.x < 0) {
            collision.gameObject.GetComponent<PlayerMovements>().isMovingLeft = false;
         }

         if(transform.position.x > 0) {
            collision.gameObject.GetComponent<PlayerMovements>().isMovingRight = false;
         }
      }
   }
}
