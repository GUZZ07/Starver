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
	public class SkeletronEx : StarverBoss
	{
		#region Fields
		private const int MaxArms = 4;
		private int[] ArmsUpper = new int[MaxArms];
		private int[] ArmsLower = new int[MaxArms];
		#endregion
		#region Ctor
		public SkeletronEx() : base(2)
		{
			ComingMessage = "你感到全身上下的骨头都在战粟...";
			ComingMessageColor = Color.Gray;
			TaskNeed = 25;
			RawType = NPCID.SkeletronHead;
			DefaultDefense = 50;
			DefaultLife = 110000;
			DefaultLifes = 70;
			Drops = new DropItem[]
			{
				new DropItem(new int[]{Currency.Melee},3,5,0.8f),
				new DropItem(new int[]{ Currency.Magic},1,11,1,false)
			};
		}
		#endregion
		#region Spawn
		public override void Spawn(Vector2 where, int lvl = CriticalLevel)
		{
			base.Spawn(where, lvl);
			floats[0] = PI / 2;
			Mode = BossMode.ThrowingBone;
			//NewArms();
			//RealNPC.dontTakeDamage = true;
		}
		#endregion
		#region LifeDown
		public override void LifeDown()
		{
			base.LifeDown();
			RealNPC.aiStyle = None;
			unsafe
			{
				floats[0] = PI / 2;
				WhereToGo = FromPolar(floats[0], 16 * 25);
				FakeVelocity += (Vector)(TargetPlayer.Center + WhereToGo - Center).ToLenOf(1);
			}
			//NewArms();
			//RealNPC.dontTakeDamage = true;
		}
		#endregion
		#region RealAI
		public override void RealAI()
		{
			#region Common
			GoToWhereToGo();
			if (Starver.IsPE && Timer % 31 == 0)
			{
				StartWall(Target);
			}
			#endregion
			#region Modes
			switch(Mode)
			{
				#region WaitForMode
				case BossMode.WaitForMode:
					SelectMode();
					break;
				#endregion
				#region ThrowingBone
				case BossMode.ThrowingBone:
					if (modetime > 60 * 10)
					{
						ResetMode();
					}
					if (Timer % 3 == 0)
					{
						ThrowingBone();
					}
					break;
				#endregion
				#region SummonFollows
				case BossMode.SummonFollows:
					if(modetime > 60 * 6)
					{
						ResetMode();
					}
					if(Timer % 66 == 0)
					{
						SummonFollows();
					}
					break;
				#endregion
				#region LineBone
				case BossMode.LineBone:
					if (modetime > 60 * 10)
					{
						ResetMode();
					}
					if(Timer % 4 == 0)
					{
						BoneLine();
					}
					break;
				#endregion
				#region BonePullBack
				case BossMode.BonePullBack:
					if(modetime > 60 * 8)
					{
						ResetMode();
					}
					if(Timer % 60 * 4 == 0)
					{
						ProjSector(TargetPlayer.Center, -16 * 28 / 60 / 4, 16 * 28, Rand.NextAngle(), PI * 2 / 3, 115, ProjectileID.SkeletonBone, 14);
					}
					break;
					#endregion
			}
			#endregion
		}
		#endregion
		#region AIs
		#region BoneLine
		private unsafe void BoneLine()
		{
			if(floats[1] < 44)
			{
				Proj(vector, Vector2.Zero, ProjectileID.SkeletonBone, 117, 10f);
				vector -= Vel;
				floats[1]++;
			}
			else
			{
				vector = (Vector)(TargetPlayer.Center + Rand.NextVector2(16 * 10));
				Vel = (Vector)(vector - TargetPlayer.Center).ToLenOf(16 * 3);
				floats[1] = 0;
			}
		}
		#endregion
		#region SummonFollows
		private new void SummonFollows()
		{
			int alive = AlivePlayers();
			int idx = NPC.NewNPC((int)Center.X, (int)Center.Y, NPCID.RaggedCaster, Index);
			Main.npc[idx].life = Main.npc[idx].lifeMax = alive * 6000;
			Main.npc[idx].defense = 1800;
			Main.npc[idx].velocity = Rand.NextVector2(26.66f);
			NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, null, idx);
			idx = NPC.NewNPC((int)Center.X, (int)Center.Y, NPCID.Paladin, Index);
			Main.npc[idx].life = Main.npc[idx].lifeMax = alive * 16000;
			Main.npc[idx].defense = 7800;
			Main.npc[idx].velocity = Rand.NextVector2(26.66f);
			NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, null, idx);
		}
		#endregion
		#region ThrowingBone
		private void ThrowingBone()
		{
			vector = (Vector)(TargetPlayer.Center - Center);
			vector.Length = 10;
			Proj(Center, vector, ProjectileID.SkeletonBone,124);
		}
		#endregion
		#region SelectMode
		private unsafe void SelectMode()
		{
			switch(lastMode)
			{
				#region ThrowingBone
				case BossMode.ThrowingBone:
					Mode = BossMode.SummonFollows;
					break;
				#endregion
				#region SummonFollows
				case BossMode.SummonFollows:
					Mode = BossMode.LineBone;
					floats[1] = 44;
					break;
				#endregion
				#region LineBone
				case BossMode.LineBone:
					Mode = BossMode.BonePullBack;
					break;
				#endregion
				#region BonePullBack
				case BossMode.BonePullBack:
					Mode = BossMode.ThrowingBone;
					break;
					#endregion
			}
			modetime = 0;
		}
		#endregion
		#region GoToWhereToGo
		private unsafe void GoToWhereToGo()
		{
			if (Vector2.Distance(TargetPlayer.Center + WhereToGo, Center) < 16 * 4)
			{
				while (Vector2.Distance(TargetPlayer.Center + WhereToGo, Center) < 16 * 4)
				{
					floats[0] += 2 * PI / 3;
					WhereToGo = FromPolar(floats[0], 16 * 25);

					FakeVelocity /= 2;
				}
			}
			else
			{
				FakeVelocity += (Vector)(TargetPlayer.Center + WhereToGo - Center).ToLenOf(1);
			}
		}
		#endregion
		#region CheckArms
		private bool CheckArms()
		{
			bool flag = false;
			for (int i = 0; i < MaxArms; i++)
			{
				flag |= Main.npc[ArmsLower[i]].active && Main.npc[ArmsLower[i]].type == NPCID.SkeletronHand;
				flag |= Main.npc[ArmsUpper[i]].active && Main.npc[ArmsUpper[i]].type == NPCID.SkeletronHand;
			}
			return flag;
		}
		#endregion
		#region NewArms
		private void NewArms()
		{
			for (int i = 0; i < MaxArms; i++)
			{
				#region Left
				ArmsLower[i] = NewNPC((Vector)Center, Vector.Zero, NPCID.SkeletronHand, 30000, 650);
				Main.npc[ArmsLower[i]].target = Target;
				Main.npc[ArmsLower[i]].ai[0] = -1;
				Main.npc[ArmsLower[i]].ai[1] = Index;
				Main.npc[ArmsLower[i]].ai[2] = -222;
				ArmsUpper[i] = NewNPC((Vector)Center, Vector.Zero, NPCID.SkeletronHand, 30000, 650);
				Main.npc[ArmsUpper[i]].target = Target;
				Main.npc[ArmsUpper[i]].ai[0] = -1;
				Main.npc[ArmsUpper[i]].ai[1] = Index;
				Main.npc[ArmsUpper[i]].ai[2] = -222;
				#endregion
				#region Right
				i++;
				ArmsLower[i] = NewNPC((Vector)Center, Vector.Zero, NPCID.SkeletronHand, 30000, 650);
				Main.npc[ArmsLower[i]].target = Target;
				Main.npc[ArmsLower[i]].ai[0] = -1;
				Main.npc[ArmsLower[i]].ai[1] = Index;
				Main.npc[ArmsLower[i]].ai[2] = -222;
				Main.npc[ArmsLower[i]].ai[3] = 150f;
				ArmsUpper[i] = NewNPC((Vector)Center, Vector.Zero, NPCID.SkeletronHand, 30000, 650);
				Main.npc[ArmsUpper[i]].target = Target;
				Main.npc[ArmsUpper[i]].ai[0] = -1;
				Main.npc[ArmsUpper[i]].ai[1] = Index;
				Main.npc[ArmsUpper[i]].ai[2] = -222;
				Main.npc[ArmsUpper[i]].ai[3] = 150f;
				#endregion
			}
		}
		#endregion
		#endregion
	}
}
