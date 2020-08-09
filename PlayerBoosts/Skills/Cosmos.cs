using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Starvers.PlayerBoosts.Skills
{
	public class Cosmos : StarverSkill
	{
		public Cosmos()
		{
			LevelNeed = 2000;
			MPCost = 200;
			CD = 60 * 90;
			ForceCD = true;
			Author = "zhou_Qi";
			Description = @"重置绝大多数技能的冷却，使用者即刻致残
""在死亡的时间中积攒力量，这也不失为一种'秩序'""";
			Summary = "[2000][击败机械三王解锁]以血量为代价清空技能CD";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			for (int i = 0; i < player.Skills.Length; i++)
			{
				if (player.Skills[i].Skill.ForceCD)
				{
					continue;
				}
				player.Skills[i].CD = 0;
			}
			player.Life /= 3;
		}
	}
}
