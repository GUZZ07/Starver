using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Starvers.AuraSystem.Skills.Base;

namespace Starvers.AuraSystem.Skills
{
	public class Sacrifice : Skill
	{
		public Sacrifice() : base(SkillIDs.Sacrifice)
		{
			MP = 0;
			CD = 60 * 20;
			Author = "三叶草";
			Description = @"来源未知的古老秘术
血量减少最大血量的一半,回复你所有的mp";
			Level = 10;
			SetText();
		}
		public override void Release(StarverPlayer player, Vector2 vel)
		{
			player.MP = player.MaxMP;
			player.TPlayer.statLife /= 2;
			if (player.TPlayer.statLife < 1)
			{
				player.TSPlayer.KillPlayer();
			}
			else
			{
				player.SendData(PacketTypes.PlayerHp, "", player.Index);
			}
		}
	}
}
