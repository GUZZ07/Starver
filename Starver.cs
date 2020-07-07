using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Starvers
{
	#region Using Alias
	using Assembly = System.Reflection.Assembly;
	#endregion
	#region Using Namespaces
	using Enemies.Bosses;
	using PlayerBoosts;
	#endregion
	[ApiVersion(2, 1)]
	public class Starver : TerrariaPlugin
	{
		#region Fields
		public const string ConfigPath = "tshock//StarverConfig.json";
		public readonly static Color DamageColor = Color.Yellow;
		public const int MaxSkillSlot = 5;
		public const float SpearSpeed = 4.07f;

		private int timer;
		private Random rand;
		#endregion
		#region Properties
		public static Starver Instance
		{
			get;
			private set;
		}

		public StarverConfig Config
		{
			get;
			private set;
		}
		#region Managers
		public PlayerDataManager PlayerDatas
		{
			get;
			private set;
		}
		public PlayerManager Players
		{
			get;
			private set;
		}
		public SkillManager Skills
		{
			get;
			private set;
		}
		public BossManager Bosses
		{
			get;
			private set;
		}
		public ProjLaunchTaskManager ProjTasks
		{
			get;
			private set;
		}
		#endregion
		#region Plugin Infos
		public override string Name => nameof(Starver);
		public override string Description => nameof(Starver);
		public override string Author => "1413";
		#endregion
		#endregion
		#region Load & Unload
		public Starver(Main game) : base(game)
		{

		}
		public override void Initialize()
		{
			Instance = this;

			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
			{
				var path = Path.Combine("Nugets", args.Name.Substring(0, args.Name.IndexOf(",")) + ".dll");
				if (File.Exists(path))
				{
					return Assembly.LoadFrom(path);
				}
				return null;
			};
			rand = new Random();
			Config = StarverConfig.Read(ConfigPath);
			ProjTasks = new ProjLaunchTaskManager();
			Skills = new SkillManager();
			Bosses = new BossManager();
			Players = new PlayerManager(TShock.Players.Length);
			PlayerDatas = new PlayerDataManager(Config.StorageType);
			#region Test
#if false
			var data = new PlayerData(-444) { Level = 3 };
			var skills = new PlayerSkillData[5];
			skills[0] = new PlayerSkillData
			{
				ID = (byte)SkillIDs.LawAias,
				BindByProj = true,
				BindID = ProjectileID.Spear
			};
			data.SetSkillDatas(skills);
			PlayerDatas.SaveData(data);
#endif
			#endregion
			#region Hooks
			ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
			ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
			ServerApi.Hooks.NpcStrike.Register(this, OnNpcStrike);
			ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
			ServerApi.Hooks.NetGetData.Register(this, OnGetData);
			TShockAPI.Hooks.PlayerHooks.PlayerPostLogin += OnPostLogin;
			GetDataHandlers.NewProjectile += OnNewProjectile;
			#endregion
			#region Commands
			Commands.ChatCommands.Add(new Command(Perms.Normal, MainCommand, "starver"));
			Commands.ChatCommands.Add(new Command(Perms.Aura.Normal, AuraCommand, "aura", "au"));
			Commands.ChatCommands.Add(new Command(Perms.Boss.Spawn, BossCommand, "stboss"));
			#endregion
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			#region Hooks
			ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
			ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);
			ServerApi.Hooks.NpcStrike.Deregister(this, OnNpcStrike);
			ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
			ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
			TShockAPI.Hooks.PlayerHooks.PlayerPostLogin -= OnPostLogin;
			GetDataHandlers.NewProjectile -= OnNewProjectile;
			#endregion
		}

		#endregion
		#region Events
		#region NewProj
		private void OnNewProjectile(object sender, GetDataHandlers.NewProjectileEventArgs args)
		{
			var player = Players[args.Player.Index];
			player.OnNewProj(args);
		}
		#endregion
		#region PostLogin
		private void OnPostLogin(PlayerPostLoginEventArgs args)
		{
			Players[args.Player.Index] = new StarverPlayer(args.Player.Index);
		}
		#endregion
		#region Join & Leave
		private void OnJoin(JoinEventArgs args)
		{
			if (!TShock.Players[args.Who].IsLoggedIn)
			{
				Players[args.Who] = StarverPlayer.GetGuest(args.Who);
			}
		}
		private void OnLeave(LeaveEventArgs args)
		{
			Players[args.Who]?.OnLeave();
			Players[args.Who] = null;
		}
		#endregion
		#region Update
		private void OnUpdate(EventArgs args)
		{
			timer++;
			Players.Update();
			Bosses.Update();
			ProjTasks.Update();
			Skills.Update();
		}
		#endregion
		#region NPCStrike
		private void OnNpcStrike(NpcStrikeEventArgs args)
		{
			var player = Players[args.Player.whoAmI];
			player.OnStrikeNpc(args);
		}
		#endregion
		#region GetData
		private void OnGetData(GetDataEventArgs args)
		{
			var player = Players[args.Msg.whoAmI];
			if (player != null && player.TSPlayer.IsLoggedIn)
			{
				player.OnGetData(args);
			}
		}
		#endregion
		#endregion
		#region Commands
		private void MainCommand(CommandArgs args)
		{
			var option = string.Empty;
			if (args.Parameters.Count > 0)
			{
				option = args.Parameters[0].ToLower();
			}
			switch (option)
			{
				case "exception":
					if (!args.Player.HasPermission(Perms.Test))
					{
						goto default;
					}
					new Thread(() => throw new Exception()).Start();
					break;
				case "up":
					if (args.Player is TSServerPlayer)
					{
						args.Player.SendInfoMessage("服务器无法使用该命令");
					}
					else
					{
						var player = Players[args.Player.Index];
						player.TryUpgrade();
						player.SendText($"当前等级: {player.Level}\n当前经验: {player.Exp}", Color.Yellow);
					}
					break;
				default:
					args.Player.SendInfoMessage("up    升级");
					args.Player.SendInfoMessage("help  帮助");
					break;
			}
		}
		private void AuraCommand(CommandArgs args)
		{
			var option = string.Empty;
			if (args.Parameters.Count > 0)
			{
				option = args.Parameters[0].ToLower();
			}
			#region FindSkill
			StarverSkill FindSkill(string nameOrID)
			{
				StarverSkill skill = null;
				if (int.TryParse(nameOrID, out int skillID))
				{
					if (!skillID.InRange(0, StarverSkill.Count - 1))
					{
						args.Player.SendErrorMessage($"无效的技能ID: {skillID}");
						return null;
					}
					skill = Skills[skillID];
				}
				else
				{
					skill = Skills.FirstOrDefault(skill => skill.Name.StartsWith(nameOrID, StringComparison.OrdinalIgnoreCase));
					if (skill == null)
					{
						args.Player.SendErrorMessage($"找不到技能: {nameOrID}");
						args.Player.SendErrorMessage("使用/au list [页码] 以查看技能列表");
						return null;
					}
				}
				return skill;
			}
			#endregion
			switch (option)
			{
				case "unbind":
					{
						#region CheckParamsCount
						if (args.Parameters.Count < 2)
						{
							goto default;
						}
						#endregion
						#region CheckSlotValid
						if (!int.TryParse(args.Parameters[1], out int slot) || !slot.InRange(1, MaxSkillSlot))
						{
							args.Player.SendErrorMessage($"无效的slot: {args.Parameters[1]}");
							args.Player.SendErrorMessage("应为1/2/3/4/5");
							break;
						}
						slot--;
						#endregion
						var player = Players[args.Player.Index];
						player.UnBind(slot);
					}
					break;
				case "bind":
					{
						#region CheckParamsCount
						if (args.Parameters.Count < 3)
						{
							goto default;
						}
						#endregion
						#region CheckSlotValid
						if (!int.TryParse(args.Parameters[1], out int slot) || !slot.InRange(1, MaxSkillSlot))
						{
							args.Player.SendErrorMessage($"无效的slot: {args.Parameters[1]}");
							args.Player.SendErrorMessage("应为1/2/3/4/5");
							break;
						}
						slot--;
						#endregion
						#region FindSkill
						var skill = FindSkill(args.Parameters[2]);
						if (skill == null)
						{
							break;
						}
						#endregion
						#region CheckSkill
						var player = Players[args.Player.Index];
						if (!skill.CanSet(player))
						{
							break;
						}
						if (skill.ID != player.Skills[slot].ID && player.Skills.Count(skl => skl.ID == skill.ID) != 0)
						{
							player.SendErrorText("你已经绑定了该技能");
							return;
						}
						#endregion
						#region CheckBinding
						switch (player.HeldItem.useStyle)
						{
							// 长矛
							case ItemUseStyleID.Shoot when player.HeldItem.noUseGraphic:
								player.BindSkill(slot, skill, true, player.HeldItem.shoot);
								break;
							// 射出类
							case ItemUseStyleID.Shoot:
								player.BindSkill(slot, skill, false, player.HeldItem.type);
								break;
							// 短剑
							case ItemUseStyleID.Rapier:
								player.BindSkill(slot, skill, false, player.HeldItem.type);
								break;
							// 剑气啊火花魔杖啊什么的
							case ItemUseStyleID.Swing when player.HeldItem.shoot != 0:
								player.BindSkill(slot, skill, true, player.HeldItem.shoot);
								break;
							default:
								if (skill.SpecialBindTo(player))
								{
									break;
								}
								args.Player.SendErrorMessage("该武器无法绑定技能");
								return;
						}
						args.Player.SendMessage(skill.Name, Color.Aqua);
						args.Player.SendMessage(skill.Introduction, Color.Aqua);
						#endregion
					}
					break;
				case "list":
					#region ListSkill
					if (args.Parameters.Count < 2 || !int.TryParse(args.Parameters[1], out int page))
					{
						args.Player.SendInfoMessage("用法:  /au list [页码]");
						break;
					}
					page = Math.Max(1, page);
					page = Math.Min(page, Skills.SkillLists.Length);
					args.Player.SendSuccessMessage("当前页: {0}/{1}", page, Skills.SkillLists.Length);
					args.Player.SendInfoMessage(Skills.SkillLists[page - 1]);
					break;
				#endregion
				#region Sounds
				//case "sounds":
				//	Task.Run(() =>
				//	{
				//		try
				//		{
				//			for (int i = 0; i < SoundID.TrackableLegacySoundCount; i++)
				//			{
				//				var sound = new NetMessage.NetSoundInfo(args.TPlayer.Center, (ushort)i);
				//				NetMessage.PlayNetSound(sound);
				//				args.Player.SendInfoMessage($"soundid: {i}");
				//				Thread.Sleep(5000);
				//			}
				//		}
				//		catch (Exception)
				//		{

				//		}
				//	});
				//	break;
				#endregion
				case "help":
					#region Help
					{
						if (args.Parameters.Count < 2)
						{
							goto default;
						}
						var skill = FindSkill(args.Parameters[1]);
						if (skill == null)
						{
							goto default;
						}
						args.Player.SendMessage(skill.Name, Color.Aqua);
						args.Player.SendMessage(skill.Introduction, Color.Aqua);
					}
					break;
				#endregion
				default:
					args.Player.SendInfoMessage("bind <slot> <skill> 将制定技能绑定到手中的武器上");
					args.Player.SendInfoMessage("unbind <slot> 技能解绑");
					args.Player.SendInfoMessage("list  查看技能列表");
					args.Player.SendInfoMessage("help [技能id]  查看技能介绍");
					args.Player.SendInfoMessage("help  帮助");
					break;
			}
		}
		private void BossCommand(CommandArgs args)
		{
			if (args.Parameters.Count == 0 || !int.TryParse(args.Parameters[0], out int index))
			{
				for (int i = 0; i < Bosses.Count; i++)
				{
					args.Player.SendInfoMessage($"{i + 1} {Bosses[i]}");
				}
				return;
			}
			index--;
			if (args.Parameters.Count < 2 || !int.TryParse(args.Parameters[1], out int level))
			{
				level = StarverBoss.DefaultLevel;
			}
			Bosses[index].Spawn(args.TPlayer.Center + rand.NextVector2(16 * 50), level);
		}
		#endregion
	}
}
