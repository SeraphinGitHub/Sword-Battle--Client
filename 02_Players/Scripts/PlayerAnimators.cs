using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimators : MonoBehaviour {
   
   [Header("Left Animators")]
   public Animator left_AnimArm;
   public Animator left_AnimSword;
   
   [Header("Right Animators")]
   public Animator right_AnimArm;
   public Animator right_AnimSword;


   // Private Variables
   private string playerSide;
   private string enemySide;
   private GameObject localPlayer;
   private GameObject gameHandler;


   // ====================================================================================
   // Start
   // ====================================================================================
   private void Start() {
      gameHandler = GameObject.Find("_GameHandler");
		playerSide = gameHandler.GetComponent<GameHandler>().playerSide;
		enemySide = gameHandler.GetComponent<GameHandler>().enemySide;
		localPlayer = gameHandler.GetComponent<PlayerRandomize>().localPlayer;
   }


   // ====================================================================================
   // Public Methods
   // ====================================================================================
   public void SetSwordColor(string charaterSide, string swordColor) {

      if(charaterSide == "Left") left_AnimSword.SetTrigger(swordColor);
      if(charaterSide == "Right") right_AnimSword.SetTrigger(swordColor);
   }

   public void SetAnim(string animType, string leftAnim, string rightAnim) {

      if(transform == localPlayer.transform) {

         // ******************
         if(playerSide == "Left") {
            if(animType == "attack") left_AnimArm.SetTrigger(leftAnim);
         }

         if(playerSide == "Right") {
            if(animType == "attack") right_AnimArm.SetTrigger(rightAnim);
         }
         // ******************
      }

      else {
         // ******************
         if(enemySide == "Left") {
            if(animType == "attack") left_AnimArm.SetTrigger(leftAnim);
         }

         if(enemySide == "Right") {
            if(animType == "attack") right_AnimArm.SetTrigger(rightAnim);
         }
         // ******************
      }
   }

   // public void AnimSetTrigger(
   //    string playerSide,
   //    string leftAnimName,
   //    Animator leftAnimator,
   //    string rightAnimName,
   //    Animator rightAnimator) {

   //    if(playerSide == "Left") leftAnimator.SetTrigger(leftAnimName);
   //    if(playerSide == "Right") rightAnimator.SetTrigger(rightAnimName);
   // }

   // public void AnimSetBool(
   //    bool animState,
   //    string leftAnimName,
   //    Animator leftAnimator,
   //    string rightAnimName,
   //    Animator rightAnimator) {

   //    if(playerSide == "Left") leftAnimator.SetBool(leftAnimName, animState);
   //    if(playerSide == "Right") rightAnimator.SetBool(rightAnimName, animState);
   // }
}
