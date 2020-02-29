using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace Starvers.BossSystem.Bosses
{
	using Vector = TOFOUT.Terraria.Server.Vector2;
	using BigInt = System.Numerics.BigInteger;
	using Microsoft.Xna.Framework;
	using Starvers.WeaponSystem;

	public class QueenBeeEx : Base.StarverBoss
	{
		#region Fields
		#endregion
		#region Ctor
		public QueenBeeEx():base(2)
		{
			TaskNeed = 24;
			RawType = NPCID.QueenBee;
			Name = "蜂后";
			DefaultLife = 45500;
			DefaultDefense = 50;
			DefaultLifes = 60;
			StarverAI[0] = 0;
			Drops = new DropItem[]
			{
				new DropItem(new int[]{Currency.Ranged,Currency.Melee }, 3, 7, 0.53f)
			};
		}
		#endregion
		#region Spawn
		public override void Spawn(Vector2 where, int lvl = 2000)
		{
			base.Spawn(where, lvl);
			Mode = BossMode.SummonFollows;
			unsafe
			{
				StarverAI[0] = 1;
			}
		}
		#endregion
		#region RealAI
		public unsafe override void RealAI()
		{
			switch(Mode)
			{
				#region SelectMode
				case BossMode.WaitForMode:
					SelectMode();
					break;
				#endregion
				#region StingerArray
				case BossMode.QueenBeeStingerArray:
					if(modetime > 60 * 8)
					{
						ResetMode();
						break;
					}
					if (Timer % 3 == 0)
					{
						StingerArray();
					}
					break;
				#endregion
				#region StingerRain
				case BossMode.QueenBeeStingerRain:
					if(modetime > 60 * 7.46f)
					{
						ResetMode();
						break;
					}
					StingerRain();
					break;
				#endregion
				#region StingerRound
				case BossMode.QueenBeeStingerRound:
					if(StarverAI[1] > 6)
					{
						StarverAI[1] = 0;
						ResetMode();
						break;
					}
					if (Timer % 73 == 0)
					{
						switch(StarverAI[0])
						{
							case 1f:
								ProjCircle(Center, 2, 15, ProjectileID.Stinger, 20, 112, 2);
								break;
							case -1f:
								ProjCircle(Center, 15 * 73, 15, ProjectileID.Stinger, 26, 106, 1);
								break;
						}
						StarverAI[1] += 0.5f;
						StarverAI[0] *= -1;
					}
					break;
				#endregion
				#region SummonFollows
				case BossMode.SummonFollows:
					if (modetime > 60 * 5)
					{
						ResetMode();
						break;
					}
					if(Timer % 60 == 0)
					{
						SummonFollows();
					}
					break;
					#endregion
			}
			#region TrackingTarget
			if (Timer % 101 == 0)
			{
				WhereToGo = (Vector)(TargetPlayer.Center + Rand.NextVector2(16 * 20));
			}
			FakeVelocity += WhereToGo - (Vector)Center;
			FakeVelocity.Length = 2;
			#endregion
		}
		#endregion
		#region Ais
		#region SelectMode
		private void SelectMode()
		{
			modetime = 0;
			switch(lastMode)
			{
				#region StingerArray
				case BossMode.QueenBeeStingerArray:
					Mode = BossMode.QueenBeeStingerRain;
					Vel.X = 0;
					Vel.Y = 16 * 30;
					vector = (Vector)Center - Vel;
					Vel.Y = 14;
					break;
				#endregion
				#region StingerRain
				case BossMode.QueenBeeStingerRain:
					Mode = BossMode.QueenBeeStingerRound;
					break;
				#endregion
				#region StingerRound
				case BossMode.QueenBeeStingerRound:
					Mode = BossMode.SummonFollows;
					break;
				#endregion
				#region SummonFollows
				case BossMode.SummonFollows:
					Mode = BossMode.QueenBeeStingerArray;
					break;
					#endregion
			}
		}
		#endregion
		#region StingerArray
		private void StingerArray()
		{
			Proj(Center + Rand.NextVector2(Rand.Next(16 * 30)), Rand.NextVector2(Rand.Next(1,10)), ProjectileID.Stinger, 120);
		}
		#endregion
		#region StingerRain
		private void StingerRain()
		{
			vector = (Vector)Center;
			vector.Y -= 16 * 30;
			Proj(vector + Rand.NextVector2(16 * 60, 0), Vel, ProjectileID.Stinger, 108);
		}
		#endregion
		#region SummonFollows
		private new void SummonFollows()
		{
			int alive = AlivePlayers();
			int idx = NewNPC((Vector)Center, (Vector)Rand.NextVector2(26.66f), NPCID.HornetSpikey, alive * 400, 78);
			SendData(idx);
			idx = NewNPC((Vector)Center, (Vector)Rand.NextVector2(26.66f), NPCID.MossHornet, alive * 400, 78);
			SendData(idx);
		}
		#endregion
		#endregion
	}
}
