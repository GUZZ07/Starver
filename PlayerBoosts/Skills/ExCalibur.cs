using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Starvers.PlayerBoosts.Skills
{
	public class ExCalibur : UltimateSkill
	{
		public ExCalibur()
		{
			Description = "三叶草制作的最强技能";
			Author = "三叶草";
		}
		protected override void InternalRelease(StarverPlayer player, Vector vel)
		{
			Task.Run(() =>
			{
				try
				{
					var damage = Math.Min(2000, player.Level / 80 + 900);
					player.ProjCircle(player.Center, 32, 16, ProjectileID.DD2SquireSonicBoom, 10, 1080);
					player.ProjSector(player.Center, 16, 16, vel.Angle, Math.PI / 4, 1310, ProjectileID.NebulaBlaze2, 3);
					var vertical = vel.ToVertical(84);
					player.ProjLine(player.Center + vertical, player.Center - vertical, vel.ToLenOf(18f), 10, 2300, ProjectileID.TerraBeam);
					vertical.Length = 16 * 5;
					var center = player.Center;
					var go = vel.ToLenOf(16 * 1.5f);
					for (int i = 0; i < 9; i++)
					{
						player.ProjLine(center - vertical, center + vertical, default, 10, damage, ProjectileID.SolarWhipSwordExplosion);
						center += go;
						Thread.Sleep(1500 / 8);
					}
				}
				catch
				{

				}
			});
		}
	}
}
