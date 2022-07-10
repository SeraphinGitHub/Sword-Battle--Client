using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStateOnLoad : MonoBehaviour {
    
    [SerializeField] private GameObject[] ShowUIObjOnLoad = default;
    [SerializeField] private GameObject[] HideUIObjOnLoad = default;

    // Awake
    private void Awake() {
        foreach(GameObject UIObj in ShowUIObjOnLoad) {
            UIObj.SetActive(true);
        }
        
        foreach(GameObject UIObj in HideUIObjOnLoad) {
            UIObj.SetActive(false);
        }
    }
}
