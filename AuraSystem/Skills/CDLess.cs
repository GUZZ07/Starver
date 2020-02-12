using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Starvers.AuraSystem.Skills.Base;

namespace Starvers.AuraSystem.Skills
{
	public class CDLess : Skill
	{
		public CDLess() : base(SkillIDs.CDLess)
		{
			Author = "1413";
			Description = "技能CD太长了？来试试他吧!\n10s内其他技能无CD";
			CD = 60 * 360;
			MP = 20;
			Level = 500;
			ForceCD = true;
			SetText();
		}
		public override void Release(StarverPlayer player, Vector2 vel)
		{
			AsyncRelease(player);
		}
		private async void AsyncRelease(StarverPlayer player)
		{
			player.IgnoreCD = true;
			for (int i = 0; i < player.CDs.Length; i++)
			{
				player.CDs[i] = 0;
			}
			await Task.Run(() =>
			{
				Thread.Sleep(10000);
			});
			player.IgnoreCD = false;
		}
	}
}
