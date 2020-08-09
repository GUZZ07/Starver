using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Starvers.PlayerBoosts.Skills
{
	public class PosionFog : StarverSkill
	{
		public PosionFog()
		{
			MPCost = 170;
			LevelNeed = 1500;
			CD = 60 * 10;
			Author = "Deaths";
			Description = "制造一片毒雾";
			Summary = "[1500][击败血肉之墙解锁]在身边生成大量毒雾";
		}
		public override async void Release(StarverPlayer player, Vector vel)
		{
			await Task.Run(delegate
			{
				try
				{
					for (int i = 0; i < 8; i++)
					{
						player.ProjCircle(player.Center, 32 + 34 * i, 1, ProjectileID.ToxicCloud, 10 + 5 * i, 103);
						Thread.Sleep(1000 / 60);
					}
				}
				catch (Exception)
				{

				}
			});
		}
		public override bool CanSet(StarverPlayer player)
		{
			if (!Main.hardMode)
			{
				player.SendText("该技能已被血肉之墙封印", 220, 20, 60);
				return false;
			}
			return base.CanSet(player);
		}
	}
}


