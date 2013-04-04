using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// Class to determine sizing of tiles
namespace Tile_Engine
{
	
    static class Tile
    {
		public struct tileValues
		{
			public int imageIndex;
			public int type;
			public int slope;
			public int[] rotation;
			public int slopeDiv;
			public Vector2 imageDim;
			public Vector3 cubeDim;
			
			// For Base /  Topper Tiles
			/// <summary>
			/// Holds parameter data for base and topper tiles within the current tileset
			/// </summary>
			/// <param name="index">Location index into the tileset image</param>
			/// <param name="tiletype">Type of tile</param>
			/// <param name="id90">Location index of tile apperance at 90 degrees of rotation</param>
			/// <param name="id180">Location index of tile apperance at 180 degrees of rotation</param>
			/// <param name="id270">Location index of tile apperance at 270 degrees of rotation</param>
			public tileValues(int index, int tiletype, int id90, int id180, int id270)
			{
				this.imageIndex = index;
				this.type = tiletype;
				this.rotation = new int[3];
				rotation[0] = id90;
				rotation[1] = id180;
				rotation[2] = id270;
				this.slope = -1;
				this.slopeDiv = 8;
				this.imageDim = new Vector2(1.0f, 1.0f);
				this.cubeDim = new Vector3(8.0f, 8.0f, 8.0f);
				
			}

			// For Height Tiles
			/// <summary>
			/// Holds parameter data for height tiles within the current tileset
			/// </summary>
			/// <param name="index">Location index into the tileset image</param>
			/// <param name="tiletype">Type of tile</param>
			/// <param name="slopemap">Location index of the tile's slope map</param>
			/// <param name="id90">Location index of tile apperance at 90 degrees of rotation</param>
			/// <param name="id180">Location index of tile apperance at 180 degrees of rotation</param>
			/// <param name="id270">Location index of tile apperance at 270 degrees of rotation</param>
			/// <param name="sdiv">Index of the tiles ending slope location</param>
			public tileValues(int index, int tiletype, int slopemap, int sdiv, int id90, int id180, int id270)
			{
				this.imageIndex = index;
				this.type = tiletype;
				this.rotation = new int[3];
				rotation[0] = id90;
				rotation[1] = id180;
				rotation[2] = id270;
				this.slope = slopemap;
				this.slopeDiv = sdiv;
				this.imageDim = new Vector2(1.0f, 1.0f);
				this.cubeDim = new Vector3(8.0f, 8.0f, 8.0f);
			}

			// For Scenery Tiles
			/// <summary>
			/// Holds parameter data for scenery tiles within the current tileset
			/// </summary>
			/// <param name="index">Location index into the tileset image</param>
			/// <param name="tiletype">Type of tile</param>
			/// <param name="id90">Location index of tile apperance at 90 degrees of rotation</param>
			/// <param name="id180">Location index of tile apperance at 180 degrees of rotation</param>
			/// <param name="id270">Location index of tile apperance at 270 degrees of rotation</param>
			/// <param name="imWid">Width (in tiles) of the tile image</param>
			/// <param name="imHei">Height (in tiles) of the tile image</param>
			/// <param name="cubeW">Width (in cubes) of the tile in cubespace</param>
			/// <param name="cubeH">Height (in cubes) of the tile in cubespace</param>
			/// <param name="cubeD">Depth (in cubes) of the tile in cubespace</param>
			public tileValues(int index, int tiletype, int id90, int id180, int id270, 
									int imWid, int imHei, int cubeW, int cubeH, int cubeD)
			{
				this.imageIndex = index;
				this.type = tiletype;
				this.rotation = new int[3];
				rotation[0] = id90;
				rotation[1] = id180;
				rotation[2] = id270;
				this.slope = -1;
				this.slopeDiv = 8;
				this.imageDim = new Vector2(imWid, imHei);
				this.cubeDim = new Vector3(cubeW, cubeH, cubeD);
			}

		}

        static public Texture2D TileSetTexture; // Tileset texture image

		// Tile parameters
        static public int tileWidth{get; set;}
		static public int tileHeight{get; set;}
		static public int TileStepX{get; set;}
		static public int TileStepY{get; set;}
		static public int OddRowXOffset{get; set;}
		static public int HeightTileOffset{ get; set; }
		static public int MaxTileNum{ get; set; }
		static public int numTiles{ get; set; }
		static public bool hashGenerated{ get; set;}

		static public Dictionary<int, tileValues> tilesetMap;

		// Determines the number of tiles on a single row on the texture image
		// the image MUST be an even multiple of the width of a single 'tile'
		// there can be no spacing between tiles
        static public Rectangle GetSourceRectangle(int tileIndex)
        {
			int tileY; 
			int tileX;
			Vector2 tileDim;


			if(Tile.hashGenerated)
			{
				tileY = tilesetMap[tileIndex].imageIndex / (TileSetTexture.Width / tileWidth);
				tileX = tilesetMap[tileIndex].imageIndex % (TileSetTexture.Width / tileWidth);
				tileDim = tilesetMap[tileIndex].imageDim;
			}
			else
			{
				tileY = tileIndex / (TileSetTexture.Width / tileWidth);
				tileX = tileIndex % (TileSetTexture.Width / tileWidth);
				tileDim = new Vector2(1.0f, 1.0f);
			}
			

			if(tileY > (TileSetTexture.Height / tileHeight))
				return new Rectangle(-1, -1, -1, -1);

            return new Rectangle(tileX * tileWidth, tileY * tileHeight, tileWidth * (int)tileDim.X, tileHeight * (int)tileDim.Y);
        }

		static public int GetTileType(int tileIndex)
		{
			return tilesetMap[tileIndex].type;
		}

		static public int GetTileIndex(int tileIndex, int rotation)
		{
			if(!Tile.hashGenerated)
				return tileIndex;

			if(rotation == 0)
				return tileIndex;
			else
				return tilesetMap[tileIndex].rotation[rotation - 1];
		}

		static public Vector2 GetTileImageDimension(int tileIndex)
		{
			if(!Tile.hashGenerated)
				return new Vector2(1.0f, 1.0f);

			return tilesetMap[tileIndex].imageDim;
		}

		static public int GetTileSlopeMap(int tileIndex)
		{
			return tilesetMap[tileIndex].slope;
		}
		
		static public int GetTileSlopeIndex(int tileIndex)
		{
			return tilesetMap[tileIndex].slopeDiv;
		}

		static public void genTileHash(string path)
		{

			List<string> data = new List<string>(); // Holds the file line by line

			// Data aquasition control variable
			int dir = -1;
			hashGenerated = false; // list generation control

			StreamReader sr = null; // File stream reader object

			try
			{
				sr = new StreamReader(path); // Open the file for reading
				using(sr)
				{
					string s = ""; // string for line read
					while ((s = sr.ReadLine()) != null)
					{
						// Control flags are found within '#' blocks
						if (s.Contains('#'))
						{
							// Set control directions
							if (s.IndexOf("TILESET FILE") > -1)
							{
								//dir = 0;
							}

							else if (s.IndexOf("SLOPE MAP FILE") > -1)
							{
								//dir = 1;
							}

							else if (s.IndexOf("TILE PARAMETERS") > -1)
							{
								dir = 2;
							}
							else if(s.IndexOf("KEY:VALUES PAIRS") > -1)
							{
								dir = 3;
							}

						}
						// Data is found outside of '#' blocks
						else
						{
							// Parse out the current line
							ParseLine(s, dir);
						}
					}
				}
				hashGenerated = true;
				Tile.MaxTileNum = tilesetMap.Count-1;
			}	
			catch
			{
				Console.WriteLine("DID NOT GENERATE TILESET HASH!");
				Tile.MaxTileNum = 0;
			}
		}

		// Parse out the current file read line
		static private void ParseLine(string line, int dir)
		{
			if (line.CompareTo("") != 0) // Skip empty lines
			{
				string[] tokens = line.Split(' '); // tokenize the string by spaces

				switch (dir) // Data type control switching
				{
					case 0: // Tileset file path
						break;
					case 1: // Slope map file path
						break;

					case 2: // Tile parameters
						if(line.IndexOf("NUMTILES") > -1)
						{
							int capacity = int.Parse(tokens[1]);
							tilesetMap = new Dictionary<int,tileValues>(capacity);
							numTiles = int.Parse(tokens[1]);
						}
						break;

					case 3:
						// Bad line data or empty space
						if (tokens.Length < 7)
							return;

						if(tokens[2].CompareTo("base") == 0)
						{
							tilesetMap.Add(int.Parse(tokens[0]),
												new tileValues(
														int.Parse(tokens[1]),
														0,
														int.Parse(tokens[4]),
														int.Parse(tokens[5]),
														int.Parse(tokens[6])
												)
											);
						}
						else if(tokens[2].CompareTo("height") == 0)
						{
							tilesetMap.Add(int.Parse(tokens[0]),
												new tileValues(
													int.Parse(tokens[1]),
													1,
													int.Parse(tokens[3]),
													int.Parse(tokens[4]),
													int.Parse(tokens[5]),
													int.Parse(tokens[6]),
													int.Parse(tokens[7])
												)
											);
						}
						else if(tokens[2].CompareTo("topper") == 0)
						{
							tilesetMap.Add(int.Parse(tokens[0]),
												new tileValues(
														int.Parse(tokens[1]),
														2,
														int.Parse(tokens[4]),
														int.Parse(tokens[5]),
														int.Parse(tokens[6])
												)
											);
						}
						else if(tokens[2].CompareTo("scenery") == 0)
						{
							tilesetMap.Add(int.Parse(tokens[0]),
												new tileValues(
														int.Parse(tokens[1]),
														3,
														int.Parse(tokens[4]),
														int.Parse(tokens[5]),
														int.Parse(tokens[6]),
														int.Parse(tokens[7]),
														int.Parse(tokens[8]),
														int.Parse(tokens[9]),
														int.Parse(tokens[10]),
														int.Parse(tokens[11])
												)
											);
						}

						break;

					default:
						break;
				}
			}
			else { } // If no read: undetermined, at current do nothing
		}
    }
}
