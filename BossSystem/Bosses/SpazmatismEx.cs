using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.BossSystem.Bosses
{
	using Base;
	using Microsoft.Xna.Framework;
	using Starvers.WeaponSystem;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using Terraria;
	using Terraria.ID;
	using TShockAPI;
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public class SpazmatismEx : StarverBoss
	{
		#region Fields
		/// <summary>
		/// 加速度
		/// </summary>
		private Vector A;
		/// <summary>
		/// 用于循环中
		/// </summary>
		private int LoopCount;
		private Projectile proj;
		private float FlamingLength;
		/// <summary>
		/// Rush里的proj使用
		/// </summary>
		private int idx;
		private Vector RealCenter;
		private bool AutoSetWhereToGo;
		private bool AutoSetFakeVelocity;
		/// <summary>
		/// 用于控制WhereToGO
		/// </summary>
		private Vector Direct;
		/// <summary>
		/// 什么时候免疫伤害
		/// </summary>
		private bool WhichDontTakeDamage;
		private IProjSet Fires = new ProjQueue(130);
		private new IProjSet Projs = new ProjQueue(60);
		#endregion
		#region Properties
		/// <summary>
		/// 相对位置(要加上玩家坐标,以RealCenter计算)
		/// </summary>
		private new Vector RelativePos
		{
			get
			{
				return RealCenter - (Vector)TargetPlayer.Center;
			}
			set
			{
				RealCenter = (Vector)(TargetPlayer.Center) + value;
			}
		}
		#endregion
		#region Ctor
		public SpazmatismEx() : base(3)
		{
			TaskNeed = 37;
			RawType = NPCID.Spazmatism;
			DefaultLife = 37500;
			DefaultLifes = 100;
			DefaultDefense = 587;
			Drops = new DropItem[]
			{
				new DropItem(new int[]{ Currency.Minion, Currency.Magic }, 1, 28),
				new DropItem(new int[]{ Currency.Minion, Currency.Magic }, 1, 28),
				new DropItem(new int[]{ Currency.Minion, Currency.Magic }, 1, 28),
				new DropItem(new int[]{ Currency.Minion, Currency.Magic }, 1, 28),
				new DropItem(new int[]{ Currency.Minion, Currency.Magic }, 1, 28),
			};
		}
		#endregion
		#region Spawn
		public override void Spawn(Vector2 where, int lvl = 2000)
		{
			base.Spawn(where, lvl);
			Direct = FromPolar(0, 16 * 20);
			AutoSetWhereToGo = true;
			AutoSetFakeVelocity = true;
			lastMode = BossMode.SpazmatismBetsyFireBall;
			Projs.Reset();
			RealCenter = (Vector)RealNPC.Center;
		}
		#endregion
		#region AI
		public override void AI(object args = null)
		{
			#region Base
			if (!CheckActive())
			{
				return;
			}
			if (!RealNPC.active)
			{
				RealNPC.active = true;
				RealNPC.life = RealNPC.lifeMax;
				RealNPC.Center = LastCenter;
				SendData();
			}
			if (Target < 0 || Target >= Starver.Players.Length || TargetPlayer == null || !TargetPlayer.Active)
			{
				TargetClosest();
				if (Target < 0 || Target >= Starver.Players.Length || TargetPlayer == null || !TargetPlayer.Active)
				{
					KillMe();
					return;
				}
			}
			if (Vector2.Distance(Center, TargetPlayer.Center) > 16 * 300)
			{
				if (IgnoreDistance)
				{
					Center = TargetPlayer.Center + Rand.NextVector2(16 * 20);
				}
				else
				{
					KillMe();
					return;
				}
			}
			if (NightBoss && Main.dayTime)
			{
				TSPlayer.Server.SetTime(false, 0);
			}
			++Timer;
			RealAI();
			++modetime;
			//Velocity = FakeVelocity;
			//SendData();
			#endregion
			#region Extend
			//Velocity = Timer % 7 >= 3 ? -FakeVelocity : FakeVelocity;
			if(Vector2.Distance(RealCenter,TargetPlayer.Center) > 16 * 290)
			{
				RealCenter = (Vector)(TargetPlayer.Center + Rand.NextVector2(16 * 14));
			}
			RealCenter += FakeVelocity;
			SendData();
			#endregion
		}
		#endregion
		#region RealAI
		public unsafe override void RealAI()
		{
			#region Mode
			switch (Mode)
			{
				#region SelectMode
				case BossMode.WaitForMode:
					SelectMode();
					break;
				#endregion
				#region Flaming
				case BossMode.SpazmatismFlaming:
					if (modetime > 60 * 9)
					{
						RushStart();
						break;
					}
					if (Timer % 2 == 0)
					{
						Flaming();
					}
					break;
				#endregion
				#region FlameNormal
				case BossMode.SpazmatismFlameNormal:
					if(modetime > 60 * 8.5f)
					{
						RushStart();
						break;
					}
					if(Timer % 8 == 0)
					{
						FlameNormal();
					}
					break;
				#endregion
				#region BetsyFireBall
				case BossMode.SpazmatismBetsyFireBall:
					if(StarverAI[1] > 4)
					{
						StarverAI[1] = 0;
						RushStart();
						break;
					}
					if(Timer % 6 == 0)
					{
						BetsyFireBall();
					}
					break;
				#endregion
				#region Rush
				case BossMode.Rush:
					Rush();
					break;
					#endregion
			}
			#endregion
			#region Common
			if (Timer % 4 > 1)
			{
				//vector = RelativePos;
				RealNPC.dontTakeDamage = WhichDontTakeDamage;
				Center = (Vector)(TargetPlayer.Center - RelativePos);
				//SendData();
			}
			else
			{
				RealNPC.dontTakeDamage = !WhichDontTakeDamage;
				Center = RealCenter;	//TargetPlayer.Center + RelativePos;
			}
			//RealNPC.rotation = (float)(TargetPlayer.Center - Center).Angle();
			//SendData(); 在StarverBoss::AI()里头有
			UpdateVel();
			if(FakeVelocity.Length > 20)
			{
				FakeVelocity.Length = 20;
			}
			if(Timer % 18 == 0)
			{
				Proj(Center + Rand.NextVector2(16 * 5, 16 * 5), Rand.NextVector2(1), ProjectileID.EyeFire, 210);
			}
			#endregion
		}
		#endregion
		#region AIS
		#region BetsyFireBall
		private unsafe void BetsyFireBall()
		{
			if (StarverAI[0] >= 10)
			{
				Projs.Launch(3);
			}
			if(StarverAI[0] < 15)
			{
				Vel = (Vector)Rand.NextVector2(16 * 30);
				vector = -Vel;
				Vel.Length = Rand.Next(28, 33);
				proj = Main.projectile[Proj(TargetPlayer.Center + vector, Vector2.Zero, ProjectileID.DD2BetsyFireball, 232)];
				proj.aiStyle = -1;
				Projs.Push(proj.whoAmI, Vel);
				Vel = (Vector)Rand.NextVector2(16 * 30);
				vector = -Vel;
				Vel.Length = Rand.Next(28, 33);
				proj = Main.projectile[Proj(TargetPlayer.Center + vector, Vector2.Zero, ProjectileID.DD2BetsyFireball, 232)];
				proj.aiStyle = -1;
				Projs.Push(proj.whoAmI, Vel);
			}
			else if(StarverAI[0] > 30)
			{
				Projs.Reset();
				StarverAI[0] = -1;
				StarverAI[1]++;
			}
			StarverAI[0] += 1;
		}
		#endregion
		#region FlameNormal
		private void FlameNormal()
		{
			Vel = (Vector)(TargetPlayer.Center - Center);
			Vel.Angle -= PI / 10;
			Vel.Angle += Rand.NextAngle() / 5;
			Vel.Length = 18 + Rand.Next(7);
			Proj(Center, Vel, ProjectileID.CursedFlameHostile, 218);
		}
		#endregion
		#region Rush
		private unsafe new void Rush()
		{
			WhereToGo = FromPolar((TargetPlayer.Center - Center).Angle(), 16 * 10) + (Vector)TargetPlayer.Center;
			if (StarverAI[0] < 16)
			{
				if (Timer % 10 == 0)
				{
					idx = Proj(Center + Rand.NextVector2(16 * 5), Rand.NextVector2(1), ProjectileID.CursedFlameHostile, 223);
					Vel = (WhereToGo - (Vector)Main.projectile[idx].Center);
					Vel.Length = 44;
					Fires.Push(idx, Vel);
					StarverAI[0]++;
				}
			}
			else if (StarverAI[0] < 50)
			{
				if (Timer % 10 == 0)
				{
					idx = Proj(Center + Rand.NextVector2(16 * 5), Rand.NextVector2(1), ProjectileID.CursedFlameHostile, 223);
					Vel = (WhereToGo - (Vector)Main.projectile[idx].Center);
					Vel.Length = 44;
					Fires.Push(idx, Vel);
					StarverAI[0]++;
				}
				if (Timer % 16 == 0)
				{
					Fires.Launch(1);
				}
			}
			else if (StarverAI[0] < 90)
			{
				for (LoopCount = 0; LoopCount < 2; LoopCount++)
				{
					idx = Proj(Center + Rand.NextVector2(16 * 5), Rand.NextVector2(1), ProjectileID.CursedFlameHostile, 223);
					Vel = (WhereToGo - (Vector)Main.projectile[idx].Center);
					Vel.Length = 44;
					Fires.Push(idx, Vel);
				}
				StarverAI[0]++;
			}
			else if (StarverAI[0] < 100)
			{
				StarverAI[0] = 100;
				FakeVelocity = WhereToGo - (Vector)Center;
				FakeVelocity.Length = 30;
				Fires.Launch();
				Fires.Reset();
			}
			else
			{
				if (StarverAI[0]++ < 100 + 60 * 2)
				{
					AutoSetWhereToGo = true;
					//AutoSetFakeVelocity = true;
					FakeVelocity.Length = -1;
					Direct *= -1;
					StarverAI[0] = 0;
					StarverAI[2] = 0;
					Mode = BossMode.WaitForMode;
				}
			}
		}
		#endregion
		#region Flaming
		private void Flaming()
		{
			FlamingLength = Math.Min((TargetPlayer.Center - Center).Length() + 16 * 8, 16 * 30);
			Vel = (Vector)Center + FromPolar((-RelativePos).Angle, FlamingLength);
			ProjLine(Center, Vel, Vector2.Zero, (int)FlamingLength / 40, 219, ProjectileID.Flames);
			#region Wasted
			/*
			Vel = (Vector)Center + FromPolar((-RelativePos).Angle + PI / 18, FlamingLength);
			ProjLine(Center, Vel, Vector2.Zero, 20, 219, ProjectileID.Flames);
			Vel = (Vector)Center + FromPolar((-RelativePos).Angle - PI / 18, FlamingLength);
			ProjLine(Center, Vel, Vector2.Zero, 20, 219, ProjectileID.Flames);
			*/
			/*
			foreach (var proj in Main.projectile)
			{
				if (!proj.active)
				{
					continue;
				}
				if (proj.type == ProjectileID.Flames)
				{
					proj.timeLeft = 5;
					NetMessage.SendData((int)PacketTypes.ProjectileNew, -1, -1, null, proj.whoAmI);
				}
			}
			*/
			#endregion
		}
		#endregion
		#region UPdateVel
		private void UpdateVel()
		{
			if (AutoSetWhereToGo && Timer % 2 == 0)
			{
				WhereToGo = (Vector)(TargetPlayer.Center);
				WhereToGo += Direct;
				Direct.Angle += PI / 180;
			}
			if (AutoSetFakeVelocity && Timer % 30 == 0)
			{
				A = WhereToGo - RealCenter;
				A.Length = 5;
				FakeVelocity += A;
			}
		}
		#endregion
		#region RushStart
		private void RushStart()
		{
			AutoSetWhereToGo = false;
			//AutoSetFakeVelocity = false;
			RealCenter = (Vector)(TargetPlayer.Center + Rand.NextVector2(16 * 30));
			FakeVelocity = Vector.Zero;
			Vel = (WhereToGo - RealCenter);
			Vel.Length = 33;
			modetime = 0;
			RushBegin(true);
		}
		#endregion
		#region SelectMode
		private void SelectMode()
		{
			modetime = 0;
			WhichDontTakeDamage ^= true;
			switch(lastMode)
			{
				#region Flaming
				case BossMode.SpazmatismFlaming:
					Mode = BossMode.SpazmatismFlameNormal;
					break;
				#endregion
				#region FlameNormal
				case BossMode.SpazmatismFlameNormal:
					Mode = BossMode.SpazmatismBetsyFireBall;
					break;
				#endregion
				#region BetsyFireBall
				case BossMode.SpazmatismBetsyFireBall:
					Mode = BossMode.SpazmatismFlaming;
					break;
					#endregion
			}
		}
		#endregion
		#endregion
	}
}
