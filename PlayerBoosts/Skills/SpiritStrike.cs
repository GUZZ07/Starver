using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace Starvers.PlayerBoosts.Skills
{
	public class SpiritStrike : StarverSkill
	{
		protected static int rec = 1200;
		protected static Vector2 DefaultVector = Vector2.Zero;
		protected double d = 0;
		public SpiritStrike()
		{
			MPCost = 170;
			CD = 60 * 80;
			LevelNeed = 200;
			Author = "wither";
			Description = "对一定范围内的敌对生物发动攻击，获得60s闪避效果";
			Summary = "[200][击败蠕虫/大脑解锁]对近距离的敌人发起攻击，获得闪避效果";
		}
#warning 将来换成realm
		public override void Release(StarverPlayer player, Vector vel)
		{
			const float radium = 16 * 25;
			Action<Projectile> action = proj =>
			{
				proj.owner = 255;
				proj.aiStyle = -1;
				proj.timeLeft = 60 * 10;
				proj.netImportant = true;
				proj.SendData();
			};
			player.ProjCircleExNoBC(player.Center, Rand.NextAngle(), radium, action, ProjectileID.VortexLightning, 30, -1);
			Task.Run(() =>
			{
				try
				{
					const int interval = 15;
					const int time = 60 * 10;
					for (int i = 0; i < time / interval; i++)
					{
						foreach (var npc in Main.npc)
						{
							if (npc.active && !npc.dontTakeDamage && !npc.friendly && npc.Distance(player.Center) <= radium)
							{
								npc.StrikeNPC(140 + Rand.Next(10), 20, 0, true, fromNet: true, entity: player);
							}
						}
						Thread.Sleep(interval * 1000 / 60);
					}
				}
				catch (Exception e)
				{
					TSPlayer.Server.SendErrorMessage(e.ToString());
				}
			});
			player.SetBuff(BuffID.ShadowDodge);
		}
		public override bool CanSet(StarverPlayer player)
		{
			if (!NPC.downedBoss2)
			{
				player.SendText("该技能已被血腐的力量封印", 199, 21, 133);
				return false;
			}
			return base.CanSet(player);
		}
	}
}
