using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Starvers.AuraSystem.Skills.Base;
using Terraria;
using TShockAPI;

namespace Starvers.AuraSystem.Skills
{
	public class WindRealm:Skill
	{
		public WindRealm() : base(SkillIDs.WindRealm)
		{
			BossBan = true;
			MP = 12;
			CD = 60 * 10;
			Author = "三叶草";
			Description = @"吹飞所有怪物以及敌对弹幕
别当着肉山的面用, 你会后悔的";
			Level = 175;
			SetText();
		}
		public override void Release(StarverPlayer player, Vector2 vel)
		{
			for (int i = 0; i < Main.npc.Length; i++)
			{
				NPC npc = Main.npc[i];
				if (npc == null || !npc.active)
				{
					continue;
				}
				float dis = (player.TPlayer.position - npc.position).Length();
				float cos = Vector2.Dot(npc.position - player.TPlayer.position, vel) / (vel.Length() * (npc.position - player.TPlayer.position).Length());
				if (dis < 800 && cos <= 1 && cos >= 0.5)
				{
					npc.velocity += (vel * 15 + (npc.position - player.TPlayer.position) / (npc.position - player.TPlayer.position).Length() * 8) * (800 - dis) / 800;
					TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", i);
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
			if (player.LastSkill == (int)SkillIDs.WindRealm)
			{
				player.NewProj(player.Center, vel * 10, 116, player.Level / 10 + 1, 1);
			}
			player.TPlayer.velocity += -vel * 4f;
			TSPlayer.All.SendData(PacketTypes.PlayerUpdate, "", player.Index);
		}
	}
}
