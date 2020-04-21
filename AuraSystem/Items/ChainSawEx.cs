using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Microsoft.Xna.Framework;

namespace Starvers.AuraSystem.Items
{
	public abstract class ChainSawEx : StarverItem
	{
		protected ChainSawEx(int itemType) : base(itemType, 2000)
		{

		}
		public override void UseItem(StarverPlayer player)
		{

		}
		public override void ControlUseItem(StarverPlayer player)
		{
			if (player.Timer % 3 != 0)
			{
				return;
			}
			var size = player.HeldItem.Size;
			var angle = player.ItemUseAngle;

			var vector = Utils.FromPolar(angle, 1);
			var verticalVector = vector.Vertical();

			float scale = 8;
			if (player.Level > 5000)
			{
				scale = 16;
			}
			var length = scale * size.X;
			var width = scale * size.Y;
			foreach (var npc in Main.npc)
			{
				if (!npc.active || npc.townNPC || npc.friendly || npc.damage < 1)
				{
					continue;
				}
				var npcToPlayer = npc.Center - player.Center;
				var dotL = Vector2.Dot(npcToPlayer, vector);
				var dotW = Vector2.Dot(npcToPlayer, verticalVector);
				dotW = Math.Abs(dotW);
				if (0 <= dotL && dotL <= length && 0 <= dotW && dotW <= width / 2)
				{
					var dir = npc.Center.X > player.Center.X ? 1 : -1;
					var damage = player.HeldItemDamage;
					var knockback = player.HeldItemKnockback;
					var crit = Starver.Rand.Next(100) < player.MeleeCrit;
					npc.StrikeNPC(damage, knockback, dir, crit: crit, entity: player);
				}
			}
		}
	}
}
