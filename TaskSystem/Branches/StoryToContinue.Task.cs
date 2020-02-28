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
	using TShockAPI;

	public partial class StoryToContinue
	{
		private class Task : BranchTask
		{
			private const float task2playerspeed = 5;
			private const float task2more = 3.625f;
			/// <summary>
			/// Main.dungeonX〈 Main.maxTilesX / 2 ? 1 : -1
			/// </summary>
			private int direction;
			private string[] startMsgs;
			private short msgInterval;
			private short msgCurrent;
			private int process;
			private int count;
			private int countRequire;
			private List<int> enemies;
			private List<int> specialEnemies;
			private Data16 datas;
			private Vector2 startPos;
			private Vector2 targetPos;
			private Vector2 vector;
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
							name = "轩辕爱做的事";
							countRequire = 200;
							startMsgs = new[]
							{
								"该死的...",
								"这是个空包裹",
								"看来他们已经摧毁了里面的物品",
								"上次的事情还没找他们算账",
								"正好这次一起算了",
								"给, 拿着这把", 
								"什么?",
								"你说你想要轩辕枪?",
								"因为射速快?",
								"那玩意儿不好用, 容易卡壳",
								"所以你还是用我给你的这把枪吧",
								"替我好好收拾下他们",
								"[c/000fff:你需要击落200架直升机]"
							};
							break;
						}
					case 3:
						{
							name = "配置药水(1)";
							countRequire = 5;
							startMsgs = new[]
							{
								"为了在最短时间内恢复",
								"我需要调制一些药水",
								"我要你帮我收集几样材料",
								"第一种材料的名称叫做\"    \"",
								"它生长在地狱的岩浆之中",
								"但是由于它的某些性质",
								"你一次只能携带一株",
								"替我采集5株回来"
							};
							break;
						}
					case 4:
						{
							name = "配置药水(2)";
							startMsgs = new[]
							{
								"配置这药水所需要的第二种材料叫做\"   \"",
								"它只出现在满月的凌晨,当2点以后它就会消失",
								"而且只有当采集到一朵后, 才会出现第二朵",
								"每颗植株的出现地点都相距很远",
								"所以你要快"
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
								"[c/000fff:你不会以为这种雕虫小技就能困住我吧?]",
								"[c/008800:你消耗了2/3的血量与全部的MP破除了阵式]",
								"[c/000fff:我倒要看看, 你有没有自己说的那样厉害]",
								"你不会以为这就能破了我的阵势吧?",
								"你不过是把自己困在了一个更大的阵势里",
								"这次, 你没有机会了"
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
				direction = Main.dungeonX < Main.maxTilesX / 2 ? 1 : -1;
				switch (ID)
				{
					case 0:
						{
							specialEnemies = new List<int>();
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
					case 2:
						{
							specialEnemies = new List<int>();
							targetPos = startPos;
							vector = new Vector2(0, -5);
							datas.ByteValue1 = (byte)Starver.Rand.Next(2);
							datas.ByteValue0 = datas.ByteValue1 switch
							{
								0 => 4,
								1 => 10,
								_ => throw null
							};
							startMsgs[5] += datas.ByteValue1 switch
							{
								0 => "霰弹枪",
								1 => "狙击步枪",
								_ => throw null
							};
							break;
						}
					case 3:
						{
							datas.ShortValue6 = 5 * 60 * 60;
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
									escape.EscapeX(targetPos.X, speed: 5);
									process++;
								}
								else if (process == 1)
								{
									specialEnemies.RemoveAll(idx => !Main.npc[idx].active);
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
										if (specialEnemies.Count < 2)
										{
											while (specialEnemies.Count < 4)
											{
												SpawnEnemyAttacker();
											}
										}
									}
									if (!targetHeli.Active)
									{
										if (targetHeli.Escaped || Vector2.Distance(vector, TargetPlayer.Center) > 40 * 16)
										{
											TargetPlayer.SendFailMessage("目标已逃跑");
											End(false);
											return;
										}
										End(true);
										return;
									}
									vector = targetHeli.Center;
								}
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
											Starvers.Utils.SendCombatMsg(TargetPlayer.Center + distance.ToLenOf(16 * 30), tip, Color.GreenYellow);
										}
										else
										{
											process++;
											Starvers.Utils.SendCombatMsg(targetPos, "就在这里", Color.GreenYellow);
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
										Starvers.Utils.SendCombatMsg(TargetPlayer.Center + distance.ToLenOf(16 * 30), tip, Color.GreenYellow);
									}
									else
									{
										End(true);
									}
								}
								break;
							}
						case 2:
							{
								if (Timer % 3 == 0)
								{
									TargetPlayer.Velocity = vector;
									TargetPlayer.Center = targetPos;
								}
								if (!TargetPlayer.HasBuff(BuffID.Invisibility))
								{
									TargetPlayer.SetBuff(BuffID.Invisibility);
								}
								if (!TargetPlayer.HasBuff(BuffID.ShadowDodge))
								{
									TargetPlayer.SetBuff(BuffID.ShadowDodge);
								}
								if (datas.IntValue1 > 0)
								{
									datas.IntValue1--;
								}
								targetPos += vector;
								if (process == 0)
								{
									if (targetPos.Y <= 250 * 16)
									{
										vector.X = -vector.Y;
										vector.Y = 0;
										vector.X *= direction;
										TargetPlayer.SendInfoMessage("已到达指定高度, 准备开始射击");
										process++;
									}
								}
								else if (process == 1)
								{
									specialEnemies.RemoveAll(idx => !Main.npc[idx].active);
									if (Timer % 180 == 0)
									{
										if (specialEnemies.Count < 6)
										{
											int rand = Starver.Rand.Next(6, 12) - specialEnemies.Count;
											for (int i = 0; i < rand; i++)
											{
												SpawnEnemyTarget();
											}
										}
									}
									if (count < countRequire && Math.Min(targetPos.X, Main.maxTilesX * 16 - targetPos.X) < 16 * 30)
									{
										TargetPlayer.SendFailMessage("你失败了...");
										TargetPlayer.SendDeBugMessage($"击杀数: {count}");
										End(false);
										return;
									}
									if (count == countRequire)
									{
										TargetPlayer.SendSuccessMessage("目标已达成");
										End(true);
									}
								}
								break;
							}
						case 3:
							{
								if (!datas[3])
								{
									if (TargetPlayer.ZoneHell && TargetPlayer.InLiquid(1))
									{
										var len = TargetPlayer.Velocity.Length();
										if (len >= 9)
										{
											if (!datas[0])
											{
												TargetPlayer.SendMessage("要采集这种材料, 必须静止在岩浆中", 255, 177, 255);
												datas[0] = true;
											}
										}
										else if (len.InRange(4, 9))
										{
											if (datas.ShortValue7 == 0)
											{
												if (Timer % 120 == 0)
												{
													if (!datas[1])
													{
														TargetPlayer.SendMessage("慢点...再慢点", 255, 125, 255);
														datas[1] = true;
													}
												}
												datas.ShortValue7 -= 4;
											}
											else
											{
												if (Timer % 120 == 0)
												{
													if (!datas[2])
													{
														TargetPlayer.SendMessage("不要动, 这会导致采集进度丢失", 0, 255, 0);
														datas[2] = true;
													}
												}
												datas.ShortValue7 -= 2;
											}
										}
										else
										{
											if (datas.ShortValue7 >= datas.ShortValue6)
											{
												datas[3] = true;
												TargetPlayer.SendInfoMessage("已收集到第" + (count + 1) + "株");
												TargetPlayer.SendInfoMessage("快把它带回去");
											}
											else
											{
												datas.ShortValue7 += 5;
												if (Timer % 40 == 0)
												{
													double process = 40 * 5;
													process /= datas.ShortValue6;
													process *= 100;
													TargetPlayer.SendCombatMsg("+" + (int)process + "%", Color.Blue);
												}
											}
										}
										if (Timer % 60 == 0)
										{
											double process = datas.ShortValue7;
											process /= datas.ShortValue6;
											process *= 100;
											TargetPlayer.AppendixMsg = $"采集进度: {(int)process}%";
										}
									}
									else
									{
										datas[1] = false;
										datas[2] = false;
										datas.ShortValue7 = 0;
									}
								}
								else if (Timer % 90 == 0)
								{
									var distance = startPos - TargetPlayer.Center;
									var len = distance.Length();
									if (len > 10 * 16)
									{
										string tip = $"还有{len / 16}块方格距离";
										Starvers.Utils.SendCombatMsg(TargetPlayer.Center + distance.ToLenOf(16 * 30), tip, Color.GreenYellow);
									}
									else
									{
										count++;
										if (count < countRequire)
										{
											TargetPlayer.SendInfoMessage($"{count}/{countRequire} 株材料已采集");
											TargetPlayer.SendInfoMessage($"还差{countRequire - count}株");
											datas[3] = false;
										}
										else
										{
											End(true);
										}
									}
								}
								break;
							}
						case 7:
							{
								if (process == 0)
								{
									TargetPlayer.Life = TargetPlayer.Life * 2 / 3;
									TargetPlayer.Mana = TargetPlayer.Mana * 2 / 3;
									TargetPlayer.mp = TargetPlayer.mp * 2 / 3;
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
					case 2:
						{
							if (Starver.NPCs[args.NPC.whoAmI] is ElfHeliEx heli && heli.ExpandDatas[0])
							{
								args.Handled = true;
								heli.ExpandDatas.ByteValue1 += (byte)(datas.ByteValue0 * (args.Crit ? 2 : 1));
								if (heli.ExpandDatas.ByteValue1 > 9)
								{
									heli.Life = 0;
									heli.SendCombatMsg($"击杀数: {++count} / {countRequire}", Color.MediumVioletRed);
									heli.CheckDead();
									heli.KillMe();
								}
								else
								{
									heli.SendCombatMsg($"{9 - heli.ExpandDatas.ByteValue1} / {9}", Color.Yellow);
								}
							}
							break;
						}
				}
			}

			public override void CreatingProj(GetDataHandlers.NewProjectileEventArgs args)
			{
				base.CreatingProj(args);
				switch(ID)
				{
					case 2:
						{
							args.Handled = true;
							bool flag =
								args.Type != ProjectileID.BulletHighVelocity &&
								args.Type != ProjectileID.MoonlordBullet;
							if (flag)
							{
								Main.projectile[args.Index].KillMeEx();
								if (datas.IntValue1 == 0) 
								{
									if (datas.ByteValue1 == 0) // 霰弹
									{
										TargetPlayer.ProjSector
										(
											Center: TargetPlayer.Center,
											speed: args.Velocity.Length(),
											r: 0,
											interrad: args.Velocity.Angle(),
											rad: Math.PI / 3,
											Damage: 0,
											type: ProjectileID.MoonlordBullet,
											num: 9
										);
										datas.IntValue1 = 40; // 发射间隔
									}
									else if (datas.ByteValue1 == 1) // 狙击
									{
										TargetPlayer.NewProj(targetPos, args.Velocity, ProjectileID.ChlorophyteBullet, 1);
										TargetPlayer.NewProj(targetPos, args.Velocity * 4, ProjectileID.ChlorophyteBullet, 1);
										datas.IntValue1 = 90;
									}
#if false
									else if(datas.ByteValue0 == 2) // 步枪
									{
										TargetPlayer.NewProj(targetPos, args.Velocity, ProjectileID.MeteorShot, 0);
										if (++datas.IntValue2 == 60)
										{
											TargetPlayer.SendInfoMessage("重新装填弹药...");
											datas.IntValue3 = 60 * 6;
											datas.IntValue2 = 0;
										}
										else
										{
											datas.IntValue1 = 4;
										}
									}
									else if (datas.ByteValue0 == 3) // 这是冲锋枪
									{
										TargetPlayer.NewProj(targetPos, args.Velocity, ProjectileID.MeteorShot, 0);
										if (++datas.IntValue2 == 30)
										{
											TargetPlayer.SendInfoMessage("重新装填弹药...");
											datas.IntValue3 = 60 * 2;
											datas.IntValue2 = 0;
										}
										else
										{
											datas.IntValue1 = 1;
										}
									}
#endif
								}
							}
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
					case 2:
					case 3:
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
					case 4:
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
							RewardLevelMultiply(1.2);
							if (TargetPlayer.ManaMax < 270)
							{
								TargetPlayer.ManaMax = 270;
								TargetPlayer.SendInfoMessage("获得奖励: 魔力上限提升至270");
							}
							break;
						}
					case 1:
						{
							RewardLevelMultiply(1.2);
							if (TargetPlayer.ManaMax < 280)
							{
								TargetPlayer.ManaMax = 280;
								TargetPlayer.SendInfoMessage("获得奖励: 魔力上限提升至280");
							}
							break;
						}
					case 2:
						{
							RewardLevelMultiply(1.2);
							TargetPlayer.GiveItem(AuraSystem.StarverAuraManager.SkillSlot[4].Item);
							TargetPlayer.GiveItem(ItemID.PaladinsShield);
							TargetPlayer.GiveItem(ItemID.FlyingKnife);
							break;
						}
					case 3:
						{
							RewardLevelMultiply(1.2);
							if (TargetPlayer.ManaMax < 290)
							{
								TargetPlayer.ManaMax = 290;
								TargetPlayer.SendInfoMessage("获得奖励: 魔力上限提升至290");
							}
							TargetPlayer.GiveItem(ItemID.MoonStone);
							TargetPlayer.GiveItem(ItemID.NeptunesShell);
							break;
						}
					case 4:
						{
							RewardLevelMultiply(1.2);
							if (TargetPlayer.ManaMax < 300)
							{
								TargetPlayer.ManaMax = 300;
								TargetPlayer.SendInfoMessage("获得奖励: 魔力上限提升至300");
							}
							break;
						}
					case 5:
						{
							RewardLevelMultiply(1.2);
							if (TargetPlayer.ManaMax < 310)
							{
								TargetPlayer.ManaMax = 310;
								TargetPlayer.SendInfoMessage("获得奖励: 魔力上限提升至310");
							}
							break;
						}
					case 6:
						{
							RewardLevelMultiply(1.2);
							break;
						}
					case 7:
						{
							RewardLevel(2000);
							RewardLevelMultiply(2);
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
				heli.WonderAttack(heli.Center, new Vector2(Starver.Rand.NextDirection() * Starver.Rand.Next(9, 13), 0), 60 * 5, false);
				specialEnemies?.Add(heli.Index);
			}
			private void SpawnEnemyTarget()
			{
				var rand = Starver.Rand;
				int dir = rand.NextDirection();
				float speed = vector.X + dir * (task2more + rand.NextFloat());
				Vector2 from = new Vector2(-dir * 70 * 16, 6 * 16 * rand.NextDirection() * rand.Next(1, 4));
				float endX;
				{
					float time = (70 * 16 + Math.Abs(from.X)) / Math.Abs(speed - vector.X);
					float distance = time * speed;
					endX = from.X + distance;
				}
				Vector2 to = new Vector2(endX, from.Y);
				var heli = NewEnemy(targetPos + from);
				heli.Escape(targetPos + to, dir * speed);
				heli.ExpandDatas[0] = true;
				specialEnemies.Add(heli.Index);
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
