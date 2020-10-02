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
		private int Alpha
		{
			get => (int)TNPC.ai[3];
			set => TNPC.ai[3] = value;
		}

		public BrainEx() : base(NPCID.BrainofCthulhu)
		{
			defLifes = 6;
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
			ProjActing = GetNextProjActing(ProjActing?.State);
			ResetMovingMachine();

			if (ProjActing.State == BossState.BrainBloodDropping)
			{
				Moving.Offset = new Vector2(0, -16 * 25);
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

			ProjActing.Begin();
		}

		private BossStateMachine GetNextProjActing(BossState? last)
		{
			switch (last)
			{
				case BossState.SpawnCreeper:
					{
						return new BrainBloodDropping(this);
					}
				case BossState.BrainBloodDropping:
					{
						var machine = new EllipseShot(this, ProjectileID.DD2DrakinShot, ProjectileID.DD2DrakinShot)
						{
							Rotation = Rand.NextAngle(),
							RotationSpeed = Math.PI / 6 / 60,
							Count = 20,
							Damage = 50,
							TotalTime = 60 * 10,
							AxisA = 16 * 6,
							AxisB = 16 * 10,
						};
						machine.Linked = new EllipseShot(this, ProjectileID.NebulaBolt)
						{
							Rotation = machine.Rotation,
							RotationSpeed = Math.PI / 6 / 60,
							Count = 16,
							Damage = 70,
							ShotingDelay = 30,
							TotalTime = 60 * 10 - 30,
							AxisA = 16 * 6,
							AxisB = 16 * 10,
						};
						return machine;
					}
				case BossState.EllipseShot:
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
							Radium = 16 * 7.5f,
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
								ID = NPCID.SeekerHead,
								Defense = 1500,
								LifeMax = 1000
							},
							Radium = 16 * 7.5f,
							Count = 5,
							AngleBegin = angleMid - Math.PI / 4,
							AngleEnd = angleMid + Math.PI / 4,
							TotalTime = 60 * 3 * 3,
							Interval = 180,
							SummonDelay = 60 * 1,
							ServantSpeed = 4f
						};
						machine.Linked.Linked = new SummonServants(this)
						{
							ServantData = new NPCData
							{
								ID = NPCID.BloodSquid,
								Defense = 500,
								LifeMax = 750
							},
							Radium = 16 * 7.5f,
							Count = 5,
							AngleBegin = angleMid - Math.PI / 4,
							AngleEnd = angleMid + Math.PI / 4,
							TotalTime = 60 * 3 * 3,
							Interval = 180,
							SummonDelay = 60 * 2,
							ServantSpeed = 4f
						};
						return machine;
					}
			}
			return new SpawnCreeper(this)
			{
				DeflectionAngle = Math.PI / 10
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
