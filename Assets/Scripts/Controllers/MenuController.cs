using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class MenuController : MonoBehaviour {
	//reference to other controllers
	BuildModeController bmc;
	//prefabs
	public GameObject buttonPrefab;
	public GameObject menuPrefab;
	//objects
	GameObject menu;
	GameObject canvas;

	void Start () {
		//get reference to other controllers
		bmc = GameObject.FindObjectOfType<BuildModeController>();
		//get objects
		menu = GameObject.Find("Menu");
		canvas = GameObject.Find("Canvas");
		//generate sub-menus
		List<GameObject> buttons = new List<GameObject>();
		for(int i = 0;i < menu.transform.childCount;i++){
			buttons.Add(menu.transform.GetChild(i).gameObject);
		}
		foreach(GameObject button in buttons){
			switch(button.name){
				case "Floors":
					GenerateFloorMenu();
					break;
				case "Walls":
					GenerateWallMenu();
					break;
				case "Tables":
					GenerateTableMenu();
					break;
				case "Chairs":
					GenerateChairMenu();
					break;
			}
		}
	}

	/// <summary>
	/// open a UI building sub-menu, according to button pressed
	/// </summary>
	public void OpenMenu (string name) {
		GameObject subMenu = canvas.transform.FindChild(name + "Menu").gameObject;
		menu.SetActive(false);
		subMenu.SetActive(true);
	}

	/// <summary>
	/// open the UI building super-menu
	/// </summary>
	public void BackToMenu () {
		for(int i = 0;i < canvas.transform.childCount;i++){
			if(canvas.transform.GetChild(i).name.Contains("Menu") == true){
				canvas.transform.GetChild(i).gameObject.SetActive(false);
			}
		}
		menu.SetActive(true);
	}

	/// <summary>
	/// generates the floor sub-menu at start
	/// </summary>
	public void GenerateFloorMenu(){
		GameObject subMenu = GenerateGeneralMenu("Floor");
		int i = 0;
		foreach(TileType tileType in Enum.GetValues(typeof(TileType))){
			if(tileType != TileType.Grid){
				GameObject subButton = GameObject.Instantiate(buttonPrefab);
				subButton.transform.SetParent(subMenu.transform);
				subButton.transform.localPosition = new Vector3(-450 + (70 * i),35,0);
				i++;
				if(tileType != TileType.Empty){
					subButton.name = "Build" + tileType.ToString();
					subButton.transform.GetChild(0).GetComponent<Text>().text = AddSpacesBeforeUppercase("Build" + tileType.ToString());
				} else {
					subButton.name = "Bulldoze";
					subButton.transform.GetChild(0).GetComponent<Text>().text = "Bulldoze";
				}
				subButton.GetComponent<Button>().onClick.AddListener (() => { CallTileFunction(subButton); });
			}
		}
	}

	/// <summary>
	/// generates the wall sub-menu at start
	/// </summary>
	public void GenerateWallMenu(){
		GameObject subMenu = GenerateGeneralMenu("Wall");
		int i = 0;
		foreach(WallType wallType in Enum.GetValues(typeof(WallType))){
			if(wallType != WallType.Default){
				GameObject subButton = GameObject.Instantiate(buttonPrefab);
				subButton.transform.SetParent(subMenu.transform);
				subButton.transform.localPosition = new Vector3(-450 + (70 * i),35,0);
				i++;
				subButton.name = "Build" + wallType.ToString();
				subButton.transform.GetChild(0).GetComponent<Text>().text = AddSpacesBeforeUppercase("Build" + wallType.ToString());
				subButton.GetComponent<Button>().onClick.AddListener (() => { CallObjFunction(subButton); });
			}
		}
	}

	/// <summary>
	/// generates the table sub-menu at start
	/// </summary>
	public void GenerateTableMenu(){
		GameObject subMenu = GenerateGeneralMenu("Table");
		int i = 0;
		foreach(TableType tableType in Enum.GetValues(typeof(TableType))){
			if(tableType != TableType.Default){
				GameObject subButton = GameObject.Instantiate(buttonPrefab);
				subButton.transform.SetParent(subMenu.transform);
				subButton.transform.localPosition = new Vector3(-450 + (70 * i),35,0);
				i++;
				subButton.name = "Build" + tableType.ToString();
				subButton.transform.GetChild(0).GetComponent<Text>().text = AddSpacesBeforeUppercase("Build" + tableType.ToString());
				subButton.GetComponent<Button>().onClick.AddListener (() => { CallObjFunction(subButton); });
			}
		}
	}

	/// <summary>
	/// generates the chair sub-menu at start
	/// </summary>
	public void GenerateChairMenu(){
		GameObject subMenu = GenerateGeneralMenu("Chair");
		int i = 0;
		foreach(ChairType chairType in Enum.GetValues(typeof(ChairType))){
			if(chairType != ChairType.Default){
				GameObject subButton = GameObject.Instantiate(buttonPrefab);
				subButton.transform.SetParent(subMenu.transform);
				subButton.transform.localPosition = new Vector3(-450 + (70 * i),35,0);
				i++;
				subButton.name = "Build" + chairType.ToString();
				subButton.transform.GetChild(0).GetComponent<Text>().text = AddSpacesBeforeUppercase("Build" + chairType.ToString());
				subButton.GetComponent<Button>().onClick.AddListener (() => { CallObjFunction(subButton); });
			}
		}
	}

	/// <summary>
	/// generates the sub-menu panel
	/// </summary>
	/// <returns>the generated sub-menu panel</returns>
	/// <param name="name">name of sub-menu</param>
	GameObject GenerateGeneralMenu(string name){
		GameObject subMenu = GameObject.Instantiate(menuPrefab);
		subMenu.transform.SetParent(canvas.transform);
		subMenu.transform.position = new Vector3(-subMenu.transform.localPosition.x,90,0);
		subMenu.name = name + "Menu";
		subMenu.SetActive(false);
		subMenu.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => { BackToMenu(); });
		return subMenu;
	}

	/// <summary>
	/// adds the spaces before each uppercase letter in a string, other than the first letter
	/// </summary>
	/// <returns>the final generated string</returns>
	/// <param name="finalString">string to alter</param>
	string AddSpacesBeforeUppercase(string finalString){
		for(int j = 0; j < finalString.Length; j++){
			if(char.IsUpper(finalString[j]) == true && j - 1 >= 0 && char.IsWhiteSpace(finalString[j - 1]) == false){
				finalString = finalString.Insert(j," ");
			}
		}
		return finalString;
	}

	/// <summary>
	/// helper function that calls bmc.SetTileMode() with the correct paramter
	/// </summary>
	/// <param name="button">button calling the function</param>
	void CallTileFunction(GameObject button){
		if(button.name == "Bulldoze"){
			bmc.SetTileMode("Empty");
		} else {
			bmc.SetTileMode(button.name.Remove(0,5));
		}
	}

	/// <summary>
	/// helper function that calls bmc.SetObjMode() with the correct paramter
	/// </summary>
	/// <param name="button">button calling the function</param>
	void CallObjFunction(GameObject button){
		bmc.SetObjMode(button.name.Remove(0,5));
	}
}