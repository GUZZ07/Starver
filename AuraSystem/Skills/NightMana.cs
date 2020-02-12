using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Starvers.AuraSystem.Skills
{
	using Base;
    using Terraria.ID;

    public class NightMana : Skill
	{
		public NightMana()
		{
			MP = 200;
			CD = 60 * 35;
			Description = @"向前方发射黑暗能量
""相对应的，一些魔力甚至可以吞噬光线，因而也具有相当程度的威能""
""黑暗中摇曳的光明""";
			Author = "zhou_Qi";
			Level = 800;
			SetText();
		}
		public override void Release(StarverPlayer player, Vector2 vel)
		{
			player.SetBuff(BuffID.Obstructed, 60);
			Vector2 vertical = vel.Vertical();
			vertical.Normalize();
			int bolt = ProjectileID.BlackBolt;
			if (Starver.IsPE)
			{
				bolt = ProjectileID.RocketIII;
			}
			int t = Rand.Next(5, 9);
			int damage = (int)(100 * (1 + Math.Log(player.Level)));
			for (int i = 0; i < t; i++)
			{
				var offset = Utils.FromPolar(Rand.NextDouble(-Math.PI / 12, Math.PI / 12), Rand.NextFloat(16 * 0f, 16 * 3.5f));
				player.NewProj(player.Center + offset, vel + vertical * Rand.NextFloat(0, 5), bolt, damage);
			}
		}
	}
}
