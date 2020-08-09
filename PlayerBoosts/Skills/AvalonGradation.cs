using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Starvers.PlayerBoosts.Skills
{
	public class AvalonGradation : StarverSkill
	{
		public const float R = 16 * 80;
		public static void Update(StarverPlayer player)
		{
			if (player.AvalonGradationTime <= 0)
			{
				return;
			}
			player.AvalonGradationTime--;
			foreach (var proj in Main.projectile)
			{
				if ( !proj.active)
				{
					continue;
				}
				if (proj.friendly == false && Vector2.Distance(player.Center, proj.Center) < R)
				{
					proj.active = false;
					proj.type = 0;
					proj.SendData();
				}
			}
		}
		public AvalonGradation()
		{
			CD = 60 * 50;
			MPCost = 300;
			LevelNeed = 2800;
			Author = "1413";
			Description = "消除你身边的大部分敌对弹幕,持续10s";
			Summary = "[2800][击败机械三王解锁]持续消除你周围的威胁";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			player.SendText("你得到了来自幻想乡的庇护", 115,74,18);
			player.AvalonGradationTime += 60 * 10;
		}
		public override bool CanSet(StarverPlayer player)
		{
			if (!(NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3))
			{
				player.SendText("该技能已被三位机械头目共同封印", 192, 192, 192);
				return false;
			}
			return base.CanSet(player);
		}
	}
}
