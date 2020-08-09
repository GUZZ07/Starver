using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Starvers.PlayerBoosts.Skills
{
	public class CDLess : StarverSkill
	{
		public CDLess()
		{
			Author = "1413";
			Description = "技能CD太长了？来试试他吧!\n10s内其他技能无CD";
			CD = 60 * 360;
			MPCost = 1000;
			LevelNeed = 5000;
			ForceCD = true;
			Summary = "[5000][击败石巨人解锁]让你使用技能后无需再等待";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			if (!player.IgnoreCD)
			{
				AsyncRelease(player);
			}
		}
		private async void AsyncRelease(StarverPlayer player)
		{
			player.IgnoreCD = true;
			for (int i = 0; i < player.Skills.Length; i++)
			{
				player.Skills[i].CD = 0;
			}
			await Task.Run(() =>
			{
				Thread.Sleep(10000);
			});
			player.IgnoreCD = false;
		}
	}
}
