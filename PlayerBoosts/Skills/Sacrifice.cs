using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Starvers.PlayerBoosts.Skills
{
	public class Sacrifice : StarverSkill
	{
		public Sacrifice()
		{
			MPCost = 0;
			CD = 60 * 20;
			Author = "三叶草";
			Description = @"来源未知的古老秘术
血量减少最大血量的一半,回复你所有的mp";
			LevelNeed = 10;
		}
		public override bool CanSet(StarverPlayer player)
		{
			player.SendBlueText("该技能已被神秘力量封印");
			return false;
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			//player.MPCost = player.MaxMP;
			//player.TPlayer.statLife /= 2;
			//if (player.TPlayer.statLife < 1)
			//{
			//	player.TSPlayer.KillPlayer();
			//}
			//else
			//{
			//	player.SendData(PacketTypes.PlayerHp, "", player.Index);
			//}
		}
	}
}
