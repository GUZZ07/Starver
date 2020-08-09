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
	public class FlameBurning : StarverSkill
	{
		public FlameBurning()
		{
			CD = 60 * 10;
			MPCost = 80;
			Author = "麦克虚妄";
			Description = "制造一片扇形的火焰";
			LevelNeed = 100;
			Summary = "[100][击败克眼解锁]释放穿透火焰";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			double dirold = vel.Angle;
			for (double d = dirold - Math.PI / 12; d < dirold + Math.PI / 12; d += Math.PI / 24)
			{
				player.ProjLine(player.Center, player.Center +  player.FromPolar(d, 16 * 50), Vector2.Zero, 45,80, ProjectileID.Flames);
				Thread.Sleep(20);
			}
		}
	}
}
