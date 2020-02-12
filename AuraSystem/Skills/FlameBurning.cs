using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Starvers.AuraSystem.Skills.Base;
using Terraria.ID;

namespace Starvers.AuraSystem.Skills
{
	public class FlameBurning:Skill
	{
		public FlameBurning():base(SkillIDs.FlameBurning)
		{
			CD = 60 * 6;
			MP = 20;
			Author = "麦克虚妄";
			Description = "制造一片扇形的火焰";
			Level = 20;
			SetText();
		}
		public override void Release(StarverPlayer player, Vector2 vel)
		{
			double dirold = Math.Atan2(vel.Y, vel.X);
			for (double d = dirold - Math.PI / 12; d < dirold + Math.PI / 12; d += Math.PI / 24)
			{
				player.ProjLine(player.Center, player.Center +  player.FromPolar(d, 16 * 50), Vector2.Zero, 45,80, ProjectileID.Flames);
				Thread.Sleep(20);
			}
		}
	}
}
