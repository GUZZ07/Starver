using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using OTAPI.Tile;
using Microsoft.Xna.Framework;

namespace Starvers.Tiles
{
	/// <summary>
	/// 用于保存一个TileSection的物块
	/// </summary>
	public class TileSectionData
	{
		private readonly ITile[] tiles;
		public int Width { get; }
		public int Height { get; }
		public TileSectionData(TileSection section)
		{
			Width = section.Width;
			Height = section.Height;
			tiles = new ITile[section.Width * section.Height];
			for (int i = 0; i < section.Width; i++)
			{
				for (int j = 0; j < section.Height; j++)
				{
					this[i, j] = new Tile(section[i, j]);
				}
			}
		}
		public ITile this[int x, int y]
		{
			get => tiles[x * Height + y];
			set => tiles[x * Height + y] = value;
		}
		public ITile this[Point point]
		{
			get => this[point.X, point.Y];
			set => this[point.X, point.Y] = value;
		}
		public ITile this[int index]
		{
			get => tiles[index];
			set => tiles[index] = value;
		}

	}
}
