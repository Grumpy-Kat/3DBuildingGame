using UnityEngine;
using System.Collections;

public class RotateCamera : MonoBehaviour {

	public GameObject target;
	public Quaternion prevRotate;

	void Start(){
		transform.LookAt (target.transform);
	}

	void Update(){
		prevRotate = transform.rotation;
		transform.LookAt(target.transform);
		if(Input.GetKey(KeyCode.UpArrow)){
			if(transform.position.z >= 0 && Mathf.Sign(transform.position.z) == 0 || transform.position.z <= 0 && Mathf.Sign(transform.position.z) == 1){

			} else{
				transform.Translate (Vector3.up * 10.0f * Time.deltaTime);
			}
		} else if(Input.GetKey(KeyCode.DownArrow)){
			if(transform.position.z >= 0 && Mathf.Sign(transform.position.z) == 0 || transform.position.z <= 0 && Mathf.Sign(transform.position.z) == 1){
				
			} else{
				transform.Translate (Vector3.down * 10.0f * Time.deltaTime);
			}
		} else if(Input.GetKey(KeyCode.LeftArrow)){
			transform.Translate (Vector3.left * 10.0f * Time.deltaTime);
		} else if(Input.GetKey(KeyCode.RightArrow)){
			transform.Translate (Vector3.right * 10.0f * Time.deltaTime);
		}
		if(prevRotate.y > transform.rotation.y && !Input.GetKey(KeyCode.LeftArrow) || prevRotate.y > transform.rotation.y && !Input.GetKey(KeyCode.RightArrow)){
			//print (prevRotate.y.ToString() + "is more than" + transform.rotation.y.ToString());
			transform.Rotate(Vector3.up * -180);
			transform.LookAt(target.transform);
		} else if(prevRotate.y < transform.rotation.y && !Input.GetKey(KeyCode.LeftArrow) || prevRotate.y < transform.rotation.y && !Input.GetKey(KeyCode.RightArrow)){
			//print (prevRotate.y.ToString() + "is less than" + transform.rotation.y.ToString());
			transform.Rotate(0,-180,0);
			transform.LookAt(target.transform);
		}
	}
}
