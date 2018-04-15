using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScaleItem : MonoBehaviour {

	public GameObject activeObject;
	public bool scale = false;
	public GameObject scaleTool;
	public bool isOn = true;

	//Input vars
	public GameObject xInput;
	public float xVal;
	public GameObject yInput;
	public float yVal;
	public GameObject zInput;
	public float zVal;

	void Update(){
		if (Input.GetMouseButtonDown (1) && scale == false) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast (ray, out hit, 100)){
				if(hit.transform.gameObject.name != "Ground"){
					activeObject = hit.transform.gameObject;
					scale = true;
					scaleTool.SetActive(true);
				}
			}
		} else if(Input.GetMouseButtonDown(2) && scale == true || Input.GetMouseButtonDown(1) && scale == true || GetComponent<MoveItem>().movement == true && GetComponent<MoveItem>().activeObject == gameObject && scale == true || Input.GetKeyDown(KeyCode.Escape) && scale == true){
			scale = false;
			scaleTool.SetActive(false);
			isOn = false;
		}
		if (scale == true && activeObject == gameObject) {
			if(isOn == false){
				if (xVal == 0) {
					xInput.GetComponent<InputField> ().text = 1.ToString ();
				} else {
					xInput.GetComponent<InputField> ().text = xVal.ToString ();
				}
				if (yVal == 0) {
					yInput.GetComponent<InputField> ().text = 1.ToString ();
				} else {
					yInput.GetComponent<InputField> ().text = yVal.ToString ();
				}
				if (zVal == 0) {
					zInput.GetComponent<InputField> ().text = 1.ToString ();
				} else {
					zInput.GetComponent<InputField> ().text = zVal.ToString ();
				}
				isOn = true;
			}
			xVal = float.Parse (xInput.GetComponent<InputField>().text);
			yVal = float.Parse (yInput.GetComponent<InputField>().text);
			zVal = float.Parse (zInput.GetComponent<InputField>().text);
			if(xVal == 0 && yVal == 0 && zVal == 0){
				transform.localScale = new Vector3(1f,1f,1f);
			} else if(xVal == 0 && yVal == 0){
				transform.localScale = new Vector3(1f,1f,zVal);
			} else if(xVal == 0 && zVal == 0){
				transform.localScale = new Vector3(1f,yVal,1f);
			} else if(yVal == 0 && zVal == 0){
				transform.localScale = new Vector3(xVal,1f,1f);
			} else if(xVal == 0){
				transform.localScale = new Vector3(1f,yVal,zVal);
			} else if(yVal == 0){
				transform.localScale = new Vector3(xVal,1f,zVal);
			} else if(zVal == 0){
				transform.localScale = new Vector3(xVal,yVal,1f);
			} else {
				transform.localScale = new Vector3(xVal,yVal,zVal);
			}
		}
	}
}
