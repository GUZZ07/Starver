using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.AuraSystem.Skills
{
	using Base;
	using Microsoft.Xna.Framework;

	public class Cosmos : Skill
	{
		public Cosmos() : base(SkillIDs.Cosmos)
		{
			Level = 1000;
			MP = 200;
			CD = 60 * 60;
			ForceCD = true;
			Author = "zhou_Qi";
			Description = @"重置绝大多数技能的冷却，使用者即刻死亡
""在死亡的时间中积攒力量，这也不失为一种'秩序'""";
			SetText();
		}
		public override void Release(StarverPlayer player, Vector2 vel)
		{
			for (int i = 0; i < player.CDs.Length; i++)
			{
				if (player.GetSkill(i).ForceCD)
				{
					continue;
				}
				player.CDs[i] = 0;
			}
			player.Life /= 2;
		}
	}
}
