using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRandomize : MonoBehaviour {

   public int randomizeDelay = 3;
   public float posX = 6.5f;
   public float posY = 0.3f;

   [Header("Attached Prefabs")]
   public GameObject playerPrefab;
   
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
   [HideInInspector] public bool isBattleOnGoing = false;
   [HideInInspector] public List<string> playerPropsList = new List<string>();

   private string playerHead;
   private string playerHairColor;
   private string playerTabardColor;
   private int sidePos;

   private Animator leftSwordAnim;
   private Animator rightSwordAnim;

   private GameObject[] playerSpritesArray;
   private GameObject[] leftPlayerHeadArray;
   private GameObject[] leftPlayerHairArray;
   private GameObject[] rightPlayerHeadArray;
   private GameObject[] rightPlayerHairArray;
   private GameObject[] playerTabardArray;

   private GameObject newPlayer;
   private GameObject playerUI;
   private GameObject getGameObj(string name) { return newPlayer.transform.Find(name).gameObject; }


   // ====================================================================================
   // Methods
   // ====================================================================================
   private void SetSpritesArrays() {

      // Player Side
      playerSpritesArray = new GameObject[] {
         getGameObj("Sprites_LeftPlayer"),
         getGameObj("Sprites_RightPlayer"),
      };

      // Left Player Head
      leftPlayerHeadArray = new GameObject[] {
         getGameObj("Sprites_LeftPlayer/Head/Face_Short_Hair"),
         getGameObj("Sprites_LeftPlayer/Head/Face_Long_Hair"),
      };
      
      // Left Player Hair
      leftPlayerHairArray = new GameObject[] {
         getGameObj("Sprites_LeftPlayer/Head/Face_Short_Hair/Short_Hair"),
         getGameObj("Sprites_LeftPlayer/Head/Face_Long_Hair/Long_Hair"),
      };
      
      // Right Player Head
      rightPlayerHeadArray = new GameObject[] {
         getGameObj("Sprites_RightPlayer/Head/Face_Short_Hair"),
         getGameObj("Sprites_RightPlayer/Head/Face_Long_Hair"),
      };
      
      // Right Player Head
      rightPlayerHairArray = new GameObject[] {
         getGameObj("Sprites_RightPlayer/Head/Face_Short_Hair/Short_Hair"),
         getGameObj("Sprites_RightPlayer/Head/Face_Long_Hair/Long_Hair"),
      };

      // Player Tabard
      playerTabardArray = new GameObject[] {
         getGameObj("Sprites_LeftPlayer/Body/Tabard"),
         getGameObj("Sprites_RightPlayer/Body/Tabard"),
      };
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

      playerPropsList.Add(playerSide);
      playerPropsList.Add(playerHead);
      playerPropsList.Add(playerHairColor);
      playerPropsList.Add(playerTabardColor);
   }

   public void InstantiatePlayer () {
      StartCoroutine(DelaySetPlayer());
   }

   IEnumerator DelaySetPlayer() {
      yield return new WaitForSeconds(randomizeDelay);
      
      if(isBattleOnGoing) {
         newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

         SetSpritesArrays();
         UnsetPlayer();
         SetPlayer(playerPropsList);
      }
      else yield break;
   }

   public void SetPlayer(List<string> propsList) {

      // Set player side
      for(int i_Side = 0; i_Side < playerSideArray.Length; i_Side++) {
         if(propsList[0] == playerSideArray[i_Side]) {

            playerSpritesArray[i_Side].SetActive(true);
            sidePos = 2 *i_Side -1;


            // Set player hair
            for(int i_Hair = 0; i_Hair < playerHairArray.Length; i_Hair++) {
               if(propsList[1] == playerHairArray[i_Hair]) {

                  leftPlayerHeadArray[i_Hair].SetActive(true);
                  rightPlayerHeadArray[i_Hair].SetActive(true);
                  

                  // Set player hair & body color
                  for(int i_Color = 0; i_Color < playerColorArray.Length; i_Color++) {

                     // Hair color
                     if(propsList[2] == playerColorArray[i_Color]) {
                        leftPlayerHairArray[i_Hair].GetComponent<SpriteRenderer>().color = hairColorsArray[i_Color];
                        rightPlayerHairArray[i_Hair].GetComponent<SpriteRenderer>().color = hairColorsArray[i_Color];
                     }

                     // Tabard color 
                     if(propsList[3] == playerColorArray[i_Color]) {
                        playerTabardArray[i_Side].GetComponent<SpriteRenderer>().color = bodyColorsArray[i_Color];
                     }
                  }
               }
            }        
         }
      }

      newPlayer.transform.position = new Vector3(posX *sidePos, posY, 0);
      getGameObj("Player_UI").SetActive(true);
   }

   public void UnsetPlayer() {

      // Hide Player Sprites
      for(int i_Side = 0; i_Side < playerSideArray.Length; i_Side++) {
         playerSpritesArray[i_Side].SetActive(false);

         // Hide Player Head
         for(int i_Hair = 0; i_Hair < playerHairArray.Length; i_Hair++) {
            leftPlayerHeadArray[i_Hair].SetActive(false);
            rightPlayerHeadArray[i_Hair].SetActive(false);
         }        
      }
   }

   public void UnsetBattleUI() {
      if(newPlayer) Destroy(newPlayer);
      playerPropsList.Clear();
   }
}
