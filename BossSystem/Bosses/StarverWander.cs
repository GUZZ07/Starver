using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Starvers.BossSystem.Bosses
{
	using Base;
	using Microsoft.Xna.Framework;
	using Starvers.WeaponSystem;
	using Terraria;
	using Terraria.ID;
	using TShockAPI;
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public class StarverWander : StarverBoss
	{
		#region Fields
		/// <summary>
		/// len = 16 * 14
		/// </summary>
		protected Vector2 UnitY = new Vector2(0, 16 * 14);
		protected Vector2 UnitX = new Vector2(16, 0);
		protected int Ammo = ProjectileID.VortexLaser;
		protected int wait = 1;
		private DropItem[] DropsNormal = new DropItem[]
		{
			new DropItem(new int[]{ Currency.Ranged }, 15, 30, 0.73f),
			new DropItem(new int[]{ Currency.Ranged }, 15, 30, 0.73f),
			new DropItem(new int[]{ Currency.Ranged }, 15, 30, 0.73f),
		};
		private DropItem[] DropsEx = new DropItem[]
		{
			new DropItem(new int[]{ Currency.Ranged }, 30, 55),
			new DropItem(new int[]{ Currency.Ranged }, 30, 55),
			new DropItem(new int[]{ Currency.Ranged }, 30, 55),
		};
		#endregion
		#region Ctor
		public StarverWander():base(4)
		{
			TaskNeed = 27;
			DefaultLife = 2000;
			DefaultLifes = 200;
			DefaultDefense = 0;
			RawType = NPCID.DukeFishron;
			Name = "The Starver Wander";
			// FullName = "Gnawyzarc The Starver Wander";
			floats[3] = NPCID.VortexRifleman;
			floats[2] = NPCID.VortexSoldier;
			Vel.X = Vel.Y = 10;
		}
		#endregion
		#region Downed
		protected override void BeDown()
		{
			base.BeDown();
			if (ExVersion && EndTrial && GetType() == typeof(StarverWander))
			{
				EndTrialProcess++;
				StarverPlayer.All.SendMessage("...", Color.HotPink);
				StarverPlayer.All.SendMessage("你们等着", Color.HotPink);
			}
		}
		#endregion
		#region Fail
		public override void OnFail()
		{
			base.OnFail();
			if(ExVersion && EndTrial && GetType() == typeof(StarverWander))
			{
				StarverPlayer.All.SendMessage("你们还是太弱了...", Color.Pink);
				if (Lifes > LifesMax / 2)
				{
					StarverPlayer.All.SendMessage("这么轻易就死了...", Color.Pink);
				}
				if(Lifes < 20)
				{
					StarverPlayer.All.SendMessage("好可惜,你们就差一点...", Color.Pink);
				}
				EndTrial = false;
				EndTrialProcess = 0;
			}
		}
		#endregion
		#region Spawn
		public unsafe override void Spawn(Vector2 where, int lvl = CriticalLevel)
		{
			base.Spawn(where, lvl);
			Mode = BossMode.CraShoot1;
			floats[0] = 1;
			UnitX.X = ExVersion ? 26 : 16;
			Ammo = ExVersion ? ProjectileID.VortexLaser : ProjectileID.VortexAcid;
			wait = ExVersion ? 2 : 1;
			Drops = ExVersion ? DropsEx : DropsNormal;
		}
		#endregion
		#region RealAI
		public override void RealAI()
		{
			#region Modes
			switch (Mode)
			{
				#region SelectMode
				case BossMode.WaitForMode:
					SelectMode();
					break;
				#endregion
				#region Shoot1
				case BossMode.CraShoot1:
					if (modetime > 60 * 7.5)
					{
						ResetMode();
						floats[1] = 0;
						break;
					}
					if (Timer % (5 / wait) == 0)
					{
						Shoot1();
					}
					break;
				#endregion
				#region Shoot2
				case BossMode.CraShoot2:
					if (modetime > 60 * 10)
					{
						ResetMode();
						floats[1] = 0;
						break;
					}
					if (Timer % (60 / wait) == 0)
					{
						Shoot2();
					}
					break;
				#endregion
				#region Shoot3
				case BossMode.CraShoot3:
					if (modetime > 60 * 10)
					{
						ResetMode();
						floats[1] = 0;
						break;
					}
					if (Timer % (55 / wait) == 0)
					{
						Shoot3();	
					}
					break;
				#endregion
				#region Shoot4
				case BossMode.Crashoot4:
					if (modetime > 60 * 12)
					{
						ResetMode();
						break;
					}
					Shoot4();
					break;
				#endregion
				#region VortexSphere
				case BossMode.CraVortexSphere:
					if (floats[1] > 20)
					{
						floats[1] = 0;
						ResetMode();
						break;
					}
					if (Timer % 20 == 0)
					{
						VortexSphere();	
					}
					break;
				#endregion
				#region SummonFollows
				case BossMode.SummonFollows:
					if (modetime > 60 * 7)
					{
						ResetMode();
						break;
					}
					if (Timer % 25 == 0)
					{
						SummonFollows();
					}
					break;
				#endregion
				#region Mist
				case BossMode.Mist:
					if (modetime > 60 * 9)
					{
						ResetMode();
						floats[1] = 0;
						break;
					}
					floats[1] += PI / 9;
					if (Timer % 60 == 0)
					{
						foreach (var player in Starver.Players)
						{
							if (player == null || !player.Active)
							{
								continue;
							}
							for (int i = 0; i < 8; i++)
							{
								Proj(player.Center + FromPolar(floats[1] + PI * i / 4, 16 * 20), FromPolar(floats[1] + PI * i / 4 + PI * 3 / 4, 20), ProjectileID.CultistBossIceMist, 266, 5f, -3e3f, 1);
							}
						}
					}
					break;
				#endregion
				#region FireBall
				case BossMode.CraFireBall:
					if (floats[1] > 6)
					{
						floats[1] = 0;
						ResetMode();
						break;
					}
					if (Timer % (int)(60 + 15 * floats[1]) == 0)
					{
						FireBall();
					}
					break;
					#endregion
			}
			#endregion
			#region Common
			if (floats[0] > 0)
			{
				WhereToGo = new Vector(-16 * 30, 0);
			}
			else
			{
				WhereToGo = new Vector(16 * 30, 0);
			}
			WhereToGo += (Vector)TargetPlayer.Center;
			FakeVelocity = WhereToGo - (Vector)Center;
			FakeVelocity /= 10;
			if (ExVersion)
			{
				RealNPC.ai[0] = 11f;
				RealNPC.ai[1] = 0f;
				RealNPC.ai[2] = 0f;
				TargetPlayer.TPlayer.ZoneTowerVortex = true;
				if (Timer % 60 == 0)
				{
					TargetPlayer.SendData(PacketTypes.Zones, "", Target);
				}
			}
			#endregion
		}
		#endregion
		#region AIs
		#region FireBall
		protected unsafe void FireBall()
		{
			if (ExVersion)
			{
				foreach (var player in Starver.Players)
				{
					if (player == null || !player.Active)
					{
						continue;
					}
					ProjCircle(player.Center, 16 * 29 + 19 * floats[1], 24, ProjectileID.CultistBossFireBall, 8 + (int)floats[1], 233);
				}
			}
			else
			{
				ProjCircle(TargetPlayer.Center, 16 * 29 + 19 * floats[1], 19, ProjectileID.CultistBossFireBall, 8 + (int)floats[1], 186);
			}
			floats[1]++;
		}
		#endregion
		#region VortexSphere
		protected unsafe void VortexSphere()
		{
			floats[1]++;
			vector.X += 16 * 4;
			Vel = (Vector)TargetPlayer.Center - vector;
			Vel.Length = 10;
			int idx = Proj(vector, Vel, ProjectileID.VortexVortexLightning, 360, 3f);
			Main.projectile[idx].velocity += (TargetPlayer.Center - Main.projectile[idx].Center).ToLenOf(14);
			//Proj(vector, (TargetPlayer.Center -  vector).ToLenOf(22), ProjectileID.VortexLightning, 120, 3f, Main.projectile[idx].ai[1], Rand.Next(80));
			vector.Y -= 16 * 20;
			Vel = (Vector)TargetPlayer.Center - vector;
			Vel.Length = 10;
			idx = Proj(vector, Vel, ProjectileID.VortexVortexLightning, 360, 3f);
			Main.projectile[idx].velocity += (TargetPlayer.Center - Main.projectile[idx].Center).ToLenOf(14);
			//Proj(vector, (TargetPlayer.Center -  vector).ToLenOf(22), ProjectileID.VortexLightning, 120, 3f, Main.projectile[idx].ai[1], Rand.Next(80));
			vector.Y += 16 * 20;
		}
		#endregion
		#region Shoot4
		protected void Shoot4()
		{
			Proj(Center, Rand.NextVector2(22), Ammo, 232, 3f);
			if (ExVersion)
			{
				Proj(Center, Rand.NextVector2(22), Ammo, 350, 3f);
				Proj(Center, Rand.NextVector2(22), Ammo, 352, 3f);
			}
		}
		#endregion
		#region Shoot3
		protected void Shoot3()
		{
			ProjLine(TargetPlayer.Center - new Vector2(16 * 33 * floats[1], 16 * 20), TargetPlayer.Center - new Vector2(16 * 33 * floats[1], -16 * 20), floats[1] * vector, 25, 202, Ammo);
			floats[1] *= -1;
		}
		#endregion
		#region Shoot2
		protected void Shoot2()
		{
			UnitY = (TargetPlayer.Center - Center).Vertical();
			UnitY.Length(16 * 14);
			ProjSector(Center + UnitY, ExVersion ? 23 : 16, 3, floats[1], PI / 4, 220, Ammo, 12);
			ProjSector(Center - UnitY, ExVersion ? 23 : 16, 3, floats[1], PI / 4, 220, Ammo, 12);
			if (ExVersion)
			{
				ProjSector(Center, 23, 3, floats[1], PI / 4, 260, Ammo, 12);
			}
		}
		#endregion
		#region Shoot1
		protected void Shoot1()
		{
			floats[1] += PI / 40;
			if (floats[1] > PI)
			{
				floats[1] -= PI;
			}
			else if (floats[1] > PI / 2)
			{
				vector.Angle = PI - floats[1];
			}
			else
			{
				vector.Angle = floats[1];
			}
			if (floats[0] < 0)
			{
				vector.X *= -1;
				UnitX.X = -1 * Math.Abs(UnitX.X);
			}
			else
			{
				UnitX.X = Math.Abs(UnitX.X);
			}
			Vel = vector;
			Vel.Y *= -1;
			Proj(Center, UnitX, Ammo, 240, 2f);
			Proj(Center, vector, Ammo, 240, 2f);
			Proj(Center, Vel, Ammo, 200, 2f);
		}
		#endregion
		#region SelectMode
		protected unsafe void SelectMode()
		{
			LastCenter = Center;
			modetime = 0;
			floats[0] *= -1;
			switch (lastMode)
			{
				#region shot1
				case BossMode.CraShoot1:
					Mode = BossMode.CraVortexSphere;
					vector = (Vector)TargetPlayer.Center;
					vector.X -= 16 * 10 * 4;
					vector.Y += 16 * 10;
					break;
				#endregion
				#region vortexsphere
				case BossMode.CraVortexSphere:
					Mode = BossMode.CraShoot2;
					if (floats[0] > 0)
					{
						floats[1] = 0;
					}
					else
					{
						floats[1] = PI;
					}
					break;
				#endregion
				#region Shot2
				case BossMode.CraShoot2:
					if (ExVersion)
					{
						Mode = BossMode.Mist;
						floats[1] = 0;
					}
					else
					{
						Mode = BossMode.SummonFollows;
					}
					break;
				#endregion
				#region mist&follows
				case BossMode.Mist:
				case BossMode.SummonFollows:
					Mode = BossMode.CraShoot3;
					vector = new Vector(16, 0);
					floats[1] = 1;
					break;
				#endregion
				#region shot3
				case BossMode.CraShoot3:
					Mode = BossMode.CraFireBall;
					break;
				#endregion
				#region fireball
				case BossMode.CraFireBall:
					Mode = BossMode.Crashoot4;
					break;
				#endregion
				#region shot4
				case BossMode.Crashoot4:
					vector.Angle = 0;
					vector.Length = ExVersion ? 26 : 16;
					Mode = BossMode.CraShoot1;
					break;
					#endregion
			}
		}
		#endregion
		#endregion
	}
}
