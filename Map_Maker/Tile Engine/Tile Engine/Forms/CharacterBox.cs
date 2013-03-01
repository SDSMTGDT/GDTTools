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
	public partial class CharacterBox : Form
	{

		public string SName;
		public string CUp;
		public string CDown;
		public string CLeft;
		public string CRight;
		public int startx;
		public int starty;

		public bool ok;
		public bool showbox;

		public CharacterBox(string n, string u, string d, string l, string r, int x, int y)
		{
			SName = n;
			CUp = u;
			CDown = d;
			CLeft = l;
			CRight = r;
			startx = x;
			starty = y;
			InitializeComponent();

		}
		
		private void CharacterBox_Load(object sender, EventArgs e)
		{
			ok = true;
			showbox = false;
			Panel charPanel = new Panel(){ Name = "Char Panel", Left = 0, Top = 5, Width = this.Width, Height = this.Height };
			Label charLabel = new Label(){ Text = "Character Settings", Left = 30, Top = 10, Height = 15};
			Label charName = new Label() { Text = "Character Name", Left = 30, Top = 25, Height = 15, };
			TextBox charNameText = new TextBox(){ Name = "Character Name", Text = SName, Left = 30, Top = 40, Width = 80};
			Label charMove = new Label() { Text = "Character Movement Keys", Left = 30, Top = 70, Height = 15, Width = 200};
			Label charUp = new Label() { Text = "Up", Left = 30, Top = 88, Height = 15, Width = 40};
			TextBox charUpText = new TextBox(){ Name = "Up", Text = CUp, Left = 80, Top = 85, Width = 40};
			Label charDown = new Label() { Text = "Down", Left = 125, Top = 88, Height = 15, Width = 40 };
			TextBox charDownText = new TextBox(){ Name = "Down", Text = CDown, Left = 175, Top = 85, Width = 40};
			Label charLeft = new Label() { Text = "Left", Left = 30, Top = 108, Height = 15, Width = 40 };
			TextBox charLeftText = new TextBox(){ Name = "Left", Text = CLeft, Left = 80, Top = 105, Width = 40};
			Label charRight = new Label() { Text = "Right", Left = 125, Top = 108, Height = 15, Width = 40 };
			TextBox charRightText = new TextBox(){ Name = "Right", Text = CRight, Left = 175, Top = 105, Width = 40};

			Label charPosLabel = new Label() { Text = "Character Starting Position", Left = 30, Top = 125, Width = 200, Height = 15};
			Label charPosX = new Label() { Text = "X: ", Left = 30, Top = 143, Width = 30};
			TextBox charPosXText = new TextBox() { Name = "X Position", Text = startx.ToString(), Left = 60, Top = 140, Width = 30 };
			Label charPosY = new Label() { Text = "Y: ", Left = 100, Top = 143, Width = 30 };
			TextBox charPosYText = new TextBox() { Name = "Y Position", Text = starty.ToString(), Left = 130, Top = 140, Width = 30 };

			Button okbutton = new Button(){ Text = "Ok", Left = 30, Top = 205};
			okbutton.Click += new EventHandler(okbutton_Click);

			Button cancelButton = new Button(){ Text = "Cancel", Left = 105, Top = 205};
			cancelButton.Click += new EventHandler(cancelButton_Click);

			charPanel.Controls.Add(charLabel);
			charPanel.Controls.Add(charName);
			charPanel.Controls.Add(charNameText);
			charPanel.Controls.Add(charMove);
			charPanel.Controls.Add(charUp);
			charPanel.Controls.Add(charUpText);
			charPanel.Controls.Add(charDown);
			charPanel.Controls.Add(charDownText);
			charPanel.Controls.Add(charLeft);
			charPanel.Controls.Add(charLeftText);
			charPanel.Controls.Add(charRight);
			charPanel.Controls.Add(charRightText);
			charPanel.Controls.Add(charPosLabel);
			charPanel.Controls.Add(charPosX);
			charPanel.Controls.Add(charPosXText);
			charPanel.Controls.Add(charPosY);
			charPanel.Controls.Add(charPosYText);

			charPanel.Controls.Add(okbutton);
			charPanel.Controls.Add(cancelButton);

			this.Controls.Add(charPanel);
			this.AcceptButton = okbutton;
			this.CancelButton = cancelButton;
			this.Location = new Point(50, 80);

		}

		void okbutton_Click(object sender, EventArgs e)
		{
			string checkKey = "";
			string errstr = "";
			string posstr = "";
			bool success = true;
			bool possuccess = true;

			this.SName = ((TextBox)this.Controls.Find("Character Name", true).ElementAt(0)).Text;

			// Single Keys
			this.CUp = ((TextBox)this.Controls.Find("Up", true).ElementAt(0)).Text.ToUpper();
			checkKey += this.CUp;

			this.CDown = ((TextBox)this.Controls.Find("Down", true).ElementAt(0)).Text.ToUpper();
			if (checkKey.IndexOf(this.CDown) != -1)
			{
				success = false;
				errstr += "Down, ";
			}
			else
				checkKey += this.CDown;

			this.CLeft = ((TextBox)this.Controls.Find("Left",true).ElementAt(0)).Text.ToUpper();
			if(checkKey.IndexOf(this.CLeft) != -1)
			{
				success = false;
				errstr += "Left, ";
			}
			else
				checkKey += this.CLeft;

			this.CRight = ((TextBox)this.Controls.Find("Right", true).ElementAt(0)).Text.ToUpper();
			if(checkKey.IndexOf(this.CRight) != -1)
			{
				success = false;
				errstr += "Right, ";
			}
			else
				checkKey += this.CRight;

			try{ this.startx = (int)(uint.Parse(((TextBox)this.Controls.Find("X Position", true).ElementAt(0)).Text)); }
			catch{ possuccess = false; posstr += "X Position"; }

			try{ this.starty = (int)(uint.Parse(((TextBox)this.Controls.Find("Y Position", true).ElementAt(0)).Text)); }
			catch{ possuccess = false; posstr += "Y Position"; }

			if(success && possuccess)
			{
				ok = true;
				this.Close();
			}
			else if(!success)
			{
				if (errstr.Length > 40)
				{
					int index = errstr.IndexOf(' ', 40);
					errstr.Insert(index, "\n");
				}

				showbox = true;
				MessageBox.Show("Invalid: Double Key Mapping Detected!\nMultiple Mappings for:\n" + errstr);
				showbox = false;
			}
			else if(!possuccess)
			{
				showbox = true;
				MessageBox.Show("Invalid Input for: " + posstr);
				showbox = false;
			}
		}
		void cancelButton_Click(object sender, EventArgs e)
		{
			if(!showbox)
			{
				ok = false;
				this.Close();
			}
		}
	}
}
