using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace Starvers.NPCSystem.NPCs
{
	using Debug = System.Diagnostics.Debug;
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public class ElfHeliEx : StarverNPC
	{
		#region Fields
		public const int MaxShots = 8;
		/// <summary>
		/// 0: 守卫 1: 追击 2: 巡逻 3: 逃跑 4: 巡逻追击
		/// </summary>
		private byte work = 1;
		private Action shot;
		private float requireDistance;
		private Vector2 targetPos;
		private Vector2 myVector;
		private int t;
		private BitsByte flags;
		#endregion
		#region Properties
		private bool TrackingPlayerIfCloseTo
		{
			get => flags[0];
			set => flags[0] = value;
		}
		protected override float CollidingIndex => DamageIndex;
		public override float DamageIndex { get; set; } = 1;
		#endregion
		#region Ctor
		public ElfHeliEx()
		{
			RawType = NPCID.ElfCopter;
			DefaultLife = 5000;
			Checker = SpawnChecker.SpecialSpawn;
			CollideDamage = 100;
			DamagedIndex = 0.05f;
			AIStyle = None;
			NoGravity = true;
			requireDistance = 16 * 40;
		}
		#endregion
		#region Spawn
		public override void OnSpawn()
		{
			base.OnSpawn();
			RealNPC.lifeMax = DefaultLife;
			RealNPC.life = DefaultLife;
			int shotType = Rand.Next(MaxShots + 4);
			if (shotType >= MaxShots)
			{
				shotType = 2;
			}
			else if (shotType == 6)
			{
				DamagedIndex *= 10;
			}
			SetShot(shotType);
		}
		#endregion
		#region AI
		protected override void RealAI()
		{
			if(RealNPC.type != NPCID.ElfCopter)
			{
				Starver.NPCs[Index] = NPCs[Index] = null;
				_active = false;
				return;
			}
			switch (work)
			{
				case 0:
					AI_Guard();
					break;
				case 1:
					AI_Attack();
					break;
				case 2:
					AI_Wonder();
					break;
				case 3:
					AI_Escape();
					break;
				case 4:
					AI_WonderAttack();
					break;
			}
		}
		private void AI_Attack()
		{
			shot();
			if (Timer % 60 == 0)
			{
				if (DistanceToTarget() > requireDistance)
				{
					FakeVelocity = (Vector)(TargetPlayer.Center - Center);
					FakeVelocity.Length = Rand.Next(10, 20);
					FakeVelocity.Angle += Rand.NextDouble(-Math.PI / 9, Math.PI / 9);
				}
				else
				{
					if (FakeVelocity.Length > 3)
					{
						FakeVelocity /= 3;
					}
					else if (FakeVelocity.Length < 1)
					{
						FakeVelocity.Length = Rand.NextFloat(1, 3);
					}
					FakeVelocity.Angle = Rand.NextAngle();
				}
			}
		}
		private void AI_Guard()
		{
			bool finded = false;
			float dist = float.MaxValue;
			foreach (var player in Starver.Players)
			{
				if (player == null || !player.Active)
				{
					continue;
				}
				float now = Vector2.Distance(targetPos, player.Center);
				if (now < requireDistance && now < dist)
				{
					Target = player;
					finded = true;
				}
			}
			if (finded)
			{
				shot();
			}
			Center = myVector + new Vector2(0, 16 * 5 * (float)Math.Sin(Timer * PI * 2 / 120));
		}
		private void AI_Wonder()
		{
			bool finded = false;
			float dist = float.MaxValue;
			foreach (var player in Starver.Players)
			{
				if (player == null || !player.Active)
				{
					continue;
				}
				float now = Vector2.Distance(targetPos, player.Center);
				if (now < requireDistance && now < dist)
				{
					Target = player;
					finded = true;
				}
			}
			if (finded)
			{
				shot();
			}
			Vector2 v2 = myVector;
			Center = targetPos + v2.ToLenOf(myVector.Length() * (float)Math.Sin(Timer * PI * 2 / 120));
		}
		private void AI_Escape()
		{
			if (Vector2.Distance(targetPos, Center) > 16 * 10)
			{
				FakeVelocity = (Vector)(targetPos - Center);
				FakeVelocity.Length = 19;
			}
			else
			{
				KillMe();
			}
		}
		private void AI_WonderAttack()
		{
			if (Timer++ % t == 0)
			{
				FakeVelocity *= -1;
			}
			if (Vector2.Distance(Center, TargetPlayer.Center) < requireDistance)
			{
				shot();
				if (TrackingPlayerIfCloseTo)
				{
					Attack(TargetPlayer);
				}
			}
		}
		#endregion
		#region Shots
		/// <summary>
		/// 类霰弹枪
		/// </summary>
		private void Shot_0()
		{
			if (Timer % 45 == 0 && Timer % (2 * 60 + 3 * 45) <= 3 * 45)
			{
				Vel = (Vector)(Center - TargetPlayer.Center);
				ProjSector(Center, 18, 16, Vel.Angle, PI * 4 / 9, 53, ProjectileID.BulletDeadeye, 5);
			}
		}
		/// <summary>
		/// shot2的加强版
		/// </summary>
		private void Shot_1()
		{
			if (Timer % (60 * 4 + 60) < 60 && Timer % 2 == 0)
			{
				Vel = (Vector)(TargetPlayer.Center - Center);
				Vel.Length = 27;
				Proj(Center, Vel, ProjectileID.BulletDeadeye, 28, 13);
			}
		}
		/// <summary>
		/// 类步枪
		/// </summary>
		private void Shot_2()
		{
			if (Timer % (60 * 4 + 60) < 60 && Timer % 3 == 0)
			{
				Vel = (Vector)(TargetPlayer.Center - Center);
				Vel.Length = 230;
				Proj(Center, Vel, ProjectileID.BulletDeadeye, 16);
			}
		}
		/// <summary>
		/// 类狙击
		/// </summary>
		private void Shot_3()
		{
			if (Timer % (60 * 5) == 0)
			{
				Vel = (Vector)(TargetPlayer.Center - Center);
				Vel.Length = 50;
				Proj(Center, Vel, ProjectileID.BulletDeadeye, 74, 20);
			}
		}
		/// <summary>
		/// 类步枪扫射
		/// </summary>
		private void Shot_4()
		{
			if (Timer % (60 * 6) < 60 * 3)
			{
				if (Timer % (60 * 3) < (60 * 3 / 2) && Timer % 3 == 0)
				{
					Vel = (Vector)(TargetPlayer.Center - Center);
					Vel.Length = 20;
					Vel.Angle -= PI / 5 / 2;
					Vel.Angle += PI / 5 * (Timer % (60 * 3) / 3) / (60);
					Proj(Center, Vel, ProjectileID.BulletDeadeye, 15);
				}
			}
			else
			{
				if (Timer % (60 * 3) < (60 * 3 / 2) && Timer % 3 == 0)
				{
					Vel = (Vector)(TargetPlayer.Center - Center);
					Vel.Length = 20;
					Vel.Angle += PI / 5 / 2;
					Vel.Angle -= PI / 5 * (Timer % (60 * 3) / 3) / (60);
					Proj(Center, Vel, ProjectileID.BulletDeadeye, 15);
				}
			}
		}
		/// <summary>
		/// 火箭扫射
		/// </summary>
		private void Shot_5()
		{
			if (Timer % (60 * 6) < 60 * 3)
			{
				if (Timer % (60 * 3) < (60 * 3 / 2) && Timer % 20 == 0)
				{
					Vel = (Vector)(TargetPlayer.Center - Center);
					Vel.Length = 22;
					Vel.Angle -= PI / 5 / 2;
					Vel.Angle += PI * 2 / 5 * (Timer % (60 * 3) / 20) / (9);
					Proj(Center, Vel, ProjectileID.RocketSkeleton, 52);
				}
			}
			else
			{
				if (Timer % (60 * 3) < (60 * 3 / 2) && Timer % 20 == 0)
				{
					Vel = (Vector)(TargetPlayer.Center - Center);
					Vel.Length = 22;
					Vel.Angle += PI / 5 / 2;
					Vel.Angle -= PI * 2 / 5 * (Timer % (60 * 3) / 20) / (9);
					Proj(Center, Vel, ProjectileID.RocketSkeleton, 52);
				}
			}
		}
		/// <summary>
		/// 四向发射追踪导弹
		/// </summary>
		private void Shot_6()
		{
			if (Timer % 130 == 0)
			{
				if (DistanceToTarget() < 16 * 12)
				{
					FakeVelocity = (Vector)(Center - TargetPlayer.Center);
					FakeVelocity.Length = 19;
				}
				else
				{
					LaunchProjs(24, Math.PI * 2 * (Timer % 1300) / 1300, ProjectileID.SaucerMissile, 4, 42);
				}
			}
		}
		/// <summary>
		/// 吐火
		/// </summary>
		private void Shot_7()
		{
			if (Timer % 25 == 0)
			{
				Vel = (Vector)(TargetPlayer.Center - Center);
				Vel.Length = 23.66f;
				Vel.Angle += Rand.NextDouble(-Math.PI / 6, Math.PI / 6);
				Proj(Center, Vel, ProjectileID.CursedFlameHostile, 12);
			}
		}
		#endregion
		#region Methods
		public void Guard(Vector2 where, Vector2? mypos = null)
		{
			work = 0;
			targetPos = where;
			myVector = mypos ?? targetPos + Rand.NextVector2(16 * 40, 16 * 40);
			requireDistance = 16 * 40;
		}
		public void Attack(StarverPlayer target)
		{
			work = 1;
			Target = target;
			requireDistance = 16 * 40;
		}
		public void Escape(Vector2 where)
		{
			work = 3;
			targetPos = where;
		}
		public void Wonder(Vector2 where, Vector2? wondering = null)
		{
			work = 2;
			targetPos = where;
			myVector = wondering ?? new Vector2(16 * 5, 0);
		}
		public void Wonder(Vector2? wondering, float attackRadium)
		{
			Wonder(Center, wondering);
			requireDistance = attackRadium;
		}
		public void WonderAttack(Vector2 where, Vector2 velocity, int cycle, bool tracking)
		{
			work = 4;
			t = cycle;
			myVector = where;
			FakeVelocity = (Vector)velocity;
			Center = where;
			Timer = (uint)cycle / 2;
			TrackingPlayerIfCloseTo = tracking;
		}
		public void SetShot(int id)
		{
			Debug.Assert(0 <= id && id < MaxShots, $"id out of range: {id}");
			shot = id switch
			{
				0 => Shot_0,
				1 => Shot_1,
				2 => Shot_2,
				3 => Shot_3,
				4 => Shot_4,
				5 => Shot_5,
				6 => Shot_6,
				7 => Shot_7,
				_ => throw new InvalidOperationException()
			};
		}
		#endregion
	}
}
