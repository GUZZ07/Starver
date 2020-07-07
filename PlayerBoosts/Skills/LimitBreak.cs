using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.PlayerBoosts.Skills
{
	
	using Microsoft.Xna.Framework;

	public class LimitBreak : StarverSkill
	{
		public LimitBreak()
		{
			CD = 60 * 30;
			LevelNeed = 20;
			MPCost = 80;
			Author = "zhou_Qi";
			Description = @"获得一个向上的足以直达太空的速度
""七点九""
""与传送门搭配效果更佳""";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			player.Velocity += new Vector2(0, -170);
		}
	}
}
