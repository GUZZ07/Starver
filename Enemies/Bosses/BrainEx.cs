using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace Starvers.Enemies.Bosses
{
	public class BrainEx : StarverBoss
	{
		private PlayerTracker Moving;
		private BossStateMachine ProjActing;
		private int t;

		public BrainEx() : base(NPCID.BrainofCthulhu)
		{
			defLifes = 8;
			defLife = 45000;
			defDefense = 1400;
		}

		public override void Spawn(Vector2 position, int level = 2000)
		{
			base.Spawn(position, level);

			TNPC.damage = 130;

			Moving = new PlayerTracker(this);
			Moving.Begin();

			ResetMovingMachine();

			ProjActing = new SpawnCreeper(this)
			{
				DeflectionAngle = Math.PI / 10
			};
			ProjActing.Begin();
		}

		protected override void RealAI()
		{
			TNPC.ai[0] = -3f;
			TNPC.aiStyle = -1;

			Moving.Update();
			if (!Active)
			{
				return;
			}
			ProjActing.Update();
			if (ProjActing.IsEnd)
			{
				ToNextProjActing();
			}
		}

		private void ToNextProjActing()
		{
			t++;
			ProjActing = GetNextProjActing(t);
			DontTakeDamage = false;
			ResetMovingMachine();
			#region AdjustMoving
			if (ProjActing.State == BossState.BrainBloodDropping)
			{
				Moving.Offset = new Vector2(0, -16 * 50);
				Moving.MaxSpeed = 40;
				Moving.WarpingInterval = null;
			}
			else if (ProjActing.State == BossState.EllipseShot)
			{
				Moving.Offset = Rand.NextVector2(16 * 40);
				Moving.MaxSpeed = 40;
				Moving.WarpingInterval = null;
			}
			else if (ProjActing.State == BossState.SummonServants)
			{
				Moving.MaxSpeed = 0;
				Moving.WarpingInterval = 60;
			}
			else if (ProjActing.State == BossState.BrainEx5)
			{
				Moving.MaxSpeed = 0;
				Moving.WarpingInterval = null;
			}
			else if (ProjActing.State == BossState.ShotFromPlayers)
			{
				Moving.MaxSpeed = 0;
				Moving.WarpingInterval = null;
			}
			#endregion
			ProjActing.Begin();
		}

		private BossStateMachine GetNextProjActing(int t)
		{
			if (Lifes > LifesMax / 2)
			{
				switch (t % 10 + 1)
				{
					case 1:
						return GetProjActingMachine(BossState.SpawnCreeper);
					case 2:
					case 5:
					case 9:
						return GetProjActingMachine(BossState.BrainBloodDropping);
					case 3:
					case 8:
						return GetProjActingMachine(BossState.SummonServants);
					case 7:
						return GetProjActingMachine(BossState.EllipseShot);
					case 4:
					case 6:
					case 10:
						return GetProjActingMachine(BossState.BrainEx5);
				}
			}
			else if (Lifes > LifesMax / 4)
			{
				switch (t % 10 + 1)
				{
					case 1:
						return GetProjActingMachine2(BossState.SpawnCreeper);
					case 2:
						return GetProjActingMachine2(BossState.BrainBloodDropping);
					case 3:
					case 9:
						return GetProjActingMachine2(BossState.ShotFromPlayers);
					case 8:
						return GetProjActingMachine2(BossState.SummonServants);
					case 5:
					case 7:
						return GetProjActingMachine2(BossState.EllipseShot);
					case 4:
					case 6:
					case 10:
						return GetProjActingMachine2(BossState.BrainEx5);
				}
			}
			return GetProjActingMachine2(BossState.SpawnCreeper);
		}

		private BossStateMachine GetProjActingMachine(BossState state)
		{
			switch (state)
			{
				case BossState.BrainBloodDropping:
					{
						return new BrainBloodDropping(this);
					}
				case BossState.EllipseShot:
					{
						var machine = new EllipseShot(this, ProjectileID.BloodShot, ProjectileID.BloodShot)
						{
							Rotation = Rand.NextAngle(),
							RotationSpeed = Math.PI / 6 / 60,
							Count = 20,
							Damage = 25,
							TotalTime = 60 * 10,
							AxisA = 16 * 6,
							AxisB = 16 * 10,
						};
						machine.Linked = new EllipseShot(this, ProjectileID.BloodNautilusShot)
						{
							Rotation = machine.Rotation,
							RotationSpeed = Math.PI / 6 / 60,
							Count = 16,
							Damage = 35,
							ShotingDelay = 30,
							TotalTime = 60 * 10 - 30,
							AxisA = 16 * 6,
							AxisB = 16 * 10,
						};
						return machine;
					}
				case BossState.SummonServants:
					{
						double angleMid = (Center - TargetPlayer.Center).Angle();
						var machine = new SummonServants(this)
						{
							ServantData = new NPCData
							{
								ID = NPCID.IchorSticker,
								Defense = 1500,
								LifeMax = 1000
							},
							Radium = 16 * 6f,
							Count = 5,
							AngleBegin = angleMid - Math.PI / 4,
							AngleEnd = angleMid + Math.PI / 4,
							TotalTime = 60 * 3 * 3,
							Interval = 180,
							SummonDelay = 60 * 0,
							ServantSpeed = 4f
						};
						machine.Linked = new SummonServants(this)
						{
							ServantData = new NPCData
							{
								ID = NPCID.BloodSquid,
								Defense = 500,
								LifeMax = 750
							},
							Radium = 16 * 6f,
							Count = 5,
							AngleBegin = angleMid - Math.PI / 4,
							AngleEnd = angleMid + Math.PI / 4,
							TotalTime = 60 * 3 * 3,
							Interval = 180,
							SummonDelay = 90,
							ServantSpeed = 4f
						};
						return machine;
					}
				case BossState.BrainEx5:
					{
						return new BrainEx5(this)
						{
							ProjIDs = new int[] { ProjectileID.DeathLaser },
							Damage = 55,
							AccumulationTime = 60 * 6,
							HitDuration = 60 * 2,
							HitRadius = 16 * 80,
							UpingSpeed = 2.4f
						};
					}
			}
			return new SpawnCreeper(this)
			{
				DeflectionAngle = Math.PI / 10
			};
		}
		private BossStateMachine GetProjActingMachine2(BossState state)
		{
			switch (state)
			{
				case BossState.ShotFromPlayers:
					{
						static float f(double t)
						{
							var sin = Math.Sin(2.5 * t + 2.5 * 4);
							return (float)(6.5 + 5 * sin * sin * sin * sin);
						};
						var data = new ProjCurveData
						{
							ID = ProjectileID.CursedFlameHostile,
							Damage = 48,
							FuncPos = t => 010f * Vector.FromPolar(t, f(t)),
							FuncVel = t => 0.5f * Vector.FromPolar(t, f(t)),
							ParaBegin = 0,
							ParaEnd = Math.PI * 2,
							Increment = Math.PI * 2 / 20
						};
						return new ShotFromPlayers(this)
						{
							HitRadius = 16 * 100,
							ProjData = data,
							LaunchInterval = 90,
							LaunchDelay = 89,
							TotalTime = 90 * 5
						};
					}
				case BossState.BrainBloodDropping:
					{
						return new BrainBloodDropping(this)
						{
							Damage = 55,
							DroppingInterval = 2,
							DroppingWidth = 16 * 120,
							TotalTime = 60 * 10,
							DroppingSpeed = 14
						};
					}
				case BossState.EllipseShot:
					{
						var machine = new EllipseShot(this, ProjectileID.BloodShot, ProjectileID.BloodShot)
						{
							Rotation = Rand.NextAngle(),
							RotationSpeed = Math.PI / 6 / 60,
							Count = 20,
							Damage = 35,
							TotalTime = 60 * 10,
							ShotingInterval = 120,
							AxisA = 16 * 6,
							AxisB = 16 * 10,
						};
						machine.Linked = new EllipseShot(this, ProjectileID.BloodNautilusShot)
						{
							Rotation = machine.Rotation + Math.PI / 2,
							RotationSpeed = Math.PI / 6 / 60,
							Count = 16,
							Damage = 35,
							ShotingDelay = 60,
							TotalTime = 60 * 10 - 60,
							ShotingInterval = 120,
							AxisA = 16 * 6,
							AxisB = 16 * 10,
						};
						return machine;
					}
				case BossState.SummonServants:
					{
						double angleMid = (Center - TargetPlayer.Center).Angle();
						var machine = new SummonServants(this)
						{
							ServantData = new NPCData
							{
								ID = NPCID.IchorSticker,
								Defense = 1500,
								LifeMax = 2500
							},
							Radium = 16 * 6f,
							Count = 5,
							AngleBegin = angleMid - Math.PI / 4,
							AngleEnd = angleMid + Math.PI / 4,
							TotalTime = 60 * 3 * 2,
							Interval = 180,
							SummonDelay = 60 * 0,
							ServantSpeed = 9f
						};
						machine.Linked = new SummonServants(this)
						{
							ServantData = new NPCData
							{
								ID = NPCID.BloodSquid,
								Defense = 100,
								LifeMax = 1500
							},
							Radium = 16 * 6f,
							Count = 5,
							AngleBegin = angleMid - Math.PI / 4,
							AngleEnd = angleMid + Math.PI / 4,
							TotalTime = 60 * 3 * 2,
							Interval = 180,
							SummonDelay = 90,
							ServantSpeed = 9f
						};
						return machine;
					}
				case BossState.BrainEx5:
					{
						return new BrainEx5(this)
						{
							ProjIDs = new int[] { ProjectileID.DeathLaser },
							Damage = 55,
							AccumulationTime = 60 * 6,
							HitDuration = 60 * 2,
							HitRadius = 16 * 80,
							UpingSpeed = 2.4f
						};
					}
			}
			return new SpawnCreeper(this)
			{
				DeflectionAngle = Math.PI / 10,
				Count = 10
			};
		}

		private void ResetMovingMachine()
		{
			Moving.MaxSpeed = 16 * 0.2f;
			Moving.MaxDistance = 16 * 90;
			Moving.WarpingInterval = 60 * 6;
			Moving.WarpingDistance = 16 * 45;
		}
	}
}
