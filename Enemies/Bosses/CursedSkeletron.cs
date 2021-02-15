using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace Starvers.Enemies.Bosses
{
	using Clone = Npcs.SkeletronExClone;
	using Hand = Npcs.SkeletronExHand;
	public class CursedSkeletron : StarverBoss
	{
		private Vector FakeVelocity;
		private int state;
		private int stateCounter;

		public CursedSkeletron() : base(NPCID.SkeletronHead)
		{
			defLifes = 5;
			defLife = 48000;
			defDefense = 1200;
			AfraidSun = true;
		}

		public override void Spawn(Vector2 position, int level = 2000)
		{
			base.Spawn(position, level);
			TNPC.aiStyle = -1;
			ResetMovement();
			ResetProjB();
		}

		protected override void ReSpawn()
		{
			base.ReSpawn();
			TNPC.aiStyle = -1;
		}

		protected override void RealAI()
		{
			if (TargetPlayer?.Alive != true)
			{
				TryFindTarget();
				if (TargetPlayer?.Alive != true)
				{
					TurnToAir();
					return;
				}
			}
			ProjBehavior();
			UpdateMovement();
		}
		#region Movement
		private const float followingDistance = 16 * 300;
		private bool withinFollowing;
		private Vector2? posLockto;
		private Vector2 relativePos;
		private Vector2 fakeTarget;
		private int trackPara;

		private void ResetMovement()
		{
			relativePos = new Vector2(0, 16 * 40);
			trackPara = 0;
			posLockto = null;
			withinFollowing = false;
			FakeVelocity = default;
		}
		private void UpdateMovement()
		{
			if (TNPC.aiStyle == 11)
			{
				return;
			}
			if(!withinFollowing)
			{
				if (TNPC.Distance(TargetPlayer.Center) <= followingDistance)
				{
					withinFollowing = true;
					fakeTarget = (2 * TargetPlayer.Center + Center) / 3; // center____ __target
				}
				else
				{
					float s = TNPC.Distance(TargetPlayer.Center) / 16;
					Vector2 vMax = (TargetPlayer.Center - Center).ToLenOf(s * s / 100);
					Vector2 accel = (vMax - FakeVelocity) / 120;
					FakeVelocity += (Vector)accel;
				}
			}
			else
			{
				if (TNPC.Distance(TargetPlayer.Center) > followingDistance)
				{
					withinFollowing = false;
				}
			}
			if (withinFollowing)
			{
				if (posLockto != null)
				{
					relativePos = (Vector)posLockto;
					FakeVelocity = (Vector)(relativePos - Center) / 4;
				}
				else
				{
					double t = trackPara * Math.PI / 180;
					double ρ = (2 + Math.Abs(3 * Math.Sin(2 * t)) + Math.Abs(2 * Math.Cos(3 * t))) * 16 * 24;
					relativePos = new Vector2
					{
						X = (float)(ρ * Math.Cos(t + Math.PI / 2)),
						Y = (float)(ρ * Math.Sin(t + Math.PI / 2))
					};
					FakeVelocity = (Vector)((relativePos + fakeTarget) - Center) / 4;
					if (FakeVelocity.Length > 20)
					{
						FakeVelocity.Length = 20;
					}
					if (Vector2.Distance(fakeTarget, TargetPlayer.Center) > 10)
					{
						fakeTarget += (TargetPlayer.Center - fakeTarget).ToLenOf(9);
					}
					trackPara++;
				}
			}
			Velocity = FakeVelocity;
			UpdateToClient();
		}
		#endregion
		#region ProjBehavior
		private Vector2 vector;			// 可跨多次
		private int timer;
		private int stateTimer;			// 每次切换state后清零
		private void ResetProjB()
		{
			state = 0;
			stateCounter = 1;
			timer = 0;
			stateTimer = 0;
		}
		private void ProjBehavior()
		{
			timer++;
			stateTimer++;
			if (Lifes > LifesMax * 7 / 10)
			{
				switch (state)
				{
					case 0:
						#region switch state
						switch (stateCounter++ % 12 + 1)
						{
							case 1:
							case 3:
							case 7:
							case 10:
								state = 1;
								break;
							case 2:
							case 8:
								state = 4;
								break;
							case 4:
							case 11:
								state = 5;
								break;
							case 5:
							case 9:
								state = 2;
								break;
							case 12:
								state = 3;
								break;
						}
						stateTimer = -1;
						#endregion
						break;
					case 1:             // 1.	Boss向最玩家接连喷吐出若干团骨头
						{
							if (stateTimer % 150 == 0)
							{
								var vel = (TargetPlayer.Center - Center).ToLenOf(22);
								ProjCircleEx(Center, 0, 16 * 5, vel, ProjectileID.SkeletonBone, 16, 20);
							}
							if (stateTimer >= 60 * 15)
							{
								state = 0;
							}
						}
						break;
					case 2:				// 2.	Boss周围生成一圈静止巨型骷髅头弹幕，然后周围召唤两个无敌分身一起转动，并向玩家发射巨型骷髅头的弹幕
						{
							if (stateTimer == 0)
							{
								var clone1 = Starver.Instance.NPCs.SpawnNPC<Clone>(Center);
								var clone2 = Starver.Instance.NPCs.SpawnNPC<Clone>(Center);
								var clone3 = Starver.Instance.NPCs.SpawnNPC<Clone>(Center);
								clone1.SetParas(this, 0 * Math.PI / 3, Math.PI / 300, 4, 90, 00, 18, 60 * 18);
								clone2.SetParas(this, 1 * Math.PI / 3, Math.PI / 300, 4, 90, 30, 18, 60 * 18);
								clone3.SetParas(this, 2 * Math.PI / 3, Math.PI / 300, 4, 90, 60, 18, 60 * 18);
							}
							if (stateTimer >= 60 * 18)
							{
								state = 0;
							}
						}
						break;
					case 3:				// 3.	Boss停止转动并向最邻近的玩家自旋靠近，期间骷髅王持续向随机方向发射褴褛邪教徒法师的弹幕（差不多每秒3发）
						{
							if (stateTimer == 0)
							{
								TNPC.damage = 300;
								TNPC.aiStyle = 11;
								TNPC.ai[1] = 1;
								TNPC.ai[2] = 400 - 60 * 10;
							}
							if (stateTimer % 30 == 0)
							{
								var vel = Rand.NextVector2(19);
								NewProj(vel, ProjectileID.LostSoulHostile, 22);
							}
							if (stateTimer == 60 * 10)
							{
								TNPC.damage = 100;
								TNPC.aiStyle = -1;
								state = 0;
							}
						}
						break;
					case 4:				// 4.	Boss周围随机位置生成若干个食人魔重踏（印象中这个是有一个向上喷发的动画的）
										//		并吼叫一声，屏幕上端向下发射一排密集魔教徒弹幕，屏幕底端向上发射一排密集魔教徒弹幕，
										//		两排弹幕错开并在到达屏幕中间位置时爆开
						{
							if (stateTimer == 0)
							{
								vector = TargetPlayer.Center;
								for (int i = 0; i < 5; i++)
								{
									var pos = Rand.NextVector2(Rand.NextFloat(4, 40) * 16);
									NewProj(pos, Vector.Zero, ProjectileID.DD2OgreSmash, 28);
								}
								var pLU = new Vector2(-16 * 55, -16 * 30) + vector;
								var pLD = new Vector2(-16 * 55, +16 * 30) + vector;
								var pRU = new Vector2(+16 * 55, -16 * 30) + vector;
								var pRD = new Vector2(+16 * 55, +16 * 30) + vector;
								var vel = new Vector2(0, 55 * 16 / (60 * 14));
								var UtoD = ProjLineReturns(pLU, pRU,  vel, 25, 33, ProjectileID.InfernoHostileBolt);
								var DtoU = ProjLineReturns(pLD, pRD, -vel, 25, 33, ProjectileID.InfernoHostileBolt);

								static void kill(Projectile proj, ref int timer, ref bool isEnd)
								{
									if(timer++ == 60 * 14)
									{
										isEnd = true;
										proj.active = false;
										proj.netImportant = true;
										proj.SendData();
									}
								}
								for (int i = 0; i < 25; i++)
								{
									Starver.Instance.ProjsController.Add(new ProjController(UtoD[i], kill));
									Starver.Instance.ProjsController.Add(new ProjController(DtoU[i], kill));
								}
							}
							if (stateTimer == 60 * 14)
							{
								var pL = new Vector2(-16 * 50, 0) + vector;
								var pR = new Vector2(+16 * 50, 0) + vector;
								var pU = new Vector2(0, -16 * 50) + vector;
								var pD = new Vector2(0, +16 * 50) + vector;
								ProjLine(pL, pR, new Vector2(0.1f, 0), 25, 33, ProjectileID.SaucerLaser);
								ProjLine(pU, pD, new Vector2(0, 0.1f), 25, 33, ProjectileID.SaucerLaser);
							}
							if (stateTimer == 60 * 17)
							{
								state = 0;
							}
						}
						break;
					case 5:
										// 5.	Boss静止并召唤若干条手臂分摊在Boss两边，蓄力2 s后所有手向所朝方向高速飞走,
										//		沿途向后喷射出大量骨头和血弹，飞离一个屏幕长度距离后自爆并爆出若干骨头和血弹
						{
							if (stateTimer == 0)
							{
								posLockto = new Vector(0, -25 * 16);
								var focus = new Vector(0, -40 * 16) + (Vector)TargetPlayer.Center;

								var hand1L = Starver.Instance.NPCs.SpawnNPC<Hand>(focus);
								var hand1D = Starver.Instance.NPCs.SpawnNPC<Hand>(focus);
								var hand2L = Starver.Instance.NPCs.SpawnNPC<Hand>(focus);
								var hand2D = Starver.Instance.NPCs.SpawnNPC<Hand>(focus);

								const double π = Math.PI;

								var v1L = Vector.FromPolar(π / 2 + 1 * π / 5, 16 * 30);
								var v1D = Vector.FromPolar(π / 2 - 1 * π / 5, 16 * 30);
								var v2L = Vector.FromPolar(π / 2 + 2 * π / 5, 16 * 30);
								var v2D = Vector.FromPolar(π / 2 - 2 * π / 5, 16 * 30);

								hand1L.SetParas(this, 60 * 9, Rand.Next(30), 30, 22, focus + v1L, v1L.ToLenOf(2));
								hand1D.SetParas(this, 60 * 9, Rand.Next(30), 30, 22, focus + v1D, v1L.ToLenOf(2));
								hand2L.SetParas(this, 60 * 9, Rand.Next(30), 30, 22, focus + v2L, v1L.ToLenOf(2));
								hand2D.SetParas(this, 60 * 9, Rand.Next(30), 30, 22, focus + v2D, v1L.ToLenOf(2));
							}
							if (stateTimer == 60 * 9)
							{
								posLockto = null;
								state = 0;
							}
						}
						break;
				}
				if (timer % 45 == 0)
				{
					NewProj(Vector2.Zero, ProjectileID.DD2DarkMageRaise, 22, 0);
				}
			}
		}
		#endregion
	}
}
