using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
namespace Starvers.TaskSystem.Branches
{
	using Vector = TOFOUT.Terraria.Server.Vector2;
	using NPCSystem.NPCs;
	public partial class StoryToContinue
	{
		private class Task : BranchTask
		{
			public int? ID { get; }
			public override BLID BLID => BLID.StoryToContinue;

			public Task(int? id, StarverPlayer player = null) : base(player)
			{
				ID = id;
			}

			public void SetDefault()
			{

			}

			public (bool success, string msg) CanStartTask(StarverPlayer player)
			{
				switch (ID)
				{
					case 0:
					case 1:
					case 2:
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
					default:
						throw new InvalidOperationException("空任务");
				}
			}

			private void Success()
			{
				RewardPlayer();
				TargetPlayer.BranchTaskEnd(true);
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
							if (TargetPlayer.ManaMax < 220)
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
							if (TargetPlayer.Level < 1200)
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

			#region Utils
			private static ElfHeliEx NewEnemy(Vector2 where, Vector2 vel = default)
			{
				if (HeliRoot == null)
				{
					HeliRoot = NPCSystem.StarverNPC.RootNPCs.First(npc => npc is ElfHeliEx) as ElfHeliEx;
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
