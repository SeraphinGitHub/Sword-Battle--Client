using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitPlayer : MonoBehaviour {

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
   [HideInInspector] public GameObject localPlayer;
   [HideInInspector] public GameObject enemyPlayer;
   [HideInInspector] public PlayerHandler localPH;
   [HideInInspector] public PlayerHandler enemyPH;


   // Private Variables
   private int sidePos;

   private List<string> sideList;
   private List<string> hairStyleList;
   private List<string> hairColorList;
   private List<string> tabardColorList;
   private List<string> swordColorList;

   private GameObject[] spritesArray;
   private GameObject[] leftHeadArray;
   private GameObject[] leftHairStyleArray;
   private GameObject[] rightHeadArray;
   private GameObject[] rightHairStyleArray;
   private GameObject[] tabardArray;

   private GameHandler gameHandler;
   private SwordCollider localSwordCol;

   
   // ====================================================================================
   // Start
   // ====================================================================================
   private void Start() {
      gameHandler = GetComponent<GameHandler>();
   }


   // ====================================================================================
   // Public Methods
   // ====================================================================================
   public List<string> RunRandomize(List<string> enemyPropsList) {
      
      // Set Props List
      SetPorpsLists();

      // Remove enemyPlayer props to avoid same props for localPlayer
      if(enemyPropsList.Count != 0) {

         sideList.Remove(enemyPropsList[0]);
         hairStyleList.Remove(enemyPropsList[1]);
         hairColorList.Remove(enemyPropsList[2]);
         tabardColorList.Remove(enemyPropsList[3]);
         swordColorList.Remove(enemyPropsList[4]);
      }
      
      int randomSide = Random.Range(0, sideList.Count);
      int randomHead = Random.Range(0, hairStyleList.Count);
      int randomHairColor = Random.Range(0, hairColorList.Count);
      int randomTabardColor = Random.Range(0, tabardColorList.Count);
      int randomSwordColor = Random.Range(0, swordColorList.Count);

      List<string> randomPropsList = new List<string>() {
         sideList[randomSide],
         hairStyleList[randomHead],
         hairColorList[randomHairColor],
         tabardColorList[randomTabardColor],
         swordColorList[randomSwordColor],
      };

      // Reset Props List
      SetPorpsLists();

      return randomPropsList;
   }

   public void InstantiatePlayer (List<string> propsList, bool isLocalPlayer) {

      GameObject playerInstance = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
      PlayerHandler instPH = playerInstance.GetComponent<PlayerHandler>();

      SetPlayerSprites(playerInstance);
      UnsetPlayer();
      SetPlayer(propsList);

      instPH.SetCharacterSide(propsList[0]);
      instPH.SetSwordColor(propsList[4]);

      // Local Player
      if(isLocalPlayer) {
         localPlayer = playerInstance;
         localPH = playerInstance.GetComponent<PlayerHandler>();
         localPH.isLocalPlayer = true;

         localSwordCol = localPH.swordColliders[localPH.sideIndex].GetComponent<SwordCollider>();
         localSwordCol.localPH = localPH;
      }

      // Enemy Player
      else {
         enemyPlayer = playerInstance;
         enemyPH = playerInstance.GetComponent<PlayerHandler>();
         localSwordCol.enemyPlayer = playerInstance;
         localSwordCol.enemyPH = enemyPH;
      }

      playerInstance.transform.position = new Vector3(instPH.spawnX *sidePos, instPH.spawnY, 0);
   }

   public void DestroyAllPlayers() {
      if(localPlayer) Destroy(localPlayer);
      if(enemyPlayer) Destroy(enemyPlayer);
   }


   // ====================================================================================
   // Private Methods
   // ====================================================================================
   private GameObject getGameObj(GameObject player, string name) {
      return player.transform.Find(name).gameObject;
   }

   private void SetPorpsLists() {

      sideList = new List<string>() {
         "Left",
         "Right",
      };

      hairStyleList = new List<string>() {
         "Short",
         "Long",
      };

      hairColorList = new List<string>() {
         "Blue",
         "Green",
         "Yellow",
         "Red",
         "Violet",
      };

      tabardColorList = new List<string>() {
         "DarkBlue",
         "DarkGreen",
         "DarkYellow",
         "DarkRed",
         "DarkViolet",
      };

      swordColorList = new List<string>() {
         "Green",
         "Violet",
         "Red",
      };
   }

   private void SetPlayerSprites(GameObject player) {

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

   private void SetPlayer(List<string> propsList) {
      
      // Set player side
      for(int i_Side = 0; i_Side < sideList.Count; i_Side++) {
         if(propsList[0] == sideList[i_Side]) {

            spritesArray[i_Side].SetActive(true);
            sidePos = 2 *i_Side -1;


            // Set player hair
            for(int i_Hair = 0; i_Hair < hairStyleList.Count; i_Hair++) {
               if(propsList[1] == hairStyleList[i_Hair]) {

                  leftHeadArray[i_Hair].SetActive(true);
                  rightHeadArray[i_Hair].SetActive(true);
                  

                  // Set player hair & tabard color
                  for(int i_Color = 0; i_Color < hairColorList.Count; i_Color++) {

                     // Hair color
                     if(propsList[2] == hairColorList[i_Color]) {
                        leftHairStyleArray[i_Hair].GetComponent<SpriteRenderer>().color = hairColorsArray[i_Color];
                        rightHairStyleArray[i_Hair].GetComponent<SpriteRenderer>().color = hairColorsArray[i_Color];
                     }

                     // Tabard color 
                     if(propsList[3] == tabardColorList[i_Color]) {
                        tabardArray[i_Side].GetComponent<SpriteRenderer>().color = tabardColorsArray[i_Color];
                     }
                  }
               }
            }
         }
      }
   }

   private void UnsetPlayer() {

      // Hide Player Sprites
      for(int i_Side = 0; i_Side < sideList.Count; i_Side++) {
         spritesArray[i_Side].SetActive(false);

         // Hide Player Head
         for(int i_Hair = 0; i_Hair < hairStyleList.Count; i_Hair++) {
            leftHeadArray[i_Hair].SetActive(false);
            rightHeadArray[i_Hair].SetActive(false);
         }        
      }
   }

}