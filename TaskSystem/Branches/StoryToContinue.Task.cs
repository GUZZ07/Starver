using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
namespace Starvers.TaskSystem.Branches
{
	using Vector = TOFOUT.Terraria.Server.Vector2;
	using NPCSystem.NPCs;
	using Events;
	using Tiles;

	public partial class StoryToContinue
	{
		private class Task : BranchTask
		{
			private string[] startMsgs;
			private short msgInterval;
			private short msgCurrent;
			private int process;
			private int count;
			private int countRequire;
			private List<int> enemies;
			private List<int> trackingEnemies;
			private Vector2 startPos;
			private Vector2 targetPos;
			private Vector2 alsopos;
			private Action ActionsInEnd;
			private ElfHeliEx targetHeli;

			public int? ID { get; }
			public override BLID BLID => BLID.StoryToContinue;

			public Task(int? id, StarverPlayer player = null) : base(player)
			{
				ID = id;
			}

			public void SetDefault()
			{
				msgInterval = 60 * 3 / 2;
				switch (ID)
				{
					case 0:
						{
							name = "清理眼线";
							startMsgs = new[]
							{
								"感谢你把我护送到这里",
								"我不希望我的他们发现我在这里",
								"沿途上的守卫已经被你干掉了",
								"但是还有一些家伙躲在了角落里",
								"别让他们逃走了",
							};
							break;
						}
					case 1:
						{
							name = "取回包裹";
							startMsgs = new[]
							{
								"由于离开的太突然",
								"我有一样重要物品忘记带上了",
								"请你替我把它带回来",
								"但要小心, 他们一定会猜到你会回去的"
							};
							break;
						}
					case 2:
						{
							name = "GaeBolg";
							startMsgs = new[]
							{
								"该死的...",
								"这是个空包裹",
								"看来他们已经摧毁了里面的物品",
								"上次的事情还没找他们算账",
								"正好这次一起算了",
								"给, 拿着它", // GaeBolg
								"替我好好收拾下他们"
							};
							break;
						}
					case 3:
						{
							name = "    ";
							startMsgs = new[]
							{
								"为了在最短时间内恢复",
								"我需要调制一些药水",
								"我要你帮我收集几样材料",
								"第一种材料的名称叫做\"    \"",
								"它只出现在满月的凌晨,当2点以后它就会消失",
								"而且只有当采集到一朵后, 才会出现第二朵",
								"每颗植株的出现地点都相距很远",
								"所以你要快"
							};
							break;
						}
					case 4:
						{
							name = "    ";
							startMsgs = new[]
							{
								"配置这药水所需要的第二种材料叫做\"   \"",
								"它生长在地狱的岩浆之中",
								"但是由于它的某些性质",
								"你一次只能携带一株",
								"替我采集4株回来"
							};
							break;
						}
					case 5:
						{
							name = "和谈";
							startMsgs = new[]
							{
								"它们突然对我说, 想跟我想和",
								"并且送来了我所需要的第三种材料",
								"但我觉得事情没那么简单",
								"我要你替我去见一见他们",
								"看看他们葫芦里卖的是什么药"
							};
							break;
						}
					case 6:
						{
							name = "    ";
							startMsgs = new[]
							{
								"[c/000fff:这里没有人]",
								"[c/000fff:你发现水晶球里有一些文字]",
								"我已经恢复的差不多了",
								"不出我所料的话",
								"那是个圈套",
								"我要让他们付出代价",
								"这是他们的3个头目",
								"解决掉他们",
								"在这之后, 到第四个地方与我接应"
							};
							break;
						}
					case 7:
						{
							name = "陷阱";
							startMsgs = new[]
							{
								"[c/000fff:(你的脚下突然出现了一个法阵,你无法移动了)]",
								"哈哈哈",
								"你上当了",
								"从一开始我就是来灭掉你的",
								"但没想到你居然一次次的打败了我的部下",
								"我不得不亲自出马对付你",
								"这次, 你没有机会了",
								"[c/000fff:你不会以为这种雕虫小技就能困住我吧?]",
								"[c/008800:你消耗了2/3的血量与全部的MP破除了阵式]",
								"[c/000fff:我倒要看看, 你有没有自己说的那样厉害]",
							};
							break;
						}
				}
			}

			public override void Start()
			{
				base.Start();
				enemies = new List<int>();
				startPos = TargetPlayer.Center;
				switch (ID)
				{
					case 0:
						{
							trackingEnemies = new List<int>();
							int x = (Main.dungeonX < Main.maxTilesX / 2) switch
							{
								false => 0,
								true => Main.maxTilesX
							};
							targetPos = new Vector2(x * 16, Main.maxTilesY * 16 / 10);
							break;
						}
					case 1:
						{
							var (endpoint, section, data) = GTemple.Run();
							section.UpdateToPlayer();
							targetPos = endpoint;
							ActionsInEnd += () =>
							{
								section.Restore(data);
								section.UpdateToPlayer();
							};
							break;
						}
				}
			}

			public override void Updating(int Timer)
			{
				base.Updating(Timer);
				if (msgCurrent < startMsgs.Length)
				{
					if (Timer % msgInterval == 0)
					{
						TargetPlayer.SendMessage(startMsgs[msgCurrent++], new Color(255, 233, 233));
					}
					switch(ID)
					{
						case 7:
							{
								TargetPlayer.Velocity = default;
								TargetPlayer.Center = startPos;
								break;
							}
					}
				}
				else
				{
					enemies.RemoveAll(idx => Starver.NPCs[idx]?.Active != true);
					switch (ID)
					{
						case 0:
							{
								if (process == 0)
								{
									ref ElfHeliEx escape = ref targetHeli;
									escape = NewEnemy(startPos + new Vector2(0, -16 * 5));
									escape.RealNPC.boss = true;
									escape.Defense = -1101;
									escape.Life = escape.LifeMax = 15000;
									escape.Escape(targetPos);
									process++;
								}
								else if (process == 1)
								{
									trackingEnemies.RemoveAll(idx => !Main.npc[idx].active);
									Vector2 total = targetPos - startPos;
									Vector2 valid = total;
									Vector2 ofPlayer = TargetPlayer.Center - startPos;
									valid.Normalize();
									valid *= Vector2.Dot(valid, ofPlayer);
									if (valid.Length() / (total.Length() / countRequire) > count)
									{
										count++;
										TargetPlayer.SendDeBugMessage("AMbusher spawned");
										SpawnEnemyAmbusher();
									}
									if (Timer % 210 == 0)
									{
										if (trackingEnemies.Count < 5)
										{
											while (trackingEnemies.Count < 5)
											{
												SpawnEnemyAttacker();
											}
											int rand = Starver.Rand.Next(5);
											for (int i = 0; i < rand; i++)
											{
												SpawnEnemyAttacker();
											}
										}
									}
								}
								if (!targetHeli.Active)
								{
									if (targetHeli.Escaped || Vector2.Distance(alsopos, TargetPlayer.Center) > 40 * 16)
									{
										TargetPlayer.SendFailMessage("目标已逃跑");
										End(false);
									}
									End(true);
									return;
								}
								alsopos = targetHeli.Center;
								break;
							}
						case 1:
							{
								if (process == 0)
								{
									if (msgCurrent >= startMsgs.Length && Timer % 90 == 0)
									{
										var distance = targetPos - TargetPlayer.Center;
										var len = distance.Length();
										if (len > 5 * 16)
										{
											string tip = $"还有{len / 16}块方格距离";
											Utils.SendCombatMsg(TargetPlayer.Center + distance.ToLenOf(16 * 30), tip, Color.GreenYellow);
										}
										else
										{
											process++;
											Utils.SendCombatMsg(targetPos, "就在这里", Color.GreenYellow);
											TargetPlayer.SendMessage("(你取走了这里的物品, 准备带回去)", 0, 0, 255);
										}
									}
								}
								else if (process == 1 && Timer % 90 == 0)
								{
									var distance = startPos - TargetPlayer.Center;
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
								break;
							}
						case 7:
							{
								if(process == 0)
								{
									TargetPlayer.Life /= 2;
									TargetPlayer.Mana /= 2;
									TargetPlayer.mp /= 2;
									process++;
								}
								break;
							}
						default:
							throw new InvalidOperationException("空任务!");
					}
				}
			}

			public override void StrikingNPC(NPCStrikeEventArgs args)
			{
				base.StrikingNPC(args);
				switch (ID)
				{
					case 0:
						{
							if (Starver.NPCs[args.NPC.whoAmI] == targetHeli)
							{
								args.RealDamage = Math.Min(args.RealDamage / 2, 50 + Starver.Rand.Next(-6, 6));
								if (args.Crit)
								{
									args.RealDamage *= 2;
								}
							}
							break;
						}
				}
			}

			public override void ReleasingSkill(ReleaseSkillEventArgs args)
			{
				base.ReleasingSkill(args);
				switch(ID)
				{
					case 2:
						{
							args.SkillID = AuraSystem.SkillIDs.GaeBolg;
							args.MPCost = 0;
							args.CD = 60 * 4;
							break;
						}
				}
			}

			protected override void End(bool success)
			{
				base.End(success);
				ActionsInEnd?.Invoke();
				ClearEnemies();
				if (success)
				{
					Success();
				}
				else
				{
					Fail();
				}
			}

			public (bool success, string msg) CanStartTask(StarverPlayer player)
			{
				switch (ID)
				{
					case 0:
					case 3:
					case 4:
					case 5:
					case 6:
					case 7:
						{
							Vector2 dungeon = new Vector2(Main.dungeonX * 16, Main.dungeonY * 16);
							if (Vector2.Distance(dungeon, player.Center) > 16 * 20)
							{
								return (false, "你必须在地牢入口开启这个任务");
							}
							return (true, null);
						}
					case 1:
						{
							if (Starver.Players.Count(ply => (ply?.BranchTask as Task)?.ID == 1) != 0)
							{
								return (false, "请先等待其他玩家完成本任务");
							}
							Vector2 dungeon = new Vector2(Main.dungeonX * 16, Main.dungeonY * 16);
							if (Vector2.Distance(dungeon, player.Center) > 16 * 20)
							{
								return (false, "你必须在地牢入口开启这个任务");
							}
							return (true, null);
						}
					case 2:
						{
							return (false, "这个任务被神秘力量封印了");
						}
					default:
						throw new InvalidOperationException("空任务");
				}
			}

			private void Success()
			{
				RewardPlayer();
			}

			private void Fail()
			{
				
			}

			private void RewardLevelMultiply(double factor)
			{
				RewardLevel((int)(TargetPlayer.Level * (factor - 1)));
			}

			private void RewardLevel(int lvl)
			{
				TargetPlayer.Level += lvl;
				TargetPlayer.SendInfoMessage("获得奖励: 等级提升" + lvl);
			}

			private void RewardPlayer()
			{
				switch (ID)
				{
					case 0:
						{
							RewardLevelMultiply(1.1);
							if (TargetPlayer.ManaMax < 270)
							{
								TargetPlayer.ManaMax = 270;
								TargetPlayer.SendInfoMessage("获得奖励: 魔力上限提升至270");
							}
							break;
						}
					case 1:
						{
							RewardLevelMultiply(1.1);
							if (TargetPlayer.ManaMax < 280)
							{
								TargetPlayer.ManaMax = 280;
								TargetPlayer.SendInfoMessage("获得奖励: 魔力上限提升至280");
							}
							break;
						}
					case 2:
						{
							RewardLevelMultiply(1.1);
							TargetPlayer.GiveItem(AuraSystem.StarverAuraManager.SkillSlot[2].Item);
							TargetPlayer.GiveItem(ItemID.CelestialMagnet);
							TargetPlayer.GiveItem(ItemID.ManaCrystal, 10);
							break;
						}
					case 3:
						{
							RewardLevelMultiply(1.1);
							if (TargetPlayer.ManaMax < 290)
							{
								TargetPlayer.ManaMax = 290;
								TargetPlayer.SendInfoMessage("获得奖励: 魔力上限提升至290");
							}
							break;
						}
					case 4:
						{
							RewardLevelMultiply(1.1);
							if (TargetPlayer.ManaMax < 300)
							{
								TargetPlayer.ManaMax = 300;
								TargetPlayer.SendInfoMessage("获得奖励: 魔力上限提升至300");
							}
							break;
						}
					case 5:
						{
							RewardLevelMultiply(1.1);
							if (TargetPlayer.ManaMax < 310)
							{
								TargetPlayer.ManaMax = 310;
								TargetPlayer.SendInfoMessage("获得奖励: 魔力上限提升至310");
							}
							break;
						}
					case 6:
						{
							RewardLevelMultiply(1.1);
							break;
						}
					case 7:
						{
							RewardLevelMultiply(1.1);
							if (TargetPlayer.ManaMax < 400)
							{
								TargetPlayer.ManaMax = 400;
								TargetPlayer.SendInfoMessage("获得奖励: 魔力上限提升至400");
							}
							TargetPlayer.bldata.Bonus |= EffectBonus.DamageDecrease1;
							TargetPlayer.SendInfoMessage("获得奖励: 受到的伤害降低5%");
							break;
						}
				}
			}

			public override string ToString()
			{
				return $"{nameof(StoryToContinue)}--{ID + 1}";
			}

			#region Utils
			private void ClearEnemies()
			{
				enemies.ForEach(idx => Main.npc[idx].active = false);
				enemies.Clear();
			}
			private void SpawnEnemyAttacker()
			{
				var heli = NewEnemy(TargetPlayer.Center + Starver.Rand.NextVector2(16 * 40, 16 * 40));
				trackingEnemies?.Add(heli.Index);
			}
			private void SpawnEnemyAmbusher()
			{

			}
			private ElfHeliEx NewEnemy(Vector2 where, Vector2 vel = default)
			{
				if (HeliRoot == null)
				{
					HeliRoot = NPCSystem.StarverNPC.RootNPCs.First(npc => npc is ElfHeliEx) as ElfHeliEx;
				}
				int idx = NPCSystem.StarverNPC.NewNPC((Vector)where, (Vector)vel, HeliRoot);
				var heli = (ElfHeliEx)NPCSystem.StarverNPC.NPCs[idx];
				heli.IgnoreDistance = true;
				heli.Defense /= 2;
				heli.DamageIndex = 3;
				enemies.Add(heli.Index);
				return heli;
			}
			private static ElfHeliEx HeliRoot;
			#endregion
			#region GTemple
			private static class GTemple
			{
				private static Random rand;
				public static (Vector2 endpoint, TileSection section, TileSectionData data) Run()
				{
					const int Paint = 0;
					rand = new Random();
					Point point = new Point(Main.maxTilesX - 200 - 100, 50);
					TileSection section = new TileSection(point.X - 200, point.Y, 200 + 200 + 40, 230);
					var data = section.SaveDatas();
					#region MainBuilding
					// PlaceTileAt(point.X, point.Y, TileID.LihzahrdBrick);
					// Main.tile[point.X, point.Y].color(Paint);
					// Main.tile[point.X, point.Y].wallColor(Paint);
					for (int i = 1; i < 200; i++)
					{
						PlaceTileAt(point.X, point.Y + i, TileID.LihzahrdBrick);
						PlaceWallAt(point.X, point.Y + i, WallID.LihzahrdBrickUnsafe);
						Main.tile[point.X, point.Y + i].color(Paint);
						Main.tile[point.X, point.Y + i].wallColor(Paint);
						for (int j = 0; j < i - 1; j++)
						{
							PlaceTileAt(point.X + j, point.Y + i, TileID.LihzahrdBrick);
							PlaceTileAt(point.X - j, point.Y + i, TileID.LihzahrdBrick);
							PlaceWallAt(point.X + j, point.Y + i, WallID.LihzahrdBrickUnsafe);
							PlaceWallAt(point.X - j, point.Y + i, WallID.LihzahrdBrickUnsafe);
							Main.tile[point.X + j, point.Y + i].color(Paint);
							Main.tile[point.X - j, point.Y + i].color(Paint);
							Main.tile[point.X + j, point.Y + i].wallColor(Paint);
							Main.tile[point.X - j, point.Y + i].wallColor(Paint);
						}
						PlaceTileAt(point.X + i - 1, point.Y + i, TileID.LihzahrdBrick);
						PlaceTileAt(point.X - i + 1, point.Y + i, TileID.LihzahrdBrick);
						Main.tile[point.X + i - 1, point.Y + i].color(Paint);
						Main.tile[point.X - i + 1, point.Y + i].color(Paint);
					}
					#endregion
					Point entry = point;
					entry.Y += 25;
					entry.X += 25;
					RemoveAll(entry.X, entry.Y);
					RemoveAll(entry.X, entry.Y + 1);
					RemoveAll(entry.X, entry.Y + 2);
					RemoveAll(entry.X + 1, entry.Y + 1);
					RemoveAll(entry.X + 1, entry.Y + 2);
					RemoveAll(entry.X + 2, entry.Y + 2);
					for (int i = 0; i < 25; i++)
					{
						RemoveTileAt(entry.X - i, entry.Y);
						RemoveTileAt(entry.X - i, entry.Y + 1);
						RemoveTileAt(entry.X - i, entry.Y + 2);
					}
					// PlaceLockedDoor(entry.X - 2, entry.Y);
					Point path = entry;
					path.X -= 25;
					path.Y += 3;
					{
						int n1 = rand.Next(2, 5);
						int n2 = rand.Next(2, 5);
						for (int i = 0; i < 30; i++)
						{
							RemoveTileAt(path.X, path.Y + i);
							for (int j = 0; j < n1; j++)
							{
								RemoveTileAt(path.X + j, path.Y + i);
							}
							for (int j = 0; j < n2; j++)
							{
								RemoveTileAt(path.X - j, path.Y + i);
							}
							if (rand.Next(100) > 95)
							{
								int last = n1;
								do
								{
									n1 = rand.Next(2) == 1 ? 1 : -1 + last;
									if (n1 < 2)
									{
										n1 = rand.Next(2, 5);
									}
								}
								while (n1 >= 5);
							}
							if (rand.Next(100) > 95)
							{
								int last = n2;
								do
								{
									n2 = rand.Next(2) == 1 ? 1 : -1 + last;
									if (n2 < 2)
									{
										n2 = rand.Next(2, 5);
									}
								}
								while (n2 >= 5);
							}
						}
					}
					{
						path.Y += 30;
						int n1 = rand.Next(2, 5);
						int n2 = rand.Next(2, 5);
						for (int i = 0; i < 25 + 20; i++)
						{
							RemoveTileAt(path.X + i, path.Y);
							for (int j = 0; j < n1; j++)
							{
								RemoveTileAt(path.X + i, path.Y + j);
							}
							for (int j = 0; j < n2; j++)
							{
								RemoveTileAt(path.X + i, path.Y - j);
							}
							if (rand.Next(100) > 80)
							{
								int last = n1;
								do
								{
									n1 = rand.Next(2) == 1 ? 1 : -1 + last;
									if (n1 < 2)
									{
										n1 = rand.Next(2, 5);
									}
								}
								while (n1 >= 5);
							}
							if (rand.Next(100) > 80)
							{
								int last = n2;
								do
								{
									n2 = rand.Next(2) == 1 ? 1 : -1 + last;
									if (n2 < 2)
									{
										n2 = rand.Next(2, 5);
									}
								}
								while (n2 >= 5);
							}
						}
						path.X += 25 + 20;
					}
					bool left = true;
					int t = 0;
					// excuter.SendInfoMessage($"start: {point.X}, {point.Y}");
					while (path.Y - point.Y < 200 - 30)
					{
						// excuter.SendInfoMessage($"t: {++t}");
						// excuter.SendInfoMessage($"lvl: {path.Y - point.Y}");
						// Down(ref path, rand.Next(40, 60));
						#region Down
						{
							int depth = rand.Next(6, 10);
							int n1 = rand.Next(2, 5);
							int n2 = rand.Next(2, 5);
							for (int i = 0; i < depth; i++)
							{
								RemoveTileAt(path.X, path.Y + i);
								for (int j = 0; j < n1; j++)
								{
									RemoveTileAt(path.X + j, path.Y + i);
								}
								for (int j = 0; j < n2; j++)
								{
									RemoveTileAt(path.X - j, path.Y + i);
								}
								if (rand.Next(100) > 95)
								{
									int last = n1;
									do
									{
										n1 = rand.Next(2) == 1 ? 1 : -1 + last;
										if (n1 < 2)
										{
											n1 = rand.Next(2, 5);
										}
									}
									while (n1 >= 5);
								}
								if (rand.Next(100) > 95)
								{
									int last = n2;
									do
									{
										n2 = rand.Next(2) == 1 ? 1 : -1 + last;
										if (n2 < 2)
										{
											n2 = rand.Next(2, 5);
										}
									}
									while (n2 >= 5);
								}
							}
							path.Y += depth;
						}
						#endregion
						if (left)
						{
							// excuter.SendInfoMessage($"t: {++t}");
							#region Left
							{
								int lvl = path.Y - point.Y;
								int distance = LevelTiles(lvl) - (point.X + lvl - 10 - path.X);
								int n1 = rand.Next(2, 5);
								int n2 = rand.Next(2, 5);
								for (int i = 0; i < distance; i++)
								{
									RemoveTileAt(path.X - i, path.Y);
									for (int j = 0; j < n1; j++)
									{
										RemoveTileAt(path.X - i, path.Y + j);
									}
									for (int j = 0; j < n2; j++)
									{
										RemoveTileAt(path.X - i, path.Y - j);
									}
									if (rand.Next(100) > 98)
									{
										path.Y += rand.Next(1, 3);
									}
									if (rand.Next(100) > 95)
									{
										int last = n1;
										do
										{
											n1 = rand.Next(2) == 1 ? 1 : -1 + last;
											if (n1 < 2)
											{
												n1 = rand.Next(2, 5);
											}
										}
										while (n1 >= 5);
									}
									if (rand.Next(100) > 95)
									{
										int last = n2;
										do
										{
											n2 = rand.Next(2) == 1 ? 1 : -1 + last;
											if (n2 < 2)
											{
												n2 = rand.Next(2, 5);
											}
										}
										while (n2 >= 5);
									}
								}
								path.X -= distance;
							}
							#endregion
							left = false;
						}
						else
						{
							// excuter.SendInfoMessage($"t: {++t}");
							#region Right
							{
								int lvl = path.Y - point.Y;
								int distance = LevelTiles(lvl) - (lvl - (point.X - path.X));
								int n1 = rand.Next(2, 5);
								int n2 = rand.Next(2, 5);
								for (int i = 0; i < distance; i++)
								{
									RemoveTileAt(path.X + i, path.Y);
									for (int j = 0; j < n1; j++)
									{
										RemoveTileAt(path.X + i, path.Y + j);
									}
									for (int j = 0; j < n2; j++)
									{
										RemoveTileAt(path.X + i, path.Y - j);
									}
									if (rand.Next(100) > 98)
									{
										path.Y += rand.Next(1, 3);
									}
									if (rand.Next(100) > 95)
									{
										int last = n1;
										do
										{
											n1 = rand.Next(2) == 1 ? 1 : -1 + last;
											if (n1 < 2)
											{
												n1 = rand.Next(2, 5);
											}
										}
										while (n1 >= 5);
									}
									if (rand.Next(100) > 95)
									{
										int last = n2;
										do
										{
											n2 = rand.Next(2) == 1 ? 1 : -1 + last;
											if (n2 < 2)
											{
												n2 = rand.Next(2, 5);
											}
										}
										while (n2 >= 5);
									}
								}
								path.X += distance;
							}
							#endregion
							left = true;
						}
					}
					rand = null;
					return (path.ToVector2() * 16, section, data);
				}
				private static int LevelTiles(int lvl)
				{
					return lvl * 2 + 1 - 10 - 10;
				}
				private static void RemoveAll(int x, int y)
				{
					RemoveTileAt(x, y);
					RemoveWallAt(x, y);
				}
				private static void RemoveWallAt(int x, int y)
				{
					Main.tile[x, y].wall = 0;
				}
				private static void RemoveTileAt(int x, int y)
				{
					Main.tile[x, y].active(false);
				}
				private static void PlaceTileAt(int x, int y, ushort type)
				{
					Main.tile[x, y].active(true);
					Main.tile[x, y].slope(0);
					Main.tile[x, y].type = type;
				}
				private static void PlaceWallAt(int x, int y, byte wall)
				{
					Main.tile[x, y].wall = wall;
				}
				private static void PlaceLockedDoor(int x, int y)
				{
					Main.tile[x, y].type = TileID.ClosedDoor;
					Main.tile[x, y].sTileHeader = 32;
					Main.tile[x, y].bTileHeader = 0;
					Main.tile[x, y].bTileHeader2 = 6;
					Main.tile[x, y].bTileHeader2 = 2;
					Main.tile[x, y].frameX = 36;
					Main.tile[x, y].frameY = 594;

					y += 1;
					Main.tile[x, y].type = TileID.ClosedDoor;
					Main.tile[x, y].sTileHeader = 32;
					Main.tile[x, y].bTileHeader = 0;
					Main.tile[x, y].bTileHeader2 = 66;
					Main.tile[x, y].bTileHeader2 = 1;
					Main.tile[x, y].frameX = 18;
					Main.tile[x, y].frameY = 612;

					y += 1;
					Main.tile[x, y].type = TileID.ClosedDoor;
					Main.tile[x, y].sTileHeader = 32;
					Main.tile[x, y].bTileHeader = 0;
					Main.tile[x, y].bTileHeader2 = 1;
					Main.tile[x, y].bTileHeader2 = 1;
					Main.tile[x, y].frameX = 0;
					Main.tile[x, y].frameY = 630;
				}
			}
			#endregion
		}
	}
}
