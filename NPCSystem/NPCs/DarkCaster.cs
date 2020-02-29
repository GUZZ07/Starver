using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Starvers.TaskSystem;

namespace Starvers.NPCSystem.NPCs
{
	using Terraria;
	using TOFOUT.Terraria.Server;
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public class DarkCaster : StarverNPC
	{
		#region Fields
		protected delegate void MyAIDelegate(DarkCaster This);
		/// <summary>
		/// 长度为MaxInternalAIStyle
		/// </summary>
		protected static MyAIDelegate[] InternalAIs = new MyAIDelegate[MaxInternalAIStyle]
		{
			AI_0,
			AI_1,
			AI_2,
			AI_3,
			AI_4
		};
		/// <summary>
		/// <para>0 : 一次3幽灵同时发出</para>
		/// <para>1 : 直接在玩家身边召唤幽灵</para>
		/// <para>2 : 5连发</para>
		/// <para>3 : 先发射水球, 若水球还存活则"分裂"为火球</para>
		/// <para>5 : 先生成子弹并规划好路线, 随后发射</para>
		/// </summary>
		protected int InternalAIStyle;
		protected const int MaxInternalAIStyle = 5;
		protected static SpawnChecker DungeonChecker = SpawnChecker.DungeonLike;
		/// <summary>
		/// 保留弹幕位置用
		/// </summary>
		protected int ProjIndex;
		protected IProjSet ProjSet;
		#endregion
		#region Properties
		protected override float CollidingIndex => DamageIndex;
		#endregion
		#region Ctor
		public DarkCaster() : base()
		{
			AfraidSun = true;
			SpaceOption |= SpawnSpaceOptins.InScreen;
			RawType = NPCID.DarkCaster;
			DefaultLife = 15000;
			DefaultDefense = 3000;
			Checker = SpawnChecker.RareNight;
			Checker.Task = TaskID.SkeletronEx;
			CollideDamage = 400;
			DamagedIndex = 0.1f;
			AIStyle = None;
			NoGravity = true;
		}
		#endregion
		#region Spawn
		public override void OnSpawn()
		{
			base.OnSpawn();
			InternalAIStyle = Rand.Next(MaxInternalAIStyle);
			if(InternalAIStyle == 4)
			{
				ProjSet = new ProjStack();
			}
		}
		#endregion
		#region RealAI
		protected override void RealAI()
		{
			InternalAIs[InternalAIStyle](this);
		}
		#endregion
		#region AIs
		protected static void AI_0(DarkCaster This)
		{
			if(This.Timer % (60 * 3 / 2) == 0)
			{
				This.Vel = (Vector)(This.TargetPlayer.Center - This.Center);
				This.ProjSector(This.Center, 8, 3, This.Vel.Angle, PI / 3, 200, ProjectileID.LostSoulHostile, 3);
			}
		}
		protected static void AI_1(DarkCaster This)
		{
			uint Mod = This.Timer % (60 * 4);
			if (Mod == 0)
			{
				This.ProjCircle(This.TargetPlayer.Center, 16 * 30, 5, ProjectileID.LostSoulHostile, 8, 200);
			}
		}
		protected static void AI_2(DarkCaster This)
		{
			uint Mod = This.Timer % (60 * 4);
			switch(Mod)
			{
				case 0:
				case 20:
				case 40:
				case 60:
				case 80:
					This.Vel = (Vector)(This.TargetPlayer.Center - This.Center);
					This.Vel.Length = 9;
					This.Proj(This.Center, This.Vel, ProjectileID.LostSoulHostile, 200);
					break;
				case 60 * 4 - 1:
					This.Warp();
					break;
			}
		}
		protected static void AI_3(DarkCaster This)
		{
			uint Mod = This.Timer % (60 * 5 / 2);
			switch (Mod)
			{
				case 20:
					This.Vel = (Vector)(This.TargetPlayer.Center - This.Center);
					This.Vel.Length = 9;
					This.ProjIndex = This.Proj(This.Center, This.Vel, ProjectileID.FrostBlastHostile, 180);
					break;
				case 80:
					if (Projs[This.ProjIndex].active && Projs[This.ProjIndex].type == ProjectileID.FrostBlastHostile)
					{
						This.ProjCircle(Projs[This.ProjIndex].Center, 1, This.Vel.Length / 2, ProjectileID.InfernoHostileBolt, 6, 180 / 2);
						Projs[This.ProjIndex].active = false;
						NetMessage.SendData((int)PacketTypes.ProjectileNew, -1, -1, null, This.ProjIndex);
					}
					break;
			}
		}
		protected static void AI_4(DarkCaster This)
		{
			uint Mod = This.Timer % (60 * 5);
			if (Mod < 60 * 5 / 2)
			{
				if (Mod % 5 == 0)
				{
					This.Vel = (Vector)Rand.NextVector2(16 * 35, 16 * 35);
					This.ProjIndex = This.Proj(This.Center + This.Vel, Vector.Zero, ProjectileID.FrostBlastHostile, 123);
					if (This.ProjIndex >= 200 || This.ProjIndex < 0)
					{
						return;
					}
					This.Vel = (Vector)(This.TargetPlayer.Center - Projs[This.ProjIndex].Center);
					This.Vel.Length = 6;
					This.ProjSet.Push(This.ProjIndex, This.Vel);
				}
			}
			else if (Mod != 60 * 5 - 1)
			{
				if (Mod % 10 == 0)
				{
					This.ProjSet.Launch(1);
				}
			}
			else
			{
				This.Vel = (Vector)(This.TargetPlayer.Center - This.Center);
				This.Vel.Length = 10;
				This.ProjSet.Launch(This.Vel);
			}
		}
		#endregion
		#region OnStrike
		public override void OnStrike(int RealDamage, float KnockBack, StarverPlayer player)
		{
			base.OnStrike(RealDamage, KnockBack, player);
			Warp();
			SendData();
		}
		#endregion
		#region Warp
		protected void Warp()
		{
			if (Target != -1 && Target < Starver.Players.Length)
			{
				Position = CalcSpawnPosInScreen((Vector)TargetPlayer.Center);
			}
		}
		#endregion
		#region Check
		protected override bool CheckSpawn(StarverPlayer player)
		{
			if (BossSystem.Bosses.Base.StarverBoss.EndTrial)
				return false;
			var PlayerChecker = player.GetSpawnChecker();
			return StarverConfig.Config.TaskNow >= Checker.Task && (Checker.Match(PlayerChecker) || DungeonChecker.Match(PlayerChecker)) && SpawnTimer % Checker.SpawnRate == 0 && Rand.NextDouble() < Checker.SpawnChance;
		}
		#endregion
	}
}
