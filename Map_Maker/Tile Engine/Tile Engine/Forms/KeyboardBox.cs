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
	public partial class KeyboardBox : Form
	{
		public KeyBoardShortcuts Shortcuts;
		
		public struct KeyBoardShortcuts
		{
			public string trashKey;
			public string magnifyKey;
			public string saveKey;
			public string loadKey;
			public string settingsKey;
			public string authorKey;
			public string functionKey;
			public string selectKey;
			public string coordsKey;
			public string eventsKey;
			public string hiddenKey;
			public string upKey;
			public string downKey;
			public string leftKey;
			public string rightKey;
			public string charKey;
			public string playKey;
			public string walkKey;
			public string cubeSpaceKey;

			public KeyBoardShortcuts(string tk, string mk, string sk, string lk, string stk, string ak, 
										string fk, string selk, string coordk, string eventk, string hidk,
										string uk, string dk, string leftk, string rightk, string ck, string pk,
										string wk, string csk)
			{
				this.trashKey = tk;
				this.magnifyKey = mk;
				this.saveKey = sk;
				this.loadKey = lk;
				this.settingsKey = stk;
				this.authorKey = ak;
				this.functionKey = fk;
				this.selectKey = selk;
				this.coordsKey = coordk;
				this.eventsKey = eventk;
				this.hiddenKey = hidk;
				this.walkKey = wk;
				this.cubeSpaceKey = csk;
				
				this.upKey = uk;
				this.downKey = dk;
				this.leftKey = leftk;
				this.rightKey = rightk;
				this.charKey = ck;
				this.playKey = pk;
			}
		}

		public bool ok;
		private bool showbox;
		public KeyboardBox(string tk, string mk, string sk, string lk, string stk, string ak, string fk,
							string selk, string coordk, string eventk, string hidk,
							string uk, string dk, string leftk, string rightk, string ck, string pk,
							string wk, string csk)
		{
			Shortcuts = new KeyBoardShortcuts(tk, mk, sk, lk, stk, ak, fk, selk, coordk, eventk, hidk, uk, dk, leftk, rightk, ck, pk, wk, csk);
			InitializeComponent();
		}
		void KeyboardBox_Load(object sender, System.EventArgs e)
		{
			ok = true;
			showbox = false;
			Panel ShortcutPanel = new Panel(){Name = "Shortcut Panel", Top = 5, Width = this.Width, Height = 290};
			Label ShortcutLabel = new Label(){Text = "Keyboard Shortcuts:", Top = 10, Left = 30, Height = 15};
			
			// Col 1
			Label TrashLabel = new Label(){Text = "Trash", Top = 33, Left = 30, Height = 15};
			Label SaveLabel = new Label() { Text = "Save Level", Top = 53, Left = 30, Height = 15, Width = 80 };
			Label SCtrl = new Label() { Text = "Ctrl +", Top = 53, Left = 150, Height = 15, Width = 40 };			
			Label SettingsLabel = new Label() { Text = "Change Settings", Top = 73, Left = 30, Height = 15 };
			Label FunctionsLabel = new Label() { Text = "Functions", Top = 93, Left = 30, Height = 15 };
			Label UpLabel = new Label() { Text = "Increment Current Tile", Top = 113, Left = 30, Height = 15, Width = 120 };
			Label UCtrl = new Label() { Text = "Ctrl +", Top = 113, Left = 150, Height = 15, Width = 40 };						
			Label LeftLabel = new Label() { Text = "Increment Tileset View", Top = 133, Left = 30, Height = 15, Width = 120 };
			Label LeCtrl = new Label() { Text = "Ctrl +", Top = 133, Left = 150, Height = 15, Width = 40 };
			Label CubeLabel = new Label() { Text = "Toggle Cubespace", Top = 153, Left = 30, Height = 15, Width = 120 };
			Label CubeCtrl = new Label() { Text = "Ctrl +", Top = 153, Left = 150, Height = 15, Width = 40 };
			
			TextBox TrashText = new TextBox() { Name = "Trash", Text = Shortcuts.trashKey, Top = 30, Left = 190, Height = 15, Width = 40 };
			TextBox SaveText = new TextBox() { Name = "Save Level", Text = Shortcuts.saveKey, Top = 50, Left = 190, Height = 15, Width = 40 };
			TextBox SettingsText = new TextBox() { Name = "Change Settings", Text = Shortcuts.settingsKey, Top = 70, Left = 190, Height = 15, Width = 40 };
			TextBox FunctionsText = new TextBox() { Name = "Functions", Text = Shortcuts.functionKey, Top = 90, Left = 190, Height = 15, Width = 40 };
			TextBox UpText = new TextBox() { Name = "Increment Current Tile", Text = Shortcuts.upKey, Top = 110, Left = 190, Height = 15, Width = 40 };
			TextBox LeftText = new TextBox() { Name = "Decrement Tileset View", Text = Shortcuts.leftKey, Top = 130, Left = 190, Height = 15, Width = 40 };
			TextBox CubeText = new TextBox() { Name = "Toggle Cubespace", Text = Shortcuts.cubeSpaceKey, Top = 150, Left = 190, Height = 15, Width = 40 };
			
			// Col 2
			Label MagLabel = new Label() { Text = "Magnify", Top = 33, Left = 230, Height = 15 };						
			Label LoadLabel = new Label() { Text = "Load Level", Top = 53, Left = 230, Height = 15, Width = 80 };
			Label LCtrl = new Label() { Text = "Ctrl +", Top = 53, Left = 350, Height = 15, Width = 40};								
			Label AuthorLabel = new Label() { Text = "Author's Notes", Top = 73, Left = 230, Height = 15 };						
			Label SelectLabel = new Label() { Text = "Place/Select Tiles", Top = 93, Left = 230, Height = 15 };			
			Label DownLabel = new Label() { Text = "Decrement Current Tile", Top = 113, Left = 230, Height = 15, Width = 122 };			
			Label DCtrl = new Label() { Text = "Ctrl +", Top = 113, Left = 350, Height = 15, Width = 40 };									
			Label RightLabel = new Label() { Text = "Decrement Tileset View", Top = 133, Left = 230, Height = 15, Width = 124 };
			Label RiCtrl = new Label() { Text = "Ctrl +", Top = 133, Left = 350, Height = 15, Width = 40 };

			TextBox MagText = new TextBox() { Name = "Magnify", Text = Shortcuts.magnifyKey, Top = 30, Left = 390, Height = 15, Width = 40 };
			TextBox LoadText = new TextBox() { Name = "Load Level", Text = Shortcuts.loadKey, Top = 50, Left = 390, Height = 15, Width = 40 };
			TextBox AuthorText = new TextBox() { Name = "Author's Notes", Text = Shortcuts.authorKey, Top = 70, Left = 390, Height = 15, Width = 40 };
			TextBox SelectText = new TextBox() { Name = "Place/Select Tiles", Text = Shortcuts.selectKey, Top = 90, Left = 390, Height = 15, Width = 40 };
			TextBox DownText = new TextBox() { Name = "Decrement Current Tile", Text = Shortcuts.downKey, Top = 110, Left = 390, Height = 15, Width = 40 };
			TextBox RightText = new TextBox() { Name = "Increment Tileset View", Text = Shortcuts.rightKey, Top = 130, Left = 390, Height = 15, Width = 40 };

			// Col 3
			Label CoordLabel = new Label() { Text = "Show Coordinates", Top = 33, Left = 430, Height = 15 };
			Label EventsLabel = new Label() { Text = "Show Events", Top = 53, Left = 430, Height = 15 };
			Label HiddenLabel = new Label() { Text = "Underground View", Top = 73, Left = 430, Height = 15 }; 
			Label CharLabel = new Label(){ Text = "Character Settings", Top = 93, Left = 430, Height = 15 };
			Label PlayLabel = new Label(){ Text = "Change Modes", Top = 113, Left = 430, Height = 15 };
			Label PlayCtrl = new Label() { Text = "Ctrl +", Top = 113, Left = 550, Height = 15, Width = 40 };
			Label WalkLabel = new Label() { Text = "Toggle Walkable", Top = 133, Left = 430, Height = 15 };
			Label WalkCtrl = new Label() { Text = "Ctrl +", Top = 133, Left = 550, Height = 15, Width = 40 };
			
			TextBox CoordText = new TextBox() { Name = "Show Coordinates", Text = Shortcuts.coordsKey, Top = 33, Left = 590, Height = 15, Width = 40 };
			TextBox EventsText = new TextBox() { Name = "Show Events", Top = 53, Text = Shortcuts.eventsKey, Left = 590, Height = 15, Width = 40 };
			TextBox HiddenText = new TextBox() { Name = "Underground View", Top = 73, Text = Shortcuts.hiddenKey, Left = 590, Height = 15, Width = 40 }; 
			TextBox CharText = new TextBox() { Name = "Character Settings", Top = 93, Text = Shortcuts.charKey, Left = 590, Height = 15, Width = 40 };
			TextBox PlayText = new TextBox() { Name = "Play Mode", Top = 113, Text = Shortcuts.playKey, Left = 590, Height = 15, Width = 40 };
			TextBox WalkText = new TextBox() { Name = "Toggle Walkable", Top = 133, Text = Shortcuts.walkKey, Left =  590, Height = 15, Width = 40 };

			Button okButton = new Button(){ Text = "Ok", Top = 175, Left = 30};
			okButton.Click += new EventHandler(okButton_Click);

			Button cancelButton = new Button(){ Text = "Cancel", Top = 175, Left = 105 };
			cancelButton.Click += new EventHandler(cancelButton_Click);

			ShortcutPanel.Controls.Add(ShortcutLabel);
			ShortcutPanel.Controls.Add(TrashLabel);
			ShortcutPanel.Controls.Add(MagLabel);
			ShortcutPanel.Controls.Add(SaveLabel);
			ShortcutPanel.Controls.Add(SCtrl);
			ShortcutPanel.Controls.Add(LoadLabel);
			ShortcutPanel.Controls.Add(LCtrl);
			ShortcutPanel.Controls.Add(SettingsLabel);
			ShortcutPanel.Controls.Add(AuthorLabel);
			ShortcutPanel.Controls.Add(FunctionsLabel);
			ShortcutPanel.Controls.Add(SelectLabel);
			ShortcutPanel.Controls.Add(UpLabel);
			ShortcutPanel.Controls.Add(UCtrl);
			ShortcutPanel.Controls.Add(DownLabel);
			ShortcutPanel.Controls.Add(DCtrl);
			ShortcutPanel.Controls.Add(LeftLabel);
			ShortcutPanel.Controls.Add(LeCtrl);
			ShortcutPanel.Controls.Add(RiCtrl);
			ShortcutPanel.Controls.Add(RightLabel);
			ShortcutPanel.Controls.Add(CoordLabel);
			ShortcutPanel.Controls.Add(EventsLabel);
			ShortcutPanel.Controls.Add(HiddenLabel);
			ShortcutPanel.Controls.Add(CharLabel);
			ShortcutPanel.Controls.Add(PlayLabel);
			ShortcutPanel.Controls.Add(PlayCtrl);
			ShortcutPanel.Controls.Add(WalkLabel);
			ShortcutPanel.Controls.Add(WalkCtrl);
			ShortcutPanel.Controls.Add(CubeLabel);
			ShortcutPanel.Controls.Add(CubeCtrl);

			ShortcutPanel.Controls.Add(TrashText);
			ShortcutPanel.Controls.Add(MagText);
			ShortcutPanel.Controls.Add(CoordText);
			ShortcutPanel.Controls.Add(SaveText);
			ShortcutPanel.Controls.Add(LoadText);
			ShortcutPanel.Controls.Add(EventsText);
			ShortcutPanel.Controls.Add(SettingsText);
			ShortcutPanel.Controls.Add(AuthorText);
			ShortcutPanel.Controls.Add(HiddenText);
			ShortcutPanel.Controls.Add(FunctionsText);
			ShortcutPanel.Controls.Add(SelectText); 
			ShortcutPanel.Controls.Add(CharText);
			ShortcutPanel.Controls.Add(UpText);
			ShortcutPanel.Controls.Add(DownText);
			ShortcutPanel.Controls.Add(LeftText);
			ShortcutPanel.Controls.Add(RightText);
			ShortcutPanel.Controls.Add(PlayText);
			ShortcutPanel.Controls.Add(WalkText);
			ShortcutPanel.Controls.Add(CubeText);

			ShortcutPanel.Controls.Add(okButton);
			ShortcutPanel.Controls.Add(cancelButton);

			this.Controls.Add(ShortcutPanel);
			this.AcceptButton = okButton;
			this.CancelButton = cancelButton;
			this.Location = new Point(50, 80);
			this.Deactivate += new EventHandler(cancelButton_Click);
		}
		
		private	void cancelButton_Click(object sender, EventArgs e)
		{
			if (!showbox)
			{
				ok = false;
				this.Close();
			}
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			string checkKey = "";
			string errstr = "";
			bool success = true;

			// Single Keys
			this.Shortcuts.trashKey = ((TextBox)this.Controls.Find("Trash", true).ElementAt(0)).Text.ToUpper();
			checkKey += this.Shortcuts.trashKey;

			this.Shortcuts.magnifyKey = ((TextBox)this.Controls.Find("Magnify", true).ElementAt(0)).Text.ToUpper();
			if(checkKey.IndexOf(this.Shortcuts.magnifyKey) != -1)
			{
				success = false;
				errstr += "Magnify, ";
			}
			else
				checkKey += this.Shortcuts.magnifyKey;

			this.Shortcuts.settingsKey = ((TextBox)this.Controls.Find("Change Settings", true).ElementAt(0)).Text.ToUpper();
			if(checkKey.IndexOf(this.Shortcuts.settingsKey) != -1)
			{
				success = false;
				errstr += "Settings, ";
			}
			else
				checkKey += this.Shortcuts.settingsKey;

			this.Shortcuts.authorKey = ((TextBox)this.Controls.Find("Author's Notes", true).ElementAt(0)).Text.ToUpper();
			if (checkKey.IndexOf(this.Shortcuts.authorKey) != -1)
			{
				success = false;
				errstr += "Author, ";
			}
			else
				checkKey += this.Shortcuts.authorKey;

			this.Shortcuts.functionKey = ((TextBox)this.Controls.Find("Functions", true).ElementAt(0)).Text.ToUpper();
			if (checkKey.IndexOf(this.Shortcuts.functionKey) != -1)
			{
				success = false;
				errstr += "Functions, ";
			}
			else
				checkKey += this.Shortcuts.functionKey;

			this.Shortcuts.selectKey = ((TextBox)this.Controls.Find("Place/Select Tiles", true).ElementAt(0)).Text.ToUpper();
			if (checkKey.IndexOf(this.Shortcuts.selectKey) != -1)
			{
				success = false;
				errstr += "Tiles, ";
			}
			else
				checkKey += this.Shortcuts.selectKey;

			this.Shortcuts.coordsKey = ((TextBox)this.Controls.Find("Show Coordinates", true).ElementAt(0)).Text.ToUpper();
			if (checkKey.IndexOf(this.Shortcuts.coordsKey) != -1)
			{
				success = false;
				errstr += "Coordinates, ";
			}
			else
				checkKey += this.Shortcuts.coordsKey;

			this.Shortcuts.eventsKey = ((TextBox)this.Controls.Find("Show Events", true).ElementAt(0)).Text.ToUpper();
			if (checkKey.IndexOf(this.Shortcuts.eventsKey) != -1)
			{
				success = false;
				errstr += "Events, ";
			}
			else
				checkKey += this.Shortcuts.eventsKey;

			this.Shortcuts.hiddenKey = ((TextBox)this.Controls.Find("Underground View", true).ElementAt(0)).Text.ToUpper();
			if (checkKey.IndexOf(this.Shortcuts.hiddenKey) != -1)
			{
				success = false;
				errstr += "Under View, ";
			}
			else
				checkKey += this.Shortcuts.hiddenKey;

			this.Shortcuts.charKey = ((TextBox)this.Controls.Find("Character Settings", true).ElementAt(0)).Text.ToUpper();
			if(checkKey.IndexOf(this.Shortcuts.charKey) != -1)
			{
				success = false;
				errstr += "Character";
			}
			else
				checkKey += this.Shortcuts.charKey;

			if(errstr != "")
				errstr = "Single Keys: " + errstr; 


			// Combo Ctrl Keys
			checkKey = "";
			int errindex = errstr.Length;
			
			this.Shortcuts.saveKey = ((TextBox)this.Controls.Find("Save Level", true).ElementAt(0)).Text.ToUpper();
			if (checkKey.IndexOf(this.Shortcuts.saveKey) != -1)
			{
				success = false;
				errstr += "Save, ";
			}
			else
				checkKey += this.Shortcuts.saveKey;

			this.Shortcuts.loadKey = ((TextBox)this.Controls.Find("Load Level", true).ElementAt(0)).Text.ToUpper();
			if (checkKey.IndexOf(this.Shortcuts.loadKey) != -1)
			{
				success = false;
				errstr += "Load, ";
			}
			else
				checkKey += this.Shortcuts.loadKey;

			this.Shortcuts.upKey = ((TextBox)this.Controls.Find("Increment Current Tile", true).ElementAt(0)).Text.ToUpper();
			if (checkKey.IndexOf(this.Shortcuts.upKey) != -1)
			{
				success = false;
				errstr += "Inc Tile, ";
			}
			else
				checkKey += this.Shortcuts.upKey;

			this.Shortcuts.downKey = ((TextBox)this.Controls.Find("Decrement Current Tile", true).ElementAt(0)).Text.ToUpper();
			if (checkKey.IndexOf(this.Shortcuts.downKey) != -1)
			{
				success = false;
				errstr += "Dec Tile, ";
			}
			else
				checkKey += this.Shortcuts.downKey;

			this.Shortcuts.leftKey = ((TextBox)this.Controls.Find("Decrement Tileset View", true).ElementAt(0)).Text.ToUpper();
			if (checkKey.IndexOf(this.Shortcuts.leftKey) != -1)
			{
				success = false;
				errstr += "Dec Tileset, ";
			}
			else
				checkKey += this.Shortcuts.leftKey;

			this.Shortcuts.rightKey = ((TextBox)this.Controls.Find("Increment Tileset View", true).ElementAt(0)).Text.ToUpper();
			if (checkKey.IndexOf(this.Shortcuts.rightKey) != -1)
			{
				success = false;
				errstr += "Inc Tileset, ";
			}
			else
				checkKey += this.Shortcuts.rightKey;

			this.Shortcuts.playKey = ((TextBox)this.Controls.Find("Play Mode", true).ElementAt(0)).Text.ToUpper();
			if (checkKey.IndexOf(this.Shortcuts.playKey) != -1)
			{
				success = false;
				errstr += "Change Mode, ";
			}
			else
				checkKey += this.Shortcuts.playKey;


			this.Shortcuts.walkKey = ((TextBox)this.Controls.Find("Toggle Walkable", true).ElementAt(0)).Text.ToUpper();
			this.Shortcuts.cubeSpaceKey = ((TextBox)this.Controls.Find("Toggle Cubespace", true).ElementAt(0)).Text.ToUpper();

			if(success)
			{
				this.ok = true;
				this.Close();
			}
			else	
			{
				if(errstr.Length > errindex)
				{
					string temp1 = errstr.Substring(0, errindex);
					string temp2 = errstr.Substring(errindex);
					errstr = temp1 + "\nCtrl Keys: " + temp2;
				}
				if(errstr.Length > 40)
				{
					int index = errstr.IndexOf(' ', 40);
					errstr.Insert(index, "\n");
				}

				showbox = true;
				MessageBox.Show("Invalid: Double Key Mapping Detected!\nMultiple Mappings for:\n" + errstr);
				showbox = false;
			}
		}

	}
}
