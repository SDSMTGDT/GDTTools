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
	public partial class Settings : Form
	{
		private PictureBox picBox1;
		private ComboBox slopeCombo;
		private ComboBox walkableCombo;
		private ComboBox typeCombo;
		private TextBox MapWidth;
		private TextBox MapHeight; 
		private Button confirmation;
		
		public int SMapWidth{ get; set; }
		public int SMapHeight{ get; set; }
		public int slope{ get; set;}
		public int walkable { get; set; }
		public int type { get; set; }
		public uint tileId;
		public List<int> layers;
		public int maxTile;

		public bool ok;
		private bool showbox;
		public string tilefile;
		public int tileWid;
		public int tileHei;

		
		public Settings(int width, int height, int mTile, List<int> lay, string tilesetFile, int tWid, int tHei)
		{
			tilefile = tilesetFile;
			tileWid = tWid;
			tileHei = tHei;
			layers = new List<int>(lay);
			SMapWidth = width;
			SMapHeight = height;
			maxTile = mTile;
			InitializeComponent();
		}

		private void Settings_Load(object sender, EventArgs e)
		{
			ok = true;
			showbox = false;
			#region TilePanel
			Panel Tilepanel = new Panel() { Width = 300, Height = 150};
			Label Tilesettings = new Label() { Text = "Tile Settings", Left = 50, Top = 10 };
			Label walklabel = new Label() { Text = "Walkable", Left = 50, Top = 10 + 25, Height = 15};
			
			walkableCombo = new ComboBox() { Left = 50, Top = 10 + 45 };
			walkableCombo.Items.Add("Yes");
			walkableCombo.Items.Add("No");
			walkableCombo.SelectedIndex = 0;
			walkableCombo.DropDownStyle = ComboBoxStyle.DropDownList;

			Label typelabel = new Label() { Text = "Force Tile Type", Left = 50, Top = 10 + 80, Height = 15};
			typeCombo = new ComboBox() { Left = 50, Top = 10 + 100 };
			typeCombo.Items.Add("Auto");
			typeCombo.Items.Add("Base Tiles");
			typeCombo.Items.Add("Height Tiles");
			typeCombo.Items.Add("Topper Tiles");
			typeCombo.Items.Add("Scenery Tiles");
			typeCombo.SelectedIndex = 0;
			typeCombo.DropDownStyle = ComboBoxStyle.DropDownList;

			Tilepanel.Controls.Add(Tilesettings);
			Tilepanel.Controls.Add(walklabel);
			Tilepanel.Controls.Add(walkableCombo);
			Tilepanel.Controls.Add(typelabel);
			Tilepanel.Controls.Add(typeCombo);

			Panel PicturePanel = new Panel() { Width = 200, Height = 200, Top = 150};
			Label PictureLabel = new Label() {Text = "Slope Map Settings", Left = 50, Top = 10, Height = 15};
			slopeCombo = new ComboBox() { Left = 50, Top = 10 + 20 };
			slope = 0;
			slopeCombo.Items.Add("None");
			slopeCombo.Items.Add("Map 1");
			slopeCombo.Items.Add("Map 2");
			slopeCombo.Items.Add("Map 3");
			slopeCombo.Items.Add("Map 4");
			slopeCombo.Items.Add("Map 5");
			slopeCombo.Items.Add("Map 6");
			slopeCombo.Items.Add("Map 7");
			slopeCombo.Items.Add("Map 8");
			slopeCombo.SelectedIndex = 0;
			slopeCombo.DropDownStyle = ComboBoxStyle.DropDownList;

			Image slopemaps = Image.FromFile(@"Content\Images\default_slopemaps.png");
			
			picBox1 = new PictureBox(){Width = slopemaps.Width / 8, Height = slopemaps.Height, 
													Left = 75, Top = 10 + 55};

			picBox1.Paint += new System.Windows.Forms.PaintEventHandler(DrawImageRect);

			slopeCombo.SelectedIndexChanged += new EventHandler(slopeselect_SelectedIndexChanged);
			
			PicturePanel.Controls.Add(PictureLabel);
			PicturePanel.Controls.Add(slopeCombo);
			PicturePanel.Controls.Add(picBox1);

			#endregion

			#region TilesetPanel
			Panel TilesetPanel = new Panel(){ Left = 200, Width = 300, Height = 200 };
			Label Tileset = new Label(){ Text = "Tileset Settings", Top = 10, Left = 30, Height = 15};
			Label CurTileset = new Label(){ Text = "Current Tileset", Left = 30, Top = 35, Height = 15 };
			TextBox TilesetText = new TextBox(){ Name = "Current Tileset", Text = tilefile.Substring(tilefile.LastIndexOf('\\') + 1), Left = 30, Top = 55, Width = 150, ReadOnly = true };
			Label TileWidth = new Label(){ Text = "Tile Width", Left = 30, Top = 75, Height = 15, Width = 70 };
			TextBox TileWidthText = new TextBox(){ Name = "Tile Width", Text = tileWid.ToString(), Left = 30, Top = 90, Width = 40 };
			Label TileHeight = new Label(){ Text = "Tile Height", Left = 120, Top = 75, Height = 15 };
			TextBox TileHeightText = new TextBox(){ Name = "Tile Height", Text = tileHei.ToString(), Left = 120, Top = 90, Width = 40 };

			Button browseBut = new Button(){Text = "Browse", Left = 185, Top = 52 };
			browseBut.Click += browseBut_Click;

			TilesetPanel.Controls.Add(Tileset);
			TilesetPanel.Controls.Add(CurTileset);
			TilesetPanel.Controls.Add(TilesetText);
			TilesetPanel.Controls.Add(TileWidth);
			TilesetPanel.Controls.Add(TileWidthText);
			TilesetPanel.Controls.Add(TileHeight);
			TilesetPanel.Controls.Add(TileHeightText);
			TilesetPanel.Controls.Add(browseBut);

			#endregion

			#region MapPanel
			Panel MapPanel = new Panel(){ Top = 120, Width = 300, Height = 200, Left = 200};
			Label MapLabel = new Label(){Text = " Map Settings", Top = 10, Left = 30};
			Label WidthLabel = new Label(){Text = " Map Width ", Top = 35, Left = 30, Height = 15};
			Label HeightLabel = new Label(){Text = " Map Height ", Top = 35, Left = 150, Height = 15};
			MapWidth = new TextBox(){Text = SMapWidth + "", Top = 55, Left = 30};
			MapHeight = new TextBox(){Text = SMapHeight + "", Top = 55, Left = 150};

			Label MapLayers = new Label(){Text = "Add Layers to the Map: ", Top = 75, Left = 30, Width = 130, Height = 15};
			Label TileId = new Label(){ Text = "Layer Tile Id:", Top = 98, Left = 30, Width = 90, Height = 15};
			TextBox TileIdText = new TextBox(){ Name = "TileId", Top = 95, Left = 120, Width = 30};

			Button addLayer = new Button(){Name = "Add Layer", Text = "Add", Top = 118, Left = 30};
			addLayer.Click += new EventHandler(addLayer_Click);

			Button removeLayer = new Button(){Name = "Remove Layer", Text = "Remove", Top = 118, Left = 105};
			removeLayer.Click += new EventHandler(removeLayer_Click);

			Button removeAll = new Button(){Text = "Remove All", Top = 118, Left = 180};
			removeAll.Click += new EventHandler(removeAll_Click);

			ListBox layers = new ListBox(){Name = "Layers", Top = 145, Left = 30, Width = 230, Height = 90};

			MapPanel.Controls.Add(MapLabel);
			MapPanel.Controls.Add(WidthLabel);
			MapPanel.Controls.Add(HeightLabel);
			MapPanel.Controls.Add(MapWidth);
			MapPanel.Controls.Add(MapHeight);
			MapPanel.Controls.Add(MapLayers);
			MapPanel.Controls.Add(TileId);
			MapPanel.Controls.Add(TileIdText);
			MapPanel.Controls.Add(addLayer);
			MapPanel.Controls.Add(removeLayer);
			MapPanel.Controls.Add(removeAll);
			MapPanel.Controls.Add(layers);

			
			#endregion 

			confirmation = new Button() { Text = "Ok", Left = 30, Top = 295 };
			confirmation.Click += confirmation_Click;

			Button cancelButton = new Button(){ Text = "Cancel", Left = 105, Top = 295};
			cancelButton.Click += cancelBut_Click;

			this.Location = new Point(50, 80);

			this.Controls.Add(confirmation);
			this.Controls.Add(cancelButton);
			this.Controls.Add(MapPanel);
			this.Controls.Add(TilesetPanel);
			this.Controls.Add(Tilepanel);
			this.Controls.Add(PicturePanel);

			this.AcceptButton = confirmation;
			this.CancelButton = cancelButton;
			this.Deactivate += new EventHandler(cancelBut_Click);
			InitializeLayers();
		}

		void browseBut_Click(object sender, EventArgs e)
		{
			String filepath = "";
			
			OpenFileDialog openFileDialog2 = new OpenFileDialog(); // File dialog box
			openFileDialog2.InitialDirectory = openFileDialog2.InitialDirectory = @"./"; // Set directory
			openFileDialog2.Filter = "png files (*.png)|*.png|jpg files (*.jpg)|*.jpg|All files (*.*)|*.*"; // Determine expected filetypes
			openFileDialog2.FilterIndex = 1; // Set file dialog filter
			openFileDialog2.RestoreDirectory = true; // Restore the directory 	
			showbox = true;		
			if (openFileDialog2.ShowDialog() == DialogResult.OK)
			{
				try
				{
					if (openFileDialog2.OpenFile() != null)
					{
						filepath = openFileDialog2.FileName;
					}
				}
				catch
				{
					MessageBox.Show("Error: Failed to open file");
				}

			}
			showbox = false;

			((TextBox)this.Controls.Find("Current Tileset", true).ElementAt(0)).Text = filepath.Substring(filepath.LastIndexOf('\\') + 1);
			tilefile = filepath;
		}

		private void InitializeLayers()
		{
			var Layers = (ListBox)this.Controls.Find("Layers", true).ElementAt(0);
			for(int l = 0; l < layers.Count; l++)
				Layers.Items.Add("LAYER" + (l+1) + ": TileId = " + layers.ElementAt(l));
		}
		
		private void slopeselect_SelectedIndexChanged(object sender, EventArgs e)
		{
			slope = ((ComboBox) sender).SelectedIndex;
			picBox1.Refresh();
		}

		private void DrawImageRect(object sender, PaintEventArgs e)
		{
			Image slopemaps = Image.FromFile(@"Content\Images\default_slopemaps.png");
			if(slope > 0 && slope < 10)
				e.Graphics.DrawImage(slopemaps,
						new Rectangle(0, 0, slopemaps.Width / 8, slopemaps.Height),
						new Rectangle((slopemaps.Width / 8) * (slope - 1), 0, slopemaps.Width / 8, slopemaps.Height),
						GraphicsUnit.Pixel);
			else
			{
				e.Graphics.DrawRectangle(new Pen(Color.LightGray, 4.0f), 0, 0, slopemaps.Width / 8, slopemaps.Height);
			}			
		}

		private void confirmation_Click(object sender, EventArgs e)
		{
			ok = true;
			bool success = true;
			string errstr = "";
			walkable = walkableCombo.SelectedIndex;
			type = typeCombo.SelectedIndex;
			slope = slopeCombo.SelectedIndex;	
			
			try{ SMapWidth = int.Parse(MapWidth.Text); }
			catch{ errstr = "Map Width"; success = false; }
			
			if(success)
			{
			try{ SMapHeight = int.Parse(MapHeight.Text); }
			catch{ errstr = "Map Height"; success = false; }
			}

			if(success)
			{
			try{ tileWid = (int)uint.Parse((this.Controls.Find("Tile Width", true).ElementAt(0)).Text); }
			catch{ errstr = "Tile Width"; success = false; }
			}

			if(success)
			{
			try{ tileHei = (int)uint.Parse((this.Controls.Find("Tile Height", true).ElementAt(0)).Text); }
			catch{ errstr = "Tile Height"; success = false; }
			}

			if(success)
			{
				if(slope != 0)
					type = 3;
				this.Close();
			}	
			else
			{
				showbox = true;
				MessageBox.Show("Invalid data detected for " + errstr);
				showbox = false;
			}
		}

		void addLayer_Click(object sender, EventArgs e)
		{
			bool success = true;
			string errstr = "";
			var TileId = (TextBox)this.Controls.Find("TileId", true).ElementAt(0);
			if(success)
			{
				try{ tileId = uint.Parse(TileId.Text); }
				catch{	errstr = "Error: Invalid entry in TileId"; success = false; }
			}	
			if(success)
			{
				if(tileId > maxTile)
					tileId = (uint)maxTile;

				var Layers = (ListBox)this.Controls.Find("Layers", true).ElementAt(0);
				Layers.Items.Add("LAYER" + (layers.Count + 1) +": TileId = " + tileId);
				layers.Add((int)tileId);

			}
			else
			{
				showbox = true;
				MessageBox.Show(errstr);
				showbox = false;
			}
		} 

		void removeLayer_Click(object sender, EventArgs e)
		{
			var Layers = (ListBox)this.Controls.Find("Layers", true).ElementAt(0);
			if(Layers.Items.Count != 0)
			{
				Layers.Items.RemoveAt(Layers.Items.Count - 1);
				layers.RemoveAt(layers.Count - 1);
			}
		}
		void removeAll_Click(object sender, EventArgs e)
		{
			var Layers = (ListBox)this.Controls.Find("Layers", true).ElementAt(0);
			Layers.Items.Clear();
			layers.Clear();
		}

		private void cancelBut_Click(object sender, EventArgs e)
		{
			if(!showbox)
			{
				ok = false;
				this.Close();
			}
		}

	}
}
