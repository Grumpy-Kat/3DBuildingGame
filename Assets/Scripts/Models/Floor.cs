using UnityEngine;
using System.Collections.Generic;

public class Floor {
	Tile[,] tiles;
	int floorId;

	public Floor(int floor,World world){
		tiles = new Tile[world.width,world.height];
		floorId = floor;
		for (int x = 0; x < world.width; x++) {
			for (int y = 0; y < world.height; y++) {
				tiles[x,y] = world.GetTileAt(x,y,floorId);
				world.GetTileAt(x,y,floorId).floor = this;
			}
		}
	}
}