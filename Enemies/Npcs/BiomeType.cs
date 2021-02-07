using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.Enemies.Npcs
{
	[Flags]
	public enum BiomeType
	{
		None = 0,
		Grass = 1 << 0,
		Corrupt = 1 << 1 | Evil,
		Crimson = 1 << 2 | Evil,
		Jungle = 1 << 3,
		Dungeon = 1 << 4,
		Tower = 1 << 5,
		TowerVortex = Tower | 1 << 6,
		TowerStardust = Tower | 1 << 7,
		TowerNebula = Tower | 1 << 8,
		TowerSolar = Tower | 1 << 9,
		Beach = 1 << 10 | Grass,
		Sky = 1 << 11,
		Hell = 1 << 12,
		Holy = 1 << 13,
		UnderGround = 1 << 14,
		Rain = 1 << 15,
		Metor = 1 << 16,
		Temple = Jungle | 1 << 17,
		Dessert = 1 << 18,
		Icy = 1 << 19,
		Evil = 1 << 20,
	}
}
