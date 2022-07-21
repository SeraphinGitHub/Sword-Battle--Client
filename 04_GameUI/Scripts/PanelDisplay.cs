using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelDisplay : MonoBehaviour {

    public GameObject Panel = default;

    public void HidePanel() {
        Panel.SetActive(false);
    }

    public void ShowPanel() {
        Panel.SetActive(true);
    }
}