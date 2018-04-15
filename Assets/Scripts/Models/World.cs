using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;

public class World {
	//list of all Rooms in the world
	public List<Room> rooms;
	//list of all Objects currently placed in the world
	public List<Object> objects;
	//maps between data and graphics
	public Dictionary<string,Object> stringObjectMap { get; protected set; }
	public Dictionary<TileType,Sprite> tileTypeSpriteMap { get; protected set; }
	//all tiles are stored in this array
	protected Tile[,,] tiles;
	//pos of tiles
	public int width { get; protected set; }
	public int height { get; protected set; }
	public int maxFloors { get; protected set; }
	//actions
	Action<Object> cbObjectCreated;

	/// <summary>
	/// initializes a new instance of the <see cref="World"/> class (constructer)
	/// </summary>
	/// <param name="width">the width, in tiles, of the world</param>
	/// <param name="height">the height, in tiles, of the world</param>
	public World(int width = 100, int height = 100, int maxFloors = 5){
		//initializes a list of all Objects currently placed in the world
		objects = new List<Object>();
		//initializes maps between data and graphics
		stringObjectMap = new Dictionary<string, Object>();
		tileTypeSpriteMap = new Dictionary<TileType, Sprite>();
		//creates all objs and tiles, specifically loads the data from XML files and retrieves their graphics
		LoadObjectXML();
		LoadTileXML();
		this.width = width;
		this.height = height;
		this.maxFloors = maxFloors;
		rooms = new List<Room>();
		rooms.Add(new Room());
		tiles = new Tile[width, height,maxFloors];
		for(int z = 0; z < maxFloors; z++){
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					tiles[x,y,z] = new Tile(this,GetOutsideRoom(),x,y);
				}
			}
			new Floor(z,this);
		}
	}

	public Room GetOutsideRoom(){
		if(rooms.Count != 0){
			return rooms[0];
		}
		Debug.LogError("No outside room found!");
		return null;
	}

	public void AddRoom(Room r){
		rooms.Add(r);
	}

	public void DeleteRoom(Room r){
		if(r == GetOutsideRoom()){
			Debug.LogError("Tried to delete outside!");
			return;
		}
		//remove r from list
		rooms.Remove(r);
		//reassign all tiles to outside
		r.UnassignAllTiles();
	}

	public int GetRoomID(Room r){
		return rooms.IndexOf(r);
	}

	/// <summary>
	/// loads the XML file on object data
	/// </summary>
	public void LoadObjectXML(){
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(Application.dataPath + "/Resources/Data Files/Object.xml");
		XmlNode root = xmlDoc.DocumentElement;
		XmlNodeList nodeList = root.ChildNodes;
		foreach(XmlNode node in nodeList){
			XmlNodeList innerNodeList = node.ChildNodes;
			foreach(XmlNode innerNode in innerNodeList){
				UnityEngine.Object prefab = Resources.Load("Prefabs/" + innerNode.Name + "/" + innerNode.Attributes[0].Value);
				float defaultY = float.Parse(innerNode.Attributes[1].Value);
				ObjType objType = ObjType.Default;
				foreach(ObjType objTypes in Enum.GetValues(typeof(ObjType))){
					if(innerNode.Name == objTypes.ToString()){
						objType = objTypes;
					}
				}
				bool connectsToNeighbors = false;
				if(innerNode.Attributes.Count > 2 && innerNode.Attributes[2].Value == "true"){
					connectsToNeighbors = true;
				}
				if(objType == ObjType.Default){
					Debug.Log("LoadFurnitureXML: There was an object with ObjType.Default, meaning it is probably an unadded or unrecognized object. It will still be add to the stringObjectMap, but probably need to be added to the rest of the system. It's supposed class name was " + innerNode.Name + " and it's subtype was " + innerNode.Attributes[0].Value + ".");
				}
				Object objInstance = Object.CreateInstance(objType,prefab,defaultY,connectsToNeighbors,innerNode.Attributes[0].Value);
				stringObjectMap.Add(innerNode.Attributes[0].Value,objInstance);
			}
		}
	}

	/// <summary>
	/// loads the XML file on tile data
	/// </summary>
	public void LoadTileXML(){
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(Application.dataPath + "/Resources/Data Files/Tile.xml");
		XmlNode root = xmlDoc.DocumentElement;
		XmlNodeList nodeList = root.ChildNodes;
		foreach(XmlNode node in nodeList){
			foreach(TileType tileType in Enum.GetValues(typeof(TileType))){
				if(node.Attributes[0].Value == tileType.ToString()){
					Sprite sprite = Resources.Load<Sprite>("Tiles/" + tileType.ToString());
					tileTypeSpriteMap.Add(tileType,sprite);
				}
			}
		}
	}

	/// <summary>
	/// places the Object in the World
	/// </summary>
	/// <returns>Object that is placed, or null if it's an invalid position</returns>
	/// <param name="obj">Object that is trying to be placed</param>
	/// <param name="pos">position that the object is trying to be placed at</param>
	/// <param name="rotLevel">rotLevel, which helps to determine the position and rotation of the object</param>
	public Object PlaceObject(Object obj,Vector3 pos,int rotLevel){
		//TODO: Assumes object is 1 * 1
		Object objPlaced = Object.PlaceObject(obj,pos,rotLevel);
		if(objPlaced == null){
			//failed to place Object (most likely already spot taken)
			return null;
		}
		objects.Add(objPlaced);
		if(cbObjectCreated != null){
			cbObjectCreated(objPlaced);
		}
		objPlaced.UpdateNeighbors(objPlaced);
		return objPlaced;
	}

	/// <summary>
	/// gets the tile, from array, at a passed x, y, and z pos, presented in tiles
	/// </summary>
	/// <returns>the <see cref="Tile"/> from any array</returns>
	/// <param name="x">the x coordinate, in tiles</param>
	/// <param name="y">the y coordinate, in tiles</param>
	/// <param name="z">the z coordinate, in tiles</param>
	public Tile GetTileAt(int x, int y,int z){
		if(x > width-1 || x < 0 || y > height-1 || y < 0){
			Debug.LogError("Tile (" + x + "," + y + ") is out of range!");
			return null;
		}
		return tiles[x,y,z];
	}
	
	/// <summary>
	/// registering/unregistering ObjectCreated callback
	/// </summary>
	/// <param name="cb">callback</param>
	public void RegisterObjectCreated(Action<Object> cb){
		cbObjectCreated += cb;
	}
	
	public void UnregisterObjectCreated(Action<Object> cb){
		cbObjectCreated -= cb;
	}
}