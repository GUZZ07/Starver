using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace Starvers.Enemies.Npcs
{
	using Dropping = Terraria.GameContent.ItemDropRules.CommonCode;
	public class FloatingSkeleton : StarverNPC
	{
		private static SpawnChecker sChecker = SpawnChecker.ZombieLike;
		private int timer;
		public FloatingSkeleton()
		{

		}

		public override void AI()
		{
			timer++;
			if (TNPC.HasPlayerTarget)
			{
				switch (timer % 180)
				{
					case 139:
					case 159:
					case 179:
						var target = Starver.Instance.Players[TNPC.target];
						var velocity = target.Center - Center;
						velocity.Length(rand.NextFloat(8.5f, 10f));
						var damage = rand.Next(20, 40) * DamageIndex;
						NewProj(Velocity, ProjectileID.SkeletonBone, (int)damage);
						break;
				}
			}
		}

		public override bool CheckSpawn(StarverPlayer player)
		{
			return sChecker.Match(player.GetNPCSpawnChecker());
		}

		public override void DropItems()
		{
			if (rand.NextDouble() > 0.5)
			{
				Dropping.DropItemFromNPC(TNPC, ItemID.Bone, rand.Next(3, 8));
			}
		}

		public override void Initialize()
		{
			TNPC.type = NPCID.Skeleton;
			TNPC.life = 4000;
			TNPC.lifeMax = 4000;
			TNPC.defense = 800;
			TNPC.aiStyle = 2;
			TNPC.damage = 88;
		}
	}
}
