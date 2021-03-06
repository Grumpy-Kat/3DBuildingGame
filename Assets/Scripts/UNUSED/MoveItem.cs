﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MoveItem : MonoBehaviour {
	
	public GameObject activeObject;
	public bool movement = false;
	public GameObject positionTool;
	public bool isOn = true;
	
	//Input vars
	public GameObject xInput;
	public float xVal;
	public GameObject yInput;
	public float yVal;
	public GameObject zInput;
	public float zVal;
	
	void Update(){
		if (Input.GetMouseButtonDown (0) && movement == false) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast (ray, out hit, 100)){
				if(hit.transform.gameObject.name != "Ground"){
					activeObject = hit.transform.gameObject;
					movement = true;
					positionTool.SetActive(true);
				}
			}
		} else if(Input.GetMouseButtonDown(2) && movement == true || Input.GetMouseButtonDown(1) && movement == true || Input.GetKeyDown(KeyCode.Escape) && movement == true){
			movement = false;
			positionTool.SetActive(false);
			isOn = false;
		}
		if (movement == true && activeObject == gameObject) {
			if (isOn == false) {
				if (xVal == 0) {
					xInput.GetComponent<InputField> ().text = 0.ToString ();
				} else {
					xInput.GetComponent<InputField> ().text = xVal.ToString ();
				}
				if (yVal == 0) {
					yInput.GetComponent<InputField> ().text = 0.ToString ();
				} else {
					yInput.GetComponent<InputField> ().text = yVal.ToString ();
				}
				if (zVal == 0) {
					zInput.GetComponent<InputField> ().text = 0.ToString ();
				} else {
					zInput.GetComponent<InputField> ().text = zVal.ToString ();
				}
				isOn = true;
			}
			xVal = float.Parse (xInput.GetComponent<InputField> ().text);
			yVal = float.Parse (yInput.GetComponent<InputField> ().text);
			zVal = float.Parse (zInput.GetComponent<InputField> ().text);
			if(xVal == 0){
				xVal = 0;
			}
			if(yVal == 0){
				yVal = 0;
			}
			if(zVal == 0){
				zVal = 0;
			}
			if(xVal != transform.parent.transform.position.x || yVal != transform.parent.transform.position.y || zVal != transform.parent.transform.position.z){
				transform.parent.transform.Translate (new Vector3 (xVal - transform.parent.transform.position.x, yVal - transform.parent.transform.position.y, zVal - transform.parent.transform.position.z));
			}
		}
	}
}
