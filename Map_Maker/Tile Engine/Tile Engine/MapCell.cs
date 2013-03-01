using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tile_Engine
{
	// Class to store data about a given 'cell'
    class MapCell
    {

		// List variable to store the number of tiles in a given cell
		public List<int> BaseTiles = new List<int>();
		// List variable to store the heighten tiles in a given cell
		public List<int> HeightTiles = new List<int>();
		// List to hold tiles stacked upon heighten tiles
		public List<int> TopperTiles = new List<int>();

		// Bool to check if the tile is accesible
		public bool walkable{ get; set;}

		// Holds the slope mapping of the current cell
		public int SlopeMap{get; set;}

		public bool Explored{get; set;}

        // Auto properties for tileId
        public int TileId
		{ 
			// Always return the base tile
			get
			{ 
				return BaseTiles.Count > 0 ? BaseTiles[0] : 0; 
			} 
			set
			{
				// Add a tile if none
				if(BaseTiles.Count > 0)
					BaseTiles[0] = value;
				else
					AddBaseTile(value); // Add to the list of tiles if some exist
			}

		}

		// Constructor
        public MapCell(int tileID)
        {
            TileId = tileID;
			walkable = true;
			SlopeMap = -1;
			Explored = false;
        }
		
		// Helper functions to add tiles to the lists
	    public void AddBaseTile(int tileID)
		{
			bool add = true;
			foreach(var tile in BaseTiles)
				if(tileID == tile)
				{
					add = false;
					break;
				}
			if(add)
				BaseTiles.Add(tileID);
		}    
		public void InsertBaseTile(int tileID)
		{
			BaseTiles.Insert(0, tileID);
		}
		public void InsertBaseTileAt(int tileID, int index)
		{
			if(index < BaseTiles.Count)
				BaseTiles.Insert(index, tileID);
		}

		public void AddHeightTile(int tileID)
		{
			HeightTiles.Add(tileID);
		}

		public void InsertHeightTile(int tileID)
		{
			HeightTiles.Insert(0, tileID);
		}
		
		public void InsertHeightTileAt(int tileID, int index)
		{
			if(index < HeightTiles.Count )
				HeightTiles.Insert(index, tileID);
			else if(index == 0 && HeightTiles.Count == 0)
				InsertHeightTile(tileID);
			else
				HeightTiles.Add(tileID);
		}

		public void AddTopperTile(int tileID)
		{
			bool add = true;
			foreach (var tile in TopperTiles)
				if (tileID == tile)
				{
					add = false;
					break;
				}
			if (add)
				TopperTiles.Add(tileID);
		}

		public void InsertTopperTile(int tileID)
		{
			TopperTiles.Insert(0, tileID);
		}
		public void InsertTopperTileAt(int tileID, int index)
		{
			if(index < TopperTiles.Count)
				TopperTiles.Insert(index, tileID);	
		}

		public void AddSceneryTile(int x, int y, int width, int height)
		{

		}

		// Helper functions to remove items in the list
		public void RemoveBaseTile()
		{
			if(BaseTiles.Count > 1)
				BaseTiles.RemoveAt(BaseTiles.Count-1);
			
			if (BaseTiles.Count == 1)
			{
				walkable = true;
				SlopeMap = -1;
			}
		}

		public void RemoveBaseTile(int tileID)
		{
			BaseTiles.RemoveAt(BaseTiles.LastIndexOf(tileID));
			if (BaseTiles.Count == 0)
			{
				walkable = true;
				SlopeMap = -1;
			}
		}

		public void RemoveHeightTile()
		{
			if( HeightTiles.Count > 0)
				HeightTiles.RemoveAt(HeightTiles.Count - 1);
			if (HeightTiles.Count == 0)
			{
				walkable = true;
				SlopeMap = -1;
			}
		}

		public void RemoveHeightTile(int TileID)
		{
			HeightTiles.RemoveAt(HeightTiles.LastIndexOf(TileID));
			if(HeightTiles.Count == 0)
			{
				walkable = true;
				SlopeMap = -1;
			}
		}

		public void RemoveHeightTileAt(int index)
		{
			HeightTiles.RemoveAt(index);
			if(HeightTiles.Count == 0)
			{
				walkable = true;
				SlopeMap = -1;
			}
		}

		public void RemoveHeightTileAt(int index, int TileID)
		{
			if(HeightTiles.ElementAt(index) == TileID)
				HeightTiles.RemoveAt(index);

			if (HeightTiles.Count == 0)
			{
				walkable = true;
				SlopeMap = -1;
			}
		}

		public void RemoveTopperTile()
		{
			if(TopperTiles.Count > 0)
				TopperTiles.RemoveAt(TopperTiles.Count - 1);
		}

		public void RemoveTopperTile(int TileID)
		{
			TopperTiles.RemoveAt(TopperTiles.LastIndexOf(TileID));		
		}

		public void RemoveSceneryTile()
		{

		}

		public void RemoveSceneryTile(int tileID)
		{

		}

		// Reset the cell
		public void clearTile()
		{
			BaseTiles.Clear();
			HeightTiles.Clear();
			TopperTiles.Clear();
			BaseTiles.Add(0);
			walkable = true;
			SlopeMap = -1;
		}
    }
}
