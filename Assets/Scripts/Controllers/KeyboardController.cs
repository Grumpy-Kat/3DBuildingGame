using UnityEngine;

public class KeyboardController : MonoBehaviour {

	//reference to other controllers
	BuildModeController bmc;
	MouseController mc;
	//color picker
	GameObject colorPicker;

	void Start(){
		bmc = GameObject.FindObjectOfType<BuildModeController>();
		mc = GameObject.FindObjectOfType<MouseController>();
		colorPicker = GameObject.FindObjectOfType<ColorPicker>().gameObject;
		colorPicker.SetActive(false);
	}

	void Update () {
		UpdateCameraRotation();
		UpdateRotLevel();
		UpdateActiveFloor();
		UpdateDeletion();
		UpdateGrid();
		UpdateMoveNeighbors();
		UpdateActiveColorPicker();
	}

	/// <summary>
	///  updates the rotLevel of Objects
	/// </summary>
	void UpdateRotLevel(){
		if(Input.GetKeyDown(KeyCode.LeftBracket) || Input.GetKeyDown(KeyCode.Comma)){
			bmc.RotLevel--;
		}
		if(Input.GetKeyDown(KeyCode.RightBracket) || Input.GetKeyDown(KeyCode.Period)){
			bmc.RotLevel++;
		}
	}
	
	/// <summary>
	///  updates the activeFloor
	/// </summary>
	void UpdateActiveFloor(){
		if(Input.GetKeyDown(KeyCode.PageUp)){
			bmc.ActiveFloor++;
		}
		if(Input.GetKeyDown(KeyCode.PageDown)){
			bmc.ActiveFloor--;
		}
	}

	/// <summary>
	/// updates the camera rotation based on input from the arrow keys
	/// </summary>
	void UpdateCameraRotation(){
		if(Input.GetKey(KeyCode.LeftArrow)){
			Camera.main.transform.RotateAround (new Vector3(50,0,50),Vector3.up,10.0f * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.RightArrow)){
			Camera.main.transform.RotateAround (new Vector3(50,0,50),Vector3.down,10.0f * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.UpArrow)){
			Camera.main.transform.Rotate(new Vector3(Time.deltaTime * 7.5f * Camera.main.transform.rotation.x,0.0f,0.0f));
		}
		if(Input.GetKey(KeyCode.DownArrow)){
			Camera.main.transform.Rotate(new Vector3(-Time.deltaTime * 7.5f * Camera.main.transform.rotation.x,0.0f,0.0f));
		}
	}

	/// <summary>
	/// updates whether an object is being deleted
	/// </summary>
	void UpdateDeletion(){
		if(Input.GetKeyDown(KeyCode.Delete)){
			bmc.Delete();
		}
	}

	/// <summary>
	/// updates the toggle of the grid
	/// </summary>
	void UpdateGrid(){
		if(Input.GetKeyDown(KeyCode.G)){
			bmc.ToggleGrid();
		}
	}

	void UpdateMoveNeighbors(){
		if(Input.GetKeyDown(KeyCode.M)){
			Object.moveNeighbors = !Object.moveNeighbors;
		}
	}

	void UpdateActiveColorPicker(){
		if(Input.GetKeyDown(KeyCode.I)){
			ColorPicker script = colorPicker.GetComponent<ColorPicker>();
			if(ToggleColorState.GetActive() == null){
				GameObject go = PoolingSystem.Spawn(mc.buttonPrefab,new Vector3(35f,584.5f,0f),Quaternion.identity);
				go.transform.SetParent(mc.buttonToggleParent.transform,true);
				mc.buttonToggles.Add(go);
				go.transform.GetChild(0).GetComponent<ToggleColorState>().SetActive();
				go.transform.GetChild(0).GetComponent<ToggleColorState>().SetConnectedObj(null);
			}
			if(script.menuOn == false){
				colorPicker.SetActive(true);
				script.menuOn = true;
			}
		}
		if(Input.GetKeyDown(KeyCode.Escape)){
			ColorPicker script = colorPicker.GetComponent<ColorPicker>();
			if(script.menuOn == true){
				script.menuOn = false;
				script.isOn = !script.isOn;
				colorPicker.SetActive(false);
			}
		}
	}
}