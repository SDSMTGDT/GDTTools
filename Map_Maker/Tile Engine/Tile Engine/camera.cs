using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tile_Engine
{
	// Viewport camera where map data will be rendered
    static class camera
    {
		// Viewport control variables
        static Vector2 location = new Vector2(0.0f, 0.0f); // Current camera location
		static float zoom; // camera zoom
		static float zoomstep; // camera zoom increment variable
		static float maxZoom = 4.0f; // maximum allowed zoom
		static float minZoom = 0.125f; // minimum allowed zoom
		static Matrix transform; // camera transformation matrix
		static float rotation; // camera rotation
		public static int ViewWidth{ get; set;} // viewport width
		public static int ViewHeight{ get; set;} // viewport height
		public static int worldWidth{ get; set;} // world width
		public static int worldHeight{ get; set;} // world height
		public static float AspectRatio{ get; set;} // Ratio of W to H
			
		public static Vector2 displayOffset{ get; set;} // viewport offsets

		// Wrapper property to auto restrict the camera movement
		public static Vector2 Location
		{
			get
			{
				return location;
			}
			set
			{
				location = new Vector2(MathHelper.Clamp(value.X, 0f, worldWidth - ViewWidth), // Remove + 200
									   MathHelper.Clamp(value.Y, 0f, worldHeight - ViewHeight)); // Remove + 100
			}
		}

		// Camera zoom control 
		public static float Zoom
		{
			get{ return zoom; }
			set
			{
				zoom = MathHelper.Clamp(value, minZoom, maxZoom);
			}

		}

		// Camera zoom increment control
		public static float ZoomStep
		{
			get{ return zoomstep; }
			set
			{
				zoomstep = MathHelper.Clamp(value, 2.0f, 8.0f);
			}
		}

		// Camera rotation control
		public static float Rotation
		{
			get{ return rotation; }
			set{ rotation = value; }
		}
		
		// Creates a matrix transform
		public static Matrix get_transformation(GraphicsDevice graphicsdevice)
		{
			transform = 
					Matrix.CreateRotationZ(Rotation) * 
					Matrix.CreateScale(new Vector3(Zoom, Zoom, 1));
			return transform;
		}
		
		// Increase the camera zoom 
		public static bool Increase_Zoom(Vector2 Center)
		{
			if(Zoom == maxZoom)
				return false;

			Zoom = Zoom * ZoomStep;
			CenterCamera(Center);
			return true;
		}

		// Decrease the camera zoom
		public static bool Decrease_Zoom(Vector2 Center)
		{
			if(Zoom == minZoom)
				return false;

			Zoom = Zoom / ZoomStep;
			CenterCamera(Center);
			return true;
		}
		
		// Center the camera around the point
		public static void CenterCamera(Vector2 Center)
		{
			Location = new Vector2(Center.X - (ViewWidth / (2 * Zoom)), Center.Y - (ViewHeight / (2 * Zoom)));
		}

		// Conversion function from world to screen coordinates
		public static Vector2 WorldtoScreen(Vector2 worldPosition)
		{
			return worldPosition - Location + displayOffset;
		}

		// Conversion function from screen to world coordinates
		public static Vector2 ScreentoWorld(Vector2 screenPosition)
		{
			return (screenPosition)/ Zoom + Location  - displayOffset;
		}

		// Move the camera based upon the given offset vector
		public static void Move(Vector2 offset)
		{
			Location += (offset / Zoom);
		}


    }
}
