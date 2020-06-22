using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Starvers.PlayerBoosts.Skills
{
	public class ExCalibur : StarverSkill
	{
		public ExCalibur()
		{
			MPCost = 2000;
			CD = 60 * 70;
			LevelNeed = 20000;
			Description = "三叶草制作的最强技能";
			Author = "三叶草";
		}
		public override void Release(StarverPlayer player)
		{
			Vector vel = Vector.FromPolar(player.ItemUseAngle, 12);
			player.ProjCircle(player.Center, 32, 16, ProjectileID.DD2SquireSonicBoom, 10, 1080);
			player.ProjSector(player.Center, 16, 16, vel.Angle, Math.PI / 4, 1310, ProjectileID.NebulaBlaze2, 3);
			player.ProjLine(player.Center, player.Center + player.FromPolar(vel.Angle, 48 * 20), vel.ToLenOf(24), 20, player.Level / 80 + 900, ProjectileID.SolarWhipSwordExplosion);
			Vector ver = vel.Vertical().ToLenOf(54);
			player.ProjLine(player.Center + ver, player.Center + player.FromPolar(vel.Angle, 48 * 20) + ver, vel.ToLenOf(24), 20, player.Level / 80 + 900, ProjectileID.SolarWhipSwordExplosion);
			player.ProjLine(player.Center - ver, player.Center + player.FromPolar(vel.Angle, 48 * 20) - ver, vel.ToLenOf(24), 20, player.Level / 80 + 900, ProjectileID.SolarWhipSwordExplosion);
			ver.Length = 84f;
			player.ProjLine(player.Center + ver, player.Center - ver, vel.ToLenOf(18f), 10, 2300, ProjectileID.TerraBeam);
		}
	}
}
