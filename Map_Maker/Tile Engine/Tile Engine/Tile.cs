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
			public int type;
			public int slope;
			public int[] rotation;
			public tileValues(int tiletype, int slopemap, int id90, int id180, int id270)
			{
				this.type = tiletype;
				this.slope = slopemap;
				rotation = new int[3];
				this.rotation[0] = id90;
				this.rotation[1] = id180;
				this.rotation[2] = id270;
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
			int tileY = tileIndex / (TileSetTexture.Width / tileWidth);
			int tileX = tileIndex % (TileSetTexture.Width / tileWidth);
			if(tileY > (TileSetTexture.Height / tileHeight))
				return new Rectangle(-1, -1, -1, -1);

            return new Rectangle(tileX * tileWidth, tileY * tileHeight, tileWidth, tileHeight);
        }

		static public int GetTileType(int tileIndex)
		{
			return tilesetMap[tileIndex].type;
		}

		static public int GetTileIndex(int tileIndex, int rotation)
		{
			if(rotation == 0)
				return tileIndex;
			else
				return tilesetMap[tileIndex].rotation[rotation - 1];
		}

		static public int GetTileSlopeMap(int tileIndex)
		{
			return tilesetMap[tileIndex].slope;
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
			}	
			catch
			{
				Console.WriteLine("DID NOT GENERATE TILESET MAPPING");
			};	
		}

		// Parse out the current file read line
		static private void ParseLine(string line, int dir)
		{
			if (line.CompareTo("") != 0) // Skip empty lines
			{
				string[] tokens = line.Split(' '); // tokenize the string by spaces

				int key, type, slope; // data acquasition variables

				switch (dir) // Data type control switching
				{
					case 0: // Tileset file path
						break;
					case 1: // Slope map file path
						break;

					case 2: // Tile parameters
						if(line.IndexOf("NUMTILES") > -1)
						{
							tilesetMap = new Dictionary<int,tileValues>(int.Parse(tokens[1]));
							for(int k = 0; k < tilesetMap.Count; k++)
								tilesetMap[k] = new tileValues(-1, -1, -1, -1, -1);

							numTiles = int.Parse(tokens[1]);
						}
						break;

					case 3:
						type = -1;

						if(tokens[1].CompareTo("base") == 0)
							type = 0;
						else if(tokens[1].CompareTo("height") == 0)
							type = 1;
						else if(tokens[1].CompareTo("topper") == 0)
							type = 2;
						else if(tokens[1].CompareTo("scenery") == 0)
							type = 3;
						if(type != 3)
							tilesetMap[int.Parse(tokens[0])] = new tileValues(type, int.Parse(tokens[2]),
																int.Parse(tokens[3]), int.Parse(tokens[4]), int.Parse(tokens[5]));
						else
							tilesetMap[int.Parse(tokens[0])] = new tileValues(type, int.Parse(tokens[2]),
															int.Parse(tokens[3]), int.Parse(tokens[4]), -1);
						
						break;

					default:
						break;
				}
			}
			else { } // If no read: undetermined, at current do nothing
		}
    }
}
