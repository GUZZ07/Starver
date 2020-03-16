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
	public class DarkMageEx : StarverBoss
	{
		#region Fields
		private int count;
		private int rtime;
		#endregion
		#region Spawn
		public override void Spawn(Vector2 vector, int lvl = 2000)
		{
			base.Spawn(vector, lvl);
			LifesMax = DefaultLifes * AlivePlayers();
			lastMode = BossMode.SummonFollows;
			WhereToGo = (Vector)Center;
		}
		#endregion
		#region Ctor
		public DarkMageEx() : base(1)
		{
			ComingMessage = "远古的力量正在缓缓凝聚...";
			ComingMessageColor = Color.Pink;
			DownedMessage = "守护者们自沉睡中苏醒...";
			DownedMessageColor = Color.Yellow;
			TaskNeed = 26;
			Name = "dark膜法师";
			LifeperPlayerType = ByLifes;
			RawType = Starver.IsPE ? NPCID.RuneWizard : NPCID.DD2DarkMageT3;
			DefaultLifes = 60;
			DefaultLife = 32500;
			DefaultDefense = 180;
			IgnoreDistance = true;
			Drops = new DropItem[]
			{
				new DropItem(new int[]{Currency.Magic},18,19,1)
			};
		}
		#endregion
		#region RealAI
		public unsafe override void RealAI()
		{
			#region Modes
			switch (Mode)
			{
				#region SelectMode
				case BossMode.WaitForMode:
					SelectMode();
					break;
				#endregion
				#region DrakinShot
				case BossMode.DarkMageDrakinShot:
					if (rtime > 8)
					{
						ResetMode();
						rtime = 0;
						floats[0] = 0;
						break;
					}
					if (Timer % 60 == 0)
					{
						DrakinShot();
					}
					break;
				#endregion
				#region SelfShot
				case BossMode.DarkMageSelfShot:
					if (modetime > 60 * 6.5f)
					{
						ResetMode();
						break;
					}
					if (Timer % 44 == 0)
					{
						SelfShot();
					}
					break;
				#endregion
				#region DarkSigil
				case BossMode.DarkMageSigil:
					if (rtime > 30)
					{
						ResetMode();
						rtime = 0;
						break;
					}
					if (Timer % 40 == 0)
					{
						DarkMageSigil();
					}
					break;
				#endregion
				#region SummonFollows
				case BossMode.SummonFollows:
					if (rtime > 9)
					{
						rtime = 0;
						ResetMode();
						break;
					}
					if (Timer % 30 == 0)
					{
						SummonFollows();
					}
					break;
					#endregion
			}
			#endregion
			#region Common
			if (Mode != BossMode.DarkMageSelfShot)
			{
				if (Timer % 60 * 3 == 0)
				{
					WhereToGo = (Vector)(Rand.NextVector2(16 * 20) + TargetPlayer.Center);
				}
				if (Timer % 10 == 0)
				{
					FakeVelocity = (Vector)(WhereToGo - Center);
					FakeVelocity /= 13;
				}
			}
			#endregion
		}
		#endregion
		#region AIs
		#region DarkMageSigil
		private void DarkMageSigil()
		{
			if (Starver.IsPE)
			{
				Vel = FromPolar(floats[0], 170);
				vector = Vel;
				Vel.Length = 4;
				Proj(TargetPlayer.Center + vector, -Vel, ProjectileID.CultistRitual, 129);
				rtime++;
				floats[0] += PI / 18;
				return;
			}
			Vel = FromPolar(floats[0], 170);
			vector = Vel;
			Vel.Length = 4;
			Proj(TargetPlayer.Center + vector, -Vel, ProjectileID.DD2DarkMageRaise, 129);
			rtime++;
			floats[0] += PI / 18;
		}
		#endregion
		#region DrakinShot
		private void DrakinShot()
		{
			Vel = (Vector)(TargetPlayer.Center - Center);
			Vel.Length = 15;
			int shot = Starver.IsPE ? ProjectileID.RuneBlast : ProjectileID.DD2DrakinShot;
			ProjSector(Center, Vel.Length, 0, Vel.Angle, Math.PI / 5, 127, shot, 5);
			ProjSector(Center, Vel.Length, 0, Vel.Angle, Math.PI / 5, 127, shot, 5);
			rtime++;
		}
		#endregion
		#region SelfShot
		private void SelfShot()
		{
			Vel = (Vector)(TargetPlayer.Center - Center);
			Vel.Length = 20;
			vector =Vel.Vertical();
			vector.Length = 16 * 10;
			int shot = Starver.IsPE ? ProjectileID.PhantasmalDeathray : ProjectileID.DD2DarkMageBolt;
			ProjLine(Center + vector, Center - vector, Vel, 13, 127, shot);
			FakeVelocity = Vel;
		}
		#endregion
		#region SummonFollows
		private new void SummonFollows()
		{
			count = AlivePlayers();
			int follow = Starver.IsPE ? NPCID.ArmoredSkeleton : NPCID.DD2SkeletonT1;
			if (rtime++ % 3 != 0)
			{
				vector = (Vector)Rand.NextVector2(100, 100);
				Vel = -vector;
				Vel.Length = 10;
				NewNPC((Vector)(Center + vector), Vel, follow, 100 * count);
			}
			else
			{
				for (int i = 0; i < 3; i++)
				{
					vector = (Vector)Rand.NextVector2(100, 100);
					Vel = -vector;
					Vel.Length = 10;
					NewNPC((Vector)(Center + vector), Vel, follow, 1200 * count);
				}
			}
		}
		#endregion
		#region SelectMode
		private void SelectMode()
		{
			modetime = 0;
			switch (lastMode)
			{
				#region SummonFollows
				case BossMode.SummonFollows:
					Mode = BossMode.DarkMageDrakinShot;
					break;
				#endregion
				#region DrakinShot
				case BossMode.DarkMageDrakinShot:
					Mode = BossMode.DarkMageSelfShot;
					break;
				#endregion
				#region SelfShot
				case BossMode.DarkMageSelfShot:
					Mode = BossMode.DarkMageSigil;
					break;
				#endregion
				#region Sigil
				case BossMode.DarkMageSigil:
					Mode = BossMode.SummonFollows;
					break;
					#endregion
			}
		}
		#endregion
		#endregion
	}
}
