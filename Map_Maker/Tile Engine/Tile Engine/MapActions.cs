using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tile_Engine
{
	public struct SpriteSheetData
	{
		public Texture2D SpriteSheet;
		public List<AnimationData> Animations;

		/// <summary>
		/// Holds data for Switching to a new Sprite Sheet 
		/// </summary>
		/// <param name="spriteSheet">Sprite Animation Texture</param>
		/// <param name="animations">List of retrievable animations</param>
		public SpriteSheetData(Texture2D spriteSheet, List<AnimationData> animations)
		{
			this.SpriteSheet = spriteSheet;
			this.Animations = animations;
		}

		/// <summary>
		/// Holds data for Switching to a new Sprite Sheet 
		/// </summary>
		/// <param name="spriteSheet">Sprite Animation Texture</param>
		/// <param name="spriteSheet"></param>
		public SpriteSheetData(Texture2D spriteSheet)
		{
			this.SpriteSheet = spriteSheet;
			this.Animations = null;
		}
	}

	public struct AnimationData
	{
		public string Name;
		public int X;
		public int Y;
		public int Width;
		public int Height;
		public int Frames;
		public float FrameLength;
		public string NextAnimation;

		/// <summary>
		/// Holds Data for a given SpriteAnimation animation sequence 
		/// </summary>
		/// <param name="name">Name of the Animation</param>
		/// <param name="x">SpriteSheet starting x location</param>
		/// <param name="y">SpriteSheet starting y location</param>
		/// <param name="width">Width of each frame</param>
		/// <param name="height">Height of each frame</param>
		/// <param name="frames">Number of frames in sequence</param>
		/// <param name="framelength">Timelength of each frame, in Seconds</param>
		public AnimationData(string name, int x, int y, int width, int height, int frames, int framelength)
		{
			this.Name = name;
			this.X = x;
			this.Y = y;
			this.Width = width;
			this.Height = height;
			this.Frames = frames;
			this.FrameLength = framelength;
			this.NextAnimation = null;
		}

		/// <summary>
		/// Holds Data for a given SpriteAnimation animation sequence and provides direction to a follow up animation sequence
		/// </summary>
		/// <param name="name">Name of the Animation</param>
		/// <param name="x">SpriteSheet starting x location</param>
		/// <param name="y">SpriteSheet starting y location</param>
		/// <param name="width">Width of each frame</param>
		/// <param name="height">Height of each frame</param>
		/// <param name="frames">Number of frames in sequence</param>
		/// <param name="framelength">Timelength of each frame, in Seconds</param>
		/// <param name="nextanimation">Followup animation sequence</param>
		public AnimationData(string name, int x, int y, int width, int height, int frames, int framelength, string nextanimation)
		{
			this.Name = name;
			this.X = x;
			this.Y = y;
			this.Width = width;
			this.Height = height;
			this.Frames = frames;
			this.FrameLength = framelength;
			this.NextAnimation = nextanimation;
		}
	}

	public struct SwapData
	{
		//	public Rectangle ActivationArea;
		public List<Vector3> InitialPos;
		public List<Vector3> FinalPos;
		/// <summary>
		/// Holds Data for swaping MapCells within a TileMap object
		/// </summary>
		/// <param name="activationArea">Location that the Swap may be initiated</param>
		/// <param name="initialPos">Starting Positions of all Cells to be swaped</param>
		/// <param name="finalPos">Final Position of all Swaped Cells</param>
		public SwapData(List<Vector3> initialPos, List<Vector3> finalPos)
		{
			//this.ActivationArea = activationArea;
			this.InitialPos = initialPos;
			this.FinalPos = finalPos;
		}
	}

	public struct TileSetData
	{
		public Texture2D TileSet;
		public int tileWidth;
		public int tileHeight;
		public int tileStepX;
		public int tileStepY;
		public int oddRowOffset;
		public int heightTileOffset;
		/// <summary>
		/// Holds Data for changing Tile object tilesets
		/// </summary>
		/// <param name="TileSet">New TileSet Texture</param>
		/// <param name="tileWidth">Width of each Tile</param>
		/// <param name="tileHeight">Height of each Tile</param>
		/// <param name="tileStepX">Display step in the horizontal direction</param>
		/// <param name="tileStepY">Display step in the vertical direction</param>
		/// <param name="oddRowOffset">Horizontal offset for odd row tile placement</param>
		/// <param name="heightTileOffset">Vertical Offset for heighttile placement</param>
		public TileSetData(Texture2D tileSet, int tileWidth, int tileHeight, int tileStepX, int tileStepY, int oddRowOffset, int heightTileOffset)
		{
			this.TileSet = tileSet;
			this.tileWidth = tileWidth;
			this.tileHeight = tileHeight;
			this.tileStepX = tileStepX;
			this.tileStepY = tileStepY;
			this.oddRowOffset = oddRowOffset;
			this.heightTileOffset = heightTileOffset;
		}
	}

	// PLACED HERE FOR COMPILER ERROR PURPOSES, IN FULL VERSION RELOCATE
	public struct LightSource
	{
		public Vector3 Position;
		public Vector4 Color;
	}

	/// <summary>
	/// Helper Class Object that Provides a Selection of GameState Augmenting Functions
	/// </summary>
	static class MapActions
	{
		/// <summary>
		/// Adds a light source to the Scene list of lights
		/// </summary>
		/// <param name="Lights">List of Scene lights</param>
		/// <param name="nLight">New light to add to the list</param>
		public static void AddLightSource(List<LightSource> Lights, LightSource nLight)
		{
			Lights.Add(nLight);
		}

		/// <summary>
		/// Removes a light source from the Scene list of Lights at the given position
		/// if no position is provided, the light at the end of the list is removed
		/// </summary>
		/// <param name="Lights">List of Scene lights</param>
		/// <param name="Pos">Position in list to remove source</param>
		public static void RemoveLightSource(List<LightSource> Lights, int Pos = -1)
		{
			if (Pos == -1)
			{
				Lights.RemoveAt(Lights.Count - 1);
			}
			else
				Lights.RemoveAt(Pos);
		}

		/// <summary>
		/// Replaces the MapCell located at the position with a new MapCell
		/// </summary>
		/// <param name="map">Current Map</param>
		/// <param name="cell">New MapCell</param>
		/// <param name="pos">Position of Old Cell</param>
		public static void ReplaceCell(TileMap map, MapCell cell, Vector2 pos)
		{
			ReplaceCell(map, cell, (int)pos.X, (int)pos.Y);
		}

		/// <summary>
		/// Replaces the MapCell located at the position with a new MapCell
		/// </summary>
		/// <param name="map">Current Map</param>
		/// <param name="cell">New MapCell</param>
		/// <param name="x">X Location of Cell</param>
		/// <param name="y">Y Location of Cell</param>
		public static void ReplaceCell(TileMap map, MapCell cell, int x, int y)
		{
			map.Rows[x].Columns[y] = cell;
		}

		/// <summary>
		/// Swaps the contents of two MapCells on a TileMap
		/// </summary>
		/// <param name="map">Current Map</param>
		/// <param name="x">X Location of Cell1</param>
		/// <param name="y">Y Location of Cell1</param>
		/// <param name="nx">X Location of Cell2</param>
		/// <param name="ny">Y Location of Cell2</param>
		public static void SwapCell(TileMap map, int x, int y, int nx, int ny)
		{
			MapCell temp = map.Rows[x].Columns[y];
			map.Rows[x].Columns[y] = map.Rows[nx].Columns[ny];
			map.Rows[nx].Columns[ny] = temp;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="map">Current Tile Map</param>
		/// <param name="swaps">Holds initial and final swaping positions</param>
		public static void SwapCells(TileMap map, SwapData swaps)
		{
			int x1, y1, x2, y2;
			MapCell temp;
			// Swap every instance in the SwapData
			for (int i = 0; i < swaps.InitialPos.Count; i++)
			{
				x1 = (int)(swaps.InitialPos.ElementAt(i).X);
				y1 = (int)(swaps.InitialPos.ElementAt(i).Y);
				x2 = (int)(swaps.FinalPos.ElementAt(i).X);
				y2 = (int)(swaps.FinalPos.ElementAt(i).Y);
				temp = map.Rows[x1].Columns[y1];
				map.Rows[x1].Columns[y1] = map.Rows[x2].Columns[y2];
				map.Rows[x2].Columns[y2] = temp;
			}
		}

		/// <summary>
		/// Changes the Sprite Sheet in use for the given SpriteAnimation Object
		/// </summary>
		/// <param name="Sprite">SpriteAnimation Object</param>
		/// <param name="text">New SpriteSheet texture for SpriteAnimation</param>
		/// <param name="animations">Animation data for each animation sequence</param>
		public static void ChangeSpriteSheets(SpriteAnimation Sprite, Texture2D text, List<AnimationData> animations)
		{
			Sprite.Texture = text;
			// Use the previous animations
			if (animations != null)
			{
				Sprite.ClearAnimations();
				foreach (AnimationData animation in animations)
				{
					if (animation.NextAnimation != null)
						Sprite.AddAnimation(animation.Name, animation.X, animation.Y, animation.Width, animation.Height, animation.Frames, animation.FrameLength);
					else
						Sprite.AddAnimation(animation.Name, animation.X, animation.Y, animation.Width, animation.Height, animation.Frames, animation.FrameLength, animation.NextAnimation);
				}
			}
		}

		/// <summary>
		/// Changes the Sprite Sheet in use for the given SpriteAnimation Object
		/// </summary>
		/// <param name="SpriteSheetData">Holds all required data for changing sprite sheets</param>
		public static void ChangeSpriteSheets(SpriteAnimation Sprite, SpriteSheetData spriteSheet)
		{
			Sprite.Texture = spriteSheet.SpriteSheet;
			if (spriteSheet.Animations != null)
			{
				Sprite.ClearAnimations();
				foreach (AnimationData animation in spriteSheet.Animations)
				{
					if (animation.NextAnimation != null)
						Sprite.AddAnimation(animation.Name, animation.X, animation.Y, animation.Width, animation.Height, animation.Frames, animation.FrameLength);
					else
						Sprite.AddAnimation(animation.Name, animation.X, animation.Y, animation.Width, animation.Height, animation.Frames, animation.FrameLength, animation.NextAnimation);
				}
			}
		}

		/// <summary>
		/// Changes the Tile Set currently in use
		/// </summary>
		/// <param name="nTileSet">Holds all required data to change tilesets</param>
		public static void ChangeTileSets(TileSetData nTileSet)
		{
			Tile.TileSetTexture = nTileSet.TileSet;
			Tile.tileWidth = nTileSet.tileWidth;
			Tile.tileHeight = nTileSet.tileHeight;
			Tile.TileStepX = nTileSet.tileStepX;
			Tile.TileStepY = nTileSet.tileStepY;
			Tile.OddRowXOffset = nTileSet.oddRowOffset;
			Tile.HeightTileOffset = nTileSet.heightTileOffset;
		}

		/// <summary>
		/// Changes the Tile Set currently in use
		/// </summary>
		/// <param name="TileSet">New TileSet Texture</param>
		/// <param name="tileWidth">Width of each Tile</param>
		/// <param name="tileHeight">Height of each Tile</param>
		/// <param name="tileStepX">Display step in the horizontal direction</param>
		/// <param name="tileStepY">Display step in the vertical direction</param>
		/// <param name="oddRowOffset">Horizontal offset for odd row tile placement</param>
		/// <param name="heightTileOffset">Vertical Offset for heighttile placement</param>
		public static void ChangeTileSets(Texture2D TileSet, int tileWidth, int tileHeight, int tileStepX, int tileStepY, int oddRowOffset, int heightTileOffset)
		{
			Tile.TileSetTexture = TileSet;
			Tile.tileWidth = tileWidth;
			Tile.tileHeight = tileHeight;
			Tile.TileStepX = tileStepX;
			Tile.TileStepY = tileStepY;
			Tile.OddRowXOffset = oddRowOffset;
			Tile.HeightTileOffset = heightTileOffset;
		}
	}
}
