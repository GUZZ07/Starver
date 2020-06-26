using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TShockAPI;

namespace Starvers.PlayerBoosts
{
	public class EnderWand : StarverSkill
	{
		public EnderWand()
		{
			MPCost = 20;
			CD = 60 * 1;
			LevelNeed = 100;
			Author = "三叶草";
			Description = "跑路专用,往释放技能的方向高速移动";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			player.Velocity += vel * 4.25f;
		}
	}
}
