using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Starvers.PlayerBoosts.Skills
{
	public class Whirlwind : StarverSkill
	{
		public Whirlwind()
		{
			CD = 60 * 10;
			MPCost = 30;
			LevelNeed = 165;
			Author = "三叶草";
			Description = "制造一个攻击一群敌人的风暴";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			int damage = 100 + (int)(100 * Math.Log(player.Level * player.Level / 2));
			player.NewProj(vel * 10, ProjectileID.DD2ApprenticeStorm, damage, 1);
			player.NewProj(Vector2.Zero, ProjectileID.SolarWhipSwordExplosion, damage, 1);
		}
	}
}
