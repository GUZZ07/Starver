using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Starvers.AuraSystem.Skills.Base;

namespace Starvers.AuraSystem.Skills
{
	public class GaeBolg : Skill
	{
		public GaeBolg() : base(SkillIDs.GaeBolg)
		{
			BossBan = true;
			MP = 45;
			CD = 60 * 15;
			Level = 350;
			Author = "三叶草";
			Description = "发着一支速度极快的黎明";
			SetText();
		}
		public override void Release(StarverPlayer player, Vector2 vel)
		{
			int damage = 122 + player.Level > 10000 ? (int)(122 * Math.Log(player.Level)) : 0;
			player.NewProj(player.Center, vel * 10, 636, 130, 1);
			player.NewProj(player.Center + vel.ToLenOf(8f), vel * 10, 636, damage, 1);
			player.NewProj(player.Center + vel.ToLenOf(16f), vel * 10, 636, damage / 10, 1);
			player.NewProj(player.Center, Vector2.Zero, 696, damage / 2, 1);
			player.NewProj(player.Center, Vector2.Zero, 612, damage / 2, 1);
		}
	}
}
