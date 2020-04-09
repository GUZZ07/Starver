using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria;

namespace Starvers.AuraSystem.Items
{
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public class ShadowBeamStaffEx : StarverItem
	{
		public ShadowBeamStaffEx() : base(ItemID.ShadowbeamStaff, 1500)
		{

		}
		public override void UseItem(StarverPlayer player)
		{
			Vector offset = ItemOffset(player);
			Vector velocity = offset;
			velocity.Length = player.HeldItem.shootSpeed;
			const short shoot = ProjectileID.WaterBolt;
			var pos = player.Center + offset;
			var damage = player.HeldItemDamage * 2;
			player.ProjSector(pos, velocity.Length, 0, velocity.Angle, Math.PI / 3, damage, shoot, 4);
		}
	}
}
