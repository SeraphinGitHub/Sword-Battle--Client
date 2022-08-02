using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GreyFilter : MonoBehaviour {	

	// Start
	private void Start() {
		transform.GetComponent<Image>().enabled = false;
	}
	
	// Grey Filter ON
	public void GreyFilterON() {
		transform.GetComponent<Image>().enabled = true;
	}
	
	// Grey Filter OFF
	public void GreyFilterOFF() {
		transform.GetComponent<Image>().enabled = false;
	}
}
