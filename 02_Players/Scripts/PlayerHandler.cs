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
   [HideInInspector] public string state;
   [HideInInspector] public bool isAttacking;
   [HideInInspector] public bool isProtecting;


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

            switch(behavior) {
               case "SwordColor":
                  swordAnimators[i].SetTrigger(animName);
               break;
               
               case "Attack":
                  state = animName;
                  armAnimators[i].SetTrigger(orientedAnim);
               break;

               case "Protect":
                  state = behavior;
                  shieldAnimators[i].SetTrigger(orientedAnim);
               break;

               case "Walk":
                  state = orientedAnim;
                  if(!isAttacking) armAnimators[i].SetTrigger(orientedAnim);
                  if(!isProtecting) shieldAnimators[i].SetTrigger(orientedAnim);
                  bodyAnimators[i].SetTrigger(orientedAnim);
               break;

               // Default Idle
               default:
                  state = behavior;
                  if(!isAttacking) armAnimators[i].SetTrigger(orientedAnim);
                  if(!isProtecting) shieldAnimators[i].SetTrigger(orientedAnim);
                  bodyAnimators[i].SetTrigger(orientedAnim);
               break;
            }
         }
      }
   }
}
