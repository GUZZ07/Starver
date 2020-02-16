using System;
using System.Linq;
using System.Collections.Generic;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;



namespace Starvers.TaskSystem.Branches
{
	using Events;
	using Vector = TOFOUT.Terraria.Server.Vector2;
	using AnalogItem = AuraSystem.Realms.AnalogItem;
	using ElfHeliEx = NPCSystem.NPCs.ElfHeliEx;
	public partial class YrtAEvah
	{
		private class Task : BranchTask
		{
			private const int ShardRegionRadium = 125;
			private const int BattleRegionRadium = 50;
			private const int EscapeRegionRadium = 10;

			private LinkedList<int> enemies;
			private string[] startMsgs;
			private short msgInterval;
			private short msgCurrent;
			private string[] words;
			private List<AnalogItem> Items;
			private short count;
			private short countRequire;
			private short targetID;
			private Vector2 startPosition;
			private Vector2 targetPosition;
			private short taskProcess;
			private short spawnedCount;
			private int TimeTicker;

			private static Random Rand => Starver.Rand;

			public int? ID { get; }
			public override BLID BLID => BLID.YrtAEvah;

			public Task(int? id, StarverPlayer player = null) : base(player)
			{
				ID = id;
			}

			public void SetDefault()
			{
				msgInterval = 60 * 3 / 2;
				switch(ID)
				{
					case 0:
						{
							name = "证明自己";
							startMsgs = new[]
							{
								"你需要先向我证明你自己的实力",
								"先去干掉5只蓝色史莱姆",
								"然后再来找我"
							};
							targetID = NPCID.BlueSlime;
							countRequire = 5;
							break;
						}
					case 1:
						{
							name = "睡个好觉";
							startMsgs = new[]
							{
								"这几天晚上总是有几只爱斯基摩僵尸来打扰我睡觉",
								"你去替我好好收拾下它们",
								"[c/008800:你需要消灭10只爱斯基摩僵尸]"
							};
							targetID = NPCID.ZombieEskimo;
							countRequire = 10;
							break;
						}
					case 2:
						{
							name = "看星星";
							startMsgs = new[]
							{
								"你知道, 对于一个法师来说, ⭐是必需品",
								"⭐非常有用, 它可以用来制作魔力药水, 魔能药水, 魔力腰带...",
								"今晚的天气很晴朗, 一定会有很多落星",
								"帮我收集尽可能多星星",
								"[c/008800:至少收集45科星星]"
							};
							targetID = ItemID.FallenStar;
							countRequire = 45;
							break;
						}
					case 3:
						{
							name = "美妙碎片";
							startMsgs = new[]
							{
								"看看这个漂浮在空中的碎片",
								"很美丽, 对吧?",
								"更重要的是, 它们蕴涵着巨大的能量",
								"今晚它们还会在一个地点大量出现",
								"在其他人捷足先登之前替我把它们带回来"
							};
							targetID = ItemID.FragmentStardust;
							countRequire = 120;
							break;
						}
					case 4:
						{
							name = "追踪者";
							startMsgs = new[]
							{
								"感谢你替我收集这些碎片",
								"但它们只是个诱饵",
								"为了引我现身...",
								"不过我还是靠这些诱饵定位到了他们",
								"解决他们, 把真正的碎片带回来"
							};
							break;
						}
					case 5:
						{
							name = "寸步不离";
							startMsgs = new[]
							{
								"可恶...",
								"碎片已经被他们动过手脚了",
								"他们施加在碎片上的东西使我失去了力量",
								"但这只是暂时的",
								"他们不会给我恢复的机会, 一定会来攻击这里",
								"我需要你护送我去\"那个地方\"",
								"那里能让我在最短时间内恢复"
							};
							break;
						}
					default:
						throw new InvalidOperationException("空任务");
				}
			}

			public override void Start()
			{
				base.Start();
				switch(ID)
				{
					case 3:
						{
							Items = new List<AnalogItem>();
							targetPosition = FindPos1();
							TimeTicker = (int)(Vector2.Distance(targetPosition, TargetPlayer.Center) / TargetPlayer.TPlayer.maxRunSpeed);
							TimeTicker += 60 * 20;
#if DEBUG
							TargetPlayer.SendDeBugMessage($"targetPos: {targetPosition}");
							TargetPlayer.SendDeBugMessage($"myPos: {TargetPlayer.Center}");
#endif
							break;
						}
					case 4:
						{
							targetPosition = FindPos2();
#if DEBUG
							TargetPlayer.SendDeBugMessage($"targetPos: {targetPosition}");
							TargetPlayer.SendDeBugMessage($"myPos: {TargetPlayer.Center}");
#endif
							break;
						}
					case 5:
						{
							targetPosition = FindPos4();
#if DEBUG
							TargetPlayer.SendDeBugMessage($"targetPos: {targetPosition}");
							TargetPlayer.SendDeBugMessage($"myPos: {TargetPlayer.Center}");
#endif
							enemies = new LinkedList<int>();
							countRequire = 5;
							break;
						}
				}
				startPosition = TargetPlayer.Center;
			}

			public (bool success, string msg) CanStartTask(StarverPlayer player)
			{
				switch (ID)
				{
					case 0:
					case 3:
					case 4:
					case 5:
						{
							return (true, null);
						}
					case 1:
					case 2:
						{
							if (Main.dayTime)
							{
								return (false, "你只能在晚上接受这个任务");
							}
							return (true, null);
						}
					default:
						throw new InvalidOperationException("空任务");
				}
			}

			public override void Updating(int Timer)
			{
				base.Updating(Timer);
				if (msgCurrent < startMsgs.Length && Timer % msgInterval == 0)
				{
					TargetPlayer.SendMessage(startMsgs[msgCurrent++], new Color(255, 233, 233));
				}
				switch (ID)
				{
					case 1:
						{
							if (Main.dayTime)
							{
								End(false);
							}
							break;
						}
					case 2:
						{
							if (Main.dayTime)
							{
								if (count >= countRequire)
								{
									End(true);
								}
								else
								{
									End(false);
								}
							}
							else
							{
								if (Timer % 30 == 0)
								{
									if (Rand.Next(100) > 80)
									{
										Vector2 where = TargetPlayer.Center;
										where.Y -= 16 * 50;
										where.X += Rand.Next(-16 * 90, 16 * 80);
										var item = new AnalogItem(ItemID.FallenStar);
										item.Center = where;
										item.TimeLeft = 60 * 15;
										Starver.Instance.Aura.AddRealm(item);
									}
								}
							}
							break;
						}
					case 3:
						{
							if (TimeTicker > 0)
							{
								if (TimeTicker-- == 1)
								{
									TargetPlayer.SendInfoMessage("碎片出现了!");
								}
								if (TimeTicker % 60 == 0)
								{
									TargetPlayer.AppendixMsg = $"碎片将在{TimeTicker / 60}s后出现";
								}
							}
							if (taskProcess == 0)
							{
								if(TimeTicker == 0)
								{
									TargetPlayer.SendFailMessage("太慢了, 太慢了");
									End(false);
									return;
								}
								if (msgCurrent >= startMsgs.Length && Timer % 90 == 0)
								{
									var distance = targetPosition - TargetPlayer.Center;
									var len = distance.Length();
									if (len > ShardRegionRadium * 16)
									{
										string tip = $"还有{len / 16}块方格距离";
										Utils.SendCombatMsg(TargetPlayer.Center + distance.ToLenOf(16 * 30), tip, Color.GreenYellow);
									}
									else
									{
										taskProcess++;
										TargetPlayer.SetBuff(BuffID.UFOMount, 60 * 60 * 60);
										Utils.SendCombatMsg(targetPosition - distance.ToLenOf(ShardRegionRadium * 8), "就在这里", Color.GreenYellow);
									}
								}
							}
							else if (taskProcess == 1)
							{
								TargetPlayer.SetBuff(BuffID.UFOMount);
								if (TimeTicker == 0 && Timer % 40 == 0)
								{
									if (spawnedCount < countRequire * 3 / 2)
									{
										float r = Rand.NextFloat();
										float length = r * ShardRegionRadium * 16;
										var item = new AnalogItem(ItemID.FragmentStardust);
										item.Center = targetPosition + Rand.NextVector2(length);
										Items.Add(item);
										Starver.Instance.Aura.AddRealm(item);
										/*
										int idx = Utils.NewItem(targetPosition + Rand.NextVector2(length), ItemID.FragmentStardust);
										Main.item[idx].keepTime = int.MaxValue;
										*/
										spawnedCount++;
									}
								}
							}
							else if (taskProcess == 2 && Timer % 90 == 0)
							{
								var distance = startPosition - TargetPlayer.Center;
								var len = distance.Length();
								if (len > 10 * 16)
								{
									string tip = $"还有{len / 16}块方格距离";
									Utils.SendCombatMsg(TargetPlayer.Center + distance.ToLenOf(16 * 30), tip, Color.GreenYellow);
								}
								else
								{
									End(true);
								}
							}
							if (taskProcess < 2 && Timer % 150 == 0)
							{
								const int limit = ShardRegionRadium * 2 * 3 / 4;
								for (int i = 0; i < limit; i++)
								{
									double angle = Math.PI * 2 * i / limit;
									int idx = Utils.NewProj(targetPosition + Utils.FromPolar(angle, ShardRegionRadium * 16), Vector2.Zero, ProjectileID.VortexVortexPortal);
									Main.projectile[idx].timeLeft = 150;
									Main.projectile[idx].aiStyle = -1;
								}
							}
							break;
						}
					case 4:
						{
							if (taskProcess == 0)
							{
								if (msgCurrent >= startMsgs.Length && Timer % 90 == 0)
								{
									var distance = targetPosition - TargetPlayer.Center;
									var len = distance.Length();
									if (len > BattleRegionRadium * 16)
									{
										string tip = $"还有{len / 16}块方格距离";
										Utils.SendCombatMsg(TargetPlayer.Center + distance.ToLenOf(16 * 30), tip, Color.GreenYellow);
									}
									else
									{
										words = new[]
										{
											"奇怪",
											"碎片哪去了",
											"",
											"!",
											"果然",
											"有埋伏",
											"来吧...",
											"[c/008800:消灭在场所有敌人]"
										};
										taskProcess++;
									}
								}
							}
							else if (taskProcess == 1)
							{
								if (msgCurrent - startMsgs.Length < words.Length)
								{
									if (Timer % msgInterval == 0)
									{
										TargetPlayer.SendMessage(words[msgCurrent++ - startMsgs.Length], new Color(255, 233, 233));
									}
								}
								else
								{
									Task_4_SpawnEnemies();
									taskProcess++;
								}
							}
							else if (taskProcess == 2)
							{
								if (enemies.Count == 0)
								{
									TargetPlayer.SendInfoMessage("带上碎片回去");
									taskProcess++;
									break;
								}
								unsafe
								{
									var t = 0;
									var span = stackalloc int[enemies.Count];
									foreach (var idx in enemies)
									{
										if (!Main.npc[idx].active)
										{
											span[t++] = idx;
										}
									}
									for (int i = 0; i < t; i++)
									{
										enemies.Remove(span[i]);
									}
								}
							}
							else if(taskProcess == 3 && Timer % 90 == 0)
							{
								var distance = startPosition - TargetPlayer.Center;
								var len = distance.Length();
								if (len > 10 * 16)
								{
									string tip = $"还有{len / 16}块方格距离";
									Utils.SendCombatMsg(TargetPlayer.Center + distance.ToLenOf(16 * 30), tip, Color.GreenYellow);
								}
								else
								{
									End(true);
								}
							}
							if (taskProcess < 2 && Timer % 150 == 0)
							{
								const int limit = BattleRegionRadium * 2 * 3 / 4;
								for (int i = 0; i < limit; i++)
								{
									double angle = Math.PI * 2 * i / limit;
									int idx = Utils.NewProj(targetPosition + Utils.FromPolar(angle, BattleRegionRadium * 16), Vector2.Zero, ProjectileID.VortexVortexPortal);
									Main.projectile[idx].timeLeft = 150;
									Main.projectile[idx].aiStyle = -1;
								}
							}
							break;
						}
					case 5:
						{
							if (msgCurrent >= startMsgs.Length)
							{
								Vector2 total = targetPosition - startPosition;
								Vector2 valid = total;
								Vector2 ofPlayer = TargetPlayer.Center - startPosition;
								valid.Normalize();
								valid *= Vector2.Dot(valid, ofPlayer);
								if (valid.Length() / (total.Length() / countRequire) > count)
								{
									count++;
									TargetPlayer.SendDeBugMessage("AMbusher spawned");
									Task_5_SpawnEnemyAmbusher();
								}
								if (Timer % 150 == 0)
								{
									const int limit = EscapeRegionRadium * 2 * 3 / 4;
									for (int i = 0; i < limit; i++)
									{
										double angle = Math.PI * 2 * i / limit;
										int idx = Utils.NewProj(targetPosition + Utils.FromPolar(angle, EscapeRegionRadium * 16), Vector2.Zero, ProjectileID.Flames);
										Main.projectile[idx].timeLeft = 150;
										Main.projectile[idx].aiStyle = -1;
									}
								}
								if (Timer % 210 == 0)
								{
									unsafe
									{
										var t = 0;
										var span = stackalloc int[enemies.Count];
										foreach (var idx in enemies)
										{
											if (!Main.npc[idx].active)
											{
												span[t++] = idx;
											}
										}
										for (int i = 0; i < t; i++)
										{
											enemies.Remove(span[i]);
										}
									}
									if (enemies.Count < 5)
									{
										while (enemies.Count < 5)
										{
											Task_5_SpawnEnemyAttacker();
										}
										int rand = Rand.Next(5);
										for (int i = 0; i < rand; i++)
										{
											Task_5_SpawnEnemyAttacker();
										}
									}
								}
								if (Timer % 90 == 0)
								{
									var distance = targetPosition - TargetPlayer.Center;
									var len = distance.Length();
									if (len > EscapeRegionRadium * 16)
									{
										string tip = $"还有{len / 16}块方格距离";
										Utils.SendCombatMsg(TargetPlayer.Center + distance.ToLenOf(16 * 30), tip, Color.GreenYellow);
									}
									else
									{
										ClearEnemies();
										End(true);
									}
								}
							}
							break;
						}
				}
			}

			public override void StrikedNPC(NPCStrikeEventArgs args)
			{
				base.StrikedNPC(args);
				switch (ID)
				{
					case 0:
					case 1:
						{
							if (!args.NPC.active)
							{
								if (args.NPC.type == targetID)
								{
									count++;
									TargetPlayer.SendCombatMSsg($"猎杀数: {count} / {countRequire}", Color.Maroon);
									if (count >= countRequire)
									{
										End(true);
									}
								}
							}
							else
							{
								if (args.NPC.life > 200 && !Main.hardMode)
								{
									args.NPC.life = 200;
								}
							}
							break;
						}
					case 4:
						{
							if (taskProcess == 2 && !args.NPC.active && IsHeli(args.NPC))
							{
								var idx = args.NPC.whoAmI;
								if (enemies.Contains(idx))
								{
									enemies.Remove(idx);
									count++;
									if (count - countRequire > 0)
									{
										TargetPlayer.SendCombatMSsg($"还剩{countRequire - count}个敌人", Color.Maroon);
									}
								}
							}
							break;
						}
				}
			}

			public override void OnPickAnalogItem(AnalogItem item)
			{
				base.OnPickAnalogItem(item);
				switch (ID)
				{
					case 2:
						{
							if (item.ID == ItemID.FallenStar)
							{
								count++;
								item.Kill();
								TargetPlayer.SendCombatMSsg($"{count}颗落星已收集", Color.Green);
								if (count == countRequire)
								{
									TargetPlayer.SendMessage($"目标已达成", Color.YellowGreen);
								}
								else if (count < countRequire)
								{
									TargetPlayer.SendMessage($"还需{countRequire - count}颗", Color.Green);
								}
							}
							break;
						}
					case 3:
						{
							if (taskProcess == 1)
							{
								if (item.ID == ItemID.FragmentStardust)
								{
									count++;
									item.Kill();
									TargetPlayer.SendCombatMSsg($"已收集碎片: {count} / {countRequire}", Color.LimeGreen);
									if (count >= countRequire)
									{
										TargetPlayer.RemoveBuff(BuffID.UFOMount);
										TargetPlayer.SendCombatMSsg("收集完成", Color.MintCream);
										TargetPlayer.SendInfoMessage("快把碎片带回去");
										ClearStardustShards();
										taskProcess++;
									}
								}
							}
							break;
						}
				}
			}

			protected override void End(bool success)
			{
				base.End(success);
				if (success)
				{
					Success();
				}
				else
				{
					Fail();
				}
			}

			private void Fail()
			{
				switch (ID)
				{
					case 1:
						{
							TargetPlayer.SendFailMessage("天亮了, 僵尸已经逃走了");
							break;
						}
					case 2:
						{
							TargetPlayer.SendFailMessage("太少了, 太少了");
							break;
						}
					case 3:
						{
							ClearStardustShards();
							break;
						}
				}
			}

			private void Success()
			{
				RewardPlayer();
			}

			private void RewardLevel(int lvl)
			{
				TargetPlayer.Level += lvl;
				TargetPlayer.SendInfoMessage("获得奖励: 等级提升" + lvl);
			}

			private void RewardPlayer()
			{
				switch(ID)
				{
					case 0:
						{
							RewardLevel(20);
							if (TargetPlayer.LifeMax < 420)
							{
								TargetPlayer.LifeMax = 420;
								TargetPlayer.SendInfoMessage("获得奖励: 生命上限提升至420");
							}
							break;
						}
					case 1:
						{
							RewardLevel(80);
							if (TargetPlayer.LifeMax < 440)
							{
								TargetPlayer.LifeMax = 440;
								TargetPlayer.SendInfoMessage("获得奖励: 生命上限提升至440");
							}
							break;
						}
					case 2:
						{
							RewardLevel(130);
							TargetPlayer.GiveItem(AuraSystem.StarverAuraManager.SkillSlot[2].Item);
							TargetPlayer.GiveItem(ItemID.CelestialMagnet);
							TargetPlayer.GiveItem(ItemID.ManaCrystal, 10);
							break;
						}
					case 3:
						{
							RewardLevel(150);
							if(TargetPlayer.ManaMax < 220)
							{
								TargetPlayer.ManaMax = 220;
								TargetPlayer.SendInfoMessage("获得奖励: 魔力上限提升至220");
							}
							break;
						}
					case 4:
						{
							RewardLevel(270);
							if (TargetPlayer.LifeMax < 460)
							{
								TargetPlayer.LifeMax = 460;
								TargetPlayer.SendInfoMessage("获得奖励: 生命上限提升至460");
							}
							if (TargetPlayer.ManaMax < 260)
							{
								TargetPlayer.ManaMax = 260;
								TargetPlayer.SendInfoMessage("获得奖励: 魔力上限提升至260");
							}
							break;
						}
					case 5:
						{
							if(TargetPlayer.Level < 1200)
							{
								TargetPlayer.Level = 1200;
								TargetPlayer.SendInfoMessage("获得奖励: 等级提升至1200");
							}
							else
							{
								RewardLevel(500);
							}
							if (TargetPlayer.LifeMax < 500)
							{
								TargetPlayer.LifeMax = 500;
								TargetPlayer.SendInfoMessage("获得奖励: 生命上限提升至500");
							}
							break;
						}
				}
			}

			public override string ToString()
			{
				return "YrtAEvah--" + ID ?? "??";
			}

			#region Utils
			private void ClearStardustShards()
			{
				for (int i = 0; i < Items.Count; i++)
				{
					var item = Items[i];
					if (Vector2.Distance(item.Center, targetPosition) < ShardRegionRadium * 16 + 32)
					{
						item.Kill();
					}
				}
				Items.Clear();
#if false
				Starver.Instance.Aura.OperateRealms(realm =>
				{
					if (realm is AnalogItem item)
					{
						if (Vector2.Distance(item.Center, targetPosition) < ShardRegionRadium * 16 + 32)
						{
							item.Kill();
						}
					}
				});
#endif
			}
			private void ClearEnemies()
			{
				foreach (var idx in enemies)
				{
					Main.npc[idx].active = false;
				}
			}

			private Vector2 FindPos1()
			{
				const int radium = ShardRegionRadium;
				Vector2 result = default;
				int X = (int)(TargetPlayer.Center.X / 16);
				int Y = (int)(TargetPlayer.Center.Y / 16);
#if DEBUG
				TargetPlayer.SendDeBugMessage($"{{X: {X}, Y: {Y}}}");
#endif
				int signX = 1;
				if (Rand.Next(2) == 1)
				{
					signX = -1;
				}
				int t = 0;
				while (radium + 200 < X && X < Main.maxTilesX - radium - 200)
				{
					X += signX * Rand.Next(100, 200);
					if (t++ > 10 && Rand.Next(100) > 80)
					{
						break;
					}
				}
#if DEBUG
				TargetPlayer.SendDeBugMessage($" t: {t}");
				TargetPlayer.SendDeBugMessage($" X: {X}");
#endif
				t = 0;
				int signY = 1;
				if (Rand.Next(2) == 1)
				{
					signY = -1;
				}
				while (radium * 2 + 20 < Y && Y < Main.maxTilesY - 2 * radium - 20)
				{
					Y += signY * Rand.Next(10, 30);
					if (t++ > 4 && Rand.Next(100) > 40)
					{
						break;
					}
				}
#if DEBUG
				TargetPlayer.SendDeBugMessage($" t: {t}");
				TargetPlayer.SendDeBugMessage($" Y: {Y}");
#endif
				#region Local Check
				static bool CheckPoint(int X, int Y)
				{
					static bool CheckTile(int X, int Y)
					{
						var tile = Main.tile[X, Y];
						System.Diagnostics.Debug.Assert(tile != null, "tile is null!");
						return !tile.active();
					}
					try
					{
						for (int i = 0; i < radium; i++)
							for (int j = 0; j < radium; j++)
							{
								if (i * i + j * j < radium * radium)
								{
									bool flag =
										CheckTile(X + j, Y + i) &&
										CheckTile(X + j, Y - i) &&
										CheckTile(X - j, Y + i) &&
										CheckTile(X - j, Y - i);
									if (!flag)
									{
										return false;
									}
								}
							}
					}
					catch (IndexOutOfRangeException)
					{
						return false;
					}
					return true;
				}
				#endregion
				for (int i = 0; i < radium * 2; i++)
				{
					for (int j = 0; j < radium * 2; j++)
					{
						if (CheckPoint(X, Y))
						{
							goto Out;
						}
						X += 1 * signX;
					}
					X -= radium * 2 * signX;
					if (X + signX * radium * 2 <= radium || Main.maxTilesX - radium <= X + signX * radium * 2)
					{
						signX *= -1;
					}
					if (Y + signY * radium * 2 <= radium * 2 || Main.maxTilesY - radium * 2 <= X + signY * radium * 2)
					{
						signY *= -1;
					}
					Y += 1 * signY;
				}
				Out:
#if DEBUG
				TargetPlayer.SendDeBugMessage($"{{X: {X}, Y: {Y}}}");
#endif
				result = new Vector2(X * 16, Y * 16);
				return result;
			}
			private Vector2 FindPos2()
			{
				Vector2 result = TargetPlayer.Center + Vector.FromPolar(Rand.NextAngle() / 12 - Math.PI / 6, Rand.Next(16 * 400, 16 * 900));
				if (Rand.Next(2) == 1)
				{
					result *= -1;
				}

				if (result.X < 0)
				{
					result.X *= -1;
				}
				if (result.X < BattleRegionRadium * 16 * 2)
				{
					result.X = BattleRegionRadium * 16 * 2;
				}
				else if (result.X >= Main.maxTilesX * 16 - BattleRegionRadium * 16 * 2)
				{
					result.X = Main.maxTilesX * 16 - BattleRegionRadium * 16 * 2;
				}

				if (result.Y < 0)
				{
					result.Y *= -1;
				}
				if (result.Y < BattleRegionRadium * 16 * 2)
				{
					result.Y = BattleRegionRadium * 16 * 2;
				}
				else if (result.Y >= Main.maxTilesY * 16 - BattleRegionRadium * 16 * 2)
				{
					result.Y = Main.maxTilesY * 16 - BattleRegionRadium * 16 * 2;
				}

				return result;
			}
			private Vector2 FindPos3(Vector2 pos2)
			{
				Vector2 result = 2 * TargetPlayer.Center - pos2;
				if (Rand.Next(2) == 1)
				{
					result *= -1;
				}

				if (result.X < ShardRegionRadium * 16 * 2)
				{
					result.X = ShardRegionRadium * 16 * 2;
				}
				else if (result.X >= Main.maxTilesX * 16 - ShardRegionRadium * 16 * 2)
				{
					result.X = Main.maxTilesX * 16 - ShardRegionRadium * 16 * 2;
				}

				if (result.Y < ShardRegionRadium * 16 * 2)
				{
					result.Y = ShardRegionRadium * 16 * 2;
				}
				else if (result.Y >= Main.maxTilesY * 16 - ShardRegionRadium * 16 * 2)
				{
					result.Y = Main.maxTilesY * 16 - ShardRegionRadium * 16 * 2;
				}

				return result;
			}
			private Vector2 FindPos4()
			{
				return new Vector2(Main.dungeonX * 16 + 8, Main.dungeonY * 16);
			}

			private void Task_4_SpawnEnemies()
			{
				enemies = new LinkedList<int>();
				Vector2 escapeTo = targetPosition;
				escapeTo.X = Main.maxTilesX * 16 - escapeTo.X;
				ElfHeliEx heli;
				for (int i = 0; i < 3; i++)
				{
					heli = NewEnemy(targetPosition + Rand.NextVector2(16 * 20, 16 * 20), default);
					heli.RealNPC.boss = true;
					heli.Escape(escapeTo);
				}
				for (int i = 0; i < 4; i++)
				{
					heli = NewEnemy(TargetPlayer.Center + Rand.NextVector2(16 * BattleRegionRadium + 16 * 4), default);
					heli.RealNPC.boss = true;
					heli.Attack(TargetPlayer);
					enemies.AddLast(heli.Index);
				}
				countRequire += 4;

				Vector2 offSet;

				offSet = new Vector2(BattleRegionRadium * 2 / 3 * 16, BattleRegionRadium * 3 / 5 * 16);

				heli = NewEnemy(targetPosition - offSet, default);
				heli.RealNPC.boss = true;
				heli.SetShot(1);
				heli.Wonder(heli.Center, offSet.Vertical().ToLenOf(BattleRegionRadium * 16 * 2 / 5));
				enemies.AddLast(heli.Index);

				heli = NewEnemy(targetPosition + offSet, default);
				heli.RealNPC.boss = true;
				heli.SetShot(1);
				heli.Wonder(heli.Center, offSet.Vertical().ToLenOf(BattleRegionRadium * 16 * 2 / 5));
				enemies.AddLast(heli.Index);

				offSet.Y *= -1;

				heli = NewEnemy(targetPosition - offSet, default);
				heli.RealNPC.boss = true;
				heli.SetShot(1);
				heli.Wonder(heli.Center, offSet.Vertical().ToLenOf(BattleRegionRadium * 16 * 2 / 5));
				enemies.AddLast(heli.Index);

				heli = NewEnemy(targetPosition + offSet, default);
				heli.RealNPC.boss = true;
				heli.SetShot(1);
				heli.Wonder(heli.Center, offSet.Vertical().ToLenOf(BattleRegionRadium * 16 * 2 / 5));
				enemies.AddLast(heli.Index);

				countRequire += 4;
			}
			private void Task_5_SpawnEnemyAmbusher()
			{
				/*
				Vector2 u = targetPosition - startPosition;
				Vector2 ofPlayer = TargetPlayer.Center - startPosition;
				u.Normalize();
				u *= Vector2.Dot(u, ofPlayer) / ofPlayer.Length();
				*/
				Vector2 u = TargetPlayer.Center;
				ElfHeliEx heli;

				heli = NewEnemy(u + new Vector2(16 * 40, 16 * 30));
				heli.SetShot(1);
				heli.Wonder(new Vector2(0, 16 * 3), 16 * 50);

				heli = NewEnemy(u + new Vector2(16 * 40, -16 * 30));
				heli.SetShot(1);
				heli.Wonder(new Vector2(0, -16 * 3), 16 * 50);

				heli = NewEnemy(u + new Vector2(-16 * 40, 16 * 30));
				heli.SetShot(1);
				heli.Wonder(new Vector2(0, 16 * 3), 16 * 50);

				heli = NewEnemy(u + new Vector2(-16 * 40, -16 * 30));
				heli.SetShot(1);
				heli.Wonder(new Vector2(0, -16 * 3), 16 * 50);
			}
			private void Task_5_SpawnEnemyAttacker()
			{
				Vector2 u = TargetPlayer.Center + Rand.NextVector2(16 * 60);
				var heli = NewEnemy(u);
				heli.Attack(TargetPlayer);
				int shot = Rand.Next(2 * ElfHeliEx.MaxShots) switch
				{
					0 => 0,
					1 => 2,
					2 => 2,
					3 => 3,
					4 => 4,
					5 => 5,
					6 => 6,
					7 => 7,
					_ => 2
				};
				heli.SetShot(shot);
				enemies.AddLast(heli.Index);
			}
			private static bool IsHeli(NPC npc)
			{
				bool result = NPCSystem.StarverNPC.NPCs[npc.whoAmI] as ElfHeliEx != null;
				return result;
			}
			private static ElfHeliEx NewEnemy(Vector2 where, Vector2 vel = default)
			{
				if(HeliRoot == null)
				{
					HeliRoot = NPCSystem.StarverNPC.RootNPCs.Where(npc => npc is ElfHeliEx).First() as ElfHeliEx;
				}
				int idx = NPCSystem.StarverNPC.NewNPC((Vector)where, (Vector)vel, HeliRoot);
				var heli = (ElfHeliEx)NPCSystem.StarverNPC.NPCs[idx];
				heli.IgnoreDistance = true;
				return heli;
			}
			private static ElfHeliEx HeliRoot;
			#endregion
		}
	}
}
