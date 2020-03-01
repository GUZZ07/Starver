using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.BossSystem.Bosses
{
	using Base;
	using Microsoft.Xna.Framework;
	using Starvers.WeaponSystem;
	using Terraria.ID;
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public class RetinazerEx : StarverBoss
	{
		#region Fields
		/// <summary>
		/// 0:正上
		/// 1:正右
		/// 2:正下
		/// 3:正左
		/// </summary>
		protected int Where = 0;
		protected const int LaserBlueTimeMax = 60 * 4;
		protected int defToCut;
		#endregion
		#region Ctor
		public RetinazerEx() : base(2)
		{
			TaskNeed = 36;
			RawType = NPCID.Retinazer;
			DefaultLife = 67000;
			DefaultLifes = 100;
			DefaultDefense = 300;
			Drops = new DropItem[]
			{
				new DropItem(new int[]{ Currency.Ranged }, 1, 28, 0.5f),
				new DropItem(new int[]{ Currency.Ranged }, 1, 28, 0.5f),
				new DropItem(new int[]{ Currency.Ranged }, 1, 28, 0.5f),
				new DropItem(new int[]{ Currency.Ranged }, 1, 28, 0.5f),
				new DropItem(new int[]{ Currency.Ranged }, 1, 28, 0.5f),
			};
		}
		#endregion
		#region Spawn
		public override void Spawn(Vector2 where, int lvl = 2000)
		{
			base.Spawn(where, lvl);
			defToCut = Defense / LifesMax;
			lastMode = BossMode.RetinazerSaucerLaser;
		}
		#endregion
		#region LifeDown
		public override void LifeDown()
		{
			Defense -= defToCut;
			base.LifeDown();
		}
		#endregion
		#region RealAI
		public override void RealAI()
		{
			#region Mode
			switch (Mode)
			{
				#region SelectMode
				case BossMode.WaitForMode:
					SelectMode();
					break;
				#endregion
				#region DeathLaser
				case BossMode.RetinazerDeathLaser:
					if (modetime > 60 * 8 || Lifes < LifesMax / 2)
					{
						ResetMode();
						break;
					}
					if (Timer % 16 == 0)
					{
						DeathLaser();
					}
					break;
				#endregion
				#region LaserBlue
				case BossMode.RetinazerLaserBlue:
					if (modetime > LaserBlueTimeMax)
					{
						ResetMode();
						break;
					}
					if (Timer % 5 == 0)
					{
						LaserBlue();
					}
					break;
				#endregion
				#region WalkerLaser
				case BossMode.RetinazerLaserWalker:
					if (modetime > 60 * 6)
					{
						ResetMode();
						break;
					}
					if (Timer % 25 == 0)
					{
						WalkerLaser();
					}
					break;
				#endregion
				#region SaucerLaser
				case BossMode.RetinazerSaucerLaser:
					if (modetime > 60 * 9)
					{
						ResetMode();
						break;
					}
					if (Timer % 2 == 0)
					{
						SaucerLaser();
					}
					break;
					#endregion
			}
			#endregion
			#region Common
			if(Timer % 3 == 0)
			{
				GoToWhere();
			}
			#endregion
		}
		#endregion
		#region AIs
		#region SaucerLaser
		private void SaucerLaser()
		{
			Vel = (Vector)(-RelativePos);
			Vel.Length = 20;
			Vel.Angle += Math.Sin(Timer * Math.PI / 90) * Math.PI / 6;
			Proj(Center, Vel, ProjectileID.SaucerLaser, 229);
		}
		#endregion
		#region WalkerLaser
		private void WalkerLaser()
		{
			Vel = (Vector)(-RelativePos);
			ProjSector(TargetPlayer.Center, 16.66666666f, Vel.Length, Vel.Angle, PI * 2 / 5, 206, ProjectileID.MartianWalkerLaser, 5, 1);
		}
		#endregion
		#region LaserBlue
		private void LaserBlue()
		{
			vector = (Vector)(-RelativePos);
			Vel = vector.Vertical();
			vector.Angle += Math.Cos(modetime / (double)LaserBlueTimeMax * Math.PI) * Math.PI / 3;
			Vel.Length = 4;
			Proj(Center + 2 * Vel, vector, ProjectileID.EyeLaser, 204 * 1 / 3);
			Proj(Center + 1 * Vel, vector, ProjectileID.EyeLaser, 204 * 2 / 3);
			Proj(Center, vector, ProjectileID.EyeLaser, 204);
			Proj(Center - 1 * Vel, vector, ProjectileID.EyeLaser, 204 * 2 / 3);
			Proj(Center - 2 * Vel, vector, ProjectileID.EyeLaser, 204 * 1 / 3);
		}
		#endregion
		#region DeathLaser
		private void DeathLaser()
		{
			Vel = (Vector)(-RelativePos);
			Vel.Length = 20;
			vector = Vel.Vertical();
			vector.Length = Rand.Next(-5, 5);
			Vel += vector;
			Proj(Center, Vel, ProjectileID.DeathLaser, 219);
		}
		#endregion
		#region GoToWhere
		protected void GoToWhere()
		{
			WhereToGo = (Vector)TargetPlayer.Center;
			switch (Where)
			{
				case 0:
					WhereToGo += new Vector(0, -400);
					break;
				case 1:
					WhereToGo += new Vector(400, 0);
					break;
				case 2:
					WhereToGo += new Vector(0, 400);
					break;
				case 3:
					WhereToGo += new Vector(-400, 0);
					break;
			}
			FakeVelocity = (Vector)(WhereToGo - Center);
			FakeVelocity /= 30;
		}
		#endregion
		#region SelectMode
		private void SelectMode()
		{
			Where++;
			Where %= 4;
			modetime = 0;
			switch(lastMode)
			{
				#region DeathLaser
				case BossMode.RetinazerDeathLaser:
					Mode = BossMode.RetinazerLaserBlue;
					break;
				#endregion
				#region LaserBlue
				case BossMode.RetinazerLaserBlue:
					Mode = BossMode.RetinazerLaserWalker;
					break;
				#endregion
				#region WalkerLaser
				case BossMode.RetinazerLaserWalker:
					Mode = BossMode.RetinazerSaucerLaser;
					break;
				#endregion
				#region SaucerLaser
				case BossMode.RetinazerSaucerLaser:
					Mode = BossMode.RetinazerDeathLaser;
					break;
					#endregion
			}
		}
		#endregion
		#endregion

	}
}
