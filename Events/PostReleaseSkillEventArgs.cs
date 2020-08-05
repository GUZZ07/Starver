using Starvers.PlayerBoosts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Starvers.Events
{
	public class PostReleaseSkillEventArgs
	{
		public StarverSkill Skill { get; }
		public int MPCost { get; }
		/// <summary>
		/// 玩家技能CD
		/// </summary>
		public int CD { get; set; }
		public StarverPlayer Player { get; }
		public PostReleaseSkillEventArgs(StarverPlayer player, StarverSkill skill, int mpCost, int cd)
		{
			Player = player;
			Skill = skill;
			MPCost = mpCost;
			CD = cd;
		}
	}
}
