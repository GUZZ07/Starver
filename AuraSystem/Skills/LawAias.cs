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
	public class LawAias : Skill
	{
		public LawAias() : base(SkillIDs.LawAias)
		{
			MP = 1500;
			CD = 60 * 50;
			Level = 5000;
			Author = "三叶草";
			Description = @"这是三叶草没做好的技能
但是1413完成这个技能的同时偏离了三叶草的原版设计意图
以释放者为圆心制造一个由弹幕组成的逐渐缩小的圆
强度随等级飞跃而飞跃";
			SetText();
		}
		public override void Release(StarverPlayer player, Vector2 vel)
		{
			int proj = ProjectileID.DemonScythe;
			if(player.Level > 10000)
			{
				proj = ProjectileID.NebulaArcanum;
			}
			else if(player.Level > 7500)
			{
				proj = ProjectileID.Typhoon;
			}
			player.ProjCircle(player.Center, 16 * 40, -15, proj, 25, 200 + (int)(200 * Math.Log(player.Level)));
		}
	}
}
