using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Starvers.PlayerBoosts.Skills
{
	public class LawAias : StarverSkill
	{
		public LawAias() 
		{
			MPCost = 1500;
			CD = 60 * 80;
			LevelNeed = 4000;
			Author = "三叶草";
			Description = @"这是三叶草没做好的技能
但是1413完成这个技能的同时偏离了三叶草的原版设计意图
以释放者为圆心制造一个由弹幕组成的逐渐缩小的圆
强度随等级飞跃而飞跃";
			Summary = "[4000][击败石巨人解锁][可成长]生成一圈环形收拢弹幕";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			int proj = ProjectileID.DemonScythe;
			if(player.Level > 10000)
			{
				proj = ProjectileID.NebulaArcanum;
			}
			else if(player.Level > 5000)
			{
				proj = ProjectileID.Typhoon;
			}
			player.ProjCircle(player.Center, 16 * 40, -15, proj, 15, 100 + (int)(100 * Math.Log(player.Level)));
			if(player.Level > 15000)
			{
				player.ProjCircle(player.Center, 16 * 35, -15, proj, 10, 100 + (int)(100 * Math.Log(player.Level)));
			}
		}
	}
}
