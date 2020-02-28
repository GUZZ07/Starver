using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.AuraSystem.Skills
{
	using Base;
	using Microsoft.Xna.Framework;
	public class Chaos : Skill
	{
		public Chaos() : base(SkillIDs.Chaos)
		{
			Level = 4000;
			CD = 60 * 30;
			MP = 0;
			ForceCD = true;
			Author = "zhou_Qi";
			Description = @"随机释放出几个技能
""创造这个技能的人，恐怕只能用'懒惰'二字来形容了吧""";
			SetText();
		}
		public unsafe override void Release(StarverPlayer player, Vector2 vel)
		{
			if(player.mp != player.MaxMP)
			{
				int slot = SkillManager.GetSlotByItemID(player.HeldItem.type) - 1;
				player.CDs[slot] -= CD;
				player.SendCombatMsg($"MP不足, 需要消耗{player.MaxMP}点MP", Color.HotPink);
				return;
			}
			player.mp = 0;
			int skillcount = Rand.Next(2, 6 + player.Level > 10000 ? 4 : 0);
			var span = stackalloc int[skillcount];
			for (int i = 0; i < skillcount; i++)
			{
				do
				{
					span[i] = Rand.Next(SkillManager.Skills.Length);
				}
				while (span[i] == Index);
			}
			for (int i = 0; i < skillcount; i++)
			{
				SkillManager.Skills[span[i]].Release(player, vel);
			}
		}
	}
}
