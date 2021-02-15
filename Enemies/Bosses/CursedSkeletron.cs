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
					double ρ = (2 + Math.Abs(3 * Math.Sin(2 * t)) + Math.Abs(2 * Math.Cos(3 * t))) * 16 * 12;
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
		public void CalcMovementState(double φ, out Vector2 pos, out Vector2 vel)
		{
			if (!withinFollowing)
			{
				pos = Center;
				vel = FakeVelocity;
				return;
			}
			double t = φ;
			double ρ = (2 + Math.Abs(3 * Math.Sin(2*t)) + Math.Abs(2 * Math.Cos(3 * t))) * 16 * 12;
			var rp = new Vector2
			{
				X = (float)(ρ * Math.Cos(t + Math.PI / 2)),
				Y = (float)(ρ * Math.Sin(t + Math.PI / 2))
			};
			vel = (rp + fakeTarget - Center) / 4;
			if (vel.Length() > 20)
			{
				vel.Length(20);
			}
			pos = fakeTarget + rp;
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
						switch (stateCounter % 12 + 1)
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
							case 6:
							case 12:
								state = 3;
								break;
							default:
								Starver.Instance.DebugMessage($"stateCounter % 12 + 1: {stateCounter % 12 + 1}");
								break;
						}
						stateCounter++;
						stateTimer = -1;
						#endregion
						break;
					case 1:             // 1.	Boss向最玩家接连喷吐出若干团骨头
						{
							if (stateTimer % 60 == 0)
							{
								var vel = (TargetPlayer.Center - Center).ToLenOf(22);
								ProjCircleEx(Center, 0, 16 * 5, vel, ProjectileID.SkeletonBone, 16, 20);
							}
							if (stateTimer >= 60 * 8)
							{
								state = 0;
							}
						}
						break;
					case 2:				// 2.	Boss周围生成一圈静止巨型骷髅头弹幕，然后周围召唤两个无敌分身一起转动，并向玩家发射巨型骷髅头的弹幕
						{
							if (stateTimer == 0)
							{
								var φ = trackPara * Math.PI / 180;
								var clone1 = Starver.Instance.NPCs.SpawnNPC<Clone>(Center);
								var clone2 = Starver.Instance.NPCs.SpawnNPC<Clone>(Center);
								var clone3 = Starver.Instance.NPCs.SpawnNPC<Clone>(Center);
								CalcMovementState(φ + 1 * Math.PI * 2 / 4, out var pos1, out clone1.Velocity);
								CalcMovementState(φ + 2 * Math.PI * 2 / 4, out var pos2, out clone2.Velocity);
								CalcMovementState(φ + 3 * Math.PI * 2 / 4, out var pos3, out clone3.Velocity);
								clone1.Center = pos1;
								clone2.Center = pos2;
								clone3.Center = pos3;
								clone1.SetParas(this, φ + 1 * Math.PI * 2 / 4, Math.PI / 180, 4, 90, 00, 18, 60 * 18);
								clone2.SetParas(this, φ + 2 * Math.PI * 2 / 4, Math.PI / 180, 4, 90, 30, 18, 60 * 18);
								clone3.SetParas(this, φ + 3 * Math.PI * 2 / 4, Math.PI / 180, 4, 90, 60, 18, 60 * 18);
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
							if (stateTimer >= 60 * 10)
							{
								TNPC.damage = 100;
								TNPC.aiStyle = -1;
								state = 0;
							}
						}
						break;
					case 4:				// 4.	生成矩形边框围住玩家, 强迫玩家躲避弹幕
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
								var hor = new Vector2(0.02f, 0);
								var ver = new Vector2(0, 0.02f);
								var LUtoLD = ProjLineReturns(pLU, pLD, ver, 20, 93, ProjectileID.SaucerLaser);
								var RUtoRD = ProjLineReturns(pRU, pRD, ver, 20, 93, ProjectileID.SaucerLaser);
								var LUtoRU = ProjLineReturns(pLU, pRU, hor, 20, 93, ProjectileID.SaucerLaser);
								var LDtoRD = ProjLineReturns(pLD, pRD, hor, 20, 93, ProjectileID.SaucerLaser);

								static void killWall(Projectile proj, ref int timer, ref bool isEnd)
								{
									if (timer - proj.localAI[0] >= 60 * 5)
									{
										proj.active = false;
										proj.SendData();
										proj.active = true;
										proj.SendData();
										proj.localAI[0] = timer;
									}
									if (timer == 60 * 14)
									{
										isEnd = true;
										proj.active = false;
										proj.SendData();
										// Starver.Instance.DebugMessage("kill wall");
									}
									timer++;
								}
								for (int i = 0; i < 20; i++)
								{
									Main.projectile[LUtoLD[i]].aiStyle = -1;
									Main.projectile[RUtoRD[i]].aiStyle = -1;
									Main.projectile[LUtoRU[i]].aiStyle = -1;
									Main.projectile[LDtoRD[i]].aiStyle = -1;
									Main.projectile[LUtoLD[i]].timeLeft = 60 * 70;
									Main.projectile[RUtoRD[i]].timeLeft = 60 * 70;
									Main.projectile[LUtoRU[i]].timeLeft = 60 * 70;
									Main.projectile[LDtoRD[i]].timeLeft = 60 * 70;
									Starver.Instance.ProjsController.Add(new ProjController(LUtoLD[i], killWall));
									Starver.Instance.ProjsController.Add(new ProjController(RUtoRD[i], killWall));
									Starver.Instance.ProjsController.Add(new ProjController(LUtoRU[i], killWall));
									Starver.Instance.ProjsController.Add(new ProjController(LDtoRD[i], killWall));
								}
							}
							if (stateTimer % 90 == 0)
							{
								void killVer(Projectile proj, ref int timer, ref bool isEnd)
								{
									if (Math.Abs(proj.Center.Y - vector.Y) >= 16 * 51)
									{
										isEnd = true;
										proj.active = false;
										proj.netImportant = true;
										proj.SendData();
									}
								}

								var begin = new Vector2(-16 * 55, -16 * 55);
								var gap = Math.Abs(begin.X * 2 / 20);
								begin.X += Rand.NextFloat(gap);
								while (begin.X <= 16 * 55)
								{
									var vel = new Vector2(0, 16 * 55 * 2 / 90f);
									var idx = NewProj(vector + begin, vel, ProjectileID.SaucerLaser, 30, 0);
									Main.projectile[idx].timeLeft = 60 * 10;
									Starver.Instance.ProjsController.Add(new ProjController(idx, killVer));
									begin.X += gap;
								}
							}
							if (stateTimer % 60 == 0)
							{
								void killHor(Projectile proj, ref int timer, ref bool isEnd)
								{
									if (Math.Abs(proj.Center.X - vector.X) >= 16 * 51)
									{
										isEnd = true;
										proj.active = false;
										proj.netImportant = true;
										proj.SendData();
									}
								}
								var dir = Rand.NextDirection();
								var pos = new Vector2(16 * 55 * dir, 16 * Rand.NextFloat(-55, 55));
								var vel = new Vector2(-dir, 0) * Rand.NextFloat(8, 18);
								var idx = NewProj(pos + vector, vel, ProjectileID.SaucerLaser, 34, 0);
								Main.projectile[idx].timeLeft = 60 * 10;
								Starver.Instance.ProjsController.Add(new ProjController(idx, killHor));
							}
							if (stateTimer == 60 * 14)
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

								var v1L = Vector.FromPolar(π / 2 + 1 * π / 8, 16 * 20);
								var v1D = Vector.FromPolar(π / 2 - 1 * π / 8, 16 * 20);
								var v2L = Vector.FromPolar(π / 2 + 2 * π / 8, 16 * 20);
								var v2D = Vector.FromPolar(π / 2 - 2 * π / 8, 16 * 20);

								hand1L.SetParas(this, 60 * 9, Rand.Next(30), 60 * 2, 22, focus + v1L, v1L.ToLenOf(0.1f));
								hand1D.SetParas(this, 60 * 9, Rand.Next(30), 60 * 2, 22, focus + v1D, v1L.ToLenOf(0.1f));
								hand2L.SetParas(this, 60 * 9, Rand.Next(30), 60 * 2, 22, focus + v2L, v1L.ToLenOf(0.1f));
								hand2D.SetParas(this, 60 * 9, Rand.Next(30), 60 * 2, 22, focus + v2D, v1L.ToLenOf(0.1f));
							}
							if (stateTimer == 60 * 9)
							{
								posLockto = null;
								state = 0;
							}
						}
						break;
				}
				if (timer % 20 == 0)
				{
					NewProj(Vector2.Zero, ProjectileID.DD2DarkMageRaise, 22, 0);
				}
			}
		}
		#endregion
	}
}
