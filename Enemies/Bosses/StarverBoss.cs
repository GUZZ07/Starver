using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using TShockAPI;

namespace Starvers.Enemies.Bosses
{
	public abstract class StarverBoss : StarverEnemy
	{
		public const int DefaultLevel = 2000;
		public string Name 
		{ 
			get; 
			protected set;
		}
		/// <summary>
		/// 伤害跟随Level的增长因子(从1开始)
		/// </summary>
		public virtual double DamageIndex
		{
			get => 1;
		}
		public int Level
		{
			get;
			protected set;
		}

		public override bool Active
		{
			get => 0 <= Index && Index < Main.maxNPCs && base.Active;
		}
		public Random Rand { get; }


		public int Target
		{
			get => TNPC.target;
			set => TNPC.target = value;
		}
		public StarverPlayer TargetPlayer
		{
			get => TNPC.HasPlayerTarget ? Starver.Instance.Players[Target] : null;
		}

		#region DefDatas
		protected int defLifes = 1;
		protected int defLife = 20000;
		protected int defDefense = 5000;
		protected int rawType;
		#endregion
		#region InstanceDatas
		public int Lifes
		{
			get;
			set;
		}
		public int LifesMax
		{
			get;
			set;
		}
		#endregion

		protected StarverBoss(int rawNpcType) : base(-1)
		{
			rawType = rawNpcType;
			Rand = new Random();
		}

		public virtual void Spawn(Vector2 position, int level = DefaultLevel)
		{
			int x = (int)position.X;
			int y = (int)position.Y;
			Index = NPC.NewNPC(x, y, rawType);
			Level = level;
			Lifes = defLifes;
			LifesMax = defLifes;
			SetNPCDatas();
			if (Name != null)
			{
				TSPlayer.All.SendData(PacketTypes.UpdateNPCName, Name, Index);
			}
		}
		protected virtual void ReSpawn()
		{
			int x = (int)Center.X;
			int y = (int)Center.Y;
			Index = NPC.NewNPC(x, y, rawType);
			SetNPCDatas();
			if (Name != null)
			{
				TSPlayer.All.SendData(PacketTypes.UpdateNPCName, Name, Index);
			}
		}
		#region SetNPCDatas
		protected void SetNPCDatas()
		{
			TNPC.boss = true;
			TNPC.lifeMax = defLife;
			TNPC.life = defLife;
			TNPC.defense = defDefense;
		}
		#endregion
		#region AI
		public virtual void AI()
		{
			if (!Active)
			{
				if (Lifes != 0)
				{
					ReSpawn();
				}
				else
				{
					return;
				}
			}
			RealAI();
		}
		protected abstract void RealAI();
		#endregion
		#region Defeated
		protected virtual void Defeated()
		{
			Kill();
		}
		#endregion
		#region Lifes--
		protected virtual void LifesDown()
		{
			Lifes--;
			TSPlayer.All.SendMessage($"{Name ?? TNPC.GivenOrTypeName} 还剩[c/ff0000:{Lifes}]/[c/ffff00:{LifesMax}]条命", Color.Blue);
			if (Lifes <= 0)
			{
				Kill();
			}
			else
			{
				Life = LifeMax;
			}
		}
		#endregion
		#region ReceiveDamage
		public override void ReceiveDamage(int damage)
		{
			Life -= damage;
			if (Life <= 0)
			{
				LifesDown();
			}
		}
		#endregion
		#region Kill
		public override void Kill()
		{
			DropItems();
			RewardExps();
			Active = false;
		}
		#endregion

		protected virtual void RewardExps()
		{

		}

		protected virtual void DropItems()
		{

		}

		public override string ToString()
		{
			return Name ?? GetType().Name;
		}
		#region States
		#region Machines
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
			public override void Update()
			{
				Timer++;
				if (created < TridentCount)
				{
					if (Timer % CreateInterval == 0)
					{
						var source = GetLaunchSource();
						var velocity = -source / TridentHitPlayerTime;
						var index = Boss.NewProj(Boss.TargetPlayer.Center + source, Vector2.Zero, ProjectileID.UnholyTridentHostile, Damage);
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
				return new Vector2(dir * 16 * 25, Y);
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
			protected BossStateMachine(BossState state, StarverBoss boss)
			{
				State = state;
				Boss = boss;
			}
			public virtual void Begin() { }
			public abstract void Update();
			public virtual void Abort() { }
		}

		protected enum BossState
		{
			EyeRush,
			EyeSummon,
			EvilTrident
		}
		#endregion
		#endregion
	}
}
