using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaApi.Server;

namespace Starvers.Events
{
	public class PreNPCStrikeEventArgs
	{
		public double DIndexLevel { get; set; }
		public double DIndexMP { get; set; }
		public StarverPlayer Player { get; }
		public NpcStrikeEventArgs RawArgs { get; }
		public PreNPCStrikeEventArgs(NpcStrikeEventArgs rawArgs, StarverPlayer player, double dIndexLevel, double dIndexMP)
		{
			RawArgs = rawArgs;
			Player = player;
			DIndexLevel = dIndexLevel;
			DIndexMP = dIndexMP;
		}
	}
}
