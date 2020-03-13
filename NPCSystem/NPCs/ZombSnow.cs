using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.NPCSystem.NPCs
{
	public class ZombSnow : Zomb
	{
		#region Ctor
		public ZombSnow()
		{
			Types = null;
			RawType = Terraria.ID.NPCID.ZombieEskimo;
			Checker.Biome = BiomeType.Icy;
		}
		#endregion
		#region RealAI
		protected override void RealAI()
		{
			if (RealNPC.type != Terraria.ID.NPCID.ZombieEskimo)
			{
				RemoveFromNPCs();
			}
		}
		#endregion
	}
}
