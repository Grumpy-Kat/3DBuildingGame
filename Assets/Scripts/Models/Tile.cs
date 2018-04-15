using UnityEngine;
using System;
using System.Collections.Generic;

public enum TileType {Empty,Grid,Wood,Tile};

public class Tile {
	
	//default TileType
	private TileType type = TileType.Empty;
	public TileType Type {
		get {
			return type;
		}
		set {
			TileType oldType = type;
			type = value;
			//call the callback to alert change and change room
			if(cbTileChanged != null && oldType != type){
				cbTileChanged(this);
			}
		}
	}
	//actions
	Action<Tile> cbTileChanged;
	//Tile data
	public World world { get; protected set; }
	public Room room;
	public Floor floor;
	public int x { get; protected set; }
	public int y { get; protected set; }
	public float width { get; protected set; }
	public float height { get; protected set; }
	public List<Object> objs { get; protected set; }
	int rotLevel;
	public int RotLevel {
		get {
			return rotLevel;
		}
		set {
			int oldRotLevel = rotLevel;
			rotLevel = value;
			//call the callback to alert change
			if(cbTileChanged != null && oldRotLevel != rotLevel){
				cbTileChanged(this);
			}
		}
	}

	/// <summary>
	/// initializes a new instance of the <see cref="Tile"/> class (constructer)
	/// </summary>
	/// <param name="world">World Object that the tile is in</param>
	/// <param name="x">the x coordinate of the tile</param>
	/// <param name="y">the y coordinate of the tile</param>
	public Tile(World world,Room room,int x,int y){
		this.world = world;
		this.room = room;
		this.x = x;
		this.y = y;
		width = 1;
		height = 1;
		objs = new List<Object>();
		rotLevel = 0;
	}

	/// <summary>
	/// registering/unregistering TileChanged callback
	/// </summary>
	/// <param name="cb">callback</param>
	public void RegisterTileChanged(Action<Tile> cb){
		cbTileChanged += cb;
	}
	
	public void UnregisterTileChanged(Action<Tile> cb){
		cbTileChanged -= cb;
	}
}