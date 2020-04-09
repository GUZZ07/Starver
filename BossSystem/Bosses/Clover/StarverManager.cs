using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Xna.Framework;

namespace Starvers.BossSystem.Bosses.Clover
{
	using Base;
	using Starvers.WeaponSystem;
	using Terraria.ID;
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public partial class StarverManager : StarverBoss
	{
		#region Fields
		internal const float Radium = 16 * 55;
		private Rectangle MyRect;
		private FinalWander CrazyWang = new FinalWander();
		private FinalRedeemer Deaths = new FinalRedeemer();
		private FinalAdjudicator Wither = new FinalAdjudicator();
		private FinalDestroyer TOFOUT = new FinalDestroyer();
		#endregion
		#region Ctor
		public StarverManager() : base(3)
		{
			TaskNeed = 42;
			FullName = "Revolc The Starver Manager";
			Name = "The Starver Manager";
			RawType = NPCID.MoonLordCore;
			DefaultDefense = 3000;
			DefaultLife = 100000;
			DefaultLifes = 1000;
			Drops = new DropItem[] 
			{
				new DropItem(new int[] { ItemID.CultistBossBag }, 20, 21),
				new DropItem(new int[] { Currency.Melee }, 99 / 4, 80),
				new DropItem(new int[] { Currency.Ranged }, 99 / 4, 80),
				new DropItem(new int[] { Currency.Magic }, 99 / 4, 80),
				new DropItem(new int[] { Currency.Minion }, 99 / 4, 80),
			};
		}
		#endregion
		#region KillMe
		public override void KillMe()
		{
			base.KillMe();
			CrazyWang.KillMe();
			Deaths.KillMe();
			Wither.KillMe();
			TOFOUT.KillMe();
		}
		#endregion
		#region Fail
		public override void OnFail()
		{
			base.OnFail();
			if (ExVersion && EndTrial)
			{
				StarverPlayer.All.SendMessage("世界正在崩塌...", Color.Black);
				int idx;
				for (int i = 0; i < 10; i++)
				{
					foreach (var ply in Starver.Players)
					{
						if(ply == null)
						{
							continue;
						}
						idx = Terraria.NPC.NewNPC((int)ply.Center.X, (int)ply.Center.Y, NPCID.MoonLordCore);
						Terraria.NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, null, idx);
						Terraria.Main.npc[idx].aiStyle = -1;
					}
					Thread.Sleep(1000);
				}
				TShockAPI.TShock.Utils.StopServer(true, "你们失败了...");
			}
		}
		#endregion
		#region Downed
		protected override void BeDown()
		{
			base.BeDown();
			if(ExVersion && EndTrial)
			{
				EndTrialProcess++;
			}
		}
		#endregion
		#region Spawn
		public override void Spawn(Vector2 where, int lvl = 5000)
		{
			#region Self
			base.Spawn(where, lvl);
			RealNPC.width = 16 * 40;
			RealNPC.height = 16 * 40;
			RealNPC.dontTakeDamage = false;
			SendData();
			#endregion
			#region TheFollows
			CrazyWang.Spawn(where, Level - 1000, this);
			Deaths.Spawn(where, Level - 1000, this);
			Wither.Spawn(where, Level - 1000, this);
			TOFOUT.Spawn(where, Level - 1000, this);
			#endregion
		}
		#endregion
		#region RealAI
		public override void RealAI()
		{
			#region TheFollows
			#region CheckActive
			if (!CrazyWang.Active)
			{
				CrazyWang.Respawn();
			}
			if(!Deaths.Active)
			{
				Deaths.Respawn();
			}
			if(!Wither.Active)
			{
				Wither.Respawn();
			}
			if(!TOFOUT.Active)
			{
				TOFOUT.Respawn();
			}
			#endregion
			#region Update
			CrazyWang.AI();
			Deaths.AI();
			Wither.AI();
			TOFOUT.AI();
			CrazyWang.DontTakeDamage = true;
			Deaths.DontTakeDamage = true;
			Wither.DontTakeDamage = true;
			TOFOUT.DontTakeDamage = true;
			//CrazyWang.RealLife = Index;
			//Deaths.RealLife = Index;
			//Wither.RealLife = Index;
			//TOFOUT.RealLife = Index;
			#endregion
			#endregion
			#region Self
			#region Common
			#region TrackingTarget
			FakeVelocity = (Vector)(TargetPlayer.Center - Center) / 13;
			#endregion
			#region DetectDamage
			DontTakeDamage = false;
			MyRect = new Rectangle((int)Center.X - 16 * 20, (int)Center.Y - 16 * 20, 16 * 40, 16 * 40);
			foreach(var proj in Terraria.Main.projectile)
			{
				if (proj.friendly == false || proj.active == false)
				{
					continue;
				}
				if(Vector2.Distance(proj.Center,Center) < 16 * 20)
				{
					RealNPC.StrikeNPC(proj.damage, 0, 0, false, true, true, Terraria.Main.player[proj.owner]);
					if ((proj.penetrate -= 1) < 1)
					{
						proj.Kill();
					}
					//proj.Damage();
				}
			}
			
			#endregion
			#endregion
			#region Mode
			#endregion
			#endregion
		}
		#endregion
	}
}
