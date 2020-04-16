using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace Starvers.AuraSystem.Accessories
{
	public class FrozenWingsEx : StarverAccessory
	{
		public FrozenWingsEx() : base(ItemID.FrozenWings, 500)
		{

		}
		public override void UpdateAccessory(StarverPlayer player)
		{
			var distance = 16 * 30;
			if (player.Level >= 1500)
			{
				distance = 16 * 45;
			}
			foreach (var npc in Main.npc)
			{
				if (!npc.active || npc.townNPC || npc.friendly || npc.damage < 1)
				{
					continue;
				}
				if (npc.Distance(player.Center) > distance)
				{
					continue;
				}
				npc.AddBuffIfNot(BuffID.Frostburn);
			}
		}
	}
}
