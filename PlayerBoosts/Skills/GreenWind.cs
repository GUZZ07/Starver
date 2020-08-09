using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.PlayerBoosts.Skills
{
	
	using Microsoft.Xna.Framework;
	using Terraria;
	using Terraria.ID;
	public class GreenWind : StarverSkill
	{
		private const float Radium = 16 * 90;
		public GreenWind()
		{
			CD = 60 * 12;
			MPCost = 150;
			LevelNeed = 2400;
			Author = "zhou_Qi";
			Description = @"向视野中的敌人发射孢子子弹
""子弹拥有着思维！这很神奇，不是吗？""
""我们需要去同情这些子弹在破碎时感受到的痛苦吗""？";
			Summary = "[2400][击败机械三王解锁]向全屏敌人释放追踪弹幕";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			int max = 3;
			max += (int)(1 * Math.Log(player.Level));
			int count = 0;
			foreach (var npc in Main.npc)
			{
				if (npc.active && Vector2.Distance(player.Center, npc.Center) < Radium)
				{
					LaunchTo(player, npc);
					if (++count == max)
					{
						break;
					}
				}
			}
		}
		private void LaunchTo(StarverPlayer player,NPC target)
		{
			Vector velocity = (Vector)(target.Center - player.Center);
			velocity.Length = 17.5f;
			Vector vertical = velocity.Vertical();
			vertical.Length = 16 * 2.5f;
			int damage = (int)(10 * (1 + Math.Log(player.Level)));
			player.ProjLine(player.Center + vertical, player.Center - vertical, velocity, 5, damage, ProjectileID.ChlorophyteBullet);
		}
	}
}
