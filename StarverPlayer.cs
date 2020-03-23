using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using TShockAPI;
using MySql;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria.ID;
using Terraria.Localization;
using System.Runtime.InteropServices;

namespace Starvers
{
	using Events;
	using TaskSystem;
	using DB;
	using BigInt = System.Numerics.BigInteger;
	using Vector = TOFOUT.Terraria.Server.Vector2;
	using Skill = AuraSystem.Skills.Base.Skill;
	using System.Diagnostics;
	using TerrariaApi.Server;

	public partial class StarverPlayer
	{
		#region FromTS
		#region Heal
		public void Heal(int health = -1)
		{
			if (health == -1)
			{
				health = TPlayer.statLifeMax2;
			}
			TSPlayer.Heal(health);
		}
		#endregion
		#region GodMode
		public bool GodMode
		{
			get => TSPlayer.GodMode;
			set => TSPlayer.GodMode = value;
		}
		#endregion
		#region Buff
		public void AddBuffIfNot(int type, int time = 60 * 60)
		{
			if (!HasBuff(type))
			{
				SetBuff(type, time);
			}
		}
		public void SetBuff(int type, int time = 3600, bool bypass = false)
		{
			TSPlayer.SetBuff(type, time, bypass);
		}
		public bool HasBuff(int type)
		{
			return FindBuffIndex(type) != -1;
		}
		public void RemoveBuff(int type)
		{
			int idx = FindBuffIndex(type);
			if (idx != -1)
			{
				TPlayer.buffTime[idx] = 0;
				TPlayer.buffType[idx] = 0;
				SendData(PacketTypes.PlayerBuff, "", Index);
			}
		}
		public int FindBuffIndex(int type)
		{
			int idx = -1;
			for (int i = 0; i < TPlayer.buffType.Length; i++)
			{
				if (TPlayer.buffType[i] == type)
				{
					idx = i;
					break;
				}
			}
			return idx;
		}
		#endregion
		#region Sends
		/// <summary>
		/// 发送悬浮文字
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="color"></param>
		public void SendCombatMsg(string msg, Color color)
		{
			if (Index == -2)
			{
				Console.WriteLine(msg);
			}
			else if (Index >= 0)
			{
				TPlayer.SendCombatMsg(msg, color);
			}
		}
		public void SendMessage(string msg, byte R, byte G, byte B)
		{
			if (UserID == -2)
			{
				Console.WriteLine(msg);
			}
			else if (UserID == -1)
			{
				TSPlayer.All.SendMessage(msg, R, G, B);
			}
			else if (Index >= 0)
			{
				TSPlayer.SendMessage(msg, R, G, B);
				/*
				if (msg.Contains("\n"))
				{
					foreach (string msg2 in msg.Split(new char[]
					{
					'\n'
					}))
					{
						SendMessage(msg2, R, G, B);
					}
					return;
				}
				this.SendData(PacketTypes.SmartTextMessage, msg, 255, R, G, B, -1);
				*/
			}
			else
			{
				TSPlayer.SendInfoMessage(msg);
			}
		}
		public void SendSuccessMessage(string msg)
		{
			SendMessage(msg, Color.DarkGreen);
		}
		public void SendFailMessage(string msg)
		{
			SendMessage(msg, Color.Blue);
		}
		public void SendDeBugMessage(string msg, bool console = false)
		{
#if DEBUG
			SendMessage(msg, Color.Blue);
			if (console)
			{
				var fore = Console.ForegroundColor;
				var back = Console.BackgroundColor;
				Console.ForegroundColor = ConsoleColor.Blue;
				Console.BackgroundColor = ConsoleColor.Yellow;
				Console.WriteLine(msg);
				Console.ForegroundColor = fore;
				Console.BackgroundColor = back;
			}
#endif
		}
		public void SendMessage(string msg, Color color)
		{
			if (Index == -2)
			{
				Console.WriteLine(msg);
			}
			else if (Index >= -1)
			{
				SendMessage(msg, color.R, color.G, color.B);
			}
			else
			{
				TSPlayer.SendMessage(msg, color);
			}
		}
		public void SendInfoMessage(string msg)
		{
			if (Index == -2)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine(msg);
				Console.ResetColor();
			}
			else if (Index >= -1)
			{
				SendMessage(msg, Color.Yellow);
			}
			else
			{
				TSPlayer.SendInfoMessage(msg);
			}
		}
		public void SendErrorMessage(string msg)
		{
			if (Index == -2)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine(msg);
				Console.ResetColor();
			}
			else if (Index >= -1)
			{
				SendMessage(msg, Color.Red);
			}
			else
			{
				TSPlayer.SendInfoMessage(msg);
			}
		}
		public void SendInfoMessage(string msg, params object[] args)
		{
			if (Index == -2)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine(msg, args);
				Console.ResetColor();
			}
			else if (Index >= -1)
			{
				SendInfoMessage(string.Format(msg, args));
			}
			else
			{
				TSPlayer.SendInfoMessage(msg, args);
			}
		}
		public void SendData(PacketTypes msgType, string text = "", int number = 0, float number2 = 0, float number3 = 0, float number4 = 0, int number5 = 0)
		{
			NetMessage.SendData((int)msgType, Index, -1, NetworkText.FromLiteral(text), number, number2, number3, number4, number5);
		}
		private static string EndLine19 = new string('\n', 19);
		private static string EndLine20 = new string('\n', 20);
		public void SendStatusMSG(string msg)
		{
			msg = EndLine19 + msg + EndLine20;
			SendData(PacketTypes.Status, msg);
		}
		#endregion
		#region HasPerm
		public bool HasPerm(string perm)
		{
			return TSPlayer.HasPermission(perm);
		}
		#endregion
		#endregion
		#region FromTPlayer
		#region Others
		public int Team
		{
			get
			{
				return TPlayer.team;
			}
			set
			{
				TPlayer.team = value;
				SendData(PacketTypes.PlayerTeam, "", Index);
			}
		}
		public int LifeMax
		{
			get => TPlayer.statLifeMax;
			set => SetLifeMax(value);
		}
		public int ManaMax
		{
			get
			{
				return TPlayer.statManaMax;
			}
			set
			{
				TPlayer.statManaMax = value;
				SendData(PacketTypes.PlayerMana, "", Index);
			}
		}
		#endregion
		#region Center
		public Vector2 Center
		{
			get
			{
				return TPlayer.Center;
			}
			set
			{
				TPlayer.Center = value;
				SendData(PacketTypes.PlayerUpdate, "", Index);
			}
		}
		#endregion
		#region Position
		public Vector2 Position
		{
			get
			{
				return TPlayer.position;
			}
			set
			{
				TPlayer.position = value;
				SendData(PacketTypes.PlayerUpdate, "", Index);
			}
		}
		#endregion
		#region Velocity
		public Vector2 Velocity
		{
			get
			{
				return TPlayer.velocity;
			}
			set
			{
				TPlayer.velocity = value;
				SendData(PacketTypes.PlayerUpdate, "", Index);
			}
		}
		#endregion
		#endregion
		#region Properties
		#region Zones
		public bool ZoneDirtLayerHeight => TilePoint.Y <= Main.rockLayer && TilePoint.Y > Main.worldSurface;
		public bool ZoneBeach => ZoneOverworldHeight && (TilePoint.X < 380 || TilePoint.X > Main.maxTilesX - 380);
		public bool ZoneOverworldHeight => TilePoint.Y <= Main.worldSurface && TilePoint.Y > Main.worldSurface * 0.349999994039536;
		public bool ZoneRockLayerHeight => TilePoint.Y <= Main.maxTilesY - 200 && (double)TilePoint.Y > Main.rockLayer;
		public bool ZoneRain => Main.raining && TilePoint.Y <= Main.worldSurface;
		public bool ZoneUnderworldHeight => TilePoint.Y > Main.maxTilesY - 200;
		public bool ZoneSkyHeight => TilePoint.Y <= Main.worldSurface * 0.349999994039536;
		public bool ZoneHell => ZoneUnderworldHeight;
		#endregion
		#region HeldItem
		public Item HeldItem => TPlayer.inventory[TPlayer.selectedItem];
		#endregion
		#region ActiveChest
		public int ActiveChest
		{
			get
			{
				return TSPlayer.ActiveChest;
			}
			set
			{
				TSPlayer.ActiveChest = value;
				SendData(PacketTypes.ChestOpen, "", value);
			}
		}
		#endregion
		#region AppendixMsg
		public string AppendixMsg
		{
			get;
			set;
		}
		#endregion
		#endregion
		#region Methods
		#region Spawn
		public void Spawn()
		{
			TSPlayer.Spawn();
		}
		#endregion
		#region Teleport
		public void TeleportTo(Vector2 target)
		{
			TSPlayer.Teleport(target.X, target.Y);
		}
		#endregion
		#region OnLeave
		public void OnLeave()
		{
			BranchTask?.OnLeave();
		}
		public void OnLogout()
		{
			BranchTask?.OnLogout();
		}
		#endregion
		#region ToggActive
		public void ToggleActive(bool active)
		{
			TPlayer.active = active;
			StarverPlayer.All.SendData(PacketTypes.PlayerActive, "", Index);
		}
		#endregion
		#region Branches
		#region CheckBranchAvaiable
		public void CheckBranchAvaiable()
		{
			if (bldata.AvaiableBLs == BLFlags.None)
			{
				bldata.AvaiableBLs = BLFlags.YrtAEvah;
			}
			else
			{
				if (BLFinished(BLID.YrtAEvah) && !BLAvaiable(BLID.StoryToContinue))
				{
					SendSuccessMessage("你已解锁支线: " + BLID.StoryToContinue);
					bldata.AvaiableBLs |= BLFlags.StoryToContinue;
				}
			}
		}
		#endregion
		#region BLFinished
		public bool BLFinished(BLID id)
		{
			return Starver.Instance.TSKS.BranchTaskLines[(int)id].Count == bldata[id];
		}
		#endregion
		#region BranchTaskEnd
		public void BranchTaskEnd(bool success)
		{
			if (success)
			{
				SendInfoMessage($"支线任务{BranchTask}已完成");
				BLdata[BranchTask.BLID]++;
				RandomRocket(3, 10);
			}
			else
			{
				SendFailMessage($"支线任务{BranchTask}失败");
			}
			BranchTask = null;
		}
		#endregion
		#region BLAvaiable
		public bool BLAvaiable(BLID id)
		{
			return BLdata.AvaiableBLs.HasFlag(id.ToBLFLags());
		}
		public bool BLAvaiable(BLFlags flag)
		{
			return BLdata.AvaiableBLs.HasFlag(flag);
		}
		#endregion
		#endregion
		#region RandRocket
		public void RandomRocket(int min, int Max)
		{
			short[] rockets =
			{
				ProjectileID.RocketFireworksBoxGreen,
				ProjectileID.RocketFireworksBoxYellow,
				ProjectileID.RocketFireworksBoxRed,
				ProjectileID.RocketFireworksBoxBlue,
				ProjectileID.RocketFireworksBoxGreen,
				ProjectileID.RocketFireworksBoxYellow,
				ProjectileID.RocketFireworksBoxRed,
				ProjectileID.RocketFireworksBoxBlue,
				ProjectileID.FireworkFountainRainbow
			};
			int count = Starver.Rand.Next(min, Max);
			while (count-- > 0)
			{
				int idx = NewProj(Center + Starver.Rand.NextVector2(16 * 60, 16 * 30), Vector2.Zero, rockets.Next(), 0);
				Main.projectile[idx].timeLeft = 0;
			}
		}
		#endregion
		#region InLiquid
		public bool InLiquid(byte type)
		{
			var tile = Main.tile[TilePoint.X, TilePoint.Y];
			return tile.liquidType() == type && tile.liquid != 0;
		}
		#endregion
		#region ShowBonus
		public void ShowBonus()
		{
			SendMessage("你当前拥有以下特殊属性:", 255, 128, 255);
			if (bldata.Bonus.HasFlag(EffectBonus.CD34))
			{
				SendInfoMessage("    技能CD缩短为原来的 3 / 4");
			}
			if (bldata.Bonus.HasFlag(EffectBonus.MP34))
			{
				SendInfoMessage("    技能MP减少为原来的 3 / 4");
			}
			if (bldata.Bonus.HasFlag(EffectBonus.DamageDecrease1))
			{
				SendInfoMessage("    pve伤害减少5%");
			}
			if (bldata.Bonus.HasFlag(EffectBonus.DamageDecrease2))
			{
				SendInfoMessage("    pve伤害减少5%");
			}
			if (bldata.Bonus.HasFlag(EffectBonus.DamageDecrease3))
			{
				SendInfoMessage("    pve伤害减少5%");
			}
			if (bldata.Bonus.HasFlag(EffectBonus.DamageMultiply1))
			{
				SendInfoMessage("    2%打出2倍伤害(仅pve)");
			}
			if (bldata.Bonus.HasFlag(EffectBonus.DamageMultiply2))
			{
				SendInfoMessage("    2%打出2倍伤害(仅pve)");
			}
			if (bldata.Bonus.HasFlag(EffectBonus.ProbabilisticDodge))
			{
				SendInfoMessage("    3%的概率躲开攻击(仅pve)");
			}
		}
		#endregion
		#region GiveItem
		public void GiveItem(int type, int stack = 1, int prefix = 0)
		{
			int number = Item.NewItem((int)Center.X, (int)Center.Y, TPlayer.width, TPlayer.height, type, stack, true, prefix, true, false);
			SendData(PacketTypes.ItemDrop, "", number);
		}
		#endregion
		#region HasItem
		public bool HasItem(int type)
		{
			return TPlayer.HasItem(type);
		}
		#endregion
		#region GetUser
		public static int? GetUserIDByName(string name)
		{
			using var reader = TShockAPI.DB.DbExt.QueryReader(TShock.DB, "select * from users where Username=@0", name);
			if (reader.Read())
			{
				return reader.Get<int?>("ID");
			}
			return null;
		}
		public static string GetUserNameByID(int id)
		{
			using var reader = TShockAPI.DB.DbExt.QueryReader(TShock.DB, "select * from users where ID=@0", id);
			if (reader.Read())
			{
				return reader.Get<string>("Username");
			}
			return null;
		}
		#endregion
		#region GetBiomes
		public NPCSystem.BiomeType GetBiomes()
		{
			NPCSystem.BiomeType biome = default;
			bool Grass = true;
			#region Zones
			if (TPlayer.ZoneDesert)
			{
				Grass = false;
				biome |= NPCSystem.BiomeType.Dessert;
			}
			if (TPlayer.ZoneHoly)
			{
				Grass = false;
				biome |= NPCSystem.BiomeType.Holy;
			}
			if (TPlayer.ZoneCorrupt)
			{
				Grass = false;
				biome |= NPCSystem.BiomeType.Corrupt;
			}
			if (TPlayer.ZoneCrimson)
			{
				Grass = false;
				biome |= NPCSystem.BiomeType.Crimson;
			}
			if (ZoneDirtLayerHeight || ZoneRockLayerHeight)
			{
				Grass = false;
				biome |= NPCSystem.BiomeType.UnderGround;
			}
			if (TPlayer.ZoneJungle)
			{
				Grass = false;
				biome |= NPCSystem.BiomeType.Jungle;
			}
			if (TPlayer.ZoneSnow)
			{
				Grass = false;
				biome |= NPCSystem.BiomeType.Icy;
			}
			if (TPlayer.ZoneMeteor)
			{
				Grass = false;
				biome |= NPCSystem.BiomeType.Metor;
			}
			if (ZoneRain)
			{
				Grass = false;
				biome |= NPCSystem.BiomeType.Rain;
			}
			if (ZoneUnderworldHeight)
			{
				Grass = false;
				biome |= NPCSystem.BiomeType.Hell;
			}
			if (TPlayer.ZoneTowerSolar)
			{
				Grass = false;
				biome |= NPCSystem.BiomeType.TowerSolar;
			}
			if (TPlayer.ZoneTowerNebula)
			{
				Grass = false;
				biome |= NPCSystem.BiomeType.TowerNebula;
			}
			if (TPlayer.ZoneTowerStardust)
			{
				Grass = false;
				biome |= NPCSystem.BiomeType.TowerStardust;
			}
			if (TPlayer.ZoneTowerVortex)
			{
				Grass = false;
				biome |= NPCSystem.BiomeType.TowerVortex;
			}
			if (ZoneSkyHeight)
			{
				biome |= NPCSystem.BiomeType.Sky;
			}
			if (ZoneBeach)
			{
				biome |= NPCSystem.BiomeType.Beach;
			}
			if (Grass)
			{
				biome |= NPCSystem.BiomeType.Grass;
			}
			#endregion
			return biome;
		}
		#endregion
		#region GetConditions
		public NPCSystem.SpawnConditions GetConditions()
		{
			NPCSystem.SpawnConditions condition = default;
			if (Main.dayTime)
			{
				condition |= NPCSystem.SpawnConditions.Day;
				if (Main.eclipse)
				{
					condition |= NPCSystem.SpawnConditions.Eclipse;
				}
			}
			else
			{
				condition |= NPCSystem.SpawnConditions.Night;
				if (Main.bloodMoon)
				{
					condition |= NPCSystem.SpawnConditions.BloodMoon;
				}
			}
			return condition;
		}
		#endregion
		#region GetSpawnChecker
		public NPCSystem.SpawnChecker GetSpawnChecker()
		{
			return new NPCSystem.SpawnChecker() { Biome = GetBiomes(), Condition = GetConditions() };
		}
		#endregion
		#region EatItems
		/// <summary>
		/// 吃掉玩家背包里从begin起不包括end的物品
		/// </summary>
		/// <param name="begin"></param>
		/// <param name="end"></param>
		public void EatItems(int begin, int end)
		{
			for (; begin < end; begin++)
			{
				TPlayer.inventory[begin].netDefaults(0);
				NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, null, Index, begin);
			}
		}
		#endregion
		#region HasWeapon
		public bool HasWeapon(WeaponSystem.Weapons.Weapon weapon)
		{
			return Weapon[weapon.Career, weapon.Index] > 0;
		}
		#endregion
		#region Update
		public void Update()
		{
			timer++;
			if (!BLFinished(BLID.YrtAEvah))
			{
				if (FindBuffIndex(BuffID.DrillMount) != -1)
				{
					RemoveBuff(BuffID.DrillMount);
				}
			}
			BranchTask?.Updating(timer);
			UpdateCD();
			UpdateTilePoint();
			BranchTask?.Updated(timer);
		}
		protected void UpdateTilePoint()
		{
			TilePoint = Center.ToTileCoordinates();
		}
		protected void UpdateCD()
		{
			for (int i = 0; i < CDs.Length; i++)
			{
				CDs[i] = Math.Max(0, CDs[i] - 1);
			}
		}
		public void UpdateMoon()
		{
			if (Dead)
			{
				if (MoonIndex > 0)
				{
					Main.npc[MoonIndex].active = false;
				}
				return;
			}
			if (MoonIndex < 0)
			{
				MoonIndex = NewMoon();
			}
			else if (
				Main.npc[MoonIndex].type == NPCID.MoonLordCore &&
				Main.npc[MoonIndex].active == false)
			{
				Main.npc[MoonIndex].active = true;
				Main.npc[MoonIndex].SetDefaults(NPCID.MoonLordCore);
			}
			Main.npc[MoonIndex].type = NPCID.MoonLordCore;
			Main.npc[MoonIndex].aiStyle = -1;
			Main.npc[MoonIndex].Center = Center;
			SendData(PacketTypes.NpcUpdate, "", MoonIndex);
		}
		public void UpdateMoonClear()
		{
			if (MoonIndex < 0)
			{
				return;
			}
			else if (
				Main.npc[MoonIndex].type == NPCID.MoonLordCore &&
				Main.npc[MoonIndex].active != false &&
				Main.npc[MoonIndex].aiStyle == -1)
			{
				Main.npc[MoonIndex].active = false;
				Main.npc[MoonIndex].type = 0;
				Main.npc[MoonIndex].aiStyle = -1;
				// Main.npc[MoonIndex].Center = Center;
				NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, null, MoonIndex);
			}
		}
		#endregion
		#region UPGrade
		/// <summary>
		/// 消耗一定经验升级
		/// </summary>
		/// <param name="ExpGet"></param>
		public void UPGrade(BigInt ExpGet)
		{
			int lvl = level;
			ExpGet += exp;
			int need = AuraSystem.StarverAuraManager.UpGradeExp(lvl);
			if (HasPerm(Perms.VIP.LessCost))
			{
				need /= 3;
			}
			while (ExpGet > need)
			{
				ExpGet -= need;
				++lvl;
				need = AuraSystem.StarverAuraManager.UpGradeExp(lvl);
				if (HasPerm(Perms.VIP.LessCost))
				{
					need /= 3;
				}
			}
			Level = lvl;
			try
			{
				exp = (int)ExpGet;
			}
			catch
			{
				exp = Math.Max(0, exp);
			}
		}
		#endregion
		#region DataOperations
		#region Read
		/// <summary>
		/// 读取数据(仅限MySQL)
		/// </summary>
		/// <param name="UserID">玩家ID</param>
		/// <returns></returns>
		public static StarverPlayer Read(int UserID, int idx)
		{
			StarverPlayer player = new StarverPlayer(UserID, idx);
			if (SaveMode == SaveModes.MySQL)
			{
				using MySqlDataReader result = db.QueryReader("SELECT * FROM Starver WHERE UserID=@0;", UserID);
				if (result.Read())
				{
					player.ReadFromReader(result);
				}
				else
				{
					TSPlayer.Server.SendInfoMessage("StarverPlugins: 玩家{0}不存在,已新建", player.Name);
					AddNewUser(player);
				}
			}
			else
			{
				throw new Exception("非MySQL存储模式下不能通过UserID读取玩家数据");
			}
			player.MP = (player.MaxMP = player.level / 3 + 100) / 2;
			return player;
		}
		public static StarverPlayer Read(int who, string Name)
		{
			StarverPlayer player;
			string tmp = SavePath + "//" + Name + ".json";
			if (File.Exists(tmp))
			{
				player = FromJson(who, File.ReadAllText(tmp));
#if DEBUG
				TSPlayer.Server.SendInfoMessage("Name:{0}", player.Name);
#endif
			}
			else
			{
				player = new StarverPlayer
				{
					Index = who,
					Name = Name
				};
			}
			player.Name = Name;
			player.MP = (player.MaxMP = player.level / 3 + 100) / 2;
			return player;
		}
		#endregion
		#region ReadFromReader
		private static StarverPlayer ReadFromReaderStatic(MySqlDataReader reader)
		{
			StarverPlayer player = new StarverPlayer();
			player.Weapon = JsonConvert.DeserializeObject<byte[,]>(reader.GetString("Weapons"));
			player.Skills = JsonConvert.DeserializeObject<int[]>(reader.GetString("Skills"));
			player.level = reader.GetInt32("Level");
			player.exp = reader.GetInt32("Exp");
			player.BLdata = BLData.Deserialize(reader.GetFieldValue<byte[]>(reader.GetOrdinal("BranchTaskDatas")));
			return player;
		}
		private void ReadFromReader(MySqlDataReader reader)
		{
			Weapon = JsonConvert.DeserializeObject<byte[,]>(reader.GetString("Weapons"));
			Skills = JsonConvert.DeserializeObject<int[]>(reader.GetString("Skills"));
			level = reader.GetInt32("Level");
			exp = reader.GetInt32("Exp");
			BLdata = BLData.Deserialize(reader.GetFieldValue<byte[]>(reader.GetOrdinal("BranchTaskDatas")));
		}
		#endregion
		#region Add
		/// <summary>
		/// 添加用户
		/// </summary>
		/// <param name="player"></param>
		public static void AddNewUser(StarverPlayer player)
		{
			int UserID = player.UserID;
			int Level = player.Level;
			int Exp = player.Exp;
			string Weapon = JsonConvert.SerializeObject(player.Weapon);
			string Skills = JsonConvert.SerializeObject(player.Skills);
			db.Excute("INSERT INTO Starver (UserId, Weapons, Skills, Level, Exp, BranchTaskDatas) VALUES ( @0 ,@1, @2, @3, @4, @5);",
				UserID, Weapon, Skills, Level, Exp, new byte[16 * 4]);
		}
		#endregion
		#region Save
		/// <summary>
		/// 保存
		/// </summary>
		public void Save()
		{
			if (SaveMode == SaveModes.MySQL)
			{
				if (UserID == -1 || IsServer || IsGuest)
				{
					return;
				}
				string wps = JsonConvert.SerializeObject(Weapon);
				string skills = JsonConvert.SerializeObject(Skills);
				db.Excute("UPDATE Starver SET Weapons=@0 WHERE UserID=@1;", wps, UserID);
				db.Excute("UPDATE Starver SET Skills=@0 WHERE UserID=@1;", skills, UserID);
				db.Excute("UPDATE Starver SET Level=@0 WHERE UserID=@1;", level, UserID);
				db.Excute("UPDATE Starver SET Exp=@0 WHERE UserID=@1;", exp, UserID);
				db.Excute("UPDATE Starver SET BranchTaskDatas=@0 WHERE UserID=@1;", BLData.Serialize(BLdata), UserID);
			}
			else
			{
				File.WriteAllText(SavePath + "//" + Name + ".json", ToJson());
			}
		}
		#endregion
		#region Reload
		/// <summary>
		/// 重新加载
		/// </summary>
		public void Reload()
		{
			if (SaveMode == SaveModes.MySQL)
			{
				StarverPlayer tempplayer = Read(UserID, Index);
				level = tempplayer.level;
				Skills = tempplayer.Skills;
				Exp = tempplayer.Exp;
				Weapon = tempplayer.Weapon;
				BLdata = tempplayer.BLdata;
			}
			else
			{
				PlayerData data = ReadData();
				Level = data.Level;
				exp = data.Exp;
				for (int i = 0; i < Skills.Length; i++)
				{
					Skills[i] = data.Skills[i];
				}
				bldata = BLData.Deserialize(data.BLDatas);
			}
			Save();
		}
		#endregion
		#region FromJson
		private static StarverPlayer FromJson(int who, string text)
		{
			var player = new StarverPlayer();
			player.Index = who;
			var data = JsonConvert.DeserializeObject<PlayerData>(text);
			player.exp = data.Exp;
			player.Level = data.Level;
			player.Skills = data.Skills.Clone() as int[];
			player.bldata = BLData.Deserialize(data.BLDatas);
			player.MaxMP = data.MaxMP;
			return player;
		}
		private PlayerData ReadData()
		{
			string path = SavePath + "//" + Name + ".json";
			PlayerData data = JsonConvert.DeserializeObject<PlayerData>(File.ReadAllText(path));
			return data;
		}
		#endregion
		#region ToJson
		private string ToJson()
		{
			var data = new PlayerData();
			data.Level = level;
			data.Exp = exp;
			data.Skills = Skills;
			data.BLDatas = BLData.Serialize(bldata);
			data.MaxMP = MaxMP;
			return JsonConvert.SerializeObject(data);
		}
		#endregion
		#endregion
		#region SetLifeMax
		public void SetLifeMax(int value)
		{
			int Life = value;
			TPlayer.SetLife(Life);
		}
		/// <summary>
		/// 根据等级自动计算
		/// </summary>
		public void SetLifeMax()
		{
			if (TPlayer.statLifeMax2 < 500)
			{
				return;
			}
			int Life = 500 + Utils.CalculateLife(level);
			TPlayer.SetLife(Life);
		}
		#endregion
		#region Projs
		#region FromPolar
		/// <summary>
		/// 极坐标获取角度
		/// </summary>
		/// <param name="rad">所需角度(弧度)</param>
		/// <param name="length"></param>
		/// <returns></returns>
		public Vector FromPolar(double rad, float length)
		{
			return Vector.FromPolar(rad, length);
		}
		#endregion
		#region NewProj
		/// <summary>
		/// 生成弹幕
		/// </summary>
		/// <param name="position"></param>
		/// <param name="velocity"></param>
		/// <param name="Type"></param>
		/// <param name="Damage"></param>
		/// <param name="KnockBack"></param>
		/// <param name="ai0"></param>
		/// <param name="ai1"></param>
		/// <returns></returns>
		public int NewProj(Vector2 position, Vector2 velocity, int Type, int Damage, float KnockBack = 20f, float ai0 = 0, float ai1 = 0)
		{
			return Utils.NewProj(position, velocity, Type, Damage, KnockBack, Index, ai0, ai1);
		}
		#endregion
		#region ProjCircle
		/// <summary>
		/// 弹幕圆
		/// </summary>
		/// <param name="Center"></param>
		/// <param name="r"></param>
		/// <param name="speed">速率</param>
		/// <param name="Type"></param>
		/// <param name="number">弹幕总数</param>
		public void ProjCircle(Vector2 Center, float r, float speed, int Type, int number, int Damage, float ai0 = 0, float ai1 = 0)
		{
			double averagerad = Math.PI * 2 / number;
			for (int i = 0; i < number; i++)
			{
				NewProj(Center + FromPolar(averagerad * i, r), -FromPolar(averagerad * i, -speed), Type, Damage, 4f, ai0, ai1);
			}
		}
		/// <summary>
		/// 弹幕圆
		/// </summary>
		/// <param name="Center"></param>
		/// <param name="angle">偏转角</param>
		/// <param name="r"></param>
		/// <param name="Vel">速率</param>
		/// <param name="Type"></param>
		/// <param name="number">弹幕总数</param>
		public void ProjCircleEx(Vector2 Center, double angle, float r, float Vel, int Type, int number, int Damage, float ai0 = 0, float ai1 = 0)
		{
			double averagerad = Math.PI * 2 / number;
			for (int i = 0; i < number; i++)
			{
				NewProj(Center + FromPolar(averagerad * i, r), -FromPolar(angle + averagerad * i, -Vel), Type, Damage, 4f, ai0, ai1);
			}
		}
		#endregion
		#region ProjCircle
		/// <summary>
		/// 弹幕圆
		/// </summary>
		/// <param name="Center"></param>
		/// <param name="r"></param>
		/// <param name="speed">速率</param>
		/// <param name="Type"></param>
		/// <param name="number">弹幕总数</param>
		/// <param name="direction">0:不动 1:向内 2:向外</param>
		public int[] ProjCircleRet(Vector2 Center, float r, float speed, int Type, int number, int Damage, byte direction = 1, float ai0 = 0, float ai1 = 0)
		{
			switch (direction)
			{
				case 0:
					speed = 0;
					break;
				case 1:
					speed *= 1;
					break;
				case 2:
					speed *= -1;
					break;
			}
			double averagerad = Math.PI * 2 / number;
			int[] arr = new int[number];
			for (int i = 0; i < number; i++)
			{
				arr[i] = NewProj(Center + FromPolar(averagerad * i, r), FromPolar(averagerad * i, -speed), Type, Damage, 4f, ai0, ai1);
			}
			return arr;
		}
		#endregion
		#region ProjSector
		/// <summary>
		/// 扇形弹幕
		/// </summary>
		/// <param name="Center">圆心</param>
		/// <param name="speed">速率</param>
		/// <param name="r">半径</param>
		/// <param name="interrad">中心半径的方向</param>
		/// <param name="rad">张角</param>
		/// <param name="Damage">伤害(带加成)</param>
		/// <param name="type"></param>
		/// <param name="num">数量</param>
		/// <param name="ai0"></param>
		/// <param name="ai1"></param>
		public void ProjSector(Vector2 Center, float speed, float r, double interrad, double rad, int Damage, int type, int num, float ai0 = 0, float ai1 = 0)
		{
			double start = interrad - rad / 2;
			double average = rad / num;
			for (int i = 0; i < num; i++)
			{
				NewProj(Center + FromPolar(start + i * average, r), FromPolar(start + i * average, speed), type, Damage, 4f, ai0, ai1);
			}
		}
		#endregion
		#region ProjLine
		/// <summary>
		/// 制造速度平行的弹幕直线
		/// </summary>
		/// <param name="Begin">起点</param>
		/// <param name="End">终点</param>
		/// <param name="Vel">速度</param>
		/// <param name="num">数量</param>
		/// <param name="Damage"></param>
		/// <param name="type"></param>
		/// <param name="ai0"></param>
		/// <param name="ai1"></param>
		public void ProjLine(Vector2 Begin, Vector2 End, Vector2 Vel, int num, int Damage, int type, float ai0 = 0, float ai1 = 0)
		{
			Vector2 average = End - Begin;
			average /= num;
			for (int i = 0; i < num; i++)
			{
				NewProj(Begin + average * i, Vel, type, Damage, 3f, ai0, ai1);
			}
		}
		/// <summary>
		/// 制造速度平行的弹幕直线
		/// </summary>
		/// <param name="Begin">起点</param>
		/// <param name="End">终点</param>
		/// <param name="Vel">速度</param>
		/// <param name="num">数量</param>
		/// <param name="Damage"></param>
		/// <param name="type"></param>
		/// <param name="ai0"></param>
		/// <param name="ai1"></param>
		public int[] ProjLineReturns(Vector2 Begin, Vector2 End, Vector2 Vel, int num, int Damage, int type, float ai0 = 0, float ai1 = 0)
		{
			int[] arr = new int[num];
			Vector2 average = End - Begin;
			average /= num;
			for (int i = 0; i < num; i++)
			{
				arr[i] = NewProj(Begin + average * i, Vel, type, Damage, 3f, ai0, ai1);
			}
			return arr;
		}
		#endregion
		#endregion
		#region Operator
		public static implicit operator TSPlayer(StarverPlayer player)
		{
			return player.TSPlayer;
		}
		public static implicit operator Player(StarverPlayer player)
		{
			return player.TPlayer;
		}
		public static implicit operator int(StarverPlayer player)
		{
			return player.Index;
		}
		#endregion
		#region SetSkill
		/// <summary>
		/// 设置技能
		/// </summary>
		/// <param name="name">技能名称(可以只取前几位字母)</param>
		/// <param name="slot">槽位</param>
		/// <param name="ServerDoThis">是否无视等级设置</param>
		public void SetSkill(string name, int slot, bool ServerDoThis = false)
		{
			slot -= 1;
			if (slot < 0 || slot > AuraSystem.SkillManager.Skills.Length)
			{
				TSPlayer.SendErrorMessage("槽位错误!");
			}
			else
			{
				foreach (var skill in AuraSystem.SkillManager.Skills)
				{
					if (skill.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase))
					{
						if (ServerDoThis == false && !skill.CanSet(this))
						{
							SendErrorMessage("设置失败");
						}
						else
						{
							Skills[slot] = skill.Index;
							if (!ServerDoThis)
							{
								TSPlayer.SendSuccessMessage("设置成功");
							}
						}
						if (!ServerDoThis)
						{
							TSPlayer.SendInfoMessage(skill.Text);
						}
						return;
					}
				}
				if (!ServerDoThis)
				{
					TSPlayer.SendErrorMessage("错误的技能名");
					TSPlayer.SendErrorMessage("list:	查看技能列表");
				}
			}
		}
		#endregion
		#region Damage
		public void Damage(int damage)
		{
			if (TryDodgeDamage())
			{
				return;
			}
			damage = GetDamage(damage);
			TSPlayer.DamagePlayer(damage);
			//NetMessage.SendPlayerHurt(Index, PlayerDeathReason.LegacyDefault(), damage, Index, false, false, 0);
		}
		public void Damage(int damage, PlayerDeathReason reason = null)
		{
			if (TryDodgeDamage())
			{
				return;
			}
			damage = GetDamage(damage);
			NetMessage.SendPlayerHurt(Index, reason ?? PlayerDeathReason.LegacyDefault(), damage, new Random().Next(-1, 1), false, false, 0);
		}
		public void Damage(int damage, Color effectTextColor)
		{
			if (TryDodgeDamage())
			{
				return;
			}
			damage = GetDamage(damage);
			TSPlayer.DamagePlayer(damage);
			SendCombatMsg(damage.ToString(), effectTextColor);
			//NetMessage.SendPlayerHurt(Index, PlayerDeathReason.LegacyDefault(), damage, Index, false, false, 0);
		}
		private bool TryDodgeDamage()
		{
			if (HasBuff(BuffID.ShadowDodge))
			{
				RemoveBuff(BuffID.ShadowDodge);
				return true;
			}
			if (bldata.Bonus.HasFlag(EffectBonus.ProbabilisticDodge))
			{
				return Starver.Rand.Next(100) < 3;
			}
			return false;
		}
		private int GetDamage(int rawdamage)
		{
			double damage = Math.Min(23000, rawdamage);
			if (bldata.Bonus.HasFlag(EffectBonus.DamageDecrease1))
			{
				damage *= 0.95;
			}
			if (bldata.Bonus.HasFlag(EffectBonus.DamageDecrease2))
			{
				damage *= 0.95;
			}
			if (bldata.Bonus.HasFlag(EffectBonus.DamageDecrease3))
			{
				damage *= 0.95;
			}
			return (int)damage;
		}
		#endregion
		#region ShowInfos
		/// <summary>
		/// 调试使用
		/// </summary>
		/// <param name="ToWho">展示信息给谁</param>
		public void ShowInfos(TSPlayer ToWho = null)
		{
			ToWho = ToWho ?? TSPlayer.Server;
			PropertyInfo[] infos = GetType().GetProperties();
			foreach (var info in infos)
			{
				try
				{
					if (info.CanWrite)
						ToWho.SendInfoMessage("\"{0}\" :{1}", info.Name, JsonConvert.SerializeObject(info.GetValue(this)));
				}
				catch
				{

				}
			}
		}
		#endregion
		#region Kick
		public void Disconnect(string reason)
		{
			TSPlayer.Disconnect(reason);
		}
		public void Kick(string reason, bool silence = false)
		{
			Disconnect(reason);
			if (!silence)
			{
				StarverPlayer.All.SendErrorMessage($"玩家{Name} 被 Kick了: {reason}");
			}
		}
		#endregion
		#region GetSkill
		public Skill GetSkill(int slot)
		{
			return AuraSystem.SkillManager.Skills[Skills[slot]];
		}
		#endregion
		#region SkillCombineCD
		public string SkillCombineCD(int slot)
		{
			Skill skill = GetSkill(slot);
			return $"{skill.Name}({(CDs[slot] + 59) / 60})";
		}
		#endregion
		#region TryGetPlayer
		public static bool TryGetTempPlayer(string Name, out StarverPlayer player)
		{
			player = null;
			switch (SaveMode)
			{
				case SaveModes.MySQL:
					{
						int? ID = GetUserIDByName(Name);
						if (!ID.HasValue)
						{
							return false;
						}
						using MySqlDataReader reader = db.QueryReader("select * from starver where UserID=@0", ID.Value);
						if (reader.Read())
						{
							player = new StarverPlayer(ID.Value, -3);
							player.ReadFromReader(reader);
							return true;
						}
						return false;
					}
				case SaveModes.Json:
					{
						string path = Path.Combine(SavePath, $"{Name}.json");
						if (File.Exists(path))
						{
							try
							{
								player = JsonConvert.DeserializeObject<StarverPlayer>(File.ReadAllText(path));
								player.Temp = true;
							}
							catch (Exception e)
							{
								TShock.Log.Error(e.ToString());
								return false;
							}
							return true;
						}
						return false;
					}
			}
			return false;
		}
		#endregion
		#region KillMe
		public void KillMe()
		{
			TSPlayer.KillPlayer();
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			return Name;
		}
		#endregion
		#endregion
		#region Hooks
		public void OnUpdateItemDrop(UpdateItemDropEventArgs args)
		{
			BranchTask?.OnUpdateItemDrop(args);
		}
		public void OnDeath()
		{
			BranchTask?.OnDeath();
		}
		public void OnPickAnalogItem(AuraSystem.Realms.AnalogItem item)
		{
			BranchTask?.OnPickAnalogItem(item);
			if (!item.Active)
			{
				item.Kill();
			}
		}
		public void OnGetData(GetDataEventArgs args)
		{
			BranchTask?.OnGetData(args);
		}
		public void StrikingNPC(NPCStrikeEventArgs args)
		{
			if (bldata.Bonus.HasFlag(EffectBonus.DamageMultiply1))
			{
				if (Starver.Rand.Next(100) < 2)
				{
					args.RealDamage *= 2;
				}
			}
			if (bldata.Bonus.HasFlag(EffectBonus.DamageMultiply2))
			{
				if (Starver.Rand.Next(100) < 2)
				{
					args.RealDamage *= 2;
				}
			}
			BranchTask?.StrikingNPC(args);
		}
		public void StrikedNPC(NPCStrikeEventArgs args)
		{
			BranchTask?.StrikedNPC(args);
		}
		public void CreatingProj(GetDataHandlers.NewProjectileEventArgs args)
		{
			if (!BLFinished(BLID.YrtAEvah))
			{
				switch(args.Type)
				{
					case ProjectileID.Dynamite:
					case ProjectileID.BouncyDynamite:
					case ProjectileID.StickyDynamite:
					case ProjectileID.Bomb:
					case ProjectileID.BouncyBomb:
					case ProjectileID.StickyBomb:
					case ProjectileID.GrenadeII:
					case ProjectileID.ProximityMineII:
					case ProjectileID.RocketII:
					case ProjectileID.RocketSnowmanII:
					case ProjectileID.GrenadeIV:
					case ProjectileID.ProximityMineIV:
					case ProjectileID.RocketIV:
					case ProjectileID.RocketSnowmanIV:
						Main.projectile[args.Index].KillMeEx();
						args.Handled = true;
						return;
				}
			}
			BranchTask?.CreatingProj(args);
		}
		public void ReleasingSkill(ReleaseSkillEventArgs args)
		{
			if (bldata.Bonus.HasFlag(EffectBonus.CD34))
			{
				args.CD = args.CD * 3 / 4;
			}
			if (bldata.Bonus.HasFlag(EffectBonus.MP34))
			{
				args.MPCost = args.MPCost * 3 / 4;
			}
			BranchTask?.ReleasingSkill(args);
		}
		public void ReleasedSkill(ReleaseSkillEventArgs args)
		{
			BranchTask?.ReleasedSkill(args);
		}
		#endregion
		#region Datas
		public NetInventory Inventory { get; }
		public double DamageIndex
		{
			get
			{
				return level switch
				{
					_ when level < 100 => 1 + 0.015 * level,
					_ when 100 <= level && level < 1000 => 1 + 1.5 + Math.Log(level / 100, 2),
					// _ when 1000 <= level && level < 10000 => 1 + 1.5 + Math.Pow(level, 0.2) + 3 * Math.Pow(level / 10000, 10) - 4
					_ when 1000 <= level && level < 10000 => 1.821928094887362 + Math.Pow(level, 0.2) + 3 * Math.Pow(level / 10000, 10),
					_ when 10000 <= level && level < 100000 => 11.131501539689296 + Math.Pow(Math.Log10(level) - 3.7, Math.Log(level / 1000, 2) + 1),
					_ => 20
				};
			}
		}
		/// <summary>
		/// 技能CD
		/// </summary>
		public int[] CDs { get; set; }
		/// <summary>
		/// 技能ID列表
		/// </summary>
		public int[] Skills { get; set; }
		
		public bool Temp { get; set; }
		
		public BranchTask BranchTask { get; set; }
		
		public int AvalonGradation { get; set; }
		public string Name { get; set; }
		
		/// <summary>
		/// 上一次捕获到释放技能
		/// </summary>
		public DateTime LastHandle { get; set; } = DateTime.Now;
		/// <summary>
		/// 玩家升级经验是否更少
		/// </summary>
		public bool LessCost => HasPerm(Perms.VIP.LessCost);
		/// <summary>
		/// 玩家存活且在线
		/// </summary>
		public bool Active => TPlayer.active && !TPlayer.dead;
		public bool Dead => TPlayer.dead;
		
		public bool IgnoreCD { get; set; }
		
		public bool ForceIgnoreCD { get; set; }
		public bool IsGuest => UserID == -3;
		/// <summary>
		/// 在MySql中的UserID
		/// <para> -1代表All</para>
		/// <para>-2代表服务器</para>
		/// -3为Guest(未注册)
		/// </summary>
		public int UserID { get; private set; }
		/// <summary>
		/// 玩家索引
		/// </summary>
		public int Index { get; internal set; }
		/// <summary>
		/// 当前等级
		/// 更改后会自动改变血量以及MP上限
		/// </summary>
		public int Level
		{
			get
			{
				return level;
			}
			set
			{
				if (level == int.MaxValue)
				{
					return;
				}
				level = value;
				if (Temp)
				{
					return;
				}
				SetLifeMax();
				MaxMP = 100 + (level / 3);
			}
		}
		/// <summary>
		/// 当前经验
		/// </summary>
		public int Exp
		{
			get
			{
				return exp;
			}
			set
			{
				if (value < exp)
				{
					exp = Math.Max(0, value);
					return;
				}
				if (Temp || Level < 100)
				{
					if (value > 0)
					{
						exp = value;
					}
					return;
				}
				long expNow = value;
				int lvl = level;
				int Need = UpGradeExp;
				if (HasPerm(Perms.VIP.LessCost))
				{
					Need /= 3;
				}
				while (expNow > Need)
				{
					expNow -= Need;
					lvl++;
					Need = UpGradeExp;
					if (HasPerm(Perms.VIP.LessCost))
					{
						Need /= 3;
					}
				}
				Level = lvl;
				exp = Math.Max(0, (int)expNow);
			}
		}
		public int MP
		{
			get
			{
				return mp;
			}
			set
			{
				mp = Math.Min(value, MaxMP);
				mp = Math.Max(0, mp);
			}
		}
		/// <summary>
		/// 当前升级所需经验
		/// </summary>
		public int UpGradeExp => AuraSystem.StarverAuraManager.UpGradeExp(level);
		public int MaxMP { get; set; }
		public byte[,] Weapon { get; set; } =
		{
			{ 0, 0, 0, 0 },
			{ 0, 0, 0, 0 },
			{ 0, 0, 0, 0 },
			{ 0, 0, 0, 0 }
		};
		public int Life
		{
			get
			{
				return TPlayer.statLife;
			}
			set
			{
				TPlayer.statLife = Math.Min(value, TPlayer.statLifeMax2);
				SendData(PacketTypes.PlayerHp, string.Empty, Index);
			}
		}
		public int Mana
		{
			get
			{
				return TPlayer.statMana;
			}
			set
			{
				TPlayer.statMana = Math.Min(value, TPlayer.statManaMax);
				SendData(PacketTypes.PlayerMana, string.Empty, Index);
			}
		}
		public ref BLData BLdata => ref bldata;
		/// <summary>
		/// 数据库连接
		/// </summary>
		internal static MySqlConnection DB
		{
			get => db;
			set => db = value;
		}
		/// <summary>
		/// 代表服务器
		/// </summary>
		internal static StarverPlayer Server => server;
		/// <summary>
		/// 代表all(仅用于SendMessage和SendData)
		/// </summary>
		internal static StarverPlayer All => all;
		/// <summary>
		/// 用于未注册用户(退出后消失)
		/// </summary>
		internal static StarverPlayer Guest => new StarverPlayer() { Name = "Guest", level = 0, UserID = -3, Index = -3 };
		/// <summary>
		/// 上一次使用的技能
		/// </summary>
		internal int LastSkill;
		internal int mp;
		internal int exp;
		internal BLData bldata;
		/// <summary>
		/// 获取对应Terraria.Player
		/// </summary>
		public Player TPlayer => TSPlayer.TPlayer;
		/// <summary>
		/// 获取对应TSPlayer
		/// </summary>
		public TSPlayer TSPlayer => UserID switch
		{
			-2 => TSPlayer.Server,
			-1 => TSPlayer.All,
			_ => TShock.Players[Index]
		};
		private int level = StarverConfig.Config.DefaultLevel;
		#endregion
		#region Privates
		#region Fields
		private Point TilePoint;
		private int timer;
		private bool IsServer;
		private int MoonIndex = -1;
		private static StarverPlayer all = new StarverPlayer() { Name = "All", level = int.MaxValue, Index = -1, UserID = -1 };
		private static StarverPlayer server = new StarverPlayer() { Name = "Server", level = int.MaxValue, Index = -2, UserID = -2, IsServer = true };
		private static string SavePath => Starver.SavePathPlayers;
		private static SaveModes SaveMode = StarverConfig.Config.SaveMode;
		private static MySqlConnection db;
		#endregion
		#region Ctor
		private StarverPlayer(bool temp = false)
		{
			Temp = temp;
			Skills = new int[Skill.MaxSlots];
			CDs = new int[Skill.MaxSlots];
		}
		private StarverPlayer(int userID, int idx) : this(idx == -3)
		{
			UserID = userID;
			Name = GetUserNameByID(UserID);
			Index = idx;
			if (0 <= idx && idx < TShock.Players.Length)
			{
				Inventory = new NetInventory(this);
			}
		}
		#endregion
		#region NewMoon
		private static int NewMoon()
		{
			int i;
			for (i = 0; i < 200; i++)
			{
				if (!Main.npc[i].active)
				{
					Main.npc[i] = new NPC();
					Main.npc[i].SetDefaults(NPCID.MoonLordCore);
					Main.npc[i].aiStyle = -1;
					NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, null, i);
					break;
				}
			}
			return i;
		}
		#endregion
		#region NewConnection
		private static MySqlConnection NewConnection()
		{
			string[] dbHost = StarverConfig.Config.MySQLHost.Split(':');
			return new MySqlConnection()
			{
				ConnectionString = string.Format("Server={0}; Port={1}; Database={2}; UserName={3}; Password={4}; Allow User Variables=True;",
					dbHost[0],
					dbHost[1],
					StarverConfig.Config.MySQLDBName,
					StarverConfig.Config.MySQLUserName,
					StarverConfig.Config.MySQLPassword)
			};
		}
		#endregion
		#region CheckTableExist
		private static bool CheckTableExist()
		{
			return db.GetSchema("Tables").AsEnumerable().Any(value => value.Table.TableName == "Starver");
		}
		#endregion
		#region CreateTable
		private static void CreateTable()
		{
			var db2 = db.Clone() as MySqlConnection;
			db2.Open();
			var creator = new TableCreator(db2);
			var Table = new SQLTable("Starver",
				new SQLColumn { Name = "UserID", DataType = MySqlDbType.Int32, Length = 4 },
				new SQLColumn { Name = "Level", DataType = MySqlDbType.Int32, Length = 4 },
				new SQLColumn { Name = "Exp", DataType = MySqlDbType.Int32, Length = 4 },
				new SQLColumn { Name = "Skills", DataType = MySqlDbType.Text, Length = 30 },
				new SQLColumn { Name = "Weapons", DataType = MySqlDbType.Text, Length = 80 },
				new SQLColumn { Name = "BranchTaskDatas", DataType = MySqlDbType.VarBinary, Length = 4 * 16 },
				new SQLColumn { Name = "ExtensionDatas", DataType = MySqlDbType.VarBinary, Length = 256 }
				);
			creator.CreateTable(Table);
		}
		#endregion
		#region cctor
		static StarverPlayer()
		{
			//StarverConfig.Config = StarverConfig.Read();
			switch (StarverConfig.Config.SaveMode)
			{
				case SaveModes.MySQL:
					{
						db = NewConnection();
						CreateTable();
						break;
					}
				case SaveModes.Json:
					{
						SaveMode = SaveModes.Json;
						break;
					}
			}
			TSPlayer.Server.SendInfoMessage("Config.SaveMode:{0}", StarverConfig.Config.SaveMode);
			TSPlayer.Server.SendInfoMessage("SaveMode:{0}", SaveMode);
		}

		#endregion
		#endregion
	}
}
