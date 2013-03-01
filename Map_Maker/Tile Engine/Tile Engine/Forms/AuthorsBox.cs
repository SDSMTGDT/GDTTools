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
	public partial class AuthorsBox : Form
	{
		private TextBox AuthorName;
		private TextBox Date;
		private TextBox Notes;
		public string name { get; set;}
		public string date { get; set; }
		public string notes { get; set; }
		public bool ok;
		private bool showbox;
		public AuthorsBox()
		{
			InitializeComponent();
		}
		public void Author_Load(object sender, EventArgs e)
		{
			ok = true;
			showbox = false;
			int topref = 10;

			this.Width = 500;
			this.Height = 375;
			
			Panel AuthorPanel = new Panel() { Width = 150, Height = 100 };
			Label AuthorLabel = new Label() { Text = "Author", Left = 50, Top = topref, Height = 15};
			AuthorName = new TextBox() {Text = System.Environment.UserName.ToString(), Left = 50, Top = topref + 20 };
			
			Label DateLebel = new Label() { Text = "Date", Left = 50, Top = topref + 50, Height = 15 };
			Date = new TextBox() {Text = DateTime.Now.ToString("M/dd/yyyy"), Left = 50, Top = topref + 70 };

			AuthorPanel.Controls.Add(AuthorLabel);
			AuthorPanel.Controls.Add(AuthorName);
			AuthorPanel.Controls.Add(DateLebel);
			AuthorPanel.Controls.Add(Date);

			Panel NotesPanel = new Panel() {Width = 420, Height = 250, Top = topref + 90};
			Label NotesLabel = new Label() {Text = "Level Notes:\nThese will be written at the top of the level file", Width = 250, Height = 35, Left = 50, Top = topref };
			Notes = new TextBox() {Left = 50, Top = topref + 35, Multiline = true, Width = 400, Height = 150};

			NotesPanel.Controls.Add(NotesLabel);
			NotesPanel.Controls.Add(Notes);
	
			Button confirmation = new Button() { Text = "Ok", Left = 280,  Top = topref + 285 };
			confirmation.Click += confirmation_Click;

			Button cancelBut = new Button() { Text = "Cancel", Left = 350, Top = topref + 285 };
			cancelBut.Click += new EventHandler(cancelBut_Click);
			this.Location = new Point(50, 80);

			this.Controls.Add(confirmation);
			this.Controls.Add(cancelBut);
			this.AcceptButton = confirmation;
			this.CancelButton = cancelBut;
			this.Deactivate += new EventHandler(cancelBut_Click);
			this.Controls.Add(AuthorPanel);
			this.Controls.Add(NotesPanel);
		}

		void cancelBut_Click(object sender, EventArgs e)
		{
			if (!showbox)
			{
				ok = false;
				this.Close();
			}
		}

		private void confirmation_Click(object sender, EventArgs e)
		{
			ok = true;
			name = AuthorName.Text;
			date = Date.Text;
			notes = Notes.Text;
			this.Close();
		}
	}
}
