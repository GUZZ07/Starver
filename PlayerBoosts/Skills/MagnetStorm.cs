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
	public class MagnetStorm : StarverSkill
	{
		public MagnetStorm()
		{
			CD = 60 * 15;
			MPCost = 120;
			Author = "1413";
			Description = "制造若干个击向周围的磁球";
			LevelNeed = 155;
		}
		public override async void Release(StarverPlayer player, Vector vel)
		{
			await Task.Run(delegate
			{
				try
				{
					for (double d = 0; d < 2 * Math.PI; d += Math.PI / 9)
					{
						player.NewProj(player.FromPolar(d, 9f), ProjectileID.MagnetSphereBolt, 182, 0);
						Thread.Sleep(10);
					}
				}
				catch (Exception)
				{

				}
			});
		}
	}
}
