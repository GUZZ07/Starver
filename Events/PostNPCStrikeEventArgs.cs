using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaApi.Server;

namespace Starvers.Events
{
	public class PostNPCStrikeEventArgs
	{
		public int ExpGet { get; }
		public StarverPlayer Player { get; }
		public NpcStrikeEventArgs RawArgs { get; }
		public int RealDamage { get; }
		public PostNPCStrikeEventArgs(NpcStrikeEventArgs rawArgs, StarverPlayer player, int realDamage, int expGet)
		{
			RawArgs = rawArgs;
			RealDamage = realDamage;
			ExpGet = expGet;
			Player = player;
		}
	}
}
