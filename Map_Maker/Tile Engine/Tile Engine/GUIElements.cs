using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;

namespace Tile_Engine
{
	class GUIElements
	{
		// Gui Textures
		public Texture2D CompleteOverlay;
		public Texture2D curTileset;
		public Texture2D curTileIndicator;
	
		// Tile set control 
		public int currentTile = 0;
		public int tileSetView = 0;
		public int maxTiles = 0;

		// State variables
		public bool trash;
		public bool load;
		public bool save;
		public bool magnify;
		public bool settings;
		public bool author;
		public bool functions;
		public bool characters;

		// Square Selection Sidelength
		int SelectionSize;

		public  int selectionSize
		{
			get{ return SelectionSize; }
		}

		// Scale
		public float xguiScale;
		public float yguiScale;

		// Event item location boxes
		public List<Rectangle> ActionBox = new List<Rectangle>();
		public List<Rectangle> GuiOutline = new List<Rectangle>();

		public float tilescale = 0.4f; // Tile set image scale
		public Vector2 tileratio; // Scale conversion ratio - tile(w,h) -> tileset(64, 64)
		// Constructor
		public GUIElements(Texture2D Complete, Texture2D tileset, Texture2D curTile, Vector2 ratio, int maxtile)
		{
			// Initialize data
			CompleteOverlay = Complete;
			currentTile = 0;
			SelectionSize = 1;
			curTileset = tileset;
			curTileIndicator = curTile;
			maxTiles = maxtile;
			xguiScale = (float)camera.ViewWidth / (float)CompleteOverlay.Width;
			yguiScale = (float)camera.ViewHeight / (float)CompleteOverlay.Height;

			tileratio = ratio;//new Vector2(64.0f / (float)Tile.tileWidth, 64.0f / (float)Tile.tileHeight);
			tileratio *= tilescale;

			// Generate and scale containment boxes
			// Save Icon
			ActionBox.Add(new Rectangle((int)(550 * xguiScale), (int)(7 * yguiScale), (int)(23 * xguiScale), (int)(30 * yguiScale)));
			// Folder Icon
			ActionBox.Add(new Rectangle((int)(579 * xguiScale), (int)(9 * yguiScale), (int)(26 * xguiScale), (int)(18 * yguiScale))); 
			// Trash Icon
			ActionBox.Add(new Rectangle((int)(608 * xguiScale), (int)(84 * yguiScale), (int)(22 * xguiScale), (int)(23 * yguiScale))); 
			// Up Arrow 
			ActionBox.Add(new Rectangle((int)(617 * xguiScale), (int)(131 * yguiScale), (int)(13 * xguiScale), (int)(14 * yguiScale))); 
			// Down Arrow
			ActionBox.Add(new Rectangle((int)(617 * xguiScale), (int)(177 * yguiScale), (int)(13 * xguiScale), (int)(14 * yguiScale))); 
			// Left Arrow
			ActionBox.Add(new Rectangle((int)(597 * xguiScale), (int)(212 * yguiScale), (int)(12 * xguiScale), (int)(10 * yguiScale))); 
			// Right Arrow
			ActionBox.Add(new Rectangle((int)(614 * xguiScale), (int)(212 * yguiScale), (int)(12 * xguiScale), (int)(10 * yguiScale)));
			// Tile Select Box
			ActionBox.Add(new Rectangle((int)(503 * xguiScale), (int)(223 * yguiScale), (int)(128 * xguiScale), (int)(128 * yguiScale)));
			// Magnifying Glass Icon
			ActionBox.Add(new Rectangle((int)(609 * xguiScale), (int)(7 * yguiScale), (int)(24 * xguiScale), (int)(22 * yguiScale)));
			// Settings Icon
			ActionBox.Add(new Rectangle((int)(549 * xguiScale), (int)(34 * yguiScale), (int)(27 * xguiScale), (int)(26 * yguiScale)));
			// Author's Notes Icon
			ActionBox.Add(new Rectangle((int)(580 * xguiScale), (int)(33 * yguiScale), (int)(28 * xguiScale), (int)(29 * yguiScale)));
			// Functions Icon
			ActionBox.Add(new Rectangle((int)(606 * xguiScale), (int)(36 * yguiScale), (int)(27 * xguiScale), (int)(23 * yguiScale)));
			// Characters Icon
			ActionBox.Add(new Rectangle((int)(550 * xguiScale), (int)(63 * yguiScale), (int)(26 * xguiScale), (int)(29 * yguiScale)));

			// Toolbox containment boxes
			// Lower Box
			GuiOutline.Add(new Rectangle((int)(492 * xguiScale), (int)(202 * yguiScale), (int)(147 * xguiScale), (int)(157 * yguiScale)));
			// Upper Boxes
			GuiOutline.Add(new Rectangle((int)(542 * xguiScale), (int)(5 * yguiScale), (int)(197 * xguiScale), (int)(196 * yguiScale)));
			
			// Inital state
			ResetState();
		}

		// Draw the current gui state
		public void Draw(SpriteBatch sb, int xOffset, int yOffset, float zoom, int rotation)
		{

			// Draw the main overlay
			#region MainOverlay
			sb.Draw(
				CompleteOverlay,
				camera.WorldtoScreen(new Vector2(camera.Location.X - xOffset, camera.Location.Y - yOffset)),
				null,
				Color.White,
				0.0f,
				Vector2.Zero,
				new Vector2(xguiScale, yguiScale) / zoom,
				SpriteEffects.None,
				0.0001f);
			#endregion

			// Draw the current tileset
			#region Tileset

			// Tileset offset location
			Vector2 tileoffset = new Vector2(1006, 446) / zoom;
			Vector2 currtileoffset = new Vector2(1124, 300) / zoom;

			// Image cutoff point
			float xdiff = (128 * xguiScale / tileratio.X) - (Tile.TileSetTexture.Width);
			float ydiff = (128 * yguiScale / tileratio.Y) - (Tile.TileSetTexture.Height);

			// Starting reference locations
			int startx = 0;
			int starty = tileSetView * Math.Abs((int)ydiff);

			// Ending reference locations
			int endx;
			int endy;

			// Determine and restrict ending reference locations
			if (xdiff >= 0)
				endx = Tile.TileSetTexture.Width;
			else
				endx = startx + (int)(Tile.TileSetTexture.Width + xdiff);

			endx = (int)MathHelper.Clamp((float)endx, 0, 128 * xguiScale / tileratio.X);

			if (ydiff >= 0)
				endy = Tile.TileSetTexture.Height;
			else
				endy = starty + (int)(Tile.TileSetTexture.Height + ydiff);

			endy = (int)MathHelper.Clamp((float)endy, 0, 128 * yguiScale / tileratio.Y);

			// Draw the current tileset 
			sb.Draw(Tile.TileSetTexture,
							tileoffset,
							new Rectangle(startx, starty, endx, endy),
							Color.White,
							0.0f,
							Vector2.Zero,
							tileratio / zoom,
							SpriteEffects.None,
							0.00001f);
							
			#endregion

			// Draw the current selected tile
			#region CurrentTile

			// Extract the dimension of the tile and find the scaling, specified by a larger height or width
			Vector2 currtileDim = Tile.GetTileImageDimension(currentTile);
			float currtileScale = Math.Max(currtileDim.X, currtileDim.Y);

			sb.Draw(Tile.TileSetTexture,
							currtileoffset,
							Tile.GetSourceRectangle(Tile.GetTileIndex(currentTile, rotation)),
							Color.White,
							0.0f,
							Vector2.Zero,
							((0.75f * xguiScale)*(tileratio.X / tilescale) / zoom) / currtileScale,
							SpriteEffects.None,
							0.0f);
			#endregion

			#region CurrentTileIndicator
			Rectangle temptile = Tile.GetSourceRectangle(Tile.GetTileIndex(currentTile, rotation));
			Vector2 IndicatorPos = tileoffset + new Vector2(temptile.X, temptile.Y - tileSetView * (Math.Abs((int)ydiff))) * tileratio;

			if((IndicatorPos.Y - tileoffset.Y) < 256 && IndicatorPos.Y - tileoffset.Y >= 0)
			{
				sb.Draw(curTileIndicator,
						IndicatorPos,
						null,
						Color.White * 0.4f,
						0.0f,
						Vector2.Zero,
						currtileDim / zoom,
						SpriteEffects.None,
						0.0f);
			}
			else if((IndicatorPos.Y - tileoffset.Y ) > 192)
				this.IterateSelectedSet(false, false);

			else if((IndicatorPos.Y - tileoffset.Y) < 0)
				this.IterateSelectedSet(true, false);


			#endregion

		}

		// Select the current tile at the (x, y) index
		public bool SelectCurrentTile(int x, int y, int tileWidth, int tileHeight)
		{
			// Set restriction
			int maxSets = (int)(curTileset.Height) / (int)(128 * yguiScale / tileratio.Y);

			// Scale up the index location
			float xbox = 128 * xguiScale / tileratio.X;

			int ybox = (int)(128 * yguiScale / tileratio.Y);
			int ydiff = (int)(128 * yguiScale - Tile.TileSetTexture.Height* tileratio.Y);

			// Tileset Row/Column variables
			int row;
			int col = (int)((x - (503 * xguiScale)) / (tileWidth * tileratio.X));
			
			// Determine row index
			if(tileSetView == maxSets)
				row = (int)((y - (223 * yguiScale) + ybox*tileSetView - (ybox + ydiff)*((maxSets > 0) ? 1 : 0)) / (tileHeight * tileratio.Y));
			else
				row = (int)((y - (223 * yguiScale) + ybox *tileSetView) / (tileHeight * tileratio.X));

			if(Tile.hashGenerated)
			{
				int tempRow = (row * curTileset.Width / (tileWidth));
				int currPosIndex = col + tempRow;
				bool found = false;
			
				int i, j = 0;
				for(i = 0; i < Tile.tilesetMap.Count; i++)
				{
					// Scenery Tiles are special
					if(Tile.tilesetMap[i].type == 3)
					{
						var tile = Tile.tilesetMap[i];
						for(int imagex = 0; imagex < tile.imageDim.X; imagex++)
							for(int imagey = 0; imagey < tile.imageDim.Y; imagey++)
								if(currPosIndex == (tile.imageIndex + imagex + imagey*10 ))
								{
									found = true;
									currentTile = i;
									break;
								}

						if(found)
							break;

					}
				}
				for(i = 0; i < Tile.tilesetMap.Count && !found; i++)
				{
					if(Tile.tilesetMap[i].type != 3)
					{
						if(Tile.tilesetMap[i].imageIndex == tempRow)
						{
							for(j = 0; j < 10; j++)
							{
								if(Tile.tilesetMap[i+j].imageIndex > (tempRow + col))
									break;
							}
							break;
						}
					}
				}
			
				// Set current tile
				if(!found)
					currentTile = i + j - 1;			
			}
			else
				currentTile = col + (row * curTileset.Width / (tileWidth));

			// Assure current state
			ResetState();

			return true;
		}

		// Overloaded method that accepts an (x, y) location
		public void selectAction(int x, int y)
		{
			selectAction(new Point(x, y));
		}

		// Select a state action based off of Mouse location
		public void selectAction(Point MousePoint)
		{
			ResetState();
			if(ActionBox[0].Contains(MousePoint)) // Save Icon
			{
				save = true;
			}
			else if(ActionBox[1].Contains(MousePoint)) // Folder Icon
			{
				load = true;
			}
			else if(ActionBox[2].Contains(MousePoint)) // Trash Icon
			{
				trash = true;
			}
			else if(ActionBox[3].Contains(MousePoint)) // Up arrow
			{
				IterateSelectedTile(true);
			}			
			else if(ActionBox[4].Contains(MousePoint)) // Down arrow
			{
				IterateSelectedTile(false);
			}
			else if(ActionBox[5].Contains(MousePoint)) // Left arrow
			{
				IterateSelectedSet(true, true);
			}
			else if(ActionBox[6].Contains(MousePoint)) // Right arrow
			{
				IterateSelectedSet(false, true);
			}
			else if(ActionBox[7].Contains(MousePoint)) // Tile Select Box
			{
				SelectCurrentTile(MousePoint.X, MousePoint.Y, Tile.tileWidth, Tile.tileHeight);
				this.ResetState();
			}
			else if(ActionBox[8].Contains(MousePoint)) // Magnifying Glass
			{
				magnify = true;
			}
			else if(ActionBox[9].Contains(MousePoint)) // Settings 
			{
				settings = true;
			}
			else if(ActionBox[10].Contains(MousePoint)) // Author's
			{
				author = true;
			}
			else if(ActionBox[11].Contains(MousePoint)) // Functions 
			{
				functions = true;
			}
			else if(ActionBox[12].Contains(MousePoint)) // Characters
			{
				characters = true;
			}
			if(this.GetState())
				SelectionSize = 1;
		}

		// Move through tiles moving up or down the set based on passed bool
		public void IterateSelectedTile(bool iterate)
		{
	
			if(iterate)
				currentTile -= 1;
			
			else 
				currentTile += 1;

			currentTile = (int)MathHelper.Clamp((float)currentTile, 0, maxTiles);
		}

		// Move through tilesets moving up or down the set based on passed bool
		public void IterateSelectedSet(bool iterate, bool inctile)
		{
		
			int maxSets = Tile.TileSetTexture.Height / (int)(128 * yguiScale / tileratio.Y);

			if (iterate)
				tileSetView -= 1;

			else 
				tileSetView += 1;


			currentTile += ((iterate ? -60 : 60) * (inctile ? 1 : 0));

			tileSetView = (int)MathHelper.Clamp((float)tileSetView, 0.0f, maxSets);
			currentTile = (int)MathHelper.Clamp(currentTile, 0, maxTiles);
		}

		public void IncreaseSelectionSize(bool size)
		{
			if(size)
				SelectionSize += 1;
			else
				SelectionSize -= 1;

			SelectionSize = (int)MathHelper.Clamp((float)SelectionSize, 1f, 11f);
		}

		public bool GetState()
		{
			return save || load || trash || magnify || settings || author || functions || characters;
		}

		public void ResetState()
		{
			save = false;
			load = false;
			trash = false;
			magnify = false;
			settings = false;
			author = false;
			functions = false;
			characters = false;
		}
		
	}
}
