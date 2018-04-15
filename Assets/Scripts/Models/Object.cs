using UnityEngine;
using System;
using System.Collections.Generic;

//ObjTypes just for Objects, do not include any subclass types!
public enum ObjType {Default,Wall,Table,Chair};

//General Object (only used as a parent class)
public class Object {

	//toggleable options for objects
	public static bool moveNeighbors = true;
	//object data properties
	public ObjType type { get; protected set; }
	public UnityEngine.Object prefab { get; protected set; }
	public float defaultY;
	public bool connectsToNeighbors { get; protected set; }
	public List<Object> neighbors;
	public bool isDeleted { get; protected set; }
	public Color color { get; protected set; }
	//object position properties
	public Vector3 pos { get; protected set; }
	public List<Tile> tiles { get; protected set; }
	public Vector3 diffPos { get; protected set; }
	public Quaternion rot { get; protected set; }
	//actions
	protected Action<Object> cbObjectChanged;
	protected Action<Object> cbNeighborsChanged;
	protected Action<Object> cbColorChanged;

	/// <summary>
	/// initializes a new instance of the <see cref="Object"/> class (constructer)
	/// </summary>
	/// <param name="type">ObjType, which is the type of the object used to define the subclass</param>
	/// <param name="prefab">Prefab of the Object</param>
	/// <param name="defaultY">default Y coordinate of the object</param>
	public Object (ObjType type,UnityEngine.Object prefab,float defaultY,bool connectsToNeighbors){
		this.type = type;
		this.prefab = prefab;
		this.defaultY = defaultY;
		this.connectsToNeighbors = connectsToNeighbors;
		neighbors = new List<Object>();
		tiles = new List<Tile>();
	}

	/// <summary>
	/// does the same thing as the constructer, except will create instances of it's subclasses
	/// </summary>
	/// <returns>the created instance returned from the constructer</returns>
	/// <param name="type">ObjType, which is the type of the object used to define the subclass</param>
	/// <param name="prefab">Prefab of the Object</param>
	/// <param name="defaultY">default Y coordinate of the object</param>
	public static Object CreateInstance(ObjType type,UnityEngine.Object prefab,float defaultY,bool connectsToNeighbors,string subType = null){
		Object obj;
		switch(type){
			case ObjType.Wall:
				WallType edgeType = WallType.Default;
				foreach(WallType subEdgeType in Enum.GetValues(typeof(WallType))){
					if(subType == subEdgeType.ToString()){
						edgeType = subEdgeType;
					}
				}
				if(edgeType == WallType.Default){
					Debug.Log("CreateInstance: There was a wall with WallType.Default, meaning it is probably an unadded or unrecognized wall. It will still be created, but probably need to be added to the rest of the system. It's type was " + subType + ".");
				}
				obj = new Wall(type,prefab,defaultY,connectsToNeighbors,edgeType);
				break;
			case ObjType.Table:
				TableType tableType = TableType.Default;
				foreach(TableType subTableType in Enum.GetValues(typeof(TableType))){
					if(subType == subTableType.ToString()){
						tableType = subTableType;
					}
				}
				if(tableType == TableType.Default){
						Debug.Log("CreateInstance: There was an table with TableType.Default, meaning it is probably an unadded or unrecognized table. It will still be created, but probably need to be added to the rest of the system. It's type was " + subType + ".");
				}
				obj = new Table(type,prefab,defaultY,connectsToNeighbors,tableType);
				break;
			case ObjType.Chair:
				ChairType chairType = ChairType.Default;
				foreach(ChairType subChairType in Enum.GetValues(typeof(ChairType))){
					if(subType == subChairType.ToString()){
						chairType = subChairType;
					}
				}
				if(chairType == ChairType.Default){
					Debug.Log("CreateInstance: There was an chair with ChairType.Default, meaning it is probably an unadded or unrecognized chair. It will still be created, but probably need to be added to the rest of the system. It's type was " + subType + ".");
				}
				obj = new Chair(type,prefab,defaultY,connectsToNeighbors,chairType);
				break;
			default:
				obj = new Object(type,prefab,defaultY,connectsToNeighbors);
				break;
		}
		return obj;
	}

	/// <summary>
	/// updates the info of the object, and it's surrounding tiles,
	/// and checks to make sure the position that the object is attempting to be placed at is valid
	/// </summary>
	/// <returns>Object that is placed, or null if it's an invalid position</returns>
	/// <param name="obj">Object that is trying to be placed</param>
	/// <param name="pos">position that the object is trying to be placed at</param>
	/// <param name="rotLevel">rotLevel, which helps to determine the position and rotation of the object</param>
	public static Object PlaceObject(Object proto,Vector3 pos,int rotLevel){
		switch(proto.type){
			case ObjType.Wall:
				Wall wallPlaced = Wall.PlaceObject((Wall)proto,pos,rotLevel);
				return wallPlaced;
			case ObjType.Table:
				Table tablePlaced = Table.PlaceObject((Table)proto,pos,rotLevel);
				return tablePlaced;
			case ObjType.Chair:
				Chair chairPlaced = Chair.PlaceObject((Chair)proto,pos,rotLevel);
				return chairPlaced;
			default:
				Object objPlaced = new Object(proto.type,proto.prefab,proto.defaultY,proto.connectsToNeighbors);
				Tile t = WorldController.Instance.World.GetTileAt(Mathf.RoundToInt(pos.x),Mathf.RoundToInt(pos.z),WorldController.Instance.MouseController.bmc.ActiveFloor);
				objPlaced.diffPos = new Vector3(0,0,0);
				//objPlaced.pos = new Vector3(t.x,proto.defaultY + (2.75f * WorldController.Instance.MouseController.bmc.ActiveFloor),t.y);
				objPlaced.pos = new Vector3(t.x,proto.defaultY,t.y);
				Quaternion rot = Quaternion.identity;
				rot.eulerAngles = new Vector3(0f,90f * rotLevel,0f);
				objPlaced.rot = rot;
				foreach(Object otherObj in t.objs){
					if(otherObj.pos == objPlaced.pos){
						Debug.LogError("PlaceObject: Trying to place an Object where there is already one");
						return null;
					}
				}
				if(t.Type == TileType.Empty){
					Debug.LogError("PlaceObject: Trying to assign an Object to an empty tile!");
					return null;
				}
				t.objs.Add(objPlaced);
				objPlaced.tiles.Add(t);
				objPlaced.UpdateNeighbors(objPlaced);
				return objPlaced;
		}
	}

	/// <summary>
	/// updates the info of the object, and it's surrounding tiles,
	/// and checks to make sure the position that the object is attempting to be placed at is valid
	/// </summary>
	/// <param name="obj">Object that is trying to be moved</param>
	/// <param name="pos">position that the object is trying to be placed at</param>
	/// <param name="rotLevel">rotLevel, which helps to determine the position and rotation of the object</param>
	/// <param name="moveNeighbors">moveNeighbors, defaults to true and determines whether the Object's neighbors should be moved</param>
	public static void MoveObject(Object obj,Vector3 pos,int rotLevel,bool moveNeighbors = true){
		switch(obj.type){
			case ObjType.Wall:
				Wall.MoveObject((Wall)obj,pos,rotLevel,moveNeighbors);
				break;
			case ObjType.Table:
				Table.MoveObject((Table)obj,pos,rotLevel,moveNeighbors);
				break;
			case ObjType.Chair:
				Chair.MoveObject((Chair)obj,pos,rotLevel,moveNeighbors);
				break;
			default:
				BuildModeController bmc = WorldController.Instance.MouseController.bmc;
				Tile prevT = WorldController.Instance.World.GetTileAt(Mathf.RoundToInt(obj.pos.x),Mathf.RoundToInt(obj.pos.z),bmc.ActiveFloor);
				Tile newT = WorldController.Instance.World.GetTileAt(Mathf.RoundToInt(pos.x),Mathf.RoundToInt(pos.z),bmc.ActiveFloor);
				if(moveNeighbors == true && obj.connectsToNeighbors == true){
					foreach(Object n in obj.neighbors){
						Vector3 nPos = pos - (obj.pos - n.pos);
						Tile nNewT = WorldController.Instance.World.GetTileAt(Mathf.RoundToInt(nPos.x),Mathf.RoundToInt(nPos.z),bmc.ActiveFloor);
						foreach(Object otherObj in nNewT.objs){
							if(otherObj.pos == nPos && otherObj != n && n.neighbors.Contains(otherObj) == false){
								Debug.LogError("MoveObject: Trying to place an Object where there is already one");
								return;
							}
						}
						if(nNewT.Type == TileType.Empty){
							Debug.LogError("MoveObject: Trying to assign an Object to an empty tile!");
							return;
						}
					}
				} else {
					foreach(Object otherObj in newT.objs){
						if(otherObj.pos == pos && otherObj != obj && obj.neighbors.Contains(otherObj) == false){
							Debug.LogError("MoveObject: Trying to place an Object where there is already one");
							return;
						}
					}
					if(newT.Type == TileType.Empty){
						Debug.LogError("MoveObject: Trying to assign an Object to an empty tile!");
						return;
					}
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
				//obj.pos = new Vector3(newT.x,obj.defaultY + (2.75f * bmc.ActiveFloor),newT.y);
				obj.pos = new Vector3(newT.x,obj.defaultY,newT.y);
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
				break;
		}
	}

	public void SetColor(Color color,bool colorNeighbors = true){
		this.color = color;
		if(colorNeighbors == true){
			if(cbColorChanged != null){
				cbColorChanged(this);
			}
		}
	}

	/// <summary>
	/// updates the info of the object, and it's surrounding tiles,
	/// and checks to make sure the position that the object is attempting to be placed at is valid
	/// </summary>
	/// <param name="obj">Object that is trying to be deleted</param>
	/// <param name="deleteNeighbors">deleteNeighbors, defaults to true and determines whether the Object's neighbors should be deleted</param>
	public static void DeleteObject(Object obj,bool deleteNeighbors = true){
		switch(obj.type){
			case ObjType.Wall:
				Wall.DeleteObject((Wall)obj);
				break;
			case ObjType.Table:
				Table.DeleteObject((Table)obj);
				break;
			case ObjType.Chair:
				Chair.DeleteObject((Chair)obj);
				break;
			default:
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
				break;
		}
	}

	/// <summary>
	/// updates the neighbors by calling the callback on the Object and it's neighbors
	/// </summary>
	/// <param name="obj">Object that is having it's neighbors updated</param>
	public void UpdateNeighbors(Object obj){
		if(obj.cbNeighborsChanged != null){
			obj.cbNeighborsChanged(obj);
		}
		Object[] objNeighbors = obj.neighbors.ToArray();
		foreach(Object n in objNeighbors){
			if(n.cbNeighborsChanged != null){
				n.cbNeighborsChanged(n);
			}
		}
	}

	/// <summary>
	/// moves the neighbors by calling the visual function in ObjectController
	/// </summary>
	/// <param name="obj">Object that is being moved</param>
	public void MoveNeighbors(Object obj){
		ObjectController.Instance.MoveNeighbors(obj);
	}

	/// <summary>
	/// delete the neighbors by calling the visual function in ObjectController
	/// </summary>
	/// <param name="obj">Object that is being moved</param>
	public void DeleteNeighbors(Object obj){
		ObjectController.Instance.DeleteNeighbors(obj);
	}

	/// <summary>
	/// helper function that sets this object's diffPos to (0,0,0)
	/// </summary>
	public void ClearDiffPos(){
		diffPos = Vector3.zero;
	}

	/// <summary>
	/// helper function that calculates this object's rotLevel based on it's rotation
	/// </summary>
	public int GetRotLevel(){
		return (int)(rot.eulerAngles.y / 90);
	}

	/// <summary>
	/// registering/unregistering ObjectChanged callback
	/// </summary>
	/// <param name="cb">callback</param>
	public void RegisterObjectChanged(Action<Object> cb){
		cbObjectChanged += cb;
	}
	
	public void UnregisterObjectChanged(Action<Object> cb){
		cbObjectChanged -= cb;
	}
	
	/// <summary>
	/// registering/unregistering NeighborsChanged callback
	/// </summary>
	/// <param name="cb">callback</param>
	public void RegisterNeighborsChanged(Action<Object> cb){
		cbNeighborsChanged += cb;
	}
	
	public void UnregisterNeighborsChanged(Action<Object> cb){
		cbNeighborsChanged -= cb;
	}

	/// <summary>
	/// registering/unregistering ColorChanged callback
	/// </summary>
	/// <param name="cb">callback</param>
	public void RegisterColorChanged(Action<Object> cb){
		cbColorChanged += cb;
	}
	
	public void UnregisterColorChanged(Action<Object> cb){
		cbColorChanged -= cb;
	}
}