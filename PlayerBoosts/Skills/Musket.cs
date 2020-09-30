using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Starvers.PlayerBoosts.Skills
{
	public class Musket : StarverSkill
	{
		public Musket()
		{
			CD = 60 * 0;
			MPCost = 60;
			Description = @"这东西很没用?
怎么可能
这可是唯一一个无CD技能";
			LevelNeed = 50;
			Author = "三叶草";
			Summary = "[50][默认解锁]无CD，释放激光弹幕";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			Vector vertical = vel.ToVertical(1);
			int count = 6 + 2 * (int)Math.Log(player.Level);
			int damage = player.Level > 2000 ? 28 : 40;
			for (int i = 0; i < count; i++)
			{
				player.NewProj
				(
					vel + vertical * Rand.NextFloat(-3, 3),
					ProjectileID.GoldenBullet,
					damage,
					1
				);
			}
		}
	}
}
