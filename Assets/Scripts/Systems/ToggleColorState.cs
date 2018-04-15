using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class ToggleColorState : MonoBehaviour {
	//reference to controllers
	ObjectController oc;
	//color vars
	public GameObject orgObj { get; protected set; }
	public Color currColor { get; protected set; }
	//state vars
	public bool active { get; protected set; }
	public bool changed { get; protected set; }

	void Start(){
		oc = ObjectController.Instance;
	}

	public static ToggleColorState GetActive(){
		foreach(ToggleColorState other in Resources.FindObjectsOfTypeAll<ToggleColorState>()){
			if(other.active == true){
				return other;
			}
		}
		Debug.Log("No active tab was found. This is unusual and should never happen. You may be setting the active boolean directly, in which case you should a provided function instead.");
		return null;
	}

	public void SetActive(){
		foreach(ToggleColorState other in GameObject.FindObjectsOfType<ToggleColorState>()){
			other.active = false;
		}
		active = true;
	}

	public GameObject GetConnectedObj(){
		return orgObj;
	}

	public void SetConnectedObj(GameObject newGameObj){
		changed = true;
		orgObj = newGameObj;
		if(newGameObj == null){
			this.gameObject.GetComponent<Image>().color = new Color(0.5f,0.5f,0.5f);
			currColor = new Color(0.5f,0.5f,0.5f);
		} else if(newGameObj.tag == "Tile") {
			this.gameObject.GetComponent<Image>().color = newGameObj.GetComponent<SpriteRenderer>().color;
			currColor = newGameObj.GetComponent<SpriteRenderer>().color;
		} else {
			this.gameObject.GetComponent<Image>().color = newGameObj.GetComponent<Renderer>().material.color;
			currColor = newGameObj.GetComponent<Renderer>().material.color;
		}
	}

	public void ChangeColor(Color newColor){
		changed = false;
		if(currColor == newColor){
			return;
		}
		currColor = newColor;
		this.gameObject.GetComponent<Image>().color = newColor;
		if(orgObj == null){
			return;
		}
		GameObject obj_go = orgObj;
		if(orgObj.transform.tag != "Tile"){
			do{
				obj_go = obj_go.transform.parent.gameObject;
			} while(obj_go.transform.IsChildOf(oc.objParent[WorldController.Instance.MouseController.bmc.ActiveFloor].transform) == false);
			if(oc.objGameObjMap.ContainsValue(obj_go)){
				Object obj = oc.objGameObjMap.Keys.ToArray()[oc.objGameObjMap.Values.ToList().IndexOf(obj_go)];
				if(obj.connectsToNeighbors == true && Object.moveNeighbors == true){
					foreach(Object n in obj.neighbors){
						if(oc.objGameObjMap.ContainsKey(n) == true){
							GameObject n_go = oc.objGameObjMap[n];
							n_go.transform.GetChild(orgObj.transform.GetSiblingIndex()).GetComponent<Renderer>().material.color = newColor;
						}
					}
				} else {
					orgObj.GetComponent<Renderer>().material.color = newColor;
				}
			}
		} else {
			TileController tc = WorldController.Instance.TileController;
			if(tc.tileGameObjMap.ContainsValue(orgObj)){
				Tile t = tc.tileGameObjMap.Keys.ToArray()[tc.tileGameObjMap.Values.ToList().IndexOf(orgObj)];
				ColorNeighboringTiles(t);
			}
		}
	}

	/// <summary>
	/// recursive floodfill algorithm only for tiles that colors the orginal tile, colors all
	/// neighbors of the same type, and then calls the function again for all neighbors
	/// </summary>
	/// <param name="t">orginal tile</param>
	void ColorNeighboringTiles(Tile t){
		World world = WorldController.Instance.World;
		TileController tc = WorldController.Instance.TileController;
		BuildModeController bmc = WorldController.Instance.MouseController.bmc;
		if(tc.tileGameObjMap[t].GetComponent<SpriteRenderer>().color != currColor){
			tc.tileGameObjMap[t].GetComponent<SpriteRenderer>().color = currColor;
		}
		Tile n1 = world.GetTileAt(t.x-1,t.y,bmc.ActiveFloor);
		if(n1 != null && n1.Type == t.Type){
			if(tc.tileGameObjMap[n1].GetComponent<SpriteRenderer>().color != currColor){
				ColorNeighboringTiles(n1);
			}
		}
		Tile n2 = world.GetTileAt(t.x+1,t.y,bmc.ActiveFloor);
		if(n2 != null && n2.Type == t.Type){
			if(tc.tileGameObjMap[n2].GetComponent<SpriteRenderer>().color != currColor){
				ColorNeighboringTiles(n2);
			}
		}
		Tile n3 = world.GetTileAt(t.x,t.y-1,bmc.ActiveFloor);
		if(n3 != null && n3.Type == t.Type){
			if(tc.tileGameObjMap[n3].GetComponent<SpriteRenderer>().color != currColor){
				ColorNeighboringTiles(n3);
			}
		}
		Tile n4 = world.GetTileAt(t.x,t.y+1,bmc.ActiveFloor);
		if(n4 != null && n4.Type == t.Type){
			if(tc.tileGameObjMap[n4].GetComponent<SpriteRenderer>().color != currColor){
				ColorNeighboringTiles(n4);
			}
		}
		Tile n5 = world.GetTileAt(t.x-1,t.y-1,bmc.ActiveFloor);
		if(n5 != null && n5.Type == t.Type){
			if(tc.tileGameObjMap[n5].GetComponent<SpriteRenderer>().color != currColor){
				ColorNeighboringTiles(n5);
			}
		}
		Tile n6 = world.GetTileAt(t.x+1,t.y-1,bmc.ActiveFloor);
		if(n6 != null && n6.Type == t.Type){
			if(tc.tileGameObjMap[n6].GetComponent<SpriteRenderer>().color != currColor){
				ColorNeighboringTiles(n6);
			}
		}
		Tile n7 = world.GetTileAt(t.x-1,t.y+1,bmc.ActiveFloor);
		if(n7 != null && n7.Type == t.Type){
			if(tc.tileGameObjMap[n7].GetComponent<SpriteRenderer>().color != currColor){
				ColorNeighboringTiles(n7);
			}
		}
		Tile n8 = world.GetTileAt(t.x+1,t.y+1,bmc.ActiveFloor);
		if(n8 != null && n8.Type == t.Type){
			if(tc.tileGameObjMap[n8].GetComponent<SpriteRenderer>().color != currColor){
				ColorNeighboringTiles(n8);
			}
		}
	}
}