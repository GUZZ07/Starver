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
	public class StarverDestroyer : StarverBoss
	{
		#region Fields
		protected bool flag;
		protected int idx;
		private const int StartCollidingDamage = 900;
		private DropItem[] DropsNormal = new DropItem[]
		{
			new DropItem(new int[]{ Currency.Melee }, 15, 30, 0.73f),
			new DropItem(new int[]{ Currency.Melee }, 15, 30, 0.73f),
			new DropItem(new int[]{ Currency.Melee }, 15, 30, 0.73f),
		};
		private DropItem[] DropsEx = new DropItem[]
		{
			new DropItem(new int[]{ Currency.Melee }, 30, 55),
			new DropItem(new int[]{ Currency.Melee }, 30, 55),
			new DropItem(new int[]{ Currency.Melee }, 30, 55),
		};
		#endregion
		#region Ctor
		public StarverDestroyer() : base(1)
		{
			TaskNeed = 30;
			Name = "The Starver Destroyer";
			FullName = "Tuofot The Starver Destroyer";
			RawType = NPCID.DukeFishron;
			IgnoreDistance = true;
			DefaultLife = 82000;
			DefaultLifes = 400;
			DefaultDefense = 4000;
			StarverAI[0] = 0;
		}
		#endregion
		#region Spawn
		public override void Spawn(Vector2 where, int lvl = CriticalLevel)
		{
			base.Spawn(where, lvl);
			flag = true;
			Mode = BossMode.Present;
			RushBegin(flag);
			Center = where + Rand.NextVector2(16 * 60);
			Drops = ExVersion ? DropsEx : DropsNormal;
			RealNPC.damage = StartCollidingDamage;
		}
		#endregion
		#region OnFail
		public override void OnFail()
		{
			base.OnFail();
			if(ExVersion && EndTrial)
			{
				EndTrial = false;
				EndTrialProcess = 0;
				StarverPlayer.All.SendMessage("你们准备好了吗?", Color.HotPink);
				StarverPlayer.All.SendMessage("你们就是下一个", Color.HotPink);
			}
		}
		#endregion
		#region LifeDown
		public override void LifeDown()
		{
			base.LifeDown();
			RealNPC.damage = (int)(StartCollidingDamage * DamageIndex);
		}
		#endregion
		#region Downed
		protected override void BeDown()
		{
			base.BeDown();
			if(ExVersion && EndTrial)
			{
				StarverPlayer.All.SendMessage("连我都拦不住你们了...", Color.HotPink);
				StarverPlayer.All.SendMessage("告诉你们一个很不好的消息...", Color.HotPink);
				StarverPlayer.All.SendMessage("??��&�%�Qv��", Color.HotPink);
				StarverPlayer.All.SendMessage("关于他的出现", Color.HotPink);
				StarverPlayer.All.SendMessage("���b�D#rʃ�Y�", Color.HotPink);
				StarverPlayer.All.SendMessage("�-�ovGo�5�K�", Color.HotPink);
				StarverPlayer.All.SendMessage("但是后来去发生了另一件事...", Color.HotPink);
				StarverPlayer.All.SendMessage("U�Ϯr}v�c��̋=�", Color.HotPink);
				StarverPlayer.All.SendMessage("没有任何事物能阻挡他了...没有...", Color.HotPink);
				StarverPlayer.All.SendMessage("包括你们...", Color.HotPink);
				EndTrialProcess++;
			}
		}
		#endregion
		#region RealAI
		public override void RealAI()
		{
			switch (Mode)
			{
				#region SelectMode
				case BossMode.WaitForMode:
					SelectMode();
					break;
				#endregion
				#region Rush
				case BossMode.Rush:
					if (modetime > 60 * 5)
					{
						Mode = BossMode.WaitForMode;
						double rad = (Center - TargetPlayer.Center).Angle();
						vector = Vector.FromPolar(rad, 16 * 30f);
						Center = TargetPlayer.Center +  vector ;
						FakeVelocity = default;
						break;
					}
					if (Timer % 3 == 0)
					{
						if (Vector2.Distance(Center, TargetPlayer.Center) > 16 * 44)
						{
							NewRush();
						}
					}
					break;
				#endregion
				#region DemonSickle
				case BossMode.DemonSickle:
					if (modetime > 60 * 15)
					{
						RushBegin();
					}
					if (Timer % 2 == 0)
					{
						DemonSickle();
					}
					break;
				#endregion
				#region Fire
				case BossMode.Fire:
					if (modetime > 60 * 12)
					{
						RushBegin();
					}
					if (Timer % 2 == 0)
					{
						Fire();
					}
					break;
				#endregion
				#region FlamingScythe
				case BossMode.FlamingScythe:
					unsafe
					{
						if (StarverAI[0] > 8)
						{
							StarverAI[0] = 0;
							RushBegin();
						}
						if (Timer % 90 == 0)
						{
							FlamingScythe((int)StarverAI[0]++);
						}
					}
					break;
				#endregion
				#region Present
				case BossMode.Present:
					if (modetime > 60 * 12)
					{
						RushBegin();
					}
					if (Timer % 75 == 0)
					{
						Present();
					}
					break;
				#endregion
				#region SummonFollows
				case BossMode.SummonFollows:
					if (modetime > 60 * 9.5f)
					{
						RushBegin();
					}
					if (Timer % 75 == 0)
					{
						SummonFollows();
					}
					break;
					#endregion
			}
			if (ExVersion)
			{
				TargetPlayer.TPlayer.ZoneTowerSolar = true;
				TargetPlayer.SendData(PacketTypes.Zones, "", Target);
			}
		}
		#endregion
		#region AIs
		#region Rush
		protected virtual void NewRush()
		{
			Rush();
			FakeVelocity *= 3;
		}
		#endregion
		#region SummonFollows
		protected new void SummonFollows()
		{
			int X = (int)Center.X;
			int Y = (int)Center.Y;
			idx = NPC.NewNPC(X, Y, NPCID.SolarSolenian);
			Main.npc[idx].life = Main.npc[idx].lifeMax = 9000;
			Main.npc[idx].defense = 200;
			Main.npc[idx].velocity = Rand.NextVector2(14);
			SendData(idx);
			for (int i = 0; i < 4; i++)
			{
				idx = NPC.NewNPC(X, Y, NPCID.SolarCrawltipedeHead + Rand.Next(7));
				Main.npc[idx].life = Main.npc[idx].lifeMax = 12000;
				Main.npc[idx].defense = 200;
				Main.npc[idx].velocity = Rand.NextVector2(14);
				SendData(idx);
			}
		}
		#endregion
		#region Present
		protected void Present()
		{
			if (ExVersion)
			{
				vector = (Vector)(TargetPlayer.Center - Center)  ;
				foreach (var player in Starver.Players)
				{
					if (player == null || !player.Active)
					{
						continue;
					}
					ProjSector(player.Center, 4, 16 * 9, -PI / 2, PI * 2 / 3, 362, ProjectileID.Present, 12);
				}
			}
			else
			{
				ProjSector(TargetPlayer.Center, 8, 16 * 9, -PI / 2, PI * 2 / 3, 281, ProjectileID.Present, 9);
			}
		}
		#endregion
		#region FlamingScythe
		protected void FlamingScythe(int Times)
		{
			foreach (var player in Starver.Players)
			{
				if (player == null || !player.Active)
				{
					continue;
				}
				ProjCircle(player.Center, 16 * 20 + Times * 100, 0.1f, ProjectileID.FlamingScythe, 8, 305, 1);
			}
		}
		#endregion
		#region Fire
		protected void Fire()
		{
			Vel = (Vector)(Center - TargetPlayer.Center) ;
			flag = Timer % 60 == 0;
			if (ExVersion)
			{
				vector = Vel;
				Vel.Angle+= (Rand.NextAngle() - PI) * 2 / 7;
				Vel.Length = 30f;
				foreach (var player in Starver.Players)
				{
					if (player == null || !player.Active)
					{
						continue;
					}
					Proj(player.Center +  vector , -Vel , ProjectileID.CursedFlameHostile, 340);
					if (flag)
					{
						ProjSector(player.Center +  vector , 33, 2, Vel.Angle+ PI, PI * 3 / 4, 600, ProjectileID.DD2BetsyFireball, 8);
					}
				}
			}
			else
			{
				Vel.Length = 20f;
				Vel.Angle+= (Rand.NextAngle() - PI) * 2 / 7;
				Proj(Center, -Vel , ProjectileID.CursedFlameHostile, 294);
				if (flag)
				{
					ProjSector(Center, 33, 2, Vel.Angle+ PI, PI * 3 / 4, 512, ProjectileID.DD2BetsyFireball, 8);
				}
			}
		}
		#endregion
		#region DemonSickle
		protected void DemonSickle()
		{
			Vel = (Vector)(Center - TargetPlayer.Center) ;
			if (ExVersion)
			{
				vector = Vel;
				Vel.Length = 30f;
				Vel.Angle+= (Rand.NextAngle() - PI) / 11;
				foreach (var player in Starver.Players)
				{
					if (player == null || !player.Active)
					{
						continue;
					}
					Proj(vector  + player.Center, -Vel , ProjectileID.DemonSickle, 325, 2f);
				}
			}
			else
			{
				Vel.Length = 25;
				Vel.Angle+= (Rand.NextAngle() - PI) / 11;
				Proj(Center, -Vel , ProjectileID.DemonSickle, 270);
			}
		}
		#endregion
		#region SelectMode
		protected void SelectMode()
		{
			modetime = 0;
			LastCenter = Center;
			switch (lastMode)
			{
				#region DemonSickle
				case BossMode.DemonSickle:
					Mode = BossMode.Fire;
					break;
				#endregion
				#region Fire
				case BossMode.Fire:
					Mode = ExVersion ? BossMode.FlamingScythe : BossMode.SummonFollows;
					break;
				#endregion
				#region FlamingScythe& SummonFollows
				case BossMode.FlamingScythe:
				case BossMode.SummonFollows:
					Mode = BossMode.Present;
					break;
				#endregion
				#region Present
				case BossMode.Present:
					Mode = BossMode.DemonSickle;
					break;
					#endregion
			}
		}
		#endregion
		#endregion
	}
}
