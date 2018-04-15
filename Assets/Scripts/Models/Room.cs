using UnityEngine;
using System.Collections.Generic;

public class Room {

	public List<Tile> tiles;

	public Room(){
		tiles = new List<Tile>();
	}

	public void AssignTile(Tile t){
		if(tiles.Contains(t)){
			//the tile is already part of this room
			return;
		}
		if(t.room != null){
			//belongs to another room
			t.room.tiles.Remove(t);
		}
		t.room = this;
		tiles.Add(t);
	}

	public void UnassignAllTiles(){
		for(int i = 0; i < tiles.Count;i++){
			tiles[i].room = tiles[i].world.GetOutsideRoom();
		}
		tiles = new List<Tile>();
	}

	public static void RoomFloodFill(Object sourceObj){
		//sourceObj is the furniture that may be splitting two rooms or forming a new one
		foreach(Tile t in sourceObj.tiles){
			Room newRoom = new Room();
			World world = t.world;
			Room oldRoom = t.room;
			if(ActualFloodFill(t,newRoom) == 0){
				//tell World that new room has been formed
				world.AddRoom(newRoom);
				//if not outside, delete the room
				if(oldRoom != null && oldRoom != world.GetOutsideRoom()){
					//oldRoom should have no tiles left
					if(oldRoom.tiles.Count > 0){
						Debug.LogError("oldRoom still has tiles left in it!");
					}
					world.DeleteRoom(oldRoom);
				}
			} else{
				newRoom.UnassignAllTiles();
			}
		}
	}
	
	protected static int ActualFloodFill(Tile t,Room newRoom){
		World world = t.world;
		Room oldRoom = t.room;
		if(t == null || (t.Type == TileType.Empty || t.Type == TileType.Grid)){
			//trying to flood fill of the map or on empty space
			return 1;
		}
		if(oldRoom == null){
			//is not a floor tile
			return 1;
		}
		//try building room for each neighbor
		if(oldRoom != newRoom){
			newRoom.AssignTile(t);
		}
		Dictionary<string,Tile> directions = new Dictionary<string,Tile>(4);
		BuildModeController bmc = WorldController.Instance.MouseController.bmc;
		if(t.x-1 < 0){
			directions.Add("left",null);
		} else{
			directions.Add("left",world.GetTileAt(t.x-1,t.y,bmc.ActiveFloor));
		}
		if(t.x+1 >= world.width){
			directions.Add("right",null);
		} else{
			directions.Add("right",world.GetTileAt(t.x+1,t.y,bmc.ActiveFloor));
		}
		if(t.y-1 < 0){
			directions.Add("down",null);
		} else{
			directions.Add("down",world.GetTileAt(t.x,t.y-1,bmc.ActiveFloor));
		}
		if(t.y+1 >= world.height){
			directions.Add("up",null);
		} else{
			directions.Add("up",world.GetTileAt(t.x,t.y+1,bmc.ActiveFloor));
		}
		foreach(Object obj in t.objs){
			if(obj.connectsToNeighbors){
				if(t.x == obj.pos.x-0.5 && directions.ContainsKey("right")){
					directions.Remove("right");
				} else if(t.x == obj.pos.x+0.5 && directions.ContainsKey("left")){
					directions.Remove("left");
				} else if(t.y == obj.pos.z-0.5 && directions.ContainsKey("up")){
					directions.Remove("up");
				} else if(t.y == obj.pos.z+0.5 && directions.ContainsKey("down")){
					directions.Remove("down");
				}
			}
		}
		if(directions.ContainsKey("right") && t.x+1 >= world.width){
			return 1;
		}
		if(directions.ContainsKey("left") && t.x-1 < 0){
			return 1;
		}
		if(directions.ContainsKey("up") && t.y+1 >= world.height){
			return 1;
		}
		if(directions.ContainsKey("down") && t.y-1 < 0){
			return 1;
		}
		foreach(string key in directions.Keys){
			if(directions[key].room != newRoom){
				if(ActualFloodFill(directions[key],newRoom) == 1){
					return 1;
				}
			}
		}
		return 0;
	}
}