using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Tile_Engine
{
	
	class ReadLevel
	{
		// Parameters
		static public Texture2D TILESET { get; set; }
		static public Texture2D SLOPEMAP { get; set; }
		static public int TILEWIDTH { get; set; } 
		static public int TILEHEIGHT { get; set; }
		static public int TILESTEPX { get; set; }
		static public int TILESTEPY { get; set; }
		static public int ODDROWOFFSETX { get; set; }
		static public int HEIGHTTILEOFFSET { get; set; }
		static public int MAPWIDTH { get; set; }
		static public int MAPHEIGHT { get; set; }

		// Read path 
		static public string filepath { get; set; }

		// Read control variables
		static bool bheight = false, bwidth = false;

		// 2D list to store level data
		public static List<MapRow> Rows = new List<MapRow>();

		// Control and content 
		static public ContentManager content;
		static bool bswaps = false, btilesets = false, bspritesheets = false;
		static int LineReadCount = -1;
		static string TexturePath = "";

		public static List<TileEventObject> Events = new List<TileEventObject>();
		private static List<Vector3> initialPos = new List<Vector3>();
		private static List<Vector3> finalPos = new List<Vector3>();
		private static List<AnimationData> animations = new List<AnimationData>();
		private static TileSetData tileset = new TileSetData();

		// Parse out the current file read line
		public static void ParseLine(string line, int dir)
		{
			if (line.CompareTo("") != 0) // Skip empty lines
			{
				string[] tokens = line.Split(' '); // tokenize the string by spaces

				int row, col, id; // data acquasition variables

				switch (dir) // Data type control switching
				{
					case 0: // Tileset file path
						TILESET = content.Load<Texture2D>(@line);
						break;
					case 1: // Slope map file path
						SLOPEMAP = content.Load<Texture2D>(@line);
						break;

					case 2: // Tile parameters
						if(line.IndexOf("MAPWIDTH") > -1)
						{
							MAPWIDTH = int.Parse(tokens[1]);
							bwidth = true; // Set flag for width achieval
						}
						else if(line.IndexOf("MAPHEIGHT") > -1)
						{
							MAPHEIGHT = int.Parse(tokens[1]);
							bheight = true; // Set flag for height achieval
						}
						else if (line.IndexOf("TILEWIDTH") > -1)
						{
							TILEWIDTH = int.Parse(tokens[1]);
						}
						else if (line.IndexOf("TILEHEIGHT") > -1)
						{
							TILEHEIGHT = int.Parse(tokens[1]);
						}
						else if (line.IndexOf("TILESTEPX") > -1)
						{
							TILESTEPX = int.Parse(tokens[1]);
						}
						else if (line.IndexOf("TILESTEPY") > -1)
						{
							TILESTEPY = int.Parse(tokens[1]);
						}
						else if (line.IndexOf("ODDROWOFFSETX") > -1)
						{
							ODDROWOFFSETX = int.Parse(tokens[1]);
						}
						else if (line.IndexOf("HEIGHTTILEOFFSET") > -1)
						{
							HEIGHTTILEOFFSET = int.Parse(tokens[1]);
						}
						break;

					case 3: // Base tiles
					case 4: // Height tiles
					case 5: // Topper tiles
					case 6: // Slope maps
					case 7: // Walkability
						row = int.Parse(tokens[0]);
						col = int.Parse(tokens[1]);
						id = int.Parse(tokens[2]);

						if (dir == 3) // Base tileds
							Rows[row].Columns[col].TileId = id;
						if (dir == 4) // Height tiles
							Rows[row].Columns[col].AddHeightTile(id);
						if (dir == 5) // Topper tiles
							Rows[row].Columns[col].AddTopperTile(id);
						if (dir == 6) // Slope maps
							Rows[row].Columns[col].SlopeMap = id;
						if (dir == 7) // Walkability
							Rows[row].Columns[col].walkable = (id == 0 ? false : true);
						break;

					case 8: // Functions

						#region Function Read Conditions
						if (bswaps)
						{
							LineReadCount--;
							if (LineReadCount < 0)
							{
								Events.Add(new TileEventObject(new SwapData(new List<Vector3>(initialPos), new List<Vector3>(finalPos)),
											new Rectangle(int.Parse(tokens[0]), int.Parse(tokens[1]),
															int.Parse(tokens[2]), int.Parse(tokens[3]))));
								initialPos.Clear();
								finalPos.Clear();
								bswaps = false;
							}
							else
							{
								initialPos.Add(new Vector3(float.Parse(tokens[0]), float.Parse(tokens[1]), float.Parse(tokens[2])));
								finalPos.Add(new Vector3(float.Parse(tokens[3]), float.Parse(tokens[4]), float.Parse(tokens[5])));
							}
						}

						if (bspritesheets)
						{
							LineReadCount--;
							if (LineReadCount < 0)
							{

								if (animations.Count > 0)
									Events.Add(new TileEventObject(new SpriteSheetData(content.Load<Texture2D>(@TexturePath), new List<AnimationData>(animations)),
												new Rectangle(int.Parse(tokens[0]), int.Parse(tokens[1]),
															  int.Parse(tokens[2]), int.Parse(tokens[3])),
															  TexturePath));
								else
									Events.Add(new TileEventObject(new SpriteSheetData(content.Load<Texture2D>(@TexturePath)),
												new Rectangle(int.Parse(tokens[0]), int.Parse(tokens[1]),
															  int.Parse(tokens[2]), int.Parse(tokens[3])),
															  TexturePath));

								bspritesheets = false;
								TexturePath = "";
								animations.Clear();
							}
							else if (LineReadCount == 0)
							{
								TexturePath = tokens[0];
							}
							else if (LineReadCount > 0)
							{
								if (tokens.Length == 7)
									animations.Add(new AnimationData(tokens[0], int.Parse(tokens[1]), int.Parse(tokens[2]),
															int.Parse(tokens[3]), int.Parse(tokens[4]),
															int.Parse(tokens[5]), int.Parse(tokens[6])));
								else if (tokens.Length == 8)
									animations.Add(new AnimationData(tokens[0], int.Parse(tokens[1]), int.Parse(tokens[2]),
															int.Parse(tokens[3]), int.Parse(tokens[4]),
															int.Parse(tokens[5]), int.Parse(tokens[6]),
															tokens[7]));
							}

						}

						if (btilesets)
						{
							LineReadCount--;
							if (LineReadCount < 0)
							{
								btilesets = false;
								Events.Add(new TileEventObject(tileset,
											new Rectangle(int.Parse(tokens[0]), int.Parse(tokens[1]),
														  int.Parse(tokens[2]), int.Parse(tokens[3])),
														  TexturePath));
							}
							else if (LineReadCount == 0)
							{
								TexturePath = tokens[0];
								tileset.TileSet = content.Load<Texture2D>(tokens[0]);
							}
							else if(LineReadCount > 0)
							{
								tileset.tileWidth = int.Parse(tokens[0]);
								tileset.tileHeight = int.Parse(tokens[1]);
								tileset.tileStepX = int.Parse(tokens[2]);
								tileset.tileStepY = int.Parse(tokens[3]);
								tileset.oddRowOffset = int.Parse(tokens[4]);
								tileset.heightTileOffset = int.Parse(tokens[5]);
							}

						}
						#endregion

						#region Function Indicators
						if (line.IndexOf("SWAPCELLS") > -1)
						{
							bswaps = true;
							LineReadCount = int.Parse(tokens[1]);

						}
						if (line.IndexOf("CHANGESPRITESHEETS") > -1)
						{
							bspritesheets = true;
							LineReadCount = int.Parse(tokens[1]);
						}
						if (line.IndexOf("CHANGETILESETS") > -1)
						{
							btilesets = true;
							LineReadCount = int.Parse(tokens[1]);
						}
						#endregion

						break;

					default:
						break;
				}

			}
			else {} // If no read: undetermined, at current do nothing
		}

		// Begin reading the level data through the use of a dialog box		
		public static bool LoadLevel()
		{

			List<string> data = new List<string>(); // Holds the file line by line

			// Data aquasition control variable
			int dir = -1;
			bool generate = true; // list generation control

			StreamReader sr = null; // File stream reader object
			OpenFileDialog openFileDialog1 = new OpenFileDialog(); // File dialog box
			openFileDialog1.InitialDirectory = openFileDialog1.InitialDirectory = @"./"; // Set directory
			openFileDialog1.Filter = "prm files (*.prm)|*.prm|All files (*.*)|*.*"; // Determine expected filetypes
			openFileDialog1.FilterIndex = 1; // Set file dialog filter
			openFileDialog1.RestoreDirectory = true; // Restore the directory 

			// If the file dialog worked correctly
			if(openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				try
				{
					// If the file exists
					if ((openFileDialog1.OpenFile() != null))
					{
						
						filepath = openFileDialog1.FileName;// Store the selected filepath
						sr = new StreamReader(filepath); // Open the file for reading
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
										dir = 0;
									}

									else if (s.IndexOf("SLOPE MAP FILE") > -1)
									{
										dir = 1;
									}

									else if (s.IndexOf("TILE PARAMETERS") > -1)
									{
										dir = 2;
									}

									else if (bheight && bwidth && s.IndexOf("BASE TILES") > -1)
									{
										dir = 3;
									}
									else if (bheight && bwidth && s.IndexOf("HEIGHT TILES") > -1)
									{
										dir = 4;
									}
									else if (bheight && bwidth && s.IndexOf("TOPPER TILES") > -1)
									{
										dir = 5;
									}
									else if (bheight && bwidth && s.IndexOf("SLOPE MAPS") > -1)
									{
										dir = 6;
									}
									else if (bheight && bwidth && s.IndexOf("WALKABILITY") > -1)
									{
										dir = 7;
									}
									else if (s.IndexOf("FUNCTIONALITY") > -1)
									{
										dir = 8;
									}

								}
								// Data is found outside of '#' blocks
								else
								{
									// Parse out the current line
									ParseLine(s, dir);

									// If the list hasn't been generated AND the dimensions are known
									if (generate && bwidth && bheight)
									{
										
										// Generate the list of size MAPHEiGHT * MAPWIDTH
										for (int y = 0; y < MAPHEIGHT; y++)
										{
											MapRow thisRow = new MapRow();
											for (int x = 0; x < MAPWIDTH; x++)
											{
												thisRow.Columns.Add(new MapCell(0));
											}
											Rows.Add(thisRow);
										}
										generate = false; // Don't recreate list 
									}
								}
							}
						}
						return true; // read sucessful
					}
					else // read failure, file does not exists or won't open
						return false;
				}
				catch( Exception ex) // Read failure, display error message
				{
					MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
					return false;
				}
			}
			// Read failure Dialog box did not open correctly
			else 
			{
				return false;
			}
		}
		public static void ResetRead()
		{
			Rows.Clear();
			Events.Clear();
			initialPos.Clear();
			finalPos.Clear();
			animations.Clear();

			bheight = false;
			bwidth = false;
			bswaps = false;
			btilesets = false;
			bspritesheets = false;
			LineReadCount = -1;

			TILEHEIGHT = 0;
			TILEWIDTH = 0;
			TILESTEPX = 0;
			TILESTEPY = 0;
			TILESET = null;
			SLOPEMAP = null;
			HEIGHTTILEOFFSET = 0;
			ODDROWOFFSETX = 0;
			MAPHEIGHT = 0;
			MAPWIDTH = 0;

		}
	}
}
