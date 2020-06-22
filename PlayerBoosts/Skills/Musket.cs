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
			MPCost = 70;
			Description = @"这东西很没用?
怎么可能
这可是唯一一个无CD技能";
			LevelNeed = 50;
			Author = "三叶草";
		}
		public override void Release(StarverPlayer player)
		{
			Vector vel = Vector.FromPolar(player.ItemUseAngle, 16);
			Vector vertical = vel.ToVertical(1);
			for (int i = 0; i < 15; i++)
			{
				player.NewProj
				(
					vel + vertical * Rand.NextFloat(-3, 3),
					ProjectileID.GoldenBullet,
					45,
					1
				);
			}
		}
	}
}
