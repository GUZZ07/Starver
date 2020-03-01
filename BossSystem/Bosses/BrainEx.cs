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
	public class BrainEx : StarverBoss
	{
		#region Fields
		private NPCSystem.NPCs.BrainFollow[] FollowsInter;
		private NPCSystem.NPCs.BrainFollow[] FollowsOuter;
		private int Inter;
		private int Outer;
		private int i;
		private int aliveFollows;
		#endregion
		#region Properties
		public int AliveFollows
		{
			get
			{
				return aliveFollows;
			}
			set
			{
				if(!Active)
				{
					return;
				}
				aliveFollows = value;
#if DEBUG
				StarverPlayer.All.SendMessage($"aliveFollows:{aliveFollows}", Color.Blue);
#endif
			}
		}
		#endregion
		#region Ctor
		public BrainEx() : base(2)
		{
			ComingMessage = "血液在沸腾...";
			ComingMessageColor = Color.Red;
			TaskNeed = 23;
			Name = "克苏鲁之脑";
			IgnoreDistance = true;
			DefaultLife = 262500;
			DefaultLifes = 30;
			RawType = NPCID.BrainofCthulhu;
			Drops = new DropItem[] 
			{
				new DropItem(new int[] { Currency.Magic, Currency.Melee }, 3, 7, 0.55555f),
				new DropItem(new int[] { Currency.Ranged }, 1, 3, 0.16f)
			};
		}
		#endregion
		#region Dtor
		/*
		~BrainEx()
		{
			for (i = 0; i < 4; ++i)
			{
				TerrariaApi.Server.ServerApi.Hooks.GameUpdate.Deregister(Starver.Instance, FollowsInter[i].AI);
				TerrariaApi.Server.ServerApi.Hooks.GameUpdate.Deregister(Starver.Instance, FollowsOuter[i].AI);
			}
			for (; i < 8; ++i)
			{
				TerrariaApi.Server.ServerApi.Hooks.GameUpdate.Deregister(Starver.Instance, FollowsOuter[i].AI);
			}
		}
		*/
		#endregion
		#region Spawn
		public override void Spawn(Vector2 where, int lvl = CriticalLevel)
		{
			base.Spawn(where, lvl);
			Center += Rand.NextVector2(16 * 30);
			NewFollows();
		}
		#endregion
		#region BeDown
		protected override void BeDown()
		{
			foreach(var ply in Starver.Players)
			{
				if(ply is null)
				{
					continue;
				}
				RealNPC.playerInteraction[ply.Index] = true;
			}
			base.BeDown();
			for (i = 0; i < Inter; ++i)
			{
				FollowsInter[i].KillMe();
				FollowsOuter[i].KillMe();
			}
			for (; i < Outer; ++i)
			{
				FollowsOuter[i].KillMe();
			}
		}
		#endregion
		#region LifeDown
		public override void LifeDown()
		{
			base.LifeDown();
			if (Lifes > 0)
			{
				NewFollows();
			}
		}
		#endregion
		#region KillMe
		public override void KillMe()
		{
			base.KillMe();
			/*
			for (i = 0; i < Starver.Players.Length; ++i)
			{
				if (TShockAPI.TShock.Players[i] == null || !TShockAPI.TShock.Players[i].Active)
				{
					continue;
				}
				TShockAPI.TShock.Players[i].Confused = false;
			}
			*/
		}
		#endregion
		#region RealAI
		//ai[0]<0时会更改无敌状态
		public override void RealAI()
		{
			if (Vector2.Distance(Center, TargetPlayer.Center) > 16 * 6.5f)
			{
				vector = (Vector)(TargetPlayer.Center - Center);
				vector.Length = 2;
				FakeVelocity = vector;
			}
			else
			{
				Center = TargetPlayer.Center + Rand.NextVector2(16 * 40);
			}
			/*
			for (i = 0; i < FollowsInter.Length; ++i)
			{
				FollowsInter[i].AI();
				FollowsOuter[i].AI();
			}
			*/
			for (; i < FollowsOuter.Length; ++i)
			{
				FollowsOuter[i].AI();
			}
			if (Timer % 120 > 60)
			{
				RealNPC.dontTakeDamage = true;
			}
			else
			{
				RealNPC.dontTakeDamage = false;
			}
			if (Timer % 60 == 0)
			{
				if (Timer % 60 * 5 == 0)
				{
					if (aliveFollows <= 1)//|| !CheckFollows())
					{
						LifeDown();
					}
				}
				TargetPlayer.Velocity += Rand.NextVector2(Rand.Next(0, 36));
			}
		}
		#endregion
		#region CheckFollows
		private bool CheckFollows()
		{
			bool result = false;
			for (i = 0; i < Inter; ++i)
			{
				result |= FollowsInter[i].Active;
				result |= FollowsOuter[i].Active;
			}
			for (; i < Outer; ++i)
			{
				result |= FollowsOuter[i].Active;
			}
			return result;
		}
		#endregion
		#region NewFollows
		private void NewFollows()
		{
			Inter = LifesMax / Lifes / 10 + 4;
			Outer = LifesMax / Lifes / 5 + 8;
			FollowsInter = new NPCSystem.NPCs.BrainFollow[Inter];
			FollowsOuter = new NPCSystem.NPCs.BrainFollow[Outer];
			for (i = 0; i < Inter; ++i)
			{
				FollowsInter[i] = new NPCSystem.NPCs.BrainFollow();
				FollowsOuter[i] = new NPCSystem.NPCs.BrainFollow();
				FollowsInter[i].Spawn((Vector)Center, this, i * PI * 2 / FollowsInter.Length, 16 * 15, 0);
				FollowsOuter[i].Spawn((Vector)Center, this, i * PI * 2 / FollowsOuter.Length, 16 * 25, 1);
			}
			for (; i < Outer; ++i)
			{
				FollowsOuter[i] = new NPCSystem.NPCs.BrainFollow();
				FollowsOuter[i].Spawn((Vector)Center, this, i * PI * 2 / FollowsOuter.Length, 16 * 25, 1);
			}
			aliveFollows = FollowsInter.Length + FollowsOuter.Length;
		}
		#endregion
	}
}
