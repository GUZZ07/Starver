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
		private static SpawnChecker sChecker = SpawnChecker.Night;
		private int timer;
		public FloatingSkeleton() : base(new Vector(30, 45))
		{
			noTileCollide = true;
			noGravity = true;
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
						velocity.Length(rand.NextFloat(12.5f, 15f));
						var damage = rand.Next(20, 40) * DamageIndex;
						NewProj(Velocity, ProjectileID.SkeletonBone, (int)damage);
						break;
				}
			}
			if (timer % 3 == 0)
			{
				TNPC.SendData();
			}
		}

		public override bool CheckSpawn(StarverPlayer player)
		{
			return false;
			timer++;
			bool success = timer % sChecker.SpawnRate == 0 && rand.NextDouble() < sChecker.SpawnChance;
			return success && sChecker.Match(player.GetNPCSpawnChecker());
		}

		public override void DropItems()
		{
			if (rand.NextDouble() < 0.5)
			{
				Dropping.DropItemFromNPC(TNPC, ItemID.Gel, rand.Next(3, 8));
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
