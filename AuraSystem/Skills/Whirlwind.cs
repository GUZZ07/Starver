using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Starvers.AuraSystem.Skills.Base;

namespace Starvers.AuraSystem.Skills
{
	public class Whirlwind : Skill
	{
		public Whirlwind() : base(SkillIDs.Whirlwind)
		{
			CD = 60 * 10;
			MP = 30;
			Level = 165;
			Author = "三叶草";
			Description = "制造一个攻击一群敌人的风暴";
			SetText();
		}
		public override void Release(StarverPlayer player, Vector2 vel)
		{
			int damage = 100 + (int)(100 * Math.Log(player.Level * player.Level / 2));
			player.NewProj(player.Center, vel * 10, 704, player.Level, 1);
			player.NewProj(player.Center, Vector2.Zero, 612, player.Level, 1);
		}
	}
}
