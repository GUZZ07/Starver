using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Starvers.PlayerBoosts.Skills
{
	public class GaeBolg : StarverSkill
	{
		public GaeBolg() 
		{
			MPCost = 45;
			CD = 60 * 15;
			LevelNeed = 350;
			Author = "三叶草";
			Description = "发着一支速度极快的黎明";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			int damage = 122 + player.Level > 10000 ? (int)(122 * Math.Log(player.Level)) : 0;
			player.NewProj(vel * 10, ProjectileID.Daybreak, 130, 1);
			player.NewProj(player.Center + vel.ToLenOf(8), vel * 10, ProjectileID.Daybreak, damage, 1);
			player.NewProj(player.Center + vel.ToLenOf(16), vel * 10, ProjectileID.Daybreak, damage / 10, 1);
			player.NewProj(Vector2.Zero, ProjectileID.DD2ExplosiveTrapT3Explosion, damage / 2, 1);
			player.NewProj(Vector2.Zero, ProjectileID.SolarWhipSwordExplosion, damage / 2, 1);
		}
	}
}
