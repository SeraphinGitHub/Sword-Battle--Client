using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSelect: MonoBehaviour {
   
   // Public Variables
   [Header("**Attached Select Name**")]
   public string     categoryName;
   public GameObject defaultSelected;

   [Header("**Attached Square Array**")]
   public GameObject[] selectArray = new GameObject[2];

   // Private Variables
   private float animDelay = 0.2f;


   // ===========================================================================================================
   // Awake() / Start()
   // ===========================================================================================================
   private void Awake() {

      string selectedName = PlayerPrefs.GetString(categoryName, defaultSelected.name);
   
      for(int i = 0; i < selectArray.Length; i++) {
         GameObject elem = selectArray[i];

         if(elem.name == selectedName) {
            elem.GetComponent<ElemSelected>().isElemSelected = true;
         }
         
         else {
            elem.GetComponent<RectTransform>().localScale = Vector3.zero;
         }
      }
   }


   // ===========================================================================================================
   // Public Methods
   // ===========================================================================================================
   public void ToggleSelection(GameObject selection) {

      bool isSelected = selection.GetComponent<ElemSelected>().isElemSelected;

      if(isSelected) return;
      StartCoroutine( TriggerSelection(selection) );

      // Save Option
      PlayerPrefs.SetString(categoryName, selection.name);
      PlayerPrefs.Save();
   }


   // ===========================================================================================================
   // Coroutines
   // ===========================================================================================================
   IEnumerator TriggerSelection(GameObject selection) {
      
      for(int i = 0; i < selectArray.Length; i++) {
         GameObject elem = selectArray[i];

         if(elem.GetComponent<ElemSelected>().isElemSelected) {
            elem.GetComponent<Animator>().SetTrigger("HideSelect");
         }
         
         else elem.GetComponent<ElemSelected>().isElemSelected = true;
      }

      selection.GetComponent<Animator>().SetTrigger("ShowSelect");

      yield return new WaitForSeconds(animDelay);

      for(int i = 0; i < selectArray.Length; i++) {
         GameObject elem = selectArray[i];

         if(elem.name != selection.name) {
            elem.GetComponent<ElemSelected>().isElemSelected = false;
         }
      }
   }

}
