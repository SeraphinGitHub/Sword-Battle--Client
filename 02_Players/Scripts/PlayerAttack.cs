using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

   private Animator anim;
   private bool isMovingRight;
   private bool isMovingLeft;

   private int randomNumber;
   public bool isLocalPlayer = true;
   public bool isAttacking = false;
   public string attackType = "";

   // Start is called before the first frame update
   void Start() {
      anim = GetComponent<Animator>();
   }
	
	public void SwordStrikeON() {
		if(isLocalPlayer) {
         // isMovingRight = GetComponent<PlayerMovements>().isMovingRight;
         // isMovingLeft = GetComponent<PlayerMovements>().isMovingLeft;
         
         // randomNumber = Random.Range(0,3);

         // if(isMovingRight == true && isMovingLeft == false && randomNumber == 2) {
         //    anim.SetTrigger("attackRight_Estoc");
         // }

         // if(isMovingLeft == true && isMovingRight == false && randomNumber == 2) {
         //    anim.SetTrigger("attackLeft_Estoc");
         // }

         // if(isMovingRight == true && isMovingLeft == false && randomNumber == 0 || isMovingRight == true && isMovingLeft == false && randomNumber == 1) {
         //    anim.SetTrigger("attackRight");
         // }

         // if(isMovingLeft == true && isMovingRight == false && randomNumber == 0 || isMovingLeft == true && isMovingRight == false && randomNumber == 1) {
            
            if(!isAttacking) {
               isAttacking = true;
               attackType = "strike";

               anim.SetTrigger("attackStrikeLeft");
               StartCoroutine(AttackTimeOut());
            }
            
         // }
      }
	}

   public void SwordEstocON() {
		if(isLocalPlayer) {

         if(!isAttacking) {
            isAttacking = true;
            attackType = "estoc";

            anim.SetTrigger("attackEstocLeft");
            StartCoroutine(AttackTimeOut());
         }
      }
	}

   IEnumerator AttackTimeOut() {
      yield return new WaitForSeconds(0.35f);
      isAttacking = false;
      attackType = "";
      anim.SetTrigger("idleLeft");
   }
}
