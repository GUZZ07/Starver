using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Starvers.PlayerBoosts.Skills
{
	public class NStrike : UltimateSkill
	{
		private const float rec = 16 * 30;
		public NStrike()
		{
			MPCost = 3500;
			CD = 60 * 70;
			LevelNeed = 20000;
			Author = "wither";
			Description = "能够和咖喱棒平起平坐的技能!\n对一定范围内的敌对生物发动攻击";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			foreach (NPC npc in Main.npc)
			{
				if (npc.active && (npc.Center - player.TPlayer.Center).Length() <= rec + Math.Min(player.Level / 5, 16 * 30)) 
				{
					npc.velocity = new Vector2(0);
					NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, null, npc.whoAmI);
					player.NewProj(npc.Center, Vector2.Zero, ProjectileID.StardustGuardianExplosion, player.Level * 10 + 1400, 0);
				}
			}
			if (player.TPlayer.hostile)
			{
				foreach (Player ply in Main.player)
				{
					if ((!ply.hostile) || (ply.Center - player.TPlayer.Center).Length() > rec + player.Level || ply.whoAmI == player.Index || (ply.team == player.TSPlayer.Team && ply.team != 0 && ply.team != 1))
					{
						continue;
					}
					ply.velocity = Vector2.Zero;
					NetMessage.SendData((int)PacketTypes.PlayerUpdate, -1, -1, null, ply.whoAmI);
					player.NewProj(ply.Center, Vector2.Zero, ProjectileID.StardustGuardianExplosion, player.Level * 10 + 1400, 0);
				}
			}
			player.SetBuff(BuffID.ShadowDodge);
		}
	}
}
