using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace Starvers.NPCSystem.NPCs
{
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public class PossessedArmorArcher : StarverNPC
	{
		#region Properties
		protected override float CollidingIndex => DamageIndex;
		#endregion
		#region Ctor
		public PossessedArmorArcher()
		{
			AfraidSun = true;
			RawType = NPCID.PossessedArmor;
			DefaultLife = 5000;
			DefaultDefense = 1200;
			dontNeedTarget = true;
			Checker = SpawnChecker.ZombieLike;
			Checker.SpawnChance /= 3;
			Checker.Task = 25;
			CollideDamage = 400;
			DamagedIndex = 0.1f;
			AIStyle = 3;
		}
		#endregion
		#region RealAI
		protected override void RealAI()
		{
			if (Timer % 60 * 7 < 60 * 3)
			{
				Velocity.X /= 2;
				if (Velocity.Y < 0)
				{
					Velocity.Y = 0;
				}
				if (CheckSecond(1))
				{
					try
					{
						Vel = (Vector)(TargetPlayer.Center - Center);
						Vel.Length = 16;
					}
					catch
					{
						Vel = FromPolar(Rand.NextAngle(), 16);
					}
					Proj(Center, Vel, ProjectileID.WoodenArrowHostile, 179, 20);
				}
			}
		}
		#endregion
		#region CheckSpawn
		protected override bool CheckSpawn(StarverPlayer player)
		{
			return base.CheckSpawn(player) && Rand.Next(StarverConfig.Config.TaskNow) > 20;
		}
		#endregion
	}
}
