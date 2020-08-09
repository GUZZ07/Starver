using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TShockAPI;

namespace Starvers.PlayerBoosts.Skills
{
	public class EnderWand : StarverSkill
	{
		public EnderWand()
		{
			MPCost = 50;
			CD = 60 * 2;
			LevelNeed = 150;
			Author = "三叶草";
			Description = "跑路专用,往释放技能的方向高速移动";
			Summary = "[150][击败蠕虫/大脑解锁]给予你一个指向速度";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			player.Velocity += vel.ToLenOf(54);
		}
	}
}
