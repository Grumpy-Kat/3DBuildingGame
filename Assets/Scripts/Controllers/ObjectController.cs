using UnityEngine;
using System.Collections.Generic;

public class ObjectController : MonoBehaviour {
	
	//public references
	public static ObjectController Instance { get; protected set; }
	//references to other controllers
	MouseController mc;
	//maps of Object instance to GameObject
	public Dictionary<Object,GameObject> objGameObjMap;
	//parent objects
	public GameObject[] objParent;
	//world instance
	World world {
		get{
			return WorldController.Instance.World;
		}
	}
	//boolean flags
	bool neighborsChanged = false;
	//temporary variables checking previous values
	public Vector3 prevDiffPos;
	public int prevRotLevel;
	
	void Start(){
		if (Instance != null) {
			Debug.LogError("ObjectController: There should only ever be one ObjectController.");
		}
		Instance = this;
		//instantiate the map
		objGameObjMap = new Dictionary<Object,GameObject>();
		//updates GameObject whenever the Furniture changes using callbacks
		world.RegisterObjectCreated(OnObjectCreated);
		//go through each existing furniture, most likely loaded in from a save file, and call the OnFurnitureCreated event manually
		foreach(Object obj in world.objects){
			OnObjectCreated(obj);
		}
		mc = GameObject.FindObjectOfType<MouseController>();
		InvokeRepeating("ClearUnusedEmptyObjects",1f,1f);
		objParent = new GameObject[5];
	}

	/// <summary>
	/// called whenever a Object is created, creates a GameObject, and adds a Sprite
	/// </summary>
	/// <param name="obj">Object, which holds the info needed to be applied</param>
	public void OnObjectCreated(Object obj){
		//create a visual GameObject with the provided data
		//FIXME: Does not conisder multi-tile objects
		//create a GameObject
		GameObject obj_go = (GameObject)Instantiate(obj.prefab,obj.pos,obj.rot);
		//add Tile and GameObject pair to dictionary
		objGameObjMap.Add(obj,obj_go);
		//add properties to GameObject based on the data's properties
		//obj_go.name = obj.type.ToString() + "_" + obj.pos.x + "_" + obj.pos.z;
		obj_go.name = obj.type.ToString() + "_";
		obj_go.tag = "Object";
		for(int i = 0;i < obj_go.transform.childCount;i++){
			obj_go.transform.GetChild(i).gameObject.tag = "Object";
		}
		obj.SetColor(obj_go.GetComponentInChildren<Renderer>().material.color);
		//create parent
		GameObject empty_go = new GameObject();
		empty_go.name = obj.type.ToString();
		empty_go.transform.SetParent(objParent[mc.bmc.ActiveFloor].transform,false);
		obj_go.transform.SetParent(empty_go.transform,false);
		//check if object is enclosing a new room
		if(obj.connectsToNeighbors){
			Room.RoomFloodFill(obj);
		}
		//register object for actions
		obj.RegisterObjectChanged(OnObjectChanged);
		obj.RegisterNeighborsChanged(OnNeighborsChanged);
		obj.RegisterColorChanged(OnColorChanged);
	}
	
	/// <summary>
	/// called whenever a change occurs to a Object
	/// </summary>
	/// <param name="obj">Object, which holds the info needed to be applied</param>
	public void OnObjectChanged(Object obj){
		//change GameObjects's position and rotation in the world based on the obj's data
		if(objGameObjMap.ContainsKey(obj) == false){
			Debug.Log ("OnObjectChanged - trying to change visuals for object not mapped!");
			return;
		}
		GameObject obj_go = objGameObjMap[obj];
		if(obj.isDeleted == true){
			objGameObjMap.Remove(obj);
			Destroy(obj_go);
			return;
		}
		obj_go.transform.position = obj.pos;
		obj_go.transform.rotation = obj.rot;
		//check if object is enclosing a new room
		if(obj.connectsToNeighbors){
			Room.RoomFloodFill(obj);
		}
	}

	/// <summary>
	/// called whenever a change occurs to a Object, so it will update it's neighbors
	/// </summary>
	/// <param name="obj">Object, which needs it's neighbors updates</param>
	public void OnNeighborsChanged(Object obj){
		if(obj.connectsToNeighbors == false){
			obj.neighbors.Clear();
			return;
		}
		Object[] objNeighbors = obj.neighbors.ToArray();
		foreach(Object n in objNeighbors){
			if(n.isDeleted == true){
				obj.neighbors.Remove(n);
			}
		}
		if(obj.neighbors.Contains(obj) == false){
			obj.neighbors.Add(obj);
		}
		if(WorldController.Instance.GetTileAtWoorldCoord(obj.pos) != null){
			if(obj.GetRotLevel() % 2 == 0){
				if(WorldController.Instance.GetTileAtWoorldCoord(obj.pos).x+1 < world.width){
					Tile t = world.GetTileAt(WorldController.Instance.GetTileAtWoorldCoord(obj.pos).x + 1,WorldController.Instance.GetTileAtWoorldCoord(obj.pos).y,mc.bmc.ActiveFloor);
					if(t != null){
						foreach(Object otherObj in t.objs){
							if(otherObj.type == obj.type && otherObj.pos == new Vector3(obj.pos.x + 1,obj.pos.y,obj.pos.z) && obj.neighbors.Contains(otherObj) == false){
								obj.neighbors.Add(otherObj);
								Object[] temp = obj.neighbors.ToArray();
								foreach(Object n in temp){
									AddNeighbors(obj,otherObj,n);
								}
							}
						}
					}
				}
				if(WorldController.Instance.GetTileAtWoorldCoord(obj.pos).x-1 > 0){
					Tile t = world.GetTileAt(WorldController.Instance.GetTileAtWoorldCoord(obj.pos).x - 1,WorldController.Instance.GetTileAtWoorldCoord(obj.pos).y,mc.bmc.ActiveFloor);
					if(t != null){
						foreach(Object otherObj in t.objs){
							if(otherObj.type == obj.type && otherObj.pos == new Vector3(obj.pos.x - 1,obj.pos.y,obj.pos.z) && obj.neighbors.Contains(otherObj) == false){
								obj.neighbors.Add(otherObj);
								Object[] temp = obj.neighbors.ToArray();
								foreach(Object n in temp){
									AddNeighbors(obj,otherObj,n);
								}
							}
						}
					}
				}
			} else {
				if(WorldController.Instance.GetTileAtWoorldCoord(obj.pos).y+1 < world.height){
					Tile t = world.GetTileAt(WorldController.Instance.GetTileAtWoorldCoord(obj.pos).x,WorldController.Instance.GetTileAtWoorldCoord(obj.pos).y + 1,mc.bmc.ActiveFloor);
					if(t != null){
						foreach(Object otherObj in t.objs){
							if(otherObj.type == obj.type && otherObj.pos == new Vector3(obj.pos.x,obj.pos.y,obj.pos.z + 1) && obj.neighbors.Contains(otherObj) == false){
								obj.neighbors.Add(otherObj);
								Object[] temp = obj.neighbors.ToArray();
								foreach(Object n in temp){
									AddNeighbors(obj,otherObj,n);
								}
							}
						}
					}
				}
				if(WorldController.Instance.GetTileAtWoorldCoord(obj.pos).y-1 > 0){
					Tile t = world.GetTileAt(WorldController.Instance.GetTileAtWoorldCoord(obj.pos).x,WorldController.Instance.GetTileAtWoorldCoord(obj.pos).y - 1,mc.bmc.ActiveFloor);
					if(t != null){
						foreach(Object otherObj in t.objs){
							if(otherObj.type == obj.type && otherObj.pos == new Vector3(obj.pos.x,obj.pos.y,obj.pos.z - 1) && obj.neighbors.Contains(otherObj) == false){
								obj.neighbors.Add(otherObj);
								Object[] temp = obj.neighbors.ToArray();
								foreach(Object n in temp){
									AddNeighbors(obj,otherObj,n);
								}
							}
						}
					}
				}
			}
		}
		foreach(Object n in obj.neighbors){
			if(objGameObjMap.ContainsKey(obj) == false){
				Debug.Log ("OnNeighborsChanged - trying to change visuals for object not mapped!");
				return;
			}
			GameObject obj_go = objGameObjMap[obj];
			if(objGameObjMap.ContainsKey(n) == false){
				Debug.Log ("OnNeighborsChanged - trying to change visuals for object not mapped!");
				return;
			}
			GameObject n_go = objGameObjMap[n];
			if(n_go.transform.parent == obj_go.transform.parent){
				continue;
			}
			n_go.transform.SetParent(obj_go.transform.parent,true);
			n.UpdateNeighbors(n);
			neighborsChanged = true;
		}
	}

	/// <summary>
	/// moves the object's neighbors visually
	/// </summary>
	/// <param name="obj">Object to move</param>
	public void MoveNeighbors(Object obj){
		//check for position change
		if(prevDiffPos == obj.diffPos && prevRotLevel == obj.GetRotLevel()){
			if(obj.neighbors.Count > 0){
				Object.MoveObject(obj,obj.pos - obj.neighbors[0].diffPos,obj.GetRotLevel(),false);
			}
			return;
		}
		//get GameObject of obj
		if(objGameObjMap.ContainsKey(obj) == false){
			Debug.Log ("OnNeighborsChanged - trying to change visuals for object not mapped!");
			return;
		}
		GameObject obj_go = objGameObjMap[obj];
		//check for rotation change
		if(prevRotLevel != obj.GetRotLevel()){
			if(obj.GetRotLevel()%2 == 1){
				for(int i=0;i<obj_go.transform.parent.childCount;i++){
						Vector3 newPos = new Vector3(obj.pos.x,obj.pos.y,obj.pos.z + i);
						Object.MoveObject(obj.neighbors[i],newPos,obj.GetRotLevel(),false);
				}
				/*for(int i=0;i<Mathf.CeilToInt(obj_go.transform.parent.childCount / 2f);i++){
					if(obj.neighbors[i] != null && obj.neighbors[i] != obj){
						Debug.Log(i);
						Vector3 newPos = new Vector3(obj.pos.x + 1,obj.pos.y,obj.pos.z + i);
						//Debug.Log("ObjPos: " + obj.pos + " | NPrevPos: " + obj.neighbors[i].pos + " | NNewPos: " + newPos);
						Object.MoveObject(obj.neighbors[i],newPos,obj.GetRotLevel(),false);
						GameObject n_go = objGameObjMap[obj.neighbors[i]];
						n_go.name = i.ToString();
					}
					if(obj.neighbors[i * 2] != null && obj.neighbors[i * 2] != obj){
						Vector3 newPos = new Vector3(obj.pos.x + 1,obj.pos.y,obj.pos.z - i);
						//Debug.Log("ObjPos: " + obj.pos + " | NPrevPos: " + obj.neighbors[i].pos + " | NNewPos: " + newPos);
						Object.MoveObject(obj.neighbors[i * 2],newPos,obj.GetRotLevel(),false);
						GameObject n_go = objGameObjMap[obj.neighbors[i]];
						n_go.name = (i * 2).ToString();
					}
				}
				if(Mathf.CeilToInt(obj_go.transform.parent.childCount / 2f)%2 == 0){
					if(obj.neighbors[obj_go.transform.parent.childCount - 1] != null && obj.neighbors[obj_go.transform.parent.childCount - 1] != obj){
						Vector3 newPos = new Vector3(obj.pos.x + 1,obj.pos.y,obj.pos.z + (obj_go.transform.parent.childCount - 1));
						//Debug.Log("ObjPos: " + obj.pos + " | NPrevPos: " + obj.neighbors[i].pos + " | NNewPos: " + newPos);
						Object.MoveObject(obj.neighbors[obj_go.transform.parent.childCount - 1],newPos,obj.GetRotLevel(),false);
						//GameObject n_go = objGameObjMap[obj.neighbors[i]];
						//n_go.name = i.ToString();
					}
				}*/
				obj_go.transform.parent.rotation = obj.rot;
				prevRotLevel = obj.GetRotLevel();
			} else {
				for(int i=0;i<obj_go.transform.parent.childCount;i++){
					Vector3 newPos = new Vector3(obj.pos.x + i,obj.pos.y,obj.pos.z);
					Object.MoveObject(obj.neighbors[i],newPos,obj.GetRotLevel(),false);
				}
				obj_go.transform.parent.rotation = obj.rot;
				prevRotLevel = obj.GetRotLevel();
			}
		}
		//update change variables
		prevDiffPos = obj.diffPos;
		prevRotLevel = obj.GetRotLevel();
		//update position
		obj_go.transform.parent.position += obj.diffPos;
		for(int i=0;i<obj_go.transform.parent.childCount;i++){
			if(obj.neighbors[i] != obj){
				Object.MoveObject(obj.neighbors[i],obj.neighbors[i].pos + obj.diffPos,obj.neighbors[i].GetRotLevel(),false);
			}
		}
		if(obj.neighbors.Count > 0){
			//Debug.Log("Before | N: " + obj.neighbors[0].diffPos + " - Org: " + obj.diffPos + " - Equals: " + (obj.neighbors[0].diffPos == obj.diffPos));
			//Object.MoveObject(obj,obj.pos - obj.neighbors[0].diffPos,(int)(obj.rot.eulerAngles.y / 90),false);
			//Debug.Log("After | N: " + obj.neighbors[0].diffPos + " - Org: " + obj.diffPos + " - Equals: " + (obj.neighbors[0].diffPos == obj.diffPos));
		} else {
			Object.MoveObject(obj,obj.pos - obj.diffPos,obj.GetRotLevel(),false);
		}
		obj.ClearDiffPos();
	}

	/// <summary>
	/// deletes the object's neighbors visually
	/// </summary>
	/// <param name="obj">Object to delete</param>
	public void DeleteNeighbors(Object obj){
		foreach(Object n in obj.neighbors){
			Object.DeleteObject(n,false);
			if(objGameObjMap.ContainsKey(n)){
				objGameObjMap.Remove(n);
			}
		}
		if(objGameObjMap.ContainsKey(obj)){
			objGameObjMap.Remove(obj);
		}
		mc.currSelectedGameObj = null;
		mc.currSelectedObj = null;
	}

	void OnColorChanged(Object obj){
		if(obj.connectsToNeighbors == true){
			foreach(Object n in obj.neighbors){
				//change color of neighbor in data
				n.SetColor(obj.color,false);
				//get GameObject of neighbor
				if(objGameObjMap.ContainsKey(n) == false){
					Debug.Log ("OnNeighborsChanged - trying to change visuals for object not mapped!");
					return;
				}
				GameObject n_go = objGameObjMap[n];
				//change color of GameObject
				Renderer[] renderers = n_go.GetComponentsInChildren<Renderer> ();
				foreach(Renderer renderer in renderers){
					renderer.material.color = obj.color;
				}
			}
		} else {
			//get GameObject of object
			if(objGameObjMap.ContainsKey(obj) == false){
				Debug.Log ("OnNeighborsChanged - trying to change visuals for object not mapped!");
				return;
			}
			GameObject obj_go = objGameObjMap[obj];
			//change color of GameObject
			Renderer[] renderers = obj_go.GetComponentsInChildren<Renderer> ();
			foreach(Renderer renderer in renderers){
				renderer.material.color = obj.color;
			}
		}
	}

	/// <summary>
	/// clears the parent objects of the walls with no children (probably moved by neighbors)
	/// </summary>
	void ClearUnusedEmptyObjects(){
		if(neighborsChanged == true){
			for(int i=0;i<objParent[mc.bmc.ActiveFloor].transform.childCount;i++){
				if(objParent[mc.bmc.ActiveFloor].transform.GetChild(i).childCount < 1){
					GameObject.Destroy(objParent[mc.bmc.ActiveFloor].transform.GetChild(i).gameObject);
				}
			}
			neighborsChanged = false;
		}
	}

	/// <summary>
	/// recursive function that adds the other object's neighbors to the object's neighbors
	/// </summary>
	/// <param name="obj">orginal object</param>
	/// <param name="otherObj">other object (probably a neighbor of the object)</param>
	/// <param name="n">object's neighbor</param>
	void AddNeighbors(Object obj,Object otherObj,Object n){
		//Debug.Log("being added - object: " + obj.pos + " | neighborAddingTo: " + otherObj.pos + " | neighborGettingAdded: " + n.pos);
		if(otherObj.neighbors.Contains(n) == true){
			//Debug.Log("Failed: ")
			return;
		}
		//Debug.Log("added");
		otherObj.neighbors.Add(n);
		/*foreach(Object childN in otherObj.neighbors){
			AddNeighbors(obj,otherObj,childN);
		}*/
		Object[] temp = otherObj.neighbors.ToArray();
		foreach(Object childN in temp){
			AddNeighbors(obj,n,childN);
		}
	}
}