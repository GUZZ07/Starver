using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.AuraSystem.Skills
{
	using Base;
	using Microsoft.Xna.Framework;
	using System.Threading;
	using Terraria;
	using Terraria.ID;
	using Vector = TOFOUT.Terraria.Server.Vector2;
	using Circle = TOFOUT.Terraria.Server.Shapes.RoundRing;
	using System.Runtime.CompilerServices;

	public class NatureStorm : Skill
	{
		#region Fields
		private const float XDistance = 16 * 65;
		private const float YDistance = 16 * 30;
		private const int UpingGreenID = ProjectileID.CrystalLeafShot;
		private const int ExStrikeID = ProjectileID.FlamingJack;
		private const int LeafType = ProjectileID.CrystalLeafShot;
		private readonly static Vector2 UpingVel = new Vector2(0, -27);
		#endregion
		#region Ctor
		public NatureStorm() : base(SkillIDs.NatureStorm)
		{
			Level = 30000;
			MP = 8000;
			CD = 60 * 100;
			ForceCD = true;
			Author = "至今尚未查出来源";
			Description = @"Excalibur, NStrike, UltimateSlash与UniverseBlast———
还有角落里的Lawaias不解的注视着它
[c/00ed00:""无论是MP消耗, 等级要求, 还是CD, 代码量乃至bug量, 你都比我们要多""]
[c/00ed00:""我们不明白, 同样是终极技能, 为什么你会得到如此高的待遇...""]
[c/0000ed:""原因很简单], [c/ff0000:?***%Key?=NotFound][c/0000ed:""]";
			SetText();
		}
		#endregion
		#region Release
		public override void Release(StarverPlayer player, Vector2 vel)
		{
			AsyncRelease(player, (Vector)vel);
		}
		private async void AsyncRelease(StarverPlayer player, Vector vel)
		{
			await Task.Run(() =>
			{
				try
				{
					int damage = (int)Math.Sqrt(player.Level);
					damage *= 20;
					damage += 1500;

					UpingGreen(player, player);

					Vector2 Distance;

					NPC target = null;

					foreach (var npc in Main.npc)
					{
						if (npc == null || !npc.active)
						{
							continue;
						}
						Distance = npc.Center - player.Center;
						Distance.X = Math.Abs(Distance.X);
						Distance.Y = Math.Abs(Distance.Y);
						#region SelectMainStrikeTarget
						if (target == null)
						{
							target = npc;
						}
						else if (target.ReceiveDamage() && target.life < npc.life)
						{
							target = npc;
						}
						#endregion
						if (Distance.X < XDistance && Distance.Y < YDistance)
						{
							UpingGreen(player, npc, 15);
						}
						Thread.Sleep(5);
					}
					#region SelectTarget
					if (target is null || target.friendly)
					{
						foreach (var npc in Main.npc)
						{
							if (!npc.active)
							{
								target = npc;
								break;
							}
						}
					}
					#endregion

					vel.Length = 18;
					int[] projs = new int[128];
					for (int i = 0; i < projs.Length; i++)
					{
						projs[i] = player.NewProj(player.Center, vel, ExStrikeID, damage);
					}
					SetTarget(projs, target.whoAmI);
					damage /= 4;
					Vector2 HitPos = Main.projectile[projs[0]].Center;
					while (CheckProjs(projs))
					{
						#region playerEffect
						if (target.active)
						{
							Vector velocity = (Vector)(target.Center - player.Center);
							velocity.Length = 17;
							Vector offset = velocity.Vertical();
							for (int i = 0; i < 6; i++)
							{
								offset.Length = Rand.Next(-16 * 3, 16 * 3);
								player.NewProj(player.Center + offset, velocity, LeafType, damage);
								Thread.Sleep(2);
							}
						}
						else for (int i = 0; i < 6; i++)
							{
								player.NewProj(player.Center, Rand.NextVector2(13), LeafType, damage);
								Thread.Sleep(2);
							}
						#endregion
						AddLeaf(projs, player);
						HitPos = Main.projectile[projs[0]].Center;
					}
					if (target.active)
					{
						HitPos = target.Center;
					}
					Boom(HitPos, player);
				}
				catch(Exception e)
				{
					string msg = e.ToString();
					StarverPlayer.All.SendErrorMessage(msg);
					StarverPlayer.Server.SendErrorMessage(msg);
				}
			});
		}
		#endregion
		#region UpingGreen
		internal static void UpingGreen(StarverPlayer player,Entity target, int timer = 50)
		{
			UpingGreen(player, target.position, target.height, target.width, timer);
		}
		internal static void AsyncUpingGreen(StarverPlayer player, Entity target)
		{
			#region Wasted
			/*
			await Task.Run(() =>
			{
				int Timer = 0;
				int speed = 16;
				Vector velocity = new Vector(0, -speed);
				Vector StartPos = (Vector)target.Center;
				StartPos.Y += target.height / 4 + target.height / 2;
				Vector Line = new Vector(2 * target.width, 0);
				Line.X = Math.Min(16 * 6, Line.X);

				int Num = (int)(Line.Length / 4);

				int damage = (int)Math.Sqrt(player.Level);
				damage *= 5;
				damage += 900;


				while (Timer++ < 6)
				{
					player.ProjLine(StartPos + Line, StartPos - Line, velocity, Num, damage, UpingGreenID);
					Thread.Sleep(3 * speed);
				}
			});
			*/
			#endregion
			AsyncUpingGreen(player, target.Center, target.height, target.width);
		}
		internal static void UpingGreen(StarverPlayer player, Vector2 Center, int Height, int Width,int timer = 50)
		{
			int Timer = 0;
			int speed = 18;
			Vector velocity = new Vector(0, -speed);
			Vector StartPos = (Vector)Center;
			StartPos.Y += Height / 3 + Height / 2;
			Vector Line = new Vector(2 * Width, 0);
			Line.X = Math.Min(16 * 6, Line.X);

			int Num = (int)(Line.Length / 8);

			int damage = (int)Math.Sqrt(player.Level);
			damage *= 5;
			damage += 900;


			while (Timer++ < timer)
			{
				player.ProjLine(StartPos + Line, StartPos - Line, velocity, Num, damage, UpingGreenID);
				Thread.Sleep(20);
			}
		}
		internal static async void AsyncUpingGreen(StarverPlayer player, Vector2 Center, int Height,int Width)
		{
			await Task.Run(() =>
			{
				int Timer = 0;
				int speed = 18;
				Vector velocity = new Vector(0, -speed);
				Vector StartPos = (Vector)Center;
				StartPos.Y += Height / 4 + Height / 2;
				Vector Line = new Vector(2 * Width, 0);
				Line.X = Math.Min(16 * 6, Line.X);

				int Num = (int)(Line.Length / 8);

				int damage = (int)Math.Sqrt(player.Level);
				damage *= 5;
				damage += 900;


				while (Timer++ < 5) 
				{
					player.ProjLine(StartPos + Line, StartPos - Line, velocity, Num, damage, UpingGreenID);
					Thread.Sleep(20);
				}
			});
		}
		#endregion
		#region CheckProjs
		private static bool CheckProjs(int[] indexes)
		{
			bool flag = true;
			foreach(var idx in indexes)
			{
				if(Main.projectile[idx].active == false || Main.projectile[idx].type != ExStrikeID)
				{
					flag = false;
					break;
				}
			}
			return flag;
		}
		#endregion
		#region AddLeaf
		private static void AddLeaf(int[] projs, StarverPlayer player)
		{
			int damage = (int)Math.Sqrt(player.Level);
			damage *= 4;
			damage += 500;
			int idx;
			idx = projs[0];
			for (int i = 0; i < 4; i++)
			{
				player.NewProj(Main.projectile[idx].Center, Rand.NextVector2(6), LeafType, damage);
			}
		}
		#endregion
		#region SetTarget
		private static void SetTarget(int[] projs,int target)
		{
			foreach(var idx in projs)
			{
				Main.projectile[idx].ai[0] = target;
				NetMessage.SendData((int)PacketTypes.ProjectileNew, -1, -1, null, idx);
			}
		}
		#endregion
		#region Boom
		private static void Boom(Vector2 pos,StarverPlayer player)
		{
			int damage = (int)Math.Sqrt(player.Level);
			damage *= 20;
			damage += 1550;

			float radium = 2 * 16;

			player.ProjCircle(pos, radium / 2, 25, ProjectileID.DD2SquireSonicBoom, 32, damage);

			for (int i = 0; i < 4; i++)
			{
				player.ProjCircle(pos, radium, 0.1f, ProjectileID.PureSpray, (int)(radium * 2 * Math.PI / 36), damage / 2);
				radium += 16 * 3f;
				Thread.Sleep(175);
			}
			player.ProjCircle(pos, 16 * 60, -25, LeafType, 50, damage);
			UpingGreen(player, pos, 16 * 3, 16 * 30);
		}
		#endregion
	}
}
