using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Starvers.PlayerBoosts.Skills
{
	public class JusticeFromSky : StarverSkill
	{
		public JusticeFromSky()
		{
			CD = 60 * 13;
			MPCost = 100;
			LevelNeed = 350;
			Author = "1413";
			Description = @"""其实这只是饥饿的轰炸炸错了位置""";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			Vector2 Start = player.Center + new Vector2(-Math.Min(player.Level, 348 * 10) - 48 * 10, -16 * 40);
			Vector2 End = player.Center + new Vector2(Math.Min(player.Level, 48 * 10) + 48 * 10, -16 * 40);
			Vector2 down = new Vector2(0, 16);
			player.ProjLine(Start, End, down, Math.Max(Math.Min(player.Level / 48,10), 10) * 2 + 20, 400, ProjectileID.RocketSnowmanIII);
		}
	}
}
