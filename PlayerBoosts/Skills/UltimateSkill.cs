using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Starvers.PlayerBoosts.Skills
{
	public abstract class UltimateSkill : StarverSkill
	{
		public UltimateSkill()
		{
			ForceCD = true;
			CD = 60 * 70;
			MPCost = 900;
			Description = @"""我们对此一无所知""
""蕴含着最终的力量""";
			LevelNeed = 20000;
		}
		public sealed override void Release(StarverPlayer player, Vector direction)
		{
			player.BlockMPRegen(60 * 5);
			InternalRelease(player, direction);
		}
		protected abstract void InternalRelease(StarverPlayer player, Vector direction);
	}
}
