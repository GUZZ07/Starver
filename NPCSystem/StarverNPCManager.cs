using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaApi.Server;
using TShockAPI;

namespace Starvers.NPCSystem
{
	using NPCs;
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public class StarverNPCManager : IStarverPlugin
	{
		#region Properties
		public StarverConfig Config => StarverConfig.Config;
		#endregion
		#region Interfeces
		public bool Enabled
		{
			get
			{
				if (Config.EnableNPC)
				{
					if (!Config.EnableAura)
					{
						StarverPlayer.Server.SendInfoMessage("使用Starver的NPC组件需要先开启Aura以及Task组件");
						return false;
					}
					return true;
				}
				return false;
			}
		}
		public void Load()
		{
			StarverNPC.Load();
			ServerApi.Hooks.GameUpdate.Register(Starver.Instance, StarverNPC.DoUpDate);
			ServerApi.Hooks.NpcLootDrop.Register(Starver.Instance, OnDrop);
			Commands.ChatCommands.Add(new Command(Perms.Test, TCommand, "snpc"));
			//ServerApi.Hooks.NpcKilled.Register(Starver.Instance, StarverNPC.OnNPCKilled);
		}
		public void UnLoad()
		{
			StarverNPC.Release();
			ServerApi.Hooks.GameUpdate.Deregister(Starver.Instance, StarverNPC.DoUpDate);
			ServerApi.Hooks.NpcLootDrop.Deregister(Starver.Instance, OnDrop);
			Commands.ChatCommands.RemoveAll(cmd => cmd.HasAlias("snpc"));
			//ServerApi.Hooks.NpcKilled.Deregister(Starver.Instance, StarverNPC.OnNPCKilled);
		}
		#endregion
		#region OnDrop
		private static void OnDrop(NpcLootDropEventArgs args)
		{
			if (StarverNPC.NPCs[args.NpcArrayIndex] != null && StarverNPC.NPCs[args.NpcArrayIndex].Active)
			{
				args.Handled = StarverNPC.NPCs[args.NpcArrayIndex].OverrideRawDrop;
				StarverNPC.NPCs[args.NpcArrayIndex].Drop(args.NpcArrayIndex);
			}
		}
		#endregion
		#region Command
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
						var heli = StarverNPC.NPCs[idx] as ElfHeliEx;
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
		#endregion
	}
}
