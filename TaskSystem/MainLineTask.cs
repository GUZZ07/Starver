using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace Starvers.TaskSystem
{
	using BossSystem;
	using CheckDelegate = Func<StarverPlayer, bool>;
	using StarverBoss = BossSystem.Bosses.Base.StarverBoss;
	using ItemLists = Dictionary<TaskDifficulty, TaskItem[]>;
	using SuperID = TaskItem.SuperID;
	public class MainLineTask : ITask
	{
		#region Fields
		protected ItemLists NeedEx;
		protected ItemLists RewardEx;
		protected string Name;
		protected string Story;
		#endregion
		#region Properties
		public static StarverBoss[] Bosses => StarverBossManager.Bosses;
		public bool IsFinished => StarverConfig.Config.TaskNow >= ID;
		public StarverBoss Boss { get; protected set; }
		public int ID { get; protected set; }
		public TaskDifficulty Difficulty { get; protected set; }
		public string Description { get; protected set; }
		public TaskItem[] Needs { get; protected set; }
		public TaskItem[] Rewards { get; protected set; }
		public int Mark { get; protected set; } = ItemID.MagicMirror;
		public int Level { get; protected set; }
		public bool Normal { get; protected set; } = true;
		public bool CheckLevel { get; protected set; }
		public bool CheckItem { get; protected set; } = true;
		#endregion
		#region Statics
		public readonly static TaskItem[] DefaultReward = new TaskItem[] { (ItemID.PlatinumCoin) };
		public readonly static TaskItem[] DefaultNeed = new TaskItem[] { (ItemID.Gel) };
		#endregion
		#region CCtor
		static MainLineTask()
		{
			var two = 2;
			if (WorldGen.SavedOreTiers.Cobalt == -1)
			{
				WorldGen.SavedOreTiers.Cobalt = 107;
				if (Starver.Rand.Next(two) == 0)
				{
					WorldGen.SavedOreTiers.Cobalt = 221;
				}
			}
			if (WorldGen.SavedOreTiers.Mythril == -1)
			{
				WorldGen.SavedOreTiers.Mythril = 108;
				if (Starver.Rand.Next(two) == 0)
				{
					WorldGen.SavedOreTiers.Mythril = 222;
				}
			}
			if (WorldGen.SavedOreTiers.Adamantite == -1)
			{
				WorldGen.SavedOreTiers.Adamantite = 111;
				if (Starver.Rand.Next(two) == 0)
				{
					WorldGen.SavedOreTiers.Adamantite = 223;
				}
			}
		}
		#endregion
		#region Ctor
		protected MainLineTask() { }
		public MainLineTask(int id, TaskDifficulty difficulty, MainLineTaskData data)
		{
			ID = id;
			Difficulty = difficulty;
			Level = data.LevelReward;
			NeedEx = data.Needs;
			RewardEx = data.Rewards;
			Name = data.Name;
			Story = data.Story;
			SetDefault();
		}
		public MainLineTask(int id, TaskDifficulty difficulty = TaskDifficulty.Hard)
		{
			ID = id;
			Level = 10 * ID;
			Difficulty = difficulty;
			switch (ID)
			{
				#region KingSlime
				case 1:
					Name = "粘黏凝胶";
					Story = "美味又易燃";
					NeedEx = new ItemLists
					{
						[TaskDifficulty.Easy] = new TaskItem[] { (ItemID.Gel, 20) },
						[TaskDifficulty.Normal] = new TaskItem[] { (ItemID.Gel, 40) },
						[TaskDifficulty.Hard] = new TaskItem[] { (ItemID.Gel, 60) },
						[TaskDifficulty.Hell] = new TaskItem[] { (ItemID.Gel, 99) },
					};
					Rewards = new TaskItem[]
					{
						(ItemID.SlimeCrown ,5)
					};
					break;
				#endregion
				#region Eye
				case 2:
					{
						Name = "凝视";
						Story = "希望站在我们和克苏鲁之眼间的不是弱小的你";

						var Easy = new TaskItem[]
						{
							(ItemID.Lens, 10)
						};
						var Normal = new TaskItem[]
						{
							(ItemID.Lens, 20),
							(ItemID.DemonEyeBanner, 1)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.Lens, 40),
							(ItemID.DemonEyeBanner, 3)
						};
						var Hell = new TaskItem[]
						{
							(ItemID.Lens, 80),
							(ItemID.DemonEyeBanner, 6)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Normal,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell
						};
						Rewards = new TaskItem[]
						{
							(ItemID.SuspiciousLookingEye,5)
						};
						break;
					}
				#endregion
				#region Evil
				case 3:
					{
						SuperID material;
						SuperID mushroom;
						SuperID summon;
						if (WorldGen.crimson)
						{
							Name = "猩红之手";
							Story = "血肉与脊骨";
							material = ItemID.Vertebrae;
							mushroom = ItemID.ViciousMushroom;
							summon = ItemID.BloodySpine;
						}
						else
						{
							Name = "幽暗裂缝";
							Story = "邪恶意识的结合体";
							material = ItemID.RottenChunk;
							mushroom = ItemID.VileMushroom;
							summon = ItemID.WormFood;
						}

						material.RuntimeBind = true;
						mushroom.RuntimeBind = true;
						summon.RuntimeBind = true;

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = new TaskItem[] { (material, 10), (mushroom, 10) },
							[TaskDifficulty.Normal] = new TaskItem[] { (material, 16), (mushroom, 12) },
							[TaskDifficulty.Hard] = new TaskItem[] { (material, 25), (mushroom, 15) },
							[TaskDifficulty.Hell] = new TaskItem[] { (material, 32), (mushroom, 20) }
						};
						Rewards = new TaskItem[]
						{
							(summon, 5)
						};
						break;
					}
				#endregion
				#region DD2 T1
				case 4:
					{
						Name = "暗黑法师";
						Story = "古老的军团";

						var Easy = new TaskItem[]
						{
							(ItemID.FallenStar, 12)
						};
						var Normal = new TaskItem[]
						{
							(ItemID.FallenStar, 28)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.FallenStar, 40),
							(ItemID.MolotovCocktail, 99)
						};
						var Hell = new TaskItem[]
						{
							(ItemID.FallenStar, 70),
							(ItemID.MolotovCocktail, 99),
							(ItemID.GoldBar, 99)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Normal,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell
						};
						Rewards = new TaskItem[]
						{
							(ItemID.DefendersForge, 2)
						};
						break;
					}
				#endregion
				#region Bee
				case 5:
					{
						Name = "蜂巢主妇";
						Story = "我的蜂蜜在哪?";

						var Easy = new TaskItem[]
						{
							(ItemID.Stinger, 9),
							(ItemID.Hive, 30) ,
							(ItemID.BottledHoney, 10)
						};
						var Normal = new TaskItem[]
						{
							(ItemID.Stinger, 15),
							(ItemID.Hive, 60) ,
							(ItemID.BottledHoney, 15)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.Stinger, 32),
							(ItemID.Vine, 16),
							(ItemID.Hive, 99),
							(ItemID.BottledHoney, 30)
						};
						var Hell = new TaskItem[]
						{
							(ItemID.HornetBanner, 4),
							(ItemID.Hive, 99),
							(ItemID.Hive, 99),
							(ItemID.BottledHoney, 30),
							(ItemID.BottledHoney, 30),
							(ItemID.BottledHoney, 30)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Normal,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell
						};
						Rewards = new TaskItem[]
						{
							(ItemID.Abeemination,5)
						};
						break;
					}
				#endregion
				#region Skeletron
				case 6:
					Name = "深黑地牢";
					Story = "打破诅咒";
					NeedEx = new ItemLists
					{
						[TaskDifficulty.Easy] = new TaskItem[] { (ItemID.Bone, 100) },
						[TaskDifficulty.Normal] = new TaskItem[] { (ItemID.Bone, 300) },
						[TaskDifficulty.Hard] = new TaskItem[] { (ItemID.BoneKey), (ItemID.BoneKey) },
						[TaskDifficulty.Hell] = new TaskItem[] { (ItemID.BoneKey), (ItemID.BoneKey), (ItemID.BoneKey), (ItemID.BoneKey) }
					};
					Rewards = new TaskItem[]
					{
						(ItemID.BoneKey, (int)difficulty),
						(ItemID.ClothierVoodooDoll)
					};
					break;
				#endregion
				#region Wall
				case 7:
					{
						Name = "仍然饥饿";
						Story = "地下世界的主人与核心";

						var Easy = new TaskItem[]
						{
							(ItemID.HellstoneBar, 70)
						};
						var Normal = new TaskItem[]
						{
							(ItemID.HellstoneBar, 70),
							(ItemID.HellbatBanner, 1)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.Hellstone, 500),
							(ItemID.LavaSlimeBanner, 4),
							(ItemID.Fireblossom, 20)
						};
						var Hell = new TaskItem[]
						{
							(ItemID.Hellstone, 999),
							(ItemID.HellbatBanner, 5),
							(ItemID.LavaSlimeBanner, 5),
							(ItemID.FireblossomSeeds, 5)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Normal,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell
						};
						Rewards = new TaskItem[]
						{
							(ItemID.GuideVoodooDoll),
							(ItemID.GuideVoodooDoll),
							(ItemID.GuideVoodooDoll),
							(ItemID.TrueNightsEdge, 1, PrefixID.Legendary)
						};
						break;
					}
				#endregion
				#region Light
				case 8:
					{
						Name = "光与暗(其一)";
						Story = "虚华美好的假象";

						var Easy = new TaskItem[]
						{
							(ItemID.SoulofLight, 20),
							(ItemID.CrystalShard, 20),
							(ItemID.LightShard, 3)
						};
						var Normal = new TaskItem[]
						{
							(ItemID.SoulofLight, 80),
							(ItemID.CrystalShard, 40),
							(ItemID.LightShard, 5)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.SoulofLight, 170),
							(ItemID.CrystalShard, 60),
							(ItemID.LightShard, 8)
						};
						var Hell = new TaskItem[]
						{
							(ItemID.SoulofLight, 300),
							(ItemID.CrystalShard, 80),
							(ItemID.LightShard, 13)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Normal,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell
						};
						Rewards = new TaskItem[]
						{
							(ItemID.LightKey,15)
						};
						break;
					}
				#endregion
				#region Night
				case 9:
					{
						Name = "光与暗(其二)";

						SuperID material;

						if (WorldGen.crimson)
						{
							Story = "血腥弥漫";
							material = ItemID.Ichor;
						}
						else
						{
							Story = "腐臭四散";
							material = ItemID.CursedFlame;
						}

						var Easy = new TaskItem[]
						{
							(ItemID.SoulofNight, 20),
							(material, 20),
							(ItemID.DarkShard, 2)
						};
						var Normal = new TaskItem[]
						{
							(ItemID.SoulofNight, 80),
							(material, 40),
							(ItemID.DarkShard, 5)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.SoulofNight, 170),
							(material, 60),
							(ItemID.DarkShard, 8)
						};
						var Hell = new TaskItem[]
						{
							(ItemID.SoulofNight, 130070),
							(material, 80),
							(ItemID.DarkShard, 13)
						};

						material.RuntimeBind = true;

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Normal,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell
						};

						Rewards = new TaskItem[]
						{
							(ItemID.NightKey,15)
						};
						break;
					}
				#endregion
				#region Twins
				case 10:
					{
						Name = "全知之眼";
						Story = "这将会是一个可怕的夜晚...";

						var Easy = new TaskItem[]
						{
							(ItemID.Lens, 20),
							(ItemID.DemonEyeBanner, 2),
							(ItemID.PalladiumOre, 222)
						};
						var Normal = new TaskItem[]
						{
							(ItemID.Lens, 25),
							(ItemID.DemonEyeBanner, 4),
							(ItemID.PalladiumOre, 333)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.Lens, 40),
							(ItemID.DemonEyeBanner, 6),
							(ItemID.PalladiumOre, 444)
						};
						var Hell = new TaskItem[]
						{
							(ItemID.Lens, 50),
							(ItemID.DemonEyeBanner, 8),
							(ItemID.PalladiumOre, 555)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Normal,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell
						};

						Rewards = new TaskItem[]
						{
							(ItemID.MechanicalEye,5)
						};
						break;
					}
				#endregion
				#region Prime
				case 11:
					{
						Name = "恐惧之主";
						Story = "周围的空气越来越冷...";

						var Easy = new TaskItem[]
						{
							(ItemID.Bone, 150),
							(ItemID.AngryBonesBanner, 4),
							(ItemID.CursedSkullBanner),
							(ItemID.OrichalcumOre, 222 * 2 / 3)
						};
						var Normal = new TaskItem[]
						{
							(ItemID.Bone, 250),
							(ItemID.AngryBonesBanner, 4),
							(ItemID.CursedSkullBanner),
							(ItemID.OrichalcumOre, 333 * 2 / 3)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.Bone, 380),
							(ItemID.AngryBonesBanner, 6),
							(ItemID.CursedSkullBanner, 2),
							(ItemID.OrichalcumOre, 444 * 2 / 3),
						};
						var Hell = new TaskItem[]
						{
							(ItemID.Bone, 700),
							(ItemID.AngryBonesBanner, 6),
							(ItemID.CursedSkullBanner, 4),
							(ItemID.OrichalcumOre, 666 * 2 / 3),
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Normal,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell
						};
						Rewards = new TaskItem[]
						{
							(ItemID.MechanicalSkull,5)
						};
						break;
					}
				#endregion
				#region Destroyer
				case 12:
					{
						Name = "破坏之王";
						Story = "你感到来自地下深处的震动...";
						SuperID material;
						SuperID banner;
						if (WorldGen.crimson)
						{
							material = ItemID.Vertebrae;
							banner = ItemID.HerplingBanner;
						}
						else
						{
							material = ItemID.RottenChunk;
							banner = ItemID.CorruptorBanner;
						}

						var Easy = new TaskItem[]
						{
							(material, 20),
							(banner, 2),
							(ItemID.TitaniumOre, 222 / 3)
						};
						var Normal = new TaskItem[]
						{
							(material, 30),
							(banner, 3),
							(ItemID.TitaniumOre, 333 / 3)
						};
						var Hard = new TaskItem[]
						{
							(material, 60),
							(banner, 6),
							(ItemID.TitaniumOre, 444 / 3)
						};
						var Hell = new TaskItem[]
						{
							(material, 90),
							(banner, 10),
							(ItemID.TitaniumOre, 666 / 3)
						};

						material.RuntimeBind = true;
						banner.RuntimeBind = true;

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Normal,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell
						};

						Rewards = new TaskItem[]
						{
							(ItemID.MechanicalWorm, 5)
						};
						break;
					}
				#endregion
				#region DD2 T2
				case 13:
					{
						Name = "食人巨魔";
						Story = "异世界的访客";

						var Easy = new TaskItem[]
						{
							(ItemID.SoulofMight, 140),
							(ItemID.SoulofSight, 140),
							(ItemID.SoulofFright,140)
						};
						var Normal = new TaskItem[]
						{
							(ItemID.SoulofMight, 200),
							(ItemID.SoulofSight, 200),
							(ItemID.SoulofFright,200)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.SoulofMight, 333),
							(ItemID.SoulofSight, 333),
							(ItemID.SoulofFright,333)
						};
						var Hell = new TaskItem[]
						{
							(ItemID.SoulofMight, 450),
							(ItemID.SoulofSight, 450),
							(ItemID.SoulofFright,450)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Normal,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell
						};
						Rewards = new TaskItem[]
						{
							(ItemID.DD2ElderCrystal, 20)
						};
						break;
					}
				#endregion
				#region Plantera
				case 14:
					{
						Name = "大南方植物";
						Story = "世纪之花灯泡";

						var Easy = new TaskItem[]
						{
							(ItemID.ChlorophyteOre, 333),
							(ItemID.AngryTrapperBanner),
						};
						var Normal = new TaskItem[]
						{
							(ItemID.ChlorophyteOre, 444),
							(ItemID.AngryTrapperBanner,2),
						};
						var Hard = new TaskItem[]
						{
							(ItemID.ChlorophyteOre, 666),
							(ItemID.TortoiseBanner, 1),
							(ItemID.AngryTrapperBanner, 3),
						};
						var Hell = new TaskItem[]
						{
							(ItemID.ChlorophyteOre, 999),
							(ItemID.TortoiseBanner, 2),
							(ItemID.AngryTrapperBanner, 4),
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Normal,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell
						};
						Rewards = new TaskItem[]
						{
							(ItemID.TheAxe),
							(ItemID.TheAxe),
							(ItemID.TheAxe)
						};
						break;
					}
				#endregion
				#region Golem
				case 15:
					{
						Name = "蜥蜴信仰";
						Story = "太阳图腾";

						var Easy = new TaskItem[]
						{
							(ItemID.TempleKey, 7),
							(ItemID.LihzahrdBanner, 2),
							(ItemID.FlyingSnakeBanner, 2),
							(ItemID.LunarTabletFragment, 33)
						};
						var Normal = new TaskItem[]
						{
							(ItemID.TempleKey, 9),
							(ItemID.LihzahrdBanner, 2),
							(ItemID.FlyingSnakeBanner, 2),
							(ItemID.LunarTabletFragment, 33)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.TempleKey, 15),
							(ItemID.LihzahrdBanner, 6),
							(ItemID.FlyingSnakeBanner, 6),
							(ItemID.LunarTabletFragment, 66)
						};
						var Hell = new TaskItem[]
						{
							(ItemID.TempleKey, 15),
							(ItemID.LihzahrdBanner, 6),
							(ItemID.FlyingSnakeBanner, 6),
							(ItemID.LunarTabletFragment, 99),
							(ItemID.LunarTabletFragment, 99),
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Normal,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell
						};
						Rewards = new TaskItem[]
						{
							(ItemID.LihzahrdPowerCell,10)
						};
						break;
					}
				#endregion
				#region DD2 T3
				case 16:
					{
						Name = "Betsy";
						Story = "灾难的化身";

						var Easy = new TaskItem[]
						{
							(ItemID.MartianConduitPlating, 150),
							(ItemID.BeetleHusk, 50)
						};
						var Normal = new TaskItem[]
						{
							(ItemID.MartianConduitPlating, 300),
							(ItemID.BeetleHusk, 50)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.MartianConduitPlating, 450),
							(ItemID.BeetleHusk, 99)
						};
						var Hell = new TaskItem[]
						{
							(ItemID.MartianConduitPlating, 999),
							(ItemID.BeetleHusk, 99),
							(ItemID.BeetleHusk, 99),
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Normal,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell,
						};
						break;
					}
				#endregion
				#region Cultist
				case 17:
					{
						Name = "狂热教徒";
						Story = "献祭与祈祷";

						var evilKey = WorldGen.crimson ? ItemID.CrimsonKey : ItemID.CorruptionKey;

						var Easy = new TaskItem[]
						{
							(ItemID.GoldenKey),
							(ItemID.ShadowKey),
							(ItemID.CosmicCarKey),
							(ItemID.NightKey),
							(ItemID.LightKey)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.GoldenKey),
							(ItemID.ShadowKey),
							(evilKey),
							(ItemID.FrozenKey),
							(ItemID.JungleKey),
							(ItemID.HallowedKey),
							(ItemID.CosmicCarKey),
							(ItemID.NightKey),
							(ItemID.LightKey)
						};
						var Hell = new TaskItem[]
						{
							(ItemID.GoldenKey),
							(ItemID.ShadowKey),
							(evilKey),
							(ItemID.FrozenKey),
							(ItemID.JungleKey),
							(ItemID.HallowedKey),
							(ItemID.CosmicCarKey),
							(ItemID.NightKey),
							(ItemID.LightKey),
							(ItemID.BoneKey)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Easy,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell
						};
						Rewards = new TaskItem[]
						{
						(ItemID.CultistBossBag),
						(ItemID.CultistBossBag),
						(ItemID.CultistBossBag),
						(ItemID.CultistBossBag),
						(ItemID.CultistBossBag),
						(ItemID.CultistBossBag),
						(ItemID.CultistBossBag),
						(ItemID.CultistBossBag)
						};
						break;
					}
				#endregion
				#region Solar
				case 18:
					{
						Name = "宇宙之怒";
						Story = "你的头脑变得麻木...";


						var Easy = new TaskItem[]
						{
							(ItemID.KingSlimeTrophy),
							(ItemID.EyeofCthulhuTrophy),
							(ItemID.QueenBeeTrophy),
							(ItemID.WallofFleshTrophy),
						};
						var Normal = new TaskItem[]
						{
							(ItemID.KingSlimeTrophy),
							(ItemID.EyeofCthulhuTrophy),
							(ItemID.QueenBeeTrophy),
							(ItemID.WallofFleshTrophy),
							(ItemID.RetinazerTrophy),
							(ItemID.SkeletronPrimeTrophy),
							(ItemID.SpazmatismTrophy),
							(ItemID.DestroyerTrophy),
							(ItemID.PlanteraTrophy)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.EyeofCthulhuTrophy),
							(ItemID.GolemTrophy),
							(ItemID.KingSlimeTrophy),
							(ItemID.MartianSaucerTrophy),
							(ItemID.PlanteraTrophy),
							(ItemID.QueenBeeTrophy),
							(ItemID.RetinazerTrophy),
							(ItemID.SkeletronPrimeTrophy),
							(ItemID.SpazmatismTrophy),
							(ItemID.WallofFleshTrophy),
							(ItemID.DestroyerTrophy),
							(ItemID.DukeFishronTrophy)
						};
						var Hell = new TaskItem[]
						{
							(ItemID.AncientCultistTrophy),
							(ItemID.BossTrophyBetsy),
							(ItemID.BossTrophyDarkmage),
							(ItemID.BossTrophyOgre),
							(ItemID.EverscreamTrophy),
							(ItemID.EyeofCthulhuTrophy),
							(ItemID.GolemTrophy),
							(ItemID.IceQueenTrophy),
							(ItemID.KingSlimeTrophy),
							(ItemID.MartianSaucerTrophy),
							(ItemID.MourningWoodTrophy),
							(ItemID.PlanteraTrophy),
							(ItemID.PumpkingTrophy),
							(ItemID.QueenBeeTrophy),
							(ItemID.RetinazerTrophy),
							(ItemID.SantaNK1Trophy),
							(ItemID.SkeletronPrimeTrophy),
							(ItemID.SpazmatismTrophy),
							(ItemID.WallofFleshTrophy),
							(ItemID.DestroyerTrophy),
							(ItemID.DukeFishronTrophy)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Normal,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell
						};
						Rewards = new TaskItem[]
						{
							(ItemID.PlatinumCoin, 200)
						};
						break;
					}
				#endregion
				#region Nebula
				case 19:
					{
						Name = "银河之力";
						Story = "你痛苦不堪...";

						var Easy = new TaskItem[]
						{
							(ItemID.SolarCoriteBanner, 2),
							(ItemID.SolarDrakomireBanner),
							(ItemID.SolarSolenianBanner),
							(ItemID.SolarSrollerBanner)
						};
						var Normal = new TaskItem[]
						{
							(ItemID.SolarCoriteBanner, 3),
							(ItemID.SolarDrakomireBanner, 2),
							(ItemID.SolarSolenianBanner, 2),
							(ItemID.SolarSrollerBanner, 2)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.SolarCoriteBanner, 6),
							(ItemID.SolarDrakomireBanner, 3),
							(ItemID.SolarSolenianBanner, 3),
							(ItemID.SolarSrollerBanner, 3)
						};
						var Hell = new TaskItem[]
						{
							(ItemID.SolarCoriteBanner, 8),
							(ItemID.SolarCrawltipedeBanner,1),
							(ItemID.SolarDrakomireBanner, 4),
							(ItemID.SolarSolenianBanner, 4),
							(ItemID.SolarSrollerBanner, 4)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Normal,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell
						};
						Rewards = new TaskItem[]
						{
							(ItemID.SolarMonolith, 99),
							(ItemID.NebulaMonolith, 99)
						};
						break;
					}
				#endregion
				#region Stardust
				case 20:
					{
						Name = "星尘粒子";
						Story = "超自然的声音环绕在你的周围...";

						var Easy = new TaskItem[]
						{
							(ItemID.StardustJellyfishBanner),
							(ItemID.StardustLargeCellBanner),
							(ItemID.StardustWormBanner),
							(ItemID.StardustSoldierBanner)
						};
						var Normal = new TaskItem[]
						{
							(ItemID.StardustJellyfishBanner, 2),
							(ItemID.StardustLargeCellBanner, 2),
							(ItemID.StardustWormBanner, 2),
							(ItemID.StardustSoldierBanner, 2)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.StardustJellyfishBanner, 3),
							(ItemID.StardustLargeCellBanner, 3),
							(ItemID.StardustWormBanner, 3),
							(ItemID.StardustSoldierBanner, 3)
						};
						var Hell = new TaskItem[]
						{
							(ItemID.StardustJellyfishBanner, 4),
							(ItemID.StardustLargeCellBanner, 4),
							(ItemID.StardustWormBanner, 4),
							(ItemID.StardustSoldierBanner, 4)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Normal,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell
						};
						Rewards = new TaskItem[]
						{
							(ItemID.StardustMonolith, 99)
						};
						break;
					}
				#endregion
				#region Vortex
				case 21:
					{
						Name = "漩涡能量";
						Story = "月亮末日慢慢靠近...";

						var Easy = new TaskItem[]
						{
							(ItemID.VortexSoldierBanner),
							(ItemID.VortexRiflemanBanner, 2),
						};
						var Normal = new TaskItem[]
						{
							(ItemID.VortexSoldierBanner),
							(ItemID.VortexRiflemanBanner, 2),
							(ItemID.VortexHornetQueenBanner, 5)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.VortexSoldierBanner, 3),
							(ItemID.VortexRiflemanBanner, 4),
							(ItemID.VortexHornetQueenBanner, 15)
						};
						var Hell = new TaskItem[]
						{
							(ItemID.VortexSoldierBanner, 4),
							(ItemID.VortexRiflemanBanner, 5),
							(ItemID.VortexHornetQueenBanner, 15)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Normal,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell
						};
						Rewards = new TaskItem[]
						{
							(ItemID.VortexMonolith,99)
						};
						break;
					}
				#endregion
				#region EyeEx
				case 22:
					{
						Name = "恐惧之眼-The Eye of Phobia";
						Story = "恐惧的本质是什么？未知的事物？死亡的接近？力量总是与诅咒并存，张裂的瞳膜里只剩下恐惧与悲伤";

						var Easy = new TaskItem[]
						{
							(ItemID.Lens, 20),
							(ItemID.BlackLens),
							(ItemID.SoulofFright, 60),
							(ItemID.WillsWings)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.Lens, 50),
							(ItemID.BlackLens, 3),
							(ItemID.SoulofFright, 180),
							(ItemID.WillsWings)
						};
						var Hell = new TaskItem[]
						{
							(ItemID.Lens, 80),
							(ItemID.BlackLens, 3),
							(ItemID.SoulofFright, 300),
							(ItemID.WillsWings)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Easy,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell
						};
						Rewards = new TaskItem[]
						{
							(ItemID.LunarBar, 80),
							(ItemID.NebulaPickup1)
						};
						break;
					}
				#endregion
				#region BrainEX
				case 23:
					{
						Boss = Bosses[0];
						Name = "混乱思维-Cerebrain";
						Story = "从猩红中被剥离出来，膨胀的血管与不断脱落的肉块使这个生物痛苦不堪。精神的力量会扭曲肉体，最终造就可怖的事实";

						var Easy = new TaskItem[]
						{
							(ItemID.Ichor, 55),
							(ItemID.SoulofNight, 100),
							(ItemID.SoulofLight, 100),
							(ItemID.Vertebrae, 44)
						};
						var Normal = new TaskItem[]
						{
							(ItemID.Ichor, 66),
							(ItemID.SoulofNight, 125),
							(ItemID.SoulofLight, 125),
							(ItemID.Vertebrae, 55)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.Ichor, 99),
							(ItemID.SoulofNight, 200),
							(ItemID.SoulofLight, 200),
							(ItemID.Vertebrae, 77)
						};
						var Hell = new TaskItem[]
						{
							(ItemID.Ichor, 99),
							(ItemID.Ichor, 99),
							(ItemID.SoulofNight, 250),
							(ItemID.SoulofLight, 250),
							(ItemID.Vertebrae, 99)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Normal,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell
						};
						Rewards = new TaskItem[]
						{
							(ItemID.LunarBar, 80),
							(ItemID.NebulaPickup1)
						};
						break;
					}
				#endregion
				#region BeeEx
				case 24:
					{
						Boss = Bosses[1];
						Name = "蜂巢意志-Hive Mind";
						Story = "愚钝的成员构成了群体的智慧。蜂群的聚合体。单一个体无法完成的壮举，依靠群体便可以达成";

						var Easy = new TaskItem[]
						{
							(ItemID.LifeFruit, 25),
							(ItemID.SoulofFlight, 100),
							(ItemID.Hive, 222)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.LifeFruit, 75),
							(ItemID.SoulofFlight, 200),
							(ItemID.Hive, 666)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Easy,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hard
						};
						Rewards = new TaskItem[]
						{
							(ItemID.RodofDiscord),
							(ItemID.RodofDiscord),
							(ItemID.RodofDiscord),
							(ItemID.RodofDiscord)
						};
						break;
					}
				#endregion
				#region SkeletonEx
				case 25:
					{
						Boss = Bosses[2];
						Name = "失落骨架-Hyperosteogeny";
						Story = "脱胎于某个高等存在的残骸，仍然残存有些许威力。力量不会随着肉体的消亡而蒸发，而是如同跗骨之蛆一般留存";

						var Easy = new TaskItem[]
						{
							(ItemID.Bone, 666),
							(ItemID.LockBox, 8),
							(ItemID.PaladinBanner)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.Bone, 666),
							(ItemID.Bone, 999),
							(ItemID.LockBox, 16),
							(ItemID.PaladinBanner, 3)
						};
						var Hell = new TaskItem[]
						{
							(ItemID.Bone, 999),
							(ItemID.Bone, 999),
							(ItemID.LockBox, 20),
							(ItemID.PaladinBanner, 4)
						};


						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Easy,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell
						};
						Rewards = new TaskItem[]
						{
							(ItemID.LunarBar, 880)
						};
						break;
					}
				#endregion
				#region DarkMage
				case 26:
					{
						Boss = Bosses[3];
						Name = "流放巫师-The Banished Enchater";
						Story = "远古封印的守卫者，被某种能量所侵蚀。他现在所能做的，仅仅只是维持其中的三道封印";

						var Easy = new TaskItem[]
						{
							(ItemID.DefenderMedal, 500),
							(ItemID.ManaCrystal, 20),
							(ItemID.Yoraiz0rWings)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.DefenderMedal, 999),
							(ItemID.ManaCrystal, 60),
							(ItemID.DD2EnergyCrystal, 10),
							(ItemID.Yoraiz0rWings)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Easy,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hard
						};
						Rewards = new TaskItem[]
						{
							(ItemID.LunarBar, 990),
							(ItemID.Yoraiz0rWings),
							(ItemID.Yoraiz0rWings),
							(ItemID.Yoraiz0rWings)
						};
						break;
					}
				#endregion
				#region StarverWander
				case 27:
					{
						Boss = Bosses[4];
						Name = "徘徊者-The StarverWander";
						Story = "饥饿，迷失，徘徊。这具畸变的肉体中寄宿着旋涡的能量";

						var Easy = new TaskItem[]
						{
							(ItemID.FragmentVortex, 200),
							(ItemID.TruffleWorm, 2),
							(ItemID.SDMG, 1, 82)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.FragmentVortex, 400),
							(ItemID.TruffleWorm, 6),
							(ItemID.SDMG, 1, 82)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Easy,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hard
						};
						Rewards = new TaskItem[]
						{
							(ItemID.Xenopopper),
							(ItemID.Xenopopper),
							(ItemID.Xenopopper),
							(ItemID.Xenopopper)
						};
						break;
					}
				#endregion
				#region StarverRedeemer
				case 28:
					{
						Boss = Bosses[Bosses.Length - 6];
						Name = "清赎者-The StarverRedeemer";
						Story = "苦难，炼狱，救赎。这具畸变的肉体中填满了星尘的微粒";

						var Easy = new TaskItem[]
						{
							(ItemID.FragmentStardust, 200),
							(ItemID.TruffleWorm, 2),
							(ItemID.MasterBait, 100),
							(ItemID.RainbowCrystalStaff, 1, 83)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.FragmentStardust, 400),
							(ItemID.TruffleWorm, 6),
							(ItemID.MasterBait, 200),
							(ItemID.RainbowCrystalStaff, 1, 83)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Easy,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hard
						};
						Rewards = new TaskItem[]
						{
							(ItemID.HotlineFishingHook),
							(ItemID.HotlineFishingHook),
							(ItemID.GoldenFishingRod),
							(ItemID.GoldenFishingRod)
						};
						break;
					}
				#endregion
				#region StarverAdjudicator
				case 29:
					{
						Boss = Bosses[Bosses.Length - 5];
						Name = "裁决者-The StarverAdjudicator";
						Story = "仲裁，审判，裁决。这具畸变的肉体中蕴含着星云的奥秘";

						var Easy = new TaskItem[]
						{
							(ItemID.FragmentNebula, 200),
							(ItemID.TruffleWorm, 2),
							(ItemID.LihzahrdBrick, 300),
							(ItemID.CoinGun),
							(ItemID.LunarFlareBook, 1, PrefixID.Mythical)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.FragmentNebula, 400),
							(ItemID.TruffleWorm, 6),
							(ItemID.LihzahrdBrick, 400),
							(ItemID.CoinGun),
							(ItemID.LunarFlareBook, 1, PrefixID.Mythical)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Easy,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hard
						};
						Rewards = new TaskItem[]
						{
							(ItemID.LaserMachinegun),
							(ItemID.LaserMachinegun),
							(ItemID.LaserMachinegun),
							(ItemID.LaserMachinegun)
						};
						break;
					}
				#endregion
				#region StarverDestroyer
				case 30:
					{
						Boss = Bosses[Bosses.Length - 4];
						Name = "毁灭者-The StarverDestroyer";
						Story = "毁灭，创造，平衡。宇宙的暴怒内蕴其中。现实总是与理想相违背，努力的结果往往被他人夺去";

						var head = ItemID.SquirePlating;
						var armor = ItemID.SquireAltShirt;
						var leg = ItemID.SquireAltPants;

						if (Starver.IsPE)
						{
							head = ItemID.HallowedMask;
							armor = ItemID.HallowedPlateMail;
							leg = ItemID.HallowedGreaves;
						}

						var Easy = new TaskItem[]
						{
							(ItemID.FragmentSolar, 400),
							(ItemID.TruffleWorm, 2),
							(head),
							(armor),
							(leg),
							(ItemID.RocketIV, 999),
							(ItemID.StarWrath, 1, 81)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.FragmentSolar, 400),
							(ItemID.TruffleWorm, 6),
							(head),
							(armor),
							(leg),
							(ItemID.RocketIV, 999),
							(ItemID.RocketIV, 999),
							(ItemID.RocketIV, 999),
							(ItemID.StarWrath, 1, 81)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Easy,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hard
						};
						Rewards = new TaskItem[]
						{
							(ItemID.InfluxWaver),
							(ItemID.InfluxWaver),
							(ItemID.InfluxWaver),
							(ItemID.InfluxWaver)
						};
						break;
					}
				#endregion
				#region Sleep
				case 31:
					{
						Boss = Bosses[Bosses.Length - 3];
						Name = "稍作休整";
						Story = "片刻的宁静";

						var Require = new TaskItem[]
						{
							(ItemID.CookedMarshmallow, 30),
							(ItemID.CookedShrimp, 30),
							(ItemID.BowlofSoup, 30),
							(ItemID.Sake, 30),
							(ItemID.BeachBall),
							(ItemID.UnicornonaStick)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Require,
							[TaskDifficulty.Normal] = Require,
							[TaskDifficulty.Hard] = Require,
							[TaskDifficulty.Hell] = Require
						};
						Rewards = new TaskItem[]
						{
							(ItemID.LunarBar, 999),
							(ItemID.LunarBar, 999),
							(ItemID.Pigronata, 300),
							(ItemID.FlaskofParty, 30)
						};
					}
					break;
				#endregion
				#region RedDevil
				case 32:
					{
						Name = "地狱领主-The Lord of the Underworld";
						Story = "恶魔一族的君主，似乎能够自如地使用地狱的威能。苦痛的亡魂在熔岩中挣扎，他们呼喊着那个早已消逝的名字";

						var Easy = new TaskItem[]
						{
							(ItemID.LivingFireBlock, 200),
							(ItemID.FireFeather, 1),
							(ItemID.UnholyTrident),
							(ItemID.InfernoFork),
						};
						var Hard = new TaskItem[]
						{
							(ItemID.LivingFireBlock, 600),
							(ItemID.FireFeather, 3),
							(ItemID.UnholyTrident),
							(ItemID.InfernoFork),
							//(ItemID.FlarefinKoi, 120)
						};
						var Hell = new TaskItem[]
						{
							(ItemID.LivingFireBlock, 800),
							(ItemID.FireFeather, 4),
							(ItemID.UnholyTrident),
							(ItemID.InfernoFork),
							//(ItemID.FlarefinKoi, 120)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Easy,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell
						};
						Rewards = new TaskItem[]
						{
							(ItemID.NebulaPickup1),
							(ItemID.LunarBar, 680),
							(ItemID.LunarBar, 680),
							(ItemID.LunarBar, 680),
							(ItemID.LunarBar, 680)
						};
						break;
					}
				#endregion
				#region Pigron
				case 33:
					{
						Boss = Bosses[5];
						Name = "畸变生物-thAt d15ToRt10N";
						Story = "在被污染的冰窟中滋生的畸形生物，无节制地进行着自我增殖。自我循环或外界摄取，这个世界支配着残酷的法则";

						var Easy = new TaskItem[]
						{
							(ItemID.Bacon, 20),
							(ItemID.ScalyTruffle),
							(ItemID.OldShoe, 5),
							(ItemID.FishingSeaweed, 5),
							(ItemID.TinCan, 5)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.Bacon, 20),
							(ItemID.ScalyTruffle),
							(ItemID.OldShoe, 15),
							(ItemID.FishingSeaweed, 15),
							(ItemID.TinCan, 15)
						};
						var Hell = new TaskItem[]
						{
							(ItemID.Bacon, 20),
							(ItemID.ScalyTruffle),
							(ItemID.OldShoe, 30),
							(ItemID.FishingSeaweed, 30),
							(ItemID.TinCan, 30)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Easy,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell
						};
						Rewards = new TaskItem[]
						{
							(ItemID.NebulaPickup2),
							(ItemID.IntenseRainbowDye, 50)
						};
						break;
					}
				#endregion
				#region PrimeEx
				case 34:
					{
						Boss = Bosses[6];
						Name = "骷髅暴徒 - The Skeletrorist";
						Story = "这位恐惧之主早已失去了往日的荣光，变异的躯壳只寄宿着无尽的愤怒。暴虐与残忍，嗜血与屠戮，欲望和执念造就的只有悲剧";

						SuperID evil = WorldGen.crimson ? ItemID.VampireKnives : ItemID.ScourgeoftheCorruptor;
						evil.RuntimeBind = true;

						var Easy = new TaskItem[]
						{
							(ItemID.SkeletronPrimeTrophy, 2),
							(evil, 1, 59),
							(ItemID.RainbowGun, 1, 83),
							(ItemID.PiranhaGun, 1, 82),
							(ItemID.StaffoftheFrostHydra, 1, 83)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.SkeletronPrimeTrophy, 6),
							(evil, 1, 59),
							(ItemID.RainbowGun, 1, 83),
							(ItemID.PiranhaGun, 1, 82),
							(ItemID.StaffoftheFrostHydra, 1, 83)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Easy,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hard
						};
						Rewards = new TaskItem[]
						{
							(ItemID.NebulaPickup3),
							(ItemID.LunarBar, 990),
							(ItemID.LunarBar, 990),
							(ItemID.LunarBar, 990),
							(ItemID.GreedyRing),
							(ItemID.GreedyRing),
							(ItemID.GreedyRing),
							(ItemID.GreedyRing)
						};
						break;
					}
				#endregion
				#region Worm
				case 35:
					{
						Boss = Bosses[7];
						Name = "地脉吸食者-The Seisminth";
						Story = "吸食了地脉的精华，但仍然保留着蠕虫的姿态。这片大地已经千疮百孔，在一次次的轮回中逐渐褪色";

						SuperID material = ItemID.RottenChunk;
						SuperID wpmaterial = ItemID.CursedFlame;

						if (WorldGen.crimson)
						{
							material = ItemID.Vertebrae;
							wpmaterial = ItemID.Ichor;
						}

						material.RuntimeBind = true;
						wpmaterial.RuntimeBind = true;

						var Easy = new TaskItem[]
						{
							(material, 99),
							(wpmaterial, 99),
							(ItemID.AncientCloth, 10),
							(ItemID.FossilOre, 100)
						};
						var Hard = new TaskItem[]
						{
							(material, 99),
							(material, 50),
							(wpmaterial, 99),
							(wpmaterial, 50),
							(ItemID.AncientCloth, 30),
							(ItemID.FossilOre, 300)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Easy,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hard
						};
						Rewards = new TaskItem[]
						{
							(ItemID.LunarBar, 200),
							(ItemID.DrillContainmentUnit)
						};
						break;
					}
				#endregion
				#region Retinazer
				case 36:
					{
						Boss = Bosses[8];
						Name = "预视全知的左眼-The Propheyes";
						Story = "最终的劫难已逐渐逼近，艰难的旅程也终于抵达尾声。未来的图景从未如此朦胧";
						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = new TaskItem[] { (ItemID.RetinazerTrophy, 2) },
							[TaskDifficulty.Normal] = new TaskItem[] { (ItemID.RetinazerTrophy, 4) },
							[TaskDifficulty.Hard] = new TaskItem[] { (ItemID.RetinazerTrophy, 6) },
							[TaskDifficulty.Hell] = new TaskItem[] { (ItemID.RetinazerTrophy, 8) }
						};
						Rewards = new TaskItem[]
						{
							(ItemID.LunarBar, 120),
							(ItemID.RedPotion, 30)
						};
						break;
					}
				#endregion
				#region Spazmatism
				case 37:
					{
						Boss = Bosses[9];
						Name = "洞悉过往的右眼-The Propheyes";
						Story = "历史的书页遗失了一角，某个过往的存在切断了与这个世界的联系。他是连接一切的钥匙";
						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = new TaskItem[] { (ItemID.SpazmatismTrophy, 2) },
							[TaskDifficulty.Normal] = new TaskItem[] { (ItemID.SpazmatismTrophy, 4) },
							[TaskDifficulty.Hard] = new TaskItem[] { (ItemID.SpazmatismTrophy, 6) },
							[TaskDifficulty.Hell] = new TaskItem[] { (ItemID.SpazmatismTrophy, 8) }
						};
						Rewards = new TaskItem[]
						{
							(ItemID.LunarBar, 120),
							(ItemID.RedPotion, 30)
						};
						break;
					}
				#endregion
				#region EndTrial1
				case 38:
					{
						Boss = Bosses[10];
						Name = "末世预言--凶兆";
						Story = "T.S.K.S:--==欢迎==--初次联络--stardust的玩家们，你们迄今为止的努力着实令人敬佩，而现在，我们有一项伟大而光荣的委托要交付于你们";

						var Easy = new TaskItem[]
						{
							(ItemID.CopperBar, 22),
							(ItemID.TinBar, 22),
							(ItemID.IronBar, 22),
							(ItemID.LeadBar, 22),
							(ItemID.SilverBar, 22),
							(ItemID.TungstenBar, 22),
							(ItemID.GoldBar, 22),
							(ItemID.PlatinumBar, 22),
							(ItemID.DemoniteBar, 22),
							(ItemID.CrimtaneBar, 22),
							(ItemID.HellstoneBar, 22),
							(ItemID.PalladiumBar, 22),
							(ItemID.CobaltBar, 22),
							(ItemID.MythrilBar, 22),
							(ItemID.OrichalcumBar, 22),
							(ItemID.AdamantiteBar, 22),
							(ItemID.TitaniumBar, 22)
						};
						var Normal = new TaskItem[]
						{
							(ItemID.CopperBar, 44),
							(ItemID.TinBar, 44),
							(ItemID.IronBar, 44),
							(ItemID.LeadBar, 44),
							(ItemID.SilverBar, 44),
							(ItemID.TungstenBar, 44),
							(ItemID.GoldBar, 44),
							(ItemID.PlatinumBar, 44),
							(ItemID.DemoniteBar, 44),
							(ItemID.CrimtaneBar, 44),
							(ItemID.HellstoneBar, 44),
							(ItemID.PalladiumBar, 44),
							(ItemID.CobaltBar, 44),
							(ItemID.MythrilBar, 44),
							(ItemID.OrichalcumBar, 44),
							(ItemID.AdamantiteBar, 44),
							(ItemID.TitaniumBar, 44)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.CopperBar, 66),
							(ItemID.TinBar, 66),
							(ItemID.IronBar, 66),
							(ItemID.LeadBar, 66),
							(ItemID.SilverBar, 66),
							(ItemID.TungstenBar, 66),
							(ItemID.GoldBar, 66),
							(ItemID.PlatinumBar, 66),
							(ItemID.DemoniteBar, 66),
							(ItemID.CrimtaneBar, 66),
							(ItemID.HellstoneBar, 66),
							(ItemID.PalladiumBar, 66),
							(ItemID.CobaltBar, 66),
							(ItemID.MythrilBar, 66),
							(ItemID.OrichalcumBar, 66),
							(ItemID.AdamantiteBar, 66),
							(ItemID.TitaniumBar, 66)
						};
						var Hell = new TaskItem[]
						{
							(ItemID.CopperBar, 88),
							(ItemID.TinBar, 88),
							(ItemID.IronBar, 88),
							(ItemID.LeadBar, 88),
							(ItemID.SilverBar, 88),
							(ItemID.TungstenBar, 88),
							(ItemID.GoldBar, 88),
							(ItemID.PlatinumBar, 88),
							(ItemID.DemoniteBar, 88),
							(ItemID.CrimtaneBar, 88),
							(ItemID.HellstoneBar, 88),
							(ItemID.PalladiumBar, 88),
							(ItemID.CobaltBar, 88),
							(ItemID.MythrilBar, 88),
							(ItemID.OrichalcumBar, 88),
							(ItemID.AdamantiteBar, 88),
							(ItemID.TitaniumBar, 88)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Normal,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell,
						};
						Rewards = new TaskItem[]
						{
							(ItemID.Fake_newchest2, 99),
							(ItemID.Fake_newchest2, 99),
							(ItemID.Fake_newchest2, 99),
							(ItemID.Fake_newchest2, 99)
						};
						Level += 1250;
						break;
					}
				#endregion
				#region EndTrial2
				case 39:
					{
						Name = "末世预言--混乱";
						Story = "T.S.K.S:你们之前已经漂亮地完成了我们的每一次委托，所以我们相信这一件委托这对你们来说绝非难事";

						var Easy = new TaskItem[]
						{
							(ItemID.SlimySaddle),
							(ItemID.HardySaddle),
							(ItemID.HoneyedGoggles),
							(ItemID.FuzzyCarrot),
							(ItemID.BlessedApple),
						};
						var Normal = new TaskItem[]
						{
							(ItemID.SlimySaddle),
							(ItemID.HardySaddle),
							(ItemID.HoneyedGoggles),
							(ItemID.FuzzyCarrot),
							(ItemID.AncientHorn),
							(ItemID.BlessedApple),
							(ItemID.CosmicCarKey)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.SlimySaddle),
							(ItemID.HardySaddle),
							(ItemID.HoneyedGoggles),
							(ItemID.FuzzyCarrot),
							(ItemID.AncientHorn),
							(ItemID.BlessedApple),
							(ItemID.BrainScrambler),
							(ItemID.CosmicCarKey)
						};
						var Hell = new TaskItem[]
						{
							(ItemID.SlimySaddle),
							(ItemID.HardySaddle),
							(ItemID.HoneyedGoggles),
							(ItemID.FuzzyCarrot),
							(ItemID.AncientHorn),
							(ItemID.BlessedApple),
							(ItemID.ReindeerBells),
							(ItemID.BrainScrambler),
							(ItemID.CosmicCarKey)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Normal,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell
						};
						Rewards = new TaskItem[]
						{
							(ItemID.BlueCultistFighterBanner, 99),
							(ItemID.BlueCultistFighterBanner, 99),
							(ItemID.BlueCultistFighterBanner, 99),
							(ItemID.BlueCultistFighterBanner, 99)
						};
						Level += 1250;
						break;
					}
				#endregion
				#region EndTrial3
				case 40:
					{
						Name = "末世预言--秩序";
						Story = "T.S.K.S:我们的确掌握着某种限制大型怪物生成的技术，但归根究底其源泉也是我们所共有的AURA能量";

						var Easy = new TaskItem[]
						{
							(ItemID.AngelWings),
							(ItemID.DemonWings),
							(ItemID.LeafWings)
						};
						var Normal = new TaskItem[]
						{
							(ItemID.AngelWings),
							(ItemID.DemonWings),
							(ItemID.BeeWings),
							(ItemID.ButterflyWings),
							(ItemID.LeafWings)
						};
						var Hard = new TaskItem[]
						{
							(ItemID.AngelWings),
							(ItemID.DemonWings),
							(ItemID.BeeWings),
							(ItemID.ButterflyWings),
							(ItemID.FairyWings),
							(ItemID.BatWings),
							(ItemID.LeafWings)
						};
						var Hell = new TaskItem[]
						{
							(ItemID.AngelWings),
							(ItemID.DemonWings),
							(ItemID.BeeWings),
							(ItemID.ButterflyWings),
							(ItemID.FairyWings),
							(ItemID.BatWings),
							(ItemID.HarpyWings),
							(ItemID.FlameWings),
							(ItemID.FrozenWings),
							(ItemID.LeafWings)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Easy,
							[TaskDifficulty.Normal] = Normal,
							[TaskDifficulty.Hard] = Hard,
							[TaskDifficulty.Hell] = Hell
						};
						Rewards = new TaskItem[]
						{
							(ItemID.BlueCultistCasterBanner, 99),
							(ItemID.BlueCultistCasterBanner, 99),
							(ItemID.BlueCultistCasterBanner, 99),
							(ItemID.BlueCultistCasterBanner, 99)
						};
						Level += 1250;
						break;
					}
				#endregion
				#region EndTrial4
				case 41:
					{
						Name = "末世预言--开端";
						Story = @"T.S.K.S:但是, 不知道各位是否想过,AURA的源泉又是什么呢?
我们解析过这种能量的构造,其波动与你们的某一种欲望十分符合";

						var Require = new TaskItem[]
						{
							(ItemID.WoodYoyo),
							(ItemID.Code1),
							(ItemID.Amarok),
							(ItemID.JungleYoyo),
							(ItemID.CrimsonYoyo),
							(ItemID.CorruptYoyo),
							(ItemID.TheEyeOfCthulhu),
							(ItemID.Yelets),
							(ItemID.Kraken),
							(ItemID.Rally),
							(ItemID.Chik),
							(ItemID.Cascade),
							(ItemID.HelFire),
							(ItemID.Valor),
							(ItemID.FormatC),
							(ItemID.Code2),
							(ItemID.Gradient)
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Require,
							[TaskDifficulty.Normal] = Require,
							[TaskDifficulty.Hard] = Require,
							[TaskDifficulty.Hell] = Require
						};
						Rewards = new TaskItem[]
						{
							(ItemID.Fake_newchest1, 99),
							(ItemID.Fake_newchest1, 99),
							(ItemID.Fake_newchest1, 99),
							(ItemID.Fake_newchest1, 99)
						};
						Level += 1250;
						break;
					}
				#endregion
				#region EndTrial5
				case 42:
					{
						Name = "末世预言--降临";
						Story = "T.S.K.S:至于委托的内容...啊，谢谢各位，我们已经快要完成了...通过这几次的任务...";

						var Require = new TaskItem[]
						{
							(ItemID.MusicBoxOverworldDay),
							(ItemID.MusicBoxAltOverworldDay),
							(ItemID.MusicBoxNight),
							(ItemID.MusicBoxRain),
							(ItemID.MusicBoxSnow),
							(ItemID.MusicBoxDesert),
							(ItemID.MusicBoxOcean),
							(ItemID.MusicBoxSpace),
							(ItemID.MusicBoxUnderground),
							(ItemID.MusicBoxMushrooms),
							(ItemID.MusicBoxJungle),
							(ItemID.MusicBoxCorruption),
							(ItemID.MusicBoxCrimson),
							(ItemID.MusicBoxTheHallow),
							(ItemID.MusicBoxHell),
							(ItemID.MusicBoxDungeon),
							(ItemID.MusicBoxTemple),
							(ItemID.MusicBoxEerie),
							(ItemID.MusicBoxEclipse),
							(ItemID.MusicBoxGoblins),
							(ItemID.MusicBoxPirates),
							(ItemID.MusicBoxMartians),
							(ItemID.MusicBoxPumpkinMoon),
							(ItemID.MusicBoxFrostMoon),
							(ItemID.MusicBoxSandstorm),
							(ItemID.MusicBoxDD2),
							(ItemID.MusicBoxTowers),
							(ItemID.MusicBoxBoss1),
							(ItemID.MusicBoxBoss2),
							(ItemID.MusicBoxBoss3),
							(ItemID.MusicBoxBoss4),
							(ItemID.MusicBoxBoss5),
							(ItemID.MusicBoxPlantera),
							(ItemID.MusicBoxLunarBoss),
						};

						NeedEx = new ItemLists
						{
							[TaskDifficulty.Easy] = Require,
							[TaskDifficulty.Normal] = Require,
							[TaskDifficulty.Hard] = Require,
							[TaskDifficulty.Hell] = Require
						};
						Rewards = new TaskItem[]
						{
							(StarverBossManager.EndTrialSummoner, 50)
						};
						break;
					}
				#endregion
				#region End
				case 43:
					{
						Name = "末世预言--终末";
						Boss = Bosses[Bosses.Length - 2];
						Story = "THE STARDUST KARMA SACRIFICE--T.S.K.S:终于...我找到了...";
						break;
					}
					#endregion
			}
			if (Normal)
			{
				SetDefault();
			}
		}
		#endregion
		#region SetDefault
		protected virtual void SetDefault()
		{
			if (NeedEx != null && NeedEx.ContainsKey(Difficulty))
				Needs = NeedEx[Difficulty];
			if(RewardEx == null)
			{
				RewardEx = new ItemLists
				{
					[TaskDifficulty.Easy] = Rewards,
					[TaskDifficulty.Normal] = Rewards,
					[TaskDifficulty.Hard] = Rewards,
					[TaskDifficulty.Hell] = Rewards
				};
			}
			else
			{
				Rewards = RewardEx[Difficulty];
			}
			if (Needs == null || StarverConfig.Config.TaskNeedNoItem)
			{
				Needs = DefaultNeed;
			}
			if (Rewards == null)
			{
				Rewards = DefaultReward;
			}

			Boss ??= ID switch
			{
				23 => Bosses[0],
				24 => Bosses[1],
				25 => Bosses[2],
				26 => Bosses[3],
				27 => Bosses[4],
				28 => Bosses[Bosses.Length - 6],
				29 => Bosses[Bosses.Length - 5],
				30 => Bosses[Bosses.Length - 4],
				31 => Bosses[Bosses.Length - 3],
				33 => Bosses[5],
				34 => Bosses[6],
				35 => Bosses[7],
				36 => Bosses[8],
				37 => Bosses[9],
				38 => Bosses[10],
				43 => Bosses[Bosses.Length - 2],
				_ => null
			};

			checker = DefaultCheck;
			if (Description == null && Name != null && Story != null)
			{
				StringBuilder SB = new StringBuilder("");
				SB.AppendFormat("主线任务#{0}--({1}){2}", ID, Name, Story == null ? "" : ("\n\"" + Story + "\""));
				if (CheckItem)
				{
					SB.AppendFormat("\n物品要求:\n[i:{0}](标识物,不消耗)", Mark);
					foreach (object obj in Needs)
					{
						SB.Append(obj);
					}
					SB.Append("\n物品奖励:\n");
					foreach (object obj in Rewards)
					{
						SB.Append(obj);
					}
				}
				Description = SB.ToString();
			}
		}
		#endregion
		#region ToDatas
		public MainLineTaskData ToDatas()
		{
			return new MainLineTaskData
			{
				Name = Name,
				Story = Story,
				Rewards = RewardEx,
				Needs = NeedEx,
				LevelReward = Level,
				ID = ID
			};
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			return Description;
		}
		#endregion
		#region Checks
		#region Check
		protected CheckDelegate checker;
		public bool Check(StarverPlayer player)
		{
			return checker(player);
		}
		protected bool DefaultCheck(StarverPlayer player)
		{
			bool flag = true;
			if (Normal)
			{
				if (!(Boss is null))
				{
					if (!Boss.Downed)
					{
						StarverPlayer.All.SendMessage($"这个任务被{Boss.Name}诅咒了...", Color.Red);
						return false;
					}
				}
				if (CheckLevel)
				{
					flag = flag && Utils.AverageLevel >= Level;
					if (flag == false)
					{
						StarverPlayer.All.SendMessage($"全员平均等级未达标({Level})", Color.Red);
					}
				}
				if (CheckItem)
				{
					flag = flag && CheckChests(player);
				}
			}
			else
			{

			}
			return flag;
		}
		#endregion
		#region CheckChests
		protected bool CheckChests(StarverPlayer player)
		{
			for (int i = 0; i < Main.chest.Length; i++)
			{
				if (Main.chest[i] == null || !CheckItems(Main.chest[i].item))
				{
					continue;
				}
				try
				{
					Reward(player, i);
				}
				catch(Exception e)
				{
					StarverPlayer.Server.SendErrorMessage($"Task 奖励物品失败: \n{e}");
				}
				return true;
			}
			return false;
		}
		#endregion
		#region CheckItem
		protected bool CheckItems(Item[] items)
		{
			if (items.Length <= Needs.Length)
			{
				return false;
			}
			if (items[0].type != Mark)
			{
				return false;
			}
			for (int i = 0; i < Needs.Length; i++)
			{
				if (!Needs[i].Match(items[i + 1]))
				{
					StarverPlayer.All.SendMessage($"所需物品{Needs[i]} 与检测到物品[i/s{items[1 + i].stack}:{items[1 + i].type}]不符", Color.Blue);
					return false;
				}
				else
				{
					StarverPlayer.All.SendMessage($"所需物品{Needs[i]} 与检测到物品[i/s{items[1 + i].stack}:{items[1 + i].type}]符合", Color.Aqua);
				}
			}
			return true;
		}
		#endregion
		#region Check_20
		private static bool Check_20()
		{
			foreach (Chest chest in Main.chest)
			{
				if (chest == null || chest.item[0].type != 50)
					continue;
				for (int i = 1; i < 21; i++)
				{
					if (chest.item[i].type > 3042 || chest.item[i].type < 2869)
					{
						goto loops;
					}
				}
				for (int i = 1; i < chest.item.Length; i++)
				{
					chest.item[i] = new Item();
				}
				return true;
				loops:;
			}
			return false;
		}
		#endregion
		#region EatItems
		protected void EatChestItem(Item[] items)
		{
			for (int i = 1; i < items.Length; i++)
			{
				items[i] = new Item();
			}
		}
		#endregion
		#endregion
		#region Rewards
		protected void Reward(StarverPlayer player, int chestIndex)
		{
			EatChestItem(Main.chest[chestIndex].item);
			RewardChestItem(Main.chest[chestIndex]);
			if (player?.ActiveChest == chestIndex)
			{
				player.ActiveChest = -1;
			}
			if (!CheckLevel)
				RewardLevel();
		}
		protected void RewardLevel()
		{
			Utils.UpGradeAll(Level);
			StarverPlayer.All.SendInfoMessage($"获得等级奖励: {Level}");
		}
		protected void RewardChestItem(Chest chest)
		{
			for (int i = 0; i < Rewards.Length && i < chest.item.Length; i++)
			{
				int num = Utils.NewItem(Rewards[i].ID, Rewards[i].Stack, Rewards[i].Prefix);
				chest.AddItemToShop(Main.item[num]);
			}
		}
		#endregion
	}
}