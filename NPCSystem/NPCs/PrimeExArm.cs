using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace Starvers.NPCSystem.NPCs
{
	using BossSystem.Bosses;
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public class PrimeExArm : StarverNPC
	{
		#region Fields
		private PrimeEx Prime;
		private const float Distance = 16 * 25;
		#endregion
		#region Ctor
		public PrimeExArm() : base(1)
		{
			NoTileCollide = true;
			RawType = NPCID.PrimeLaser;
			DefaultDefense = 2000;
			DefaultLife = 3000;
		}
		#endregion
		#region Properties
		public override float DamageIndex => Prime.DamageIndex;
		private BossMode Mode => Prime.ModeNow;
		#endregion
		#region Spawn
		public void Spawn(Vector where,PrimeEx prime,float ID)
		{
			base.Spawn(where);
			Starver.NPCs[Index] = NPCs[Index] = this;
			AIUsing[0] = PI * 2 * ID / PrimeEx.MaxArms;
			Prime = prime;
			Vel = FromPolar(PI * 2 * (ID / PrimeEx.MaxArms),Distance);
			Center = Prime.Center + Vel;
			RealNPC.aiStyle = None;
			RealNPC.ai[1] = Prime.Index;
			SendData();
		}
		#endregion
		#region RealAI
		protected override void RealAI()
		{
			if (!Prime.Active)
			{
				StarverPlayer.All.SendDeBugMessage($"{Name} Killed itself by no Prime");
				KillMe();
				return;
			}
			Target = Prime.Target;
			AIUsing[0] += PI * 2 / PrimeEx.MaxArms / 20;
			Vel = FromPolar(AIUsing[0], Distance);
			Center = Prime.Center + Vel;
			switch(Mode)
			{
				#region Laser
				case BossMode.PrimeExLaser:
					if(Timer % 70 == 0)
					{
						Laser();
					}
					break;
				#endregion
				#region Rocket
				case BossMode.PrimeExRocket:
					if (Timer % 45 == 0)
					{
						Rocket();
					}
					break;
				#endregion
				#region Missile
				case BossMode.PrimeExMissile:
					if (Timer % 40 == 0)
					{
						Missile();
					}
					break;
					#endregion
			}
			FakeVelocity = Vector.Zero;
		}
		#endregion
		#region AIs
		#region Missile
		private void Missile()
		{
			Vel = (Vector)(Center - Prime.Center);
			Vel.Length = 18;
			Proj(Center, Vel, ProjectileID.SaucerMissile, 226);
		}
		#endregion
		#region Rocket
		private void Rocket()
		{
			Vel = (Vector)(TargetPlayer.Center - Prime.Center);
			FakeVelocity = (Vector)(Center - Prime.Center);
			if(Vector.Dot(Vel, FakeVelocity) > 0)
			{
				Vel = (Vector)(TargetPlayer.Center - Center);
				Vel.Length = 19;
				Proj(Center, Vel, ProjectileID.RocketSkeleton, 195);
			}
			FakeVelocity = Vector.Zero;
		}
		#endregion
		#region Laser
		private void Laser()
		{
			Vel = (Vector)(TargetPlayer.Center - Prime.Center);
			Vel.Length = 17;
			Proj(Center, Vel, ProjectileID.SaucerLaser, 193);
		}
		#endregion
		#endregion
	}
}
