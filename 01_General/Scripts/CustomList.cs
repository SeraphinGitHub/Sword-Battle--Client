using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CustomList: MonoBehaviour {

   // Public Variables
   public Animator listAnim;
   public GameObject defaultOption;

   [Header("**Attached Option Array**")]
   public GameObject[] optionArray        = new GameObject[4];

   [Header("**Attached Shield Sprite Array**")]
   public Image[] shieldBaseSpriteArray   = new Image[6];
   public Image[] shieldOptionArray       = new Image[6];

   [Header("**Attached Case Array**")]
   public GameObject[] goldCaseArray      = new GameObject[3];
   public GameObject[] silverCaseArray    = new GameObject[3];

   [Header("**Attached Corner Array**")]
   public GameObject[] gold1CornerArray   = new GameObject[3];
   public GameObject[] gold2CornerArray   = new GameObject[3];
   public GameObject[] silver1CornerArray = new GameObject[3];
   public GameObject[] silver2CornerArray = new GameObject[3];

   [Header("**Attached Health Array**")]
   public GameObject[] goldHealthArray    = new GameObject[3];
   public GameObject[] silverHealthArray  = new GameObject[3];

   [Header("**Attached Shield Frame Array**")]
   public GameObject[] goldShieldArray    = new GameObject[18];
   public GameObject[] silverShieldArray  = new GameObject[18];


   // Private Variables 
   private float  animDelay      = 0.35f;
   private string optionCategory = "SelectedOption";
   private string categoryName;
   private string[] categoryNameArray = new string[] {
      "CaseFrame",
      "CornerFrame",
      "HealthFrame",
      "ShieldFrame",
      "ShieldColor",
   };

   private GameObject[][] caseFrameArray;
   private GameObject[][] cornerFrameArray;
   private GameObject[][] healthFrameArray;
   private GameObject[][] shieldFrameArray;
   private Image[][]      shieldColorArray;

   private GameHandler gameHandler;


   // ===========================================================================================================
   // Awake() / Start()
   // ===========================================================================================================
   private void Awake() {

      categoryName = PlayerPrefs.GetString(optionCategory, defaultOption.name);

      caseFrameArray   = new GameObject[][] {
         goldCaseArray,
         silverCaseArray,
      };

      cornerFrameArray = new GameObject[][] {
         gold1CornerArray,
         gold2CornerArray,
         silver1CornerArray,
         silver2CornerArray,
      };

      healthFrameArray = new GameObject[][] {
         goldHealthArray,
         silverHealthArray,
      };

      shieldFrameArray = new GameObject[][] {
         goldShieldArray,
         silverShieldArray,
      };

   }
   
   private void Start() {

      StartCoroutine( TriggerOption(defaultOption) );

      gameHandler = GetComponent<GameHandler>();
      Image[][] tempShieldArray = gameHandler.shieldLifeSpritesArray;

      shieldColorArray = new Image[][] {
         tempShieldArray[0],
         tempShieldArray[1],
         shieldOptionArray
      };

      SetSavedCustom();
   }


   // ===========================================================================================================
   // Public Methods
   // ===========================================================================================================
   public void ToggleOption(GameObject option) {

      bool isSelected = option.GetComponent<ElemSelected>().isElemSelected;

      if(isSelected) return;
      StartCoroutine( TriggerOption(option) );

      // Save Option
      PlayerPrefs.SetString(optionCategory, option.name);
      PlayerPrefs.Save();
   }

   public void UpdateCategory(GameObject category) {

      categoryName = category.name;
      gameHandler.VibrateButton();
   }

   public void UpdateUI(GameObject elem) {

      switch(categoryName) {
         case "CaseOption":
            SetCaseFrame(elem.name);
         break;

         case "CornerOption":
            SetCornerFrame(elem.name);
         break;

         case "HealthOption":
            SetHealthFrame(elem.name);
         break;

         case "ShieldOption":
            SetShieldFrame(elem.name);
            SetShieldColor(elem.name);
         break;
      }

      gameHandler.VibrateButton();
   }

   public void ResetCustom() {
      
      for(int i = 0; i < categoryNameArray.Length; i++) {
         PlayerPrefs.DeleteKey(categoryNameArray[i]);
      }

      PlayerPrefs.Save();
      SetSavedCustom();
   }


   // ===========================================================================================================
   // Private Methods
   // ===========================================================================================================
   private void CycleObjectArray(GameObject[] elemArray, bool state) {

      for(int i = 0; i < elemArray.Length; i++) {
         elemArray[i].SetActive(state);
      }
   }

   private void CycleImageArray(Image[][] spriteArray, int indexRef) {

      Image spriteRef = shieldBaseSpriteArray[indexRef];

      for(int i = 0; i < spriteArray.Length; i++) {
         for(int k = 0; k < spriteArray[i].Length; k++) {
            
            Image spriteToSet  = spriteArray[i][k];
            spriteToSet.sprite = spriteRef.sprite;
            spriteToSet.color  = spriteRef.color;
         }
      }
   }

   private void SetSavedCustom() {

      SetCaseFrame  ( PlayerPrefs.GetString(categoryNameArray[0], "SelectGold"  ) );
      SetCornerFrame( PlayerPrefs.GetString(categoryNameArray[1], "SelectGold1" ) );
      SetHealthFrame( PlayerPrefs.GetString(categoryNameArray[2], "SelectSilver") );
      SetShieldFrame( PlayerPrefs.GetString(categoryNameArray[3], "SelectSilver") );
      SetShieldColor( PlayerPrefs.GetString(categoryNameArray[4], "SelectYellow") );
   }

   // Switch statements
   private void SetCaseFrame(string caseFrame) {

      switch(caseFrame) {
         case "SelectGold": 
            CycleObjectArray(caseFrameArray[0], true);
            CycleObjectArray(caseFrameArray[1], false);
         break;

         case "SelectSilver": 
            CycleObjectArray(caseFrameArray[0], false);
            CycleObjectArray(caseFrameArray[1], true);
         break;
      }
   }

   private void SetCornerFrame(string cornerFrame) {

      switch(cornerFrame) {
         case "SelectGold1": 
            CycleObjectArray(cornerFrameArray[0], true);
            CycleObjectArray(cornerFrameArray[1], false);
            CycleObjectArray(cornerFrameArray[2], false);
            CycleObjectArray(cornerFrameArray[3], false);
         break;

         case "SelectGold2": 
            CycleObjectArray(cornerFrameArray[0], false);
            CycleObjectArray(cornerFrameArray[1], true);
            CycleObjectArray(cornerFrameArray[2], false);
            CycleObjectArray(cornerFrameArray[3], false);
         break;

         case "SelectSilver1": 
            CycleObjectArray(cornerFrameArray[0], false);
            CycleObjectArray(cornerFrameArray[1], false);
            CycleObjectArray(cornerFrameArray[2], true);
            CycleObjectArray(cornerFrameArray[3], false);
         break;

         case "SelectSilver2": 
            CycleObjectArray(cornerFrameArray[0], false);
            CycleObjectArray(cornerFrameArray[1], false);
            CycleObjectArray(cornerFrameArray[2], false);
            CycleObjectArray(cornerFrameArray[3], true);
         break;
      }
   }

   private void SetHealthFrame(string healthFrame) {

      switch(healthFrame) {
         case "SelectGold": 
            CycleObjectArray(healthFrameArray[0], true);
            CycleObjectArray(healthFrameArray[1], false);
         break;

         case "SelectSilver": 
            CycleObjectArray(healthFrameArray[0], false);
            CycleObjectArray(healthFrameArray[1], true);
         break;
      }
   }

   private void SetShieldFrame(string shieldFrame) {

      switch(shieldFrame) {
         case "SelectGold": 
            CycleObjectArray(shieldFrameArray[0], true);
            CycleObjectArray(shieldFrameArray[1], false);
         break;

         case "SelectSilver": 
            CycleObjectArray(shieldFrameArray[0], false);
            CycleObjectArray(shieldFrameArray[1], true);
         break;
      }
   }

   private void SetShieldColor(string shieldColor) {

      switch(shieldColor) {
         case "SelectOrange":
            CycleImageArray(shieldColorArray, 0);
         break;

         case "SelectRed":
            CycleImageArray(shieldColorArray, 1);
         break;

         case "SelectViolet":
            CycleImageArray(shieldColorArray, 2);
         break;

         case "SelectBlue":
            CycleImageArray(shieldColorArray, 3);
         break;

         case "SelectGreen":
            CycleImageArray(shieldColorArray, 4);
         break;

         case "SelectYellow":
            CycleImageArray(shieldColorArray, 5);
         break;
      }
   }


   // ===========================================================================================================
   // Coroutines
   // ===========================================================================================================
   IEnumerator TriggerOption(GameObject option) {

      for(int i = 0; i < optionArray.Length; i++) {
         GameObject elem = optionArray[i];
         elem.GetComponent<ElemSelected>().isElemSelected = true;
      }
      
      listAnim.SetTrigger("HideOptions");
      yield return new WaitForSeconds(animDelay);

      // Hide Old Option && Show current Option
      for(int i = 0; i < optionArray.Length; i++) {
         GameObject elem = optionArray[i];
         elem.SetActive(false);
         if(elem.name == option.name) elem.SetActive(true);
      }

      listAnim.SetTrigger("ShowOptions");
      yield return new WaitForSeconds(animDelay);
      
      for(int i = 0; i < optionArray.Length; i++) {
         GameObject elem = optionArray[i];
         if(elem.name != option.name) elem.GetComponent<ElemSelected>().isElemSelected = false;
      }
   }
}
