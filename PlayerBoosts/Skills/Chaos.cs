using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.PlayerBoosts.Skills
{
	
	using Microsoft.Xna.Framework;
	public class Chaos : StarverSkill
	{
		public Chaos()
		{
			LevelNeed = 5000;
			CD = 60 * 30;
			MPCost = 0;
			ForceCD = true;
			Author = "zhou_Qi";
			Description = @"随机释放出几个技能
""创造这个技能的人，恐怕只能用'懒惰'二字来形容了吧""";
			Summary = "[5000][击败石巨人解锁]随机释放几个技能";
		}

		public override bool CanSet(StarverPlayer player)
		{
			player.SendBlueText("该技能已被神秘力量封印");
			return false;
		}
		public unsafe override void Release(StarverPlayer player, Vector vel)
		{
			//if(player.mp != player.MaxMP)
			//{
			//	int slot = SkillManager.GetSlotByItemID(player.HeldItem.type) - 1;
			//	player.SendCombatText($"MP不足, 需要消耗{player.MaxMP}点MP", Color.HotPink);
			//	return;
			//}
			//player.MPCost = 0;
			int skillcount = Rand.Next(2, 6 + player.Level > 10000 ? 4 : 0);
			var span = stackalloc int[skillcount];
			for (int i = 0; i < skillcount; i++)
			{
				do
				{
					span[i] = Rand.Next(Starver.Instance.Skills.Count);
				}
				while (span[i] == ID);
			}
			for (int i = 0; i < skillcount; i++)
			{
				Starver.Instance.Skills[span[i]].Release(player, vel);
			}
		}
	}
}
