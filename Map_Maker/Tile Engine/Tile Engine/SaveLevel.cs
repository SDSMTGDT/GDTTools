using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Tile_Engine
{
	class SaveLevel
	{
		// File paths and parameters
		static public string TILESET { get; set; }
		static public string SLOPEMAP { get; set; }
		static public int TILEWIDTH { get; set; }
		static public int TILEHEIGHT { get; set; }
		static public int TILESTEPX { get; set; }
		static public int TILESTEPY { get; set; }
		static public int ODDROWOFFSETX { get; set; }
		static public int HEIGHTTILEOFFSET { get; set; }
		static public int MAPWIDTH { get; set; }
		static public int MAPHEIGHT { get; set; }
		static public List<MapRow> Rows = new List<MapRow>();
		static public string Author = "";
		static public string Date = "";
		static public string Notes = "";
		static public List<TileEventObject> Events = new List<TileEventObject>();
		static public bool saved = false;

		// Store the row data passed in 
		public static void initData(List<MapRow> curRow)
		{
			Rows.AddRange(curRow);
		}

		// Write data to the specified file
		public static void write()
		{
			string filepath;
			SaveFileDialog SaveFileDialog1 = new SaveFileDialog(); // File dialog box
			SaveFileDialog1.InitialDirectory = SaveFileDialog1.InitialDirectory = @"./"; // Set directory
			SaveFileDialog1.Filter = "prm files (*.prm)|*.prm|All files (*.*)|*.*"; // Determine expected filetypes
			SaveFileDialog1.FilterIndex = 1; // Set file dialog filter
			SaveFileDialog1.RestoreDirectory = true; // Restore the directory 

			if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				filepath = SaveFileDialog1.FileName;

				// Parse out and name the file
				int index = filepath.LastIndexOf('\\');
				int extension = filepath.LastIndexOf(".prm");
				string Filename;
				if(extension != -1)
					Filename = filepath.Substring(index + 1);
				else
					Filename = filepath.Substring(index + 1) + ".prm";
			
				// Size calculations, assumes a square map
				int maxRow = Rows.Count;
				int maxCol = Rows[0].Columns.Count;

				// Create a text file with the given name and begins to write data
				using (StreamWriter s = File.CreateText(filepath))
				{
					// Basic header information
					
					s.Write('#');
					for(int i = 0; i < 63 - Filename.Length; i++)
					{
						s.Write('*');
						if(i == (64 - Filename.Length) / 2)
							s.Write(Filename);
					}
					s.WriteLine("");
					s.WriteLine("#");
					s.WriteLine("# Simple parameter data file to generate a map for an isometric");
					s.WriteLine("# tile engine. Field areas positions may be swapped but it is");
					s.WriteLine("# suggested that they remain in the standard order:");
					s.WriteLine("# Files, parameters, base tiles, height tiles, topper tiles,");
					s.WriteLine("# slope maps, and walkability.");
					s.WriteLine("# Fields are recognized by the program through their uppercase");
					s.WriteLine("# titles. I.E. TILESET FILE. Excepting these, all lines"); 
					s.WriteLine("# beginning with a # are ignored");
					s.WriteLine("#");
	
					if(Author != null)
						s.WriteLine("# Author:" + Author);
					if(Date != null)
						s.WriteLine("# Date:" + Date);
	
					if(Author != null && Date != null)
						s.WriteLine("#");

					if (Notes != null)
					{
						s.WriteLine("# Author's Notes:");
					
						string[] tokens = Notes.Split('\n');
						foreach (string line in tokens)
						{
							if(line.Length > 62)
							{
						
								int i = 0;
								string s1 = line.Substring(0, 62);

								while(s1.Length > 61)
								{
									s.Write("# " + s1 + '\n');
									i++;
									if(i * 62 + 62 < line.Length)
										s1 = line.Substring(i * 62, 62);
									else if(i * 62 < line.Length)
										s1 = line.Substring(i * 62);
								}
								s.Write("# " + s1 + '\n');
							}
							
							else
								s.Write("# " + line + '\n');
						
						}
					}

					s.WriteLine("#***************************************************************");

					// Tileset file block
					s.WriteLine("\n#------------------------------");
					s.WriteLine("# TILESET FILE");
					s.WriteLine("# Path to the tileset file the"); 
					s.WriteLine("# first tile in the set will");
					s.WriteLine("# be used as the initial base");
					s.WriteLine("# tile for all map cells");
					s.WriteLine("#------------------------------\n");
					s.WriteLine(TILESET);

					// Slope map file block
					s.WriteLine("\n#-------------------------------");
					s.WriteLine("# SLOPE MAP FILE");
					s.WriteLine("#-------------------------------\n");
					s.WriteLine(SLOPEMAP);

					// Parameter block
					s.WriteLine("\n#-----------------------");
					s.WriteLine("# TILE PARAMETERS");
					s.WriteLine("# Specify the individual");
					s.WriteLine("# tile parameters");
					s.WriteLine("#-----------------------\n");
					s.WriteLine("TILEWIDTH " + TILEWIDTH);
					s.WriteLine("TILEHEIGHT " + TILEHEIGHT);
					s.WriteLine("TILESTEPX " + TILESTEPX);
					s.WriteLine("TILESTEPY " + TILESTEPY);
					s.WriteLine("ODDROWOFFSETX " + ODDROWOFFSETX);
					s.WriteLine("HEIGHTTILEOFFSET " + HEIGHTTILEOFFSET);
					s.WriteLine("MAPWIDTH " + MAPWIDTH);
					s.WriteLine("MAPHEIGHT " + MAPHEIGHT);

					// Tile mappings
					s.WriteLine("\n#-------------------------------");
					s.WriteLine("# TILE MAPPING");
					s.WriteLine("# First number is the row,");
					s.WriteLine("# Second number is the column");
					s.WriteLine("# Third is the tile id");
					s.WriteLine("# all values are space delimited");
					s.WriteLine("#-------------------------------");

					// Base tiles
					s.WriteLine("\n#--------------------------");
					s.WriteLine("# BASE TILES");
					s.WriteLine("#--------------------------\n");

					// Walk through every tile in the list, write base tile data if it is not default
					for(int row = 0; row < maxRow; row++)
						for(int col = 0; col < maxCol; col++)
							foreach (int tile in Rows[row].Columns[col].BaseTiles)
								if(tile != 0)
									s.WriteLine(row + " " + col + " " + tile); 

					// Height tiles
					s.WriteLine("\n#------------------------");
					s.WriteLine("# HEIGHT TILES");
					s.WriteLine("#------------------------\n");

					// Walk through every tile in the list, write height tile data if it is not default
					for (int row = 0; row < maxRow; row++)
						for (int col = 0; col < maxCol; col++)
							foreach (int tile in Rows[row].Columns[col].HeightTiles)
								s.WriteLine(row + " " + col + " " + tile);

					// Topper tiles
					s.WriteLine("\n#------------------------");
					s.WriteLine("# TOPPER TILES");
					s.WriteLine("#------------------------\n");


					// Walk through every tile in the list, write topper tile data if it is not default
					for (int row = 0; row < maxRow; row++)
						for (int col = 0; col < maxCol; col++)
							foreach (int tile in Rows[row].Columns[col].TopperTiles)
								s.WriteLine(row + " " + col + " " + tile);

					// Slope maps
					s.WriteLine("\n#-----------------------");
					s.WriteLine("# SLOPE MAPS");
					s.WriteLine("# First number is row");
					s.WriteLine("# Second is column");
					s.WriteLine("# Third is slope map id");
					s.WriteLine("#-----------------------\n");

					// Walk through every tile in the list, write slope map data if it is not default
					for (int row = 0; row < maxRow; row++)
						for (int col = 0; col < maxCol; col++)
							if(Rows[row].Columns[col].SlopeMap != -1)
								s.WriteLine(row + " " + col + " " + Rows[row].Columns[col].SlopeMap);

					// Walkability 
					s.WriteLine("\n#----------------------------------");
					s.WriteLine("# WALKABILITY");
					s.WriteLine("# First number is row");
					s.WriteLine("# Second is column");
					s.WriteLine("# Third is walkable boolean,");
					s.WriteLine("# No specification defaults to true");
					s.WriteLine("#----------------------------------\n");

					// Walk through every tile in the list, walkability data if it is not default
					for (int row = 0; row < maxRow; row++)
						for (int col = 0; col < maxCol; col++)
							if(!Rows[row].Columns[col].walkable)
								s.WriteLine(row + " " + col + " 0");

					s.WriteLine("\n#---------------");
					s.WriteLine("# FUNCTIONALITY");
					s.WriteLine("#---------------\n");

					s.WriteLine("\n#------------------------------------------");
					s.WriteLine("# SWAPS");
					s.WriteLine("# First Swap Function name");
					s.WriteLine("# Second Number of Swaps");
					s.WriteLine("# Following lines contain the intial and ");
					s.WriteLine("# final 3D map locations of each cell to be ");
					s.WriteLine("# swapped.");
					s.WriteLine("# Final line is the activation area for the ");
					s.WriteLine("# function event");
					s.WriteLine("#------------------------------------------\n");

					foreach( var Event in Events)
					{
						// Verify Struct type
						if(Event.ActionData is SwapData)
						{
							// Set up Write data
							int LineCount = ((SwapData)Event.ActionData).InitialPos.Count;
							List<Microsoft.Xna.Framework.Vector3> initialPos = ((SwapData)Event.ActionData).InitialPos;
							List<Microsoft.Xna.Framework.Vector3> finalPos = ((SwapData)Event.ActionData).FinalPos;
							Microsoft.Xna.Framework.Rectangle rect = Event.ActivationArea;
							Microsoft.Xna.Framework.Vector3 ipos;
							Microsoft.Xna.Framework.Vector3 fpos;

							// Write data						
							s.WriteLine("SWAPCELLS " + LineCount);
							for(int i = 0; i < initialPos.Count; i++)
							{
								ipos = initialPos.ElementAt(i);
								fpos = finalPos.ElementAt(i);
								s.WriteLine(ipos.X + " " + ipos.Y + " " + ipos.Z + " " + fpos.X + " " + fpos.Y + " " + fpos.Z);
							}
							s.WriteLine(rect.X + " " + rect.Y + " " + rect.Width + " " + rect.Height);
							s.WriteLine("");
						}
					}

					s.WriteLine("\n#------------------------------------------");
					s.WriteLine("# SPRITESHEETCHANGES");
					s.WriteLine("# First SpriteSheetChange Function name");
					s.WriteLine("# Second Number of Animations");
					s.WriteLine("# Following lines contain the data on each");
					s.WriteLine("# animation within the spritesheet.");
					s.WriteLine("# Second to last line is the filepath of");
					s.WriteLine("# the spritesheet texture ");
					s.WriteLine("# Final line is activation area for the ");
					s.WriteLine("# function event");
					s.WriteLine("#------------------------------------------\n");
					
					foreach (var Event in Events)
					{
						// Verify Struct type
						if (Event.ActionData is SpriteSheetData)
						{
							// Set up Write data
							int LineCount = ((SpriteSheetData)Event.ActionData).Animations.Count + 1;
							List<AnimationData> animations = ((SpriteSheetData)Event.ActionData).Animations;
							Microsoft.Xna.Framework.Rectangle rect = Event.ActivationArea;

							// Write data						
							s.WriteLine("CHANGESPRITESHEETS" + LineCount);
							foreach( var animation in animations)
							{
								if(animation.NextAnimation != null)
								s.WriteLine(animation.Name + " " + animation.X + " " + animation.Y + " " + 
											animation.Width + " " + animation.Height + " " + animation.Frames + " " +
											animation.FrameLength + " " + animation.NextAnimation);
								else
								s.WriteLine(animation.Name + " " + animation.X + " " + animation.Y + " " + 
											animation.Width + " " + animation.Height + " " + animation.Frames + " " +
											animation.FrameLength);
							}
							s.WriteLine(Event.PathInfo);
							s.WriteLine(rect.X + " " + rect.Y + " " + rect.Width + " " + rect.Height);
							s.WriteLine("");
						}
					}
				

					s.WriteLine("\n#------------------------------------------");
					s.WriteLine("# TILESETCHANGES");
					s.WriteLine("# First TileSetChange Function name");
					s.WriteLine("# Second 1 ");
					s.WriteLine("# Following line contains filepath to"); 
					s.WriteLine("# tileset texture");
					s.WriteLine("# Following line contains tileset data");
					s.WriteLine("# Final line is the activation area for the ");
					s.WriteLine("# function event");
					s.WriteLine("#------------------------------------------\n");

					foreach (var Event in Events)
					{
						// Verify Struct type
						if (Event.ActionData is TileSetData)
						{
							// Set up Write data
							Microsoft.Xna.Framework.Rectangle rect = Event.ActivationArea;
							TileSetData Data = (TileSetData)Event.ActionData;

							// Write data												
							s.WriteLine("CHANGETILESETS 1");
							s.WriteLine(Data.tileWidth + " " + Data.tileHeight + " " + Data.tileStepX + " " +
										Data.tileStepY + " " + Data.oddRowOffset + " " + Data.heightTileOffset);
							s.WriteLine(Event.PathInfo);
							s.WriteLine(rect.X + " " + rect.Y + " " + rect.Width + " " + rect.Height);
							s.WriteLine("");
						}
					}
					// Format end line
					s.WriteLine("#************************** Parameter.prm***********************");

				}
			}
		}
	}
}
