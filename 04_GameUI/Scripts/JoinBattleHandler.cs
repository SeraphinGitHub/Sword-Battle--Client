using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JoinBattleHandler : MonoBehaviour {
    
    public void SetBattleID() {
        string battleID = transform.Find("BattleID").GetComponent<TMP_Text>().text;
        string battleName = transform.Find("BattleName").GetComponent<TMP_Text>().text;
        GameObject.Find("_GameHandler").GetComponent<GameHandler>().JoinBattleRequest(battleID, battleName);
    }
}
