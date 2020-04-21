using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace Starvers.AuraSystem.Items
{
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public class BlizzardStuffEx : StarverItem
	{
		public BlizzardStuffEx() : base(ItemID.BlizzardStaff, 5000)
		{

		}
		public override void UseItem(StarverPlayer player)
		{
			var damage = player.HeldItemDamage * 3 / 2;
			var knockback = player.HeldItemKnockback;
			var vector = ItemOffset(player);
			var targetPos = player.Center + vector * 20;
			var max = 1;
			if (player.Level > 8000)
			{
				max = 3;
			}
			else if (player.Level > 6500)
			{
				max = 2;
			}
			for (int i = 0; i < max; i++)
			{
				var start = player.Center + Starver.Rand.NextVector2(16 * 50, 16 * 50);
				vector = (Vector)(start - targetPos);
				vector.Length = -player.HeldItem.shootSpeed;

				player.NewProj(start, vector, ProjectileID.Blizzard, damage, knockback, 0, Starver.Rand.Next(5));
			}
		}
	}
}
