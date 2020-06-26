using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace Starvers.PlayerBoosts.Skills
{
	public class WindRealm : StarverSkill
	{
		public WindRealm()
		{
			MPCost = 12;
			CD = 60 * 10;
			Author = "三叶草";
			Description = @"吹飞所有怪物以及敌对弹幕
别当着肉山的面用, 你会后悔的";
			LevelNeed = 175;
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			const float maxDistance = 16 * 45;
			for (int i = 0; i < Main.npc.Length; i++)
			{
				NPC npc = Main.npc[i];
				if (npc == null || !npc.active)
				{
					continue;
				}
				var playerToNpc = npc.Center - player.Center;
				var distance = playerToNpc.Length();
				var angle = Math.Abs(playerToNpc.Angle() - vel.Angle);
				playerToNpc.Normalize();
				if (distance < maxDistance && angle < Math.PI / 3)
				{
					npc.velocity += vel * 15 + playerToNpc * 8 * (maxDistance - distance) / maxDistance;
					npc.SendData();
				}
			}
			foreach (var proj in Main.projectile)
			{
				if (!proj.active)
				{
					continue;
				}
				if (proj.hostile)
				{
					proj.velocity = player.Center - proj.Center;
					proj.velocity.Length(30);
					proj.SendData();
				}
			}
			if (player.LastSkill == SkillIDs.WindRealm)
			{
				var damage = Math.Max(250, player.Level / 10) + 1;
				player.NewProj(vel * 10, ProjectileID.SwordBeam, damage, 1);
			}
			player.TPlayer.velocity += -vel * 2f;
		}
	}
}
