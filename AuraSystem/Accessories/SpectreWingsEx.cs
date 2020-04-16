using Microsoft.Xna.Framework;
using Starvers.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace Starvers.AuraSystem.Accessories
{
	public class SpectreWingsEx : StarverAccessory
	{
		public SpectreWingsEx() : base(ItemID.GhostWings, 1500)
		{

		}
		public override void UpdateAccessory(StarverPlayer player)
		{

		}
		public override void PreStrike(StarverPlayer player, NPCStrikeEventArgs args)
		{
			int max = 1;
			if (player.Level > 8000)
			{
				max = 3;
			}
			else if (player.Level > 4000)
			{
				max = 2;
			}
			int damage = Math.Min(args.RawDamage, args.RealDamage);
			for (int i = 0; i < max; i++)
			{
				int heal = Math.Min(10, damage / 50);
				heal *= 10;
				heal += Starver.Rand.Next(-5, 5);
				if (heal < 10)
				{
					return;
				}
				Vector2 offset = Starver.Rand.NextVector2(args.NPC.width, args.NPC.height);
				int idx = player.NewProj(args.NPC.Center + offset, default, ProjectileID.SpiritHeal, 0, 0);
				Main.projectile[idx].netImportant = true;
				Main.projectile[idx].ai[0] = player;
				Main.projectile[idx].ai[1] = heal;
				Main.projectile[idx].SendData();
			}
			base.PreStrike(player, args);
		}
	}
}
