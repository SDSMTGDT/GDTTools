using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Tile_Engine
{
	// Uses a list of lists to store the map cell data and their associated tiles
    class MapRow
    {
        public List<MapCell> Columns = new List<MapCell>();
    }

    class TileMap
    {

		public bool debcheck = false;

        public List<MapRow> Rows = new List<MapRow>(); // 2 Dimensional list of map cells
		public List<int> Layers = new List<int>(); // List of chunked out map layers
        public int mapWidth { get; set; } // Allowed map width
        public int mapHeight { get; set; } // Allowed map height
		private Texture2D mouseMap; // Mouse-Cell maping texture
		private Texture2D slopeMaps; // Slope maps texture
		private int Rotation;
		public int rotation
		{
			get{ return Rotation; }
			set { Rotation = (value + 4) % 4; }
		}
		
		// Exchange level data sets
		public void ChangeDataSets(List<MapRow> rows, Texture2D slopeMaps = null, Texture2D mouseMap = null)
		{
			Rotation = 0;
			Rows.Clear(); // Clear out the current level
			Rows.AddRange(rows); // Copy the contents of the loaded level into the local list
			if(slopeMaps != null)
				this.slopeMaps = slopeMaps;
			if(mouseMap != null)
				this.mouseMap = mouseMap;

			// Assumes a square map
			if(Rows.Count != 0)
			{
				mapWidth = rows[0].Columns.Count;
				mapHeight = rows.Count;
			}
			else
			{
				mapWidth = 0;
				mapHeight = 0;
			}
		}

		// Constructor, walk through our map, assign default tiles, then add some specific ones
        public TileMap(Texture2D mouseMap, Texture2D slopeMap)
        {
			this.mouseMap = mouseMap;
			this.slopeMaps = slopeMap;
			mapHeight = 99;
			mapWidth = 50;
			Rotation = 0;
			for(int y = 0; y < mapHeight; y++)
			{
				MapRow thisrow = new MapRow();
				for(int x = 0; x < mapWidth; x++)
				{ 
					thisrow.Columns.Add(new MapCell(0));
				}
				Rows.Add(thisrow);
			}
        }

		// Swap cells at the index locations
		public void SwapCell(int x, int y, int nx, int ny)
		{
			MapCell temp = Rows[x].Columns[y];
			Rows[x].Columns[y] = Rows[nx].Columns[ny];
			Rows[nx].Columns[ny] = temp;
		}

		// Overload function that removes the use of the out variable
		public Point WorldToMapCell(Point WorldPoint)
		{
				Point dummy;
				return WorldToMapCell(WorldPoint, out dummy);
		}

		// Convert world coordinates to MapCell coordinates
		public Point WorldToMapCell(Point WorldPoint, out Point localPoint)
		{
			// Point to represent map cell coordinates
			// Transformed into a rectange representation
			Point mapCell = new Point(
				(int)(WorldPoint.X / mouseMap.Width),
				((int)(WorldPoint.Y / mouseMap.Height)) * 2);

			// Holds the 'pixel in question' point 
			int localPointX = WorldPoint.X % mouseMap.Width;
			int localPointY = WorldPoint.Y % mouseMap.Height;

			// Slope direction values
			int dx = 0;
			int dy = 0;

			uint[] myUint = new uint[1];

			// See if our pixel is contained within the rectangle 'map'
			if(new Rectangle(0, 0, mouseMap.Width, mouseMap.Height).Contains(localPointX, localPointY))
			{
				// Extract a single pixel from the mouseMap and store it in a uint
				mouseMap.GetData(0, new Rectangle(localPointX,localPointY, 1, 1), myUint, 0, 1);

				if(myUint[0] == 0xFF0000FF) // Red
				{
					dx = -1;
					dy = -1;
					localPointX = localPointX + (mouseMap.Width / 2);
					localPointY = localPointY + (mouseMap.Height / 2);
				}

				if(myUint[0] == 0xFF00FF00) // Green
				{
					dx = -1;
					dy = 1;
					localPointX = localPointX + (mouseMap.Width / 2);
					localPointY = localPointY - (mouseMap.Height / 2);
				}

				if(myUint[0] == 0xFF00FFFF) // Yellow
				{
					dy = -1;
					localPointX = localPointX - (mouseMap.Width / 2);
					localPointY = localPointY + (mouseMap.Height / 2);
				}

				if(myUint[0] == 0xFFFF0000) // Blue
				{
					dy = +1;
					localPointX = localPointX - (mouseMap.Width / 2);
					localPointY = localPointY - (mouseMap.Height / 2);
				}
			}

			mapCell.X += dx;
			mapCell.Y += dy - 2;
			
			
			localPoint = new Point((int)(localPointX), (int)(localPointY));



			return mapCell;

		}

		// Conversion functions to translate worldpoints to cellmap points
		public Point WorldToMapCell(Vector2 worldPoint)
		{
			return WorldToMapCell(new Point((int)worldPoint.X, (int)worldPoint.Y));
		}

		// Retreive the cell at the world coordinate location, uses a point
		public MapCell GetCellAtWorldPoint(Point worldPoint)
		{
			Point mapPoint = WorldToMapCell(worldPoint);
			return Rows[mapPoint.Y].Columns[mapPoint.X];
		}

		// Retreive the cell at the world coordinate location, uses a vector
		public MapCell GetCellAtWorldPoint(Vector2 worldPoint)
		{
			return GetCellAtWorldPoint(new Point((int)worldPoint.X, (int)worldPoint.Y));
		}

		// Retrieve the color of a pixel on the map at the local point
		public int GetSlopeMapHeight(Point localPixel, int slopeMap)
		{
			Point texturePoint = new Point(slopeMap * mouseMap.Width + localPixel.X, localPixel.Y);

			Color[] slopeColor = new Color[1];

			if(new Rectangle(0, 0, slopeMaps.Width, slopeMaps.Height).Contains(texturePoint.X, texturePoint.Y))
			{
				// Grab the pixel data from the point using slopecolor, which as a 1 value color is grayscale
				slopeMaps.GetData(0, new Rectangle(texturePoint.X, texturePoint.Y, 1, 1), slopeColor, 0, 1);

				// Normalize the pixel data between 0 and 1
				int offset = (int)(((float)(255 - slopeColor[0].R) / 255f) * Tile.HeightTileOffset);

				return offset;
			}

			return 0;
		}

		// Retrieve the height of the tile at the world point
		public int GetSlopeHeightAtWorldPoint(Point worldPoint)
		{
			Point localPoint;
			Point mapPoint = WorldToMapCell(worldPoint, out localPoint);
			int slopeMap = Rows[mapPoint.Y].Columns[mapPoint.X].SlopeMap;

			return GetSlopeMapHeight(localPoint, slopeMap);
		}
		
		// Retreive the height of the tile at the world point, uses a vector
		public int GetSlopeHeightAtWorldPoint(Vector2 worldPoint)
		{
			return GetSlopeHeightAtWorldPoint(new Point((int)worldPoint.X, (int)worldPoint.Y));
		}

		// Total up the height of the map tile AND the slope at that point 
		public int GetOverallHeight(Point worldPoint)
		{
			Point mapCellPoint = WorldToMapCell(worldPoint);
			int height;
			
			if(Rows[mapCellPoint.Y].Columns[mapCellPoint.X].HeightTiles.Count >= 1 && Rows[mapCellPoint.Y].Columns[mapCellPoint.X].SlopeMap != -1)
				height = (Rows[mapCellPoint.Y].Columns[mapCellPoint.X].HeightTiles.Count - 1) * Tile.HeightTileOffset;
			else
				height = Rows[mapCellPoint.Y].Columns[mapCellPoint.X].HeightTiles.Count * Tile.HeightTileOffset;

			height += (int)(GetSlopeHeightAtWorldPoint(worldPoint) * (Rows[mapCellPoint.Y].Columns[mapCellPoint.X].SlopeDivision/ 8.0f));

			return height;
		}

		// Gets the height of the map tile AND the slope at that point, uses a vector
		public int GetOverallHeight(Vector2 worldPoint)
		{
			return GetOverallHeight(new Point((int) worldPoint.X, (int) worldPoint.Y));
		}

		public void addLayers()
		{
			for(int y = 0; y < mapWidth; y++)
			{
				for(int x = 0; x < mapHeight; x++)
				{
					Rows[x].Columns[y].InsertHeightTileAt(Layers.ElementAt(Layers.Count - 1), Layers.Count-1);
				}
			}
		}

		public void removeLayers(int numLayers)
		{
			for(int y = 0; y < mapWidth; y++)
			{
				for(int x = 0; x < mapHeight; x++)
				{
					for(int n = 0; n < numLayers; n++)
					{
						Rows[x].Columns[y].RemoveHeightTileAt(Layers.Count - n - 1);
					}
				}
			}
		}

		public void ResetLayers(List<int> newLayers)
		{
			Rectangle test;
			for (int y = 0; y < mapWidth; y++)
			{
				for (int x = 0; x < mapHeight; x++)
				{
					for(int n = Layers.Count - 1; n >= 0; n--)
					{
						var tile = Layers.ElementAt(n);
						if(n < Rows[x].Columns[y].HeightTiles.Count && tile == Rows[x].Columns[y].HeightTiles.ElementAt(n))
							Rows[x].Columns[y].HeightTiles.RemoveAt(n);	
					}
				}
			}
			 
			Layers.Clear();
			Layers = new List<int>(newLayers);
			for (int y = 0; y < mapWidth; y++)
			{
				for (int x = 0; x < mapHeight; x++)
				{
					for(int n = 0; n < Layers.Count; n++)
					{
						int tile = Layers.ElementAt(n);
						test = Tile.GetSourceRectangle(tile);

						if(test.X != -1)
							Rows[x].Columns[y].InsertHeightTile(Layers.ElementAt(Layers.Count - n - 1));
						else
							while(Layers.Remove(tile));
					}
				}
			}			
		}
		
		public void RotateMapClockwise(bool direction)
		{
			int height = mapHeight - 1;
			int width = mapWidth -1;
			List<MapRow> newRow = new List<MapRow>();
			
			for(int row = 0; row < mapHeight; row++)
			{
				MapRow thisRow = new MapRow();
				for(int col = 0; col < mapWidth; col++)
					thisRow.Columns.Add(new MapCell(0));
				newRow.Add(thisRow);
			}

			for(int row = 0; row < mapHeight; row++)
				for (int col = 0; col < mapWidth; col++)
					if (col != width || (row % 2 == 0)) // Disallow rotation of odd tiles in final column
					{
						// Clockwise Rotation
						if (direction) // Jab the system and rotate
						{
							newRow[2 * col + (row % 2)].Columns[width - (row + 1) / 2] = Rows[row].Columns[col];
							this.rotation--;
						}
						// CounterClockwise Rotation
						else // Mesh the system and rotate
						{
							newRow[height - (2 * col + (row % 2))].Columns[row / 2] = Rows[row].Columns[col];
							this.rotation++;
						}
					}
			Rows.Clear();
			Rows.AddRange(newRow);
		}			
    }
}
