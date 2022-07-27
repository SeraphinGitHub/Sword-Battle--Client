using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour {
   
   [Header("Player Animators")]
   public Animator[] armAnimators = new Animator[2];
   public Animator[] swordAnimators = new Animator[2];
   public Animator[] shieldAnimators = new Animator[2];
   public Animator[] bodyAnimators = new Animator[2];


   // Public Hidden Variables
   [HideInInspector] public string characterSide;
   [HideInInspector] public string attackType;
   [HideInInspector] public bool isAttacking;


   // Private Variables
   private string[] sidesArray = new string[] {
      "Left",
      "Right",
   };


   // ====================================================================================
   // Public Methods
   // ====================================================================================   
   public void SetAnim(string behavior, string animName) {

      // Set anim side: Left / Right
      for(int i = 0; i < sidesArray.Length; i++) {
         string side = sidesArray[i];

         if(characterSide == side) {
            // Has to match the name of trigger in unity controller
            string orientedAnim = side+animName;

            if(behavior == "SwordColor") swordAnimators[i].SetTrigger(animName);
            if(behavior == "Attack") armAnimators[i].SetTrigger(orientedAnim);
         }
      }
   }
}
