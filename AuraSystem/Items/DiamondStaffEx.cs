using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace Starvers.AuraSystem.Items
{
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public class DiamondStaffEx : StarverItem
	{
		public DiamondStaffEx() : base(ItemID.DiamondStaff, 100)
		{

		}
		public override void UseItem(StarverPlayer player)
		{
			Vector offset = ItemOffset(player);
			Vector velocity = offset;
			velocity.Length = 13;
			var damage = player.HeldItemDamage * 2;
			var pos = player.Center + offset;
			int idx = player.NewProj(pos, velocity, ProjectileID.StarWrath, damage, player.HeldItemKnockback);
			Terraria.Main.projectile[idx].magic = true;
		}
	}
}
