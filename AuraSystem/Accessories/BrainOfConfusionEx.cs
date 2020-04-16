using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace Starvers.AuraSystem.Accessories
{
	public class BrainOfConfusionEx : StarverAccessory
	{
		public BrainOfConfusionEx() : base(ItemID.BrainOfConfusion, 100)
		{

		}
		public override void UpdateAccessory(StarverPlayer player)
		{
			foreach (var npc in Main.npc)
			{
				if (!npc.active || npc.friendly || npc.townNPC || npc.damage < 1)
				{
					continue;
				}
				float distance = 16 * 25;
				if (player.Level > 500)
				{
					distance = 16 * 40;
				}
				if (npc.Distance(player.Center) > distance)
				{
					continue;
				}
				if (!npc.HasBuff(BuffID.Confused))
				{
					npc.AddBuff(BuffID.Confused, 60 * 60, true);
					StarverPlayer.All.SendData(PacketTypes.NpcUpdateBuff, "", npc.whoAmI);
				}
			}
		}
	}
}
