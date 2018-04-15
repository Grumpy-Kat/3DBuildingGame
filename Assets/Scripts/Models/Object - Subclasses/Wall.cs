using UnityEngine;
using System.Collections.Generic;

//WallType just for wall, do not include Objects!
public enum WallType {Default,PlainWall,BrickWall,TileWall};

//Objects that go on the edge of the tile, like walls
public class Wall : Object {

	new WallType type;

	/// <summary>
	/// initializes a new instance of the <see cref="Wall"/> class (constructer)
	/// </summary>
	/// <param name="type">Type of base Object, which is always Wall</param>
	/// <param name="prefab">Prefab of the Wall</param>
	/// <param name="wallType">WallType</param>
	/// <param name="defaultY">default Y coordinate of the object</param>
	public Wall(ObjType type,UnityEngine.Object prefab,float defaultY,bool connectsToNeighbors,WallType wallType) : base(type,prefab,defaultY,connectsToNeighbors){
		this.type = wallType;
		this.prefab = prefab;
		this.defaultY = defaultY;
		this.connectsToNeighbors = connectsToNeighbors;
	}

	/// <summary>
	/// updates the info of the object, and it's surrounding tiles,
	/// and checks to make sure the position that the object is attempting to be placed at is valid
	/// </summary>
	/// <returns>Wall that is placed, or null if it's an invalid position</returns>
	/// <param name="obj">Wall that is trying to be placed</param>
	/// <param name="pos">position that the object is trying to be placed at</param>
	/// <param name="rotLevel">rotLevel, which helps to determine the position and rotation of the object</param>
	public static Wall PlaceObject(Wall proto,Vector3 pos,int rotLevel = 0){
		Wall objPlaced = new Wall(ObjType.Wall,proto.prefab,proto.defaultY,proto.connectsToNeighbors,proto.type);
		BuildModeController bmc = WorldController.Instance.MouseController.bmc;
		Tile t = WorldController.Instance.World.GetTileAt(Mathf.RoundToInt(pos.x),Mathf.RoundToInt(pos.z),bmc.ActiveFloor);
		Tile t2 = t;
		objPlaced.diffPos = new Vector3(0,0,0);
		Quaternion rot = Quaternion.identity;
		rot.eulerAngles = new Vector3(0f,90f * rotLevel,0f);
		if(rotLevel%2 == 0){
			if((pos.z+1)%1 >= 0.5){
				t2 = WorldController.Instance.World.GetTileAt(Mathf.FloorToInt(pos.x),Mathf.FloorToInt(pos.z),bmc.ActiveFloor);
				//objPlaced.pos = new Vector3(t.x,proto.defaultY + (2.75f * bmc.ActiveFloor),t.y - (t.height / 2));
				objPlaced.pos = new Vector3(t.x,proto.defaultY,t.y - (t.height / 2));
			} else {
				t2 = WorldController.Instance.World.GetTileAt(Mathf.CeilToInt(pos.x),Mathf.CeilToInt(pos.z),bmc.ActiveFloor);
				//objPlaced.pos = new Vector3(t.x,proto.defaultY + (2.75f * bmc.ActiveFloor),t.y + (t.height / 2));
				objPlaced.pos = new Vector3(t.x,proto.defaultY,t.y + (t.height / 2));
			}
		} else if(rotLevel%2 == 1){
			if((pos.x+1)%1 >= 0.5){
				t2 = WorldController.Instance.World.GetTileAt(Mathf.FloorToInt(pos.x),Mathf.FloorToInt(pos.z),bmc.ActiveFloor);
				//objPlaced.pos = new Vector3(t.x - (t.width / 2),proto.defaultY + (2.75f * bmc.ActiveFloor),t.y);
				objPlaced.pos = new Vector3(t.x - (t.width / 2),proto.defaultY,t.y);
			} else {
				t2 = WorldController.Instance.World.GetTileAt(Mathf.CeilToInt(pos.x),Mathf.CeilToInt(pos.z),bmc.ActiveFloor);
				//objPlaced.pos = new Vector3(t.x + (t.width / 2),proto.defaultY + (2.75f * bmc.ActiveFloor),t.y);
				objPlaced.pos = new Vector3(t.x + (t.width / 2),proto.defaultY,t.y);
			}
		}
		objPlaced.rot = rot;
		if(t != null){
			foreach(Object otherObj in t.objs){
				if(otherObj.pos == objPlaced.pos){
					Debug.LogError("PlaceObject: Trying to place an Wall where there is already furniture.");
					return null;
				}
			}
		}
		if(t2 != null){
			foreach(Object otherObj in t2.objs){
				if(otherObj.pos == objPlaced.pos){
					Debug.LogError("PlaceObject: Trying to place an Wall where there is furniture.");
					return null;
				}
			}
		}
		if(t != null){
			t.objs.Add(objPlaced);
			objPlaced.tiles.Add(t);
		}
		if(t2 != null){
			t2.objs.Add(objPlaced);
			objPlaced.tiles.Add(t2);
		}
		objPlaced.UpdateNeighbors(objPlaced);
		return objPlaced;
	}
	
	/// <summary>
	/// updates the info of the object, and it's surrounding tiles,
	/// and checks to make sure the position that the object is attempting to be placed at is valid
	/// </summary>
	/// <returns>Wall that is placed, or null if it's an invalid position</returns>
	/// <param name="obj">existing Wall that is trying to be moved</param>
	/// <param name="pos">position that the object is trying to be placed at</param>
	/// <param name="rotLevel">rotLevel, which helps to determine the position and rotation of the object</param>
	/// <param name="moveNeighbors">moveNeighbors, defaults to true and determines whether the Object's neighbors should be moved</param>
	public static void MoveObject(Wall obj,Vector3 pos,int rotLevel,bool moveNeighbors = true){
		BuildModeController bmc = WorldController.Instance.MouseController.bmc;
		Tile prevT = WorldController.Instance.World.GetTileAt(Mathf.RoundToInt(obj.pos.x),Mathf.RoundToInt(obj.pos.z),bmc.ActiveFloor);
		Tile prevT2 = prevT;
		Tile newT = WorldController.Instance.World.GetTileAt(Mathf.RoundToInt(pos.x),Mathf.RoundToInt(pos.z),bmc.ActiveFloor);
		Tile newT2 = newT;
		Vector3 newPos = new Vector3(0,0,0);
		if(rotLevel%2 == 0){
			if(pos.z%1 >= 0.5){
				prevT2 = WorldController.Instance.World.GetTileAt(Mathf.FloorToInt(obj.pos.x),Mathf.FloorToInt(obj.pos.z),bmc.ActiveFloor);
				newT2 = WorldController.Instance.World.GetTileAt(Mathf.FloorToInt(pos.x),Mathf.FloorToInt(pos.z),bmc.ActiveFloor);
				//newPos = new Vector3(newT.x,obj.defaultY + (2.75f * bmc.ActiveFloor),newT.y - (newT.height / 2));
				newPos = new Vector3(newT.x,obj.defaultY,newT.y - (newT.height / 2));
			} else {
				prevT2 = WorldController.Instance.World.GetTileAt(Mathf.CeilToInt(obj.pos.x),Mathf.CeilToInt(obj.pos.z),bmc.ActiveFloor);
				newT2 = WorldController.Instance.World.GetTileAt(Mathf.FloorToInt(pos.x),Mathf.FloorToInt(pos.z),bmc.ActiveFloor);
				//newPos = new Vector3(newT.x,obj.defaultY + (2.75f * bmc.ActiveFloor),newT.y + (newT.height / 2));
				newPos = new Vector3(newT.x,obj.defaultY,newT.y + (newT.height / 2));
			}
		} else if(rotLevel%2 == 1){
			if(pos.x%1 >= 0.5){
				prevT2 = WorldController.Instance.World.GetTileAt(Mathf.FloorToInt(pos.x),Mathf.FloorToInt(pos.z),bmc.ActiveFloor);
				newT2 = WorldController.Instance.World.GetTileAt(Mathf.FloorToInt(pos.x),Mathf.FloorToInt(pos.z),bmc.ActiveFloor);
				//newPos = new Vector3(newT.x - (newT.width / 2),obj.defaultY + (2.75f * bmc.ActiveFloor),newT.y);
				newPos = new Vector3(newT.x - (newT.width / 2),obj.defaultY,newT.y);
			} else {
				prevT2 = WorldController.Instance.World.GetTileAt(Mathf.CeilToInt(pos.x),Mathf.CeilToInt(pos.z),bmc.ActiveFloor);
				newT2 = WorldController.Instance.World.GetTileAt(Mathf.FloorToInt(pos.x),Mathf.FloorToInt(pos.z),bmc.ActiveFloor);
				//newPos = new Vector3(newT.x + (newT.width / 2),obj.defaultY + (2.75f * bmc.ActiveFloor),newT.y);
				newPos = new Vector3(newT.x + (newT.width / 2),obj.defaultY,newT.y);
			}
		}
		if(moveNeighbors == true && obj.connectsToNeighbors == true){
			foreach(Object n in obj.neighbors){
				Vector3 nPos = newPos - (obj.pos - n.pos);
				Tile nNewT = WorldController.Instance.World.GetTileAt(Mathf.RoundToInt(nPos.x),Mathf.RoundToInt(nPos.z),bmc.ActiveFloor);
				Tile nNewT2 = newT;
				if(rotLevel%2 == 0){
					if(pos.z%1 >= 0.5){
						nNewT2 = WorldController.Instance.World.GetTileAt(Mathf.FloorToInt(pos.x),Mathf.FloorToInt(pos.z),bmc.ActiveFloor);
					} else {
						nNewT2 = WorldController.Instance.World.GetTileAt(Mathf.FloorToInt(pos.x),Mathf.FloorToInt(pos.z),bmc.ActiveFloor);
					}
				} else if(rotLevel%2 == 1){
					if(pos.x%1 >= 0.5){
						nNewT2 = WorldController.Instance.World.GetTileAt(Mathf.FloorToInt(pos.x),Mathf.FloorToInt(pos.z),bmc.ActiveFloor);
					} else {
						nNewT2 = WorldController.Instance.World.GetTileAt(Mathf.FloorToInt(pos.x),Mathf.FloorToInt(pos.z),bmc.ActiveFloor);
					}
				}
				foreach(Object otherObj in nNewT.objs){
					if(otherObj.pos == nPos && otherObj != n && n.neighbors.Contains(otherObj) == false){
						Debug.LogError("MoveObject: Trying to place an Wall where there is already furniture.");
						return;
					}
				}
				foreach(Object otherObj in nNewT2.objs){
					if(otherObj.pos == nPos && otherObj != n && n.neighbors.Contains(otherObj) == false){
						Debug.LogError("MoveObject: Trying to place an Wall where there is already furniture.");
						return;
					}
				}
			}
		} else {
			foreach(Object otherObj in newT.objs){
				if(otherObj.pos == newPos && otherObj != obj && obj.neighbors.Contains(otherObj) == false){
					Debug.LogError("MoveObject: Trying to place an Wall where there is already furniture.");
					return;
				}
			}
			foreach(Object otherObj in newT2.objs){
				if(otherObj.pos == newPos && otherObj != obj && obj.neighbors.Contains(otherObj) == false){
					Debug.LogError("MoveObject: Trying to place an Wall where there is already furniture.");
					return;
				}
			}
		}
		if(prevT.objs.Contains(obj) == true){
			prevT.objs.Remove(obj);
		}
		if(prevT2.objs.Contains(obj) == true){
			prevT2.objs.Remove(obj);
		}
		if(obj.tiles.Contains(prevT) == true){
			obj.tiles.Remove(prevT);
		}
		if(obj.tiles.Contains(prevT2) == true){
			obj.tiles.Remove(prevT2);
		}
		Vector3 diffPos = pos - obj.pos;
		diffPos.y = 0;
		obj.diffPos = diffPos;
		obj.pos = newPos;
		if(obj.connectsToNeighbors == true && Object.moveNeighbors == true && moveNeighbors == true){
			obj.MoveNeighbors(obj);
		}
		Quaternion rot = Quaternion.identity;
		rot.eulerAngles = new Vector3(0f,90f * rotLevel,0f);
		obj.rot = rot;
		newT.objs.Add(obj);
		newT2.objs.Add(obj);
		obj.tiles.Add(newT);
		obj.tiles.Add(newT2);
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
	public static void DeleteObject(Wall obj,bool deleteNeighbors = true){
		if(obj.isDeleted == true){
			return;
		}
		obj.isDeleted = true;
		foreach(Tile t in obj.tiles){
			if(t.objs.Contains(obj) == true){
				t.objs.Remove(obj);
			}
		}
		obj.tiles.Clear();
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