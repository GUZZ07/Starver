using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Starvers.PlayerBoosts.Skills
{
	public class TrackingMissile : StarverSkill
	{
		protected static int Target;
		protected const float dis = 16 * 100;
		protected Vector2 Pos;
		public TrackingMissile()
		{
			LevelNeed = 1000;
			MPCost = 350;
			CD = 60 * 45;
			Description = "制造若干个射向最近敌人位置的导弹";
			Author = "Deaths";
			Summary = "[1000][击败血肉之墙解锁]持续生成追踪敌人的导弹";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			Target = -1;
			foreach (NPC npc in Main.npc)
			{
				if (npc.friendly)
				{
					continue;
				}
				if ((npc.Center - player.TPlayer.Center).Length() < dis)
				{
					Target = npc.whoAmI;
					break;
				}
			}
			Task.Run(() => RealSkill(player));
		}
		protected static void RealSkill(StarverPlayer player)
		{
			try
			{
				int t = 0;
				int extend = 2 * (int)Math.Log(player.Level);
				int count = player.Level > 200 ? 8 : 16;
				while (t++ < 10 + extend)
				{
					Thread.Sleep(1000);
					if (Target != -1 && Main.npc[Target].active)
					{
						player.ProjCircle(Main.npc[Target].Center, 16 * 25, -9, ProjectileID.VortexBeaterRocket, count, 80);
					}
					else
					{
						player.ProjCircle(player.Center, 16 * 25, -9, ProjectileID.VortexBeaterRocket, count, 80);
					}
				}
			}
			catch
			{

			}
		}
	}
}
