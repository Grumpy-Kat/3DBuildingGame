using UnityEngine;
using System;

public class BuildModeController : MonoBehaviour {

	//references to other controllers
	MouseController mc;
	//building mode vars
	int rotLevel;
	public int RotLevel {
		get{
			return rotLevel;
		}
		set{
			if(value < 0){
				rotLevel = 3;
			} else if(value > 3){
				rotLevel = 0;
			} else {
				rotLevel = value;
			}
		}
	}
	int activeFloor;
	public int ActiveFloor {
		get{
			return activeFloor;
		}
		set{
			if(value >= 0 && value < 5){
				activeFloor = value;
				for(int i=0;i < 5;i++){
					if(activeFloor >= i){
						floors[i].SetActive(true);
					} else{
						floors[i].SetActive(false);
					}
				}
				Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,3f + (2.75f * activeFloor),Camera.main.transform.position.z);
			}
		}
	}
	public bool isTileBuildingMode { get; protected set; }
	string objMode;
	TileType tileMode;
	//grid vars
	bool gridOn;
	//floors
	public GameObject[] floors;

	void Start(){
		isTileBuildingMode = true;
		gridOn = false;
		mc = GameObject.FindObjectOfType<MouseController>();
		floors = new GameObject[5];
	}

	/// <summary>
	/// called to start Tiles by changing there TileType
	/// </summary>
	/// <param name="start_x">start_x, which is the x coordinate that the mouse click started</param>
	/// <param name="end_x">end_x, which is the x coordinate that the mouse click ended</param>
	/// <param name="start_y">start_y, which is the y coordinate that the mouse click started</param>
	/// <param name="end_y">end_y, which is the y coordinate that the mouse click ended</param>
	public void BuildTile(int start_x,int end_x,int start_y,int end_y){
		if(tileMode != null){ 
			for (int x = start_x; x <= end_x; x++) {
				for (int y = start_y; y <= end_y; y++) {
					Tile t = WorldController.Instance.World.GetTileAt(x,y,ActiveFloor);
					if(t != null){
						t.RotLevel = RotLevel;
						t.Type = tileMode;
					}
				}
			}
		}
	}
	
	/// <summary>
	/// called to start building Objects by calling functions to update their data and instantiating them
	/// </summary>
	/// <param name="start_x">start_x, which is the x coordinate that the mouse click started</param>
	/// <param name="end_x">end_x, which is the x coordinate that the mouse click ended</param>
	/// <param name="start_y">start_y, which is the y coordinate that the mouse click started</param>
	/// <param name="end_y">end_y, which is the y coordinate that the mouse click ended</param>
	public void BuildObject(float start_x,float end_x,float start_y,float end_y){
		if(objMode != null){
			if(RotLevel%2 == 0){
				start_x = Mathf.RoundToInt(start_x);
				end_x = Mathf.RoundToInt(end_x);
				end_y = start_y;
			} else if(RotLevel%2 == 1){
				start_y = Mathf.RoundToInt(start_y);
				end_y = Mathf.RoundToInt(end_y);
				end_x = start_x;
			}
			for (float x = start_x; x <= end_x; x+=1) {
				for (float y = start_y; y <= end_y; y+=1) {
					if(WorldController.Instance.World.stringObjectMap.ContainsKey(objMode) == false){
						Debug.LogError("Update: Missing Key in stringObjectMap!");
						return;
					}
					Object obj = WorldController.Instance.World.stringObjectMap[objMode];
					WorldController.Instance.World.PlaceObject(obj,new Vector3(x,obj.defaultY + (2.75f * ActiveFloor),y),RotLevel);
				}
			}
		}
	}

	/// <summary>
	/// toggles the tile grid
	/// </summary>
	public void ToggleGrid(){
		for(int i = 0;i<WorldController.Instance.World.width;i++){
			for(int j = 0;j<WorldController.Instance.World.height;j++){
				if(gridOn == false){
					if(WorldController.Instance.World.GetTileAt(i,j,ActiveFloor).Type == TileType.Empty){
						WorldController.Instance.World.GetTileAt(i,j,ActiveFloor).Type = TileType.Grid;
					}
				} else {
					if(WorldController.Instance.World.GetTileAt(i,j,ActiveFloor).Type == TileType.Grid){
						WorldController.Instance.World.GetTileAt(i,j,ActiveFloor).Type = TileType.Empty;
					}
				}
			}
		}
		gridOn = !gridOn;
		if(gridOn == false && isTileBuildingMode == true && tileMode == TileType.Grid){
			tileMode = TileType.Empty;
		} else if(gridOn == true && isTileBuildingMode == true && tileMode == TileType.Empty){
			tileMode = TileType.Grid;
		}
	}

	/// <summary>
	/// helper function that subtracts two rotLevels (and takes in accound all possible cases) and returns how many times something needs to be rotated
	/// </summary>
	/// <returns>rotLevel difference, in terms of how many times something needs to be rotated</returns>
	/// <param name="rotLevel1">first rotLevel</param>
	/// <param name="rotLevel2">second rotLevel</param>
	public static int SubtractRotLevel(int rotLevel1,int rotLevel2){
		if(rotLevel2 > rotLevel1){
			rotLevel2 += 4;
		}
		return rotLevel2 - rotLevel1;
	}

	/// <summary>
	/// delete an object
	/// </summary>
	public void Delete(){
		Object.DeleteObject(mc.currSelectedObj);
		mc.currSelectedGameObj = null;
		mc.isMoving = false;
	}

	/// <summary>
	/// sets the TileType to build
	/// </summary>
	/// <param name="tileType">TileType to build</param>
	public void SetTileMode(string tileType){
		isTileBuildingMode = true;
		RotLevel = 0;
		tileType = tileType.ToLower();
		if(tileType == "Empty".ToLower() || tileType == "Grid".ToLower()){
			if(gridOn == true){
				tileMode = TileType.Grid;
				return;
			} else {
				tileMode = TileType.Empty;
				return;
			}
		}
		foreach(TileType type in Enum.GetValues(typeof(TileType))){
			if(tileType == type.ToString().ToLower()){
				tileMode = type;
			}
		}
	}

	/// <summary>
	/// sets the ObjectType, or any of the subclass types, to build
	/// </summary>
	/// <param name="objType">ObjectType to build</param>
	public void SetObjMode(string objType){
		isTileBuildingMode = false;
		objMode = objType;
		RotLevel = 0;
	}
}