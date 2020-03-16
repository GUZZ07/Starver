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
	using System.Threading;
	using Terraria;
	using Terraria.ID;
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public class EyeEx : StarverBoss
	{
		#region Fields
		private byte GazingState;
		private int ModesLoopCount;
		private const int StartCollideDamage = 310;
		#endregion
		#region Properties
		private Vector Unit
		{
			get
			{
				return new Vector(0, -16 * 24) + (0, 16 * 10 * Math.Sin(Timer * Math.PI / 45));
			}
		}
		#endregion
		#region Ctor
		public EyeEx() : base(3)
		{
			ComingMessage = "你感到一种令人恐惧的注视...";
			ComingMessageColor = Color.LightGreen;
			TaskNeed = 22;
			RawType = NPCID.EyeofCthulhu;
			Name = "克苏鲁之眼";
			DefaultDefense = 5000;
			DefaultLife = 100000;
			DefaultLifes = 20;
			vector.X = 16 * 20;
			Drops = new DropItem[]
			{
				new DropItem(new []{ Currency.Melee }, 1, 5, 0.4f)
			};
		}
		#endregion
		#region Spawn
		public override void Spawn(Vector2 where, int lvl = CriticalLevel)
		{
			base.Spawn(where, lvl);
			Mode = BossMode.Explosive;
			RealNPC.damage = StartCollideDamage;
			GazingState = 0;
			ModesLoopCount = 0;
			DontTakeDamage = false;
			Defense = 6000;
		}
		#endregion
		#region LifeDown
		public override void LifeDown()
		{
			base.LifeDown();
			RealNPC.damage = (int)(StartCollideDamage * DamageIndex);
		}
		#endregion
		#region RealAI
		public override void RealAI()
		{
			#region GazingSpecial
			if (GazingState == 0 && Lifes <= LifesMax / 2)
			{
				modetime = 0;
				Mode = BossMode.Gazing;
				TargetPlayer.SendMessage("Gazing you...", 255, 0, 255);
				GazingState |= 0b00000001;
				Array.Clear(floats, 0, floats.Length);
			}
			else if (GazingState == 0b00000001 && Lifes <= LifesMax / 4)
			{
				modetime = 0;
				Mode = BossMode.Gazing;
				TargetPlayer.SendMessage("Gazing you...", 255, 0, 188);
				GazingState |= 0b00000011;
				Array.Clear(floats, 0, floats.Length);
			}
			#endregion
			#region Modes
			switch (Mode)
			{
				#region SelectMode
				case BossMode.WaitForMode:
					SelectMode();
					modetime = 0;
					break;
				#endregion
				#region Rush
				case BossMode.Rush:
					Rush();
					break;
				#endregion
				#region Sharknado
				case BossMode.Sharknado:
					if (modetime > 60 * 6)
					{
						floats[1] = 0;
						RushBegin();
					}
					if (Timer % 45 == 0)
					{
						Sharknode();
					}
					break;
				#endregion
				#region SummonFollows
				case BossMode.SummonFollows:
					if (floats[1] > 5)
					{
						floats[1] = 0;
						RushBegin();
					}
					if (Timer % 60 == 0)
					{
						SummonEyeFollows();
					}
					break;
				#endregion
				#region Trident
				case BossMode.RedDevilTrident:
					if (modetime > 60 * 8)
					{
						RushBegin();
					}
					if (Timer % 12 == 0)
					{
						EvilTrident(90);
					}
					break;
				#endregion
				#region Gazing
				case BossMode.Gazing:
					{
						if (modetime > 60 * 9)
						{
							DontTakeDamage = false;
							RushBegin();
						}
						else
						{
							FakeVelocity = default;
							DontTakeDamage = true;
							Gazing();
						}
						break;
					}
				#endregion
				#region Explosive
				case BossMode.Explosive:
					if (floats[1] > 9)
					{
						floats[1] = 0;
						RushBegin();
					}
					if (Timer % 60 == 0)
					{
						Explosive();
						++floats[1];
					}
					break;
					#endregion
			}
			#endregion
			#region Common
			if (Mode != BossMode.Rush)
			{
				Vel = (Vector)(TargetPlayer.Center + Unit - Center) / 30;
				FakeVelocity /= 50;
				FakeVelocity += Vel;
				FakeVelocity.Length = Math.Min(24, FakeVelocity.Length);
			}
			if (Lifes < LifesMax / 2)
			{
				RealNPC.ai[0] = 1f;
				RealNPC.ai[1] = 0f;
				RealNPC.ai[2] = 0f;
				RealNPC.ai[3] = 0f;
			}
			#endregion
		}
		#endregion
		#region AIs
		#region Explosive
		private void Explosive()
		{
			vector = (Vector)TargetPlayer.Center;
			ThreadPool.QueueUserWorkItem(obj =>
			{
				try
				{
					Thread.Sleep(700);
					int idx;
					double angle = Rand.NextAngle();
					for (int i = 0; i < 6; i++)
					{
						if (0 <= Target && Target < Starver.Players.Length && (TargetPlayer?.Active ?? false))
						{
							idx = Proj(vector + FromPolar(angle + PI / 3 * i, 16 * 20), Vector.Zero, ProjectileID.Explosives, 100);
							Main.projectile[idx].aiStyle = -1;
							Main.projectile[idx].owner = Target;
						}
					}
				}
				catch(Exception e)
				{
					StarverPlayer.All.SendDeBugMessage(e.ToString());
				}
			});
		}
		#endregion
		#region SummonFollows
		private void SummonEyeFollows()
		{
			if (Main.npc.Count(npc => npc.active && npc.type == NPCID.WanderingEye && npc.lifeMax > 6000) >= 3 * 6)
			{
				floats[1] = 6;
				return;
			}
			++floats[1];
			NewNPC((Vector)Center, FromPolar(PI / 2, 9), NPCID.WanderingEye, 6235, 3000);
			NewNPC((Vector)Center, FromPolar(PI / 2 + PI / 6, 9), NPCID.WanderingEye, 6235, 3000);
			NewNPC((Vector)Center, FromPolar(PI / 2 - PI / 6, 9), NPCID.WanderingEye, 6235, 3000);
		}
		#endregion
		#region Sharknado
		private void Sharknode()
		{
			floats[1] += PI / 14;
			vector = -(Vector)RelativePos;
			vector.Length = 16;
			Vel = vector;
			Vel.Angle += floats[1];
			vector.Angle -= floats[1];
			Proj(Center, vector, ProjectileID.SharknadoBolt, 83);
			Proj(Center, Vel, ProjectileID.SharknadoBolt, 80);
		}
		#endregion
		#region Gazing
		private void Gazing()
		{
			TargetPlayer.SetBuff(BuffID.Obstructed, 90);
			if (modetime % 60 == 0)
			{
				ProjCircle(TargetPlayer.Center, 16 * 30, -10, ProjectileID.RuneBlast, Rand.Next(4, 6), 120);
			}
		}
		#endregion
		#region SelectMode
		private void SelectMode()
		{
			switch (lastMode)
			{
				#region Sharknado
				case BossMode.Sharknado:
					Mode = BossMode.SummonFollows;
					break;
				#endregion
				#region SummonFollows
				case BossMode.SummonFollows:
					Mode = BossMode.RedDevilTrident;
					break;
				#endregion
				#region Trident
				case BossMode.RedDevilTrident:
					ModesLoopCount++;
					if (ModesLoopCount % 3 == 0 && Lifes <= LifesMax / 2)
					{
						TargetPlayer.SendMessage("Gazing you...", 255,0,200);
						Mode = BossMode.Gazing;
					}
					else
					{
						Mode = BossMode.Explosive;
					}
					break;
				#endregion
				#region Gazing
				case BossMode.Gazing:
					Mode = BossMode.Explosive;
					break;
				#endregion
				#region Explosive
				case BossMode.Explosive:
					Mode = BossMode.Sharknado;
					break;
					#endregion
			}
		}
		#endregion
		#endregion
	}
}
