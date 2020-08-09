using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Starvers.PlayerBoosts.Skills
{
	public class LimitlessSpark : StarverSkill
	{
		public LimitlessSpark()
		{
			MPCost = 1000;
			CD = 60 * 30;
			Author = "wither";
			LevelNeed = 1500;
			Description = "耗光你所有的MP,制造咒火团";
			Summary = "[1500][击败血肉之墙解锁]用全部的MP发射咒火团";
		}
		public override bool CanSet(StarverPlayer player)
		{
			player.SendBlueText("该技能还没做好");
			return false;
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			//int damage = (int)(10 * Math.Log10(player.Level) + Math.Min(32567, player.MP / 10)) + 200;
			//int n = (int)(10 * Math.Log(player.MP) + 10);
			//player.MPCost = 0;
			//for (int i = 0; i < n; i++)
			//{
			//	player.NewProj(player.Center,  player.FromPolar(vel.Angle + Rand.NextDouble(-Math.PI / 6, Math.PI / 6), 29), ProjectileID.CursedFlameFriendly, damage, 20f);
			//}
		}
	}
}
