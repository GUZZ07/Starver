using Microsoft.Xna.Framework;
using Starvers.NetTricks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace Starvers.Enemies.Bosses
{
	partial class StarverBoss
	{
		#region Machines
		#region FaithOfMountain
		protected class FaithOfMountainMachine : BossStateMachine
		{
			#region Fields

			private int cycle;
			private Queue<ProjLaunchTask> launchTasks;
			private int t;
			private Vector faithCenter;
			private static readonly Vector CenterOffset = new Vector(0, -130 - 50 * 16);

			private const float Radium1 = 8 * 16;
			private const float Radium2 = Radium1 * 1.414f;
			private const float Radium3 = Radium2 * 1.414f;

			private static readonly float Distance1 = (float)Math.Sqrt(2 * Radium1 * Radium1 / 0.690983005625053);
			private static readonly float Distance2 = new Vector(Distance1, Radium2).Length;
			private static readonly float Distance3 = new Vector(Distance2, Radium3).Length;

			private const float LaunchRadium1 = 4 * 16;
			private const float LaunchRadium2 = LaunchRadium1 * 1.414f;
			private const float LaunchRadium3 = LaunchRadium2 * 1.414f;

			private const int HalfRoundCount = 5;
			private const int CountInHalfRound = 4 * 5;

			private const double AngleBig = Math.PI * 2 / HalfRoundCount;
			private const double AngleSmall = Math.PI / CountInHalfRound;

			private static readonly double Angle1 = 0;
			private static readonly double Angle2 = Math.Asin(Radium2 / (Radium1 + Distance1));
			private static readonly double Angle3 = Angle2 + Math.Asin(Radium3 / (Radium2 + Distance2));
			#endregion
			public int ProjType { get; set; }
			/// <summary>
			/// 进行几次
			/// </summary>
			public int Cycle { get; set; }
			public int Damage { get; set; }

			public FaithOfMountainMachine(StarverBoss boss) : base(BossState.FaithOfMountain, boss)
			{
				ProjType = ProjectileID.DD2DarkMageBolt;
				launchTasks = new Queue<ProjLaunchTask>();
				Cycle = 1;
				Damage = 25;
			}

			protected override void InternalUpdate()
			{
				UpdateLaunchTasks();
				UpdateFaithOfMountain();
			}

			private void UpdateLaunchTasks()
			{
				if (launchTasks == null)
				{
					launchTasks = new Queue<ProjLaunchTask>();
				}
				int count = launchTasks.Count;
				while (count-- > 0)
				{
					var task = launchTasks.Dequeue();
					if (!task.CheckLaunch())
					{
						launchTasks.Enqueue(task);
					}
				}
			}

			private void UpdateFaithOfMountain()
			{
				Timer++;
				int shoot = ProjType;
				const float speed = 8;
				const int pair = 4;
				int target = Boss.Target;
				if (faithCenter.Length < 16)
				{
					faithCenter = (Vector)Main.player[target].Center + CenterOffset;
				}
				if (Timer >= 60 * 6 * 2)
				{
					Timer = 0;
					if (++cycle == Cycle)
					{
						IsEnd = true;
						return;
					}
					t = 0;
					faithCenter = (Vector)Main.player[target].Center + CenterOffset;
				}
				#region Normal
				else if (Timer < 60 * 6)
				{
					if (Timer % 3 == 0)
					{
						#region Small
						if (Timer < 60 * 1)
						{
							var t = (Timer - 60 * 0) / 3;
							var center = faithCenter + Vector.FromPolar(Angle1, Distance1);
							var angle = Angle1 + Math.PI + t * AngleSmall + Math.PI / 2;
							var faithCenterToPos = Vector.FromPolar(angle, Radium1) + center - faithCenter;

							var launchCenterAngle = Math.PI / 2 + (Angle1 + (t / pair) * (Math.PI + 2 * Math.PI / 5) / (CountInHalfRound / pair) + Math.PI / 5 - Math.PI / 2);
							var faithCenterTolaunchCenter = Vector.FromPolar(launchCenterAngle, LaunchRadium1) + center - faithCenter;

							for (int i = 0; i < HalfRoundCount; i++)
							{
								var pos = faithCenter + faithCenterToPos;
								var idx = Projectile.NewProjectile(pos, Vector2.Zero, shoot, Damage, 3);
								// TSPlayer.All.SendMessage($"faithCenter: {faithCenter}, pos: {pos}", Color.Blue);
								Main.projectile[idx].timeLeft = 6 * 60 + 30;
								Main.projectile[idx].velocity = Vector2.Zero;
								Main.projectile[idx].aiStyle = -1;
								// Main.projectile[idx].damage = 20;
								NetMessage.SendData(27, -1, -1, null, idx);
								var velocity = pos - (faithCenterTolaunchCenter + faithCenter);
								velocity.PolarRadius = -speed;
								launchTasks.Enqueue(new ProjLaunchTask(idx, velocity, 60 * 3 - Timer));
								faithCenterToPos.Angle += AngleBig;
								faithCenterTolaunchCenter.Angle += AngleBig;
							}
						}
						#endregion
						#region Medium
						else if (Timer < 60 * 2)
						{
							var t = (Timer - 60 * 1) / 3;
							var center = faithCenter + Vector.FromPolar(Angle2, Distance2);
							var angle = Angle2 + Math.PI + t * AngleSmall + Math.PI / 2;
							var faithCenterToPos = Vector.FromPolar(angle, Radium2) + center - faithCenter;

							var launchCenterAngle = Math.PI / 2 + Angle2 + (t / pair) * (Math.PI + 2 * Math.PI / 5) / (CountInHalfRound / pair) - Math.PI / 5;
							var faithCenterTolaunchCenter = Vector.FromPolar(launchCenterAngle, LaunchRadium2) + center - faithCenter;

							for (int i = 0; i < HalfRoundCount; i++)
							{
								var pos = faithCenter + faithCenterToPos;
								var idx = Projectile.NewProjectile(pos, Vector2.Zero, shoot, Damage, 3);
								Main.projectile[idx].timeLeft = 6 * 60 + 30;
								Main.projectile[idx].velocity = Vector2.Zero;
								Main.projectile[idx].aiStyle = -1;
								// Main.projectile[idx].damage = 20;
								NetMessage.SendData(27, -1, -1, null, idx);
								var velocity = pos - (faithCenterTolaunchCenter + faithCenter);
								velocity.PolarRadius = -speed;
								launchTasks.Enqueue(new ProjLaunchTask(idx, velocity, 60 * 3 - Timer + 60));
								faithCenterToPos.Angle += AngleBig;
								faithCenterTolaunchCenter.Angle += AngleBig;
							}
						}
						#endregion
						#region Large
						else if (Timer < 60 * 3)
						{
							var t = (Timer - 60 * 2) / 3;
							var center = faithCenter + Vector.FromPolar(Angle3, Distance3);
							var angle = Angle3 + Math.PI + t * AngleSmall + Math.PI / 2;
							var faithCenterToPos = Vector.FromPolar(angle, Radium3) + center - faithCenter;

							var launchCenterAngle = Math.PI / 2 + Angle3 + (t / pair) * (Math.PI + 2 * Math.PI / 5) / (CountInHalfRound / pair) - Math.PI / 5;
							var faithCenterTolaunchCenter = Vector.FromPolar(launchCenterAngle, LaunchRadium3) + center - faithCenter;

							for (int i = 0; i < HalfRoundCount; i++)
							{
								var pos = faithCenter + faithCenterToPos;
								var idx = Projectile.NewProjectile(pos, Vector2.Zero, shoot, Damage, 3);
								Main.projectile[idx].timeLeft = 6 * 60 + 30;
								Main.projectile[idx].velocity = Vector2.Zero;
								Main.projectile[idx].aiStyle = -1;
								// Main.projectile[idx].damage = 20;
								NetMessage.SendData(27, -1, -1, null, idx);
								var velocity = pos - (faithCenterTolaunchCenter + faithCenter);
								velocity.PolarRadius = speed;
								launchTasks.Enqueue(new ProjLaunchTask(idx, -velocity, 60 * 3 - Timer + 60 * 2));
								faithCenterToPos.Angle += AngleBig;
								faithCenterTolaunchCenter.Angle += AngleBig;
							}
						}
						#endregion
					}
				}
				#endregion
				#region Reversed
				else
				{
					if (Timer % 3 == 0)
					{
						#region Small
						if (Timer < 60 * 7)
						{
							var t = (Timer - 60 * 6) / 3;
							var center = faithCenter + Vector.FromPolar(-Angle1, Distance1);
							var angle = -Angle1 - t * AngleSmall + Math.PI / 2;
							Vector faithCenterToPos = Vector.FromPolar(angle, Radium1) + center - faithCenter;

							var launchCenterAngle = Math.PI * 3 / 2 - Angle1 - (t / pair) * (Math.PI + 2 * Math.PI / 5) / (CountInHalfRound / pair) + Math.PI / 5;
							var faithCenterTolaunchCenter = Vector.FromPolar(launchCenterAngle, LaunchRadium1) + center - faithCenter;

							for (int i = 0; i < HalfRoundCount; i++)
							{
								var pos = faithCenter + faithCenterToPos;
								var idx = Projectile.NewProjectile(pos, Vector2.Zero, shoot, Damage, 3);
								Main.projectile[idx].timeLeft = 6 * 60 + 30;
								Main.projectile[idx].velocity = Vector2.Zero;
								Main.projectile[idx].aiStyle = -1;
								// Main.projectile[idx].damage = 20;
								NetMessage.SendData(27, -1, -1, null, idx);
								Vector velocity = pos - (faithCenterTolaunchCenter + faithCenter);
								velocity.PolarRadius = speed;
								launchTasks.Enqueue(new ProjLaunchTask(idx, -velocity, 60 * 3 - Timer + 60 * 6));
								faithCenterToPos.Angle -= AngleBig;
								faithCenterTolaunchCenter.Angle -= AngleBig;
							}
						}
						#endregion
						#region Medium
						else if (Timer < 60 * 8)
						{
							var t = (Timer - 60 * 7) / 3;
							var center = faithCenter + Vector.FromPolar(-Angle2, Distance2);
							var angle = -Angle2 - t * AngleSmall + Math.PI / 2;
							var faithCenterToPos = Vector.FromPolar(angle, Radium2) + center - faithCenter;

							var launchCenterAngle = -Angle2 - (t / pair) * (Math.PI + 2 * Math.PI / 5) / (CountInHalfRound / pair) + Math.PI / 5 + Math.PI * 3 / 2;
							var faithCenterTolaunchCenter = Vector.FromPolar(launchCenterAngle, LaunchRadium2) + center - faithCenter;

							for (int i = 0; i < HalfRoundCount; i++)
							{
								var pos = faithCenter + faithCenterToPos;
								var idx = Projectile.NewProjectile(pos, Vector2.Zero, shoot, Damage, 3);
								Main.projectile[idx].timeLeft = 6 * 60 + 30;
								Main.projectile[idx].velocity = Vector2.Zero;
								Main.projectile[idx].aiStyle = -1;
								// Main.projectile[idx].damage = 20;
								NetMessage.SendData(27, -1, -1, null, idx);
								Vector velocity = pos - (faithCenterTolaunchCenter + faithCenter);
								velocity.PolarRadius = speed;
								launchTasks.Enqueue(new ProjLaunchTask(idx, -velocity, 60 * 3 - Timer + 60 * 7));
								faithCenterToPos.Angle -= AngleBig;
								faithCenterTolaunchCenter.Angle -= AngleBig;
							}
						}
						#endregion
						#region Large
						else if (Timer < 60 * 9)
						{
							var t = (Timer - 60 * 8) / 3;
							var center = faithCenter + Vector.FromPolar(-Angle3, Distance3);
							var angle = -Angle3 - t * AngleSmall + Math.PI / 2;
							var faithCenterToPos = Vector.FromPolar(angle, Radium3) + center - faithCenter;

							var launchCenterAngle = -Angle3 - (t / pair) * (Math.PI + 2 * Math.PI / 5) / (CountInHalfRound / pair) + Math.PI * 3 / 2 + Math.PI / 5;
							var faithCenterTolaunchCenter = Vector.FromPolar(launchCenterAngle, LaunchRadium3) + center - faithCenter;

							for (int i = 0; i < HalfRoundCount; i++)
							{
								var pos = faithCenter + faithCenterToPos;
								var idx = Projectile.NewProjectile(pos, Vector2.Zero, shoot, Damage, 3);
								Main.projectile[idx].timeLeft = 6 * 60 + 30;
								Main.projectile[idx].velocity = Vector2.Zero;
								Main.projectile[idx].aiStyle = -1;
								// Main.projectile[idx].damage = 20;
								NetMessage.SendData(27, -1, -1, null, idx);
								var velocity = pos - (faithCenterTolaunchCenter + faithCenter);
								velocity.PolarRadius = speed;
								launchTasks.Enqueue(new ProjLaunchTask(idx, -velocity, 60 * 3 - Timer + 60 * 8));
								faithCenterToPos.Angle -= AngleBig;
								faithCenterTolaunchCenter.Angle -= AngleBig;
							}
						}
						#endregion
					}
				}
				#endregion
			}

		}
		#endregion
		#region FakeDuckRush
		// 朱砂左右都不超过25 * 16像素
		protected class FakeDuckRushMachine : BossStateMachine
		{
			private int released;
			private int[] useWhich;

			public int Count { get; set; }
			public int ReleaseInterval { get; set; }
			public FakeDuckRushMachine(StarverBoss boss) : base(BossState.FakeDuckRush, boss)
			{
				ReleaseInterval = 30;
				Count = 6;
			}
			public override void Begin()
			{
				useWhich = new int[Count];
				base.Begin();
			}
			protected override void InternalUpdate()
			{
				if (Timer++ % (ReleaseInterval + 1) == 0)
				{
					ReleaseDuck();
					if (++released == Count)
					{
						IsEnd = true;
						return;
					}
				}
			}

			private bool UnUsed(int index)
			{
				for (int i = 0; i < released; i++)
				{
					if (useWhich[i] == index)
					{
						return false;
					}
				}
				return true;
			}

			private void ReleaseDuck()
			{
				int dir = Boss.Rand.NextDirection();
				int index = 200;
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					if (!Main.npc[i].active && UnUsed(i))
					{
						index = i;
						useWhich[released] = i;
						break;
					}
				}
				FakeNPC duke = new FakeNPC();
				duke.Index = (byte)index;
				duke.NetID = NPCID.DukeFishron;
				duke.Target = Boss.Target;
				duke.Life = 3000;
				duke.Direction = (byte)dir;
				duke.Velocity = dir * new Vector2(23, 0);
				duke.Position = Boss.TargetPlayer.Center - dir * new Vector2(25 * 16, 0);
				duke.NPCai[0] = 1;
				duke.NPCai[1] = 0;
				duke.NPCai[2] = 3;
				duke.NPCai[3] = 0;
				duke.SendData();
			}
		}
		#endregion
		#region PlayerTracker
		protected class PlayerTracker : BossStateMachine
		{
			public Vector2 Offset { get; set; }
			public float? MaxSpeed { get; set; }
			public float SlowDownFactor { get; set; }
			public bool IsPause { get; set; }
			public PlayerTracker(StarverBoss boss) : base(BossState.TrackingPlayer, boss)
			{
				SlowDownFactor = 20;
			}
			protected override void InternalUpdate()
			{
				if (IsPause)
				{
					return;
				}
				Boss.Velocity = Boss.TargetPlayer.Center + Offset - Boss.Center;
				Boss.Velocity /= 20;
				if (MaxSpeed != null && Boss.Velocity.Length() > MaxSpeed)
				{
					Boss.Velocity.Length((float)MaxSpeed);
				}
				if (Timer++ % 3 == 0)
				{
					Boss.UpdateToClient();
				}
			}
		}
		#endregion
		#region EvilTrident
		protected class EvilTridentMachine : BossStateMachine
		{
			private int created;
			private int launched;
			private ProjLaunchTask[] projs;

			public int Damage { get; set; }
			public int TridentCount { get; set; }
			public int CreateInterval { get; set; }
			public int LaunchDelay { get; set; }
			public int TridentHitPlayerTime { get; set; }
			public EvilTridentMachine(StarverBoss boss) : base(BossState.EvilTrident, boss)
			{
				Damage = 100;
				TridentCount = 28;
				CreateInterval = 5;
				LaunchDelay = 30;
				TridentHitPlayerTime = 20;
			}
			public override void Begin()
			{
				projs = new ProjLaunchTask[TridentCount];
				base.Begin();
			}
			protected override void InternalUpdate()
			{
				Timer++;
				if (created < TridentCount)
				{
					if (Timer % CreateInterval == 0)
					{
						var source = GetLaunchSource();
						var velocity = -source / TridentHitPlayerTime;
						var index = Boss.NewProj(Boss.TargetPlayer.Center + source, source.ToLenOf(-1f), ProjectileID.UnholyTridentHostile, Damage);
						projs[created++] = new ProjLaunchTask(index, velocity, LaunchDelay);
					}
				}
				if (launched < TridentCount)
				{
					for (int i = launched; i < created; i++)
					{
						if (projs[i].CheckLaunch())
						{
							launched++;
						}
					}
				}
				else
				{
					IsEnd = true;
					return;
				}
			}
			public override void Abort()
			{
				for (int i = 0; i < created; i++)
				{
					projs[i].Cancel();
				}
				base.Abort();
			}

			private Vector2 GetLaunchSource()
			{
				var dir = Boss.Rand.NextDirection();
				var Y = Boss.Rand.NextFloat(-16 * 7.5f, 16 * 7.5f);
				return new Vector2(dir * 16 * 30, Y);
			}
		}
		#endregion
		#endregion
		#region Base
		protected abstract class BossStateMachine
		{
			public BossState State { get; }
			public StarverBoss Boss { get; }
			public int Timer { get; protected set; }
			public bool IsEnd { get; protected set; }
			protected BossStateMachine(BossState state, StarverBoss boss)
			{
				State = state;
				Boss = boss;
			}
			public void Update()
			{
				if (IsEnd)
				{
#if DEBUG
					throw new InvalidOperationException();
#endif
					return;
				}
				InternalUpdate();
			}
			public virtual void Begin() { }
			public virtual void Abort() { }
			protected abstract void InternalUpdate();
		}
		#endregion

		protected enum BossState
		{
			EyeRush,
			EyeSummon,
			EvilTrident,
			TrackingPlayer,
			FakeDuckRush,
			FaithOfMountain
		}
	}
}
