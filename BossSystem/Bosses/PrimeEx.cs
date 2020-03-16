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
	using System.Threading;
	using Terraria.ID;
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public class PrimeEx : StarverBoss
	{
		#region Fields
		internal const int MaxArms = 10;
		private bool ArmsActive;
		private Vector A;
		private NPCSystem.NPCs.PrimeExArm[] Arms;
		#endregion
		#region Properties
		internal BossMode ModeNow => Mode;
		#endregion
		#region Ctor
		public PrimeEx() : base(2)
		{
			TaskNeed = 34;
			RawType = NPCID.SkeletronPrime;
			DefaultLife = 110000;
			DefaultLifes = 90;
			DefaultDefense = 30;
			Arms = new NPCSystem.NPCs.PrimeExArm[MaxArms];
			for (int i = 0; i < MaxArms; i++)
			{
				Arms[i] = new NPCSystem.NPCs.PrimeExArm();
			}
			Drops = new DropItem[]
			{
				new DropItem(new int[] {Currency.Melee },9,15,0.75f),
				new DropItem(new int[] {Currency.Ranged, Currency.Minion },9,15,0.75f),
			};
		}
		#endregion
		#region LifeDown
		public override void LifeDown()
		{
			base.LifeDown();
			NewArms();
		}
		#endregion
		#region Spawn
		public override void Spawn(Vector2 where, int lvl = 2000)
		{
			base.Spawn(where, lvl);
			Vel = vector;
			FakeVelocity = Vector.Zero;
			//Vel.Length = 16 * 20;
			//Center = TargetPlayer.Center + Vel;
			//vector = vector.Deflect(PI / 2);
			//vector.Length = 18;
			//FakeVelocity = vector;
			lastMode = BossMode.PrimeExMissile;
			NewArms();
		}
		#endregion
		#region RealAI
		public override void RealAI()
		{
			#region Common
			//if(!SettedTracking)
			{
			//	SettedTracking = true;
			//	TrackingCenter = (Vector)TargetPlayer.Center;
			}
			if (Timer % 4 == 0)
			{
				UpdateVel();
			}
			ArmsActive = CheckArms();
			RealNPC.dontTakeDamage = false;
			#endregion
			#region Modes
			switch (Mode)
			{
				#region SelectMode
				case BossMode.WaitForMode:
					SelectMode();
					break;
				#endregion
				#region Laser
				case BossMode.PrimeExLaser:
					if(modetime > 70 * 6)
					{
						ResetMode();
						break;
					}
					if(Timer % 70 == 0)
					{
						Laser();
					}
					break;
				#endregion
				#region Rocket
				case BossMode.PrimeExRocket:
					if(modetime > 60 * 6)
					{
						ResetMode();
						break;
					}
					if(Timer % 45 == 0)
					{
						Rocket();
					}
					break;
				#endregion
				#region Missile
				case BossMode.PrimeExMissile:
					if(modetime > 40 * 9)
					{
						ResetMode();
						break;
					}
					if(Timer % 40 == 0)
					{
						Missile();
					}
					break;
					#endregion
			}
			#endregion
			#region ArmsAI
			//ArmsAI();
			#endregion
		}
		#endregion
		#region AIs
		#region Missile
		private void Missile()
		{
			if(ArmsActive)
			{
				return;
			}
			ProjCircle(Center, 2, 18, ProjectileID.SaucerMissile, 20, 221);
		}
		#endregion
		#region Rocket
		private void Rocket()
		{
			if (ArmsActive)
			{
				return;
			}
			unsafe
			{
				Vector* Circle = stackalloc Vector[10];
				vector = (Vector)(TargetPlayer.Center - Center);
				floats[0] = (float)vector.Angle;
				for (int t = 0; t < 10; t++)
				{
					Circle[t] = (Vector)Center + FromPolar(floats[0] - PI / 2 + t * PI / 10, 16 * 17);
					vector = (Vector)TargetPlayer.Center - Circle[t];
					vector.Length = 20;
					Proj(Circle[t], vector, ProjectileID.RocketSkeleton, 201);
				}
			}
		}
		#endregion
		#region Laser
		private void Laser()
		{
			if(ArmsActive)
			{
				return;
			}
			vector = (Vector)(TargetPlayer.Center - Center);
			Vel = vector.Vertical();
			Vel.Length = 16 * 10;
			vector.Length = 26;
			ProjLine(Center + Vel, Center - Vel, vector, 18, 203, ProjectileID.SaucerLaser);
		}
		#endregion
		#region UPdateVel
		private unsafe void UpdateVel()
		{
			floats[1] += 4 * PI / 45;
			if (!ArmsActive)
			{
				WhereToGo = (Vector)TargetPlayer.Center - FakeRound;
				WhereToGo.Y -= 16 * 10;
				FakeVelocity = WhereToGo - (Vector)Center;
				FakeVelocity /= 25;
			}
			else
			{
				WhereToGo = (Vector)TargetPlayer.Center + FromPolar(floats[1], 16 * 60);
				A = WhereToGo - (Vector)Center;
				A.Length = 14f;
				FakeVelocity += A;
			}
			if (FakeVelocity.Length < 40)
			{
				FakeVelocity.Length = 40;
			}
			else if(FakeVelocity.Length > 80)
			{
				FakeVelocity.Length = 80;
			}
		}
		#endregion
		#region SelectMode
		private void SelectMode()
		{
			modetime = 0;
			switch(lastMode)
			{
				#region Laser
				case BossMode.PrimeExLaser:
					Mode = BossMode.PrimeExRocket;
					break;
				#endregion
				#region Rocket
				case BossMode.PrimeExRocket:
					Mode = BossMode.PrimeExMissile;
					break;
				#endregion
				#region Missile
				case BossMode.PrimeExMissile:
					Mode = BossMode.PrimeExLaser;
					break;
					#endregion
			}
		}
		#endregion
		#region ArmsAI
		private void ArmsAI()
		{
			foreach(var Arm in Arms)
			{
				Arm.AI();
			}
		}
		#endregion
		#region CheckArms
		private bool CheckArms()
		{
			bool flag = false;
			foreach(var Arm in Arms)
			{
				flag |= Arm.Active;
			}
			return flag;
		}
		#endregion
		#region NewArms
		private void NewArms()
		{
			DontTakeDamage = true;
			for (int i = 0; i < MaxArms; i++)
			{
				Arms[i].Spawn((Vector)Center, this, i);
			}
		}
		#endregion
		#region FakeRound
		private unsafe Vector FakeRound
		{
			get
			{
				vector = FromPolar(floats[1], 16 * 120);
				vector.Y -= 16 * 70; //圆心下移
				if(vector.Y < 0)
				{
					vector.Y = (float)(Math.Sqrt(16 * 16 * 120 * 120 - vector.X * vector.X) - 16 * 70); // y小于0,就解方程把y换成正根
				}
				return vector;
			}
		}
		#endregion
		#endregion
	}
}
