using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using TMPro;
using System;

public class GameHandler : MonoBehaviour {
   
   // ********** Server Options **********
      private int    FPS = 40;
      // private string serverURL = "http://localhost:3000/";
      private string serverURL = "https://sword-battle.onrender.com";
   // ********** Server Options **********


   [Header("**Attached Components**")]
   public GameObject joinBtnPrefab;
   public GameObject connectingText;
   public GameObject connectedText;
   public GameObject failedConnectText;
   public RectTransform scrollContent;
   public Image loadingBarSprite;

   [Header("**Health Sprites**")]
   public Image[] L_HealthSpritesArray     = new Image[2];
   public Image[] R_HealthSpritesArray     = new Image[2];

   [Header("**Left Shield Segments**")]
	public Animator[] LSh_SegArray          = new Animator[6];
   public Image[]    LSh_LifeSpritesArray  = new Image[6];
   public Image[]    LSh_FluidSpritesArray = new Image[6];

   [Header("**Right Shield Segments**")]
	public Animator[] RSh_SegArray          = new Animator[6];
   public Image[]    RSh_LifeSpritesArray  = new Image[6];
   public Image[]    RSh_FluidSpritesArray = new Image[6];


   [Header("**Attached Canvas**")]
   public GameObject loadingScreen;
   public GameObject langMenu;
   public GameObject mainMenu;
   public GameObject findBattleMenu;
   public GameObject optionsMenu;
   public GameObject optionsClickable;
   public GameObject optionsClicked;
   public GameObject battleUI;
   public GameObject customizeMenu;
   public GameObject messageUI;
   public GameObject damageTaken;
   public GameObject damageBlocked;
   public GameObject damageDone;

   [Header("**Attached Animator**")]
   public Animator loadingGearsAnim;
   public Animator loadingBarAnim;
   public Animator playerNameAnim;
   public Animator battleNameAnim;

   [Header("**Attached Buttons**")]
   public GameObject[] panelsBtn = new GameObject[7];

   [Header("**Attached InputFields**")]
   public InputField playerNameField;
   public InputField battleNameField;

   [Header("**Attatched TexMeshPro**")]
   public TextMeshProUGUI serverMessageTMP;
   public TextMeshProUGUI countDownTMP;
   public TextMeshProUGUI battleNameTMP;
   public TextMeshProUGUI leftPlayerNameTMP;
   public TextMeshProUGUI rightPlayerNameTMP;
   public TextMeshProUGUI damageTakenTMP;
   public TextMeshProUGUI damageBlockedTMP;
   public TextMeshProUGUI damageDoneTMP;


   // Hidden Public Variables
   [HideInInspector] public SocketIOUnity socket;
   [HideInInspector] public List<string>  playerProps;
   [HideInInspector] public List<string>  enemyProps;

   [HideInInspector] public Animator[][]  shieldSegmentsArray;
   [HideInInspector] public Image[][]     healthSpritesArray;
   [HideInInspector] public Image[][]     shieldLifeSpritesArray;
   [HideInInspector] public Image[][]     shieldFluidSpritesArray;
   [HideInInspector] public int[]         statesIndexArray = new int[3];

   [HideInInspector] public float  frameRate;
   [HideInInspector] public float  showPayerUIDelay;
   [HideInInspector] public string battleID;
   [HideInInspector] public string swordColor;
   [HideInInspector] public string playerName;
   [HideInInspector] public string enemyName;


   // Private Variables
   private string _SecurityToken_;

   private string[] selectedLangArray;
   private string[] lang_French   = new string[] {
      "L'hôte à quitté le match !",
      "L'invité à quitté le match !",
      " à gagné ce match !",
      "Bloque ",
   };
   private string[] lang_English  = new string[] {
      "Host player has left battle !",
      "Join player has left battle !",
      " won this battle !",
      "Block ",
   };
   private string[] lang_Japanese = new string[] {
      "ホスト プレイヤー退出しました !",
      "参加プレイヤー退出しました !",
      " はこのゲームに勝った !",
      "ガード ",
   };

   private float loginBarValue;
   private float toggleGearDelay       = 0.75f;
   private float toggleEmptyFieldDelay = 1.5f;

   private int battleCountBeforeScroll = 4;
   private int firstBattleOffsetY      = 15;
   private int battleOffsetY           = 177; // JoinBtn Height + OffsetY

   private int connectTimer   = 0;
   private int loginBarDelay  = 3; // seconds
   private int randomizeDelay = 1; // seconds
   private int respawnTime    = 5; // seconds
   private int endBattleCD    = 3; // seconds

   private bool isConnected;
   private bool isAuthentified;
   private bool isLoadBarUpdatable;
   private bool isBattleOnGoing;
   private bool isNewBattleCreated;
   private bool isSearchingBattles;
   private bool hasJoinedBattle;
   
   private string createdBattleName;
   private string joinedBattleName;
   private string playerSide;
   private string enemySide;

   private List<string> existBattle_IDList = new List<string>();
   private List<string> newBattles_IDList  = new List<string>();

   private CreateBattleClass newBattle;
   private InitPlayer        initPlayer;
   private PlayerHandler     enemyPH;
   private PlayerHandler     localPH;

   private Animator damageTakenAnim;
   private Animator damageBlockedAnim;
   private Animator damageDoneAnim;


   // ===========================================================================================================
   // Transfert Data Classes
   // ===========================================================================================================
   [System.Serializable]
   class PlayerClass {

      public string name;
      public string side;
      public string hairStyle;
      public string hairColor;
      public string tabardColor;
      public string swordColor;

      public PlayerClass(
      string name,
      string side,
      string hairStyle,
      string hairColor,
      string tabardColor,
      string swordColor) {

         this.name          = name;
         this.side          = side;
         this.hairStyle     = hairStyle;
         this.hairColor     = hairColor;
         this.tabardColor   = tabardColor;
         this.swordColor    = swordColor;
      }
   }

   [System.Serializable]
   class CreateBattleClass {

      public string battleName;
      public PlayerClass player;

      public CreateBattleClass(string battleName, PlayerClass player) {
         this.battleName    = battleName;
         this.player        = player;
      }
   }

   [System.Serializable]
   class FoundBattleClass {

      public string id;
      public string name;

      public FoundBattleClass(string id, string name) {
         this.id = id;
         this.name = name;
      }
   }
   
   [System.Serializable]
   class LeavingPlayerClass {
      
      public bool isHostPlayer;
      public bool isJoinPlayer;

      public LeavingPlayerClass(bool isHostPlayer, bool isJoinPlayer) {
         this.isHostPlayer = isHostPlayer;
         this.isJoinPlayer = isJoinPlayer;
      }
   }

   // Server SyncPack
   [System.Serializable]
   class SyncPackClass {
      
      public int[] statesIndexArray;
      public float playerPosX;
      public float walkDirX;
      public bool  isProtecting;

      public SyncPackClass(
      int[] statesIndexArray,
      float playerPosX,
      float walkDirX,
      bool  isProtecting) {

         this.statesIndexArray = statesIndexArray;
         this.playerPosX       = playerPosX;
         this.walkDirX         = walkDirX;
         this.isProtecting     = isProtecting;
      }
   }

   [System.Serializable]
   class DamageEnemyClass {

      public int playerHealth;
      public int shieldHealth;

      public DamageEnemyClass(
      int playerHealth,
      int shieldHealth) {
         
         this.playerHealth = playerHealth;
         this.shieldHealth = shieldHealth;
      }
   }
   

   // ===========================================================================================================
   // Awake() / Start() / Update()
   // ===========================================================================================================
   private void Awake() {

      // Init security token
      _SecurityToken_ = GetComponent<Security>().iniSecurityToken();
      
      // Init Socket IO
      socket = new SocketIOUnity(serverURL, new SocketIOOptions {
         Query = new Dictionary<string, string> {
            {"token", "UNITY" }
         },
         EIO = 4,
         Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
      });
      socket.JsonSerializer = new NewtonsoftJsonSerializer();
      socket.Connect();

      // Show UI components
      loadingScreen.SetActive(true);
      connectingText.SetActive(true);
      optionsClickable.SetActive(true);
      playerNameAnim.transform.gameObject.SetActive(false);
      battleNameAnim.transform.gameObject.SetActive(false);
      
      // Hide UI components
      connectedText.SetActive(false);
      failedConnectText.SetActive(false);
      findBattleMenu.SetActive(false);
      optionsClicked.SetActive(false);
      customizeMenu.SetActive(false);
      messageUI.SetActive(false);

      Vibration.Init();
      SwitchToMainMenu();
      initPlayer = GetComponent<InitPlayer>();

      // Set health bars 
      healthSpritesArray      = new Image[][] {
         L_HealthSpritesArray,
         R_HealthSpritesArray
      };

      // Set shield segments
      shieldSegmentsArray     = new Animator[][] {
         LSh_SegArray,
         RSh_SegArray
      };

      shieldLifeSpritesArray  = new Image[][] {
         LSh_LifeSpritesArray,
         RSh_LifeSpritesArray
      };

      shieldFluidSpritesArray = new Image[][] {
         LSh_FluidSpritesArray,
         RSh_FluidSpritesArray
      };

      damageTakenAnim   = damageTaken.GetComponent<Animator>();
      damageBlockedAnim = damageBlocked.GetComponent<Animator>();
      damageDoneAnim    = damageDone.GetComponent<Animator>();
   }

   private void Start() {

      LoadLanguage();
      LoadNamesFields();
      
      StartCoroutine( ShowLoadingGear()   );
      StartCoroutine( LoadingConnection() );

      float frameBase  = Mathf.Floor(1000 /FPS);
      frameRate        = frameBase /1000;
      loginBarValue    = 1 /frameBase;
      showPayerUIDelay = randomizeDelay +0.5f;


      // ===================================
      // Socket Listening Events
      // ===================================
      socket.OnAnyInUnityThread((channel, response) => {
         
         // Connection
         if(channel == "connected") {
            Authentification();
         }

         if(channel == "authFailed") {
            AuthentificationFailed();
         }

         if(channel == "authSucces") {
            AuthentificationSucceded();
         }

         // Create Battle
         if(channel == "battleCreated") {
            battleID = response.GetValue().GetRawText();
         }
         
         // Find Battle
         if(channel == "battleFound") {
            var battlesArray = response.GetValue<FoundBattleClass[]>();
            SetBattleList(battlesArray);
         }
         
         // Join Battle
         if(channel == "joinAccepted") {
            var enemyPlayer = response.GetValue<PlayerClass>();
            JoinBattleAccepted(enemyPlayer);
         }

         if(channel == "battleJoined") {
            InitBattle();
            StartCoroutine(BattleCreated(joinedBattleName));
         }

         if(channel == "enemyJoined") {
            var enemyPlayer = response.GetValue<PlayerClass>();
            InitEnemyPlayer(enemyPlayer);

            localPH.ResetPosition();
            ResetShieldSegments();
         }

         // Leave Battle
         if(channel == "battleEnded") {
            var leavingPlayer = response.GetValue<LeavingPlayerClass>();
            EnemyLeaveBattle(leavingPlayer);
         }

         // Receive Sync
         if(channel == "ReceiveSync") {
            var syncPack = response.GetValue<SyncPackClass>();
            ReceiveServerSync(syncPack);
         }

         if(channel == "GetDamages") {
            var damagesPack = response.GetValue<DamageEnemyClass>();
            localPH.UpdatePlayerHealth(damagesPack.playerHealth, damagesPack.shieldHealth);
         }

         // Reset Battle
         if(channel == "battleReset") {
            StartCoroutine( RespawnTimeOut() );
         }

      });
   }

   private void Update() {

      if(isLoadBarUpdatable) loadingBarSprite.fillAmount += loginBarValue *Time.deltaTime;
   }
   
   
   // ===========================================================================================================
   // Public Methods
   // ===========================================================================================================
   public void UpdatePlayerName() {

      PlayerPrefs.SetString("PlayerName", playerNameField.text);
      PlayerPrefs.Save();
      playerName = playerNameField.text;
   }

   public void UpdateBattleName() {

      PlayerPrefs.SetString("BattleName", battleNameField.text);
      PlayerPrefs.Save();
      createdBattleName = battleNameField.text;
   }

   public void CreateBattle() {

      VibrateButton();
      
      if(!isBattleOnGoing
      && playerName != ""
      && createdBattleName != "") {

         InitBattle();

         playerProps = initPlayer.RunRandomize(new List<string>());
         playerSide  = playerProps[0];
         swordColor  = playerProps[4];

         // Set hostPlayer
         PlayerClass hostPlayer = new PlayerClass(
            playerName,
            playerProps[0],
            playerProps[1],
            playerProps[2],
            playerProps[3],
            playerProps[4]
         );

         // Set battle
         newBattle = new CreateBattleClass(
            createdBattleName,
            hostPlayer
         );

         StartCoroutine(BattleCreated(createdBattleName));

         if(!isAuthentified && !isNewBattleCreated) isNewBattleCreated = true;
         if(isAuthentified) socket.Emit("createBattle", newBattle);
      }

      else {
         if(playerName == "")        StartCoroutine( ToggleEmptyField(playerNameAnim, "Left")  );
         if(createdBattleName == "") StartCoroutine( ToggleEmptyField(battleNameAnim, "Right") );
      }
   }

   public void FindBattle() {
      
      VibrateButton();

      // Clear ScrollContent
      newBattles_IDList.Clear();
      existBattle_IDList.Clear();

      foreach(Transform joinBtn in scrollContent) {
         Destroy(joinBtn.gameObject);
      }

      isSearchingBattles = true;
      StartCoroutine( SearchBattleList() );
   }

   public void JoinBattleRequest(string battleID, string battleName) {
      // Used in > JoinBattleHandler.cs
      
      if(!hasJoinedBattle) {
         hasJoinedBattle = true;

         joinedBattleName = battleName;
         socket.Emit("joinRequest", battleID);
      }
   }

   public void StopSearchBattle() {

      isSearchingBattles = false;
   }

   public void LeaveBattle() {

      VibrateButton();

      isBattleOnGoing = false;
      hasJoinedBattle = false;
      initPlayer.DestroyAllPlayers();
      enemyProps.Clear();
      CancelInvoke();

      socket.Emit("EndBattle", battleID);
      battleID = "";

      SwitchToMainMenu();
   }

   public void QuitApplication() {

      VibrateButton();
      socket.Disconnect();
      isAuthentified = false;

      Application.Quit();
   }

   public void BattleLoose() {

      socket.Emit("battleLoose");
   }
   
   public void ToggleWinnerText(string winnerName) {
      
      serverMessageTMP.text = winnerName.Trim('"') + selectedLangArray[2];
      serverMessageTMP.gameObject.SetActive(true);
   }

   public void SetDamageTakenTMP(float damagesValue) {

      string damageStr     = "-" + damagesValue.ToString();
      Vector3 textPosition = initPlayer.localPlayer.transform.position;
      ToggleDamageText(damageStr, textPosition, damageTakenTMP, damageTaken, damageTakenAnim);
   }

   public void SetDamageBlockedTMP(float damagesValue) {
      
      string damageStr     = selectedLangArray[3] + damagesValue.ToString();
      Vector3 textPosition = localPH.shieldColliders[localPH.sideIndex].transform.position;
      ToggleDamageText(damageStr, textPosition, damageBlockedTMP, damageBlocked, damageBlockedAnim);
   }

   public void SetDamageDoneTMP(float damagesValue) {
      
      Vector3 textPosition = initPlayer.enemyPlayer.transform.position;
      ToggleDamageText(damagesValue.ToString(), textPosition, damageDoneTMP, damageDone, damageDoneAnim);
   }
   
   public void VibrateButton() {
      Vibration.Vibrate(100);
   }

   public void ResetPlayerPrefs() {

      PlayerPrefs.DeleteAll();
      PlayerPrefs.Save();
   }
   
   // Languages
   public void ToggleFrench() {

      selectedLangArray = lang_French;

      PlayerPrefs.SetString("Language", "French");
      PlayerPrefs.Save();
      
      foreach(var btn in panelsBtn) {
         btn.transform.Find("Text-French"  ).gameObject.SetActive(true);
         btn.transform.Find("Text-English" ).gameObject.SetActive(false);
         btn.transform.Find("Text-Japanese").gameObject.SetActive(false);
      }
   }

   public void ToggleEnglish() {
      
      selectedLangArray = lang_English;

      PlayerPrefs.SetString("Language", "English");
      PlayerPrefs.Save();

      foreach(var btn in panelsBtn) {
         btn.transform.Find("Text-French"  ).gameObject.SetActive(false);
         btn.transform.Find("Text-English" ).gameObject.SetActive(true);
         btn.transform.Find("Text-Japanese").gameObject.SetActive(false);
      }
   }

   public void ToggleJapanese() {
      
      selectedLangArray = lang_Japanese;

      PlayerPrefs.SetString("Language", "Japanese");
      PlayerPrefs.Save();

      foreach(var btn in panelsBtn) {
         btn.transform.Find("Text-French"  ).gameObject.SetActive(false);
         btn.transform.Find("Text-English" ).gameObject.SetActive(false);
         btn.transform.Find("Text-Japanese").gameObject.SetActive(true);
      }
   }



   // ===========================================================================================================
   // Private Methods
   // ===========================================================================================================
   private void Authentification() {

      socket.Emit("checkToken", _SecurityToken_);
   }

   private void AuthentificationFailed() {

      connectingText.SetActive(false);
      connectedText.SetActive(false);
      failedConnectText.SetActive(true);
   }   
   
   private void AuthentificationSucceded() {
      
      isConnected    = true;
      isAuthentified = true;

      if(isNewBattleCreated) socket.Emit("createBattle", newBattle);

      connectingText.SetActive(false);
      connectedText.SetActive(true);
      failedConnectText.SetActive(false);
      loadingBarSprite.fillAmount = 1f;

      StartCoroutine(HideLoadingGear());
   }  

   private void LoadNamesFields() {

      playerNameField.text = PlayerPrefs.GetString("PlayerName");
      playerName = playerNameField.text;

      battleNameField.text = PlayerPrefs.GetString("BattleName");
      createdBattleName = battleNameField.text;
   }

   private void LoadLanguage() {

      string language = PlayerPrefs.GetString("Language");
      
      switch(language) {
         case "French"  : ToggleFrench();
         break;

         case "English" : ToggleEnglish();
         break;

         case "Japanese": ToggleJapanese();
         break;

         default : ToggleEnglish();
         break;
      }
   }

   private void SetBattleText(string name) {
      battleNameTMP.text = name;
   }

   private void SetNameText(string side, string name) {
      if(side == "Left") leftPlayerNameTMP.text = name;
      if(side == "Right") rightPlayerNameTMP.text = name;
   }

   private void InitBattle() {

      SetBattleText("");
      SetNameText("Left", "");
      SetNameText("Right", "");

      isBattleOnGoing = true;
      SwitchToBattle();
   }

   private void ResetShieldSegments() {

      foreach(Animator[] side in shieldSegmentsArray) {
         foreach(Animator segAnim in side) {
            
            segAnim.gameObject.SetActive(true);
            segAnim.enabled = true;
         }
      }
   }

   private void ResetPlayersBars() {

      ResetShieldSegments();

      // Foreach sides
      for(int i = 0; i < healthSpritesArray.Length; i++) {

         // Clear shield segments
         for(int j = 0; j < shieldLifeSpritesArray[i].Length; j++) {
            
            shieldLifeSpritesArray[i][j].fillAmount  = 0f;
            shieldFluidSpritesArray[i][j].fillAmount = 0f;
         }

         // Clear health bars
         for(int k = 0; k < healthSpritesArray[i].Length; k++) {
            healthSpritesArray[i][k].fillAmount = 0f;
         }
      }
   }

   private void SwitchToBattle() {

      langMenu.SetActive(false);
      mainMenu.SetActive(false);
      findBattleMenu.SetActive(false);
      optionsMenu.SetActive(false);
      optionsClickable.SetActive(true);
      optionsClicked.SetActive(false);
      battleUI.SetActive(true);
      messageUI.SetActive(true);
      damageTaken.SetActive(true);
      damageDone.SetActive(true);

      damageTakenTMP.text = "";
      damageDoneTMP.text = "";

      ResetPlayersBars();
   }

   private void SwitchToMainMenu() {
      
      langMenu.SetActive(true);
      mainMenu.SetActive(true);
      optionsMenu.SetActive(false);
      customizeMenu.SetActive(false);
      battleUI.SetActive(false);
      serverMessageTMP.gameObject.SetActive(false);
      countDownTMP.gameObject.SetActive(false);
      damageTaken.SetActive(false);
      damageDone.SetActive(false);
   }

   private void BackToMainMenu() {
      
      if(!isBattleOnGoing) {
         if(initPlayer.localPlayer) Destroy(initPlayer.localPlayer);
         if(!mainMenu.activeSelf) SwitchToMainMenu();
      }
   }

   private void SetBattleList(FoundBattleClass[] battlesArray) {

      newBattles_IDList.Clear();

      // ========================
      // Add new Battles
      // ========================
      foreach (var battle in battlesArray) {
         newBattles_IDList.Add(battle.id);

         // If new battle not rendered already
         if(!existBattle_IDList.Contains(battle.id)) {
            int existBattleCount = existBattle_IDList.Count;
            float joinBtn_PosY = -firstBattleOffsetY -battleOffsetY *existBattleCount;

            GameObject joinBtn = Instantiate(joinBtnPrefab, new Vector3(0, joinBtn_PosY), Quaternion.identity);
            joinBtn.transform.SetParent(scrollContent, false);

            joinBtn.transform.Find("BattleID").gameObject.GetComponent<TMP_Text>().text = battle.id;
            joinBtn.transform.Find("BattleName").gameObject.GetComponent<TMP_Text>().text = battle.name;

            // Extend ScorllContent height
            if(existBattleCount >= battleCountBeforeScroll) {
               scrollContent.sizeDelta = new Vector2(0, scrollContent.sizeDelta.y + battleOffsetY);
            }
            existBattle_IDList.Add(battle.id);
         }
      }
      

      // ========================
      // Remove old Battles
      // ========================
      int renderedBattleCount = scrollContent.childCount;

      // For all rendered battle
      for(int i = 0; i < renderedBattleCount; i++) {
         Transform joinBtn = scrollContent.GetChild(i);
         string BattleID = joinBtn.Find("BattleID").GetComponent<TMP_Text>().text;            
         
         // If rendered battle doesn't exist anymore 
         if(!newBattles_IDList.Contains(BattleID)) {
            
            Destroy(joinBtn.gameObject);
            existBattle_IDList.Remove(BattleID);

            // Move up other rendered battle after the destroyed one
            for(int j = 0; j < renderedBattleCount -i; j++) {
               RectTransform OtherjoinBtn = scrollContent.GetChild(j +i).GetComponent<RectTransform>();
               float OtherjoinBtn_PosY = OtherjoinBtn.anchoredPosition.y + battleOffsetY;
               OtherjoinBtn.anchoredPosition = new Vector2(0, OtherjoinBtn_PosY);
            }

            // Shorten ScorllContent height
            int existBattleCount = existBattle_IDList.Count;
            if(existBattleCount >= battleCountBeforeScroll) {
               scrollContent.sizeDelta = new Vector2(0, scrollContent.sizeDelta.y - battleOffsetY);
            }
         }
      }
   }

   private void JoinBattleAccepted(PlayerClass enemyPlayer) {
      
      // Set enemy player (Host player)
      enemyName = enemyPlayer.name;
      enemySide = enemyPlayer.side;
      
      enemyProps = new List<string>() {
         enemyPlayer.side,
         enemyPlayer.hairStyle,
         enemyPlayer.hairColor,
         enemyPlayer.tabardColor,
         enemyPlayer.swordColor
      };
      
      // Randomize joinPlayer, out of hostPlayer props
      playerProps = initPlayer.RunRandomize(enemyProps);
      playerSide  = playerProps[0];
      swordColor  = playerProps[4];

      // Set joinPlayer
      PlayerClass joinPlayerProps = new PlayerClass(
         playerName,
         playerProps[0],
         playerProps[1],
         playerProps[2],
         playerProps[3],
         playerProps[4]
      );

      // On received event ("battleJoined") ==> Coroutine BattleCreated();
      socket.Emit("joinBattle", joinPlayerProps);
   }

   private void InitEnemyPlayer(PlayerClass enemyPlayer) {

      serverMessageTMP.gameObject.SetActive(false);

      List<string> joinPropsList = new List<string>() {
         enemyPlayer.side,
         enemyPlayer.hairStyle,
         enemyPlayer.hairColor,
         enemyPlayer.tabardColor,
         enemyPlayer.swordColor,
      };      
      
      enemySide = enemyPlayer.side;
      enemyName = enemyPlayer.name;

      // Instantiate enemy player
      initPlayer.InstantiatePlayer(joinPropsList, false);
      enemyPH = initPlayer.enemyPH;

      SetNameText(enemyPlayer.side, enemyPlayer.name);

      // Start server sync
      StartSync("ServerSync");
   }

   private void EnemyLeaveBattle(LeavingPlayerClass leavingPlayer) {

      // Host leave
      if(leavingPlayer.isHostPlayer) {

         enemyPH.ClearBars();
         Destroy(initPlayer.enemyPlayer);

         isBattleOnGoing = false;         
         enemyProps.Clear();

         serverMessageTMP.text = selectedLangArray[0];
         serverMessageTMP.gameObject.SetActive(true);
         countDownTMP.gameObject.SetActive(true);
         SetNameText(enemySide, "");

         StartCoroutine( StartCountDown(endBattleCD) );
      }

      // Joined leave
      if(leavingPlayer.isJoinPlayer) {
         
         enemyPH.ClearBars();
         Destroy(initPlayer.enemyPlayer);

         serverMessageTMP.text = selectedLangArray[1];
         serverMessageTMP.gameObject.SetActive(true);
         SetNameText(enemySide, "");
      }
   }
   
   private void ToggleRespawnCD() {

      countDownTMP.gameObject.SetActive(true);
      StartCoroutine( StartCountDown(respawnTime) );
   }

   private void ToggleDamageText(string damageStr, Vector3 toggledPos, TextMeshProUGUI damageTMP, GameObject damageTypeGO, Animator damageTypeAnim) {
      
      damageTMP.text = damageStr;
      damageTypeGO.transform.position = toggledPos;
      damageTypeAnim.SetTrigger("DmgFadeOut");
   }


   // ===========================================================================================================
   // Coroutines
   // ===========================================================================================================
   IEnumerator LoadingConnection() {

      connectTimer++;
      
      if(connectTimer == loginBarDelay) {
         isLoadBarUpdatable = true;
         loadingBarSprite.fillAmount = 0f;
         loadingBarAnim.SetTrigger("Show");
      }
      
      yield return new WaitForSeconds(1f);

      if(!isConnected) StartCoroutine( LoadingConnection() );
   }

   IEnumerator ShowLoadingGear() {

      yield return new WaitForSeconds(toggleGearDelay);
      
      loadingGearsAnim.SetTrigger("Connecting");
   }

   IEnumerator HideLoadingGear() {

      loadingGearsAnim.SetTrigger("Hide");

      if(isLoadBarUpdatable) {
         isLoadBarUpdatable = false;
         loadingBarAnim.SetTrigger("Hide");
      }

      yield return new WaitForSeconds(toggleGearDelay);

      loadingScreen.SetActive(false);
   }

   IEnumerator BattleCreated(string battleName) {
      yield return new WaitForSeconds(randomizeDelay);

      if(isBattleOnGoing) {

         SetBattleText(battleName);
         SetNameText(playerSide, playerName);

         // Instantiate LocalPlayer
         initPlayer.InstantiatePlayer(playerProps, true);
         localPH = initPlayer.localPH;
         InitEvents();

         // Instantiate EnemyPlayer if exists
         if(enemyProps.Count != 0) {

            initPlayer.InstantiatePlayer(enemyProps, false);
            enemyPH = initPlayer.enemyPH;
            SetNameText(enemySide, enemyName);
         }

         // Start server sync
         if(initPlayer.enemyPlayer) StartSync("ServerSync");
      }
      else yield break;
   }

   IEnumerator SearchBattleList() {

      if(isSearchingBattles) {
         if(isAuthentified) socket.Emit("findBattle");

         yield return new WaitForSeconds(0.7f);
         StartCoroutine( SearchBattleList() );
      }
   }
   
   IEnumerator StartCountDown(int countDownValue) {

      countDownTMP.text = countDownValue.ToString();
      yield return new WaitForSeconds(1f);
      countDownValue--;

      if(countDownValue > 0) StartCoroutine( StartCountDown(countDownValue) );
      else BackToMainMenu();
   }

   IEnumerator RespawnTimeOut() {

      localPH.isFighting = false;
      ToggleRespawnCD();
      CancelInvoke();

      yield return new WaitForSeconds( (float)respawnTime );

      localPH.PlayerRespawn();
      enemyPH.PlayerRespawn();
      ResetShieldSegments();

      serverMessageTMP.gameObject.SetActive(false);
      countDownTMP.gameObject.SetActive(false);

      // Start server sync
      StartSync("ServerSync");
   }

   IEnumerator ToggleEmptyField(Animator anim, string side) {

      anim.transform.gameObject.SetActive(true);
      anim.SetTrigger(side + "Show");

      yield return new WaitForSeconds(toggleEmptyFieldDelay);
      anim.SetTrigger(side + "Hide");

      yield return new WaitForSeconds(0.5f);
      anim.transform.gameObject.SetActive(false);
   }


   // ===========================================================================================================
   // Events
   // ===========================================================================================================
   private event EventHandler tr_moveLeft;
   private event EventHandler tr_moveRight;
	private event EventHandler tr_stopMove;
	private event EventHandler tr_strikeAttack;
	private event EventHandler tr_estocAttack;
	private event EventHandler tr_protect;
	private event EventHandler tr_stopProtect;
   
   private void InitEvents() {

      tr_moveLeft     = localPH.Ev_MoveLeft;
      tr_moveRight    = localPH.Ev_MoveRight;
      tr_stopMove     = localPH.Ev_StopMove;
      tr_strikeAttack = localPH.Ev_StrikeAttack;
      tr_estocAttack  = localPH.Ev_EstocAttack;
      tr_protect      = localPH.Ev_Protect;
      tr_stopProtect  = localPH.Ev_StopProtect;
   }
   
   public void TriggerEvents(string trigger) {

      if(trigger == "MoveLeft"    && tr_moveLeft     != null) tr_moveLeft    (this, EventArgs.Empty);
      if(trigger == "MoveRight"   && tr_moveRight    != null) tr_moveRight   (this, EventArgs.Empty);
      if(trigger == "StopMove"    && tr_stopMove     != null) tr_stopMove    (this, EventArgs.Empty);
      if(trigger == "Strike"      && tr_strikeAttack != null) tr_strikeAttack(this, EventArgs.Empty);
      if(trigger == "Estoc"       && tr_estocAttack  != null) tr_estocAttack (this, EventArgs.Empty);
      if(trigger == "Protect"     && tr_protect      != null) tr_protect     (this, EventArgs.Empty);
      if(trigger == "StopProtect" && tr_stopProtect  != null) tr_stopProtect (this, EventArgs.Empty);
   }


   // ===========================================================================================================
   // Server Sync Methods
   // ===========================================================================================================
   public void StartSync(string methodName) {
      InvokeRepeating(methodName, 0, frameRate);
   }


   // Emit Sync
   public void ServerSync() {
      if(initPlayer.localPlayer && initPlayer.enemyPlayer) {
         
         SyncPackClass sync = new SyncPackClass(

            statesIndexArray,
            Mathf.Floor(localPH.transform.position.x *10) /10,
            localPH.walkDirX,
            localPH.isProtecting
         );

         socket.Emit("ServerSync", sync);
      }
   }

   public void SendDamageToEnemy() {

      DamageEnemyClass damagesPack = new DamageEnemyClass(
         enemyPH.playerHealth,
         enemyPH.shieldHealth
      );

      socket.Emit("EnemyDamage", damagesPack);
   }


   // Receive Sync
   private void ReceiveServerSync(SyncPackClass syncPack) {
      if(initPlayer.enemyPlayer) {

         enemyPH.isProtecting = syncPack.isProtecting;
         enemyPH.SetEnemyAnim  (syncPack.statesIndexArray);
         enemyPH.EnemyMovements(syncPack.walkDirX, syncPack.playerPosX);
      }
   }

}