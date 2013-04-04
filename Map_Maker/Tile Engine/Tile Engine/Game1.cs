using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

/// Summary 
/// Generates, Maintains, and allows Graphical Augmentation of 
/// an isometric tile map
/// 
/// Author:
///	Hunter Feltman
///	
/// Mapping Code modified from Kurt Jaegers 
/// 

namespace Tile_Engine
{
    /// <summary>
    /// Game Controller Class
	/// Handles Initialization, Drawing, and Event Routing.
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics; // Graphics manager
        SpriteBatch spriteBatch; // Sprite batch
		SpriteFont pericles6; // Usable font

		// Keypress event states
		KeyboardState current; // Current state of the Keyboard
		KeyboardState previous; // Last state of the keyboard

		TileMap myMap; // Create a new map class
		GUIElements gui; // Create a new gui control class

		// List of Map events, stored virtually for later encoding
		List<TileEventObject> Events;

		// Helpful Settings and Editing Forms
		Settings setbox; // Change Tile, Editor, and Map Settings
		AuthorsBox autbox; // Add notes to Level File
		FuncBox funcbox; // Add Functionality to Map
		KeyboardBox keybox; // Control Editor Shortcut Keys
		CharacterBox charbox; // Change Character Data

		Point StartingPos; // Character Starting Position, Map Coordinates

		// Camera window size, in Tiles
        float squaresAcross = 24;
        float squaresDown = 54;

		// Camera offsets, in Pixels
		int baseOffsetX = -32;
		int baseOffsetY = -64;

		// Depth augment variable to draw tiles correctly
		float heightRowDepthMod = 0.000001f;

		// Tile Drawing Ratio for Over/Under sized tiles
		Vector2 drawratio;
	
		// View display state variables
		int displayCoords = 0;
		int ShowEvents = 0;
		int view = 0;
		int preview;
		int slopeMap = -1;
		int tileType = 0;

		const int ViewFrames = 3;

		// Keypress Single/Continuous press variables
		float waitTime = 1.4f;
		float CurrentWait = 0;
		float ShiftWait = 0;
		bool walkable = true;

		// Texture storage variables
		Texture2D hilight;
		Texture2D trash;
		Texture2D MagGlass;
		Texture2D sStart;
		Texture2D sEnd;
		Texture2D activeArea;
		Texture2D Wireframe;
		Texture2D Flatframe;
		Texture2D charStart;
		Texture2D WireframeView;
		Texture2D NoWalk;
		Texture2D CubeText;

		// Magnifiying glass image offset, in Pixels
		int MagOffset = 25;

		// IO/Editor State variables
		bool ctrlkey = false;
		bool sftkey = false;
		bool lpressed = false;
		bool rpressed = false;
		bool keyshortcuts = false;
		bool formwaiting = false;
		bool makemap = true;
		bool fastIterate = false;
		bool go = true;
		bool tileIterate = false;
		bool layTrash = false;
		bool cubeSpace = false;
							
		// Filepath strings
		string tilesetfile;
		string slopemapfile;
		string tilesetdatafile;

		// Demo Sprite Class Variable and Name
		SpriteAnimation vlad;
		string SpriteName;

		// Lists of indicies for expanding tile placement/removal selection area
		List<Point> selectionEvenList = new List<Point>();
		List<Point> selectionOddList = new List<Point>();
		List<Point> selectionList;

		Vector2[,] cubeMapping;
		int cubeSpaceDivisions = 8;
 
		// Debuggin stop variable
		bool debcheck = false;

		// Constructor
		/// <summary>
		/// Initializes game control devices
		/// I.E. graphics manager, content directory,
		/// back buffer height/width
		/// </summary>
        public Game1()
        {
			// Set reference to Device Manager, Content Directory, and Window Dimensions
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
			graphics.PreferredBackBufferWidth = 1280;
			graphics.PreferredBackBufferHeight = 720;
			
		}

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// Initializes the names of default files and sets up events relating to the 
		/// main window.
        /// </summary>
        protected override void Initialize()
        {
            // Make the mouse visible on the screen
			this.IsMouseVisible = true;

			// Initial Image Files
			tilesetfile = @"Textures\Tilesets\default_tileset";
			tilesetdatafile = @"default_tileset.meta";
			slopemapfile = @"Textures\Tilesets\default_slopemaps";

			// Gain a handle on the main application window and subscribe to some events
			System.Windows.Forms.Form gameWindow = System.Windows.Forms.Form.FromHandle(this.Window.Handle) as System.Windows.Forms.Form;
			if(gameWindow != null)
			{
				// Subscribe to window closing event
				gameWindow.FormClosing += new System.Windows.Forms.FormClosingEventHandler(CheckSave);
			}	
            base.Initialize();
        }

        /// <summary>
        /// LoadContent sets up/loads in all of the initial media 
		/// and application level data and settings
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

			// Inital map parameters
			Tile.tileWidth = 64;
			Tile.tileHeight = 64;
			
			// Calculate the Drawing Ratio
			drawratio = new Vector2(64.0f / (float)Tile.tileWidth, 64.0f / (float)Tile.tileHeight);

			// Set Tile Display Parameters
			Tile.TileStepX = 64;
			Tile.TileStepY = 16;
			Tile.HeightTileOffset = 32;
			Tile.OddRowXOffset = 32;
			Tile.genTileHash(tilesetdatafile);
			
			// Initialize Keyboard, Content, and Camera
			KeyboardShortcuts();
			LoadContentPipeline();
			SetUpCamera();

			// Set Level Read content manager and Saved State
			ReadLevel.content = Content;
			ReadLevel.graphicsDevice = graphics.GraphicsDevice;

			SaveLevel.saved = true;

			// Initialize the GUI Manager
			gui = new GUIElements(Content.Load<Texture2D>(@"Textures\GUIElements\CompleteOverView"),
									Tile.TileSetTexture,
									Content.Load<Texture2D>(@"Textures\GUIElements\CurrentTile"),
									drawratio, Tile.MaxTileNum);

			// Initialize Event list
			Events = new List<TileEventObject>(ReadLevel.Events);
			
			// Set initial starting position
			StartingPos = myMap.WorldToMapCell(new Vector2(100, 100));

			// Set up sprites, forms, and selection sizing
			SetUpSprites();
			SetUpForms();
			SetUpSelection(gui.selectionSize);
			float xShift = 0.0f, yShift = 0.0f;
			SetUpCubeMapping(cubeSpaceDivisions, xShift, yShift);
        }

		/// <summary>
		/// Sets the initial keyboard shortcut keys
		/// </summary>
		private void KeyboardShortcuts()
		{
			// Inital Keyboard mapping
			KeyBoardController.trashKey = Keys.T;
			KeyBoardController.magnifyKey = Keys.M;
			KeyBoardController.saveKey = Keys.S;
			KeyBoardController.loadKey = Keys.L;
			KeyBoardController.settingsKey = Keys.S;
			KeyBoardController.authorKey = Keys.A;
			KeyBoardController.functionKey = Keys.F;
			KeyBoardController.selectKey = Keys.X;
			KeyBoardController.keyKey = Keys.K;
			KeyBoardController.coordsKey = Keys.C;
			KeyBoardController.eventsKey = Keys.E;
			KeyBoardController.hiddenKey = Keys.H;
			KeyBoardController.upKey = Keys.Up;
			KeyBoardController.downKey = Keys.Down;
			KeyBoardController.leftKey = Keys.Left;
			KeyBoardController.rightKey = Keys.Right;
			KeyBoardController.characterKey = Keys.P;
			KeyBoardController.CharacterUp = Keys.Up;
			KeyBoardController.CharacterDown = Keys.Down;
			KeyBoardController.CharacterLeft = Keys.Left;
			KeyBoardController.CharacterRight = Keys.Right;
			KeyBoardController.ChangeModes = Keys.M;
			KeyBoardController.ViewKey = Keys.V;
			KeyBoardController.walkableKey = Keys.W;
			KeyBoardController.cubeSpaceKey = Keys.C;

		}

		/// <summary>
		/// Load in current application media
		/// from the content pipeline
		/// </summary>
		private void LoadContentPipeline()
		{
			// Gui object textures
			trash = Content.Load<Texture2D>(@"Textures\GUIElements\trash");
			MagGlass = Content.Load<Texture2D>(@"Textures\GUIElements\MagGlass");
			NoWalk = Content.Load<Texture2D>(@"Textures\GUIElements\donotwalk");
			CubeText = Content.Load<Texture2D>(@"Textures\GUIElements\CubeSpace");
			// Generate a new map
			myMap = new TileMap(
				Content.Load<Texture2D>(@"Textures\Tilesets\mousemap"),
				Content.Load<Texture2D>(slopemapfile));

			// Current tile hilight
			hilight = Content.Load<Texture2D>(@"Textures\Tilesets\hilight");

			// Load in the tile image
            Tile.TileSetTexture = Content.Load<Texture2D>(tilesetfile);
			if(Tile.MaxTileNum == 0)
				Tile.MaxTileNum = (Tile.TileSetTexture.Height / Tile.tileHeight) * (Tile.TileSetTexture.Width / Tile.tileWidth) - 1;
			
			Wireframe = Content.Load<Texture2D>(@"Textures\Tilesets\Wireframe");
			WireframeView = Content.Load<Texture2D>(@"Textures\Tilesets\WireframeLG");

			Flatframe = Content.Load<Texture2D>(@"Textures\Tilesets\Flatframe");
			charStart = Content.Load<Texture2D>(@"Textures\Tilesets\startingPos");

			// Event Location Hilights
			sStart = Content.Load<Texture2D>(@"Textures\Tilesets\swapStart");
			sEnd = Content.Load<Texture2D>(@"Textures\Tilesets\swapEnd");
			activeArea = Content.Load<Texture2D>(@"Textures\Tilesets\activationArea");


			// Load in font 
			pericles6 = Content.Load<SpriteFont>(@"Fonts\Pericles6");
		}

		#region Initial Setups
		/// <summary>
		/// Initialize Application camera position, 
		/// orientation and aspect ratio
		/// </summary>
		private void SetUpCamera()
		{
			// Camera view is equal to the screen buffer size
			camera.ViewWidth = this.graphics.PreferredBackBufferWidth;
			camera.ViewHeight = this.graphics.PreferredBackBufferHeight;

			// World max width and height are equal to the map dimension values multiplied by the tile sizing
			camera.worldWidth = ((myMap.mapWidth - 2) * Tile.TileStepX);
			camera.worldHeight = ((myMap.mapHeight - 2) * Tile.TileStepY);

			// Set up the aspect ratio, w / h
			camera.AspectRatio = (float)(camera.worldWidth - camera.ViewWidth) / (float)(camera.worldHeight - camera.ViewHeight);

			// Inital camera position, so that we never see the half rendered 'edge' of the screen
			camera.displayOffset = new Vector2(baseOffsetX, baseOffsetY);
			camera.Zoom = 1.0f;
			camera.ZoomStep = 2.0f;
			camera.Rotation = 0.0f;

		}

		/// <summary>
		/// Initilize sprites used in the system
		/// </summary>
		private void SetUpSprites()
		{
			// Sprite Name

			// Load the sprite data
			vlad = new SpriteAnimation("Vlad", Content.Load<Texture2D>(@"Textures\Characters\T_Vlad_Sword_Walking_48x48"),
									   null);

			// Declare the sprite animation sequences
			vlad.AddAnimation("WalkEast", 0, 48 * 0, 48, 48, 8, 0.1f);
			vlad.AddAnimation("WalkNorth", 0, 48 * 1, 48, 48, 8, 0.1f);
			vlad.AddAnimation("WalkNorthEast", 0, 48 * 2, 48, 48, 8, 0.1f);
			vlad.AddAnimation("WalkNorthWest", 0, 48 * 3, 48, 48, 8, 0.1f);
			vlad.AddAnimation("WalkSouth", 0, 48 * 4, 48, 48, 8, 0.1f);
			vlad.AddAnimation("WalkSouthEast", 0, 48 * 5, 48, 48, 8, 0.1f);
			vlad.AddAnimation("WalkSouthWest", 0, 48 * 6, 48, 48, 8, 0.1f);
			vlad.AddAnimation("WalkWest", 0, 48 * 7, 48, 48, 8, 0.1f);

			vlad.AddAnimation("IdleEast", 0, 48 * 0, 48, 48, 1, 0.2f);
			vlad.AddAnimation("IdleNorth", 0, 48 * 1, 48, 48, 1, 0.2f);
			vlad.AddAnimation("IdleNorthEast", 0, 48 * 2, 48, 48, 1, 0.2f);
			vlad.AddAnimation("IdleNorthWest", 0, 48 * 3, 48, 48, 1, 0.2f);
			vlad.AddAnimation("IdleSouth", 0, 48 * 4, 48, 48, 1, 0.2f);
			vlad.AddAnimation("IdleSouthEast", 0, 48 * 5, 48, 48, 1, 0.2f);
			vlad.AddAnimation("IdleSouthWest", 0, 48 * 6, 48, 48, 1, 0.2f);
			vlad.AddAnimation("IdleWest", 0, 48 * 7, 48, 48, 1, 0.2f);

			// Start at location 100, 100
			vlad.Position = new Vector2(100, 100);
			vlad.DrawOffset = new Vector2(-24, -38); // Sprite image offset
			vlad.CurrentAnimation = "WalkEast"; // Inital state
			vlad.IsAnimating = true; // Make animate

		}

		/// <summary>
		/// Set up window's forms for system control
		/// </summary>
		private void SetUpForms()
		{
		
			setbox = new Settings(myMap.mapWidth, myMap.mapHeight, Tile.MaxTileNum, myMap.Layers, tilesetfile, Tile.tileWidth, Tile.tileHeight,
									walkable, slopeMap, tileType);
			autbox = new AuthorsBox();
			funcbox = new FuncBox(Events);
			keybox = new KeyboardBox(KeyBoardController.trashKey.ToString(), KeyBoardController.magnifyKey.ToString(), 
									 KeyBoardController.saveKey.ToString(),	 KeyBoardController.loadKey.ToString(), 
									 KeyBoardController.settingsKey.ToString(), KeyBoardController.authorKey.ToString(),
									 KeyBoardController.functionKey.ToString(), KeyBoardController.selectKey.ToString(),
									 KeyBoardController.coordsKey.ToString(), KeyBoardController.eventsKey.ToString(),
									 KeyBoardController.hiddenKey.ToString(),
									 KeyBoardController.upKey.ToString(), KeyBoardController.downKey.ToString(),
									 KeyBoardController.leftKey.ToString(), KeyBoardController.rightKey.ToString(),
									 KeyBoardController.characterKey.ToString(), KeyBoardController.ChangeModes.ToString(),
									 KeyBoardController.walkableKey.ToString(),
									 KeyBoardController.cubeSpaceKey.ToString());

			charbox = new CharacterBox(vlad.spName, KeyBoardController.CharacterUp.ToString(), KeyBoardController.CharacterDown.ToString(),
										KeyBoardController.CharacterLeft.ToString(), KeyBoardController.CharacterRight.ToString(),
										StartingPos.X, StartingPos.Y);

		}

		/// <summary>
		/// Set up the selection expansion lists based upon the
		/// maximum side length provided
		/// </summary>
		/// <param name="selSize">Specifies the maximum length of the selection box</param>
		private void SetUpSelection(int selSize)
		{
			selectionEvenList.Clear();
			selectionOddList.Clear();
			int startx = 0, y = 0; 
			for(int n = 0; n < selSize; n++)
			{
				startx = -(int)Math.Ceiling(n / 2.0);
				y = n;

				for(int x = 0; x < (2*n + 1); x++)
				{	
					selectionEvenList.Add(new Point(startx + (n % 2 == 0 ? x / 2 : (x + 1) / 2), y));
					if(x <= (2*n - 1) / 2)
						y++;
					else
						y--;
					
				}
			}

			for (int n = 0; n < selSize; n++)
			{
				startx = -(int)Math.Ceiling(n / 2.0);
				y = n;

				for (int x = 0; x < (2 * n + 1); x++)
				{
					selectionOddList.Add(new Point(-(startx + (n % 2 == 0 ? x / 2 : (x + 1) / 2)), y));
					if (x <= (2 * n - 1) / 2)
						y++;
					else
						y--;
				}
			}
			#region Commented Points 6x6
			/*
			selectionList.Add(new Point(0,0));
			
			selectionList.Add(new Point(-1,1)); 
			selectionList.Add(new Point(0,2)); 
			selectionList.Add(new Point(0,1)); 

			selectionList.Add(new Point(-1, 2)); 
			selectionList.Add(new Point(-1, 3)); 
			selectionList.Add(new Point(0, 4)); 
			selectionList.Add(new Point(0, 3)); 
			selectionList.Add(new Point(1, 2)); 
			
			selectionList.Add(new Point(-2, 3)); 
			selectionList.Add(new Point(-1, 4)); 
			selectionList.Add(new Point(-1, 5)); 
			selectionList.Add(new Point(0, 6)); 
			selectionList.Add(new Point(0, 5)); 
			selectionList.Add(new Point(1, 4)); 
			selectionList.Add(new Point(1, 3)); 
			
			selectionList.Add(new Point(-2, 4)); 
			selectionList.Add(new Point(-2, 5)); 
			selectionList.Add(new Point(-1, 6)); 
			selectionList.Add(new Point(-1, 7)); 
			selectionList.Add(new Point(0, 8)); 
			selectionList.Add(new Point(0, 7)); 
			selectionList.Add(new Point(1, 6)); 
			selectionList.Add(new Point(1, 5)); 
			selectionList.Add(new Point(2, 4)); 
			
			selectionList.Add(new Point(-3, 5)); 
			selectionList.Add(new Point(-2, 6)); 
			selectionList.Add(new Point(-2, 7)); 
			selectionList.Add(new Point(-1, 8)); 
			selectionList.Add(new Point(-1, 9)); 
			selectionList.Add(new Point(0, 10)); 
			selectionList.Add(new Point(0, 9)); 
			selectionList.Add(new Point(1, 8)); 
			selectionList.Add(new Point(1, 7)); 
			selectionList.Add(new Point(2, 6)); 
			selectionList.Add(new Point(2, 5)); 
			*/
			#endregion

		}

		/// <summary>
		/// Set up the list for cubespace mapping based on the number
		/// of divisions per cube side, and x/y shifting offsets
		/// </summary>
		/// <param name="divisions">Number of divisions per cube side</param>
		/// <param name="xShift">x placement shifted offset</param>
		/// <param name="yShift">y placement shifted offset</param>
		private void SetUpCubeMapping(int divisions, float xShift, float yShift)
		{
			cubeMapping = new Vector2[divisions, divisions];
			float xMult = Tile.tileWidth / (2 * divisions);
			float yMult = Tile.tileHeight / (4 * divisions);


			// Generate the mappings from CubeSpace to pixel offsets
			for(int x = 0; x < divisions; x++)
				for(int y = 0; y < divisions; y++)
				{
					cubeMapping[x, y] = new Vector2((y + x) * xMult - 32, (x - y) * yMult); 
				}
		}
		#endregion

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updates character positions
		/// camera movement, world position, and tile locations.
		/// Handles keyboard and mouse inputs. Including object movements
		/// setting changes, map changes, form activation and gui events.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

			if(formwaiting && Keyboard.GetState().IsKeyUp(Keys.Escape))
			{	
				this.keyshortcuts = false;
				formwaiting = false;
				gui.ResetState();
			}

			if (this.IsActive && !gui.settings && !gui.author && !gui.functions && !this.keyshortcuts && !gui.characters)
			{
		
			// Keyboard events
			KeyboardState ks = Keyboard.GetState();
			#region Keyboard Control
		
			if(ctrlkey && tileIterate)
			{
				CurrentWait += (float)gameTime.ElapsedGameTime.TotalSeconds;

				if (waitTime < CurrentWait)
				{
					fastIterate = true;
				}
			}

			#region Keyboard Shortcuts

			current = Keyboard.GetState();
			if (current != previous || fastIterate)
			{
				if(ks.IsKeyDown(Keys.D))
				{
					debcheck = !debcheck;
					myMap.debcheck = !debcheck;
				}
				if(ks.IsKeyDown(Keys.LeftControl) || ks.IsKeyDown(Keys.RightControl) || fastIterate)
				{
					#region CtrlKeys
					ctrlkey = true;
					if (ks.IsKeyDown(KeyBoardController.saveKey))
					{
						this.gui.save = true;
					}
					else if (ks.IsKeyDown(KeyBoardController.loadKey))
					{
						this.gui.load = true;
					}
					else if (ks.IsKeyDown(KeyBoardController.ChangeModes))
					{
						makemap = !makemap;
						int oddoff = 0;
						if (StartingPos.Y % 2 == 1)
							oddoff = Tile.OddRowXOffset;
						vlad.Position = new Vector2(StartingPos.X * Tile.TileStepX + oddoff - vlad.DrawOffset.X,
													(StartingPos.Y + 1) * Tile.TileStepY - vlad.DrawOffset.Y);
					}
					else if(gui.trash)
					{
						layTrash = !layTrash;
					}
					else if(ks.IsKeyDown(KeyBoardController.walkableKey))
					{
						walkable = !walkable;
					}
					else if(ks.IsKeyDown(KeyBoardController.ViewKey))
					{
						preview = view;
						view = 0;
					}
					else if(ks.IsKeyDown(KeyBoardController.cubeSpaceKey))
					{
						if(!(gui.selectionSize > 1))
							cubeSpace = !cubeSpace;
					}
					else if(ks.IsKeyDown(Keys.R))
					{
						myMap.RotateMapClockwise(true);
						float cwidth = camera.worldWidth - camera.ViewWidth;
						camera.Location = new Vector2(cwidth - (camera.Location.Y * camera.AspectRatio), camera.Location.X / camera.AspectRatio);	
						Console.WriteLine("Location: " + camera.Location.ToString());
					}

					else if(fastIterate || ks.IsKeyDown(KeyBoardController.upKey) || ks.IsKeyDown(KeyBoardController.downKey) )
					{	
						if(fastIterate)
						{
							ShiftWait += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
							if(ShiftWait > 20)
							{
								go = true;
								ShiftWait = 0;
							}
							else
								go = false;
						}
						else 
							go = true;
						
						if(go)
						{
							tileIterate = true;
							if(ks.IsKeyDown(KeyBoardController.upKey))
							{
								gui.IterateSelectedTile(true);
							}
							else if(ks.IsKeyDown(KeyBoardController.downKey))
							{
								gui.IterateSelectedTile(false);
							}
						}
					}
					else if(ks.IsKeyDown(KeyBoardController.leftKey))
					{
						gui.IterateSelectedSet(true, true);
					}
					else if(ks.IsKeyDown(KeyBoardController.rightKey))
					{
						gui.IterateSelectedSet(false, true);
					}

					#endregion
				}
				else if(ks.IsKeyDown(Keys.LeftShift) || ks.IsKeyDown(Keys.RightShift))
				{
					#region ShiftKeys
					sftkey = true;
					if(ks.IsKeyDown(Keys.Up))
					{
						gui.IncreaseSelectionSize(true);
						SetUpSelection(gui.selectionSize);
					}
					else if(ks.IsKeyDown(Keys.Down))
					{
						gui.IncreaseSelectionSize(false);
						SetUpSelection(gui.selectionSize);
					}
					if(gui.selectionSize > 1)
						cubeSpace = false;

					#endregion					
				}
				else if (ks.IsKeyDown(KeyBoardController.coordsKey))
				{
					displayCoords = (displayCoords + 1) % 2;
				}
				else if (ks.IsKeyDown(KeyBoardController.eventsKey))
				{
					ShowEvents = (ShowEvents + 1) % 2;
				}
				else if (ks.IsKeyDown(KeyBoardController.hiddenKey))
				{
					if(view == 1)
						view = preview;
					else
					{
						preview = view;
						view = 1;
					}
				}
				else if(ks.IsKeyDown(Keys.W))
				{
					if(view == 2)
						view = preview;
					else
					{
						preview = view;
						view = 2;
					}
				}
				else if(ks.IsKeyDown(KeyBoardController.ViewKey))
				{
					preview = view;
					view = (view + 1) % ViewFrames;
				}
				else if (ks.IsKeyDown(KeyBoardController.magnifyKey))
				{
					this.gui.ResetState();
					this.gui.magnify = true;
				}
				else if( ks.IsKeyDown(KeyBoardController.trashKey))
				{
					this.gui.ResetState();
					this.gui.trash = true;
				}
				else if (ks.IsKeyDown(KeyBoardController.authorKey))
				{
					this.gui.author = true;
				}	
				else if (ks.IsKeyDown(KeyBoardController.functionKey))
				{
					this.gui.functions = true;
				}
				else if (ks.IsKeyDown(KeyBoardController.settingsKey))
				{
					this.gui.settings = true;
				}
				else if (ks.IsKeyDown(KeyBoardController.selectKey))
				{
					this.gui.ResetState();
				}
				else if (ks.IsKeyDown(KeyBoardController.keyKey))
				{
					this.keyshortcuts = true;
				}
				else if (ks.IsKeyDown(KeyBoardController.characterKey))
				{
					this.gui.characters = true;
				}
				else if (ks.IsKeyDown(Keys.R))
				{
					myMap.RotateMapClockwise(false);
					float cheight = camera.worldHeight - camera.ViewHeight;
					camera.Location = new Vector2((camera.Location.Y * camera.AspectRatio), cheight - camera.Location.X / camera.AspectRatio);
				}

				if(Tile.tilesetMap[gui.currentTile].type != 3)
					cubeSpace = false;

				#endregion



				if (ks.IsKeyDown(Keys.Escape))
				{
					Exit();
				}

				if(ks.IsKeyUp(Keys.LeftControl) && ks.IsKeyUp(Keys.RightControl))
				{
					ctrlkey = false;
				}

				if(ks.IsKeyUp(Keys.LeftShift) && ks.IsKeyUp(Keys.RightShift))
				{
					sftkey = false;
				}

				if(!ctrlkey || ks.IsKeyUp(KeyBoardController.downKey) && ks.IsKeyUp(KeyBoardController.upKey))
				{
					tileIterate = false;
					fastIterate = false;
					CurrentWait = 0;
				}
				previous = current;

			}	
			else if(!ctrlkey && !sftkey && makemap)
			{
				if (ks.IsKeyDown(Keys.Left))
				{
					camera.Move(new Vector2(-2, 0));
				}
				if (ks.IsKeyDown(Keys.Right))
				{
					camera.Move(new Vector2(2, 0));
				}
				if (ks.IsKeyDown(Keys.Up))
				{
					camera.Move(new Vector2(0, -2));
				}
				if (ks.IsKeyDown(Keys.Down))
				{
					camera.Move(new Vector2(0, 2));
				}
			}		
			
			if (!makemap && !ctrlkey)
			{
				#region SpriteControl
				Vector2 moveVector = Vector2.Zero;
				Vector2 moveDir = Vector2.Zero;
				string animation = "";

				if (ks.IsKeyDown(KeyBoardController.CharacterUp) && ks.IsKeyDown(KeyBoardController.CharacterLeft))
				{
					moveDir = new Vector2(-2, -1);
					animation = "WalkNorthWest";
					moveVector += new Vector2(-2, -1);
				}

				else if (ks.IsKeyDown(KeyBoardController.CharacterUp) && ks.IsKeyDown(KeyBoardController.CharacterRight))
				{
					moveDir = new Vector2(2, -1);
					animation = "WalkNorthEast";
					moveVector += new Vector2(2, -1);
				}

				else if (ks.IsKeyDown(KeyBoardController.CharacterDown) && ks.IsKeyDown(KeyBoardController.CharacterLeft))
				{
					moveDir = new Vector2(-2, 1);
					animation = "WalkSouthWest";
					moveVector += new Vector2(-2, 1);
				}

				else if (ks.IsKeyDown(KeyBoardController.CharacterDown) && ks.IsKeyDown(KeyBoardController.CharacterRight))
				{
					moveDir = new Vector2(2, 1);
					animation = "WalkSouthEast";
					moveVector += new Vector2(2, 1);
				}

				else if (ks.IsKeyDown(KeyBoardController.CharacterUp))
				{
					moveDir = new Vector2(0, -2);
					animation = "WalkNorth";
					moveVector += new Vector2(0, -2);
				}

				else if (ks.IsKeyDown(KeyBoardController.CharacterLeft))
				{
					moveDir = new Vector2(-2, 0);
					animation = "WalkWest";
					moveVector += new Vector2(-2, 0);
				}

				else if (ks.IsKeyDown(KeyBoardController.CharacterRight))
				{
					moveDir = new Vector2(2, 0);
					animation = "WalkEast";
					moveVector += new Vector2(2, 0);
				}

				else if (ks.IsKeyDown(KeyBoardController.CharacterDown))
				{
					moveDir = new Vector2(0, 2);
					animation = "WalkSouth";
					moveVector += new Vector2(0, 2);
				}

				// Disallow movement if next position is not walkable 
				if (myMap.GetCellAtWorldPoint(vlad.Position + moveDir).walkable == false)
				{
					moveDir = Vector2.Zero;
				}

				// Disallow movement if next position is not sloped or in the same height level
				if (Math.Abs(myMap.GetOverallHeight(vlad.Position) - myMap.GetOverallHeight(vlad.Position + moveDir)) > 10)
				{
					moveDir = Vector2.Zero;
				}

				if (moveDir.Length() != 0)
				{
					vlad.MoveBy((int)moveDir.X, (int)moveDir.Y);
					if (vlad.CurrentAnimation != animation)
						vlad.CurrentAnimation = animation;
				}
				else
				{
					vlad.CurrentAnimation = "Idle" + vlad.CurrentAnimation.Substring(4);
				}

				// Restrict the bounds of the sprites movement to within 100 pixels of the screen dimensions
				Vector2 testPosition = camera.WorldtoScreen(vlad.Position) * camera.Zoom;

				if (testPosition.X < 100)
					camera.Move(new Vector2(testPosition.X - 100, 0));

				if (testPosition.X > (camera.ViewWidth - 300))
					camera.Move(new Vector2(testPosition.X - (camera.ViewWidth - 300), 0));

				if (testPosition.Y < 100)
					camera.Move(new Vector2(0, testPosition.Y - 100));

				if (testPosition.Y > (camera.ViewHeight - 100))
					camera.Move(new Vector2(0, testPosition.Y - (camera.ViewHeight - 100)));

				float vladX = MathHelper.Clamp(
								vlad.Position.X, 0 + Math.Abs(vlad.DrawOffset.X) + Math.Abs(baseOffsetX), camera.worldWidth);
				float vladY = MathHelper.Clamp(
								vlad.Position.Y, 0 + Math.Abs(vlad.DrawOffset.Y) + Math.Abs(baseOffsetY), camera.worldHeight);

				vlad.Position = new Vector2(vladX, vladY);
				vlad.Update(gameTime);
				#endregion
			}

			#endregion

			// Loading and saving events
			#region Load N' Save
			
			if(gui.load)
			{
				// Load the level from file
				if(ReadLevel.LoadLevel())
				{	
					// Change parameters after load
					tilesetfile = ReadLevel.TILESETFILE;
					tilesetdatafile = ReadLevel.TILESETFILE.Substring(0, ReadLevel.TILESETFILE.LastIndexOf('.')) + ".meta";
					Tile.TileSetTexture = ReadLevel.TILESET;
					Tile.tileWidth = ReadLevel.TILEWIDTH;
					Tile.tileHeight = ReadLevel.TILEHEIGHT;
					Tile.TileStepX = ReadLevel.TILESTEPX;
					Tile.TileStepY = ReadLevel.TILESTEPY;
					Tile.HeightTileOffset = ReadLevel.HEIGHTTILEOFFSET;
					Tile.OddRowXOffset = ReadLevel.ODDROWOFFSETX;
					Tile.genTileHash(tilesetdatafile);

					// Change to the new map data
					myMap.ChangeDataSets(ReadLevel.Rows, ReadLevel.SLOPEMAP);

					// World max width and height are equal to the map dimension values multiplied by the tile sizing
					camera.worldWidth = ((myMap.mapWidth - 2) * Tile.TileStepX);
					camera.worldHeight = ((myMap.mapHeight - 2) * Tile.TileStepY);

					// Change events
					Events.Clear();
					Events = new List<TileEventObject>(ReadLevel.Events);
				}
				// Reset the read class
				ReadLevel.ResetRead();

				// Reset load state 
				gui.load = false;
			}
			
			if(gui.save)
			{
				// Initialize save data
				SaveLevel.TILESET = tilesetfile;
				SaveLevel.SLOPEMAP = slopemapfile;
				SaveLevel.TILEWIDTH = Tile.tileWidth;
				SaveLevel.TILEHEIGHT = Tile.tileHeight;
				SaveLevel.TILESTEPX = Tile.TileStepX;
				SaveLevel.TILESTEPY = Tile.TileStepY;
				SaveLevel.HEIGHTTILEOFFSET = Tile.HeightTileOffset;
				SaveLevel.ODDROWOFFSETX = Tile.OddRowXOffset;
				SaveLevel.MAPHEIGHT = myMap.mapHeight;
				SaveLevel.MAPWIDTH = myMap.mapWidth;
				SaveLevel.initData(myMap.Rows);
				SaveLevel.Author = autbox.name;
				SaveLevel.Date = autbox.date;
				SaveLevel.Notes = autbox.notes;
				SaveLevel.Events.AddRange(Events);

				// Reorient the map
				for(int i = 0; i < 4; i++)
				{
					if(myMap.rotation == 0)
						break;
					else
						myMap.RotateMapClockwise(true);
				}

				// Write data to file
				SaveLevel.write();
				SaveLevel.saved = true;

				// Clear out the save data
				SaveLevel.Rows.Clear(); 
				SaveLevel.Events.Clear();

				// Reset save state
				gui.save = false;
			}
			#endregion

			// Mouse events
			#region Mouse Control
	
			// Current mouse state
			MouseState mouseStateCurrent = Mouse.GetState();
	
			// Determine relative mouse location
			int heightrowOffset = 0;
			Vector2 MouseLoc = camera.ScreentoWorld(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
			Point CubeSpacePoint;
			Point MousePoint = myMap.WorldToMapCell(new Point((int)MouseLoc.X, (int)MouseLoc.Y), out CubeSpacePoint);
			MousePoint.Y += (2 *myMap.Layers.Count);
			Point MouseLocPoint = new Point((int)((MouseLoc.X  - camera.Location.X + baseOffsetX) * camera.Zoom), (int)((MouseLoc.Y - camera.Location.Y + baseOffsetY) * camera.Zoom));
		
			// Change Y based on offset
			if ((MousePoint.Y) % 2 == 1)
			{
				heightrowOffset = Tile.OddRowXOffset;
			}

			// Left button press contols
			#region LeftButton
			
			// Debounce the button
			if(mouseStateCurrent.LeftButton == ButtonState.Pressed)
			{
				lpressed = true;
			}
			if(mouseStateCurrent.LeftButton == ButtonState.Released && lpressed)
			{
				// Reset mouse state variable
				lpressed = false;

				// If we aren't in the toolbox
				if (!(gui.GuiOutline[0].Contains(MouseLocPoint) || gui.GuiOutline[1].Contains(MouseLocPoint)))
				{
					SaveLevel.saved = false;

					#region Gui Changes
					// Increase or decrease the camera zoom
					if(gui.magnify)
					{
						if(ctrlkey)
						{
							// Decrease zoom and change viewport rendering
							if(camera.Decrease_Zoom(MouseLoc))
							{
								squaresAcross *= (int)camera.ZoomStep;
								squaresDown *= (int)camera.ZoomStep;
							}
						}
						else
						{
							// Increase zoom and change the viewport rendering
							if(camera.Increase_Zoom(MouseLoc))
							{
								squaresAcross /= (int)camera.ZoomStep;
								squaresDown /= (int)camera.ZoomStep;
							}
						}
					}
					#endregion

					#region Map Changes
					else
					{
						int mouseX, mouseY, selMax = 0;
						for (int findmax = 0; findmax < gui.selectionSize * gui.selectionSize; findmax++)
						{
							mouseY = MousePoint.Y + selectionList.ElementAt(findmax).Y;
							mouseX = MousePoint.X + selectionList.ElementAt(findmax).X;
							try
							{
								selMax = Math.Max(selMax, myMap.Rows[mouseY].Columns[mouseX].HeightTiles.Count);
							}
							catch //If selection leaves map do nothing
							{ }
						}

						for(int sel = 0; sel < gui.selectionSize * gui.selectionSize; sel++)
						{
							if(MousePoint.Y % 2 == 1)
								selectionList = selectionOddList;
							else
								selectionList = selectionEvenList;

							mouseX = MousePoint.X + selectionList.ElementAt(sel).X;
							mouseY = MousePoint.Y + selectionList.ElementAt(sel).Y;

							if (gui.trash) // Remove tiles
							{
								int trashPoint = (layTrash ? selMax - 1 : 0);

								if (myMap.Rows[mouseY].Columns[mouseX].SceneryTiles.Count > trashPoint)
									myMap.Rows[mouseY].Columns[mouseX].RemoveSceneryTile();

								else if (myMap.Rows[mouseY].Columns[mouseX].TopperTiles.Count > trashPoint)
									myMap.Rows[mouseY].Columns[mouseX].RemoveTopperTile();

								else if (myMap.Rows[mouseY].Columns[mouseX].HeightTiles.Count > trashPoint)
									myMap.Rows[mouseY].Columns[mouseX].RemoveHeightTile();

								else if (myMap.Rows[mouseY].Columns[mouseX].BaseTiles.Count > trashPoint)
									myMap.Rows[mouseY].Columns[mouseX].RemoveBaseTile();
								
									
							}
							else if(!gui.GetState())// Add tiles
							{
								try
								{
									if(tileType == 1)
									{
										myMap.Rows[mouseY].Columns[mouseX].AddBaseTile(gui.currentTile);		
									}
									else if(tileType == 2)
									{
										myMap.Rows[mouseY].Columns[mouseX].AddHeightTile(gui.currentTile);		
									}
									else if(tileType == 3)
									{
										myMap.Rows[mouseY].Columns[mouseX].AddTopperTile(gui.currentTile);
									}
									else if(tileType == 4)
									{
										myMap.Rows[mouseY].Columns[mouseX].AddSceneryTile(gui.currentTile);
									}
									
									else if(Tile.hashGenerated)
									{
										switch(Tile.GetTileType(gui.currentTile))
										{
											case 0:
												myMap.Rows[mouseY].Columns[mouseX].AddBaseTile(gui.currentTile);
												break;

											case 1: 
												myMap.Rows[mouseY].Columns[mouseX].AddHeightTile(gui.currentTile);
												break;

											case 2:
												myMap.Rows[mouseY].Columns[mouseX].AddTopperTile(gui.currentTile);
												break;

											case 3:
												if(cubeSpace)
												{
													CubeSpacePoint.Y = CubeSpacePoint.Y - 16;

													CubeSpacePoint.X = (CubeSpacePoint.X / 4) * 4 - 32;
													CubeSpacePoint.Y = (CubeSpacePoint.Y / 2) * 2;
													myMap.Rows[mouseY].Columns[mouseX].AddSceneryTile(gui.currentTile, CubeSpacePoint.X, CubeSpacePoint.Y);
												
												}
												else
													myMap.Rows[mouseY].Columns[mouseX].AddSceneryTile(gui.currentTile, 0.0f, 0.0f);
												
												break;

											default:
												break;
										}

										myMap.Rows[mouseY].Columns[mouseX].SlopeMap = Tile.GetTileSlopeMap(gui.currentTile);
										myMap.Rows[mouseY].Columns[mouseX].walkable = walkable;
									}
									else
									{
										// Determine if the current tile is a height tile or not
										uint[] myuint = new uint[1];
										int tileY = gui.currentTile / (Tile.TileSetTexture.Width / Tile.tileWidth);
										int tileX = gui.currentTile % (Tile.TileSetTexture.Width / Tile.tileWidth);
										Tile.TileSetTexture.GetData<uint>(0, new Rectangle(tileX * Tile.tileWidth + (int)(Tile.tileWidth * 0.5f), tileY * Tile.tileHeight + (int)(Tile.tileHeight * 0.45), 1, 1), myuint, 0, 1);
					
										// Add height tile
										if (myuint[0] != 0)
										{
											myMap.Rows[mouseY].Columns[mouseX].AddHeightTile(gui.currentTile);		
										}
										else
										{
											// Add base tile or topper tile based upon height tile count
											if ((myMap.Rows[mouseY].Columns[mouseX].HeightTiles.Count)== 0)
												myMap.Rows[mouseY].Columns[mouseX].AddBaseTile(gui.currentTile);
											else
												myMap.Rows[mouseY].Columns[mouseX].AddTopperTile(gui.currentTile);
										}
									
										myMap.Rows[mouseY].Columns[mouseX].SlopeMap = slopeMap;
									
									}
								}
								catch
								{
									Console.WriteLine("Error in Tile addition/Removal");
								}
							}
						}
					}
					#endregion
				}
				else
				{
					// Choose an action based upon the mouse location
					gui.selectAction(MouseLocPoint);
				}

			}
			#endregion

			// Right button press controls
			#region RightButton

			// Debounce the button
			if(mouseStateCurrent.RightButton == ButtonState.Pressed)
			{
				rpressed = true;
			}
			if(mouseStateCurrent.RightButton == ButtonState.Released && rpressed)
			{
				// Decrease the camera zoom and change viewport rendering
				if(gui.magnify)
				{
					if(camera.Decrease_Zoom(MouseLoc))
					{
						squaresAcross *= (int)camera.ZoomStep;
						squaresDown *= (int)camera.ZoomStep;
					}
				}
				// Reset mouse state variable
				rpressed = false;
			}
			#endregion
		
			#endregion

			// Windows Forms events
			#region Forms Control
			if(gui.settings)
			{
				setbox.Dispose();
				setbox = new Settings(myMap.mapWidth, myMap.mapHeight, Tile.MaxTileNum, myMap.Layers, tilesetfile, 
										Tile.tileWidth, Tile.tileHeight,
										walkable, slopeMap, tileType);
				setbox.Show();
				setbox.FormClosed += new System.Windows.Forms.FormClosedEventHandler(setbox_FormClosed);
			}
			else if(gui.author)
			{
				autbox.Dispose();
				autbox = new AuthorsBox();
				autbox.Show();
				autbox.FormClosed += new System.Windows.Forms.FormClosedEventHandler(autbox_FormClosed);
			}
			else if(gui.functions)
			{
				autbox.Dispose();
				List<TileEventObject> tempList = new List<TileEventObject>(Events);
				funcbox = new FuncBox(tempList);
				funcbox.Show();
				funcbox.FormClosed += new System.Windows.Forms.FormClosedEventHandler(funcbox_FormClosed);
			}
			else if(this.keyshortcuts)
			{
				keybox.Dispose();
				keybox = new KeyboardBox(KeyBoardController.trashKey.ToString(), KeyBoardController.magnifyKey.ToString(),
										 KeyBoardController.saveKey.ToString(), KeyBoardController.loadKey.ToString(),
										 KeyBoardController.settingsKey.ToString(), KeyBoardController.authorKey.ToString(),
										 KeyBoardController.functionKey.ToString(), KeyBoardController.selectKey.ToString(),
										 KeyBoardController.coordsKey.ToString(), KeyBoardController.eventsKey.ToString(),
										 KeyBoardController.hiddenKey.ToString(),
										 KeyBoardController.upKey.ToString(), KeyBoardController.downKey.ToString(),
										 KeyBoardController.leftKey.ToString(), KeyBoardController.rightKey.ToString(),
										 KeyBoardController.characterKey.ToString(), KeyBoardController.ChangeModes.ToString(),
										 KeyBoardController.walkableKey.ToString(),
										 KeyBoardController.cubeSpaceKey.ToString());
				keybox.Show();
				keybox.FormClosed += new System.Windows.Forms.FormClosedEventHandler(keybox_FormClosed);
			}
			else if(gui.characters)
			{
				charbox.Dispose();
				charbox = new CharacterBox(vlad.spName, KeyBoardController.CharacterUp.ToString(), KeyBoardController.CharacterDown.ToString(),
							KeyBoardController.CharacterLeft.ToString(), KeyBoardController.CharacterRight.ToString(),
							StartingPos.X, StartingPos.Y);

				charbox.Show();
				charbox.FormClosed += new System.Windows.Forms.FormClosedEventHandler(charbox_FormClosed);

			}
			#endregion
			
			}

			base.Update(gameTime);
        }
		
		// Handle clean up and use of data after close of a settings form
		#region Form Control Methods

		/// <summary>
		/// Handles data control after setbox had been closed
		/// </summary>
		/// <param name="sender">Form control handle</param>
		/// <param name="e">Event argument list</param>
		private void setbox_FormClosed(object sender, EventArgs e)
		{
			if(setbox.ok)
			{
				gui.settings = false;
				myMap.ResetLayers(setbox.layers);
				setbox.layers.Clear();

				if (setbox.tilefile.IndexOf("default_tileset", 0) != -1 || setbox.tilefile == "")
				{
					Tile.TileSetTexture = Content.Load<Texture2D>(@"Textures\Tilesets\default_tileset");
				}
				else
				{
					FileStream textureread = File.Open(setbox.tilefile, FileMode.Open, FileAccess.Read, FileShare.Read);
					Tile.TileSetTexture = Texture2D.FromStream(GraphicsDevice, textureread);
					tilesetfile = setbox.tilefile;
					tilesetdatafile = tilesetfile.Substring(0, tilesetfile.LastIndexOf('.')) + ".meta";
				}

				
				Tile.tileWidth = setbox.tileWid;
				Tile.tileHeight = setbox.tileHei;
				Tile.genTileHash(tilesetdatafile);
				walkable = setbox.walkable;
				slopeMap = setbox.slope - 1;
				tileType = setbox.type;

				drawratio = new Vector2(64.0f / (float)Tile.tileWidth, 64.0f / (float)Tile.tileHeight);

				gui.curTileset = Tile.TileSetTexture;
				gui.tileratio = drawratio * gui.tilescale;

			}
			else
				formwaiting = true;
		}

		/// <summary>
		/// Handles data control after autbox had been closed
		/// </summary>
		/// <param name="sender">Form control handle</param>
		/// <param name="e">Event argument list</param>
		private void autbox_FormClosed(object sender, EventArgs e)
		{
			if(autbox.ok)
				gui.author = false;
			else	
				formwaiting = true;
		}

		/// <summary>
		/// Handles data control after funcbox had been closed
		/// </summary>
		/// <param name="sender">Form control handle</param>
		/// <param name="e">Event argument list</param>
		private void funcbox_FormClosed(object sender, EventArgs e)
		{
			if(funcbox.ok)
			{
				gui.functions = false;
				SaveLevel.saved = false;
				Events.Clear();

				// Sets up swap cell events that the user specified inside the funcbox form
				#region Swap Events
				// Swap Events
				foreach (var Swap in funcbox.swaps)
				{
					List<Vector3> slist = new List<Vector3>();
					List<Vector3> elist = new List<Vector3>();
					
					// Single cell
					if(Swap.fendx == -1)
					{
						Vector3 start = new Vector3(Swap.bstartx, Swap.bstarty, 0);
						Vector3 end = new Vector3(Swap.bendx, Swap.bendy, 0);
						slist.Add(start);
						elist.Add(end);
						Events.Add(new TileEventObject(new SwapData(new List<Vector3>(slist), new List<Vector3>(elist)),
							new Rectangle((int) Swap.activate.startx, (int) Swap.activate.starty,
											Math.Abs((int)Swap.activate.endx - (int)Swap.activate.startx), 
											Math.Abs((int) Swap.activate.endy - (int)Swap.activate.starty))));
					}
					// Cell Range
					else
					{
						for(int x = (int)Swap.bstartx; x <= (int)Swap.bendx; x++)
							for(int y = (int)Swap.bstarty; y <= (int)Swap.bendy; y++)
								slist.Add(new Vector3(x, y, 0));
							
						for(int x = (int) Swap.fstartx; x <= (int)Swap.fendx; x++)
							for(int y = (int)Swap.fstarty; y <= (int)Swap.fendy; y++)
								elist.Add(new Vector3(x, y, 0));

						Events.Add(new TileEventObject(new SwapData(new List<Vector3>(slist), new List<Vector3>(elist)),
							new Rectangle((int)Swap.activate.startx, (int)Swap.activate.starty,
											Math.Abs((int)Swap.activate.endx - (int)Swap.activate.startx),
											Math.Abs((int)Swap.activate.endy - (int)Swap.activate.starty))));
					}

				}
				#endregion

				// Sets up Tileset change events that the user specified inside the funcbox form
				#region Tileset Events
				foreach(var tile in funcbox.tilesets)
				{
					FileStream textureread = File.Open(tile.filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
					Events.Add(new TileEventObject(new TileSetData(Texture2D.FromStream(GraphicsDevice, textureread),
													(int)tile.tilewidth, (int)tile.tileheight, (int)tile.tilestepx, (int)tile.tilestepy,
													(int)tile.oddrowoffset, (int)tile.heighttileoffset),
													new Rectangle((int)tile.activate.startx, (int)tile.activate.starty,
													Math.Abs((int)tile.activate.endx - (int)tile.activate.startx),
													Math.Abs((int)tile.activate.endy - (int)tile.activate.starty)), tile.filepath));
					textureread.Close();
				}
				#endregion

				// Sets up SpriteSheet change events that the user specified inside the funcbox form
				#region SpriteSheet Events
				foreach(var sprite in funcbox.spritesheets)
				{
					FileStream textureread = File.Open(sprite.spfilepath, FileMode.Open, FileAccess.Read, FileShare.Read);
					List<AnimationData> animations = new List<AnimationData>();
					foreach(var animation in sprite.animations)
					{
						animations.Add(new AnimationData(animation.spName, (int)animation.spX, (int)animation.spY, 
											(int)animation.spWidth, (int)animation.spHeight, (int)animation.spFrames, 
											(int)animation.spFrameLength, animation.spNextAnimation));
					}

					Events.Add(new TileEventObject(new SpriteSheetData(Texture2D.FromStream(GraphicsDevice, textureread), animations),
													new Rectangle((int)sprite.activate.startx, (int)sprite.activate.starty,
													Math.Abs((int)sprite.activate.endx - (int)sprite.activate.startx),
													Math.Abs((int)sprite.activate.endy - (int)sprite.activate.starty)), 
													sprite.spfilepath));
					textureread.Close();
				}
				#endregion
			}
			else 
				formwaiting = true;
		}

		/// <summary>
		/// Handles data control after keybox had been closed
		/// </summary>
		/// <param name="sender">Form control handle</param>
		/// <param name="e">Event argument list</param>
		private void keybox_FormClosed(object sender, EventArgs e)
		{
			// Tries to parse out the keys specified by the user into valid keyboard characters
			#region Set Keys
			if(keybox.ok)
			{
				this.keyshortcuts = false;
				try{ KeyBoardController.trashKey = (Keys)Enum.Parse(typeof(Keys), keybox.Shortcuts.trashKey); }
				catch { };
				try { KeyBoardController.magnifyKey = (Keys)Enum.Parse(typeof(Keys), keybox.Shortcuts.magnifyKey); }
				catch { };
				try { KeyBoardController.saveKey = (Keys)Enum.Parse(typeof(Keys), keybox.Shortcuts.saveKey); }
				catch { };
				try { KeyBoardController.loadKey = (Keys)Enum.Parse(typeof(Keys), keybox.Shortcuts.loadKey); }
				catch { };
				try { KeyBoardController.settingsKey = (Keys)Enum.Parse(typeof(Keys), keybox.Shortcuts.settingsKey); }
				catch { };
				try { KeyBoardController.authorKey = (Keys)Enum.Parse(typeof(Keys), keybox.Shortcuts.authorKey); }
				catch { };
				try { KeyBoardController.functionKey = (Keys)Enum.Parse(typeof(Keys), keybox.Shortcuts.functionKey); }
				catch { };
				try { KeyBoardController.selectKey = (Keys)Enum.Parse(typeof(Keys), keybox.Shortcuts.selectKey); }
				catch { };
				try { KeyBoardController.coordsKey = (Keys)Enum.Parse(typeof(Keys), keybox.Shortcuts.coordsKey); }
				catch { };
				try { KeyBoardController.eventsKey = (Keys)Enum.Parse(typeof(Keys), keybox.Shortcuts.eventsKey); }
				catch { };
				try { KeyBoardController.hiddenKey = (Keys)Enum.Parse(typeof(Keys), keybox.Shortcuts.hiddenKey); }
				catch { };
				try { KeyBoardController.upKey = (Keys)Enum.Parse(typeof(Keys), keybox.Shortcuts.upKey); }
				catch { };
				try { KeyBoardController.downKey = (Keys)Enum.Parse(typeof(Keys), keybox.Shortcuts.downKey); }
				catch { };
				try { KeyBoardController.leftKey = (Keys)Enum.Parse(typeof(Keys), keybox.Shortcuts.leftKey); }
				catch { };
				try { KeyBoardController.rightKey = (Keys)Enum.Parse(typeof(Keys), keybox.Shortcuts.rightKey); }
				catch { };
				try { KeyBoardController.characterKey = (Keys)Enum.Parse(typeof(Keys), keybox.Shortcuts.charKey); }
				catch { };
				try { KeyBoardController.ChangeModes= (Keys)Enum.Parse(typeof(Keys), keybox.Shortcuts.playKey); }
				catch { };
				try { KeyBoardController.walkableKey = (Keys)Enum.Parse(typeof(Keys), keybox.Shortcuts.walkKey); }
				catch { };
				try { KeyBoardController.cubeSpaceKey = (Keys)Enum.Parse(typeof(Keys), keybox.Shortcuts.cubeSpaceKey); }
				catch { };
			}
			else
			{
				formwaiting = true;
			}

			#endregion
		}
		
		/// <summary>
		/// Handles data control after charbox had been closed
		/// </summary>
		/// <param name="sender">Form control handle</param>
		/// <param name="e">Event argument list</param>
		private void charbox_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
		{
			if(charbox.ok)
			{
				try { KeyBoardController.CharacterUp = (Keys)Enum.Parse(typeof(Keys), charbox.CUp); }
				catch { };
				try { KeyBoardController.CharacterDown = (Keys)Enum.Parse(typeof(Keys), charbox.CDown); }
				catch { };
				try { KeyBoardController.CharacterLeft = (Keys)Enum.Parse(typeof(Keys), charbox.CLeft); }
				catch { };
				try { KeyBoardController.CharacterRight = (Keys)Enum.Parse(typeof(Keys), charbox.CRight); }
				catch { };
				StartingPos = new Point(charbox.startx, charbox.starty);
				SpriteName = charbox.SName;
				gui.characters = false;
			}
			else
				formwaiting = true;
		}

		#endregion

        /// <summary>
        /// Called to redraw all current items within the application
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
			// Clear the screan
            GraphicsDevice.Clear(Color.Black);

			// Texture image variable
			Texture2D text;

			// Start drawing sequence
			// Draw sprites in an ordered seqence from back (1.0f) to from (0.0f)
			// Use the camera's matrix transform during rendering
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, 
								null,
								null,
								null,
								null,
								camera.get_transformation(graphics.GraphicsDevice));
				
			// Determine what 'square' on the 'map' that the camera is currently pointing at
            Vector2 firstSquare = new Vector2(camera.Location.X / Tile.TileStepX, camera.Location.Y / Tile.TileStepY );
            int firstX = (int) firstSquare.X;
			int firstY = (int) firstSquare.Y;

			// Determine the shift of our drawing rectangle as we move the camera 
			// as the camera moves in increments less than that of a single tile
            Vector2 squareOffset = new Vector2(camera.Location.X % Tile.TileStepX, camera.Location.Y % Tile.TileStepY);
            int offsetX = (int)squareOffset.X;
            int offsetY = (int)squareOffset.Y;

			// maximum depth to draw any MapCell
			float maxdepth = ((myMap.mapWidth + 1) + ((myMap.mapHeight + 1) * Tile.tileWidth )) * 10;
			float depthOffset;

			// Local sprite map point
			Point vladMapPoint = myMap.WorldToMapCell(new Point((int)vlad.Position.X, (int)vlad.Position.Y));

			int curRotation = myMap.rotation;

			#region DrawMap
            for(int y = 0; y < squaresDown; y++)
            {
				// If we are at an odd row, apply an offset
				int rowOffset = 0;
				if((firstY + y) % 2 == 1)
					rowOffset = (int)(Tile.OddRowXOffset);
				
				// Only render tiles in the viewport
                for(int x = 0; x < squaresAcross; x++)
                {
					// Determine starting tile
					int mapx = (firstX + x);
					int mapy = (firstY + y);
					
					// Determine depth offset from position
					depthOffset = 0.7f - ((mapx + (mapy * Tile.tileWidth)) / maxdepth);

					// Don't try to draw tiles that don't exist
					if((mapx >= myMap.mapWidth) || (mapy >= myMap.mapHeight))
						continue;

					// Set the texture tileset
					text = Tile.TileSetTexture;

					// Draw all base tiles in viewport
					#region BaseTiles

					// Draw every base tile at the current index
					float basecount = 0.0f;

					foreach(int tileID in myMap.Rows[y + firstY].Columns[x + firstX].BaseTiles)
					{
						
						#region Hidden Mode
						if(view == 1)
						{
							spriteBatch.Draw(
								// Texture to be drawn
								Flatframe,
								// Where on the 'screen' to draw a tile, use a row offset for odd row hexes
								camera.WorldtoScreen(
										new Vector2((mapx * Tile.TileStepX) + rowOffset,
														mapy * Tile.TileStepY)),

								// Determine what type of tile to draw
								null,

								(this.view == 1 ? Color.White * 0.4f : Color.White), // Draw in original colors
								0.0f, // No Rotation 
								Vector2.Zero, // 0, 0 origin
								1.0f,// No scaling
								SpriteEffects.None, // No flips
								1.0f - basecount); // Starting layer

							// Increment depth offset to stack tiles
							basecount += 0.0000001f;
						}
						#endregion

						#region Movement Mode
						else if (view == 2)
						{
							spriteBatch.Draw(
								// Texture to be drawn
								activeArea,//text,
								// Where on the 'screen' to draw a tile, use a row offset for odd row hexes
								camera.WorldtoScreen(
										new Vector2((mapx * Tile.TileStepX) + rowOffset,
														mapy * Tile.TileStepY + 32)),

								// Determine what type of tile to draw
								null,//Tile.GetSourceRectangle(tileID),

								(myMap.Rows[mapy].Columns[mapx].walkable) ? Color.White * 1.5f : Color.DarkGray, // Draw in original colors
								0.0f, // No Rotation 
								Vector2.Zero, // 0, 0 origin
								1.0f,// No scaling
								SpriteEffects.None, // No flips
								1.0f - basecount); // Starting layer

							// Increment depth offset to stack tiles
							basecount += 0.0000001f;

						}
						
						#endregion
						
						#region Normal Mode
						else
						{
						spriteBatch.Draw(
							// Texture to be drawn
							text, 
							
							// Where on the 'screen' to draw a tile, use a row offset for odd row hexes
							camera.WorldtoScreen(
									new Vector2((mapx * Tile.TileStepX) + rowOffset, 
													mapy * Tile.TileStepY)),

							// Determine what type of tile to draw
							Tile.GetSourceRectangle(Tile.GetTileIndex(tileID, curRotation)),

							Color.White, // Draw in original colors
							0.0f, // No Rotation 
							Vector2.Zero, // 0, 0 origin
							drawratio,// No scaling
							SpriteEffects.None, // No flips
							1.0f - basecount); // Starting layer

							// Increment depth offset to stack tiles
							basecount += 0.0000001f;

						}
						#endregion

					}
					#endregion

					// Draw all height tiles in viewport
					#region HeightTiles

					// Draw all of the additional heights to the given tile
					int heightRow = 0;
					foreach (int tileID in myMap.Rows[y + firstY].Columns[x + firstX].HeightTiles)
					{
						#region Hidden Mode
						if(view == 1)
						{
							spriteBatch.Draw(
								// Texture to be drawn
							Wireframe,//Tile.TileSetTexure,

							// Where on the 'screen' to draw a tile, use a row offset for odd row tiles
							camera.WorldtoScreen(
								new Vector2(
									(mapx * Tile.TileStepX) + rowOffset,
									mapy * Tile.TileStepY - (heightRow * Tile.HeightTileOffset))),

							// Determine what type of tile to draw
							null,

							(heightRow < myMap.Layers.Count ? new Color(1f - 0.08f * heightRow, 1f, 1f - 0.08f * heightRow) : Color.Green), // Draw in original colors
							0.0f, // No Rotation
							Vector2.Zero, // Origin is not moved

							1.0f, // scaling

							SpriteEffects.None, // No flips

							// Increase the depthoffset by 0.0000001f every height we add
								// this will allow it to move above existing items without
								// interfering with the surrounding depth values
							depthOffset - ((float)heightRow * heightRowDepthMod));
						}
						#endregion

						#region Movement Mode
						else if(view == 2)
						{
							spriteBatch.Draw(
								// Texture to be drawn
								WireframeView,//Tile.TileSetTexure,

								// Where on the 'screen' to draw a tile, use a row offset for odd row tiles
								camera.WorldtoScreen(
									new Vector2(
										(mapx * Tile.TileStepX) + rowOffset,
										mapy * Tile.TileStepY - (heightRow * Tile.HeightTileOffset))),

								// Determine what type of tile to draw
								null,

								(myMap.Rows[mapy].Columns[mapx].walkable) ? Color.White * 1.5f : Color.DarkGray, // Draw in original colors
								0.0f, // No Rotation
								Vector2.Zero, // Origin is not moved

								1.0f, // scaling

								SpriteEffects.None, // No flips

								// Increase the depthoffset by 0.0000001f every height we add
								// this will allow it to move above existing items without
								// interfering with the surrounding depth values
								depthOffset - ((float)heightRow * heightRowDepthMod));
						}
						#endregion

						#region Normal Mode
						else
						{
						spriteBatch.Draw(
							// Texture to be drawn
							text,//Tile.TileSetTexure,

							// Where on the 'screen' to draw a tile, use a row offset for odd row tiles
							camera.WorldtoScreen(
								new Vector2(
									(mapx * Tile.TileStepX) + rowOffset,
									mapy * Tile.TileStepY - (heightRow * Tile.HeightTileOffset))),

							// Determine what type of tile to draw
							Tile.GetSourceRectangle(Tile.GetTileIndex(tileID, curRotation)),

							Color.White, // Draw in original colors
							0.0f, // No Rotation
							Vector2.Zero, // Origin is not moved

							drawratio, // scaling
							
							SpriteEffects.None, // No flips

							// Increase the depthoffset by 0.0000001f every height we add
							// this will allow it to move above existing items without
							// interfering with the surrounding depth values
							depthOffset - ((float)heightRow * heightRowDepthMod));
						}
						#endregion

						heightRow++; // Increment the height and redraw the tile when we have one
					}
					#endregion

					// Draw all Topper tiles in viewport
					#region TopperTiles

					float toppercount = 0.0f;
					// Loop through all of the topper tiles in the current cell
					// holding the current height level so we can add everything 
					// at the same level
					foreach (int tileID in myMap.Rows[y + firstY].Columns[x + firstX].TopperTiles)
					{
						
						#region Hidden Mode
						if(view == 1)
						{
							spriteBatch.Draw(

							// Texture to be drawn
							Flatframe,// (this.view == 1 ? Flatframe : text), //Tile.TileSetTexure,

							// Where on the 'screen' to draw a tile, use a row offset for odd row hexes
							camera.WorldtoScreen(
									new Vector2(
										(mapx * Tile.TileStepX) + rowOffset,
										(mapy * Tile.TileStepY) - (heightRow * Tile.HeightTileOffset))),

							// Determine what type of tile to draw
							null,

							Color.White, // Draw in original colors
							0.0f, // No rotation
							Vector2.Zero, // Origin is not moved
							1.0f,
							SpriteEffects.None, // No flips

							// Increase the depthoffset by 0.0000001f every height we add
								// this will allow it to move above existing items without
								// interfering with the surrounding depth values
							depthOffset - ((float)heightRow * heightRowDepthMod) - toppercount);
							toppercount += 0.0000001f;
						}
						#endregion

						#region Movement Mode
						else if(view == 2)
						{
						spriteBatch.Draw(

							// Texture to be drawn
							activeArea,// (this.view == 1 ? Flatframe : text), //Tile.TileSetTexure,

							// Where on the 'screen' to draw a tile, use a row offset for odd row hexes
							camera.WorldtoScreen(
									new Vector2(
										(mapx * Tile.TileStepX) + rowOffset, 
										(mapy * Tile.TileStepY) - (heightRow * Tile.HeightTileOffset))),

							// Determine what type of tile to draw
							Tile.GetSourceRectangle(tileID),

							(myMap.Rows[mapy].Columns[mapx].walkable) ? Color.White * 1.5f : Color.DarkGray, // Draw in original colors
							0.0f, // No rotation
							Vector2.Zero, // Origin is not moved
							1.0f,
							SpriteEffects.None, // No flips

							// Increase the depthoffset by 0.0000001f every height we add
							// this will allow it to move above existing items without
							// interfering with the surrounding depth values
							depthOffset - ((float)heightRow * heightRowDepthMod ) - toppercount);
							toppercount += 0.0000001f;
							}

						#endregion

						#region Normal Mode
						else
						{
							spriteBatch.Draw(

								// Texture to be drawn
								text,// (this.view == 1 ? Flatframe : text), //Tile.TileSetTexure,

								// Where on the 'screen' to draw a tile, use a row offset for odd row hexes
								camera.WorldtoScreen(
										new Vector2(
											(mapx * Tile.TileStepX) + rowOffset, 
											(mapy * Tile.TileStepY) - (heightRow * Tile.HeightTileOffset))),

								// Determine what type of tile to draw
								Tile.GetSourceRectangle(Tile.GetTileIndex(tileID, curRotation)),

								Color.White, // Draw in original colors
								0.0f, // No rotation
								Vector2.Zero, // Origin is not moved
								drawratio,
								SpriteEffects.None, // No flips

								// Increase the depthoffset by 0.0000001f every height we add
								// this will allow it to move above existing items without
								// interfering with the surrounding depth values
								depthOffset - ((float)heightRow * heightRowDepthMod ) - toppercount);
							toppercount += 0.0000001f;
						}
						#endregion

					}
					#endregion 

					// Draw all Scenery tiles in viewport
					#region SceneryTiles

					foreach (var sceneTile in myMap.Rows[y + firstY].Columns[x + firstX].SceneryTiles)
					{
						#region Hidden Mode
						if (view == 1)
						{
							spriteBatch.Draw(

							// Texture to be drawn
							activeArea,

							// Where on the 'screen' to draw a tile, use a row offset for odd row hexes
							camera.WorldtoScreen(
									new Vector2(
										(mapx * Tile.TileStepX) + rowOffset,
										(mapy * Tile.TileStepY) - (heightRow * Tile.HeightTileOffset) + Tile.HeightTileOffset)),

							// Determine what type of tile to draw
							null,

							Color.DarkSlateBlue, // Draw in original colors
							0.0f, // No rotation
							Vector2.Zero, // Origin is not moved
							1.0f,
							SpriteEffects.None, // No flips

							// Increase the depthoffset by 0.0000001f every height we add
								// this will allow it to move above existing items without
								// interfering with the surrounding depth values
							depthOffset - ((float)heightRow * heightRowDepthMod) - toppercount);
							toppercount += 0.0000001f;
						}
						#endregion

						#region Movement Mode
						else if (view == 2)
						{
							spriteBatch.Draw(
								// Texture to be drawn
								activeArea,//text,
								// Where on the 'screen' to draw a tile, use a row offset for odd row hexes
								camera.WorldtoScreen(
										new Vector2((mapx * Tile.TileStepX) + rowOffset,
														mapy * Tile.TileStepY + 32)),

								// Determine what type of tile to draw
								null,//Tile.GetSourceRectangle(tileID),

								(myMap.Rows[mapy].Columns[mapx].walkable) ? Color.White * 1.5f : Color.DarkGray, // Draw in original colors
								0.0f, // No Rotation 
								Vector2.Zero, // 0, 0 origin
								1.0f,// No scaling
								SpriteEffects.None, // No flips
								1.0f - basecount); // Starting layer

							// Increment depth offset to stack tiles
							
						}

						#endregion

						#region Normal Mode
						else
						{
							Tile.tileValues curTile = Tile.tilesetMap[sceneTile.tileID];
							Vector2 sceneryOffset = new Vector2(-(curTile.imageDim.X - 1) / 2, curTile.imageDim.Y + (int)((curTile.imageDim.Y + 1) / 2));
							// Draw the selected tile at the max height of the current tile
							spriteBatch.Draw(Tile.TileSetTexture,
								camera.WorldtoScreen(
									new Vector2(
										((mapx + (int)sceneryOffset.X) * Tile.TileStepX) + rowOffset
										+ sceneTile.cubePos.X, // X placement
										((mapy + 2) * Tile.TileStepY) -
										(int)sceneryOffset.Y * Tile.HeightTileOffset -
										heightRow * Tile.HeightTileOffset + sceneTile.cubePos.Y)), // Y placement

									Tile.GetSourceRectangle(Tile.GetTileIndex(sceneTile.tileID, curRotation)),
									Color.White,
									0.0f,
									Vector2.Zero,
									drawratio,
									SpriteEffects.None,
									depthOffset - ((float)heightRow * heightRowDepthMod) - basecount);
							basecount += 0.0000001f;
						}
						#endregion
					}
					#endregion

					///////////////////////////////////HIDING THE SPRITE/////////////////////////////
					// Modify the depth of the sprite when it is behind a heightened tile
					if ((mapx == vladMapPoint.X) && (mapy == vladMapPoint.Y))
					{
						vlad.DrawDepth = depthOffset - ((float)heightRow + 2) * heightRowDepthMod;
					} 

					// Display relative coordinate positions when displaycoords in enabled
					if(displayCoords == 1)
                	{
					
					spriteBatch.DrawString(pericles6, (x + firstX).ToString() + ", " + (y + firstY).ToString(),
											new Vector2((x * Tile.TileStepX) - offsetX + rowOffset + baseOffsetX + 24,
											(y * Tile.TileStepY) - offsetY + baseOffsetY + 48), Color.White, 0f, Vector2.Zero,
											1.0f, SpriteEffects.None, 0.01f);  
					}
				}
			}
			#endregion

			#region mouseImages

			// Deterime mouse screen and map cell coordinates
			Vector2 hilightLoc = camera.ScreentoWorld(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
			Point hilightPoint = myMap.WorldToMapCell(new Point((int) hilightLoc.X, (int) hilightLoc.Y));

			// Use an offset on odd rows
			#region Set SelectionList
			int hilightrowOffset = 0;
			int selectionOffset = 0;
			int negate = 1;
			if((hilightPoint.Y) % 2 == 1)
			{
				hilightrowOffset =(int)(Tile.OddRowXOffset );
				selectionOffset = 1;
				selectionList = selectionOddList;
				negate = -1;
			}
			else
				selectionList = selectionEvenList;
			#endregion

			// Draw the hilight of the selection size the the current mapcell 
			#region Draw Highlight
			for (int sel = 0; sel < gui.selectionSize*gui.selectionSize; sel++)
			{
				if(sel % 2 == 1)
					selectionOffset = negate * (int)Tile.OddRowXOffset;
				else
					selectionOffset = 0;

				spriteBatch.Draw(
						hilight, // Texture to be drawn	
						camera.WorldtoScreen( // Convert to screen coords
							new Vector2(
								(( hilightPoint.X + selectionList.ElementAt(sel).X) * Tile.TileStepX ) + hilightrowOffset + selectionOffset, // X location
								(hilightPoint.Y + 2 + selectionList.ElementAt(sel).Y) * Tile.TileStepY)),// Y location
					
						new Rectangle(0, 0, 64, 32), // Texture image reference
						Color.White * 0.3f, // Invoke transparency
						0.0f, // no rotation
						Vector2.Zero, // origin at 0,0
						1.0f, // no scaling
						SpriteEffects.None, // no effects
						0.011f); // Draw above other tiles
			}
			#endregion

			// Determine current map cell location 
			int mouseMapx = (firstX + hilightPoint.X);
			int mouseMapy = (firstY + hilightPoint.Y + (2 * myMap.Layers.Count));

			Vector2 MouseLoc = camera.ScreentoWorld(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
			Point CubeSpacePoint;
			Point MousePoint = myMap.WorldToMapCell(new Point((int)MouseLoc.X, (int)MouseLoc.Y), out CubeSpacePoint);

			// layer offset
			hilightPoint.Y += (2 * myMap.Layers.Count);
			MousePoint.Y += (2 * myMap.Layers.Count);

			// Calculate depth at current location
			// Determine current map cell location 
			float mousedepthOffset = 0.7f - ((mouseMapx + (mouseMapy  * Tile.tileWidth)) / maxdepth);
			
			// Get the count of height cells at the current location
			int heightCount;
			try
			{
				heightCount = myMap.Rows[MousePoint.Y].Columns[MousePoint.X].HeightTiles.Count;
			}
			catch
			{
				heightCount = 0;
			}
			
			// If the magnifying glass tool is selected
			if(gui.magnify)
			{
				#region Draw Magnifiy
				// Determine with glass to draw depending on state
				int multiplier = 1;
				if(ctrlkey)
					multiplier = 2;

				// Draw the glass at the max height of the current tile
				spriteBatch.Draw(MagGlass,
						camera.WorldtoScreen(
								new Vector2(
									(hilightPoint.X * Tile.TileStepX) + hilightrowOffset, // X placement
									((hilightPoint.Y + 2) * Tile.TileStepY - Tile.HeightTileOffset) - // Y placement
									Tile.tileHeight / 4)),
								new Rectangle(0 + MagOffset * multiplier, 0, 24, 23),
								Color.White,
								0.0f,
								new Vector2(-gui.ActionBox[2].Width / gui.xguiScale, -gui.ActionBox[2].Height * 2 / gui.yguiScale),
								1.0f,
								SpriteEffects.None,
								mousedepthOffset - (0.0001f * heightCount));
				#endregion
			}
			
			else
			{

				int mouseX, mouseY, selMax = 0, prevMax = 0;
				#region Set Trash Offset
				float trashdepthOffset = 0;
				for (int findmax = 0; findmax < gui.selectionSize * gui.selectionSize; findmax++)
				{
					mouseY = MousePoint.Y + selectionList.ElementAt(findmax).Y;
					mouseX = MousePoint.X + selectionList.ElementAt(findmax).X;
					try
					{
						prevMax = selMax;
						selMax = Math.Max(selMax, myMap.Rows[mouseY].Columns[mouseX].HeightTiles.Count);
						if(prevMax != selMax)
							trashdepthOffset = 0.7f - (((mouseMapx + mouseX) + ((mouseMapy + mouseY) * Tile.tileWidth)) / maxdepth);
					}
					catch //If selection leaves map
					{}
				}
				#endregion

				// If the trash tool is selected
				if (gui.trash)
				{
					#region Draw Trash
					// Draw the trash tool icon at the max height of the current tile
					spriteBatch.Draw(trash,
							camera.WorldtoScreen(
									new Vector2(
										(hilightPoint.X * Tile.TileStepX) + hilightrowOffset, // X placement
										((hilightPoint.Y + 2) * Tile.TileStepY - Tile.HeightTileOffset) - // Y placement
										selMax  * Tile.HeightTileOffset - (Tile.tileHeight * (2 - gui.selectionSize) / 4 ))),
									null,
									Color.White,
									0.0f,
									new Vector2(-gui.ActionBox[2].Width / gui.xguiScale, -gui.ActionBox[2].Height * 2 / gui.yguiScale),
									1.0f,
									SpriteEffects.None,
									(trashdepthOffset == 0.0 ? mousedepthOffset : trashdepthOffset) - (0.001f * selMax));
					#endregion
				}

				for(int sel = 0; sel < gui.selectionSize*gui.selectionSize; sel++)
				{
					if(MousePoint.Y % 2 == 1)
					{
						selectionList = selectionOddList;
						negate = -1;
					}
					else
					{
						selectionList = selectionEvenList;
						negate = 1;
					}
					mouseY = MousePoint.Y + selectionList.ElementAt(sel).Y;
					mouseX = MousePoint.X + selectionList.ElementAt(sel).X;					
					
					mousedepthOffset = 0.7f - ((mouseX + (mouseY * Tile.tileWidth)) / maxdepth) - 0.0000001f;

					try
					{
						heightCount = myMap.Rows[mouseY].Columns[mouseX].HeightTiles.Count;
					}
					catch
					{ // If selection leaves map
						heightCount = 0;
					}

					if(sel % 2 == 1)
						selectionOffset = negate * (int)Tile.OddRowXOffset;
					else
						selectionOffset = 0;
					
					#region Draw Helping Placement Marker
					if(heightCount != 0 && (layTrash ? heightCount == selMax : true))
						spriteBatch.Draw(activeArea,
							camera.WorldtoScreen(
									new Vector2(
										((hilightPoint.X + selectionList.ElementAt(sel).X) * Tile.TileStepX) + hilightrowOffset + selectionOffset, // X placement
										((hilightPoint.Y + 2 + selectionList.ElementAt(sel).Y) * Tile.TileStepY) - // Y placement
												Tile.HeightTileOffset * (layTrash ? selMax : heightCount))),
									null,
									Color.White * 0.3f,
									0.0f,
									Vector2.Zero,
									1.0f,
									SpriteEffects.None,
									0.001f);
					#endregion
					
					//Draw current tile
					if(!gui.trash)
					{
						if(Tile.tilesetMap[gui.currentTile].type == 3)
						{					
							#region Draw Scenery Tiles
					
							if(cubeSpace)
							{
								CubeSpacePoint.Y = CubeSpacePoint.Y - 16;

								CubeSpacePoint.X = ((CubeSpacePoint.X) / 4) * 4 - 32;
								CubeSpacePoint.Y = (CubeSpacePoint.Y / 2) * 2;
		
								Vector2 CubeSpaceOffset = new Vector2(CubeSpacePoint.X, CubeSpacePoint.Y);//Vector2.Zero;
							
								Tile.tileValues curTile = Tile.tilesetMap[gui.currentTile];
								Vector2 sceneryOffset = new Vector2(-(curTile.imageDim.X - 1) / 2, curTile.imageDim.Y + (int)((curTile.imageDim.Y + 1) / 2));
								// Draw the selected tile at the max height of the current tile
								spriteBatch.Draw(Tile.TileSetTexture,
									camera.WorldtoScreen(
										new Vector2(
											((hilightPoint.X + (int)sceneryOffset.X + selectionList.ElementAt(sel).X ) * Tile.TileStepX) + 
												hilightrowOffset + selectionOffset + CubeSpaceOffset.X, // X placement
											((hilightPoint.Y + 2 + selectionList.ElementAt(sel).Y) * Tile.TileStepY) -
											(int)sceneryOffset.Y * Tile.HeightTileOffset -
											heightCount * Tile.HeightTileOffset + 
											CubeSpaceOffset.Y)),// Y placement

										Tile.GetSourceRectangle(Tile.GetTileIndex(gui.currentTile, curRotation)),
										Color.White,
										0.0f,
										Vector2.Zero,
										drawratio,
										SpriteEffects.None,
										mousedepthOffset - (0.0001f * heightCount));
	
							}
							else 
							{

								Tile.tileValues curTile = Tile.tilesetMap[gui.currentTile];
								Vector2 sceneryOffset = new Vector2(-(curTile.imageDim.X - 1) / 2, curTile.imageDim.Y + (int)((curTile.imageDim.Y + 1) / 2) );
								// Draw the selected tile at the max height of the current tile
								spriteBatch.Draw(Tile.TileSetTexture,
									camera.WorldtoScreen(
										new Vector2(
											((hilightPoint.X + (int)sceneryOffset.X + selectionList.ElementAt(sel).X) * Tile.TileStepX) + hilightrowOffset + selectionOffset, // X placement
											((hilightPoint.Y + 2 + selectionList.ElementAt(sel).Y) * Tile.TileStepY) -  
											(int)sceneryOffset.Y * Tile.HeightTileOffset -
											heightCount * Tile.HeightTileOffset)),// Y placement
												
										Tile.GetSourceRectangle(Tile.GetTileIndex(gui.currentTile, curRotation)),
										Color.White,
										0.0f,
										Vector2.Zero,
										drawratio,
										SpriteEffects.None,
										mousedepthOffset - (0.0001f * heightCount));

							}

							#endregion
						}
						else
						{
							#region Draw Non-scenery tiles
							// Draw the selected tile at the max height of the current tile
							spriteBatch.Draw(Tile.TileSetTexture,
								camera.WorldtoScreen(
									new Vector2(
										((hilightPoint.X + selectionList.ElementAt(sel).X) * Tile.TileStepX) + hilightrowOffset + selectionOffset, // X placement
										((hilightPoint.Y + 2 + selectionList.ElementAt(sel).Y) * Tile.TileStepY) - Tile.HeightTileOffset - // Y placement
												Tile.HeightTileOffset * heightCount)),
									Tile.GetSourceRectangle(Tile.GetTileIndex(gui.currentTile, curRotation)),
									Color.White,
									0.0f,
									Vector2.Zero,
									drawratio,
									SpriteEffects.None,
									mousedepthOffset - (0.0001f * heightCount));
							#endregion

						}
						if(!walkable || cubeSpace)
						{
							#region Draw Not-Walkable icon
							// Draw the selected tile at the max height of the current tile
							spriteBatch.Draw((cubeSpace ? CubeText : NoWalk),
								camera.WorldtoScreen(
										new Vector2(
											((hilightPoint.X + selectionList.ElementAt(sel).X) * Tile.TileStepX) + hilightrowOffset + selectionOffset - 20, // X placement
											((hilightPoint.Y + 2 + selectionList.ElementAt(sel).Y) * Tile.TileStepY) - Tile.HeightTileOffset - // Y placement
													Tile.HeightTileOffset * heightCount)),
										null,
										Color.White,
										0.0f,
										Vector2.Zero,
										1.0f,
										SpriteEffects.None,
										0.00015f );
							#endregion
						}	
					}
				}
			}

			#endregion

			#region EventImages
			if(ShowEvents == 1)
			{

				int startOffset = 0;
				if (StartingPos.Y % 2 == 1)
				{
					startOffset = Tile.OddRowXOffset;
				}
				spriteBatch.Draw(
						charStart, // Texture to be drawn
						camera.WorldtoScreen( // Convert to screen coords
							new Vector2(
								(StartingPos.X * Tile.TileStepX) + startOffset, // X location
								(StartingPos.Y + 2) * Tile.TileStepY)), // Y location

						new Rectangle(0, 0, 64, 32), // Texture image reference
						Color.White * 0.3f, // Invoke transparency
						0.0f, // no rotation
						Vector2.Zero, // origin at 0,0
						1.0f, // no scaling
						SpriteEffects.None, // no effects
						0.011f); // Draw above other tiles

				foreach( var Event in Events)
				{
					#region SwapEvents
					if(Event.ActionData is SwapData)
					{
						List<Vector3> slist = new List<Vector3>(((SwapData)Event.ActionData).InitialPos);
						List<Vector3> elist = new List<Vector3>(((SwapData)Event.ActionData).FinalPos);
				
						// Draw the starting position
						foreach( var ipos in slist)
						{
							int swaprowOffset = 0;
							if((ipos.Y) % 2 == 1)
							{
								swaprowOffset = Tile.OddRowXOffset;
							}

							spriteBatch.Draw(
									sStart, // Texture to be drawn
									camera.WorldtoScreen( // Convert to screen coords
										new Vector2(
											(ipos.X * Tile.TileStepX) + swaprowOffset, // X location
											(ipos.Y + 2) * Tile.TileStepY)), // Y location

									new Rectangle(0, 0, 64, 32), // Texture image reference
									Color.White * 0.3f, // Invoke transparency
									0.0f, // no rotation
									Vector2.Zero, // origin at 0,0
									1.0f, // no scaling
									SpriteEffects.None, // no effects
									0.011f); // Draw above other tiles
						}
						foreach( var fpos in elist)
						{
							int swaprowOffset = 0;
							if ((fpos.Y) % 2 == 1)
							{
								swaprowOffset = Tile.OddRowXOffset;
							}

							spriteBatch.Draw(
									sEnd, // Texture to be drawn
									camera.WorldtoScreen( // Convert to screen coords
										new Vector2(
											(fpos.X * Tile.TileStepX) + swaprowOffset, // X location
											(fpos.Y + 2) * Tile.TileStepY)), // Y location

									new Rectangle(0, 0, 64, 32), // Texture image reference
									Color.White * 0.3f, // Invoke transparency
									0.0f, // no rotation
									Vector2.Zero, // origin at 0,0
									1.0f, // no scaling
									SpriteEffects.None, // no effects
									0.011f); // Draw above other tiles
						}
					}
					#endregion

					#region Event Activation Area
					for(int ax = Event.ActivationArea.X; ax < Event.ActivationArea.Width + Event.ActivationArea.X; ax++)
						for(int ay = Event.ActivationArea.Y; ay < Event.ActivationArea.Height + Event.ActivationArea.Y; ay++)
						{
							int swaprowOffset = 0;
							if (ay % 2 == 1)
							{
								swaprowOffset = Tile.OddRowXOffset;
							}
								spriteBatch.Draw(
									activeArea, // Texture to be drawn
									camera.WorldtoScreen( // Convert to screen coords
										new Vector2(
											(ax * Tile.TileStepX) + swaprowOffset, // X location
											(ay + 2) * Tile.TileStepY)), // Y location

									new Rectangle(0, 0, 64, 32), // Texture image reference
									Color.White * 0.3f, // Invoke transparency
									0.0f, // no rotation
									Vector2.Zero, // origin at 0,0
									1.0f, // no scaling
									SpriteEffects.None, // no effects
									0.011f); // Draw above other tiles
						}
					#endregion
				}
			}
			#endregion

			// Draw the gui
			gui.Draw(spriteBatch, baseOffsetX, baseOffsetY, camera.Zoom, myMap.rotation);

			if(!makemap)
				vlad.Draw(spriteBatch, 0, -myMap.GetOverallHeight(vlad.Position));

			// End drawing
			spriteBatch.End();

            base.Draw(gameTime);
        }

		/// <summary>
		/// When invoked, fires all event functions that 
		/// exist within the current location
		/// </summary>
		/// <param name="Location">Location to check for events</param>
		/// <param name="sprite">current sprite for event to act upon</param>
		private void UpdateEvents(Point Location, SpriteAnimation sprite)
		{
			foreach (TileEventObject events in Events)
			{
				if (events.ActivationArea.Contains(Location))
				{
					if (events.ActionData is TileSetData)
					{
						MapActions.ChangeTileSets((TileSetData)events.ActionData);
					}
					else if (events.ActionData is SpriteSheetData)
					{
						MapActions.ChangeSpriteSheets(sprite, (SpriteSheetData)events.ActionData);
					}
					else if (events.ActionData is SwapData)
					{
						MapActions.SwapCells(myMap, (SwapData)events.ActionData);
					}
				}
			}
		}

		/// <summary>
		/// Called when the app window attempts to close
		/// Prompts to save the map if unsaved changes are detected
		/// </summary>
		/// <param name="sender">Window object</param>
		/// <param name="e">Event argument list</param>
		private void CheckSave(object sender, System.Windows.Forms.FormClosingEventArgs e)
		{
			if(!SaveLevel.saved)
			{
				var message = System.Windows.Forms.MessageBox.Show("\t   Unsaved Changes!\n                 Would you like to Save?", "Save?", System.Windows.Forms.MessageBoxButtons.YesNo);
				
				if(message == System.Windows.Forms.DialogResult.OK)
				{
					// Initialize save data
					SaveLevel.TILESET = tilesetfile;
					SaveLevel.SLOPEMAP = slopemapfile;
					SaveLevel.TILEWIDTH = Tile.tileWidth;
					SaveLevel.TILEHEIGHT = Tile.tileHeight;
					SaveLevel.TILESTEPX = Tile.TileStepX;
					SaveLevel.TILESTEPY = Tile.TileStepY;
					SaveLevel.HEIGHTTILEOFFSET = Tile.HeightTileOffset;
					SaveLevel.ODDROWOFFSETX = Tile.OddRowXOffset;
					SaveLevel.MAPHEIGHT = myMap.mapHeight;
					SaveLevel.MAPWIDTH = myMap.mapWidth;
					SaveLevel.initData(myMap.Rows);
					SaveLevel.Author = autbox.name;
					SaveLevel.Date = autbox.date;
					SaveLevel.Notes = autbox.notes;

					for(int i = 0; i < 4; i++)
					{
						if(myMap.rotation == 0)
							break;
						else
							myMap.RotateMapClockwise(true);
					}
					
					// Write data to file
					SaveLevel.write();

				}
			}
		}
	}
}
