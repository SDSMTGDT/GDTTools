using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Tile_Engine
{
	public class TileEventObject
	{
		public object ActionData;
		public Rectangle ActivationArea;
		public string PathInfo;

		/// <summary>
		/// Stores the Event Action Data and Activation Location 
		/// </summary>
		/// <param name="sender">Event Action Object data</param>
		/// <param name="activationArea">Viable area for firing event</param>
		public TileEventObject(object sender, Rectangle activationArea, string pathInfo = null)
		{
			this.ActionData = sender;
			this.ActivationArea = activationArea;
			this.PathInfo = pathInfo;
		}

		public TileEventObject(object sender, int x1, int y1, int x2, int y2)
		{
			this.ActionData = sender;
			this.ActivationArea = new Rectangle(x1, y1, x2, y2);
		}

	}

}
