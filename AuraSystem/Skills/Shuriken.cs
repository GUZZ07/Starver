using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Starvers.AuraSystem.Skills.Base;

namespace Starvers.AuraSystem.Skills
{
	public class Shuriken:Skill
	{
		protected Vector2 Pos;
		public Shuriken():base(SkillIDs.Shuriken)
		{
			MP = 8;
			CD = 60 * 1;
			Author = "三叶草";
			Description = @"你的初始技能, mp消耗低, CD短
显然最适合新手
但不适合老手, 你可能会因为这招被其他人打";
			SetText();
		}
		public override void Release(StarverPlayer player, Vector2 vel)
		{
			Pos = player.Center;
			int level = player.Level;
			for (int i = 0; i < (player.Level / 50 + 10 <= 50 ? player.Level / 50 + 10 : 50); i++)
			{
				player.NewProj(Pos, vel * 5, 3, level / 10 + 5, 1);
				Pos.X += 20;
			}
		}
	}
}
