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
		#region Data Structures
		protected struct NPCData
		{
			public int ID;
			public int LifeMax;
			public int Defense;

			public int SummonNPC(Vector2 pos, Vector2 velocity = default)
			{
				int idx = NPC.NewNPC((int)pos.X, (int)pos.Y, ID);
				Main.npc[idx].lifeMax = LifeMax;
				Main.npc[idx].life = LifeMax;
				Main.npc[idx].defense = Defense;
				Main.npc[idx].velocity = velocity;
				Main.npc[idx].SpawnedFromStatue = true;
				Main.npc[idx].SendData();
				return idx;
			}
		}
		protected struct ProjCurveData
		{
			public int ID;
			public double ParaBegin;
			public double ParaEnd;
			public double Increment;
			public int Damage;
			public int Knockback;
			public Func<double, Vector2> FuncPos;
			public Func<double, Vector2> FuncVel;

			public void Apply(Vector2 Center)
			{
				double t = ParaBegin;
				while (t <= ParaEnd)
				{
					Utils.NewProj(Center + FuncPos(t), FuncVel(t), ID, Damage, Knockback);
					t += Increment;
				}
			}
		}
		#endregion
		#region EyeSharknado
		protected class EyeSharknadoMachine : BossStateMachine
		{
			private double angle;
			private Vector vector;
			private Vector vel;

			public int Damage { get; set; }
			public EyeSharknadoMachine(StarverBoss boss) : base(BossState.EyeSharknado, boss)
			{
				Damage = 50;
			}

			protected override void InternalUpdate()
			{
				if (++Timer == 60 * 6)
				{
					IsEnd = true;
					return;
				}
				if (Timer % 45 == 0)
				{
					angle += Math.PI / 14;
					vector = (Vector)Boss.RelativePos;
					vector.Length = 16;
					vel = vector;
					vel.Angle += angle;
					vector.Angle -= angle;
					Boss.NewProj(vector, ProjectileID.SharknadoBolt, Damage);
					Boss.NewProj(vel, ProjectileID.SharknadoBolt, Damage);
				}
			}
		}
		#endregion
		#region GazingYou
		protected class GazingYouMachine : BossStateMachine
		{
			public int Damage { get; set; }
			public int TotalTime { get; set; }
			public Color MessageColor { get; set; }
			public GazingYouMachine(StarverBoss boss) : base(BossState.GazingYou, boss)
			{
				Damage = 40;
				TotalTime = 60 * 8;
				MessageColor = Color.Magenta;

				OnEnd += delegate
				{
					if (Boss.Active)
					{
						Boss.DontTakeDamage = false;
					}
				};
			}

			public override void Begin()
			{
				Boss.TargetPlayer?.SendText("盯着你...", MessageColor);
				Boss.DontTakeDamage = true;
				base.Begin();
			}

			public override void Abort()
			{
				if (Boss.Active)
				{
					Boss.DontTakeDamage = false;
				}
				base.Abort();
			}

			protected override void InternalUpdate()
			{
				var player = Boss.TargetPlayer;
				player.AddBuffIfNot(BuffID.Obstructed, 90);
				if (Timer % 60 == 0)
				{
					Boss.ProjCircle(player.Center, 16 * 30, -10, ProjectileID.RuneBlast, Rand.Next(4, 6), Damage);
				}
				if (++Timer == TotalTime)
				{
					IsEnd = true;
				}
			}
		}
		#endregion
		#region FaithOfMountain
		protected class FaithOfMountainMachine : BossStateMachine
		{
			#region Fields

			private int cycle;
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
				Cycle = 1;
				Damage = 25;
			}

			protected override void InternalUpdate()
			{
				UpdateFaithOfMountain();
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
				if (Timer == 60 * 6)
				{
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
								var idx = Projectile.NewProjectile(Boss.GetProjSource(), pos, Vector2.Zero, shoot, Damage, 3);
								// TSPlayer.All.SendMessage($"faithCenter: {faithCenter}, pos: {pos}", Color.Blue);
								Main.projectile[idx].timeLeft = 6 * 60 + 30;
								Main.projectile[idx].velocity = Vector2.Zero;
								Main.projectile[idx].aiStyle = -1;
								// Main.projectile[idx].damage = 20;
								NetMessage.SendData(27, -1, -1, null, idx);
								var velocity = pos - (faithCenterTolaunchCenter + faithCenter);
								velocity.PolarRadius = -speed;
								Starver.Instance.ProjsController.Add(new ProjLauncher(idx, velocity, 60 * 3 - Timer));
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
								var idx = Projectile.NewProjectile(Boss.GetProjSource(), pos, Vector2.Zero, shoot, Damage, 3);
								Main.projectile[idx].timeLeft = 6 * 60 + 30;
								Main.projectile[idx].velocity = Vector2.Zero;
								Main.projectile[idx].aiStyle = -1;
								// Main.projectile[idx].damage = 20;
								NetMessage.SendData(27, -1, -1, null, idx);
								var velocity = pos - (faithCenterTolaunchCenter + faithCenter);
								velocity.PolarRadius = -speed;
								Starver.Instance.ProjsController.Add(new ProjLauncher(idx, velocity, 60 * 3 - Timer + 60));
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
								var idx = Projectile.NewProjectile(Boss.GetProjSource(), pos, Vector2.Zero, shoot, Damage, 3);
								Main.projectile[idx].timeLeft = 6 * 60 + 30;
								Main.projectile[idx].velocity = Vector2.Zero;
								Main.projectile[idx].aiStyle = -1;
								// Main.projectile[idx].damage = 20;
								NetMessage.SendData(27, -1, -1, null, idx);
								var velocity = pos - (faithCenterTolaunchCenter + faithCenter);
								velocity.PolarRadius = speed;
								Starver.Instance.ProjsController.Add(new ProjLauncher(idx, -velocity, 60 * 3 - Timer + 60 * 2));
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
								var idx = Projectile.NewProjectile(Boss.GetProjSource(), pos, Vector2.Zero, shoot, Damage, 3);
								Main.projectile[idx].timeLeft = 6 * 60 + 30;
								Main.projectile[idx].velocity = Vector2.Zero;
								Main.projectile[idx].aiStyle = -1;
								// Main.projectile[idx].damage = 20;
								NetMessage.SendData(27, -1, -1, null, idx);
								Vector velocity = pos - (faithCenterTolaunchCenter + faithCenter);
								velocity.PolarRadius = speed;
								Starver.Instance.ProjsController.Add(new ProjLauncher(idx, -velocity, 60 * 3 - Timer + 60 * 6));
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
								var idx = Projectile.NewProjectile(Boss.GetProjSource(), pos, Vector2.Zero, shoot, Damage, 3);
								Main.projectile[idx].timeLeft = 6 * 60 + 30;
								Main.projectile[idx].velocity = Vector2.Zero;
								Main.projectile[idx].aiStyle = -1;
								// Main.projectile[idx].damage = 20;
								NetMessage.SendData(27, -1, -1, null, idx);
								Vector velocity = pos - (faithCenterTolaunchCenter + faithCenter);
								velocity.PolarRadius = speed;
								Starver.Instance.ProjsController.Add(new ProjLauncher(idx, -velocity, 60 * 3 - Timer + 60 * 7));
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
								var idx = Projectile.NewProjectile(Boss.GetProjSource(), pos, Vector2.Zero, shoot, Damage, 3);
								Main.projectile[idx].timeLeft = 6 * 60 + 30;
								Main.projectile[idx].velocity = Vector2.Zero;
								Main.projectile[idx].aiStyle = -1;
								// Main.projectile[idx].damage = 20;
								NetMessage.SendData(27, -1, -1, null, idx);
								var velocity = pos - (faithCenterTolaunchCenter + faithCenter);
								velocity.PolarRadius = speed;
								Starver.Instance.ProjsController.Add(new ProjLauncher(idx, -velocity, 60 * 3 - Timer + 60 * 8));
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
		#region FakeDukeRush
		// 朱砂左右都不超过25 * 16像素
		protected class FakeDukeRushMachine : BossStateMachine
		{
			private int released;
			private int[] useWhich;
			private int dir;

			public int Count { get; set; }
			public int ReleaseInterval { get; set; }
			public FakeDukeRushMachine(StarverBoss boss) : base(BossState.FakeDukeRush, boss)
			{
				ReleaseInterval = 50;
				Count = 6;
			}
			public override void Begin()
			{
				dir = Rand.NextDirection();
				useWhich = new int[Count];
				base.Begin();
			}
			protected override void InternalUpdate()
			{
				if (Timer++ % (ReleaseInterval + 1) == 0)
				{
					ReleaseDuke();
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

			private void ReleaseDuke()
			{
				dir *= -1;
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
			/// <summary>
			/// 超过后会瞬移到玩家身边
			/// </summary>
			public float? MaxDistance { get; set; }
			public float? MaxSpeed { get; set; }
			/// <summary>
			/// boss.Velocity /= SlowDownFactor
			/// </summary>
			public float SlowDownFactor { get; set; }
			public int? WarpingInterval
			{
				get;
				set;
			}
			public float WarpingDistance
			{
				get;
				set;
			}
			public bool DontMove { get; set; }
			public bool TrackingEveryWhere { get; set; }
			public PlayerTracker(StarverBoss boss) : base(BossState.TrackingPlayer, boss)
			{
				SlowDownFactor = 20;
				TrackingEveryWhere = true;
			}
			protected override void InternalUpdate()
			{
				#region EnsureTarget
				VerifyTarget();
				if (!Boss.TNPC.HasValidTarget)
				{
					Boss.TurnToAir();
					return;
				}
				if (MaxDistance != null && Boss.DistanceToTarget() > MaxDistance)
				{
					if (TrackingEveryWhere)
					{
						var distance = 16 * 100f;
						if (MaxDistance is float md)
						{
							distance = Math.Min(distance, md);
						}
						Boss.Center = Boss.TargetPlayer.Center + Rand.NextVector2(distance);
					}
					else
					{
						Boss.TurnToAir();
						return;
					}
				}
				#endregion
				if (DontMove)
				{
					return;
				}
				Boss.Velocity = Boss.TargetPlayer.Center + Offset - Boss.Center;
				Boss.Velocity /= SlowDownFactor;
				if (MaxSpeed != null && Boss.Velocity.Length() > MaxSpeed)
				{
					Boss.Velocity.Length((float)MaxSpeed);
				}
				if (Timer % WarpingInterval == 0)
				{
					var distance = WarpingDistance;
					if (MaxDistance is float md)
					{
						distance = Math.Min(distance, md);
					}
					Boss.Center = Boss.TargetPlayer.Center + Rand.NextVector2(distance);
				}
				if (Timer % 3 == 0)
				{
					Boss.UpdateToClient();
				}
				Timer++;
			}

			private void VerifyTarget()
			{
				if (Boss.TargetPlayer?.Alive != true)
				{
					FindTarget();
				}
			}

			private void FindTarget()
			{
				StarverPlayer target = Starver.Instance.Players.FindPlayerClosestTo(Boss.Center);
				Boss.Target = target?.Index ?? -1;
			}
		}
		#endregion
		#region EvilTrident
		protected class EvilTridentMachine : BossStateMachine
		{
			private int created;
			private int[] projs;

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
				projs = new int[TridentCount];
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
						projs[created++] = index;
						Starver.Instance.ProjsController.Add(new ProjLauncher(index, velocity, LaunchDelay));
					}
				}
				else if (Timer > CreateInterval * TridentCount + LaunchDelay * 3)
				{
					IsEnd = true;
					return;
				}
			}
			public override void Abort()
			{
				for (int i = 0; i < created; i++)
				{
					Starver.Instance.ProjsController.CancelLaunch(projs[i]);
				}
				base.Abort();
			}

			private Vector2 GetLaunchSource()
			{
				var dir = Rand.NextDirection();
				var Y = Rand.NextFloat(-16 * 7.5f, 16 * 7.5f);
				return new Vector2(dir * 16 * 30, Y);
			}
		}
		#endregion
		#region SpawnCreeper
		protected class SpawnCreeper : BossStateMachine
		{
			private NPC[] creepers;
			private int[] shotingDelays;
			public int LaserShotingInterval
			{
				get;
				set;
			}
			public int Count
			{
				get;
				set;
			}
			public int CreeperDefense
			{
				get;
				set;
			}
			public int CreeperLifeMax
			{
				get;
				set;
			}
			public int LaserDamage
			{
				get;
				set;
			}
			public float LaserSpeed
			{
				get;
				set;
			}
			/// <summary>
			/// 实际偏转为[-θ,+θ]
			/// </summary>
			public double DeflectionAngle
			{
				get;
				set;
			}
			public SpawnCreeper(StarverBoss boss) : base(BossState.SpawnCreeper, boss)
			{
				Count = 10;
				CreeperDefense = 300;
				CreeperLifeMax = 5000;
				LaserShotingInterval = 120;
				LaserDamage = 40;
				LaserSpeed = 14;

				OnEnd += delegate
				{
					Boss.DontTakeDamage = false;
				};
			}

			public override void Begin()
			{
				base.Begin();
				Boss.DontTakeDamage = true;

				SummonCreepers();
			}
			public override void Abort()
			{
				base.Abort();
				Boss.DontTakeDamage = false;
				foreach (var creeper in creepers)
				{
					if (creeper.active)
					{
						creeper.active = false;
						creeper.SendData();
					}
				}
			}
			protected override void InternalUpdate()
			{
				bool alive = UpdateCreepers();
				if (!alive)
				{
					IsEnd = true;
					return;
				}
				Timer++;
			}

			private bool UpdateCreepers()
			{
				int aliveCount = 0;
				for (int i = 0; i < creepers.Length; i++)
				{
					var creeper = creepers[i];
					if (!creeper.active)
					{
						continue;
					}
					if (Timer % LaserShotingInterval == shotingDelays[i])
					{
						CreeperShot(creeper);
					}
					aliveCount++;
				}
				return aliveCount > 0;
			}
			private void SummonCreepers()
			{
				shotingDelays = new int[Count];
				for (int i = 0; i < Count; i++)
				{
					shotingDelays[i] = Rand.Next(LaserShotingInterval);
				}
				creepers = new NPC[Count];
				for (int i = 0; i < Count; i++)
				{
					var pos = Boss.Center + Vector.FromPolar(2 * Math.PI / Count * i, 16 * 25);
					int idx = NPC.NewNPC((int)pos.X, (int)pos.Y, NPCID.Creeper);
					creepers[i] = Main.npc[idx];
					creepers[i].velocity = (pos - Boss.Center).ToLenOf(15);
					creepers[i].lifeMax = CreeperLifeMax;
					creepers[i].life = CreeperLifeMax;
					creepers[i].defense = CreeperDefense;
					creepers[i].SpawnedFromStatue = true;
					creepers[i].SendData();
				}
			}
			private void CreeperShot(NPC creeper)
			{
				Vector2 direction = Boss.TargetPlayer.Center - creeper.Center;
				direction.Length(LaserSpeed);
				direction.AngleAdd(Rand.NextDouble(-DeflectionAngle, DeflectionAngle));
				Boss.NewProj(creeper.Center, direction, ProjectileID.EyeLaser, LaserDamage, 0);
			}
		}
		#endregion
		#region BrainBloodShot
		protected class BrainBloodDropping : BossStateMachine
		{
			public int TotalTime
			{
				get;
				set;
			}
			public int DroppingInterval
			{
				get;
				set;
			}
			public float DroppingSpeed
			{
				get;
				set;
			}
			/// <summary>
			/// 全宽度(不是半宽度)
			/// </summary>
			public float DroppingWidth
			{
				get;
				set;
			}
			public int Damage
			{
				get;
				set;
			}

			public BrainBloodDropping(StarverBoss boss) : base(BossState.BrainBloodDropping, boss)
			{
				TotalTime = 8 * 60;
				DroppingInterval = 3;
				Damage = 35;
				DroppingWidth = 16 * 150;
				DroppingSpeed = 12;
			}

			protected override void InternalUpdate()
			{
				if (Timer % DroppingInterval == 0)
				{
					var pos = Boss.Center + Rand.NextVector2(DroppingWidth, 0);
					var vel = new Vector2(0, DroppingSpeed);
					Boss.NewProj(pos, vel, ProjectileID.BloodShot, Damage, 0);
				}
				if (Timer++ == TotalTime)
				{
					IsEnd = true;
				}
			}
		}
		#endregion
		#region EllipseShot
		protected class EllipseShot : BossStateMachine
		{
			private int t;

			public int ShotingDelay
			{
				get;
				set;
			}
			public double RotationSpeed
			{
				get;
				set;
			}
			public double Rotation
			{
				get;
				set;
			}
			public float AxisA
			{
				get;
				set;
			}
			public float AxisB
			{
				get;
				set;
			}
			public int Count
			{
				get;
				set;
			}
			public int[] ProjIDs
			{
				get;
				set;
			}
			public int Damage
			{
				get;
				set;
			}
			public float Speed
			{
				get;
				set;
			}
			public float Knockback
			{
				get;
				set;
			}
			public int ShotingInterval
			{
				get;
				set;
			}
			public int TotalTime
			{
				get;
				set;
			}

			public EllipseShot(StarverBoss boss, params int[] projIDs) : base(BossState.EllipseShot, boss)
			{
				ProjIDs = projIDs;
				Count = 12;
				AxisA = 16 * 06 * 1.25f;
				AxisB = 16 * 10 * 1.25f;
				TotalTime = 60 * 10;
				ShotingInterval = 60;
				Speed = 8;
				Damage = 50;
			}

			protected override void InternalUpdate()
			{
				if (Timer % ShotingInterval == ShotingDelay)
				{
					var projID = ProjIDs[t++];
					double angle = Rotation;
					for (int i = 0; i < Count; i++)
					{
						var pos = Vector.FromPolar(Math.PI * 2 / Count * i, AxisA);
						var velocity = pos.ToLenOf(Speed);

						pos.Y *= AxisB / AxisA;
						velocity.Y *= AxisB / AxisA;

						pos.Angle += angle;
						velocity.Angle += angle;

						Boss.NewProj(Boss.Center + pos, velocity, projID, Damage, Knockback);
					}

					t %= ProjIDs.Length;
				}
				Rotation += RotationSpeed;
				if (Timer++ == TotalTime)
				{
					IsEnd = true;
				}
			}
		}
		#endregion
		#region SummonServants
		protected class SummonServants : BossStateMachine
		{
			public int TotalTime
			{
				get;
				set;
			}
			public int Interval
			{
				get;
				set;
			}
			public int SummonDelay
			{
				get;
				set;
			}
			public double AngleBegin
			{
				get;
				set;
			}
			public double AngleEnd
			{
				get;
				set;
			}
			public float Radium
			{
				get;
				set;
			}
			public int Count
			{
				get;
				set;
			}
			public NPCData ServantData
			{
				get;
				set;
			}
			public float ServantSpeed
			{
				get;
				set;
			}
			public SummonServants(StarverBoss boss) : base(BossState.SummonServants, boss)
			{

			}

			protected override void InternalUpdate()
			{
				if (Timer % Interval == SummonDelay)
				{
					double angle = AngleBegin;

					double increment = (AngleBegin - AngleEnd) * (Count + 1) / Count;

					for (int i = 0; i < Count; i++)
					{
						var pos = Vector.FromPolar(angle, Radium);
						ServantData.SummonNPC(Boss.Center + pos, pos.ToLenOf(ServantSpeed));
						angle += increment;
					}
				}
				if (Timer++ == TotalTime)
				{
					IsEnd = true;
				}
			}
		}
		#endregion
		#region BrainEx5
		/// <summary>
		/// 
		/// </summary>
		protected class BrainEx5 : BossStateMachine
		{
			private int[] hitTypes;
			private Vector2[] hitPoints;
			public int Damage
			{
				get;
				set;
			}
			public int[] ProjIDs
			{
				get;
				set;
			}
			public int AccumulationTime
			{
				get;
				set;
			}
			public int HitDuration
			{
				get;
				set;
			}
			public float HitRadius
			{
				get;
				set;
			}
			public float UpingSpeed
			{
				get;
				set;
			}

			public BrainEx5(StarverBoss boss) : base(BossState.BrainEx5, boss)
			{

			}

			public override void Begin()
			{
				base.Begin();
				Boss.NewProj(Vector2.Zero, ProjectileID.BloodNautilusTears, -1);
			}
			protected override void InternalUpdate()
			{
				if (Timer < AccumulationTime)
				{
					int time = AccumulationTime - Timer;
					time = (int)(time / (float)AccumulationTime * 10);
					time = (int)MathHelper.Clamp(time, 1, 10);
					if (Timer < AccumulationTime - 30 && Timer % (30 / time) == 0)
					{
						AccumulationEffect();
						AccumulationEffect();
						AccumulationEffect();
					}
				}
				else if (Timer == AccumulationTime)
				{
					hitPoints = Starver.Instance
						.Players
						.Where(player => player?.Alive == true && Boss.TNPC.Distance(player.Center) < HitRadius)
						.Select(player => player.Center + new Vector2(0, player.TPlayer.height / 2))
						.ToArray();
					hitTypes = new int[hitPoints.Length];
					for (int i = 0; i < hitTypes.Length; i++)
					{
						hitTypes[i] = ProjIDs.Next();
					}
					for (int i = 0; i < hitPoints.Length; i++)
					{
						Boss.ProjCircle(hitPoints[i], 16 * 8, 0.1f, ProjectileID.CrimsonSpray, 36, -1);
					}
					Boss.ProjCircle(Boss.Center, 16 * 8, 0.1f, ProjectileID.CrimsonSpray, 36, -1);
					Boss.ProjCircle(Boss.Center, 16 * 01, 4.5f, ProjectileID.DeathLaser, 120, 120);
				}
				else if (Timer - AccumulationTime < HitDuration)
				{
					if (Timer % 15 == 0)
					{
						Vector halfWidth = (16 * 5.5, 0);
						Vector vel = (0, -UpingSpeed);
						for (int i = 0; i < hitPoints.Length; i++)
						{
							Boss.ProjLine(hitPoints[i] - halfWidth, hitPoints[i] + halfWidth, vel, 5, Damage, hitTypes[i]);
						}
					}
				}
				else
				{
					IsEnd = true;
				}
				Timer++;
			}

			private void AccumulationEffect()
			{
				var time = AccumulationTime - Timer;

				var pos = Rand.NextVector2(Rand.Next(16 * 15, 16 * 45));
				var vel = pos.ToLenOf(-pos.Length() / time / 2.5f);
				var idx = Boss.NewProjNoBC(Boss.Center + pos, vel, ProjectileID.DeathLaser, -1, 0);
				Main.projectile[idx].timeLeft = (int)Math.Ceiling(pos.Length() / vel.Length());
				Main.projectile[idx].SendData();
			}
		}
		#endregion
		#region ShotFromPlayers
		/// <summary>
		/// 以每个玩家为中心向外发射
		/// </summary>
		protected class ShotFromPlayers : BossStateMachine
		{
			public int TotalTime
			{
				get;
				set;
			}
			public int LaunchInterval
			{
				get;
				set;
			}
			public int LaunchDelay
			{
				get;
				set;
			}
			public float HitRadius
			{
				get;
				set;
			}
			public ProjCurveData ProjData
			{
				get;
				set;
			}
			public ShotFromPlayers(StarverBoss boss) : base(BossState.ShotFromPlayers, boss)
			{

			}

			protected override void InternalUpdate()
			{
				if (Timer % LaunchInterval == LaunchDelay)
				{
					foreach (var player in Starver.Instance.Players)
					{
						if (player?.Alive != true || Boss.TNPC.Distance(player.Center) > HitRadius)
						{
							continue;
						}
						ProjData.Apply(player.Center);
					}
				}
				if (++Timer == TotalTime)
				{
					IsEnd = true;
				}
			}
		}
		#endregion
		#region Base
		protected abstract class BossStateMachine
		{
			public BossState State { get; }
			public StarverBoss Boss { get; }
			public int Timer { get; protected set; }
			public bool IsEnd { get; protected set; }
			public bool IsPause { get; set; }
			/// <summary>
			/// 在自身update完调用update
			/// </summary>
			public BossStateMachine Linked
			{
				get;
				set;
			}

			public event Action OnEnd;

			protected Random Rand => Boss.Rand;

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
				if (IsPause || !Boss.Active)
				{
					return;
				}
				InternalUpdate();
				if (Linked?.IsEnd == false)
				{
					Linked.Update();
				}
				if (IsEnd)
				{
					End();
				}
			}
			public virtual void Begin()
			{
				Linked?.Begin();
			}
			public virtual void Abort()
			{
				Linked.Abort();
			}
			private void End()
			{
				OnEnd?.Invoke();
			}
			protected abstract void InternalUpdate();
		}
		#endregion

		protected enum BossState
		{
			EyeRush,
			EyeSummon,
			EvilTrident,
			TrackingPlayer,
			FakeDukeRush,
			FaithOfMountain,
			GazingYou,
			EyeSharknado,
			SpawnCreeper,
			BrainBloodDropping,
			EllipseShot,
			SummonServants,
			BrainEx5,
			ShotFromPlayers
		}
	}
}
