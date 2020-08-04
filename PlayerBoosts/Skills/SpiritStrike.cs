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
			MPCost = 35;
			CD = 60 * 60;
			LevelNeed = 80;
			Author = "wither";
			Description = "对一定范围内的敌对生物发动攻击";
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
							if (npc.active && !npc.friendly && npc.Distance(player.Center) <= radium)
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
	}
}
