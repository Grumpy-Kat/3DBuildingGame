using UnityEngine;
using System.Collections.Generic;

//TableType just for Tables, do not include Objects!
public enum ChairType {Default,CheapChair,BarChair,DeskChair};

//Tables, such as dining room tables, end tables, and kitchen tables
public class Chair : Object {
	
	new ChairType type;
	
	/// <summary>
	/// initializes a new instance of the <see cref="Chair"/> class (constructer)
	/// </summary>
	/// <param name="type">Type of base Object, which is always Chair</param>
	/// <param name="prefab">Prefab of the Table</param>
	/// <param name="chairType">ChairType</param>
	/// <param name="defaultY">default Y coordinate of the object</param>
	public Chair(ObjType type,UnityEngine.Object prefab,float defaultY,bool connectsToNeighbors,ChairType chairType) : base(type,prefab,defaultY,connectsToNeighbors){
		this.type = chairType;
		this.prefab = prefab;
		this.defaultY = defaultY;
		this.connectsToNeighbors = connectsToNeighbors;
	}
	
	/// <summary>
	/// updates the info of the object, and it's surrounding tiles,
	/// and checks to make sure the position that the object is attempting to be placed at is valid
	/// </summary>
	/// <returns>Table that is placed, or null if it's an invalid position</returns>
	/// <param name="obj">Chair that is trying to be placed</param>
	/// <param name="pos">position that the object is trying to be placed at</param>
	/// <param name="rotLevel">rotLevel, which helps to determine the position and rotation of the object</param>
	public static Chair PlaceObject(Chair proto,Vector3 pos,int rotLevel){
		BuildModeController bmc = WorldController.Instance.MouseController.bmc;
		Chair objPlaced = new Chair(ObjType.Chair,proto.prefab,proto.defaultY,proto.connectsToNeighbors,proto.type);
		Tile t = WorldController.Instance.World.GetTileAt(Mathf.RoundToInt(pos.x),Mathf.RoundToInt(pos.z),bmc.ActiveFloor);
		objPlaced.diffPos = new Vector3(0,0,0);
		if(rotLevel == 0){
			//objPlaced.pos = new Vector3(t.x,proto.defaultY + (2.75f * bmc.ActiveFloor),t.y + 0.2f);
			objPlaced.pos = new Vector3(t.x,proto.defaultY,t.y + 0.2f);
		} else if(rotLevel == 1){
			//objPlaced.pos = new Vector3(t.x + 0.2f,proto.defaultY + (2.75f * bmc.ActiveFloor),t.y);
			objPlaced.pos = new Vector3(t.x + 0.2f,proto.defaultY,t.y);
		} else if(rotLevel == 2){
			//objPlaced.pos = new Vector3(t.x,proto.defaultY + (2.75f * bmc.ActiveFloor),t.y - 0.2f);
			objPlaced.pos = new Vector3(t.x,proto.defaultY,t.y - 0.2f);
		} else if(rotLevel == 3){
			//objPlaced.pos = new Vector3(t.x - 0.2f,proto.defaultY + (2.75f * bmc.ActiveFloor),t.y);
			objPlaced.pos = new Vector3(t.x - 0.2f,proto.defaultY,t.y);
		}
		Quaternion rot = Quaternion.identity;
		rot.eulerAngles = new Vector3(0f,90f * rotLevel,0f);
		objPlaced.rot = rot;
		foreach(Object otherObj in t.objs){
			if(otherObj.type != ObjType.Wall){
				Debug.LogError("PlaceObject: Trying to place an Chair where there is furniture.");
				return null;
			}
		}
		if(t.Type == TileType.Empty){
			Debug.LogError("PlaceObject: Trying to assign a Chair to an empty tile!");
			return null;
		}
		t.objs.Add(objPlaced);
		proto.tiles.Add(t);
		objPlaced.UpdateNeighbors(objPlaced);
		return objPlaced;
	}

	/// <summary>
	/// updates the info of the object, and it's surrounding tiles,
	/// and checks to make sure the position that the object is attempting to be placed at is valid
	/// </summary>
	/// <returns>Table that is placed, or null if it's an invalid position</returns>
	/// <param name="obj">existing Chair that is trying to be moved</param>
	/// <param name="pos">position that the object is trying to be placed at</param>
	/// <param name="rotLevel">rotLevel, which helps to determine the position and and rotation of the object</param>
	/// <param name="moveNeighbors">moveNeighbors, defaults to true and determines whether the Object's neighbors should be moved</param>
	public static void MoveObject(Chair obj,Vector3 pos,int rotLevel,bool moveNeighbors = true){
		BuildModeController bmc = WorldController.Instance.MouseController.bmc;
		Tile prevT = WorldController.Instance.World.GetTileAt(Mathf.RoundToInt(obj.pos.x),Mathf.RoundToInt(obj.pos.z),bmc.ActiveFloor);
		Tile newT = WorldController.Instance.World.GetTileAt(Mathf.RoundToInt(pos.x),Mathf.RoundToInt(pos.z),bmc.ActiveFloor);
		foreach(Object otherObj in newT.objs){
			if(otherObj.type != ObjType.Wall && otherObj != obj && obj.neighbors.Contains(otherObj) == false){
				Debug.LogError("MoveObject: Trying to place an Object where there is already one");
				return;
			}
		}
		if(newT.Type == TileType.Empty){
			Debug.LogError("MoveObject: Trying to assign an Object to an empty tile!");
			return;
		}
		if(prevT.objs.Contains(obj) == true){
			prevT.objs.Remove(obj);
		}
		if(obj.tiles.Contains(prevT) == true){
			obj.tiles.Remove(prevT);
		}
		Vector3 diffPos = pos - obj.pos;
		diffPos.y = 0;
		obj.diffPos = diffPos;
		if(rotLevel == 0){
			//obj.pos = new Vector3(newT.x,obj.defaultY + (2.75f * bmc.ActiveFloor),newT.y + 0.2f);
			obj.pos = new Vector3(newT.x,obj.defaultY,newT.y + 0.2f);
		} else if(rotLevel == 1){
			//obj.pos = new Vector3(newT.x + 0.2f,obj.defaultY + (2.75f * bmc.ActiveFloor),newT.y);
			obj.pos = new Vector3(newT.x + 0.2f,obj.defaultY,newT.y);
		} else if(rotLevel == 2){
			//obj.pos = new Vector3(newT.x,obj.defaultY + (2.75f * bmc.ActiveFloor),newT.y - 0.2f);
			obj.pos = new Vector3(newT.x,obj.defaultY,newT.y - 0.2f);
		} else if(rotLevel == 3){
			//obj.pos = new Vector3(newT.x - 0.2f,obj.defaultY + (2.75f * bmc.ActiveFloor),newT.y);
			obj.pos = new Vector3(newT.x - 0.2f,obj.defaultY,newT.y);
		}
		if(obj.connectsToNeighbors == true && Object.moveNeighbors == true && moveNeighbors == true){
			obj.MoveNeighbors(obj);
		}
		Quaternion rot = Quaternion.identity;
		rot.eulerAngles = new Vector3(0f,90f * rotLevel,0f);
		obj.rot = rot;
		newT.objs.Add(obj);
		obj.tiles.Add(newT);
		if(obj.cbObjectChanged != null){
			obj.cbObjectChanged(obj);
		}
	}
	
	/// <summary>
	/// updates the info of the object, and it's surrounding tiles,
	/// and checks to make sure the position that the object is attempting to be placed at is valid
	/// </summary>
	/// <param name="obj">Object that is trying to be deleted</param>
	/// <param name="deleteNeighbors">deleteNeighbors, defaults to true and determines whether the Object's neighbors should be deleted</param>
	public static void DeleteObject(Chair obj,bool deleteNeighbors = true){
		obj.isDeleted = true;
		foreach(Tile t in obj.tiles){
			if(t.objs.Contains(obj) == true){
				t.objs.Remove(obj);
			}
			if(obj.tiles.Contains(t) == true){
				obj.tiles.Remove(t);
			}
		}
		obj.diffPos = new Vector3(0,0,0);
		if(obj.cbObjectChanged != null){
			obj.cbObjectChanged(obj);
		}
		if(obj.connectsToNeighbors == true && Object.moveNeighbors == true && deleteNeighbors == true){
			obj.DeleteNeighbors(obj);
		}
		obj.UpdateNeighbors(obj);
	}
}