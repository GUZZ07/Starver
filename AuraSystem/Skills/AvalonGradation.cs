using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Starvers.AuraSystem.Skills.Base;
using Terraria;
using TShockAPI;

namespace Starvers.AuraSystem.Skills
{
	public class AvalonGradation : Skill
	{
		public const float R = 16 * 80;
		public static void Update(StarverPlayer player)
		{
			if(player.AvalonGradation <= 0)
			{
				return;
			}
			player.AvalonGradation--;
			foreach (Projectile proj in Main.projectile)
			{
				if (proj == null || !proj.active)
				{
					continue;
				}
				if (proj.friendly == false && Vector2.Distance(player.Center, proj.Center) < R)
				{
					proj.active = false;
					proj.type = 0;
					NetMessage.SendData((int)PacketTypes.ProjectileNew, -1, -1, null, proj.whoAmI);
				}
			}
		}
		public AvalonGradation() : base(SkillIDs.AvalonGradation)
		{
			CD = 60 * 50;
			MP = 180;
			Level = 1000;
			Author = "1413";
			Description = "消除你身边的所有敌对弹幕,持续10s";
			SetText();
		}
		public override void Release(StarverPlayer player, Vector2 vel)
		{
			player.SendMessage("你得到了来自幻想乡的庇护", Color.Aqua);
			player.AvalonGradation += 60 * 10;
		}
	}
}
