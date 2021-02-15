using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace Starvers.Enemies.Npcs
{
	using CursedSkeletron = Bosses.CursedSkeletron;
	public class SkeletronExClone : StarverNPC
	{
		private int timeLeft;
		private int shotTimer;
		private double phi;
		private double omega;
		private int bulletNum;                  // 需要区分奇数和偶数弹
		private int shotInterval;
		private int shotDelay;
		private int damage;
		private CursedSkeletron owner;

		public SkeletronExClone() : base(Vector.Zero)
		{
			noTileCollide = true;
			noGravity = true;
		}

		public override void AI()
		{
			if (!owner.Active || timeLeft <= 0)
			{
				TurnToAir();
				return;
			}
			if (shotTimer >= shotDelay && (shotTimer - shotDelay) % shotInterval == 0)
			{
				var angle = (Center - owner.TargetPlayer.Center).Angle();
				ProjSector(Center, 16, 3, angle, Math.PI / 3, damage, ProjectileID.CursedSapling, bulletNum);
			}
			Center = owner.Center + Vector.FromPolar(phi, 16 * 5);
			phi += omega;
			UpdateToClient();
			shotTimer++;
			timeLeft--;
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
			TNPC.type = NPCID.SkeletronHead;
			TNPC.life = 40;
			TNPC.lifeMax = 40;
			TNPC.aiStyle = -1;
			TNPC.damage = 0;
			TNPC.dontTakeDamage = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="boss">本体</param>
		/// <param name="φ">绕本体旋转的初相</param>
		/// <param name="ω">绕本体旋转的角速度</param>
		/// <param name="bulletnum">单次发射出的弹幕数(区分奇偶)</param>
		/// <param name="interval">发射间隔</param>
		/// <param name="shotdelay">初次发射的延时</param>
		/// <param name="timeleft">存活时长</param>
		public void SetParas(CursedSkeletron boss, double φ, double ω, int bulletnum, int interval,int shotdelay, int dmg, int timeleft)
		{
			phi = φ;
			omega = ω;
			owner = boss;
			bulletNum = bulletnum;
			shotInterval = interval;
			damage = dmg;
			shotDelay = shotdelay;
			timeLeft = timeleft;
		}
	}
}
