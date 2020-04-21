using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria.ID;

namespace Starvers.AuraSystem.Items
{
	public class CrystalSerpentEx : StarverItem
	{
		public CrystalSerpentEx() : base(ItemID.CrystalSerpent, 2000, 200 * 7 / 1000 * 60)
		{
			
		}
		public async override void UseItem(StarverPlayer player)
		{
			await Task.Run(() =>
			{
				try
				{
					const short shoot = ProjectileID.CrystalPulse;
					var offset = ItemOffset(player);
					var velocity = offset;
					var damage = player.HeldItemDamage * 2;

					velocity.Length = player.HeldItem.shootSpeed;
					offset *= 0.5f;
					var accel = offset.Deflect(-Math.PI / 2) * 0.75f;
					offset *= 3.5f;
					var pos = player.Center;
					var pos2 = player.Center;
					var velocityY = accel * 0;
					for (int i = 0; i < 7; i++)
					{
						pos += offset;
						pos2 += offset;
						pos += velocityY;
						pos2 -= velocityY;
						player.NewProj(pos, velocity, shoot, damage, player.HeldItemKnockback);
						player.NewProj(pos2, velocity, shoot, damage, player.HeldItemKnockback);
						velocityY += accel;
						Thread.Sleep(200);
						pos += velocity * 200 * 60 / 1000;
						pos2 += velocity * 200 * 60 / 1000;
					}
				}
				catch
				{

				}
			});
		}
	}
}
