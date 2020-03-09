using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.BossSystem.Bosses
{
	using Base;
	using Microsoft.Xna.Framework;
	using Terraria;
	using Terraria.ID;
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public class CultistEx : StarverBoss
	{
		#region Fields
		private bool TrackingTarget;
		/// <summary>
		/// 计算出生多久(确保邪教有图像)
		/// </summary>
		private int SpawnCount;
		private IProjSet FireBalls = new ProjQueue();
		private IProjSet ShadowBalls = new ProjQueue(90);
		#endregion
		#region Ctor
		public CultistEx() : base(3)
		{
			TaskNeed = 42;
			RawType = NPCID.CultistBoss;
			Name = "Cultist Boss";
			DefaultLife = 50000;
			DefaultLifes = 150;
			DefaultDefense = 380;
			Drops = new DropItem[]
			{
				// new DropItem(new int[]{ ItemID.WhiteCultistArcherBanner }, 23, 49, 0.35f),
				new DropItem(new int[]{ ItemID.WhiteCultistFighterBanner }, 23, 49, 0.35f),
				new DropItem(new int[]{ ItemID.WhiteCultistCasterBanner }, 23, 49, 0.35f),
			};
		}
		#endregion
		#region Spawn
		public override void Spawn(Vector2 where, int lvl = 2000)
		{
			StarverBossManager.TriedSpawn = true;
			StarverBossManager.SpawnDelay = 0;
			base.Spawn(where, lvl);
			Mode = BossMode.CultistFireBall;
			SpawnCount = 0;
			RealNPC.ai[0] = 2f;
			RealNPC.ai[1] = 120f;
			RealNPC.aiStyle = -1;
			RealNPC.dontTakeDamage = false;
			TrackingTarget = true;
		}
		#endregion
		#region BeDown
		protected override void BeDown()
		{
			base.BeDown();
			//WorldGen.TriggerLunarApocalypse();
			StarverBossManager.Cleared = true;
			EndTrial = true;
			EndTrialProcess = 0;
		}
		#endregion
		#region RealAI
		public override void RealAI()
		{
			#region Common
			#region StartAnimation
#if false
			if (SpawnCount < 60 * 15)
			{
				RealNPC.Center = TargetPlayer.Center;
				RealNPC.position.Y -= 16 * 16;
				SpawnCount++;
				return;
			}
			else
			{
				RealNPC.dontTakeDamage = false;
				RealNPC.aiStyle = -1;
			}
#endif
			#endregion
			#region TrackingPlyer
			if (TrackingTarget && Timer % 60 == 0)
			{
				WhereToGo = (Vector)(TargetPlayer.Center);
				WhereToGo.Y -= 16 * 15;
			}
			FakeVelocity = (Vector)(WhereToGo - Center) / 20;
			#endregion
			#endregion
			#region Mode
			switch (Mode)
			{
				#region SelectMode
				case BossMode.WaitForMode:
					SelectMode();
					break;
				#endregion
				#region FireBall
				case BossMode.CultistFireBall:
					if (floats[0] > 4)
					{
						floats[0] = 0;
						ResetMode();
						break;
					}
					if (Timer % 43 == 1)
					{
						PushFireBall();
					}
					if (Timer % 43 == 32)
					{
						FireBalls.Launch();
						floats[0]++;
					}
					break;
				#endregion
				#region Lightning
				case BossMode.CultistLightning:
					if (floats[0] > PI * 2)
					{
						floats[0] = 0;
						ProjCircle(TargetPlayer.Center, 16 * 30, 0, ProjectileID.CultistBossLightningOrb, 8, 53, 0, Index);
						ResetMode();
						break;
					}
					if (Timer % 60 == 0)
					{
						Lightning();
					}
					break;
				#endregion
				#region ShadowFireBall
				case BossMode.CultistShadowFireball:
					if (floats[0] > 7)
					{
						ShadowBalls.Launch();
						floats[0] = 0;
						ResetMode();
						break;
					}
					if (Timer % 55 == 0)
					{
						if (floats[0]++ < 4)
						{
							FillShadowFireBall();
						}
						if (floats[0] > 2)
						{
							ShadowBalls.Launch(30);
						}
					}
					break;
				#endregion
				#region Mist
				case BossMode.Mist:
					if (modetime > 60 * 5)
					{
						ResetMode();
						break;
					}
					if (Timer % 56 == 0)
					{
						Mist();
					}
					break;
				#endregion
				#region SummonFollows
				case BossMode.SummonFollows:
					if (modetime > 60 * 5)
					{
						SummonFollows();
						ResetMode();
						break;
					}
					if (Timer % 22 == 0)
					{
						Proj(Center + Rand.NextVector2(16 * 30, 16 * 30), Rand.NextVector2(5), ProjectileID.AncientDoomProjectile, 252);
					}
					break;
					#endregion
			}
			#endregion
		}
		#endregion
		#region AIS
		#region SummonFollows
		private new void SummonFollows()
		{
			TrackingTarget = true;
			for (int i = 0; i < 4; i++)
			{
				NewNPC((Vector)Center, FromPolar(PI * i / 2, 33), NPCID.AncientCultistSquidhead, 30000, 60000);
			}
		}
		#endregion
		#region Mist
		private void Mist()
		{
			floats[2] = (float)(TargetPlayer.Center - Center).Angle();
			ProjSector(Center, 16, 16 * 3, floats[2], PI * 2 / 3, 158, ProjectileID.CultistBossIceMist, 3, 2, -6e3f, 1);
		}
		#endregion
		#region Lightning
		private void Lightning()
		{
			floats[0] += PI / 5;
			vector = FromPolar(floats[0], 16 * 20);
			int idx = Proj(TargetPlayer.Center + vector, Vector2.Zero, ProjectileID.CultistBossLightningOrb, 110);
			Main.projectile[idx].ai[0] = Index;
			Main.projectile[idx].timeLeft = 60;
		}
		#endregion
		#region FireBalls
		#region PushFireBall
		private void PushFireBall()
		{
			int[] result = ProjCircleWithReturn(Center, 1, 6, ProjectileID.CultistBossFireBall, 6, 243, 2);
			vector = (Vector)(TargetPlayer.Center - Center);
			vector.Length = 18;
			FireBalls.Push(result, vector);
		}
		#endregion
		#region FillShadowFireBall
		private void FillShadowFireBall()
		{
			int[] Indexes = ProjCircleWithReturn(Center, 16 * 10, Rand.NextVector2(4), ProjectileID.CultistBossFireBallClone, 10, 143);
			for (int i = 0; i < Indexes.Length; i++)
			{
				ShadowBalls.Push(Indexes[i], FromPolar(PI * 2 * i / Indexes.Length, 23));
			}
		}
		#endregion
		#endregion
		#region SelectMode
		private void SelectMode()
		{
			modetime = 0;
			switch (lastMode)
			{
				#region FireBall
				case BossMode.CultistFireBall:
					Mode = BossMode.CultistLightning;
					break;
				#endregion
				#region Lightning
				case BossMode.CultistLightning:
					Mode = BossMode.CultistShadowFireball;
					break;
				#endregion
				#region ShadowFireBall
				case BossMode.CultistShadowFireball:
					Mode = BossMode.Mist;
					break;
				#endregion
				#region Mist
				case BossMode.Mist:
					Mode = BossMode.SummonFollows;
					TrackingTarget = false;
					break;
				#endregion
				#region SummonFollows
				case BossMode.SummonFollows:
					Mode = BossMode.CultistFireBall;
					break;
					#endregion
			}
		}
		#endregion
		#endregion
	}
}
