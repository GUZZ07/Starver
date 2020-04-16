using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace Starvers.AuraSystem.Accessories
{
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public class HivePackEx : StarverAccessory
	{
		public HivePackEx() : base(ItemID.HiveBackpack, 50)
		{

		}
		public override void UpdateAccessory(StarverPlayer player)
		{

		}
		public override void OnUseItem(StarverPlayer player)
		{
			if (player.HeldItem.useAmmo == AmmoID.Arrow)
			{
				var offset = Vector.FromPolar(player.ItemUseAngle, player.HeldItem.height);
				var velocity = Vector.FromPolar(player.ItemUseAngle, player.ArrowSpeed);
				var damage = player.HeldItemDamage;
				var knockback = player.HeldItemKnockback;
				player.NewProj(player.Center + offset, velocity, ProjectileID.BeeArrow, damage, knockback);
			}
			base.OnUseItem(player);
		}
	}
}
