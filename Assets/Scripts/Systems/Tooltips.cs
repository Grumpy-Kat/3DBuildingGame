using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tooltips : MonoBehaviour {
	public Text text;
	public MouseController mouseController;
	
	void Start () {
		text = GetComponent<Text>();
		if(text == null) {
			Debug.LogError("MouseOverTileTypeText: No 'Text' UI component on this object.");
			this.enabled = false;
			return;
		}
		mouseController = GameObject.FindObjectOfType<MouseController>();
		if(mouseController == null) {
			Debug.LogError("How do we not have an instance of mouse controller?");
			return;
		}
	}
	
	void Update () {
		Tile t = mouseController.GetMouseOverTile();
			if(t != null){
			string room = "";
			if(t.room  == null){
				room = "NULL";
			} else{
				room = t.world.GetRoomID(t.room).ToString();
			}
			string objects = "";
			for(int i = 0;i < t.objs.Count;i++){
				objects += t.objs[i].type.ToString();
				if(i != t.objs.Count-1){
					objects += ",";
				}
			}
			if(objects == ""){
				objects = "NULL";
			}
			text.text = "X: " + t.x.ToString() + "\n" +
						"Y: " + t.y.ToString() + "\n" +
						"Type: " + t.Type.ToString() + "\n" +
						"Room: " + room + "\n" +
						"Objects: " + objects;
		}
	}
}
