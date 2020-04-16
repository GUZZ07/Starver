using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace Starvers.AuraSystem.Accessories
{
	public class StarVeilEx : StarverAccessory
	{
		public StarVeilEx() : base(ItemID.StarVeil, 300)
		{
			
		}
		public override void UpdateAccessory(StarverPlayer player)
		{

		}
		public override void OnDamaged(StarverPlayer player, int damage, bool crit, bool pvp)
		{
			if (Main.projectile.Count(proj => proj.active && proj.owner == player && proj.type == ProjectileID.StarWrath) >= 20 * 2 * 2 / 4)
			{
				return;
			}
			Vector2 posL = player.Center;
			Vector2 posR = player.Center;
			posL.Y -= 30 * 16;
			posR.Y -= 30 * 16;
			Vector2 offsetL = new Vector2(-16 * 4, -14 * 7.5f);
			Vector2 offsetR = new Vector2(16 * 4, -14 * 7.5f);
			Vector2 down = new Vector2(0, 12);
			for (int i = 0; i < (20 * 2 / 4); i++)
			{
				player.NewProj(posL, down, ProjectileID.StarWrath, 250, 0);
				player.NewProj(posR, down, ProjectileID.StarWrath, 250, 0);
				posL += offsetL;
				posR += offsetR;
			}
			player.AddBuffIfNot(BuffID.ShadowDodge);
			base.OnDamaged(player, damage, crit, pvp);
		}
	}
}
