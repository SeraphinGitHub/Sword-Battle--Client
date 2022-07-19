using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRandomize : MonoBehaviour {

   public int randomizeDelay = 3;
   public float posX = 6.5f;
   public float posY = 0.3f;

   [Header("Attached GameObjects")]
   public GameObject playerUI;

   [Header("Player GameObjects")]
   public GameObject[] playerSpritesArray = new GameObject[2];
   public GameObject[] leftPlayerHeadArray = new GameObject[2];
   public GameObject[] leftPlayerHairArray = new GameObject[2];
   public GameObject[] rightPlayerHeadArray = new GameObject[2];
   public GameObject[] rightPlayerHairArray = new GameObject[2];
   public GameObject[] playerTabardArray = new GameObject[2];

   [Header("Animators")]
   public Animator leftSwordAnim;
   public Animator rightSwordAnim;
   
   [Header("Hair Colors")]
   public Color[] hairColorsArray = new Color[6];

   [Header("Body Colors")]
   public Color[] bodyColorsArray = new Color[6];


   // Private Variables
   private string[] playerSideArray = new string[] {
      "Left",
      "Right",
   };
   private string[] playerHairArray = new string[] {
      "Short",
      "Long",
   };
   private string[] playerColorArray = new string[] {
      "Blue",
      "Green",
      "Yellow",
      "orange",
      "Red",
      "Violet",
   };
   [HideInInspector] public string playerSide;
   private string playerHead;
   private string playerHairColor;
   private string playerTabardColor;
   private int sidePos;


   // ====================================================================================
   // Methods
   // ====================================================================================
   private void Awake() {
      UnsetPlayer();
   }

   public void RunRandomize() {
      int randomSide = Random.Range(0, playerSideArray.Length);
      int randomHead = Random.Range(0, playerHairArray.Length);
      int randomHairColor = Random.Range(0, playerColorArray.Length);
      int randomBodyColor = Random.Range(0, playerColorArray.Length);

      playerSide = playerSideArray[randomSide];
      playerHead = playerHairArray[randomHead];
      playerHairColor = playerColorArray[randomHairColor];
      playerTabardColor = playerColorArray[randomBodyColor];

      StartCoroutine(InstantiatePlayer());
   }

   IEnumerator InstantiatePlayer() {
      yield return new WaitForSeconds(randomizeDelay);
      SetPlayer();
   }

   private void SetPlayer() {

      // Set Player Side
      for(int i = 0; i < playerSideArray.Length; i++) {
         if(playerSide == playerSideArray[i]) {
            playerSpritesArray[i].SetActive(true);
            sidePos = 2 *i -1;

            // Player Hair
            for(int j = 0; j < playerHairArray.Length; j++) {
               if(playerHead == playerHairArray[j]) {
                  leftPlayerHeadArray[j].SetActive(true);
                  rightPlayerHeadArray[j].SetActive(true);

                  // Player Color
                  for(int k = 0; k < playerColorArray.Length; k++) {
                     if(playerHairColor == playerColorArray[k]) {
                        leftPlayerHairArray[j].GetComponent<SpriteRenderer>().color = hairColorsArray[k];
                        rightPlayerHairArray[j].GetComponent<SpriteRenderer>().color = hairColorsArray[k];
                     }

                     if(playerTabardColor == playerColorArray[k]) {
                        playerTabardArray[j].GetComponent<SpriteRenderer>().color = bodyColorsArray[k];
                     }
                  }
               }
            }        
         }
      }

      transform.position = new Vector3(posX *sidePos, posY, 0);
   }

   public void UnsetPlayer() {
      playerUI.SetActive(false);

      // Hide Player Sprites
      for(int i = 0; i < playerSideArray.Length; i++) {
         playerSpritesArray[i].SetActive(false);

         // Hide Player Head
         for(int j = 0; j < playerHairArray.Length; j++) {
            leftPlayerHeadArray[j].SetActive(false);
            rightPlayerHeadArray[j].SetActive(false);
         }        
      }
   }
}
