using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace Starvers.NPCSystem.NPCs
{
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public class FloatingSkeleton : StarverNPC
	{
		#region Fields
		private static int[] SkeletonTypes =
		{
			NPCID.Skeleton,
			NPCID.SkeletonAlien,
			NPCID.SkeletonAstonaut,
			//NPCID.PantlessSkeleton,
			//NPCID.BigHeadacheSkeleton,
			//NPCID.BigMisassembledSkeleton,
			//NPCID.SmallSkeleton,
			//NPCID.SmallPantlessSkeleton,
			//NPCID.SmallHeadacheSkeleton,
			//NPCID.SmallHeadacheSkeleton
		};
		private static DropItem[] RealDrops =
		{
			new DropItem(new int[] { ItemID.Bone }, 1, 10, 0.5f, false)
		};
		private static SpawnChecker Checker_UnderGround;
		#endregion
		#region Properties
		protected override float CollidingIndex => DamageIndex;
		#endregion
		#region Ctor
		static FloatingSkeleton()
		{
			Checker_UnderGround = SpawnChecker.ZombieLike;
			Checker_UnderGround.SpawnChance /= 2;
			Checker_UnderGround.Biome = BiomeType.UnderGround;
			Checker_UnderGround.Condition = SpawnConditions.None;
		}
		public FloatingSkeleton()
		{
			dontNeedTarget = true;
			AfraidSun = true;
			OverrideRawDrop = true;
			RawType = NPCID.Skeleton;
			Types = SkeletonTypes;
			Drops = RealDrops;
			AIStyle = 2;
			DefaultLife = 3000;
			NoGravity = true;
			DefaultDefense = 800;
			NoTileCollide = true;
			Checker = SpawnChecker.ZombieLike;
			Checker.SpawnChance /= 2;
			Checker.SpawnRate *= 3;
			Checker.SpawnRate /= 2;
			DamagedIndex = 0.1f;
			CollideDamage = 350;
			SpaceOption = SpawnSpaceOptins.None;
		}
		#endregion
		#region RealAI
		protected override void RealAI()
		{
			if (Timer % (60 * 7) < 60 * 7 / 2) // 3.5 * 2s一个周期
			{
				if (CheckSecond(1.25) || CheckSecond(1.95) || CheckSecond(2.65))
				{
					try
					{
						Vel = (Vector)(TargetPlayer.Center - Center);
						Vel.Length = 19;
					}
					catch (NullReferenceException)
					{
						if (AlivePlayers() == 0)
						{
							KillMe();
							return;
						}
						TargetClosest();
						Vel = (Vector)Rand.NextVector2(19);
					}
					catch (IndexOutOfRangeException)
					{
						//TShockAPI.TShock.Log.Write(e.ToString(), System.Diagnostics.TraceLevel.Error);
						Vel = (Vector)Rand.NextVector2(19);
					}
					Proj(Center, Vel, ProjectileID.SkeletonBone, 100, 20);
				}
			}
		}
		#endregion
		#region CheckSpawn
		protected override bool CheckSpawn(StarverPlayer player)
		{
			if(BossSystem.Bosses.Base.StarverBoss.EndTrial)
			{
				return false;
			}
			SpawnChecker value = player.GetSpawnChecker();
			return StarverConfig.Config.TaskNow >= Checker.Task && (Checker.Match(value) || Checker_UnderGround.Match(value)) && SpawnTimer % Checker.SpawnRate == 0 && Rand.NextDouble() < Checker.SpawnChance && Rand.Next(StarverConfig.Config.TaskNow) > 17;
		}
		#endregion
	}
}
