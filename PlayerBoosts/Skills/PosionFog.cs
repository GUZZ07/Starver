using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Starvers.PlayerBoosts.Skills
{
	public class PosionFog : StarverSkill
	{
		public PosionFog()
		{
			MPCost = 190;
			LevelNeed = 400;
			CD = 60 * 15;
			Author = "Deaths";
			Description = "制造一片毒雾";
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
	}
}


