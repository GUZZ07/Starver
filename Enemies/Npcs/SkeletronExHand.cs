using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace Starvers.Enemies.Npcs
{
	using CursedSkeletron = Bosses.CursedSkeletron;
	public class SkeletronExHand : StarverNPC
	{
		private CursedSkeletron owner;
		private int timeLeft;
		private int shotDelay;
		private int shotTimer;
		private int shotInterval;
		private int damage;

		public SkeletronExHand() : base(new Vector(16 * 4,16 * 4))
		{
			noTileCollide = true;
			noGravity = true;
		}

		public override void AI()
		{
			if (!owner.Active)
			{
				TurnToAir();
				return;
			}
			if (timeLeft == 0)
			{
				ProjCircle(Center, 16 * 4, 18, ProjectileID.SkeletonBone, 15, damage);
				ProjCircleEx(Center, rand.NextAngle(), 16 * 2, 12, ProjectileID.BloodShot, 6, damage * 3 / 2);
				TurnToAir();
				return;
			}

			if (shotTimer < shotDelay)
			{

			}
			else if (shotTimer == shotDelay)
			{
				Velocity.Length(10);
			}
			else
			{
				if ((shotTimer - shotDelay) % shotInterval == 0)
				{
					var vel = Velocity;
					var back = Center - vel.ToLenOf(16 * 25);
					var vertical = vel.Vertical();

					vertical.Normalize();

					vel.Length(-rand.NextFloat(13, 16));
					vel += vertical * rand.NextFloat(-3, 3);

					var projID = rand.Next(0, 6) switch
					{
						0 or 1 or 2 => ProjectileID.SkeletonBone,
						3 or 4 => ProjectileID.BloodShot,
						_ => ProjectileID.WaterBolt
					};
					NewProj(back + vertical * rand.NextFloat(-24, 24), vel, projID, damage);
				}
			}
			UpdateToClient();
			timeLeft--;
			shotTimer++;
		}

		public override bool CheckSpawn(StarverPlayer player)
		{
			return false;
		}

		public override void DropItems()
		{

		}

		public override void Initialize()
		{
			TNPC.type = NPCID.SkeletronHand;
			TNPC.life = 40;
			TNPC.lifeMax = 40;
			TNPC.aiStyle = -1;
			TNPC.damage = 200;
			TNPC.dontTakeDamage = true;
		}

		public void SetParas(CursedSkeletron boss, int timeleft, int shotdelay, int interval, int dmg, Vector pos, Vector vel)
		{
			TNPC.Center = pos;
			TNPC.velocity = vel;
			TNPC.ai[1] = boss.Index;
			owner = boss;
			timeLeft = timeleft;
			shotDelay = shotdelay;
			shotInterval = interval;
			damage = dmg;
		}
	}
}
