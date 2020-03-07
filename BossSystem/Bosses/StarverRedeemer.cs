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
	public class StarverRedeemer : StarverBoss
	{
		#region Fields
		protected List<Tuple<Vector2, int, int>> tupleList = new List<Tuple<Vector2, int, int>>();
		protected List<Vector2> vector2List = new List<Vector2>();
		protected List<int> SummonList = new List<int>();
		protected int[] param1 = new int[2] { -1, 1 };
		protected short inter;
		private DropItem[] DropsNormal = new DropItem[]
		{
			new DropItem(new int[]{ Currency.Minion }, 15, 30, 0.73f),
			new DropItem(new int[]{ Currency.Minion }, 15, 30, 0.73f),
			new DropItem(new int[]{ Currency.Minion }, 15, 30, 0.73f),
		};
		private DropItem[] DropsEx = new DropItem[]
		{
			new DropItem(new int[]{ Currency.Minion }, 30, 55),
			new DropItem(new int[]{ Currency.Minion }, 30, 55),
			new DropItem(new int[]{ Currency.Minion }, 30, 55),
		};
		#endregion
		#region Ctor
		public StarverRedeemer() : base(4)
		{
			CheckType = false;
			TaskNeed = 28;
			Name = "The Starver Redeemer";
			FullName = "Shtaed The Starver Redeemer";
			DefaultLife = 3000;
			DefaultLifes = 200;
			DefaultDefense = 0;
			vector.X = 16 * 39;
			vector.Y = 0;
			RawType = NPCID.DukeFishron;
		}
		#endregion
		#region Fail
		public override void OnFail()
		{
			base.OnFail();
			if (ExVersion && EndTrial && GetType() == typeof(StarverRedeemer))
			{
				StarverPlayer.All.SendMessage("都给我消停一下", Color.Pink);
				StarverPlayer.All.SendMessage("好好待着别动", Color.Pink);
				EndTrial = false;
				EndTrialProcess = 0;
			}
		}
		#endregion
		#region BeDown
		protected override void BeDown()
		{
			base.BeDown();
			if (EndTrial && ExVersion && GetType() == typeof(StarverRedeemer))
			{
				StarverPlayer.All.SendMessage("好吧,你们赢了", Color.HotPink);
				StarverPlayer.All.SendMessage("不过还有几个最难对付的在后边等着你们呢", Color.HotPink);
				StarverPlayer.All.SendMessage("我就待在这看着", Color.HotPink);
				EndTrialProcess++;
			}
		}
		#endregion
		#region Spawn
		public override void Spawn(Vector2 where, int lvl = 2000)
		{
			base.Spawn(where, lvl);
			inter = ExVersion ? (short)20 : (short)30;
			Mode = BossMode.DeathsTwinkle;
			RealNPC.type = NPCID.LunarTowerStardust;
			vector.Y = 0;
			vector.X = ExVersion ? 16 * 33 : 16 * 26;
			Drops = ExVersion ? DropsEx : DropsNormal;
		}
		protected void Spawn(Vector2 where, int lvl, double AngleStart, float radium = -1)
		{
			Spawn(where, lvl);
			if (radium > 0)
			{
				vector.Y = 0;
				vector.X = radium;
			}
			vector.Angle = AngleStart;
		}
		#endregion
		#region RealAI
		public override void RealAI()
		{
			switch (Mode)
			{
				#region SelectMode
				case BossMode.WaitForMode:
					switch (lastMode)
					{
						#region Twinkle
						case BossMode.DeathsTwinkle:
							Mode = BossMode.DeathsSummonFollows;
							break;
						#endregion
						#region SummonFollows
						case BossMode.DeathsSummonFollows:
							Mode = BossMode.DeathsFlowInvaderShot;
							inter = ExVersion ? (short)30 : (short)60;
							break;
						#endregion
						#region FlowInvaderShot
						case BossMode.DeathsFlowInvaderShot:
							Mode = BossMode.DeathsLaser;
							inter = ExVersion ? (short)7 : (short)10;
							break;
						#endregion
						#region Laser
						case BossMode.DeathsLaser:
							Mode = BossMode.DeathsTwinkle;
							inter = ExVersion ? (short)20 : (short)30;
							break;
							#endregion
					}
					modetime = 0u;
					LastCenter = Center;
					break;
				#endregion
				#region SummonFollows
				case BossMode.DeathsSummonFollows:
					if(modetime > 60 * 9)
					{
						ResetMode();
						break;
					}
					if(Timer % 25 == 0)
					{
						AddFollows();
					}
					break;
				#endregion
				#region Twinkle
				case BossMode.DeathsTwinkle:
					if (modetime > 60 * 9)
					{
						ResetMode();
						break;
					}
					Twinkle();
					break;
				#endregion
				#region FlowInvaderShot
				case BossMode.DeathsFlowInvaderShot:
					if (modetime > 60 * 16)
					{
						ResetMode();
						break;
					}
					FlowInvaderShot();
					break;
				#endregion
				#region Laser
				case BossMode.DeathsLaser:
					if (modetime > 60 * 9)
					{
						ResetMode();
						break;
					}
					Laser();
					break;
					#endregion
			}
			SummonFollows();
			if (Level > 3000)
			{
				vector.Angle += PI / 120;
			}
			else
			{
				vector.Angle += PI * 2 / 100;
			}
			if (ExVersion)
			{
				RealNPC.ai[0] = 11f;
				RealNPC.ai[1] = 0f;
				RealNPC.ai[2] = 0f;
				TargetPlayer.TPlayer.ZoneTowerStardust = true;
				if (Timer % 60 == 0)
				{
					TargetPlayer.SendData(PacketTypes.Zones, "", Target);
				}
			}
			Center = TargetPlayer.Center + vector;
		}
		#endregion
		#region ModeAIs
		#region AddFollows
		protected void AddFollows()
		{
			int add;
			switch (Rand.Next(5))
			{
				case 0:
					add = NPCID.StardustCellBig;
					break;
				case 1:
					add = NPCID.StardustJellyfishBig;
					break;
				case 2:
					add = NPCID.StardustWormHead;
					break;
				case 3:
					add = NPCID.StardustSoldier;
					break;
				case 4:
					add = NPCID.StardustSpiderBig;
					break;
				default:
					add = NPCID.StardustCellSmall;
					break;
			}
			SummonList.Add(add);
		}
		#endregion
		#region SummonFollows
		protected new void SummonFollows()
		{
			if (Timer % 45 == 0 && SummonList.Count > 0)
			{
				float[] ai = Rawai;
				{
					int num1 = SummonList.Next();
					ai[1] = 30 * Starver.Rand.Next(5, 16);
					int num2 = Starver.Rand.Next(3, 6);
					int num3 = Starver.Rand.Next(0, 4);
					int index1 = 0;
					tupleList.Add(item: Tuple.Create(RealNPC.Top - (Vector2.UnitY * 120f), num2, 0));
					int num4 = 0;
					while (tupleList.Count > 0)
					{
						Vector2  vector2_1 = tupleList[0].Item1;
						int num6 = 1;
						int num7 = 1;
						if (num4 > 0 && num3 > 0 && (Starver.Rand.Next(3) != 0 || num4 == 1))
						{
							num7 = Starver.Rand.Next(Math.Max(1, tupleList[0].Item2));
							++num6;
							--num3;
						}
						for (int index2 = 0; index2 < num6; ++index2)
						{
							int num8 = tupleList[0].Item3;
							if (num4 == 0)
								num8 = param1.Next();
							else if (index2 == 1)
								num8 *= -1;
							float num9 = (float)((num4 % 2 == 0 ? 0.0 : 3.14159274101257) + (0.5 - Starver.Rand.NextFloat()) * 0.785398185253143 + num8 * 0.785398185253143 * (num4 % 2 == 0).ToDirectionInt());
							float num10 = (float)(100.0 + 50.0 * Starver.Rand.NextFloat());
							int num11 = tupleList[0].Item2;
							if (index2 != 0)
								num11 = num7;
							if (num4 == 0)
							{
								num9 = (float)((0.5 - Starver.Rand.NextFloat()) * 0.785398185253143);
								num10 = (float)(100.0 + 100.0 * Starver.Rand.NextFloat());
							}
							Vector2  vector2_2 = (-Vector2.UnitY).RotatedBy(num9, new Vector2()) * num10;
							if (num11 - 1 < 0)
								vector2_2 = Vector2.Zero;
							index1 = Proj(vector2_1,  vector2_2, ProjectileID.StardustTowerMark, 180, 0.0f, -num4 * 10f, (float)(0.5 + Starver.Rand.NextFloat() * 0.5));
							if (ExVersion)
							{
								for (int i = 0; i < Starver.Players.Length; i++)
								{
									if (Starver.Players[i] is null)
									{
										continue;
									}
									if (i == Target || Main.player[i] == null || Starver.Players[i].Active)
									{
										continue;
									}
									index1 = Proj(Starver.Players[i].Center,  vector2_2, ProjectileID.StardustTowerMark, 180, 0.0f, -num4 * 10f, (float)(0.5 + Starver.Rand.NextFloat() * 0.5));
								}
							}
							else
							{
								index1 = Proj(vector2_1,  vector2_2, ProjectileID.StardustTowerMark, 180, 0.0f, -num4 * 10f, (float)(0.5 + Starver.Rand.NextFloat() * 0.5));
							}
							vector2List.Add(vector2_1 +  vector2_2);
							if (num4 < num2 && tupleList[0].Item2 > 0)
							{
								tupleList.Add(Tuple.Create(vector2_1 +  vector2_2, num11 - 1, num8));
							}
						}
						tupleList.Remove(tupleList[0]);
					}
					Main.projectile[index1].localAI[0] = num1;
					SummonList.Clear();
				}
			}

		}
		#endregion
		#region Twinkle
		protected unsafe void Twinkle()
		{
			if (Timer % inter == 0)
			{
				int follow;
				if (ExVersion)
				{
					foreach (var player in Starver.Players)
					{
						if(player is null)
						{
							continue;
						}
						Vel = (Vector)(player.Center +  vector);
						follow = NPC.NewNPC((int)Vel.X, (int)Vel.Y, NPCID.StardustSpiderSmall);
						NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, null, follow);
						Vel = (Vector)(player.Center +  vector);
						follow = NPC.NewNPC((int)Vel.X, (int)Vel.Y, NPCID.StardustSpiderSmall);
						NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, null, follow);
					}
				}
				else
				{
					Vel = (Vector)(TargetPlayer.Center +  vector);
					follow = NPC.NewNPC((int)Vel.X, (int)Vel.Y, NPCID.StardustSpiderSmall);
					NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, null, follow);
					follow = NPC.NewNPC((int)Vel.X, (int)Vel.Y, NPCID.StardustSpiderSmall);
					NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, null, follow);
				}
			}
		}
		#endregion
		#region FlowInvaderShot
		protected void FlowInvaderShot()
		{
			if (Timer % inter == 0)
			{
				if (ExVersion)
				{
					foreach (var player in Starver.Players)
					{
						if (player is null)
						{
							continue;
						}
						ProjSector(player.Center + RelativePos, 17, 3, RelativePos.Angle() + PI, PI * 2 / 3, 240, ProjectileID.StardustJellyfishSmall, 5);
					}
				}
				else
				{
					ProjSector(Center, 17, 3,  vector.Angle+ PI, PI * 2 / 3, 180, ProjectileID.StardustJellyfishSmall, 5);
				}
			}
		}
		#endregion
		#region Laser
		protected void Laser()
		{
			if (Timer % inter == 0)
			{
				if (ExVersion)
				{
					foreach (var player in Starver.Players)
					{
						if (player == null || !player.Active)
						{
							continue;
						}
						Vel = (Vector)Rand.NextVector2(16 * 29f);
						Proj(player.Center + Vel, -Vel / 16 / 29 * 3 + Rand.NextVector2(5f, 5f), ProjectileID.DeathLaser, 220, 9f);
					}
				}
				else
				{
					Vel = (Vector)Rand.NextVector2(16 * 29f);
					Proj(TargetPlayer.Center + Vel, -Vel / 16 / 29 * 1.75f + Rand.NextVector2(5f, 5f), ProjectileID.DeathLaser, 170, 9f);
				}
			}
		}
		#endregion
		#endregion
	}
}
