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
	public class SporeSacEx : StarverAccessory
	{
		public SporeSacEx() : base(ItemID.SporeSac, 2000)
		{

		}

		public override void UpdateAccessory(StarverPlayer player)
		{
			const short shoot = ProjectileID.CrystalLeafShot;
			if (player.Timer % 150 == 0)
			{
				#region Level-Binded Datas
				NPC targetNPC = null;
				float oldDistance = 16 * 400;
				int max = 15;
				int damage = 100;
				float knockback = 1f;
				float maxDistance = 16 * 60;
				if (player.Level >= 8000)
				{
					max = 25;
					damage = 150;
					knockback = 4f;
					maxDistance = 16 * 90;
				}
				else if (player.Level >= 5000)
				{
					max = 20;
					damage = 125;
					knockback = 2f;
				}
				#endregion
				#region New Spore
				var count = max - Main.projectile.Count(proj =>
				{
					return
						proj.active &&
						proj.owner == player &&
						(proj.type == ProjectileID.SporeTrap || proj.type == ProjectileID.SporeTrap2);
				});
				for (int i = 0; i < count; i++)
				{
					var offset = Starver.Rand.NextVector2(16 * 60, 16 * 60);
					var velocity = Starver.Rand.NextVector2(0.75f);
					player.NewProj(player.Center + offset, velocity, ProjectileID.SporeTrap2, damage, knockback);
				}
				#endregion
				#region FindTarget
				foreach (var npc in Main.npc)
				{
					if (!npc.active || npc.friendly || npc.dontTakeDamage)
					{
						continue;
					}
					var distance = npc.Distance(player.Center);
					if (distance <= maxDistance)
					{
						if (targetNPC == null || distance < oldDistance)
						{
							targetNPC = npc;
							oldDistance = distance;
						}
					}
				}
				#endregion
				#region Extra Attack
				if (player.Level > 6000)
				{
					NPC targetNPC2 = null;
					oldDistance = 16 * 400;
					foreach (var npc in Main.npc)
					{
						if (!npc.active || npc.friendly || npc.dontTakeDamage || npc == targetNPC)
						{
							continue;
						}
						var distance = npc.Distance(player.Center);
						if (distance <= maxDistance)
						{
							if (targetNPC2 == null || distance < oldDistance)
							{
								targetNPC2 = npc;
								oldDistance = distance;
							}
						}
					}
					if (targetNPC2?.active == true)
					{
						foreach (var proj in Main.projectile)
						{
							if (proj.active && proj.owner == player)
							{
								switch (proj.type)
								{
									case ProjectileID.SporeTrap:
									case ProjectileID.SporeTrap2:
										var velocity = CalcVelocity(12, proj.Center, targetNPC2);
										player.NewProj(proj.Center, velocity.ToLenOf(13), shoot, proj.damage, proj.knockBack);
										break;
								}
							}
						}
					}
				}
				#endregion
				#region Normal Attack
				if (targetNPC?.active != true)
				{
					return;
				}
				foreach (var proj in Main.projectile)
				{
					if (proj.active && proj.owner == player)
					{
						switch (proj.type)
						{
							case ProjectileID.SporeTrap:
							case ProjectileID.SporeTrap2:
								var velocity = CalcVelocity(12, proj.Center, targetNPC);
								player.NewProj(proj.Center, velocity.ToLenOf(13), shoot, proj.damage, proj.knockBack);
								break;
						}
					}
				}
				#endregion
			}
		}
		private static Vector2 CalcVelocity(float speed, Vector2 start, NPC target)
		{
			if (speed < target.velocity.Length())
			{
				return (target.Center - start).ToLenOf(speed);
			}
			int time;
			Vector2 end = target.Center;
			Vector2 velocity = (target.Center - start).ToLenOf(speed);
			for (int i = 0; i < 20; i++)
			{
				time = (int)Math.Ceiling(Vector2.Distance(start, end) / speed);
				Vector2 newEnd = target.Center + time * target.velocity;
				velocity = (end - start).ToLenOf(speed);
				if (Vector2.Distance(end, newEnd) < 16)
				{
					return velocity;
				}
				end = newEnd;
			}
			return velocity;
		}
	}
}
