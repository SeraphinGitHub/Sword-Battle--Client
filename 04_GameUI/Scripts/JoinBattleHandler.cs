using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JoinBattleHandler : MonoBehaviour {

    private GameHandler gameHandler;

    private void Start() {
        gameHandler = GameObject.Find("_GameHandler").GetComponent<GameHandler>();
    }
    
    public void SetBattleID() {
        string battleID = transform.Find("BattleID").GetComponent<TMP_Text>().text;
        string battleName = transform.Find("BattleName").GetComponent<TMP_Text>().text;

        gameHandler.battleID = battleID;
        gameHandler.JoinBattleRequest(battleID, battleName);
    }
}
