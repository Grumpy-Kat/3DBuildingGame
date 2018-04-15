using UnityEngine;
using System.Collections.Generic;

public class WorldController : MonoBehaviour {
	
	//public references
	public static WorldController Instance { get; protected set; }
	public World World { get; protected set; }
	//references to other controller
	public MouseController MouseController { get; protected set; }
	public TileController TileController { get; protected set; }

	void OnEnable(){
		if (Instance != null) {
			Debug.LogError("WorldController: There should only ever be one WorldController.");
		}
		Instance = this;
		//create a World with all Objects
		World = new World ();
		MouseController = GameObject.FindObjectOfType<MouseController>();
		TileController = GameObject.FindObjectOfType<TileController>();
		GameObject.Find("Ground").transform.position = new Vector3((World.width / 2)-0.5f,-0.16f,(World.height / 2)-0.5f);
		Camera.main.transform.position = new Vector3(World.width / 2,3f,(World.height / 2) - 13);
	}
	
	/// <summary>
	/// gets the tile at woorld coordinates given
	/// </summary>
	/// <returns>the tile at the woorld coordinates</returns>
	/// <param name="coord">Tile's coordinates in world being passed</param>
	public Tile GetTileAtWoorldCoord(Vector3 coord){
		int x = Mathf.CeilToInt (coord.x);
		int y = Mathf.CeilToInt (coord.z);
		return World.GetTileAt (x,y,MouseController.bmc.ActiveFloor);
	}
}