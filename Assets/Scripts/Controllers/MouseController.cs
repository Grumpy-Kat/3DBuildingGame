using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using System.Linq;

public class MouseController : MonoBehaviour {
	//reference to other controllers
	public BuildModeController bmc { get; protected set; }
	ObjectController oc;
	TileController tc;
	//mouse position vars
	public Vector3 currFramePos { get; protected set; }
	Vector3 lastFramePos;
	//dragging vars
	Vector3 dragStartPos;
	bool isDragging;
	//moving vars
	Vector3 attemptedObjPos;
	public GameObject currSelectedGameObj;
	public Object currSelectedObj;
	public GameObject currSelectedTileGameObj;
	int prevCurrSelectedObjRot;
	public bool isMoving;
	//dragging/cursor vars
	public GameObject cursorPrefab;
	List<GameObject> dragPreviews;
	GameObject dragPreviewParent;
	public GameObject buttonPrefab;
	public List<GameObject> buttonToggles { get; protected set; }
	public GameObject buttonToggleParent { get; protected set; }

	void Start(){
		bmc = GameObject.FindObjectOfType<BuildModeController>();
		oc = GameObject.FindObjectOfType<ObjectController>();
		tc = GameObject.FindObjectOfType<TileController>();
		dragPreviews = new List<GameObject>();
		dragPreviewParent = GameObject.Find ("BuildingPreview");
		buttonToggles = new List<GameObject>();
		buttonToggleParent = GameObject.FindObjectOfType<ColorPicker>().gameObject;
		PoolingSystem.Preload(buttonPrefab,1);
	}

	void Update(){
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray,out hit,100f)){
			currFramePos = hit.point;
		}
		Debug.Log(currFramePos);
		UpdateTileDrag();
		UpdateCameraDrag();
		UpdateCameraZoom();
		UpdateObjectMovement();
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray,out hit,100)){
			lastFramePos = hit.point;
		}
	}

	public Tile GetMouseOverTile() {
		return WorldController.Instance.World.GetTileAt(
			Mathf.RoundToInt(currFramePos.x), 
			Mathf.RoundToInt(currFramePos.z),
			bmc.ActiveFloor
		);
	}

	/// <summary>
	/// updates info when the mouse is dragging to call functions to place Objects and update TileTypes
	/// </summary>
	void UpdateTileDrag(){
		//stop if moving existing Objects
		if(isMoving){
			return;
		}
		//stop if over UI
		if(EventSystem.current.IsPointerOverGameObject() && !isDragging){
			return;
		}
		//start drag
		if (Input.GetMouseButtonDown (0)) {//left mouse button
			dragStartPos = currFramePos;
			isDragging = true;
		}
		//x vars
		float start_x = dragStartPos.x;
		float end_x = currFramePos.x;
		if(end_x < start_x){
			float temp = end_x;
			end_x = start_x;
			start_x = temp;
		}
		//y vars
		float start_y = dragStartPos.z;
		float end_y = currFramePos.z;
		if(end_y < start_y){
			float temp = end_y;
			end_y = start_y;
			start_y = temp;
		}
		//remove old drag previews
		while(dragPreviews.Count > 0){
			GameObject go = dragPreviews[0];
			dragPreviews.RemoveAt(0);
			PoolingSystem.Despawn(go);
		}	
 		//display a preview of the drag area
		Quaternion rot = Quaternion.identity;
		rot.eulerAngles = new Vector3(90f,0f,0f);
		if(bmc.isTileBuildingMode == false){
			if(bmc.RotLevel%2 == 0){
				end_y = start_y;
			} else if(bmc.RotLevel%2 == 1){
				end_x = start_x;
			}
		}
		if(Input.GetMouseButton(0)){//left mouse button
			for (int x = Mathf.RoundToInt(start_x); x <= Mathf.RoundToInt(end_x); x++) {
				for (int y = Mathf.RoundToInt(start_y); y <= Mathf.RoundToInt(end_y); y++) {
					Tile t = WorldController.Instance.World.GetTileAt(x,y,bmc.ActiveFloor);
					if(t != null){
						//display the building hint on top of the tile pos
						GameObject go = PoolingSystem.Spawn(cursorPrefab,new Vector3(x,2.75f * bmc.ActiveFloor,y),rot);
						go.transform.SetParent(dragPreviewParent.transform,true);
						dragPreviews.Add(go);
					}
				}
			}
		}
		//end drag
		if (Input.GetMouseButtonUp (0)) {//left mouse button
			if(bmc.isTileBuildingMode == true){
				bmc.BuildTile(Mathf.RoundToInt(start_x),Mathf.RoundToInt(end_x),Mathf.RoundToInt(start_y),Mathf.RoundToInt(end_y));
			} else {
				bmc.BuildObject(start_x,end_x,start_y,end_y);
			}
			isDragging = false;
		}
	}
	
	/// <summary>
	/// updates the camera position if the middle mouse button is pressed
	/// </summary>
	void UpdateCameraDrag(){
		if(Input.GetMouseButton(2)){//middle mouse button
			Vector3 diff = lastFramePos - currFramePos;
			Camera.main.transform.Translate(new Vector3(diff.x,diff.z,0.0f));
		}
	}

	/// <summary>
	/// updates the camera zoom level every frame based on the mouse scrollwheel
	/// </summary>
	void UpdateCameraZoom(){
		Camera.main.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * Camera.main.fieldOfView / 2;
		Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView,85,165f);
	}

	/// <summary>
	/// updates whether the user is moving objects
	/// </summary>
	void UpdateObjectMovement(){
		//stop if dragging out new Objects
		if(isDragging){
			return;
		}
		//stop if over UI
		if(EventSystem.current.IsPointerOverGameObject() && !isMoving){
			return;
		}
		//attempt to find object
		if (Input.GetMouseButtonDown (1) && !isMoving) {//right mouse button
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray,out hit,100)){
				Debug.Log(hit.transform.name);
				if(hit.transform.tag == "Object"){
					currSelectedGameObj = hit.transform.gameObject;
					bmc.RotLevel = 0;
					prevCurrSelectedObjRot = (int)currSelectedGameObj.transform.rotation.eulerAngles.y / 90;
					do{
						currSelectedGameObj = currSelectedGameObj.transform.parent.gameObject;
					} while(currSelectedGameObj.transform.IsChildOf(oc.objParent[bmc.ActiveFloor].transform) == false);
					if(oc.objGameObjMap.ContainsValue(currSelectedGameObj)){
						foreach(GameObject go in buttonToggles){
							if(go.activeSelf){
								PoolingSystem.Despawn(go);
							}
						}
						Renderer[] renderers = currSelectedGameObj.GetComponentsInChildren<Renderer> ();
						for(int i = 0;i < renderers.Length;i++){
							GameObject go = PoolingSystem.Spawn(buttonPrefab,new Vector3(35f,584.5f - (30f * i),0f),Quaternion.identity);
							go.transform.SetParent(buttonToggleParent.transform,true);
							buttonToggles.Add(go);
							go.transform.GetChild(0).GetComponent<ToggleColorState>().SetActive();
							go.transform.GetChild(0).GetComponent<ToggleColorState>().SetConnectedObj(renderers[i].gameObject);
						}
					}
				} else if(hit.transform.tag == "Tile"){
					GameObject hitObj = hit.transform.gameObject;
					if(tc.tileGameObjMap.ContainsValue(hitObj)){
						if(tc.tileGameObjMap.Keys.ToArray()[tc.tileGameObjMap.Values.ToList().IndexOf(hitObj)].Type != TileType.Empty || tc.tileGameObjMap.Keys.ToArray()[oc.objGameObjMap.Values.ToList().IndexOf(currSelectedGameObj)].Type != TileType.Grid){
							currSelectedTileGameObj = hitObj;
							foreach(GameObject go in buttonToggles){
								if(go.activeSelf){
									PoolingSystem.Despawn(go);
								}
							}
						}
						Renderer[] renderers = hitObj.GetComponents<SpriteRenderer> ();
						for(int i = 0;i < renderers.Length;i++){
							GameObject go = PoolingSystem.Spawn(buttonPrefab,new Vector3(35f,584.5f - (30f * i),0f),Quaternion.identity);
							go.transform.SetParent(buttonToggleParent.transform,true);
							buttonToggles.Add(go);
							go.transform.GetChild(0).GetComponent<ToggleColorState>().SetActive();
							go.transform.GetChild(0).GetComponent<ToggleColorState>().SetConnectedObj(renderers[i].gameObject);
						}
					}
				}
			}
			if(currSelectedGameObj != null){
				oc.prevRotLevel = (int)(currSelectedGameObj.transform.rotation.eulerAngles.y / 90);
				oc.prevDiffPos = new Vector3(0,0,0);
			}
			isMoving = true;
		}
		//move objects
		if(currSelectedGameObj != null){
			if(oc.objGameObjMap.ContainsValue(currSelectedGameObj)){
				currSelectedObj = oc.objGameObjMap.Keys.ToArray()[oc.objGameObjMap.Values.ToList().IndexOf(currSelectedGameObj)];
				int rot = currSelectedObj.GetRotLevel();
				if(rot != prevCurrSelectedObjRot + bmc.RotLevel){
					rot += bmc.RotLevel;
				}
				Object.MoveObject(currSelectedObj,currFramePos,rot);
			} else {
				Debug.Log("UpdateObjectMovement: The objGameObjMap does not contain the current selected object!");
			}
		}
		//place Object
		if (Input.GetMouseButtonUp (0)) {//left mouse button
			if(currSelectedObj != null){
				currSelectedObj.UpdateNeighbors(currSelectedObj);
			}
			currSelectedGameObj = null;
			isMoving = false;
		}
	}
}