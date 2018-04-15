using UnityEngine;
using System.Collections.Generic;

public class TileController : MonoBehaviour {
	
	//maps of Object instance to GameObject
	public Dictionary<Tile,GameObject> tileGameObjMap;
	//parent objects
	GameObject tileParent;
	//world instance
	World world {
		get{
			return WorldController.Instance.World;
		}
	}
	
	void Start(){
		//instantiate dictionary
		tileGameObjMap = new Dictionary<Tile,GameObject>();
		//create a GameObject for each tile
		tileParent = GameObject.Find("TileParent");
		for(int z = 0; z < world.maxFloors; z++){
			//create a GameObject
			GameObject floor_go = new GameObject();
			//add properties to GameObject based on Floor's properties
			floor_go.name = "Floor_" + z;
			floor_go.transform.position = new Vector3(0,z * 2.75f,0);
			floor_go.transform.SetParent(tileParent.transform,true);
			//create a GameObject
			GameObject objParent = new GameObject();
			//add properties to GameObject based on ObjectParent's properties
			objParent.name = "ObjectParent";
			objParent.transform.position = new Vector3(0,z * 2.75f,0);
			objParent.transform.SetParent(floor_go.transform,true);
			//add to array
			ObjectController.Instance.objParent[z] = objParent;
			for (int x = 0; x < world.width; x++) {
				for (int y = 0; y < world.height; y++) {
					//get Tile's properties
					Tile t = world.GetTileAt(x,y,z);
					//create a GameObject
					GameObject tile_go = new GameObject();
					//add properties to GameObject based on Tile's properties
					tile_go.name = "Tile_" + x + "_" + y;
					tile_go.transform.position = new Vector3(t.x,0,t.y);
					tile_go.transform.Rotate(new Vector3(90,0,0));
					tile_go.transform.SetParent(floor_go.transform,false);
					tile_go.tag = "Tile";
					SpriteRenderer tile_sr = tile_go.AddComponent<SpriteRenderer>();
					tile_sr.sprite = world.tileTypeSpriteMap[TileType.Empty];
					BoxCollider tile_bc = tile_go.AddComponent<BoxCollider>();
					tile_bc.size = new Vector3(1,1,1);
					//add to dictionary
					tileGameObjMap.Add(t,tile_go);
					//updates GameObject whenever the Tile changes using callbacks
					t.RegisterTileChanged(OnTileChanged);
				}
			}
			WorldController.Instance.MouseController.bmc.floors[z] = floor_go;
		}
		WorldController.Instance.MouseController.bmc.ActiveFloor = 0;
	}

	/// <summary>
	/// called whenever a change occurs to a tile's TileType and updates SpriteRenderer
	/// </summary>
	/// <param name="tile_data">Tile Data, which holds the info needed to be applied</param>
	void OnTileChanged(Tile t){
		if(tileGameObjMap.ContainsKey(t) == false){
			Debug.Log("OnTileChanged - Doesn't contain tile_data.");
			return;
		}
		GameObject tile_go = tileGameObjMap[t];
		if (tile_go == null) {
			Debug.Log("OnTileChanged - tileGameObjectMap returned GameObject is null");
			return;
		}
		if(WorldController.Instance.World.tileTypeSpriteMap.ContainsKey(t.Type) == false){
			Debug.LogError("Update: Missing Key in tileTypeSpriteMap!");
			return;
		}
		Sprite tileSprite = WorldController.Instance.World.tileTypeSpriteMap[t.Type];
		SpriteRenderer tile_sr = tile_go.GetComponent<SpriteRenderer>();
		tile_sr.sprite = tileSprite;
		if(tile_sr.color != Color.white){
			ToggleColorState active = ToggleColorState.GetActive();
			if(active != null && active.GetConnectedObj() == tile_go){
				active.ChangeColor(Color.white);
				if(t.Type != TileType.Empty && t.Type != TileType.Grid){
					active.SetConnectedObj(tile_go);
				} else {
					active.SetConnectedObj(null);
				}
			} else {
				tile_sr.GetComponent<SpriteRenderer>().color = Color.white;
			}
		}
		Quaternion rot = Quaternion.identity;
		rot.eulerAngles = new Vector3(90f,90f * t.RotLevel,0f);
		tile_go.transform.rotation = rot;
		Debug.Log(t.x + "_" + t.y + "_" + t.Type.ToString() + "_" + t.world.GetRoomID(t.room));
	}
}