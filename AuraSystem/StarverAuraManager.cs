using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using Terraria.ID;
using TShockAPI;
using Starvers.AuraSystem.Realms;
using Starvers.AuraSystem.Realms.Generics;
using Starvers.AuraSystem.Realms.Interfaces;
using Starvers.AuraSystem.Realms.Conditioners;

namespace Starvers.AuraSystem
{
	public class StarverAuraManager : IStarverPlugin
	{
		#region Fields
		private int timer;
		[Obsolete("魔界之门已废弃", true)]
		internal static int EvilGateSpawnCountDown;
		[Obsolete("魔界之门已废弃", true)]
		internal dynamic Stellaria;
		[Obsolete("魔界之门已废弃", true)]
		internal Vector2[] LastPos;
		private Command AuraCommand;
		private Command TestCommand;
		private string[] SkillLists;
		private Queue<IRealm> TheRealms;
		private List<Type> RealmTypes;
		private string[] RealmNames;
		#endregion
		#region Properties
		private static StarverConfig Config => StarverConfig.Config;
		public bool Enabled => Config.EnableAura;
		public static AuraSkillWeapon[] SkillSlot { get; private set; } = new AuraSkillWeapon[Skills.Base.Skill.MaxSlots]
		{
			new AuraSkillWeapon(ItemID.Spear,ProjectileID.Spear,100),
			new AuraSkillWeapon(ItemID.Trident,ProjectileID.Trident,800),
			new AuraSkillWeapon(ItemID.Swordfish,ProjectileID.Swordfish,4000),
			new AuraSkillWeapon(ItemID.DarkLance,ProjectileID.DarkLance,8000),
			new AuraSkillWeapon(ItemID.ObsidianSwordfish,ProjectileID.ObsidianSwordfish,30000)
		};
		#endregion
		#region I & D
		public void Load()
		{
			LoadVars();
			LoadHandlers();
			LoadSkillList();
			LoadCommands();
		}
		public void UnLoad()
		{
			UnLoadHandlers();
			UnLoadSkillList();
			UnLoadCommands();
		}
		#endregion
		#region Loads
		private void LoadVars()
		{
#if false
			{
				if (Config.EnableTask)
					EvilGateSpawnCountDown = 60 * 9;
				try
				{
					Stellaria = ServerApi.Plugins.Where(container => container.Plugin.Name == nameof(Stellaria)).First().Plugin;
				}
				catch (Exception)
				{
					Console.WriteLine("Aura 寻找 Stellaria失败");
				}
				try
				{
					LastPos = Stellaria.LastPos;
				}
				catch
				{
					Console.WriteLine("Aura 需要使用魔改过的 Stellaria");
				}
			}
#endif
			SkillManager.LoadSkills();
			TheRealms = new Queue<IRealm>();
			RealmTypes = new List<Type>
			{
				typeof(ApoptoticRealm),
				typeof(BlindingRealm),
				typeof(ReflectingRealm),
				typeof(ReflectingRealm<EllipseReflector>),
				typeof(BlindingRealm<EllipseConditioner>),
				typeof(ReflectingRealm<CircleReflector>),
			};
			{
				RealmNames = new string[RealmTypes.Count];
				for (int i = 0; i < RealmNames.Length; i++)
				{
					string joined = string.Empty;
					if (RealmTypes[i].IsGenericType)
					{
						var arr = RealmTypes[i].GetGenericArguments().Select(type =>
						{
							return type.Name;
						});
						joined = $"<{string.Join(", ", arr)}>";
					}
					RealmNames[i] = $"{RealmTypes[i].Name}{joined}";
				}
			}
		}
		private void LoadHandlers()
		{
			ServerApi.Hooks.GameUpdate.Register(Starver.Instance, OnUpdate);

			GetDataHandlers.NewProjectile += OnProj;
		}
		private void LoadSkillList()
		{
			int line = 5;
			int column = 4;
			int page = (int)Math.Ceiling((float)SkillManager.Skills.Length / column / line);
			StringBuilder SB = new StringBuilder(10 * 4 * 5);
			SkillLists = new string[page];
			int idx;
			for (int i = 0; i < page; i++)
			{
				SB.AppendLine($"技能列表({i + 1}/{page}):");
				for (int j = 0; j < line; j++)
				{
					for (int k = 0; k < column; k++)
					{
						idx = k + j * column + i * column * line;
						if (idx < SkillManager.Skills.Length)
						{
							SB.Append($"{SkillManager.Skills[idx].Name}    ");
						}
						else
						{
							break;
						}
					}
					SB.AppendLine();
				}
				SB.Length -= 1;
				SkillLists[i] = SB.ToString();
				SB.Clear();
			}
		}
		private void LoadCommands()
		{
			AuraCommand = new Command(Perms.Aura.Normal, Command, "au", "aura");
			TestCommand = new Command(Perms.Test, CommandForTest, "realm");

			Commands.ChatCommands.Add(AuraCommand);
			Commands.ChatCommands.Add(TestCommand);
		}
		#endregion
		#region UnLoads
		private void UnLoadHandlers()
		{
			ServerApi.Hooks.GameUpdate.Deregister(Starver.Instance, OnUpdate);

			GetDataHandlers.NewProjectile -= OnProj;
		}
		private void UnLoadSkillList()
		{
			SkillLists = null;
		}
		private void UnLoadCommands()
		{
			Commands.ChatCommands.Remove(AuraCommand);
			Commands.ChatCommands.Remove(TestCommand);
		}
		#endregion
		#region Realms
		public void AddRealm(IRealm realm)
		{
			realm.Start();
			TheRealms.Enqueue(realm);
		}
		private void UpdateRealms()
		{
			int count = TheRealms.Count;
			IRealm realm;
			for (int i = 0; i < count; i++)
			{
				realm = TheRealms.Dequeue();
				realm.Update();
				if (realm.Active)
				{
					TheRealms.Enqueue(realm);
				}
			}
		}
		#region UpdateEvilGate
		[Obsolete("魔界之门已废弃", true)]
		private void UpdateEvilGate()
		{
			bool canSpawn = NPC.downedMoonlord || Starver.Config.EvilWorld;
			if (canSpawn && EvilGateSpawnCountDown > 0)
			{
				if (--EvilGateSpawnCountDown != 0)
					return;
				var gate = new GateOfEvil<CircleConditioner>
				{
					Center = new Vector2(Main.spawnTileX, Main.spawnTileY - 100) * 16,
					NewBySystem = true
				};
				AddRealm(gate);
			}
		}
		#endregion
		#endregion
		#region OperateRealms
		public void OperateRealms(Action<IRealm> operation)
		{
			TheRealms.ForEach(operation);
		}
		#endregion
		#region Hooks
		#region OnUpdate
		private void OnUpdate(object args)
		{
			timer++;
			UpdateRealms();
#if false
			if (Config.EnableTask && Config.EvilWorld || NPC.downedMoonlord)
			{
				UpdateEvilGate();
			}
#endif
		}
		#endregion
		#region OnProj
		private void OnProj(object sender, GetDataHandlers.NewProjectileEventArgs args)
		{
			if (args.Type == ProjectileID.RocketII || args.Type == ProjectileID.RocketIV || args.Type == ProjectileID.RocketSnowmanII || args.Type == ProjectileID.RocketSnowmanIV)
			{
				Main.projectile[args.Index].active = false;
				return;
			}
			if ((!Main.projectile[args.Index].friendly) && Main.projectile[args.Index].damage <= 300)
			{
				Main.projectile[args.Index].damage = (int)(Config.TaskNow / 4D);
				NetMessage.SendData((int)PacketTypes.ProjectileNew, -1, -1, null, args.Index);
				return;
			}
			int slot = SkillManager.GetSlot(args.Type);
			if (slot > 0 && slot < 6 && (DateTime.Now - Starver.Players[args.Owner].LastHandle).TotalSeconds > 1)
			{
				Starver.Players[args.Owner].LastHandle = DateTime.Now;
				SkillManager.Handle(Starver.Players[args.Owner], args.Velocity, slot);
			}
		}
		#endregion
		#endregion
		#region Command
		private void CommandForTest(CommandArgs args)
		{
			int value;
			try
			{
				value = int.Parse(args.Parameters[0]);
			}
			catch
			{
				value = 0;
			}
			if (value == 0)
			{
				for (int i = 0; i < RealmTypes.Count; i++)
				{
					args.Player.SendInfoMessage($"{i + 1}: {RealmNames[i]}");
				}
				return;
			}
			else
			{
				IRealm realm;
				switch (value)
				{
					case 3:
						{
							int owner;
							try
							{
								owner = int.Parse(args.Parameters[1]);
							}
							catch
							{
								owner = 255;
							}
							var Realm = new ReflectingRealm(owner)
							{
								Center = args.TPlayer.Center
							};
							if (args.Parameters.Count >= 3 && int.TryParse(args.Parameters[2], out int timeLeft))
							{
								Realm.DefaultTimeLeft = timeLeft;
							}
							realm = Realm;
							break;
						}
					case 4:
						{
							int owner;
							try
							{
								owner = int.Parse(args.Parameters[1]);
							}
							catch
							{
								owner = 255;
							}
							var Realm = new ReflectingRealm<EllipseReflector>(owner)
							{
								Center = args.TPlayer.Center
							};
							if (args.Parameters.Count >= 3 && int.TryParse(args.Parameters[2], out int timeLeft))
							{
								Realm.DefaultTimeLeft = timeLeft;
							}
							realm = Realm;
							break;
						}
					default:
						{
							realm = Activator.CreateInstance(RealmTypes[value - 1]) as IRealm;
							realm.Center = args.TPlayer.Center;
							break;
						}
				}
				AddRealm(realm);
			}
		}
		private void Command(CommandArgs args)
		{
			string p = args.Parameters.Count < 1 ? "None" : args.Parameters[0];
			StarverPlayer player = args.SPlayer();
			switch (p.ToLower())
			{
				#region up
				case "up":
					int exp = player.Exp;
					int lvl = player.Level;
					int need = UpGradeExp(lvl);
					if (player.HasPerm(Perms.VIP.LessCost))
					{
						need /= 3;
						while (exp > need)
						{
							exp -= need;
							need = UpGradeExp(++lvl) / 3;
						}
					}
					else
					{
						while (exp > need)
						{
							exp -= need;
							need = UpGradeExp(++lvl);
						}
					}
					player.Level = lvl;
					player.Exp = exp;
					player.Save();
					player.SendInfoMessage("当前等级:{0}", player.Level);
					player.SendInfoMessage("所需经验:{0}", UpGradeExp(player.Level));
					break;
				#endregion
				#region ForceUp
				case "forceup":
					if (!player.HasPerm(Perms.Aura.ForceUp))
					{
						goto default;
					}
					int up;
					try
					{
						up = int.Parse(args.Parameters[1]);
					}
					catch
					{
						up = 1;
					}
					player.Level += up;
					player.SendInfoMessage("当前等级:{0}", player.Level);
					player.Save();
					break;
				#endregion
				#region toexp
				case "toexp":
					Item item = player.TPlayer.inventory[0];
					int bagexp;
					try
					{
						bagexp = item.stack * BagExp(item.type);
						item.netDefaults(0);
						if (bagexp == 0)
						{
							throw new Exception();
						}
						player.Exp += bagexp;
						player.SendData(PacketTypes.PlayerSlot, "", player.Index, 0);
						player.Save();
					}
					catch
					{
						player.SendMessage("请将可兑换为经验的物品(主要为boss袋子)放置在背包第一格", Color.Red);
					}
					break;
				#endregion
				#region Setlvl
				case "setlvl":
					if (!player.HasPerm(Perms.Aura.SetLvl))
					{
						goto default;
					}
					try
					{
						player.Level = int.Parse(args.Parameters[1]);
						player.SendInfoMessage("设置成功");
						player.SendInfoMessage("当前等级:{0}", player.Level);
					}
					catch
					{
						player.SendMessage("正确用法:	/aura setlvl <level>", Color.Red);
					}
					break;
				#endregion
				#region list
				case "list":
					{
						int page = 1;
						if (args.Parameters.Count > 1)
						{
							if (!int.TryParse(args.Parameters[1], out page))
							{
								page = 1;
							}
						}
						page -= 1;
						if (page < 0)
						{
							page = 0;
						}
						else if (page >= SkillLists.Length)
						{
							page = SkillLists.Length - 1;
						}
						player.SendInfoMessage(SkillLists[page]);
					}
					break;
				#endregion
				#region buy
				case "buy":
					try
					{
						need = int.Parse(args.Parameters[1]) - 1;
						if (player.Exp >= SkillSlot[need].Cost)
						{
							player.GiveItem(SkillSlot[need].Item);
							player.Exp -= SkillSlot[need].Cost;
							player.SendMessage("购买成功", new Color(0, 0x80, 0));
						}
						else
						{
							player.SendMessage("你需要更多的经验来获取该技能物品", Color.Red);
						}
					}
					catch
					{
						player.SendMessage("应输入正确的武器序号:{1 ,2, 3, 4, 5}中的一个", Color.Red);
					}
					break;
				#endregion
				#region Set
				case "set":
					{
						int slot;
						if (args.Parameters.Count < 3 || !int.TryParse(args.Parameters[1], out slot))
						{
							player.SendInfoMessage("格式错误");
							player.SendInfoMessage("正确用法:	set <slot> <skilltype>");
						}
						else
						{
							player.SetSkill(args.Parameters[2], slot);
						}
						break;
					}
				#endregion
				#region CDLess
				case "cd":
					if (!player.HasPerm(Perms.Aura.IgnoreCD))
					{
						goto default;
					}
					player.ForceIgnoreCD ^= true;
					player.SendInfoMessage($"ForceIgnoreCD: {player.ForceIgnoreCD}");
					break;
				#endregion
				#region help
				case "help":
					try
					{
						int id = int.Parse(args.Parameters[1]);
						args.Player.SendMessage(SkillManager.Skills[id].Name, Color.Aqua);
						args.Player.SendMessage(SkillManager.Skills[id].Text, Color.Aqua);
					}
					catch
					{
						if (args.Parameters.Count >= 2)
						{
							string sklname = args.Parameters[1].ToLower();
							foreach (var skl in SkillManager.Skills)
							{
								if (skl.Name.ToLower().IndexOf(sklname) == 0)
								{
									args.Player.SendMessage(skl.Name, Color.Aqua);
									args.Player.SendMessage(skl.Text, Color.Aqua);
									return;
								}
							}
							args.Player.SendErrorMessage("技能名称错误");
							args.Player.SendErrorMessage(" list:  查看技能列表");
							break;
						}
						goto default;
					}
					break;
				#endregion
				#region default
				default:
					player.SendInfoMessage(HelpTexts.Aura);
					if (player.HasPerm(Perms.Aura.ForceUp))
					{
						player.SendInfoMessage("    forceup <levelup>:强制升级");
					}
					if (player.HasPerm(Perms.Aura.SetLvl))
					{
						player.SendInfoMessage("    setlvl <level>:设置等级");
					}
					break;
					#endregion
			}
		}
		#endregion
		#region Calculates
		#region NPCDefense
		internal static int NPCDefense(int raw)
		{
			float scale = Config.TaskNow + 1;
			return Convert.ToInt32(raw * scale);
		}
		#endregion
		#region NPCDamage
		public static int NPCDamage(int raw)
		{
			if (Config.TaskNow < 18)
			{
				return (int)(raw * Config.TaskNow / 5f + 1);
			}
			else
			{
				return (int)(raw * Config.TaskNow / 3f + 1);
			}
		}
		#endregion
		#region NPCLife
		internal static int NPCLife(int raw, bool isboss = false)
		{
			float scale = 1;
			scale += Config.TaskNow * 2.75f;
			float now = raw * scale;
			if (!isboss)
			{
				now = Math.Min(short.MaxValue, now);
			}
			raw = Convert.ToInt32(now);
			return raw;
		}
		#endregion
		#region UpgradeExp
		internal static int UpGradeExp(int lvl)
		{
			if (lvl < 1000)
			{
				return (int)(lvl * 10.5f);
			}
			else if (lvl < 2500)
			{
				return (int)(lvl * 15.5f);
			}
			else if (lvl < (int)1e4)
			{
				return Math.Min((int)6.25e6f, (int)(lvl * Math.Log(lvl)));
			}
			else if (lvl < (int)1e5)
			{
				return Math.Min((int)5.1e5f, (int)(lvl * 10 * Math.Log(lvl)));
			}
			else
			{
				return (int)1e6f;
			}
		}
		#endregion
		#region BagExp
		internal static int BagExp(int id)
		{
			int exp = 0;
			if (id == ItemID.CultistBossBag)
			{
				exp = (int)2e9;
			}
			if (id >= ItemID.KingSlimeBossBag && id <= ItemID.MoonLordBossBag)
			{
				exp = (id + 11 - ItemID.KingSlimeBossBag);
				exp *= exp;
				exp *= exp;
			}
			else if (id == ItemID.BossBagBetsy)
			{
				exp = BagExp(ItemID.FishronBossBag);
			}
#if false
			else if (id >= ItemID.WhiteCultistArcherBanner && id <= ItemID.WhiteCultistFighterBanner)
			{
				id = id - ItemID.WhiteCultistArcherBanner + 2;
				exp = 1000000 * id;
			}
#endif
			return exp;
		}
		#endregion
		#endregion
	};
}
