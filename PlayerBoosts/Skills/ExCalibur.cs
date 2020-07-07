using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Starvers.PlayerBoosts.Skills
{
	public class ExCalibur : UltimateSkill
	{
		public ExCalibur()
		{
			MPCost = 2000;
			CD = 60 * 70;
			LevelNeed = 20000;
			Description = "三叶草制作的最强技能";
			Author = "三叶草";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			var damage = Math.Min(2000, player.Level / 80 + 900);
			player.ProjCircle(player.Center, 32, 16, ProjectileID.DD2SquireSonicBoom, 10, 1080);
			player.ProjSector(player.Center, 16, 16, vel.Angle, Math.PI / 4, 1310, ProjectileID.NebulaBlaze2, 3);
			player.ProjLine(player.Center, player.Center + player.FromPolar(vel.Angle, 48 * 20), vel.ToLenOf(24), 20, damage, ProjectileID.SolarWhipSwordExplosion);
			Vector vertical = vel.ToVertical(54);
			player.ProjLine(player.Center + vertical, player.Center + player.FromPolar(vel.Angle, 48 * 20) + vertical, vel.ToLenOf(24), 20, damage, ProjectileID.SolarWhipSwordExplosion);
			player.ProjLine(player.Center - vertical, player.Center + player.FromPolar(vel.Angle, 48 * 20) - vertical, vel.ToLenOf(24), 20, damage, ProjectileID.SolarWhipSwordExplosion);
			vertical.Length = 84f;
			player.ProjLine(player.Center + vertical, player.Center - vertical, vel.ToLenOf(18f), 10, 2300, ProjectileID.TerraBeam);
		}
	}
}
