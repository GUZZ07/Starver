using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.BossSystem.Bosses
{
	using System.Runtime.InteropServices;
	using Base;
	using Microsoft.Xna.Framework;
	using Starvers.WeaponSystem;
	using Terraria.ID;
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public class StarverAdjudicator : StarverBoss
	{
		#region Field
		protected Vector GravityVel = new Vector(0, 25);
		protected unsafe short* InvTypes; //{ ProjectileID.RocketSkeleton, ProjectileID.NebulaLaser, ProjectileID.SaucerScrap, ProjectileID.SaucerMissile };
		private DropItem[] DropsNormal = new DropItem[]
		{
			new DropItem(new int[]{ Currency.Magic }, 15, 30, 0.73f),
			new DropItem(new int[]{ Currency.Magic }, 15, 30, 0.73f),
			new DropItem(new int[]{ Currency.Magic }, 15, 30, 0.73f),
		};
		private DropItem[] DropsEx = new DropItem[]
		{
			new DropItem(new int[]{ Currency.Magic }, 30, 55),
			new DropItem(new int[]{ Currency.Magic }, 30, 55),
			new DropItem(new int[]{ Currency.Magic }, 30, 55),
		};
		#endregion
		#region Ctor
		public StarverAdjudicator() : base(4)
		{
			TaskNeed = 29;
			Name = "The Starver Adjudicator";
			FullName = "Iesnet The Starver Adjudicator";
			DefaultLife = 1400000;
			DefaultLifes = 320;
			DefaultDefense = 990;
			RawType = NPCID.DukeFishron;
			unsafe
			{
				StarverAI[2] = NPCID.NebulaHeadcrab;
				StarverAI[3] = NPCID.NebulaBrain;
				InvTypes = (short*)Marshal.AllocHGlobal(sizeof(short) * 4).ToPointer();
				*InvTypes = ProjectileID.NebulaLaser;
				*++InvTypes = ProjectileID.RocketSkeleton;
				*++InvTypes = ProjectileID.SaucerScrap;
				*++InvTypes = ProjectileID.SaucerMissile;
				InvTypes -= 3;
			}
		}
		#endregion
		#region dtor
		unsafe ~StarverAdjudicator()
		{
			Marshal.FreeHGlobal(new IntPtr(InvTypes));
		}
		#endregion
		#region Fail
		public override void OnFail()
		{
			base.OnFail();
			if(ExVersion && EndTrial)
			{
				StarverPlayer.All.SendMessage("再去中核世界经历几百次轮回吧", Color.Pink);
				EndTrial = false;
				EndTrialProcess = 0;
			}
		}
		#endregion
		#region Downed
		protected override void BeDown()
		{
			base.BeDown();
			if(ExVersion && EndTrial)
			{
				StarverPlayer.All.SendMessage("你们不过是侥幸罢了...", Color.HotPink);
				EndTrialProcess++;
			}
		}
		#endregion
		#region Spawn
		public override void Spawn(Vector2 where, int lvl = CriticalLevel)
		{
			base.Spawn(where, lvl);
			vector.X = 16 * 30;
			Mode = BossMode.WitherBolt;
			Vel.Y = 0;
			unsafe
			{
				if (ExVersion)
				{
					StarverAI[0] = 30;
					Vel.X = 16 * 10;
					Drops = DropsEx;
				}
				else
				{
					StarverAI[0] = 50;
					Vel.X = 16 * 16;
					Drops = DropsNormal;
				}
			}
		}
		protected void Spawn(Vector2 where, int lvl, double AngleStart, float radium = -1)
		{
			Spawn(where, lvl);
			if(radium > 0)
			{
				vector.X = radium;
			}
			vector.Angle = AngleStart;
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
					LastCenter = Center;
					modetime = 0;
					switch (lastMode)
					{
						#region Bolt
						case BossMode.WitherBolt:
							Mode = BossMode.WitherLostSoul;
							break;
						#endregion
						#region LostSoul
						case BossMode.WitherLostSoul:
							Mode = BossMode.WitherSphere;
							unsafe
							{
								StarverAI[0] = ExVersion ? 60 : 110;
							}
							break;
						#endregion
						#region Sphere
						case BossMode.WitherSphere:
							if (ExVersion)
							{
								Mode = BossMode.WitherSaucerLaser;
							}
							else
							{
								Mode = BossMode.SummonFollows;
							}
							break;
						#endregion
						#region Laser/Summon
						case BossMode.WitherSaucerLaser:
						case BossMode.SummonFollows:
							Mode = BossMode.WitherInvincible;
							DamagedIndex = 0f;
							break;
						#endregion
						#region Invincible
						case BossMode.WitherInvincible:
							Mode = BossMode.WitherBolt;
							DamagedIndex = 1f;
							break;
							#endregion
					}
					break;
				#endregion
				#region Bolt
				case BossMode.WitherBolt:
					if (modetime > 60 * 8)
					{
						ResetMode();
						break;
					}
					if (Timer % StarverAI[0] == 0)
					{
						WitherBolt();
					}
					break;
				#endregion
				#region LostSoul
				case BossMode.WitherLostSoul:
					if(modetime > 60 * 10)
					{
						ResetMode();
						break;
					}
					WitherLostSoul();
					break;
				#endregion
				#region Invincible
				case BossMode.WitherInvincible:
					if(modetime > 60 * 13)
					{
						ResetMode();
						break;
					}
					if (Timer % 3 == 0)
					{
						WitherInvincible();
					}
					break;
				#endregion
				#region Laser
				case BossMode.WitherSaucerLaser:
					if(modetime > 60 * 10)
					{
						ResetMode();
						Vel.Y = 0;
						Vel.X = 16 * (ExVersion ? 10 : 16);
						break;
					}
					if (Timer % 3 == 0)
					{
						WitherLaser();
					}
					break;
				#endregion
				#region SummonFollows
				case BossMode.SummonFollows:
					if(modetime > 60 * 6)
					{
						ResetMode();
						break;
					}
					if (Timer % 30 == 0)
					{
						SummonFollows();
					}
					break;
				#endregion
				#region Sphere
				case BossMode.WitherSphere:
					if (modetime > 60 * 9)
					{
						ResetMode();
						StarverAI[0] = ExVersion ? 35 : 50;
						break;
					}
					if (Timer % StarverAI[0] == 0)
					{
						WitherSphere();
					}
					break;
					#endregion
			}
			#endregion
			#region Common
			if (Mode != BossMode.WitherInvincible)
			{
				vector.Angle += PI / 120;
			}
			if (ExVersion)
			{
				TargetPlayer.TPlayer.ZoneTowerNebula = true;
				TargetPlayer.SendData(PacketTypes.Zones, "", Target);
			}
			#endregion
		}
		#endregion
		#region AIs
		#region Bolt
		protected unsafe void WitherBolt()
		{
			if (ExVersion)
			{
				foreach (var player in Starver.Players)
				{
					if (player == null || !player.Active)
					{
						continue;
					}
					ProjSector(player.Center + vector, 15, 2, vector.Angle+ PI, PI * 3 / 4, 492, ProjectileID.NebulaBolt, 18);
				}
			}
			else
			{
				ProjSector(Center, 13, 2, vector.Angle+ PI, PI /2, 321, ProjectileID.NebulaBolt, 12);
			}
		}
		#endregion
		#region LostSoul
		protected void WitherLostSoul()
		{
			if (ExVersion)
			{
				foreach (var player in Starver.Players)
				{
					if (player == null || !player.Active)
					{
						continue;
					}
					Proj(player.Center + Vel, Vector.FromPolar(Vel.Angle+ PI / 2, 22), ProjectileID.LostSoulHostile, 213, 10f);
				}
			}
			else
			{
				Proj(TargetPlayer.Center + Vel, Vector.FromPolar(Vel.Angle- PI / 2, 18), ProjectileID.LostSoulHostile, 200, 3f);
			}
			Vel.Angle+= PI / 20;
		}
		#endregion
		#region Invincible
		protected unsafe void WitherInvincible()
		{
			Proj(TargetPlayer.Center + new Vector(0, -16 * 30) + Rand.NextVector2(16 * 40, 0), GravityVel, InvTypes[Rand.Next(4)], 560, 20f);
		}
		#endregion
		#region Laser
		protected void WitherLaser()
		{
			Vel = Vector.FromPolar(Rand.NextAngle() / 12 + PI * 5 / 12, 19);
			foreach (var player in Starver.Players)
			{
				if (player == null || !player.Active)
				{
					continue;
				}
				Proj(player.Center - GravityVel * 20, Vel, ProjectileID.SaucerLaser, 160, 2f);
				Proj(player.Center + GravityVel * 20, Vel.Deflect(PI), ProjectileID.SaucerLaser, 171, 2f);
			}
		}
		#endregion
		#region Sphere
		protected unsafe void WitherSphere()
		{
			if (ExVersion)
			{
				foreach (var player in Starver.Players)
				{
					if (player == null || !player.Active)
					{
						continue;
					}
					ProjCircle(player.Center + vector, 2, 30, ProjectileID.NebulaSphere, 18, 480,2);
				}
			}
			else
			{
				ProjCircle(Center, 2, 22, ProjectileID.NebulaSphere, 10, 330,2);
			}
		}
		#endregion
		#endregion
	}
}
