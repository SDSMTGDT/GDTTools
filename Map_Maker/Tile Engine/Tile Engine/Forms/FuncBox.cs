using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tile_Engine
{
	public partial class FuncBox : Form
	{
		private Button confirmBut;
		private ComboBox FuncCombo;
		private TextBox astartxtext;
		private TextBox astartytext;
		private TextBox aendxtext; 
		private TextBox aendytext;

		// Data Structs
		public struct ActivationArea
		{
			public uint startx;
			public uint starty;
			public uint endx;
			public uint endy;
			public ActivationArea(ActivationArea acivate)
			{
				this.startx = acivate.startx;
				this.starty = acivate.starty;
				this.endx = acivate.endx;
				this.endy = acivate.endy;
			}
			public ActivationArea(uint sx, uint sy, uint ex, uint ey)
			{
				startx = sx;
				starty = sy;
				endx = ex;
				endy = ey;
			}
		}

		public struct CellSwapEvent
		{
			public uint bstartx;
			public uint bstarty;
			public uint bendx;
			public uint bendy;
			public int fstartx;
			public int fstarty;
			public int fendx;
			public int fendy;
			public ActivationArea activate;
			public CellSwapEvent(uint bsx, uint bsy, uint bex, uint bey, int fsx, int fsy, int fex, int fey, ActivationArea active)
			{
				bstartx = bsx;
				bstarty = bsy;
				bendx = bex;
				bendy = bey;
				fstartx = fsx;
				fstarty = fsy;
				fendx = fex;
				fendy = fey;
				activate = active;
			} 
		}

		public struct TilesetChangeEvent
		{
			public string filepath;
			public uint tilewidth;
			public uint tileheight;
			public uint tilestepx;
			public uint tilestepy;
			public uint oddrowoffset;
			public uint heighttileoffset;
			public ActivationArea activate;
			public TilesetChangeEvent(string fpath, uint twidth, uint theight, uint tstepx, uint tstepy, uint oddoff, uint heightoff, ActivationArea active)
			{
				filepath = fpath;
				tilewidth = twidth;
				tileheight = theight;
				tilestepx = tstepx;
				tilestepy = tstepy;
				oddrowoffset = oddoff;
				heighttileoffset = heightoff;
				activate = active;
			}
		}

		public struct Animations
		{
			public string spName;
			public uint spX;
			public uint spY;
			public uint spWidth;
			public uint spHeight;
			public uint spFrames;
			public float spFrameLength;
			public string spNextAnimation;
			public Animations(string name, uint sx, uint sy, uint swidth, uint sheight, uint sframes, float sflen, string next)
			{
				spName = name;
				spX = sx;
				spY = sy;
				spWidth = swidth;
				spHeight = sheight;
				spFrames = sframes;
				spFrameLength = sflen;
				spNextAnimation = next;
			}
		}

		public struct SpriteSheetChangeEvent
		{
			public string spfilepath;
			public List<Animations> animations;
			public ActivationArea activate;
			public SpriteSheetChangeEvent(string fpath, List<Animations> animate, ActivationArea active)
			{
				spfilepath = fpath;
				animations = new List<Animations>();
				animations.AddRange(animate);
				activate = active;
			}
		}

		public List<Animations> animate = new List<Animations>();

		// Event Lists
		public List<CellSwapEvent> swaps = new List<CellSwapEvent>();
		public List<TilesetChangeEvent> tilesets = new List<TilesetChangeEvent>();
		public List<SpriteSheetChangeEvent> spritesheets = new List<SpriteSheetChangeEvent>();

		// Activation Area vars
		public uint astartx;
		public uint astarty;
		public uint aendx;
		public uint aendy;
		
		// Cell swap vars
		public uint swbstartx = 0;
		public uint swbstarty = 0;
		public uint swbendx = 0;
		public uint swbendy = 0;
		public uint swfstartx = 0;
		public uint swfstarty = 0;
		public uint swfendx = 0;
		public uint swfendy = 0;

		// Tileset change vars
		public string tsfilepath = "";
		public uint tstilewidth = 0;
		public uint tstileheight = 0;
		public uint tstilestepx = 0;
		public uint tstilestepy = 0;
		public uint tsoddrowoffset = 0;
		public uint tsheighttileoffset = 0;

		// Spritesheet change vars
		public string spfilepath = "";
		public string spName = "";
		public uint spX = 0;
		public uint spY = 0;
		public uint spWidth = 0;
		public uint spHeight = 0;
		public uint spFrames = 0;
		public float spFrameLength = 0f;
		public string spNextAnimation = "";

		// Exit code
		public bool ok = true;
		List<TileEventObject> events;
		private int swcount = 0;
		private int tscount = 0;
		private int spcount = 0;

		public int swCount
		{
			get{ return swcount; }
			set{ swcount = (value < 0 ? 0 : value); }
		}

		public int tsCount
		{
			get { return tscount; }
			set { tscount = (value < 0 ? 0 : value); }
		}
		public int spCount
		{
			get{ return spcount; }
			set{ spcount = (value < 0 ? 0 : value); }
		}

		private int swindex = 1;
		private int tsindex = 3;
		private int spindex = 5;
		private bool showbox;

		public FuncBox(List<TileEventObject> gevents)
		{
			events = new List<TileEventObject>(gevents);
			InitializeComponent();
		}
	
		public void FuncBox_Load(object sender, EventArgs e)
		{
			int topref = 20;
		
			swCount = 0;
			tsCount = 0;
			spCount = 0;
			ok = true;
			showbox = false;
			Panel FuncPanel = new Panel(){ Width = 200, Height = 60 };
			Label FuncLabel = new Label(){ Text = "Functions", Top = topref, Height = 15, Left = 30 };
			FuncCombo = new ComboBox(){ Top = topref + 20, Left = 30, Width = 130};
			FuncCombo.Items.Add("Events List");
			FuncCombo.Items.Add("Swap Cell Data");
			FuncCombo.Items.Add("Change Tilesets");
			FuncCombo.Items.Add("Change SpriteSheets");
			FuncCombo.AllowDrop = false;
			FuncCombo.DropDownStyle = ComboBoxStyle.DropDownList;
			FuncCombo.SelectedIndex = 0;
			FuncCombo.SelectedIndexChanged += new EventHandler(FuncComboChange);

			FuncPanel.Controls.Add(FuncLabel);
			FuncPanel.Controls.Add(FuncCombo);

			Panel ActivationPanel = new Panel() { Top = 80, Width = 200, Height = 100 };
			Label ActivationLabel = new Label() { Text = "Activation Area", Top = 0, Left = 30, Height = 15 };
			Label AStartx = new Label() { Text = "Start X", Top = topref, Left = 30, Width = 40, Height = 15 };
			Label AStarty = new Label() { Text = "Start Y", Top = topref, Left = 90, Width = 40, Height = 15 };
			astartxtext = new TextBox() { Text = "0", Top = topref + 15, Left = 30, Width = 50 };
			astartytext = new TextBox() { Text = "0", Top = topref + 15, Left = 90, Width = 50 };
			
			Label AEndx = new Label() { Text = "End X", Top = topref + 40, Left = 30, Width = 40, Height = 15 };
			Label AEndy = new Label() { Text = "End Y", Top = topref + 40, Left = 90, Width = 40, Height = 15 };
			aendxtext = new TextBox() { Text = "0", Top = topref + 55, Left = 30, Width = 50 };
			aendytext = new TextBox() { Text = "0", Top = topref + 55, Left = 90, Width = 50 };

			ActivationPanel.Controls.Add(ActivationLabel);
			ActivationPanel.Controls.Add(AStartx);
			ActivationPanel.Controls.Add(AStarty);
			ActivationPanel.Controls.Add(astartxtext);
			ActivationPanel.Controls.Add(astartytext);
			ActivationPanel.Controls.Add(AEndx);
			ActivationPanel.Controls.Add(AEndy);
			ActivationPanel.Controls.Add(aendxtext);
			ActivationPanel.Controls.Add(aendytext);

			SetUpExternPanels();

			Button addButton = new Button(){Name = "Add", Text = "Add", Left = 30, Top = this.Height - 100, Width = 65};
			addButton.Click += new EventHandler(addButton_Click);
			addButton.Visible = false;

			Button removeButton = new Button() {Name = "Remove", Text = "Remove", Left = 100, Top = this.Height - 100, Width = 65};
			removeButton.Click += new EventHandler(removeButton_Click);

			confirmBut = new Button(){Text = "Ok", Left = 30, Top = this.Height - 70, Width = 65};
			confirmBut.Click += new EventHandler(confirmBut_Click);

			Button cancelBut = new Button() {Text = "Cancel", Left = 100, Top = this.Height - 70, Width = 65};
			cancelBut.Click += new EventHandler(cancelBut_Click);
			
			this.AcceptButton = confirmBut;
			this.CancelButton = cancelBut;

			this.Controls.Add(FuncPanel);
			this.Controls.Add(ActivationPanel);

			this.Controls.Add(addButton);
			this.Controls.Add(confirmBut);
			this.Controls.Add(removeButton);
			this.Controls.Add(cancelBut);
			this.Deactivate += new EventHandler(cancelBut_Click);
			this.Location = new Point(50, 80);

			InitializeEvents();
		}

		public void InitializeEvents()
		{

			var swaplist = (ListBox)this.Controls.Find("Swap Events", true).ElementAt(0);
			var tilelist = (ListBox)this.Controls.Find("Tile Events", true).ElementAt(0);
			var spritelist = (ListBox)this.Controls.Find("Sprite Events", true).ElementAt(0);
			var allEvents = (ListBox)this.Controls.Find("Events List",true).ElementAt(0);

			foreach (var gEvent in events)
			{
				#region Swaps
				if (gEvent.ActionData is SwapData)
				{
					swaplist.BeginUpdate();
					var data = ((SwapData)gEvent.ActionData);
					//Single
					if (data.InitialPos.Count == 1)
					{
						swaplist.Items.Add("CELL SWAP: (" + data.InitialPos.ElementAt(0).X + " , " + data.InitialPos.ElementAt(0).Y +
											") <-> (" + data.FinalPos.ElementAt(0).X + " , " + data.FinalPos.ElementAt(0).Y +
											") | Activation Area: (" + gEvent.ActivationArea.X + " , " + gEvent.ActivationArea.Y +
											") - (" + (int)(gEvent.ActivationArea.X + gEvent.ActivationArea.Width) +
											" , " + (int)(gEvent.ActivationArea.Y + gEvent.ActivationArea.Height) + ")");

						swaps.Add(new CellSwapEvent((uint)data.InitialPos.ElementAt(0).X, (uint)data.InitialPos.ElementAt(0).Y,
													(uint)data.FinalPos.ElementAt(0).X, (uint)data.FinalPos.ElementAt(0).Y, -1, -1, -1, -1,
													new ActivationArea((uint)gEvent.ActivationArea.X, (uint)gEvent.ActivationArea.Y,
															(uint)(gEvent.ActivationArea.X + gEvent.ActivationArea.Width),
															(uint)(gEvent.ActivationArea.Y + gEvent.ActivationArea.Height))));

					}
					//Range
					else
					{
						var listend = data.InitialPos.Count - 1;
						swaplist.Items.Add("RANGE SWAP: Initial: (" + data.InitialPos.ElementAt(0).X + " , " + data.InitialPos.ElementAt(0).Y +
											") - (" + data.InitialPos.ElementAt(listend).X + " , " + data.InitialPos.ElementAt(listend).Y +
											") <-> Final: (" + data.FinalPos.ElementAt(0).X + " , " + data.FinalPos.ElementAt(0).Y +
											") - (" + data.FinalPos.ElementAt(listend).X + " , " + data.FinalPos.ElementAt(listend).Y +
											") | Activation Area: (" + gEvent.ActivationArea.X + " , " + gEvent.ActivationArea.Y +
											") - (" + (int)(gEvent.ActivationArea.X + gEvent.ActivationArea.Width) +
											" , " + (int)(gEvent.ActivationArea.Y + gEvent.ActivationArea.Height) + ")");

						swaps.Add(new CellSwapEvent((uint)data.InitialPos.ElementAt(0).X, (uint)data.InitialPos.ElementAt(0).Y,
													(uint)data.InitialPos.ElementAt(listend).X, (uint)data.InitialPos.ElementAt(listend).Y,
													(int)data.FinalPos.ElementAt(0).X, (int)data.FinalPos.ElementAt(0).Y,
													(int)data.FinalPos.ElementAt(listend).X, (int)data.FinalPos.ElementAt(listend).Y,
													new ActivationArea((uint)gEvent.ActivationArea.X, (uint)gEvent.ActivationArea.Y,
															(uint)(gEvent.ActivationArea.X + gEvent.ActivationArea.Width),
															(uint)(gEvent.ActivationArea.Y + gEvent.ActivationArea.Height))));
					}

					swaplist.EndUpdate();
				}
				#endregion
				
				#region TileSet Changes
				if (gEvent.ActionData is TileSetData)
				{
					tilelist.BeginUpdate();
					var data = ((TileSetData)gEvent.ActionData);

					int fileindex = gEvent.PathInfo.LastIndexOf('\\');
					string fname = gEvent.PathInfo;

					if (fileindex != -1)
						fname = gEvent.PathInfo.Substring(fileindex + 1);


					tilelist.Items.Add("TILESET: Filename: " + fname + " Tilewidth: " + data.tileWidth + 
							" Tileheight: " + data.tileHeight +	" Tilestepx: " + data.tileStepX + 
							" Tilestepy " + data.tileStepY + " Oddrowoffset: " + data.oddRowOffset + 
							" Heighttileoffset " + data.heightTileOffset +
							") | Activation Area: (" + gEvent.ActivationArea.X + " , " + gEvent.ActivationArea.Y +
							") - (" + (int)(gEvent.ActivationArea.X + gEvent.ActivationArea.Width) +
							" , " + (int)(gEvent.ActivationArea.Y + gEvent.ActivationArea.Height) + ")");

					tilesets.Add(new TilesetChangeEvent(gEvent.PathInfo, (uint)data.tileWidth, (uint)data.tileHeight, 
														(uint)data.tileStepX, (uint)data.tileStepY, (uint)data.oddRowOffset, 
														(uint)data.heightTileOffset,
										new ActivationArea((uint)gEvent.ActivationArea.X, (uint)gEvent.ActivationArea.Y,
															(uint)(gEvent.ActivationArea.X + gEvent.ActivationArea.Width),
															(uint)(gEvent.ActivationArea.Y + gEvent.ActivationArea.Height))));

				}
				#endregion

				#region SpriteSheet Changes
				if (gEvent.ActionData is SpriteSheetData)
				{
					tilelist.BeginUpdate();
					var data = ((SpriteSheetData)gEvent.ActionData);

					int fileindex = gEvent.PathInfo.LastIndexOf('\\');
					string fname = gEvent.PathInfo;

					if(fileindex != -1)
						fname = gEvent.PathInfo.Substring(fileindex+1);

					spritelist.Items.Add("SPRITESHEET: Filename: " + fname + " Animations: " + data.Animations.Count +
							") | Activation Area: (" + gEvent.ActivationArea.X + " , " + gEvent.ActivationArea.Y +
							") - (" + (int)(gEvent.ActivationArea.X + gEvent.ActivationArea.Width) +
							" , " + (int)(gEvent.ActivationArea.Y + gEvent.ActivationArea.Height) + ")");

					List<Animations> tempanimate = new List<Animations>();
					foreach(var animation in data.Animations)
					{
						tempanimate.Add(new Animations(animation.Name, (uint)animation.X, (uint)animation.Y, (uint)animation.Width,
														(uint)animation.Height, (uint)animation.Frames, animation.FrameLength, 
														animation.NextAnimation));
					}

					spritesheets.Add(new SpriteSheetChangeEvent(gEvent.PathInfo, tempanimate,
										new ActivationArea((uint)gEvent.ActivationArea.X, (uint)gEvent.ActivationArea.Y,
															(uint)(gEvent.ActivationArea.X + gEvent.ActivationArea.Width),
															(uint)(gEvent.ActivationArea.Y + gEvent.ActivationArea.Height))));


				}

				#endregion
			}
				
			#region Event List
			foreach (var swap in swaplist.Items)
			{
				allEvents.Items.Insert(swindex + swcount, swap);
				swCount++;
				tsindex++;
				spindex++;	
			}
			foreach(var tile in tilelist.Items)
			{
				allEvents.Items.Insert(tsindex+tscount, tile);
				tsCount++;
				spindex++;
			}
			foreach(var sprite in spritelist.Items)
			{
				allEvents.Items.Insert(spindex+spcount, sprite);
				spCount++;
			}
			#endregion
			
		}

		private void SetUpExternPanels()
		{

			#region All Events
			Panel EventPanel = new Panel(){ Name = "Event Panel", Left = 165, Top = 5, Width = 435, Height = 300};
			Label EventLabel = new Label(){ Text = "Events:", Left = 30, Top = 15, Width = 80, Height = 15};

			ListBox EventsList = new ListBox(){ Name = "Events List", Left = 30, Top = 30, Width = 380, Height = 220, HorizontalScrollbar = true};
			EventsList.Items.Add("Swaps: ");
			EventsList.Items.Add(" ");
			EventsList.Items.Add("Tileset Changes: ");
			EventsList.Items.Add(" ");
			EventsList.Items.Add("SpriteSheet Changes: ");
			

			EventPanel.Controls.Add(EventLabel);
			EventPanel.Controls.Add(EventsList);
			#endregion

			#region Swaps
			Panel SwapPanel = new Panel() { Name = "Swap Panel", Top = 5, Width = 435, Height = 300, Left = 165 };
			Label SwapLabel = new Label() { Text = "Swap Map Cell Controls", Left = 30, Top = 15, Width = 80, Height = 15 };
			RadioButton Single = new RadioButton(){Name = "Single", Text = "Individual", Left = 30, Top = 30, Width = 90};
			RadioButton Range = new RadioButton(){Name = "Range", Text = "Range", Left = 120, Top = 30, Width = 60 };
			Single.CheckedChanged += new EventHandler(SwapCheckedChanged);
			Single.Checked = true;
			
			Panel SinglePanel = new Panel(){Name = "Single Panel", Left = 0, Width = 300, Top = 55, Height = 80};
			Label startx = new Label(){Text = "Starting Cell X", Left = 30, Top = 0, Width = 90, Height = 15};
			Label starty = new Label(){Text = "Starting Cell Y", Left = 120, Top = 0, Width = 120, Height = 15};
			TextBox startxtext = new TextBox(){Name = "SStart X", Text = "0", Left = 30, Top = 15, Width = 60};
			TextBox startytext = new TextBox(){Name = "SStart Y", Text = "0", Left = 120, Top = 15, Width = 60}; 
			
			Label endx = new Label(){ Text = "Ending Cell X", Left = 30, Top = 40, Width = 90, Height = 15};
			Label endy = new Label(){ Text = "Ending Cell Y", Left = 120, Top = 40, Width = 90, Height = 15};
			TextBox endxtext = new TextBox(){Name = "SEnd X", Text = "0", Left = 30, Top = 55, Width = 60};
			TextBox endytext = new TextBox() {Name = "SEnd Y", Text = "0", Left = 120, Top = 55, Width = 60 }; 
			SinglePanel.Controls.Add(startx);
			SinglePanel.Controls.Add(starty);
			SinglePanel.Controls.Add(startxtext);
			SinglePanel.Controls.Add(startytext);
			SinglePanel.Controls.Add(endx);
			SinglePanel.Controls.Add(endy);
			SinglePanel.Controls.Add(endxtext);
			SinglePanel.Controls.Add(endytext);


			Panel RangePanel = new Panel() { Name = "Range Panel", Left = 0, Width = 300, Top = 55, Height = 80};
			Label rblabel = new Label() { Text = "Intial Set Starting Coords", Left = 30, Top = 0, Width = 200, Height = 15 };
			Label rbstartx = new Label() { Text = "X1:", Left = 30, Top = 18, Width = 21, Height = 12};
			Label rbstarty = new Label() { Text = "Y1:", Left = 81, Top = 18, Width = 21, Height = 12 };
			Label rbendx = new Label() { Text = "X2:", Left = 132, Top = 18, Width = 21, Height = 12 };
			Label rbendy = new Label() { Text = "Y2:", Left = 183, Top = 18, Width = 21, Height = 12 };
			
			TextBox rbstartxtext = new TextBox() {Name = "RBStart X", Text = "0", Left = 51, Top = 15, Width = 30 };
			TextBox rbstartytext = new TextBox() {Name = "RBStart Y", Text = "0", Left = 102, Top = 15, Width = 30 };
			TextBox rbendxtext = new TextBox() {Name = "RBEnd X", Text = "0", Left = 153, Top = 15, Width = 30 };
			TextBox rbendytext = new TextBox() {Name = "RBEnd Y", Text = "0", Left = 204, Top = 15, Width = 30 };

			Label rflabel = new Label() { Text = "Final Set Starting Coords", Left = 30, Top = 35, Width = 200, Height = 15 };
			Label rfstartx = new Label() { Text = "X1:", Left = 30, Top = 53, Width = 21, Height = 12 };
			Label rfstarty = new Label() { Text = "Y1:", Left = 81, Top = 53, Width = 21, Height = 12 };
			Label rfendx = new Label() { Text = "X2:", Left = 132, Top = 53, Width = 21, Height = 12 };
			Label rfendy = new Label() { Text = "Y2:", Left = 183, Top = 53, Width = 21, Height = 12 };

			TextBox rfstartxtext = new TextBox() {Name = "RFStart X", Text = "0", Left = 51, Top = 50, Width = 30 };
			TextBox rfstartytext = new TextBox() {Name = "RFStart Y", Text = "0", Left = 102, Top = 50, Width = 30 };
			TextBox rfendxtext = new TextBox() {Name = "RFEnd X", Text = "0", Left = 153, Top = 50, Width = 30 };
			TextBox rfendytext = new TextBox() {Name = "RFEnd Y", Text = "0", Left = 204, Top = 50, Width = 30 };

			RangePanel.Controls.Add(rblabel);
			RangePanel.Controls.Add(rbstartx);
			RangePanel.Controls.Add(rbstarty);
			RangePanel.Controls.Add(rbendx);
			RangePanel.Controls.Add(rbendy);
			RangePanel.Controls.Add(rbstartxtext);
			RangePanel.Controls.Add(rbstartytext);
			RangePanel.Controls.Add(rbendxtext);
			RangePanel.Controls.Add(rbendytext);

			RangePanel.Controls.Add(rflabel);
			RangePanel.Controls.Add(rfstartx);
			RangePanel.Controls.Add(rfstarty);
			RangePanel.Controls.Add(rfstartxtext);
			RangePanel.Controls.Add(rfstartytext);
			RangePanel.Controls.Add(rfendx);
			RangePanel.Controls.Add(rfendy);
			RangePanel.Controls.Add(rfendxtext);
			RangePanel.Controls.Add(rfendytext);

			ListBox SwapEvents = new ListBox() { Name = "Swap Events", HorizontalScrollbar = true, 
													Top = 140, Left = 30, Height = 110, Width = 380 };

			SwapPanel.Controls.Add(SwapLabel);
			SwapPanel.Controls.Add(Single);
			SwapPanel.Controls.Add(Range);			
			SwapPanel.Controls.Add(SinglePanel);
			SwapPanel.Controls.Add(RangePanel);
			SwapPanel.Controls.Add(SwapEvents);
			SwapPanel.Hide();
			#endregion

			#region TileSet Changes
			Panel TilePanel = new Panel() { Name = "Tile Panel", Top = 5, Width = 435, Height = 300, Left = 165 };
			Label TileLabel = new Label() { Text = "Change TileSet Controls", Left = 30, Top = 15, Width = 200, Height = 15 };
			Label TileFilePath = new Label() { Text = "Tileset FilePath:", Left = 30, Top = 30, Height = 14 };
			TextBox TilePath = new TextBox() { Name = "Tileset File", Left = 30, Top = 44, Width = 200};
			Button TileFileBrowse = new Button() { Name = "Tile Browse", Text = " Browse", Left = 235, Top = 43, Height = 20};
			TileFileBrowse.Click += new EventHandler(FileBrowse_Click);

			Label tileWidthLabel = new Label(){Text = "Tile Width:", Left = 30, Top = 67, Height = 20, Width = 65};
			Label tileHeightLabel = new Label() { Text = "Tile Height:", Left = 170, Top = 67, Height = 20, Width = 65 };
			Label tileStepXLabel = new Label() { Text = "Tile Step X:", Left = 30, Top = 87, Height = 20, Width = 65 };
			Label tileStepYLabel = new Label() { Text = "Tile Step Y:", Left = 170, Top = 87, Height = 20, Width = 65 };
			Label oddRowOffsetLabel = new Label() { Text = "Odd Row Offset:", Left = 30, Top = 107, Height = 20, Width = 100 };
			Label heightTileOffsetLabel = new Label() { Text = "Height Tile Offset:", Left = 170, Top = 107, Height = 20, Width = 100 };

			TextBox tileWidthText = new TextBox() { Name = "Tile Width", Text = "0", Left = 135, Width = 30, Top = 65, MaxLength = 4};
			TextBox tileHeightText = new TextBox() { Name = "Tile Height", Text = "0", Left = 280, Width = 30, Top = 65, MaxLength = 4 };
			TextBox tileStepxText = new TextBox() { Name = "Tile Step X", Text = "0", Left = 135, Width = 30, Top = 85, MaxLength = 4 };
			TextBox tileStepyText = new TextBox() { Name = "Tile Step Y", Text = "0", Left = 280, Width = 30, Top = 85, MaxLength = 4 };
			TextBox oddRowOffsetText = new TextBox() { Name = "Odd Row Offset", Text = "0", Left = 135, Width = 30, Top = 105, MaxLength = 4 };
			TextBox heightTileOffsetText = new TextBox() { Name = "Height Tile Offset", Text = "0", Left = 280, Width = 30, Top = 105, MaxLength = 4 };

			ListBox TileEvents = new ListBox(){	Name = "Tile Events", HorizontalScrollbar = true, 
													Top = 140, Left = 30, Height = 110,	Width = 380	};

			TilePanel.Controls.Add(TileLabel);
			TilePanel.Controls.Add(TileFilePath);
			TilePanel.Controls.Add(TilePath);
			TilePanel.Controls.Add(TileFileBrowse);
			TilePanel.Controls.Add(tileWidthLabel);
			TilePanel.Controls.Add(tileHeightLabel);
			TilePanel.Controls.Add(tileStepXLabel);
			TilePanel.Controls.Add(tileStepYLabel);
			TilePanel.Controls.Add(oddRowOffsetLabel);
			TilePanel.Controls.Add(heightTileOffsetLabel);
			TilePanel.Controls.Add(TileEvents);
			TilePanel.Controls.Add(tileWidthText);
			TilePanel.Controls.Add(tileHeightText);
			TilePanel.Controls.Add(tileStepxText);
			TilePanel.Controls.Add(tileStepyText);
			TilePanel.Controls.Add(oddRowOffsetText);
			TilePanel.Controls.Add(heightTileOffsetText);
			TilePanel.Hide();
			#endregion

			#region SpriteSheet Changes
			Panel SpritePanel = new Panel() { Name = "Sprite Panel", Top = 5, Width = 435, Height = 300, Left = 165 };
			Label SpriteLabel = new Label() { Text = "Change SpriteSheet Controls", Left = 30, Top = 15, Width = 200, Height = 15 };
			Label SpriteFilePath = new Label() { Text = "SpriteSheet FilePath:", Left = 30, Top = 28, Height = 14, Width = 180 };
			TextBox SpritePath = new TextBox() { Name = "SpriteSheet File", Left = 30, Top = 42, Width = 185};
			Button SpriteFileBrowse = new Button() {Name = "Sprite Browse", Text = " Browse", Left = 220, Top = 41, Width = 65, Height = 20};
			SpriteFileBrowse.Click += new EventHandler(FileBrowse_Click);

			Label SpriteName = new Label() { Text = "Animation Name:", Left = 30, Top = 63, Width = 91, Height = 13 };
			Label SpriteFrames = new Label() { Text = "Frames: ", Left = 205, Top = 63, Width = 50, Height = 13 };
			Label SpriteFrameLength = new Label() { Text = "Length: ", Left = 205, Top = 85, Width = 50, Height = 13 };
			Label SpriteNextAnimation = new Label() { Text = "Next Animation: ", Left = 30, Top = 83, Width = 85, Height = 13 };
			Label SpriteX = new Label() { Text = "X Loc: ", Left = 30, Top = 103, Width = 50, Height = 13 };
			Label SpriteY = new Label() { Text = "Y Loc: ", Left = 140, Top = 103, Width = 50, Height = 13 };
			Label SpriteWidth = new Label() { Text = "Width: ", Left = 30, Top = 123, Width = 50, Height = 13 };
			Label SpriteHeight = new Label() { Text = "Height: ", Left = 140, Top = 123, Width = 50, Height = 13 };

			TextBox SpriteNameText = new TextBox() { Name = "Animation Name", Left = 121, Top = 62, Width = 69};
			TextBox SpriteFramesText = new TextBox() { Name = "Sprite Frames", Text = "0", Left = 255, Top = 62, Width = 30 };
			TextBox SpriteFrameLengthText = new TextBox() { Name = "Sprite Frame Length", Text = "0", Left = 255, Top = 82, Width = 30 };
			TextBox SpriteNextAnimationText = new TextBox() { Name = "Sprite Next Animation", Left = 121, Top = 82, Width = 69 };
			TextBox SpriteXText = new TextBox() { Name = "Sprite X", Text = "0", Left = 80, Top = 100, Width = 30 };
			TextBox SpriteYText = new TextBox() { Name = "Sprite Y", Text = "0", Left = 190, Top = 100, Width = 30 };
			TextBox SpriteWidthText = new TextBox() { Name = "Sprite Width", Text = "0", Left = 80, Top = 120, Width = 30, Height = 10 };
			TextBox SpriteHeightText = new TextBox() { Name = "Sprite Height", Text = "0", Left = 190, Top = 120, Width = 30 };
			
			ListBox SpriteEvents = new ListBox(){ Name = "Sprite Events", HorizontalScrollbar = true, Top = 140,
												Left = 30, Height = 110, Width = 380 };

			ListBox AnimationData = new ListBox() { Name = "Animation Data", HorizontalScrollbar = true, Top = 41,
												Left = 290, Height = 107, Width = 120};

			Button spAdd = new Button() { Name = "Add Animation", Text = "Add", Left = 225, Top = 102, Width = 60, Height = 18 };
			spAdd.Click += new EventHandler(spAdd_Click);

			Button spRem = new Button() { Name = "Remove Animation", Text = "Remove", Left = 225, Top = 120, Width = 60, Height = 18 };
			spRem.Click += new EventHandler(spRem_Click);

			SpritePanel.Controls.Add(SpriteLabel);
			SpritePanel.Controls.Add(SpriteFilePath);
			SpritePanel.Controls.Add(SpritePath);
			SpritePanel.Controls.Add(SpriteFileBrowse);
			SpritePanel.Controls.Add(SpriteName);
			SpritePanel.Controls.Add(SpriteX);
			SpritePanel.Controls.Add(SpriteY);
			SpritePanel.Controls.Add(SpriteWidth);
			SpritePanel.Controls.Add(SpriteHeight);
			SpritePanel.Controls.Add(SpriteFrames);
			SpritePanel.Controls.Add(SpriteFrameLength);
			SpritePanel.Controls.Add(SpriteNextAnimation);
			SpritePanel.Controls.Add(SpriteNameText);
			SpritePanel.Controls.Add(SpriteXText);
			SpritePanel.Controls.Add(SpriteYText);
			SpritePanel.Controls.Add(SpriteWidthText);
			SpritePanel.Controls.Add(SpriteHeightText);
			SpritePanel.Controls.Add(SpriteFramesText);
			SpritePanel.Controls.Add(SpriteFrameLengthText);
			SpritePanel.Controls.Add(SpriteNextAnimationText);
			SpritePanel.Controls.Add(SpriteEvents);
			SpritePanel.Controls.Add(AnimationData);
			SpritePanel.Controls.Add(spAdd);
			SpritePanel.Controls.Add(spRem);

			#endregion

			this.Controls.Add(EventPanel);
			this.Controls.Add(SpritePanel);
			this.Controls.Add(SwapPanel);
			this.Controls.Add(TilePanel);
		}


		private void FuncComboChange(object sender, EventArgs e)
		{
			var eventpan = (Panel)this.Controls.Find("Event Panel",false).ElementAt(0);
			var swappan = (Panel)this.Controls.Find("Swap Panel", false).ElementAt(0);
			var tilepan = (Panel)this.Controls.Find("Tile Panel", false).ElementAt(0);
			var spritepan = (Panel)this.Controls.Find("Sprite Panel", false).ElementAt(0);
			eventpan.Hide();
			swappan.Hide();
			tilepan.Hide();
			spritepan.Hide();

			Button add = (Button)this.Controls.Find("Add", false).ElementAt(0);
			Button remove = (Button)this.Controls.Find("Remove",false).ElementAt(0);
	
			if(((ComboBox)sender).SelectedIndex == 0)
			{
				add.Visible = false;
				remove.Visible = true;
				eventpan.Show();
			}
			else
			{
				add.Visible = true;
				remove.Visible = true;
				if(((ComboBox)sender).SelectedIndex == 1)
				{
					swappan.Show();
				}
				else if(((ComboBox)sender).SelectedIndex == 2)
				{
					tilepan.Show();
				}
				else if(((ComboBox)sender).SelectedIndex == 3)
				{
					spritepan.Show();
				}
			}
		}

		private void SwapCheckedChanged(object sender, EventArgs e)
		{
			if (((RadioButton)sender).Checked)
			{
				foreach (var control in this.Controls.Find("Single Panel", true))
					control.Show();
				foreach (var control in this.Controls.Find("Range Panel", true))
					control.Hide();
			}
			else
			{
				foreach (var control in this.Controls.Find("Single Panel", true))
					control.Hide();
				foreach (var control in this.Controls.Find("Range Panel", true))
					control.Show();
			}
		}


		private bool CheckSwapInput(Panel sender)
		{
			bool success = true, range = true;
			string errstr = "";

			if (((RadioButton)sender.Controls.Find("Single", false).ElementAt(0)).Checked)
			{
				#region Single Data
				if (success)
				{
					try { swbstartx = uint.Parse(((TextBox)sender.Controls.Find("SStart X", true).ElementAt(0)).Text); }
					catch { success = false; errstr = "SwapData: Starting Cell X"; }
				}
				if (success)
				{
					try { swbstarty = uint.Parse(((TextBox)sender.Controls.Find("SStart Y", true).ElementAt(0)).Text); }
					catch { success = false; errstr = "SwapData: Starting Cell Y"; }
				}
				if (success)
				{
					try { swbendx = uint.Parse(((TextBox)sender.Controls.Find("SEnd X", true).ElementAt(0)).Text); }
					catch { success = false; errstr = "SwapData: Ending Cell X"; }
				}
				if (success)
				{
					try { swbendy = uint.Parse(((TextBox)sender.Controls.Find("SEnd Y", true).ElementAt(0)).Text); }
					catch { success = false; errstr = "SwapData: Ending Cell Y"; }
				}
				#endregion
			}
			else
			{
				#region Range Data
				if (success)
				{
					try { swbstartx = uint.Parse(((TextBox)sender.Controls.Find("RBStart X", true).ElementAt(0)).Text); }
					catch { success = false; errstr = "SwapData: Initial Coordinates - X1"; }
				}
				if (success)
				{
					try { swbstarty = uint.Parse(((TextBox)sender.Controls.Find("RBStart Y", true).ElementAt(0)).Text); }
					catch { success = false; errstr = "SwapData: Initial Coordinates - Y1"; }
				}
				if (success)
				{
					try { swbendx = uint.Parse(((TextBox)sender.Controls.Find("RBEnd X", true).ElementAt(0)).Text); }
					catch { success = false; errstr = "SwapData: Initial Coordinates - X2"; }
				}
				if (success)
				{
					try { swbendy = uint.Parse(((TextBox)sender.Controls.Find("RBEnd Y", true).ElementAt(0)).Text); }
					catch { success = false; errstr = "SwapData: Initial Coordinates - Y2"; }
				}
				if (success)
				{
					try { swfstartx = uint.Parse(((TextBox)sender.Controls.Find("RFStart X", true).ElementAt(0)).Text); }
					catch { success = false; errstr = "SwapData: Final Coordinates - X1"; }
				}
				if (success)
				{
					try { swfstarty = uint.Parse(((TextBox)sender.Controls.Find("RFStart Y", true).ElementAt(0)).Text); }
					catch { success = false; errstr = "SwapData: Final Coordinates - Y1"; }
				}
				if (success)
				{
					try { swfendx = uint.Parse(((TextBox)sender.Controls.Find("RFEnd X", true).ElementAt(0)).Text); }
					catch { success = false; errstr = "SwapData: Final Coordinates - X2"; }
				}
				if (success)
				{
					try { swfendy = uint.Parse(((TextBox)sender.Controls.Find("RFEnd Y", true).ElementAt(0)).Text); }
					catch { success = false; errstr = "SwapData: Final Coordinates - Y2"; }
				}

				if (success)
				{
					int tempsbx = (int)swbstartx, tempsby = (int)swbstarty, tempebx = (int)swbendx, tempeby = (int)swbendy;
					int tempsfx = (int)swfstartx, tempsfy = (int)swfstarty, tempefx = (int)swfendx, tempefy = (int)swfendy;
					Console.WriteLine(tempsbx);
					if ((Math.Abs(tempsbx - tempebx) != (Math.Abs(tempsfx - tempefx)) || (Math.Abs(tempsby - tempeby)) != Math.Abs(tempsfy - tempefy)))
						range = false;
				}
				#endregion
			}
			showbox = true;
			if (!success)
				MessageBox.Show("Invalid Data in " + errstr);
			else if (!range)
				MessageBox.Show("Error, Swap Ranges Dimensions Don't Match!");
			showbox = false;

			return success && range;
		}

		private bool CheckTileInput(Panel sender)
		{
			bool success = true;
			string errstr = "";
			if (success)
			{
				tsfilepath = ((TextBox)sender.Controls.Find("Tileset File", true).ElementAt(0)).Text;
				if (tsfilepath == "")
				{
					success = false;
					errstr = "Tileset File Name: Missing File Path";
				}
				else
				{
					try
					{
						System.IO.StreamReader sr = new System.IO.StreamReader(tsfilepath);
						sr.Close();
					}
					catch
					{
						errstr = "Tileset File Name: Error 404 File not Found";
					}
				}
			}

			if (success)
			{
				try { tstilewidth = uint.Parse(((TextBox)sender.Controls.Find("Tile Width", true).ElementAt(0)).Text); }
				catch { success = false; errstr = "Tileset Data: Tile Width"; }
			}
			if (success)
			{
				try { tstileheight = uint.Parse(((TextBox)sender.Controls.Find("Tile Height", true).ElementAt(0)).Text); }
				catch { success = false; errstr = "Tileset Data: Tile Height"; }
			}
			if (success)
			{
				try { tstilestepx = uint.Parse(((TextBox)sender.Controls.Find("Tile Step X", true).ElementAt(0)).Text); }
				catch { success = false; errstr = "Tileset Data: Tile Step X"; }
			}
			if (success)
			{
				try { tstilestepy = uint.Parse(((TextBox)sender.Controls.Find("Tile Step Y", true).ElementAt(0)).Text); }
				catch { success = false; errstr = "Tileset Data: Tile Step Y"; }
			}
			if (success)
			{
				try { tsoddrowoffset = uint.Parse(((TextBox)sender.Controls.Find("Odd Row Offset", true).ElementAt(0)).Text); }
				catch { success = false; errstr = "Tileset Data: Odd Row Offset"; }
			}
			if (success)
			{
				try { tsheighttileoffset = uint.Parse(((TextBox)sender.Controls.Find("Height Tile Offset", true).ElementAt(0)).Text); }
				catch { success = false; errstr = "Tileset Data: Height Tile Offset"; }
			}
			if (!success)
			{
				showbox = true;
				MessageBox.Show("Invalid data in " + errstr);
				showbox = false;
			}
			return success;
		}

		private bool CheckSpriteSheetInput(Panel sender)
		{
			bool success = true;
			string errstr = "";
			if (success)
			{
				spfilepath = ((TextBox)sender.Controls.Find("SpriteSheet File", true).ElementAt(0)).Text;
				if (spfilepath == "")
				{
					success = false;
					errstr = "SpriteSheet File Name: Missing File Path";
				}
				else
				{
					try
					{
						System.IO.StreamReader sr = new System.IO.StreamReader(spfilepath);
						sr.Close();
					}
					catch
					{
						errstr = "SpriteSheet File Name: Error 404 File not Found";
					}
				}
			}
			if (!success)
			{
				showbox = true;
				MessageBox.Show("Invalid Data found in " + errstr);
				showbox = false;
			}
			return success;
		}

		private bool CheckAnimationData(Panel sender)
		{
			bool success = true;
			string errstr = "";
			if (success)
			{
				spName = ((TextBox)this.Controls.Find("Animation Name", true).ElementAt(0)).Text;
				if (spName == "")
				{
					success = false;
					errstr = "SpriteSheet Data: Missing Animation Name";
				}
			}
			if (success)
			{
				spNextAnimation = ((TextBox)this.Controls.Find("Sprite Next Animation", true).ElementAt(0)).Text;
			}
			if (success)
			{
				try { spFrames = uint.Parse(((TextBox)sender.Controls.Find("Sprite Frames", true).ElementAt(0)).Text); }
				catch { success = false; errstr = "SpriteSheet Data: Frames"; }
			}
			if (success)
			{
				try { spFrameLength = float.Parse(((TextBox)sender.Controls.Find("Sprite Frame Length", true).ElementAt(0)).Text); }
				catch { success = false; errstr = "SpriteSheet Data: Frame Length"; }
			}
			if (success)
			{
				try { spX = uint.Parse(((TextBox)sender.Controls.Find("Sprite X", true).ElementAt(0)).Text); }
				catch { success = false; errstr = "SpriteSheet Data: X Location"; }
			}
			if (success)
			{
				try { spY = uint.Parse(((TextBox)sender.Controls.Find("Sprite Y", true).ElementAt(0)).Text); }
				catch { success = false; errstr = "SpriteSheet Data: Y Location"; }
			}
			if (success)
			{
				try { spWidth = uint.Parse(((TextBox)sender.Controls.Find("Sprite Width", true).ElementAt(0)).Text); }
				catch { success = false; errstr = "SpriteSheet Data: Sprite Width"; }
			}
			if (success)
			{
				try { spHeight = uint.Parse(((TextBox)sender.Controls.Find("Sprite Height", true).ElementAt(0)).Text); }
				catch { success = false; errstr = "SpriteSheet Data: Sprite Height"; }
			}
			if (!success)
			{	
				showbox = true;
				MessageBox.Show("Invalid data in " + errstr);
				showbox = false;
			}
			return success;
		}
	
		private bool CheckActivationArea()
		{
			string errstr = "";
			bool success = true;

			try{ astartx = uint.Parse(astartxtext.Text);}
			catch{ success = false; errstr = "Activation Area: Start X";}

			if(success)
			{
				try{ astarty = uint.Parse(astartytext.Text); }
				catch{ success = false; errstr = "Activation Area: Start Y"; } 
			}

			if(success)			
			{
				try{ aendx = uint.Parse(aendxtext.Text); }
				catch{ success = false; errstr = "Activation Area: End X"; }
			}

			if(success)
			{
				try{ aendy = uint.Parse(aendytext.Text); }
				catch{ success = false; errstr = "Activation Area End Y"; }
			}		
			
			if(!success)
			{
				showbox = true;
				MessageBox.Show("Invalid Data found in " + errstr);
				showbox = false;
			}
			return success;
		}

		private void FileBrowse_Click(object sender, EventArgs e)
		{
			String filepath = "";
			OpenFileDialog openFileDialog1 = new OpenFileDialog(); // File dialog box
			openFileDialog1.InitialDirectory = openFileDialog1.InitialDirectory = @"./"; // Set directory
			openFileDialog1.Filter = "png files (*.png)|*.png|jpg files (*.jpg)|*.jpg|All files (*.*)|*.*"; // Determine expected filetypes
			openFileDialog1.FilterIndex = 1; // Set file dialog filter
			openFileDialog1.RestoreDirectory = true; // Restore the directory 
			showbox = true;
			if(openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				try
				{
					if(openFileDialog1.OpenFile() != null)
						filepath = openFileDialog1.FileName;
				}
				catch
				{
					showbox = true;
					MessageBox.Show("Error: Failed to open file");
					showbox = false;
				}
			}
			showbox = false;

			if(((Button)sender).Name == "Tile Browse")
				((TextBox)this.Controls.Find("Tileset File", true).ElementAt(0)).Text = filepath;
			else
				((TextBox)this.Controls.Find("SpriteSheet File", true).ElementAt(0)).Text = filepath;
		}
		
		private void addButton_Click(object sender, EventArgs e)
		{
			ListBox allEvents = (ListBox)this.Controls.Find("Events List", true).ElementAt(0);
			switch (FuncCombo.SelectedIndex)
			{
				case 0:
					break;

				#region CellSwaps
				case 1: // Cell swaps

					Panel swapPanel = (Panel)this.Controls.Find("Swap Panel", true).ElementAt(0);

					if (CheckSwapInput(swapPanel) && CheckActivationArea())
					{
						ListBox Eventlist = (ListBox)swapPanel.Controls.Find("Swap Events", false).ElementAt(0);

						RadioButton rBut = (RadioButton)swapPanel.Controls.Find("Single", false).ElementAt(0);
						Eventlist.BeginUpdate();

						if (rBut.Checked)
						{
							Eventlist.Items.Add("CELL SWAP: (" + swbstartx + " , " + swbstarty +
													") <-> (" + swbendx + " , " + swbendy +
													") | Activation Area: (" + astartx + " , " + astarty +
													") - (" + aendx + " , " + aendy + ")");

							swaps.Add(new CellSwapEvent(swbstartx, swbstarty, swbendx, swbendy, -1, -1, -1, -1,
											 new ActivationArea(astartx, astarty, aendx, aendy))); 

						}
						else
						{
							Eventlist.Items.Add("RANGE SWAP: Initial: (" + swbstartx + " , " + swbstarty +
													") - (" + swbendx + " , " + swbendy +
													") <-> Final: (" + swfstartx + " , " + swfstarty +
													") - (" + swfendx + " , " + swfendy +
													") | Activation Area: (" + astartx + " , " + astarty +
													") - (" + aendx + " , " + aendy + ")");

							swaps.Add(new CellSwapEvent(swbstartx, swbstarty, swbendx, swbendy, (int)swfstartx, (int)swfstarty, (int)swfendx, (int)swfendy,
										new ActivationArea(astartx, astarty, aendx, aendy))); 
						}
						Eventlist.EndUpdate();
					
						allEvents.BeginUpdate();
						
						allEvents.Items.Insert(swindex + swCount, Eventlist.Items[Eventlist.Items.Count - 1]);
						swCount++;
						tsindex++;
						spindex++;

						allEvents.EndUpdate();
					}
					break;
				#endregion

				#region Tileset Changes
				case 2: // Tileset Changes
					Panel tilePanel = (Panel)this.Controls.Find("Tile Panel", false).ElementAt(0);

					if (CheckTileInput(tilePanel) && CheckActivationArea())
					{
						string filename = tsfilepath.Substring(tsfilepath.LastIndexOf("\\") + 1);
						ListBox Eventlist = (ListBox)this.Controls.Find("Tile Events", true).ElementAt(0);
						Eventlist.BeginUpdate();
						Eventlist.Items.Add("TILESET: Filename: " + filename + " Tilewidth: " + tstilewidth + " Tileheight: " + tstileheight +
												" Tilestepx: " + tstilestepx + " Tilestepy " + tstilestepy +
												" Oddrowoffset: " + tsoddrowoffset + " Heighttileoffset " + tsheighttileoffset +
												" | Activation Area: (" + astartx + " , " + astarty +
												") - (" + aendx + " , " + aendy + ")");

						tilesets.Add(new TilesetChangeEvent(tsfilepath, tstilewidth, tstileheight, tstilestepx, tstilestepy, tsoddrowoffset, tsheighttileoffset,		
										new ActivationArea(astartx, astarty, aendx, aendy)));

						Eventlist.EndUpdate();

						allEvents.BeginUpdate();
						allEvents.Items.Insert(tsindex + tsCount, Eventlist.Items[Eventlist.Items.Count - 1]);
						tsCount++;
						spindex++;

						allEvents.EndUpdate();
					}
					break;
				#endregion

				#region SpriteSheet Changes
				case 3: // SpriteSheet Changes
					Panel spritePanel = (Panel)this.Controls.Find("Sprite Panel", false).ElementAt(0);

					if (CheckSpriteSheetInput(spritePanel) && CheckActivationArea())
					{
						string filename = spfilepath.Substring(spfilepath.LastIndexOf("\\") + 1);
						ListBox Eventlist = (ListBox)this.Controls.Find("Sprite Events", true).ElementAt(0);
						ListBox AnimationData = (ListBox)this.Controls.Find("Animation Data", true).ElementAt(0);
						Eventlist.BeginUpdate();
						Eventlist.Items.Add("SPRITESHEET: Filename: " + filename + " Animations: " + AnimationData.Items.Count +
												" | Activation Area: (" + astartx + " , " + astarty +
												") - (" + aendx + " , " + aendy + ")");
						spritesheets.Add(new SpriteSheetChangeEvent(spfilepath, animate, new ActivationArea(astartx, astarty, aendx, aendy)));

						animate.Clear();
						AnimationData.BeginUpdate();
						AnimationData.Items.Clear();
						AnimationData.EndUpdate();
						Eventlist.EndUpdate();

						allEvents.BeginUpdate();
						allEvents.Items.Insert(spindex + spCount, Eventlist.Items[Eventlist.Items.Count - 1]);
						spCount++;
						allEvents.EndUpdate();
					}
					break;
				#endregion
			}
		}

		private void removeButton_Click(object sender, EventArgs e)
		{
			ListBox allEvents = (ListBox)this.Controls.Find("Events List",true).ElementAt(0);
			ListBox swapEvents = (ListBox)this.Controls.Find("Swap Events", true).ElementAt(0);
			ListBox tileEvents = (ListBox)this.Controls.Find("Tile Events", true).ElementAt(0);
			ListBox spriteEvents = (ListBox)this.Controls.Find("Sprite Events", true).ElementAt(0);

			int selected;
			switch (FuncCombo.SelectedIndex)
			{
				case 0:
					selected = allEvents.SelectedIndex;
					if(selected != -1)
					{
						if(selected > swindex - 1 && selected != tsindex - 2 && selected != tsindex -1 && selected != spindex-2 && selected != spindex - 1)
						{
							int newSelected = selected;
							if(selected < tsindex-1)
							{
								swapEvents.BeginUpdate();
								swapEvents.Items.RemoveAt(selected - swindex);
								swapEvents.EndUpdate();
								swaps.RemoveAt(selected - swindex);
								swCount--;
								tsindex--;
								spindex--;
								newSelected = (selected - 1 < swindex) ? swindex : selected - 1;  
							}
							else if(selected < spindex-1)
							{
								tileEvents.BeginUpdate();
								tileEvents.Items.RemoveAt(selected - tsindex);
								tileEvents.EndUpdate();
								tilesets.RemoveAt(selected - tsindex);
								tsCount--;
								spindex--;
								newSelected = (selected - 1 < tsindex) ? tsindex : selected - 1;  
							}
							else
							{
								spriteEvents.BeginUpdate();
								spriteEvents.Items.RemoveAt(selected - spindex);
								spriteEvents.EndUpdate();
								spritesheets.RemoveAt(selected - spindex);
								spCount--;
								newSelected = (selected - 1 < spindex) ? spindex : selected - 1;
								if(spindex == allEvents.Items.Count-1)
								{
									newSelected = spindex -1;
								}
							}

							allEvents.BeginUpdate();
							allEvents.Items.RemoveAt(selected);
							allEvents.SelectedIndex = newSelected;
							allEvents.EndUpdate();
							
						}
					}
					break;

				case 1: // Cell swaps
				
					selected = swapEvents.SelectedIndex;
					if (selected != -1)
					{
						swapEvents.BeginUpdate();
						swapEvents.Items.RemoveAt(selected);
						swapEvents.SelectedIndex = (selected < swapEvents.Items.Count - 1 ? selected : swapEvents.Items.Count - 1);
						swaps.RemoveAt(selected);
						swapEvents.EndUpdate();

						allEvents.BeginUpdate();
						allEvents.Items.RemoveAt(selected + swindex);
						swCount--;
						tsindex--;
						spindex--;
						allEvents.EndUpdate();
					}
					break;

				case 2: // Tileset Changes
					
					selected = tileEvents.SelectedIndex;
					if (selected != -1)
					{
						tileEvents.BeginUpdate();
						tileEvents.Items.RemoveAt(selected);
						tileEvents.SelectedIndex = (selected < tileEvents.Items.Count - 1 ? selected : tileEvents.Items.Count - 1);
						tilesets.RemoveAt(selected);
						tileEvents.EndUpdate();
					
						allEvents.BeginUpdate();
						allEvents.Items.RemoveAt(selected + tsindex);
						tsCount--;
						spindex--;
						allEvents.EndUpdate();

					}

					break;

				case 3: // SpriteSheet Changes
					
					selected = spriteEvents.SelectedIndex;
					if (selected != -1)
					{
						spriteEvents.BeginUpdate();
						spriteEvents.Items.RemoveAt(selected);
						spriteEvents.SelectedIndex = (selected < spriteEvents.Items.Count - 1 ? selected : spriteEvents.Items.Count - 1);
						spritesheets.RemoveAt(selected);
						spriteEvents.EndUpdate();

						allEvents.BeginUpdate();
						allEvents.Items.RemoveAt(selected + spindex);
						spCount--;
						allEvents.EndUpdate();
					}

					break;
			}
		}

		private void spAdd_Click(object sender, EventArgs e)
		{
			Panel SpritePanel = (Panel)this.Controls.Find("Sprite Panel", true).ElementAt(0);
			if( CheckAnimationData(SpritePanel))
			{
				ListBox AnimationData = (ListBox)this.Controls.Find("Animation Data", true).ElementAt(0);
				AnimationData.BeginUpdate();
				AnimationData.Items.Add("ANIMATION DATA: Animation Name: " + spName + " Number of Frames: " + spFrames +
											" Frame Length: " + spFrameLength + " Next Animation: " + spNextAnimation +
											" X: " + spX + " Y: " + spY + " Width: " + spWidth + " Height: " + spHeight );

				animate.Add(new Animations(spName, spX, spY, spWidth, spHeight, spFrames, spFrameLength, spNextAnimation));
				Console.WriteLine(animate.Count);
				AnimationData.EndUpdate();
			}
		}

		private void spRem_Click(object sender, EventArgs e)
		{
			int selected;
			ListBox AnimationData = (ListBox)this.Controls.Find("Animation Data", true).ElementAt(0);
			AnimationData.BeginUpdate();

			selected = AnimationData.SelectedIndex;
			if (selected != -1)
			{
				AnimationData.Items.RemoveAt(selected);
				AnimationData.SelectedIndex = (selected < AnimationData.Items.Count - 1 ? selected : AnimationData.Items.Count - 1);
				animate.RemoveAt(selected);
				Console.WriteLine("Out at " + selected);
			}

			AnimationData.EndUpdate();
		}

		private void cancelBut_Click(object sender, EventArgs e)
		{
			if (!showbox)
			{
				ok = false;
				this.Close();
			}
		}

		private void confirmBut_Click(object sender, EventArgs e)
		{
			ok = true;
			this.Close();
		}
	}
}
