using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShowHideButtons : MonoBehaviour {

    [SerializeField] private GameObject UIObj = default;


    public void HideUIObj() {
        UIObj.SetActive(false);
    }


    public void ShowUIObj() {
        UIObj.SetActive(true);
    }


    // public void HideImage() {
    //     UIObj.GetComponent<Image>().enabled = false;
    // }


    // public void ShowImage() {
    //     UIObj.GetComponent<Image>().enabled = true;
    // }
}