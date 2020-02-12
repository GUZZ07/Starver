using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.AuraSystem.Skills
{
	using Base;
	using Microsoft.Xna.Framework;
	using System.Threading;
	using Terraria;
	using Terraria.ID;

	public class GreenCrit : Skill
	{
		public GreenCrit() : base(SkillIDs.GreenCrit)
		{
			CD = 60 * 300;
			MP = 25000;
			Level = 200000;
		}
		public override void Release(StarverPlayer player, Vector2 vel)
		{
			int damage = 1000 + (int)(Math.Sqrt(player.Level) * 5);
			for (int i = 0; i < 250; i++)
			{
				player.NewProj(player.Center, vel * 5, ProjectileID.FlamingJack, damage);
			}
		}
	}
}
