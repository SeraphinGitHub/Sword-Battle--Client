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


   // ===========================================================================================================
   // Awake() / Start()
   // ===========================================================================================================
   private void Awake() {

      string selectedName = PlayerPrefs.GetString(categoryName, defaultSelected.name);
   
      for(int i = 0; i < selectArray.Length; i++) {
         GameObject elem = selectArray[i];

         elem.GetComponent<Animator>().enabled = false;

         if(elem.name == selectedName) continue;
         elem.GetComponent<RectTransform>().localScale = Vector3.zero;
      }
   }

   private void Start() {

      for(int i = 0; i < selectArray.Length; i++) {
         selectArray[i].GetComponent<Animator>().enabled = true;
      }
   }


   // ===========================================================================================================
   // Public Methods
   // ===========================================================================================================
   public void ToggleSelection(GameObject selection) {

      Vector3 selectionScale = selection.GetComponent<RectTransform>().localScale;

      if(selectionScale != Vector3.zero) return;

      selection.GetComponent<Animator>().SetTrigger("ShowSelect");

      // Save Option
      PlayerPrefs.SetString(categoryName, selection.name);
      PlayerPrefs.Save();

      for(int i = 0; i < selectArray.Length; i++) {
         GameObject elem = selectArray[i];

         if(elem == selection) continue;
         if(elem.GetComponent<RectTransform>().localScale == Vector3.zero) continue;
         
         elem.GetComponent<Animator>().SetTrigger("HideSelect");
      }
   }

}
