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
      new Color32(255, 150, 0, 255),
      new Color32(255, 0, 0, 255),
      new Color32(170, 0, 255, 255),
   };

   [Header("Tabard Colors")]
   public Color[] tabardColorsArray = {
      new Color32(0, 120, 255, 255),
      new Color32(23, 171, 0, 255),
      new Color32(255, 200, 0, 255),
      new Color32(255, 120, 0, 255),
      new Color32(220, 0, 0, 255),
      new Color32(135, 0, 202, 255),
   };


   // Public Hidden Variables
   [HideInInspector] public bool isLocalPlayer;
   [HideInInspector] public string playerSide;
   [HideInInspector] public List<string> playerPropsList;
   [HideInInspector] public List<GameObject> playersList = new List<GameObject>();


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
      "Orange",
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

   private GameObject newPlayer;
   private GameObject getGameObj(string name) { return newPlayer.transform.Find(name).gameObject; }

   private GameObject[] spritesArray;
   private GameObject[] leftHeadArray;
   private GameObject[] leftHairStyleArray;
   private GameObject[] rightHeadArray;
   private GameObject[] rightHairStyleArray;
   private GameObject[] tabardArray;


   // ====================================================================================
   // Methods
   // ====================================================================================
   private void SetSpritesArrays() {

      // Player Side
      spritesArray = new GameObject[] {
         getGameObj("Sprites_LeftPlayer"),
         getGameObj("Sprites_RightPlayer"),
      };

      // Left Player Head
      leftHeadArray = new GameObject[] {
         getGameObj("Sprites_LeftPlayer/Head/Face_Short_Hair"),
         getGameObj("Sprites_LeftPlayer/Head/Face_Long_Hair"),
      };
      
      // Left Player Hair
      leftHairStyleArray = new GameObject[] {
         getGameObj("Sprites_LeftPlayer/Head/Face_Short_Hair/Short_Hair"),
         getGameObj("Sprites_LeftPlayer/Head/Face_Long_Hair/Long_Hair"),
      };
      
      // Right Player Head
      rightHeadArray = new GameObject[] {
         getGameObj("Sprites_RightPlayer/Head/Face_Short_Hair"),
         getGameObj("Sprites_RightPlayer/Head/Face_Long_Hair"),
      };
      
      // Right Player Head
      rightHairStyleArray = new GameObject[] {
         getGameObj("Sprites_RightPlayer/Head/Face_Short_Hair/Short_Hair"),
         getGameObj("Sprites_RightPlayer/Head/Face_Long_Hair/Long_Hair"),
      };

      // Player Tabard
      tabardArray = new GameObject[] {
         getGameObj("Sprites_LeftPlayer/Body/Tabard"),
         getGameObj("Sprites_RightPlayer/Body/Tabard"),
      };
   }

   public void RemoveRandomizeProps(List<string> hostPropsList) {

      // Remove hostPlayer props to avoid same props for joinPlayer
      foreach(string side in sideArray) {
         if(side != hostPropsList[0]) sideList.Add(side);
      }

      foreach(string hair in hairStyleArray) {
         if(hair != hostPropsList[1]) hairStyleList.Add(hair);
      }

      foreach(string color in colorArray) {
         if(color != hostPropsList[2]) hairColorList.Add(color);
         if(color != hostPropsList[3]) tabardColorList.Add(color);
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

   public void InstantiatePlayer (List<string> propsList) {
      
      newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
      getGameObj("Player_UI").SetActive(false);

      SetSpritesArrays();
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

      newPlayer.transform.position = new Vector3(posX *sidePos, posY, 0);
      playersList.Add(newPlayer);

      if(isLocalPlayer) getGameObj("Player_UI").SetActive(true);
   }

   public void UnsetPlayer() {

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

   public void DestroyOnePlayer(string playerToDestroy) {

      if(playersList.Count != 0) {

         if(playerToDestroy == "hostPlayer") {
            Destroy(playersList[0]);
            playersList.Remove(playersList[0]);
         }

         if(playerToDestroy == "joinPlayer") {
            
            if(playersList.Count == 1) {
               Destroy(playersList[0]);
               playersList.Remove(playersList[0]);
            }

            if(playersList.Count == 2) {
               Destroy(playersList[1]);
               playersList.Remove(playersList[1]);
            }
         }
      }
   }

   public void DestroyAllPlayers() {

      if(playersList.Count != 0) {
         foreach(var player in playersList) Destroy(player);
         playersList.Clear();
      }
   }
}
