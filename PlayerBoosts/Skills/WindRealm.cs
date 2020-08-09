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
		private const float maxDistance = 16 * 45;
		public WindRealm()
		{
			MPCost = 80;
			CD = 60 * 10;
			Author = "三叶草";
			Description = @"吹飞所有怪物以及敌对弹幕
别当着肉山的面用, 你会后悔的";
			LevelNeed = 800;
			Summary = "[800][击败骷髅王解锁]吹飞指向方向上的威胁";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			ProcessNPC(player, vel);
			ProcessProj(player, vel);
			if (player.LastSkill == SkillIDs.WindRealm)
			{
				var damage = Math.Max(250, player.Level / 10) + 1;
				player.NewProj(vel * 10, ProjectileID.SwordBeam, damage, 1);
			}
			player.Velocity += -vel;
		}
		#region ProcessNPC
		private void ProcessNPC(StarverPlayer player, Vector vel)
		{
			foreach (var npc in Main.npc)
			{
				if (!npc.active)
				{
					return;
				}
				var playerToNpc = npc.Center - player.Center;
				var distance = playerToNpc.Length();
				var angle = Vector.AngleAbs(vel, playerToNpc);
				if (distance < maxDistance && angle < Math.PI / 3)
				{
					npc.velocity += CalcKnockBack(vel, playerToNpc);
					npc.SendData();
				}
			}
		}
		#endregion
		#region ProcessProj
		private void ProcessProj(StarverPlayer player, Vector vel)
		{
			foreach (var proj in Main.projectile)
			{
				if (!proj.active)
				{
					return;
				}
				var playerToProj = player.Center - proj.Center;
				var distance = playerToProj.Length();
				if (proj.hostile && distance < maxDistance)
				{
					proj.velocity += playerToProj.ToLenOf(16 * 30 / distance);
					proj.SendData();
				}
			}
		}
		#endregion
		private static Vector2 CalcKnockBack(Vector vel, Vector2 playerToEntity)
		{
			var distance = playerToEntity.Length();
			playerToEntity.Normalize();
			return vel * 15 + playerToEntity * 8 * (maxDistance - distance) / maxDistance;
		}
		public override bool CanSet(StarverPlayer player)
		{
			if (!NPC.downedBoss3)
			{
				player.SendText("该技能已被地牢的诅咒封印", 238, 232, 170);
				return false;
			}
			return base.CanSet(player);
		}
	}
}
