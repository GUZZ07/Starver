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
    using Terraria;
    using Terraria.ID;
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public class PigronEx : StarverBoss
	{
		#region Fields
		private const int Summons = 8;
		private bool ShotDir;
		private bool Left;
		/// <summary>
		/// (16 * 24,0)
		/// </summary>
		private Vector UnitX = new Vector(16 * 24, 0);
		#endregion
		#region Ctor
		public PigronEx() : base(1)
		{
			TaskNeed = 33;
			Name = "猪龙";
			RawType = NPCID.PigronHallow;
			DefaultDefense = 53;
			DefaultLife = 30000;
			DefaultLifes = 80;
			DamagedIndex = 0.00017f;
			LifeperPlayerType = ByLifes;
			Drops = new DropItem[]
			{
				new DropItem(new int[] { Currency.Ranged,Currency.Melee }, 7, 12, 0.7f),
				new DropItem(new int[] { Currency.Magic }, 7, 10, 0.8f),
			};
		}
		#endregion
		#region Spawn
		public override void Spawn(Vector2 where, int lvl = CriticalLevel)
		{
			base.Spawn(where, lvl);
			lastMode = BossMode.SummonFollows;
		}
		#endregion
		#region RealAI
		public override void RealAI()
		{
			#region Modes
			switch(Mode)
			{
				#region SelectMode
				case BossMode.WaitForMode:
					SelectMode();
					break;
				#endregion
				#region Arrow
				case BossMode.PigronArrow:
					if(modetime > 60 * 7)
					{
						ResetMode();
						break;
					}
					if(Timer % 60 == 0)
					{
						Arrow();
					}
					break;
				#endregion
				#region Feather
				case BossMode.PigronFeather:
					if(modetime > 60 * 7)
					{
						ResetMode();
						break;
					}
					if(Timer % 60 == 0)
					{
						Feather();
					}
					break;
				#endregion
				#region Web
				case BossMode.PigronWeb:
					if(modetime > 60 * 3.7f)
					{
						ResetMode();
						break;
					}
					if(Timer % 3 == 0)
					{
						Web();
					}
					break;
				#endregion
				#region FrostBlaster
				case BossMode.FrostBlast:
					if(modetime > 60 * 5)
					{
						ResetMode();
						break;
					}
					if(Timer % 4 == 0)
					{
						FrostBlast();
					}
					break;
				#endregion
				#region SummonFollows
				case BossMode.SummonFollows:
					if(modetime > 60 * 6)
					{
						ResetMode();
					}
					if(Timer % 60 * 3 / 2 == 0)
					{
						SummonPigronFollows();
					}
					break;
					#endregion
			}
			#endregion
			#region Common
			if (Timer % 10 == 0)
			{
				if (Left)
				{
					WhereToGo = (Vector)TargetPlayer.Center - UnitX;
				}
				else
				{
					WhereToGo = (Vector)TargetPlayer.Center + UnitX;
				}
				FakeVelocity = (Vector)(WhereToGo - Center) / 20;
			}
			#endregion
		}
		#endregion
		#region AIs
		#region FrostBlast
		private void FrostBlast()
		{
			if(Left)
			{
				vector = FromPolar(0, 23);
				vector.Y += Rand.Next(-4, 4);
			}
			else
			{
				vector = FromPolar(PI, 23);
				vector.Y += Rand.Next(-4, 4);
			}
			Proj(Center, vector, ProjectileID.FrostBlastHostile, 162);
		}
		#endregion
		#region Web
		private void Web()
		{
			vector = (Vector)(TargetPlayer.Center + new Vector2(0, -16 * 30));
			Vel = new Vector(0, 10);
			NewProj(Rand.NextVector2(16 * 34, 0) + vector, Vel, ProjectileID.WebSpit, 163);
		}
		#endregion
		#region Feather
		private void Feather()
		{
			vector = (Vector)TargetPlayer.Center;
			vector.Y -= 16 * 20;
			Vel = vector + UnitX * 2;
			vector -= UnitX * 2;
			Vector tempVel = (ShotDir = !ShotDir) ? FromPolar(PI / 4, 13) : FromPolar(PI / 4 + PI / 2, 13);
			ProjLine(vector, Vel, tempVel, 23, 163, ProjectileID.HarpyFeather);
		}
		#endregion
		#region Arrow
		private void Arrow()
		{
			ProjSector(Center, 17, 2, (TargetPlayer.Center - Center).Angle(), PI * 3 / 4, 160, ProjectileID.WoodenArrowHostile, 8); ;
		}
		#endregion
		#region SummonFollows
		private void SummonPigronFollows()
		{
			vector = (Vector)(Center + new Vector(0, -16 * 20));
			Vel = (Vector)(Center + new Vector(16 * 20, 0));
			Vector Unit = (Vel - vector) / Summons;

			int count = Main.npc.Count(npc =>
			{
				return
				npc.active &&
				(npc.type == NPCID.PigronCorruption || npc.type == NPCID.PigronCrimson) &&
				(npc.defense > 3000);
			});
			if(count >= Summons * 4)
			{
				modetime = 60 * 6 + 1;
				return;
			}
			while (Vector.Distance(Vel, vector) > 16 * 2f)
			{
				NewNPC(vector, Vector.Zero, NPCID.PigronCorruption, 23300, 4000);
				NewNPC(Vel, Vector.Zero, NPCID.PigronCrimson, 23300, 4000);
				vector += Unit;
				Vel -= Unit;
			}
		}
		#endregion
		#region SelectMode
		private void SelectMode()
		{
			modetime = 0;
			Left = !Left;
			switch(lastMode)
			{
				#region Arrow
				case BossMode.PigronArrow:
					Mode = BossMode.PigronFeather;
					break;
				#endregion
				#region Feather
				case BossMode.PigronFeather:
					Mode = BossMode.PigronWeb;
					break;
				#endregion
				#region Web
				case BossMode.PigronWeb:
					Mode = BossMode.FrostBlast;
					break;
				#endregion
				#region FrostBlast
				case BossMode.FrostBlast:
					Mode = BossMode.SummonFollows;
					break;
				#endregion
				#region SummonFollows
				case BossMode.SummonFollows:
					Mode = BossMode.PigronArrow;
					break;
					#endregion
			}
		}
		#endregion
		#endregion
	}
}
