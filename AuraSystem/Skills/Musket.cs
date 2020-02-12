using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Starvers.AuraSystem.Skills.Base;
using Terraria.ID;

namespace Starvers.AuraSystem.Skills
{
	public class Musket:Skill
	{
		public Musket() : base(SkillIDs.Musket)
		{
			CD = 60 * 0;
			MP = 70;
			Description = @"这东西很没用?
怎么可能
这可是唯一一个无CD技能";
			Level = 50;
			Author = "三叶草";
			BossBan = true;
			SetText();
		}
		public override void Release(StarverPlayer player, Vector2 vel)
		{
			for (int i = 0; i < 15; i++)
			{
				player.NewProj(
					player.TPlayer.position + new Vector2(Rand.Next(3), Rand.Next(3)),
					(vel + new Vector2(Rand.Next(2) - 1, Rand.Next(2) - 1)) * 4,
					ProjectileID.GoldenBullet,
					45,
					1
					);
			}
		}
	}
}
