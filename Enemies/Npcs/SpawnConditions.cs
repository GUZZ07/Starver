using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.Enemies.Npcs
{
	public enum SpawnConditions
	{
		None = 0,
		Day = 1 << 0,
		Night = 1 << 1,
		BloodMoon = 1 << 2 | Night,
		Eclipse = 1 << 3 | Day
	}
}
