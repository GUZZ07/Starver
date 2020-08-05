using Starvers.PlayerBoosts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Starvers.Events
{
	public class PreReleaseSkillEventArgs
	{
		public StarverSkill Skill { get; set; }
		public int MPCost { get; set; }
		/// <summary>
		/// 下一次使用该槽位技能的CD
		/// </summary>
		public int CD { get; set; }
		public StarverPlayer Player { get; }
		public PreReleaseSkillEventArgs(StarverPlayer player, StarverSkill skill, int mpCost, int cd)
		{
			Player = player;
			Skill = skill;
			MPCost = mpCost;
			CD = cd;
		}
	}
}
