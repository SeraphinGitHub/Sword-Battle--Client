using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRandomize : MonoBehaviour {

   public float posX = 6.5f;
   public float posY = 0.3f;

   [Header("Attached Prefabs")]
   public GameObject playerPrefab;
   
   [Header("Hair Colors")]
   public Color[] hairColorsArray = {
      new Color32(0, 200, 225, 255),
      new Color32(30, 220, 0, 255),
      new Color32(255, 241, 54, 255),
      new Color32(255, 0, 0, 255),
      new Color32(170, 0, 255, 255),
   };

   [Header("Tabard Colors")]
   public Color[] tabardColorsArray = {
      new Color32(0, 120, 255, 255),
      new Color32(23, 171, 0, 255),
      new Color32(255, 120, 0, 255),
      new Color32(220, 0, 0, 255),
      new Color32(135, 0, 202, 255),
   };


   // Public Hidden Variables
   [HideInInspector] public string playerSide;
   [HideInInspector] public List<string> playerPropsList;

   [HideInInspector] public GameObject hostPlayer;
   [HideInInspector] public GameObject joinPlayer;
   [HideInInspector] public GameObject localPlayer;


   // Private Variables
   private string playerHairStyle;
   private string playerHairColor;
   private string playerTabardColor;

   private string[] sideArray = new string[] {
      "Left",
      "Right",
   };
   private string[] hairStyleArray = new string[] {
      "Short",
      "Long",
   };
   private string[] colorArray = new string[] {
      "Blue",
      "Green",
      "Yellow",
      "Red",
      "Violet",
   };
   
   private List<string> sideList = new List<string>();
   private List<string> hairStyleList = new List<string>();
   private List<string> hairColorList = new List<string>();
   private List<string> tabardColorList = new List<string>();

   private int sidePos;
   
   // **********************************
   private Animator leftSwordAnim;
   private Animator rightSwordAnim;
   // **********************************

   private GameObject[] spritesArray;
   private GameObject[] leftHeadArray;
   private GameObject[] leftHairStyleArray;
   private GameObject[] rightHeadArray;
   private GameObject[] rightHairStyleArray;
   private GameObject[] tabardArray;

   
   // ====================================================================================
   // Methods
   // ====================================================================================
   private GameObject getGameObj(GameObject player, string name) {
      return player.transform.Find(name).gameObject;
   }

   private void SetSpritesArrays(GameObject player) {

      // Player Side
      spritesArray = new GameObject[] {
         getGameObj(player, "Sprites_LeftPlayer"),
         getGameObj(player, "Sprites_RightPlayer"),
      };

      // Left Player Head
      leftHeadArray = new GameObject[] {
         getGameObj(player, "Sprites_LeftPlayer/Head/Face_Short_Hair"),
         getGameObj(player, "Sprites_LeftPlayer/Head/Face_Long_Hair"),
      };
      
      // Left Player Hair
      leftHairStyleArray = new GameObject[] {
         getGameObj(player, "Sprites_LeftPlayer/Head/Face_Short_Hair/Short_Hair"),
         getGameObj(player, "Sprites_LeftPlayer/Head/Face_Long_Hair/Long_Hair"),
      };
      
      // Right Player Head
      rightHeadArray = new GameObject[] {
         getGameObj(player, "Sprites_RightPlayer/Head/Face_Short_Hair"),
         getGameObj(player, "Sprites_RightPlayer/Head/Face_Long_Hair"),
      };
      
      // Right Player Head
      rightHairStyleArray = new GameObject[] {
         getGameObj(player, "Sprites_RightPlayer/Head/Face_Short_Hair/Short_Hair"),
         getGameObj(player, "Sprites_RightPlayer/Head/Face_Long_Hair/Long_Hair"),
      };

      // Player Tabard
      tabardArray = new GameObject[] {
         getGameObj(player, "Sprites_LeftPlayer/Body/Tabard"),
         getGameObj(player, "Sprites_RightPlayer/Body/Tabard"),
      };
   }

   public void RemoveRandomizeProps(List<string> propsList) {

      // Remove hostPlayer props to avoid same props for joinPlayer
      foreach(string side in sideArray) {
         if(side != propsList[0]) sideList.Add(side);
      }

      foreach(string hair in hairStyleArray) {
         if(hair != propsList[1]) hairStyleList.Add(hair);
      }

      foreach(string color in colorArray) {
         if(color != propsList[2]) hairColorList.Add(color);
         if(color != propsList[3]) tabardColorList.Add(color);
      }
   }

   private void RandomizeHostPlayer() {

      // Randomize from Arrays
      int randomSide = Random.Range(0, sideArray.Length);
      int randomHead = Random.Range(0, hairStyleArray.Length);
      int randomHairColor = Random.Range(0, colorArray.Length);
      int randomTabardColor = Random.Range(0, colorArray.Length);

      playerSide = sideArray[randomSide];
      playerHairStyle = hairStyleArray[randomHead];
      playerHairColor = colorArray[randomHairColor];
      playerTabardColor = colorArray[randomTabardColor];
   }

   private void RandomizeJoinPlayer() {

      // Randomize from Lists
      int randomSide = Random.Range(0, sideList.Count);
      int randomHead = Random.Range(0, hairStyleList.Count);
      int randomHairColor = Random.Range(0, hairColorList.Count);
      int randomTabardColor = Random.Range(0, tabardColorList.Count);

      playerSide = sideList[randomSide];
      playerHairStyle = hairStyleList[randomHead];
      playerHairColor = hairColorList[randomHairColor];
      playerTabardColor = tabardColorList[randomTabardColor];
   }

   public void RunRandomize() {
      
      if(sideList.Count != 0
      && hairStyleList.Count != 0
      && hairColorList.Count != 0
      && tabardColorList.Count != 0) {

         RandomizeJoinPlayer();

         sideList.Clear();
         hairStyleList.Clear();
         hairColorList.Clear();
         tabardColorList.Clear();
      }
      else RandomizeHostPlayer();
      
      playerPropsList = new List<string>() {
         playerSide,
         playerHairStyle,
         playerHairColor,
         playerTabardColor,
      };
   }

   public void InstantiatePlayer (List<string> propsList, string playerStatus) {

      if(!hostPlayer) hostPlayer = InitPlayer(propsList, playerStatus);
      else joinPlayer = InitPlayer(propsList, playerStatus);
   }

   private GameObject InitPlayer(List<string> propsList, string playerStatus) {
      
      GameObject playerToInit = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
      getGameObj(playerToInit, "Player_UI").SetActive(false);
      SetSpritesArrays(playerToInit);

      UnsetPlayer();
      
      // Set player side
      for(int i_Side = 0; i_Side < sideArray.Length; i_Side++) {
         if(propsList[0] == sideArray[i_Side]) {

            spritesArray[i_Side].SetActive(true);
            sidePos = 2 *i_Side -1;


            // Set player hair
            for(int i_Hair = 0; i_Hair < hairStyleArray.Length; i_Hair++) {
               if(propsList[1] == hairStyleArray[i_Hair]) {

                  leftHeadArray[i_Hair].SetActive(true);
                  rightHeadArray[i_Hair].SetActive(true);
                  

                  // Set player hair & tabard color
                  for(int i_Color = 0; i_Color < colorArray.Length; i_Color++) {

                     // Hair color
                     if(propsList[2] == colorArray[i_Color]) {
                        leftHairStyleArray[i_Hair].GetComponent<SpriteRenderer>().color = hairColorsArray[i_Color];
                        rightHairStyleArray[i_Hair].GetComponent<SpriteRenderer>().color = hairColorsArray[i_Color];
                     }

                     // Tabard color 
                     if(propsList[3] == colorArray[i_Color]) {
                        tabardArray[i_Side].GetComponent<SpriteRenderer>().color = tabardColorsArray[i_Color];
                     }
                  }
               }
            }        
         }
      }

      playerToInit.transform.position = new Vector3(posX *sidePos, posY, 0);
      
      if(playerStatus == "isLocalPlayer") {
         getGameObj(playerToInit, "Player_UI").SetActive(true);
         localPlayer = playerToInit;
      }

      return playerToInit;
   }

   private void UnsetPlayer() {

      // Hide Player Sprites
      for(int i_Side = 0; i_Side < sideArray.Length; i_Side++) {
         spritesArray[i_Side].SetActive(false);

         // Hide Player Head
         for(int i_Hair = 0; i_Hair < hairStyleArray.Length; i_Hair++) {
            leftHeadArray[i_Hair].SetActive(false);
            rightHeadArray[i_Hair].SetActive(false);
         }        
      }
   }

   public void DestroyAllPlayers() {

      if(hostPlayer && joinPlayer) {
         Destroy(hostPlayer);
         Destroy(joinPlayer);
      }
   }

}