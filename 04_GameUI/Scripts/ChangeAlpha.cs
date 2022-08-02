using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ChangeAlpha : MonoBehaviour {

    private Image sprite;
    [SerializeField] private float maxAlpha = 1f;


    // Start
    void Start() {
        sprite = transform.GetComponent<Image>();
        sprite.enabled = true;
		
		var spriteColor = sprite.color;
		spriteColor.a = 0;
		sprite.color = spriteColor;
    }


    public void HideAlpha() {
        var spriteColor = sprite.color;
		spriteColor.a = 0;
		sprite.color = spriteColor;
    }


    public void ShowAlpha() {
        var spriteColor = sprite.color;
		spriteColor.a = maxAlpha;
		sprite.color = spriteColor;
    }
}
