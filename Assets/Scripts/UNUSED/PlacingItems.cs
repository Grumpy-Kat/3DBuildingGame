using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlacingItems : MonoBehaviour {

	public GameObject item;
	public GameObject[] itemInScene;
	public GameObject player;

	//vars for ColorPicker
	GameObject toggle;
	public GameObject colorPicker;
	public GameObject rSlide;
	public GameObject gSlide;
	public GameObject bSlide;
	public GameObject hSlide;
	public GameObject sSlide;
	public GameObject vSlide;

	//vars for MoveItem
	public GameObject positionTool;
	public GameObject xInputPos;
	public GameObject yInputPos;
	public GameObject zInputPos;

	//vars for ScaleItem
	public GameObject scaleTool;
	public GameObject xInputScale;
	public GameObject yInputScale;
	public GameObject zInputScale;

	//vars for RotateItem
	public GameObject rotationTool;
	public GameObject xInputRotate;
	public GameObject yInputRotate;
	public GameObject zInputRotate;

	void Start(){
		player = GameObject.Find ("Main Camera");
		GetComponent<Button> ().onClick.AddListener (CreatePrefab);
	}

	void CreatePrefab(){
		Instantiate (item, new Vector3(0f,2f,0f), new Quaternion(0f, player.transform.rotation.y,0f,player.transform.rotation.w));
		itemInScene = GameObject.FindGameObjectsWithTag ("MenuItem");

		for (int i=0; i<itemInScene.Length; i++) {
			if(i == itemInScene.Length - 1){
				var name = itemInScene[i].name + i.ToString();
				itemInScene[i].name = name;
			}
		}
		//ColorPicker
		//itemInScene [itemInScene.Length - 1].GetComponent<ColorPicker> ().toggle = toggle;
		//itemInScene [itemInScene.Length - 1].GetComponent<ColorPicker> ().colorPicker = colorPicker;
		//itemInScene [itemInScene.Length - 1].GetComponent<ColorPicker> ().rSlide = rSlide;
		//itemInScene [itemInScene.Length - 1].GetComponent<ColorPicker> ().gSlide = gSlide;
		//itemInScene [itemInScene.Length - 1].GetComponent<ColorPicker> ().bSlide = bSlide;
		//itemInScene [itemInScene.Length - 1].GetComponent<ColorPicker> ().hSlide = hSlide;
		//itemInScene [itemInScene.Length - 1].GetComponent<ColorPicker> ().sSlide = sSlide;
		//itemInScene [itemInScene.Length - 1].GetComponent<ColorPicker> ().vSlide = vSlide;
		//MoveItem
		itemInScene [itemInScene.Length - 1].GetComponent<MoveItem> ().positionTool = positionTool;
		itemInScene [itemInScene.Length - 1].GetComponent<MoveItem> ().xInput = xInputPos;
		itemInScene [itemInScene.Length - 1].GetComponent<MoveItem> ().yInput = yInputPos;
		itemInScene [itemInScene.Length - 1].GetComponent<MoveItem> ().zInput = zInputPos;
		//ScaleItem
		itemInScene[itemInScene.Length-1].GetComponent<ScaleItem> ().scaleTool = scaleTool;
		itemInScene[itemInScene.Length-1].GetComponent<ScaleItem> ().xInput = xInputScale;
		itemInScene[itemInScene.Length-1].GetComponent<ScaleItem> ().yInput = yInputScale;
		itemInScene[itemInScene.Length-1].GetComponent<ScaleItem> ().zInput = zInputScale;
		//RotateItem
		itemInScene[itemInScene.Length-1].GetComponent<RotateItem> ().rotationTool = rotationTool;
		itemInScene[itemInScene.Length-1].GetComponent<RotateItem> ().xInput = xInputRotate;
		itemInScene[itemInScene.Length-1].GetComponent<RotateItem> ().yInput = yInputRotate;
		itemInScene[itemInScene.Length-1].GetComponent<RotateItem> ().yVal = player.transform.rotation.y;
		itemInScene[itemInScene.Length-1].GetComponent<RotateItem> ().zInput = zInputRotate;
	}
}
