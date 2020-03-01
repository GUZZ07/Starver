using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using System.Reflection;
using MySql;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using System.Threading;
using Terraria.ID;

namespace Starvers
{
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public static class Utils
	{
		#region Properties
		public static Random Rand => Starver.Rand;
		public static MySqlConnection DB => StarverPlayer.DB;
		public static StarverPlayer[] Players => Starver.Players;
		public static StarverConfig Config => StarverConfig.Config;
		#endregion
		#region DropItemInstanced
		public static void DropItemForEveryone(this NPC npc, Vector2 Position, int itemType, int itemStack = 1, bool interactionRequired = true)
		{
			if (itemType <= 0)
			{
				return;
			}
			int num = NewItem(Position, itemType, itemStack);
			if (num == -1)
			{
				return;
			}
			Terraria.Main.itemLockoutTime[num] = 54000;
			for (int i = 0; i < Starver.Players.Length; i++)
			{
				if ((npc.playerInteraction[i] || !interactionRequired) && Main.player[i].active)
				{
					Terraria.NetMessage.SendData(90, i, -1, null, num);
				}
			}
			Terraria.Main.item[num].active = false;
			npc.value = 0f;
		}
		#endregion
		#region SaveAll
		public static void SaveAll()
		{
			for (int i = 0; i < Starver.Players.Length; i++)
			{
				if (Players[i] == null || !Players[i].Active || Players[i].UserID < 0) 
				{
					continue;
				}
				Players[i].Save();
			}
		}
		#endregion
		#region UpGradeAll
		public static void UpGradeAll(int lvlup)
		{
			foreach(var player in Starver.Players)
			{
				if(player is null || !player.Active || player.IsGuest)
				{
					continue;
				}
				player.Level += lvlup;
				player.Save();
			}
#if false
			if (Config.SaveMode == SaveModes.MySQL)
			{
				SaveAll();
				using MySqlConnection connection = DB.Clone() as MySqlConnection;
				connection.Open();
				using MySqlCommand cmd = new MySqlCommand("Select UserID,Level from Starver", connection);
				using MySqlDataReader Reader = cmd.ExecuteReader(CommandBehavior.Default);
				do
				{
					try
					{
						if (Reader.Read())
						{
							int UserID = Reader.Get<int>("UserID");
							int Level = Reader.Get<int>("Level");
							if (Level > 120)
							{
								Level += lvlup;
							}
							DB.Query("update Starver set Level=@0 WHERE UserID=@1", Level, UserID);
						}
					}
					catch (Exception e)
					{
						TSPlayer.Server.SendInfoMessage(e.ToString());
					}
				}
				while (Reader.NextResult());
			}
			else
			{
				FileInfo[] files = Starver.PlayerFolder.GetFiles("*.json");
				foreach (var ply in Starver.Players)
				{
					if (ply is null)
					{
						continue;
					}
					ply.Save();
				}
				foreach (var file in files)
				{
					StarverPlayer player = StarverPlayer.Read(file.Name, true);
					if (player.Level > 120)
					{
						player.Level += lvlup;
					}
					player.Save();
				}
			}
			foreach (var ply in Starver.Players)
			{
				if (ply is null)
				{
					continue;
				}
				ply.Reload();
			}
#endif
		}
		#endregion
		#region AverageLevel
		public static int AverageLevel
		{
			get
			{
				int num = 0;
				double level = 0;
				if (Config.SaveMode == SaveModes.MySQL)
				{
					SaveAll();
					using MySqlCommand cmd = new MySqlCommand("Select UserID,Level from Starver", DB);
					using MySqlDataReader Reader = cmd.ExecuteReader();
					do
					{
						try
						{
							int UserID = Reader.Get<int>("UserID");
							int Level = Reader.Get<int>("Level");
							if (Level < Config.LevelNeed)
							{
								continue;
							}
							level += Level;
							num++;
						}
						catch (Exception e)
						{
							TSPlayer.Server.SendInfoMessage(e.ToString());
						}
					}
					while (Reader.NextResult());
				}
				else
				{
					FileInfo[] files = Starver.PlayerFolder.GetFiles("*.json");
					foreach (var file in files)
					{
						var data = JsonConvert.DeserializeObject<StarverPlayer.PlayerData>(File.ReadAllText(file.FullName));
						level += data.Level;
						num++;
					}
				}
				int result = (int)Math.Min(level / num, int.MaxValue);
				return result;
			}
		}
		#endregion
		#region CalculateLife
		public static int CalculateLife(int Level)
		{
			int Result = Math.Min(30000, Level / 3);
			return Result;
		}
		#endregion
		#region StrikeMe
		public static void StrikeMe(this NPC RealNPC, int Damage, float knockback, StarverPlayer player)
		{
			RealNPC.playerInteraction[player.Index] = true;
			int realdamage = (int)(Damage * (Rand.NextDouble() - 0.5) / 4 - RealNPC.defense * 0.5);
			RealNPC.life = Math.Max(RealNPC.life - realdamage, 0);
			RealNPC.SendCombatMsg(realdamage.ToString(), Color.Yellow);
			if (RealNPC.life < 1)
			{
				RealNPC.checkDead();
			}
			else
			{
				RealNPC.velocity.LengthAdd(knockback * (1f - RealNPC.knockBackResist));
				TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", RealNPC.whoAmI);
			}
		}
		#endregion
		#region Cmd
		public static StarverPlayer SPlayer(this CommandArgs args)
		{
			if(args.Player == TSPlayer.All)
			{
				return StarverPlayer.All;
			}
			else if(args.Player == TSPlayer.Server)
			{
				return StarverPlayer.Server;
			}
			return Starver.Players[args.Player.Index];
		}
		#endregion
		#region Activeplayers
		public static int ActivePlayers()
		{
			return Starver.Players.Count(player => player?.Active == true);
		}
		#endregion
		#region NewProj
		public static int NewProj(Vector2 position, Vector2 velocity, int Type, int Damage = 0, float KnockBack = 0, int Owner = 255, float ai0 = 0, float ai1 = 0)
		{
			if(Type >= Main.projectileTexture.Length)
			{
				Type = 0;
			}
			if (Owner == 255)
			{
				Owner = Main.myPlayer;
			}
			Damage = Math.Max(1, Math.Min(30000, Damage));
			int idx = Projectile.NewProjectile(position.X, position.Y, velocity.X, velocity.Y, Type, Damage, KnockBack, Owner, ai0, ai1);
			NetMessage.SendData((int)PacketTypes.ProjectileNew, -1, -1, null, idx);
			return idx;
		}
		#endregion
		#region SetLife
		public static void SetLife(this TSPlayer player, int Life)
		{
			player.TPlayer.SetLife(Life);
		}
		public static void SetLife(this Player player, int Life)
		{
			Life = Math.Min(Life, 30000);
			player.statLifeMax = Life;
			NetMessage.SendData((int)PacketTypes.PlayerHp, -1, -1, null, player.whoAmI);
		}
		#endregion
		#region SendCombatText
		public static void SendCombatMsg(this Entity entity, string msg, Color color)
		{
			NetMessage.SendData(Starver.CombatTextPacket, -1, -1, NetworkText.FromLiteral(msg), (int)color.PackedValue, entity.position.X + Rand.Next(entity.width), entity.position.Y + Rand.Next(entity.height), 0.0f, 0, 0, 0);
		}
		public static void SendCombatMsg(Vector2 pos, string msg, Color color)
		{
			NetMessage.SendData(Starver.CombatTextPacket, -1, -1, NetworkText.FromLiteral(msg), (int)color.PackedValue, pos.X, pos.Y, 0.0f, 0, 0, 0);
		}
		#endregion
		#region Vector2
		public static Vector2 FromPolar(double angle, float length)
		{
			return new Vector2((float)(Math.Cos(angle) * length), (float)(Math.Sin(angle) * length));
		}
		public static double Angle(this Vector2 vector)
		{
			return Math.Atan2(vector.Y, vector.X);
		}
		public static double Angle(ref this Vector2 vector, double rad)
		{
			vector = FromPolar(rad, vector.Length());
			return rad;
		}
		public static double AngleAdd(ref this Vector2 vector, double rad)
		{
			rad += Math.Atan2(vector.Y, vector.X);
			vector = FromPolar(rad, vector.Length());
			return rad;
		}
		public static Vector2 Deflect(this Vector2 vector2, double rad)
		{
			Vector2 vector = vector2;
			vector2.AngleAdd(rad);
			return vector;
		}
		public static void Length(ref this Vector2 vector, float length)
		{
			vector = FromPolar(vector.Angle(), length);
		}
		public static void LengthAdd(ref this Vector2 vector, float length)
		{
			vector = FromPolar(vector.Angle(), length + vector.Length());
		}
		public static Vector2 ToLenOf(this Vector2 vector, float length)
		{
			vector.Normalize();
			vector *= length;
			return vector;
		}
		public static Vector2 Symmetry(this Vector2 vector, Vector2 Center)
		{
			return Center * 2f - vector;
		}
		public static Vector2 Vertical(this Vector2 vector)
		{
			return new Vector2(-vector.Y, vector.X);
		}
		public static Vector ToVector(this Vector2 value)
		{
			return new Vector(value.X, value.Y);
		}
		public static Vector2 ToVector2(this Vector value)
		{
			return new Vector2(value.X, value.Y);
		}
		#endregion
		#region ReceiveDamage
		public static bool ReceiveDamage(this NPC npc)
		{
			if(npc.friendly)
			{
				return false;
			}
			if(npc.dontTakeDamage)
			{
				return false;
			}
			return true;
		}
		#endregion
		#region Rands
		/// <summary>
		/// 随机返回1或-1
		/// </summary>
		/// <param name="rand"></param>
		/// <returns></returns>
		public static int NextDirection(this Random rand)
		{
			return rand.Next(2) == 1 ? 1 : -1;
		}
		public static float NextFloat(this Random rand)
		{
			return (float)rand.NextDouble();
		}
		public static float NextFloat(this Random rand, float min, float max)
		{
			if (max < min)
			{
				throw new ArgumentException("最大值必须大等于最小值");
			}
			return (max - min) * rand.NextFloat() + min;
		}
		public static double NextAngle(this Random rand)
		{
			return rand.NextDouble() * Math.PI * 2;
		}
		public static T NextValue<T>(this Random rand, params T[] args)
		{
			int idx = rand.Next(args.Length);
			return args[idx];
		}
		/// <summary>
		/// 使用样例:
		/// <para>Range = PI / 12</para>
		/// 返回为 [-PI, PI) / (PI / (PI / 12)) = [-PI / 12, PI / 12)
		/// </summary>
		/// <param name="rand"></param>
		/// <param name="Range"></param>
		/// <returns></returns>
		public static double NextAngle(this Random rand, double Range)
		{
			return (rand.NextAngle() - Math.PI) / (Math.PI / Range);
		}
		public static double NextDouble(this Random rand, double min, double max)
		{
			if (max < min)
			{
				throw new ArgumentException("最大值必须大等于最小值");
			}
			return (max - min) * rand.NextDouble() + min;
		}
		public static Vector2 NextVector2(this Random rand, float Length)
		{
			return FromPolar(rand.NextAngle(), Length);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="rand"></param>
		/// <param name="X">实际变为-X / 2 ~ X / 2</param>
		/// <param name="Y">实际变为-Y / 2 ~ Y / 2</param>
		/// <returns></returns>
		public static Vector2 NextVector2(this Random rand, float X, float Y)
		{
			return new Vector2((float)((rand.NextDouble() - 0.5) * X), (float)((rand.NextDouble() - 0.5) * Y));
		}
		/// <summary>
		/// 随机获取一个元素
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <returns></returns>
		public static T Next<T>(this T[] array)
		{
			return array[Rand.Next(array.Length)];
		}
		public static T Next<T>(this IList<T> list)
		{
			return list[Rand.Next(list.Count)];
		}
		#endregion
		#region InRange
		/// <summary>
		/// 判断value是否在闭区间(left, right)内
		/// </summary>
		public static bool InRange(this float value, float left, float right)
		{
			return left < value && value < right;
		}
		#endregion
		#region SendData
		public static void SendData(this NPC npc)
		{
			try
			{
				NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, null, npc.whoAmI);
			}
			catch(KeyNotFoundException)
			{

			}
		}
		public static void SendData(this Projectile proj)
		{
			NetMessage.SendData((int)PacketTypes.ProjectileNew, -1, -1, null, proj.whoAmI);
		}
		public static void SendData(this Player player)
		{
			NetMessage.SendData((int)PacketTypes.PlayerUpdate, -1, -1, null, player.whoAmI);
		}
		#endregion
		#region NewItem
		public static int NewItem(int type, int stack = 1, int prefix = 0)
		{
			return Utils.NewItem(default, type, stack, prefix);
		}
		public static int NewItem(Vector2 position, int type, int stack = 1, int prefix = 0)
		{
			int idx = Item.NewItem((int)position.X, (int)position.Y, 0, 0, type, stack, false, prefix);
			return idx;
		}
		public static int AnalogNewItem(Vector2 position, int type, int stack = 1, int keepTime = 60 * 60)
		{
			int idx = 400;
			for (int i = 0; i < Main.item.Length; i++)
			{
				if (!Main.item[i].active)
				{
					idx = i;
					Main.item[i].netDefaults(type);
					Main.item[i].stack = stack;
					Main.item[i].keepTime = keepTime;
					Main.item[i].position = position;
					StarverPlayer.All.SendData(PacketTypes.UpdateItemDrop, "", i);
					break;
				}
			}
			return idx;
		}
		public static int NewItem(Vector position, int type, int stack = 1, int prefix = 0)
		{
			return Item.NewItem((int)position.X, (int)position.Y, 0, 0, type, stack, false, prefix);
		}
		#endregion
		#region TheWorldSkill
		#region ReadNPC
		public unsafe static void ReadNPCState(Vector2* NPCVelocity, int* NPCAI)
		{
			int t = -1;
			foreach (var npc in Terraria.Main.npc)
			{
				t++;
				if (!npc.active)
				{
					continue;
				}
				NPCVelocity[t] = npc.velocity;
				NPCAI[t] = npc.aiStyle;
				npc.aiStyle = -1;
				npc.velocity = Vector2.Zero;
				npc.SendData();
			}
		}
		#endregion
		#region ReadProj
		public unsafe static void ReadProjState(Vector2* ProjVelocity, int* ProjAI)
		{
			int t = -1;
			foreach (var proj in Terraria.Main.projectile)
			{
				t++;
				if (!proj.active)
				{
					continue;
				}
				ProjVelocity[t] = proj.velocity;
				ProjAI[t] = proj.aiStyle;
				proj.aiStyle = -1;
				proj.velocity = Vector2.Zero;
				proj.SendData();
			}
		}
		#endregion
		#region UpdateNPC
		public static void UpdateNPCState()
		{
			foreach (var npc in Terraria.Main.npc)
			{
				if (!npc.active)
				{
					continue;
				}
				npc.SendData();
			}
		}
		#endregion
		#region UpdateProj
		public static void UpdateProjState()
		{
			foreach (var proj in Terraria.Main.projectile)
			{
				if (!proj.active)
				{
					continue;
				}
				proj.SendData();
			}
		}
		#endregion
		#region RestoreNPC
		public unsafe static void RestoreNPCState(Vector2* NPCVelocity, int* NPCAI)
		{
			int t = -1;
			foreach (var npc in Terraria.Main.npc)
			{
				t++;
				if (!npc.active)
				{
					continue;
				}
				npc.velocity = NPCVelocity[t];
				npc.aiStyle = NPCAI[t];
				npc.SendData();
			}
		}
		#endregion
		#region RestoreProj
		public unsafe static void RestoreProjState(Vector2* ProjVelocity, int* ProjAI)
		{
			int t = -1;
			foreach (var proj in Terraria.Main.projectile)
			{
				t++;
				if (!proj.active)
				{
					continue;
				}
				proj.velocity = ProjVelocity[t];
				proj.aiStyle = ProjAI[t];
				proj.SendData();
			}
		}
		#endregion
		#endregion
		#region else
		public static void Exception(string message)
		{
			new Thread(() =>
			{
				throw new Exception(message);
			}).Start();
		}
		public static void Exception(Exception e)
		{
			new Thread(() =>
			{
				throw e;
			}).Start();
		}
		public static void KillMeEx(this Projectile proj)
		{
			proj.type = 0;
			proj.active = false;
			proj.SetDefaults(0);
			proj.SendData();
		}
		public static bool HasBuff(this NPC npc, int type)
		{
			int idx = -1;
			for (int i = 0; i < npc.buffType.Length; i++)
			{
				if(npc.buffType[i] == type && npc.buffTime[i] > 0)
				{
					idx = i;
					break;
				}
			}
			return idx != -1;
		}
		public static int ValidDefense(this NPC npc)
		{
			double defense = npc.defense;
			if(npc.ichor)
			{
				defense /= 2;
			}
			if(npc.onFire || npc.onFire2 || npc.onFrostBurn || npc.poisoned || npc.shadowFlame || npc.venom)
			{
				defense /= 1.5;
			}
			return (int)defense;
		}
		#endregion
	}
}
