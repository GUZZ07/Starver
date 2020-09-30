using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Starvers.PlayerBoosts.Skills
{
    using Terraria;
    using Terraria.ID;

    public class NightMana : StarverSkill
	{
		public NightMana()
		{
			MPCost = 140;
			CD = 60 * 25;
			Description = @"向前方发射黑暗能量
""相对应的，一些魔力甚至可以吞噬光线，因而也具有相当程度的威能""
""黑暗中摇曳的光明""";
			Author = "zhou_Qi";
			LevelNeed = 1200;
			Summary = "[1200][击败血肉之墙解锁]以暂时失明的代价发射强大的黑暗弹幕";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			player.SetBuff(BuffID.Obstructed, 60);
			Vector2 vertical = vel.Vertical();
			vertical.Normalize();
			int bolt = ProjectileID.BlackBolt;
			int t = Rand.Next(5, 9);
			int damage = (int)(100 * (1 + Math.Log(player.Level)));
			for (int i = 0; i < t; i++)
			{
				var offset = Vector.FromPolar(Rand.NextDouble(-Math.PI / 12, Math.PI / 12), Rand.NextFloat(16 * 0f, 16 * 3.5f));
				player.NewProj(player.Center + offset, vel + vertical * Rand.NextFloat(0, 5), bolt, damage);
			}
		}
		public override bool CanSet(StarverPlayer player)
		{
			if (!Main.hardMode)
			{
				player.SendText("该技能已被血肉之墙封印", 220, 20, 60);
				return false;
			}
			return base.CanSet(player);
		}
	}
}
