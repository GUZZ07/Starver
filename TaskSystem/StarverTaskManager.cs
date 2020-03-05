using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Starvers.TaskSystem.Branches;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace Starvers.TaskSystem
{
	public class StarverTaskManager : IStarverPlugin
	{
		#region Fields
		private int Timer;
		private bool LoadedTask;
		private string TasksPath;
		private string ConfigHelpPath;
		private string ItemIDsPath;
		public const int MainLineCount = 43;
		#endregion
		#region Properties
		public bool Enabled => Config.EnableTask;
		public MainLineTask[] MainLine { get; private set; } = new MainLineTask[MainLineCount];
		public BranchLine[] BranchTaskLines { get; } = new BranchLine[]
		{
			null,
			new TestLine1(),
			new YrtAEvah(),
			new StoryToContinue()
		};
		public ITask CurrentTask => MainLine[Config.TaskNow];

		private static StarverConfig Config => StarverConfig.Config;
		#endregion
		#region I & D
		public void Load()
		{
			Commands.ChatCommands.Add(new Command(Perms.Task.Normal, MainCommand, "task", "tsks") { HelpText = HelpTexts.Task });
			ServerApi.Hooks.GameUpdate.Register(Starver.Instance, OnGameUpdate);
			ServerApi.Hooks.GamePostInitialize.Register(Starver.Instance, OnInitialized);

			TasksPath = Path.Combine(TShock.SavePath, "StarverTasks.json");
			ConfigHelpPath = Path.Combine(TShock.SavePath, "关于StarverTasks.json.txt");
			ItemIDsPath = Path.Combine(TShock.SavePath, "ItemIDs.txt");
		}
		public void UnLoad()
		{
			ServerApi.Hooks.GameUpdate.Deregister(Starver.Instance, OnGameUpdate);
			ServerApi.Hooks.GamePostInitialize.Deregister(Starver.Instance, OnInitialized);
		}
		#endregion
		#region OnInitialized
		private void OnInitialized(EventArgs args)
		{
			LoadTasks();
		}
		#endregion
		#region OnUpdate
		private void OnGameUpdate(EventArgs args)
		{
			//LoadTasks();
			Timer++;
			if (Timer % 60 == 0)
			{
				#region ClearNPC
				foreach (NPC npc in Main.npc)
				{
					if (!npc.active)
					{
						continue;
					}
					bool EatThis = false;
					switch (npc.type)
					{
						case NPCID.KingSlime:
							if (Config.TaskNow < 1)
							{
								EatThis = true;
							}
							break;
						case NPCID.EyeofCthulhu:
							if (Config.TaskNow < 2)
							{
								EatThis = true;
							}
							break;
						case NPCID.EaterofWorldsHead:
						case NPCID.EaterofWorldsBody:
						case NPCID.EaterofWorldsTail:
						case NPCID.BrainofCthulhu:
							if (Config.TaskNow < 3)
							{
								EatThis = true;
							}
							break;
						case NPCID.DD2EterniaCrystal:
							if (Config.TaskNow < 4)
							{
								EatThis = true;
							}
							else if (Config.TaskNow >= 10 && Config.TaskNow < 13)
							{
								EatThis = true;
							}
							else if (Config.TaskNow == 15)
							{
								EatThis = true;
							}
							break;
						case NPCID.QueenBee:
							if (Config.TaskNow < 5)
							{
								EatThis = true;
							}
							break;
						case NPCID.SkeletronHead:
							if (Config.TaskNow < 6)
							{
								EatThis = true;
							}
							break;
						case NPCID.WallofFlesh:
							if (Config.TaskNow < 7)
							{
								EatThis = true;
							}
							break;
						case NPCID.Retinazer:
						case NPCID.Spazmatism:
							if (Config.TaskNow < 10)
							{
								EatThis = true;
							}
							break;
						case NPCID.SkeletronPrime:
							if (Config.TaskNow < 11)
							{
								EatThis = true;
							}
							break;
						case NPCID.TheDestroyer:
							if (Config.TaskNow < 12)
							{
								EatThis = true;
							}
							break;
						case NPCID.Plantera:
							if (Config.TaskNow < 14)
							{
								EatThis = true;
							}
							break;
						case NPCID.Golem:
						case NPCID.GolemHead:
							if (Config.TaskNow < 15)
							{
								EatThis = true;
							}
							break;
						case NPCID.CultistBoss:
							if (Config.TaskNow < 17)
							{
								EatThis = true;
							}
							break;
						case NPCID.LunarTowerNebula:
							if (Config.TaskNow < 18)
							{
								EatThis = true;
							}
							break;
						case NPCID.LunarTowerStardust:
							if (Config.TaskNow < 19)
							{
								EatThis = true;
							}
							break;
						case NPCID.LunarTowerVortex:
							if (Config.TaskNow < 20)
							{
								EatThis = true;
							}
							break;
						case NPCID.MoonLordCore:
							if (Config.TaskNow < 21)
							{
								EatThis = true;
							}
							break;
					}
					if (EatThis)
					{
						npc.active = false;
						NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, null, npc.whoAmI);
					}
				}
				#endregion
				#region MoonLordCountDown
				if (Config.TaskNow < 21 && NPC.MoonLordCountdown > 0)
				{
					NPC.MoonLordCountdown = 0;
					TSPlayer.All.SendData(PacketTypes.MoonLordCountdown, "", 0);
				}
				#endregion
			}
		}
		#endregion
		#region Finish
		private void Finish(bool flag, bool force = false)
		{
			flag |= force;
			if (!flag)
			{
				TSPlayer.All.SendErrorMessage("条件未达成,检查失败");
			}
			else
			{
				TSPlayer.All.SendSuccessMessage($"主线任务#{Config.TaskNow + 1}已完成");
				if (!force)
				{
					TSPlayer.All.SendMessage("物品奖励详见箱子", Color.SkyBlue);
				}
				else
				{
					TSPlayer.All.SendMessage("强制完成任务不会得到任何奖励", Color.SkyBlue);
				}
				Config.TaskNow++;
				Config.Write();
			}
		}
		#endregion
		#region LoadTask
		private void LoadTasks()
		{
			if (LoadedTask)
				return;
			if (!TryLoadFromJSON())
			{
				StarverPlayer.Server.SendErrorMessage("从Json中加载任务失败, 已使用默认任务");
				LoadNewTask();
			}
			SaveTaskToJSON();
			LoadedTask = true;
		}
		private void ReLoadTasks()
		{
			if (!LoadedTask)
			{
				LoadTasks();
				return;
			}
			if (!TryLoadFromJSON())
			{
				StarverPlayer.Server.SendErrorMessage("从Json中加载任务失败, 仍然使用当前任务");
			}
			else
			{
				SaveTaskToJSON();
			}
			LoadedTask = true;
		}
		#endregion
		#region LoadNewTask
		private bool TryLoadFromJSON()
		{
			if (!File.Exists(TasksPath))
			{
				return false;
			}
			try
			{
				string text = File.ReadAllText(TasksPath);
				MainLineTaskData[] datas = JsonConvert.DeserializeObject<MainLineTaskData[]>(text);
				if (datas.Length != MainLine.Length)
					return false;
				for (int i = 0; i < MainLine.Length; i++)
				{
					MainLine[i] = new MainLineTask(i + 1, Config.TaskLevel, datas[i]);
				}
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return false;
			}
		}
		private void LoadNewTask()
		{
			for (int i = 0; i < MainLineCount; i++)
			{
				MainLine[i] = new MainLineTask(i + 1, Config.TaskLevel);
			}
		}
		#endregion
		#region SaveTaskToJSON
		private void SaveTaskToJSON()
		{
			var datas = new MainLineTaskData[MainLine.Length];
			for (int i = 0; i < datas.Length; i++)
			{
				datas[i] = MainLine[i].ToDatas();
			}
			string text = JsonConvert.SerializeObject(datas, Formatting.Indented);
			File.WriteAllText(TasksPath, text);
			File.WriteAllText(ConfigHelpPath, MainLineTaskData.ReadMe);
			File.WriteAllText(ItemIDsPath, MainLineTaskData.ItemIDs);
		}
		#endregion
		#region Commands
		private void MainCommand(CommandArgs args)
		{
			string p = args.Parameters.Count < 1 ? "None" : args.Parameters[0];
			var player = args.SPlayer();
			//LoadTasks();
			switch (p.ToLower())
			{
				#region Reload
				case "reload":
					if (args.Player.HasPermission(Perms.Task.Reload))
					{
						try
						{
							ReLoadTasks();
						}
						catch (Exception e)
						{
							args.Player.SendErrorMessage("重新加载任务失败");
							args.Player.SendErrorMessage(e.ToString());
							break;
						}
						args.Player.SendInfoMessage("任务已成功重新加载");
					}
					else
					{
						goto default;
					}
					break;
				#endregion
				#region Check
				case "check":
					if (Config.TaskNow >= Config.TaskLock)
					{
						player.SendInfoMessage("当前主线任务已完结");
					}
					else
					{
						bool flag = MainLine[Config.TaskNow].Check(player);
						Finish(flag);
					}
					break;
				#endregion
				#region FFF
				case "fff":
					if (args.Player.HasPermission(Perms.Task.FFF))
					{
						StarverPlayer.All.SendMessage(player.Name + "强制完成了当前任务", Color.Blue);
						Finish(false, true);
					}
					else
					{
						goto default;
					}
					break;
				#endregion
				#region List
				case "list":
					if (Config.TaskNow < Config.TaskLock)
					{
						args.Player.SendInfoMessage(CurrentTask.Description);
					}
					else
					{
						args.Player.SendInfoMessage("当前任务已完结");
					}
					break;
				#endregion
				#region ListAll
				case "listall":
					if (!args.Player.HasPermission(Perms.Task.ListAll))
					{
						goto default;
					}
					else
					{
						foreach (MainLineTask task in MainLine)
						{
							player.SendInfoMessage(task.Description);
						}
					}
					break;
				#endregion
				#region Set
				case "set":
					if (!args.Player.HasPermission(Perms.Task.Set))
					{
						goto default;
					}
					else
					{
						try
						{
							int to = int.Parse(args.Parameters[1]);
							Config.TaskNow = to;
							if (Config.TaskNow > MainLineCount)
							{
								Config.TaskNow = MainLineCount;
							}
							else if (Config.TaskNow < 0)
							{
								Config.TaskNow = 0;
							}
							Config.Write();
							TSPlayer.All.SendSuccessMessage("{0}设置当前已完成任务至主线任务#{1}", player.Name, Config.TaskNow);
							TSPlayer.All.SendMessage(string.Format("要是完成任务后怪物太强玩家们打不过,尽管去捶{0}", player.Name), Color.Blue);
						}
						catch (Exception)
						{
							player.SendMessage("错误的用法!", Color.Red);
							player.SendMessage("请正确输入想要的任务序号", Color.Red);
							player.SendMessage("/Task set <任务序号>	设置当前任务为制定任务", Color.Red);
						}
					}
					break;
				#endregion
				#region Bt
				case "bt":
					{
						if (!args.Player.HasPermission(Perms.Task.BranchT))
						{
							goto default;
						}
						if (args.Parameters.Count < 2 || !int.TryParse(args.Parameters[1], out int line) || line >= BranchTaskLines.Length)
						{
							for (int i = 0; i < BranchTaskLines.Length; i++)
							{
								player.SendInfoMessage($"{i}: {BranchTaskLines[i]}");
							}
						}
						else if (args.Parameters.Count < 3 || !int.TryParse(args.Parameters[2], out int id))
						{
							for (int i = 0; i < BranchTaskLines[line].Count; i++)
							{
								player.SendInfoMessage($"{i}:  {BranchTaskLines[line][i].Description}");
							}
						}
						else
						{
							var (Success, Message) = BranchTaskLines[line].TryStartTask(player, id);
							if (!Success)
							{
								player.SendFailMessage("任务开启失败");
								player.SendFailMessage($"详细原因: {Message}");
							}
							else
							{
								player.SendCombatMsg($"任务开始: {player.BranchTask}", Color.Green);
								if (Message != null)
								{
									player.SendSuccessMessage(Message);
								}
							}
						}
						break;
					}
				#endregion
				#region Branch
				case "branch":
					{
						player.CheckBranchAvaiable();
						if (args.Parameters.Count > 1 && int.TryParse(args.Parameters[1], out int id))
						{
							if (0 <= id && id < (int)BLID.Max)
							{
								if (player.BLAvaiable((BLID)id))
								{
									if (player.bldata[(BLID)id] < BranchTaskLines[id].Count)
									{
										if(player.BranchTask != null)
										{
											player.SendFailMessage("请先完成当前任务");
											break;
										}
										(bool success,string msg) = BranchTaskLines[id].TryStartTask(player, player.bldata[(BLID)id]);
										if (success)
										{
											player.SendInfoMessage($"任务开始: {player.BranchTask}");
											if (msg != null)
											{
												player.SendSuccessMessage(msg);
											}
										}
										else
										{
											player.SendFailMessage("任务开启失败");
											player.SendFailMessage($"详细原因: {msg}");
										}
									}
									else
									{
										player.SendInfoMessage("你已经完成该支线");
									}
								}
								else
								{
									player.SendInfoMessage("当前支线尚未开启");
								}
								break;
							}
						}
						player.SendInfoMessage("已开启任务:");
						for (BLID i = (BLID)1; i < BLID.Max; i++)
						{
							if (player.BLAvaiable(i))
							{
								var branch = BranchTaskLines[(int)i];
								player.SendInfoMessage($"    {(int)i}: {branch}({player.bldata[i]} / {branch.Count})");
							}
						}
						break;
					}
				#endregion
				#region Bonus
				case "bonus":
					{
						player.ShowBonus();
						break;
					}
				#endregion
				#region SendHelpText
				default:
					player.SendInfoMessage(HelpTexts.Task);
					if (player.HasPerm(Perms.Task.ListAll))
					{
						player.SendInfoMessage("    listall    列出所有任务");
					}
					if (player.HasPerm(Perms.Task.FFF))
					{
						player.SendInfoMessage("    fff:     无视条件强制完成任务");
					}
					if (player.HasPerm(Perms.Task.Set))
					{
						player.SendInfoMessage("    set <任务序号>  设置当前任务为制定任务");
					}
					if (player.HasPerm(Perms.Task.Reload))
					{
						player.SendInfoMessage("    reload   重新加载任务");
					}
					if (player.HasPerm(Perms.Task.BranchT))
					{
						player.SendInfoMessage("    bt       branch line test");
					}
					break;
					#endregion
			}
		}
		#endregion
	}
}
