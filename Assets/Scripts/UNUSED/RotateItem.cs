using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RotateItem : MonoBehaviour {
	
	public GameObject activeObject;
	public bool rotation = false;
	public GameObject rotationTool;
	public bool isOn = true;
	
	//Input vars
	public GameObject xInput;
	public float xVal;
	public GameObject yInput;
	public float yVal;
	public GameObject zInput;
	public float zVal;
	
	void Update(){
		if (Input.GetMouseButtonDown (0) && rotation == false) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast (ray, out hit, 100)){
				if(hit.transform.gameObject.name != "Ground"){
					activeObject = hit.transform.gameObject;
					rotation = true;
					rotationTool.SetActive(true);
				}
			}
		} else if(Input.GetMouseButtonDown(2) && rotation == true || Input.GetMouseButtonDown(1) && rotation == true || Input.GetKeyDown(KeyCode.Escape) && rotation == true){
			rotation = false;
			rotationTool.SetActive(false);
			isOn = false;
		}
		if (rotation == true && activeObject == gameObject) {
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
			if(xVal != transform.parent.transform.rotation.x || yVal != transform.parent.transform.rotation.y || zVal != transform.parent.transform.rotation.z){
				transform.parent.transform.rotation = new Quaternion(xVal - transform.parent.transform.rotation.x, yVal - transform.parent.transform.rotation.y, zVal - transform.parent.transform.rotation.z,transform.parent.transform.rotation.w);
			}
		}
	}
}
