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
	public class RedDevilEx : StarverBoss
	{
		#region Fields
		/// <summary>
		/// (0, -16 * 24)
		/// </summary>
		private Vector UnitY = new Vector(0, -16 * 24);
		#endregion
		#region Ctor
		public RedDevilEx():base(1)
		{
			TaskNeed = 32;
			Name = "红魔王";
			RawType = NPCID.RedDevil;
			DefaultDefense = 68;
			DefaultLife = 30000;
			DefaultLifes = 70;
			LifeperPlayerType = ByLifes;
			Drops = new DropItem[]
			{
				new DropItem(new int[] { Currency.Minion}, 2, 6,  0.4f),
				new DropItem(new int[] { Currency.Ranged}, 4, 9,  0.4f),
				new DropItem(new int[] { Currency.Melee}, 7, 10,  0.4f),
			};
		}
		#endregion
		#region Spawn
		public override void Spawn(Vector2 where, int lvl = 2000)
		{
			base.Spawn(where, lvl);
			lastMode = BossMode.SummonFollows;
		}
		#endregion
		#region RealAI
		public override void RealAI()
		{
			#region Modes
			switch (Mode)
			{
				#region SelectMode
				case BossMode.WaitForMode:
					SelectMode();
					break;
				#endregion
				#region Flame
				case BossMode.RedDevilFlame:
					if(modetime > 60 * 6)
					{
						ResetMode();
						break;
					}
					if(Timer % 5 == 0)
					{
						Flame();
					}
					break;
				#endregion
				#region Laser
				case BossMode.RedDevilLaser:
					if(modetime > 60 * 8)
					{
						ResetMode();
						break;
					}
					if(Timer % 3 == 0)
					{
						Laser();
					}
					break;
				#endregion
				#region Trident
				case BossMode.RedDevilTrident:
					if(modetime > 60 * 9)
					{
						ResetMode();
						break;
					}
					if(Timer % 5 == 0)
					{
						EvilTrident(155);
					}
					break;
				#endregion
				#region SummonFollows
				case BossMode.SummonFollows:
					if(modetime > 60 * 6)
					{
						ResetMode();
						break;
					}
					if(Timer % 58 == 0)
					{
						SummonFollows();
					}
					break;
					#endregion
			}
			#endregion
			#region Common
			FakeVelocity = (Vector)(TargetPlayer.Center + UnitY - Center) / 14;
			#endregion
		}
		#endregion
		#region AIs
		#region SummonFollows
		private new void SummonFollows()
		{
			vector = new Vector(0,17);
			NewNPC((Vector)Center, vector, NPCID.Demon, 30000, 500);
			vector.Angle += PI / 11;
			NewNPC((Vector)Center, vector, NPCID.Demon, 30000, 500);
			vector.Angle -= 2 * PI / 11;
			NewNPC((Vector)Center, vector, NPCID.Demon, 30000, 500);
		}
		#endregion
		#region Laser
		private void Laser()
		{
			vector = (0, 16 * 38);
			Vel = FromPolar(PI / 2, 24);
			Vel.Angle += (Rand.NextAngle() - PI) / 24;
			Proj(TargetPlayer.Center + vector, Vel.Deflect(PI), ProjectileID.EyeLaser, 109);
			Proj(TargetPlayer.Center - vector, Vel, ProjectileID.EyeLaser, 109);
		}
		#endregion
		#region Flame
		private void Flame()
		{
			vector = Vector.FromPolar(Math.PI / 4 + Math.PI / 2 * Rand.Next(4), 16 * 36);
			vector += new Vector(Rand.Next(-10, 10), Rand.Next(-10, 10)) * 16;
			Vel = -vector;
			Vel.Length = 10;
			Proj(TargetPlayer.Center + vector, Vel, ProjectileID.DesertDjinnCurse, 144);
		}
		#endregion
		#region SelectMode
		private void SelectMode()
		{
			modetime = 0;
			switch(lastMode)
			{
				#region Flame
				case BossMode.RedDevilFlame:
					Mode = BossMode.RedDevilLaser;
					break;
				#endregion
				#region Laser
				case BossMode.RedDevilLaser:
					Mode = BossMode.RedDevilTrident;
					break;
				#endregion
				#region Trident
				case BossMode.RedDevilTrident:
					Mode = BossMode.SummonFollows;
					break;
				#endregion
				#region SummonFollows
				case BossMode.SummonFollows:
					Mode = BossMode.RedDevilFlame;
					break;
					#endregion
			}
		}
		#endregion
		#endregion
	}
}
