using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace Starvers.Enemies.Npcs
{
	public class StarverNPCManager
	{
		#region Properties
		private StarverNPC[] roots;
		private StarverNPC[] npcs;
		#endregion
		#region Properties
		public StarverNPC this[int index]
		{
			get
			{
				if (index < 0 || index >= Main.maxNPCs)
				{
					throw new IndexOutOfRangeException($"index: {index}");
				}
				return npcs[index];
			}
		}
		public int Count
		{
			get => Main.maxNPCs;
		}
		#endregion
		#region Ctor
		public StarverNPCManager()
		{
			Load();
		}
		#region Load & UnLoad
		public void Load()
		{
			// Commands.ChatCommands.Add(new Command(Perms.Test, TCommand, "snpc"));
			//ServerApi.Hooks.NpcKilled.Register(Starver.Instance, StarverNPC.OnNPCKilled);
			roots = new StarverNPC[]
			{
				new FloatingSkeleton()
			};
			npcs = new StarverNPC[Main.maxNPCs];
		}
		public void UnLoad()
		{
			// Commands.ChatCommands.RemoveAll(cmd => cmd.HasAlias("snpc"));
			//ServerApi.Hooks.NpcKilled.Deregister(Starver.Instance, StarverNPC.OnNPCKilled);
		}
		#endregion
		#endregion
		#region Update
		public void Update()
		{
			#region CheckSpawn
			foreach (var root in roots)
			{
				foreach (var player in Starver.Instance.Players)
				{
					if (player != null && player.Alive)
					{
						if (root.CheckSpawn(player))
						{
							if (root.TrySpawnNewNpc(player, out var npc))
							{
								npcs[npc.Index] = npc;
							}
						}
					}
				}
			}
			#endregion
			#region AIUpdate
			for (int i = 0; i < npcs.Length; i++)
			{
				if (npcs[i] == null)
				{
					continue;
				}
				if (!npcs[i].Active)
				{
					npcs[i] = null;
					return;
				}
				npcs[i].AI();
			}
			#endregion
		}
		#endregion
		#region OnDrop
		public void OnDrop(NpcLootDropEventArgs args)
		{
			if (npcs[args.NpcArrayIndex] != null && npcs[args.NpcArrayIndex].Active)
			{
				args.Handled = npcs[args.NpcArrayIndex].OverrideRawDrop;
				npcs[args.NpcArrayIndex].DropItems();
			}
		}
		#endregion
		#region Command
#if false
// 手动召唤npc的位置
		private void TCommand(CommandArgs args)
		{
			if (args.Parameters.Count < 1)
			{
				for (int i = 0; i < StarverNPC.RootNPCs.Count; i++)
				{
					args.Player.SendInfoMessage($"{i}: {StarverNPC.RootNPCs[i].Name}");
				}
			}
			else if (int.TryParse(args.Parameters[0], out int result))
			{
				int idx = StarverNPC.NewNPC((Vector)(args.TPlayer.Center + Starver.Rand.NextVector2(16 * 80, 16 * 50)), default, StarverNPC.RootNPCs[result]);
				if (StarverNPC.RootNPCs[result] is ElfHeliEx)
				{
					if (args.Parameters.Count > 1 && int.TryParse(args.Parameters[1], out int work))
					{
						var heli = npcs[idx] as ElfHeliEx;
						switch (work)
						{
							case 0:
								heli.Guard(args.TPlayer.Center);
								break;
							case 1:
								heli.Attack(args.SPlayer());
								break;
							case 2:
								heli.Wonder(args.TPlayer.Center, Starver.Rand.NextVector2(1));
								break;
							case 4:
								heli.WonderAttack(args.TPlayer.Center, new Vector(1, 0), 10, false);
								break;
						}
						if (args.Parameters.Count > 2 && int.TryParse(args.Parameters[2], out int shot))
						{
							if (0 <= shot && shot < ElfHeliEx.MaxShots)
							{
								heli.SetShot(shot);
							}
							else
							{
								throw new IndexOutOfRangeException(nameof(shot) + ": " + shot);
							}
						}
					}
				}
			}
		}
#endif
		#endregion
	}
}
