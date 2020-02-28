using Microsoft.Xna.Framework;
using Starvers.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Terraria.ID;

namespace Starvers.TaskSystem.Branches
{
	public partial class TestLine1
	{
		private class Task : BranchTask
		{
			private Action<NPCStrikeEventArgs> NPCStriked;
			private TaskData data;

			public int? ID { get; }
			public override BLID BLID => BLID.TestLine1;

			public Task(int? id, StarverPlayer player = null) : base(player)
			{
				ID = id;
			}

			public override void StrikedNPC(NPCStrikeEventArgs args)
			{
				base.StrikedNPC(args);
				NPCStriked(args);
			}

			public override void Start()
			{
				base.Start();
				TargetPlayer.SendInfoMessage(Description);
			}

			public (bool Success, string Message) CanStartTask(StarverPlayer player)
			{
				switch (ID)
				{
					case 0:
					case 1:
					case 2:
						{
							if (!player.HasItem(WeaponRequire))
							{
								return (false, StartFailResons.HaveNoPaladinsHammer);
							}
							return (true, null);
						}
					default:
						throw new InvalidOperationException("空任务");
				}
			}

			public void SetDefault()
			{
				switch (ID)
				{
					case 0:
						{
							NPCStriked = StrikedNPC_0;
							data.RequireCount = 5;
							Description = @$"使用圣骑士之锤击杀{data.RequireCount}个敌人";
							break;
						}
					case 1:
						{
							NPCStriked = StrikedNPC_1;
							data.RequireCount = 10;
							Description = @$"使用圣骑士之锤击杀{data.RequireCount}个敌人";
							break;
						}
					case 2:
						{
							NPCStriked = StrikedNPC_2;
							data.RequireCount = 15;
							Description = @$"使用圣骑士之锤击杀{data.RequireCount}个敌人";
							break;
						}
					default:
						throw new InvalidOperationException("空任务");
				}
			}

			public override string ToString()
			{
				return $"TestLine1--{ID + 1}";
			}

			#region NPCStrikeds
			private void StrikedNPC_0(NPCStrikeEventArgs args)
			{
				if (TargetPlayer.HeldItem.type == WeaponRequire && args.KilledNPC)
				{
					TargetPlayer.SendCombatMsg($"击杀量: {++data.KillCount} / {data.RequireCount}", Color.Beige);
					if (data.KillCount >= data.RequireCount)
					{
						TargetPlayer.BranchTaskEnd(true);
					}
				}
			}
			private void StrikedNPC_1(NPCStrikeEventArgs args)
			{
				if (TargetPlayer.HeldItem.type == WeaponRequire && args.KilledNPC)
				{
					TargetPlayer.SendCombatMsg($"击杀量: {++data.KillCount} / {data.RequireCount}", Color.Beige);
					if (data.KillCount >= data.RequireCount)
					{
						TargetPlayer.BranchTaskEnd(true);
					}
				}
			}
			private void StrikedNPC_2(NPCStrikeEventArgs args)
			{
				if (TargetPlayer.HeldItem.type == WeaponRequire && args.KilledNPC)
				{
					TargetPlayer.SendCombatMsg($"击杀量: {++data.KillCount} / {data.RequireCount}", Color.Beige);
					if (data.KillCount >= data.RequireCount)
					{
						TargetPlayer.BranchTaskEnd(true);
					}
				}
			}
			#endregion
			#region Fails
			private static class StartFailResons
			{
				public const string HaveNoPaladinsHammer = "你需要一个圣骑士之锤";
			}
			#endregion
			#region TaskData
			private struct TaskData
			{
				private Data16 data;

				public int KillCount
				{
					get => data.IntValue0;
					set => data.IntValue0 = value;
				}
				public int RequireCount
				{
					get => data.IntValue1;
					set => data.IntValue1 = value;
				}
			}
			#endregion
		}
	}
}
