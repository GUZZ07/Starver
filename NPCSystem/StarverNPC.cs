using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using TShockAPI;

namespace Starvers.NPCSystem
{
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public abstract class StarverNPC : BaseNPC, ICloneable
	{
		#region Enums
		#region SpawnSpaceOptions
		/// <summary>
		/// 选择NPC出生时的空间的条件
		/// </summary>
		[Flags]
		protected enum SpawnSpaceOptins
		{
			/// <summary>
			/// 无条件
			/// </summary>
			None = 0,
			/// <summary>
			/// 必须要有可以踩着的地面(需要与NoGraviry一同设置才能有效)
			/// </summary>
			StepableGround = 1 << 0,
			/// <summary>
			/// 必须没有液体
			/// </summary>
			NotWet = 1 << 1,
			/// <summary>
			/// 要生成在玩家屏幕内
			/// </summary>
			InScreen = 1 << 2
		}
		#endregion
		#endregion
		#region Fields
		protected bool afterMoon = true;
		protected bool dontNeedTarget;
		protected float[] AIUsing;
		protected int[] Types;
		protected int AIStyle = None;
		protected int RawType;
		protected int DefaultLife;
		protected int DefaultDefense;
		protected int CollideDamage;
		protected int Height = 3 * 14;
		protected int Width = 2 * 13;
		protected bool NoTileCollide;
		protected bool NoGravity;
		protected bool AfraidSun;
		protected bool EndTrialEnemy;
		protected DateTime LastSpawn = DateTime.Now;
		protected StarverNPC Root;
		protected SpawnChecker Checker;
		protected SpawnSpaceOptins SpaceOption = SpawnSpaceOptins.StepableGround;
		protected abstract void RealAI();
		#endregion
		#region Properties
		/// <summary>
		/// 碰撞伤害系数
		/// </summary>
		protected virtual float CollidingIndex { get; set; } = 1;
		protected virtual float DefenseIndex => (StarverConfig.Config.TaskNow - 20) / 2f;
		protected virtual bool Enabled => true;
		public override float DamageIndex => (float)Math.Sqrt(StarverConfig.Config.TaskNow - 20) / 5f + 1;
		public bool OverrideRawDrop { get; protected set; } = false;
		#endregion
		#region Ctor & Release
		protected StarverNPC(int AIs = 0)
		{
			AIUsing = new float[AIs];
			Name = GetType().Name;
		}
		internal static void Load()
		{
			LoadAssemblies();
			LoadNPCs();
			unsafe
			{
				Finded = (Vector*)System.Runtime.InteropServices.Marshal.AllocHGlobal(sizeof(Vector) * (8000)).ToPointer();
			}
		}
		public unsafe static void Release()
		{
			if (Finded != null)
			{
				System.Runtime.InteropServices.Marshal.FreeHGlobal((IntPtr)Finded);
				Finded = null;
			}
		}
		#endregion
		#region Spawn
		public virtual bool Spawn(Vector where)
		{
			_active = true;
			int life = DefaultLife;
			int defense = DefaultDefense;
			life += Starver.Config.TaskNow * 100;
			defense += Starver.Config.TaskNow * 60;
			if (Starver.Config.EvilWorld)
			{
				life += 4000;
				defense += 2000;
			}
			Index = NewNPC(where, Vector.Zero, RawType, life, defense);
			RealNPC.GivenName = Name;
			StarverPlayer.All.SendData(PacketTypes.UpdateNPCName, "", Index);
			return true;
		}
		#endregion
		#region DefenseIndex
		#endregion
		#region OnSpawn
		public override void OnSpawn()
		{
			#region Varient Types
			if (Types != null)
			{
				RawType = Types[Rand.Next(Types.Length)];
			}
			#endregion
			TransformTo(RawType);
			StarverPlayer.All.SendData(PacketTypes.UpdateNPCName, "", Index);
			SendData();
#if DEBUG
			StarverPlayer.All.SendDeBugMessage($"{Name}({this.Index}) has Spawned");
#endif
		}
		#endregion
		#region TransformTo
		protected void TransformTo(int type)
		{
			RawType = type;
			var (dl, ai, notc, def, nogra, dmg) = (RealNPC.life, RealNPC.aiStyle, RealNPC.noTileCollide, RealNPC.defense, RealNPC.noGravity, RealNPC.damage);
			RealNPC.SetDefaults(RawType);
			RealNPC.life = dl;
			RealNPC.lifeMax = dl;
			RealNPC.aiStyle = AIStyle;
			RealNPC.noTileCollide = notc;
			RealNPC.defense = def;
			RealNPC.noGravity = nogra;
			RealNPC.damage = dmg;
		}
		#endregion
		#region CheckSpawn
		/// <summary>
		/// 检查是否满足创建NPC条件
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		protected virtual bool CheckSpawn(StarverPlayer player)
		{
			bool Flag = 
				StarverConfig.Config.TaskNow >= Checker.Task &&
				Checker.Match(player.GetSpawnChecker()) && 
				SpawnTimer % Checker.SpawnRate == 0 && 
				Rand.NextDouble() < Checker.SpawnChance;
			if(BossSystem.Bosses.Base.StarverBoss.EndTrial)
			{
				Flag &= EndTrialEnemy;
			}
			return Flag;
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			return $"{Name}:Index({Index}),_active({_active}),RealNPC({RealNPC})";
		}
		#endregion
		#region CheckSecond
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		protected bool CheckSecond(double seconds)
		{
			return Timer % (uint)(seconds * 60) == 0;
		}
		#endregion
		#region Drop
		public void Drop(int idx)
		{
			if (Drops is null)
			{
				return;
			}
			foreach (var DropItem in Drops)
			{
				DropItem.Drop(Terraria.Main.npc[idx]);
			}
		}
		#endregion
		#region AI
		public override void AI(object args = null)
		{
			if (!Active)
			{
				if (0 <= Index && Index < NPCs.Length)
				{
					NPCs[Index] = null;
					Starver.NPCs[Index] = null;
				}
				return;
			}
			try
			{
				if (RealNPC.aiStyle == None)
				{
					if (!dontNeedTarget && (Target < 0 || Target >= Starver.Players.Length || TargetPlayer == null || !TargetPlayer.Active))
					{
						TargetClosest();
						if (Target == None || Target >= Starver.Players.Length || (Vector2.Distance(TargetPlayer.Center, Center) > 16 * 500 && !IgnoreDistance))
						{
							StarverPlayer.All.SendDeBugMessage($"{Name} killed by Invalid Target: {Target}");
							KillMe();
							return;
						}
					}
					Velocity = FakeVelocity;
				}
				if (Terraria.Main.dayTime && AfraidSun && RealNPC.Center.Y / 16 < Terraria.Main.rockLayer)
				{
					StarverPlayer.All.SendDeBugMessage($"{Name} Killed by Sun");
					KillMe();
					return;
				}
				if (TheWorld <= 0)
				{
					RealAI();
				}
			}
			catch (IndexOutOfRangeException)
			{
				StarverPlayer.All.SendDeBugMessage($"{Name} Invalid Target: {Target}");
				TargetClosest();
				if (!dontNeedTarget && (Target == None || Target >= Starver.Players.Length || (Vector2.Distance(TargetPlayer.Center, Center) > 16 * 500 && !IgnoreDistance)))
				{
					StarverPlayer.All.SendDeBugMessage($"{Name} killed by Invalid Target: {Target}");
					KillMe();
					return;
				}
			}
			if (Index != -1)
			{
				SendData(); //
			}
			++Timer;
		}
		#endregion
		#region DistanceToTarget
		public float DistanceToTarget()
		{
			return Vector2.Distance(Center, TargetPlayer.Center);
		}
		#endregion
		#region CalcSpawnPos
		protected unsafe Vector CalcSpawnPos(Vector PlayerCenter)
		{
			#region Search
			if(SpaceOption.HasFlag(SpawnSpaceOptins.InScreen))
			{
				return CalcSpawnPosInScreen(PlayerCenter);
			}
			Vector LeftUp = new Vector(PlayerCenter.X - 16 * 50 - 16 * 10, PlayerCenter.Y - 16 * 25 - 16 * 5);
			t = 0;
			float i, LimY, j, LimX;
			for (i = PlayerCenter.Y - 25 * 16 - 5 * 16, LimY = PlayerCenter.Y - 25 * 16; i < LimY; i += 16)
			{
				for (j = LeftUp.X, LimX = PlayerCenter.X + 50 * 16 + 10 * 16; j < LimX; j += 16)
				{
					if (CheckSpace(j, i))
					{
						Finded[t].X = j;
						Finded[t].Y = i;
						t++;
					}
				}
			}  //搜索玩家上方是否有空位
			for (i = PlayerCenter.Y + 25 * 16, LimY = PlayerCenter.Y + 25 * 16 + 5 * 16; i < LimY; i += 16)
			{
				for (j = LeftUp.X, LimX = PlayerCenter.X + 50 * 16 + 10 * 16; j < LimX; j += 16)
				{
					if (CheckSpace(j, i))
					{
						Finded[t].X = j;
						Finded[t].Y = i;
						t++;
					}
				}
			}  //搜索玩家下方是否有空位
			for (i = PlayerCenter.Y - 25 * 16, LimY = PlayerCenter.Y + 25 * 16; i < LimY; i += 16)
			{
				for (j = PlayerCenter.X - 50 * 16 - 10 * 16, LimX = PlayerCenter.X - 50 * 16; j < LimX; j += 16)
				{
					if (CheckSpace(j, i))
					{
						Finded[t].X = j;
						Finded[t].Y = i;
						t++;
					}
				}
			}  //搜索玩家左边
			for (i = PlayerCenter.Y - 25 * 16, LimY = PlayerCenter.Y + 25 * 16; i < LimY; i += 16)
			{
				for (j = PlayerCenter.X + 50 * 16, LimX = PlayerCenter.X + 50 * 16 + 10 * 16; j < LimX; j += 16)
				{
					if (CheckSpace(j, i))
					{
						Finded[t].X = j;
						Finded[t].Y = i;
						t++;
					}
				}
			}  //搜索玩家右边
			#endregion
			#region return
			if (t > 0)
			{
				return Finded[Rand.Next(t)];
			}
			if (NoTileCollide)
			{
				return (Vector)(PlayerCenter + Rand.NextVector2(16 * 50));
			}
			else
			{
				throw new Exception("没有合适的出生点");
			}
			#endregion
		}
		protected unsafe Vector CalcSpawnPosInScreen(Vector PlayerCenter)
		{
			#region Search
			Vector LeftUp = new Vector(PlayerCenter.X - 16 * 50, PlayerCenter.Y - 16 * 25);
			t = 0;
			float i, LimY, j, LimX;
			for (i = PlayerCenter.Y - 25 * 16, LimY = PlayerCenter.Y + 25 * 16; i < LimY; i += 16)
			{
				for (j = LeftUp.X - 50 * 16, LimX = PlayerCenter.X + 50 * 16; j < LimX; j += 16)
				{
					if (CheckSpace(j, i))
					{
						Finded[t].X = j;
						Finded[t].Y = i;
						t++;
					}
				}
			}  //搜索玩家屏幕内是否有空位
			#endregion
			#region return
			if (t > 0)
			{
				return Finded[Rand.Next(t)];
			}
			if (NoTileCollide)
			{
				return (Vector)(PlayerCenter + Rand.NextVector2(16 * 50));
			}
			else
			{
				throw new Exception("没有合适的出生点");
			}
			#endregion
		}
		#endregion
		#region CheckSpaceOptions
		#region Wet
		private static bool NotWet(int i, int j, int HeightNeed, int WidthNeed)
		{
			int StartedJ = j;
			for (; i < HeightNeed; i++)
			{
				for (j = StartedJ; j < WidthNeed; j++)
				{
					if (Terraria.Main.tile[j, i].liquid != 0)
					{
						return false;
					}
				}
			}
			return true;
		}
		#endregion
		#region Ground
		/// <summary>
		/// 检查是否有可以供踩踏的地面
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="HeightNeed"></param>
		/// <param name="WidthNeed"></param>
		/// <returns></returns>
		private bool HasGround(int i, int j, int WidthNeed)
		{
			i += (int)Math.Ceiling(Height / 16.0);
			for (; j < WidthNeed; j++)
			{
				if (Terraria.Main.tile[j, i].active())
				{
					return true;
				}
			}
			return false;
		}
		#endregion
		#endregion
		#region CheckSpace
		/// <summary>
		/// 检查以where为左上角的空间是否可以容纳NPC
		/// </summary>
		/// <param name="where"></param>
		/// <returns>空间的左上角</returns>
		private bool CheckSpace(Vector where)
		{
			return CheckSpace(where.X, where.Y);
		}
		/// <summary>
		/// 检查以where为左上角的空间是否可以容纳NPC
		/// </summary>
		/// <param name="where"></param>
		/// <returns>空间的左上角</returns>
		private bool CheckSpace(float X,float Y)
		{
			int i = (int)(Y / 16);
			int j = (int)(X / 16);
			if (Terraria.Main.tile[j, i].wall != 0)
			{
				return false;
			}
			int HeightNeed = i + (int)Math.Ceiling(Height / 16.0);
			int WidthNeed = j + (int)Math.Ceiling(Width / 16.0);
			#region CheckSpaceOptions
			#region Wet
			if (SpaceOption.HasFlag(SpawnSpaceOptins.NotWet) && !NotWet(i, j, HeightNeed, WidthNeed))
			{
				return false;
			}
			#endregion
			#region Ground
			if (SpaceOption.HasFlag(SpawnSpaceOptins.StepableGround) && NoGravity == false && !HasGround(i, j, WidthNeed))
			{
				return false;
			}
			#endregion
			#endregion
			int StartedJ = j;
			for (; i < HeightNeed; i++)
			{
				for (j = StartedJ; j < WidthNeed; j++)
				{
					if (Terraria.Main.tile[j, i].active())
					{
						return false;
					}
				}
			}
			return true;
		}
		#endregion
		#region Collide
		internal static void Collide()
		{
			bool Handled;
			Terraria.NPC RealNPC;
			for (int i = 0; i < Terraria.Main.maxNPCs; i++)
			{
				if (!Terraria.Main.npc[i].active)
				{
					continue;
				}
				if (Terraria.Main.npc[i].friendly)
				{
					continue;
				}
				if (Terraria.Main.npc[i].damage < 1)
				{
					continue;
				}
				foreach (var ply in Starver.Players)
				{
					if (ply == null || !ply.Active)
					{
						continue;
					}
					RealNPC = Terraria.Main.npc[i];
					Handled = false;
					if (
						RealNPC.position.X - ply.TPlayer.width < ply.TPlayer.position.X &&
						ply.TPlayer.position.X < RealNPC.position.X + RealNPC.width &&
						RealNPC.position.Y - ply.TPlayer.height < ply.Position.Y &&
						ply.Position.Y < RealNPC.position.Y + RealNPC.height
						)
					{
						if (NPCs[i] != null && NPCs[i].Active)
						{
							NPCs[i].OnCollide(ply, ref Handled);
						}
						int damage = RealNPC.damage;
						if (ply.TPlayer.NPCBannerBuff[Terraria.Item.NPCtoBanner(RealNPC.type)])
						{
							damage /= 2;
						}
						if ((!Handled) && damage > 0 && !ply.TPlayer.immune)
						{
							if (Starver.IsPE)
							{
								ply.Damage(damage);
							}
							else
							{
								OfPC1353(ply, RealNPC, damage);
							}
						}
					}
				}
			}
		}
		private static void OfPC1353(StarverPlayer ply, Terraria.NPC npc, int damage)
		{
			ply.Damage(damage, PlayerDeathReason.ByNPC(npc.whoAmI));
		}
		#endregion
		#region OnCollide
		/// <summary>
		/// 
		/// </summary>
		/// <param name="player">撞上那个玩家</param>
		/// <param name="handled">是否处理完毕</param>
		protected virtual void OnCollide(StarverPlayer player, ref bool handled)
		{

		}
		#endregion
		#region Globals
		private static int t;
		private unsafe static Vector* Finded;
		private static DirectoryInfo NPCFolder;
		protected static int SpawnTimer;
		protected static int counting;
		protected static int count;
		protected static int Count
		{
			get
			{
				return count;
			}
			set
			{
				count = value;
#if DEBUG
				StarverPlayer.All.SendDeBugMessage($"Count: {count}, MaxSpawns: {TShock.Config.DefaultMaximumSpawns * Alives}");
#endif
			}
		}
		protected static int Alives;
		protected static List<System.Reflection.Assembly> NPCPlugins { get; private set; } = new List<System.Reflection.Assembly>();
		protected static List<Type> NPCTypes { get; private set; } = new List<Type>();
		protected internal static List<StarverNPC> RootNPCs { get; private set; } = new List<StarverNPC>();
		protected static int NewNPC<T>(Vector where, Vector Velocity)
			where T : StarverNPC, new()
		{
			StarverNPC npc = new T();
			npc._active = true;
			npc.Index = NewNPCStatic(where, Velocity, npc.RawType, npc.DefaultLife, npc.DefaultDefense);
			npc.OnSpawn();
			Starver.NPCs[npc.Index] = NPCs[npc.Index] = npc;
			return npc.Index;
		}
		protected static int NewNPC(Vector where, Vector Velocity, Type NPCType)
		{
			if (NPCType.IsAbstract == false && NPCType.IsSubclassOf(typeof(StarverNPC)))
			{
				StarverNPC npc = Activator.CreateInstance(NPCType) as StarverNPC;
				npc._active = true;
				npc.Index = NewNPCStatic(where, Velocity, npc.RawType, npc.DefaultLife, npc.DefaultDefense);
				npc.OnSpawn();
				Starver.NPCs[npc.Index] = NPCs[npc.Index] = npc;
				return npc.Index;
			}
			else
			{
				throw new ArgumentException($"{NPCType.Name}不是StarverNPC的子类或是抽象类");
			}
		}
		protected internal static int NewNPC(Vector where, Vector Velocity, StarverNPC Root)
		{
			StarverNPC npc = Root.Clone() as StarverNPC;
			npc._active = true;
			npc.Spawn(where);
			npc.RealNPC.velocity = Velocity;
			npc.OnSpawn();
			Starver.NPCs[npc.Index] = NPCs[npc.Index] = npc;
			return npc.Index;
		}
		protected static bool IsBossFollow(Type type)
		{
			return type == typeof(NPCs.BrainFollow) || type == typeof(NPCs.PrimeExArm);
		}
		protected static void UpdateCount()
		{
			counting = 0;
			foreach(var npc in NPCs)
			{
				if(npc is null)
				{
					continue;
				}
				if(npc.Active)
				{
					counting++;
				}
			}
			if (Count != counting)
			{
				Count = counting;
			}
		}
		private static void LoadAssemblies()
		{
			NPCFolder = Starver.NPCFolder;
			NPCPlugins.Add(System.Reflection.Assembly.GetExecutingAssembly());
			FileInfo[] Files = NPCFolder.GetFiles("*.dll");
			foreach(var file in Files)
			{
				try
				{
					NPCPlugins.Add(System.Reflection.Assembly.LoadFrom(file.FullName));
				}
				catch(Exception e)
				{
					TShock.Log.ConsoleError(e.ToString());
				}
			}
			foreach(var Assembly in NPCPlugins)
			{
				LoadTypes(Assembly);
			}
		}
		private static void LoadTypes(System.Reflection.Assembly Assembly)
		{
			Type[] types = Assembly.GetTypes();
			foreach (var type in types)
			{
				if (type.IsAbstract || IsBossFollow(type) || !type.IsSubclassOf(typeof(StarverNPC)))
				{
					continue;
				}
				NPCTypes.Add(type);
			}
		}
		private static void LoadNPCs()
		{
			StarverNPC npc;
			foreach(var type in NPCTypes)
			{
				npc = Activator.CreateInstance(type) as StarverNPC;
				if (npc.Enabled)
				{
					RootNPCs.Add(npc);
				}
#if DEBUG
				//StarverPlayer.All.SendDeBugMessage($"{RootNPCs[RootNPCs.Count - 1].Name} Loaded");
				Console.ForegroundColor = ConsoleColor.Blue;
				Console.WriteLine($"{RootNPCs[RootNPCs.Count - 1].Name} Loaded");
				Console.ResetColor();
#endif
			}
		}




		public static int TheWorld { get; set; }
		public static StarverNPC[] NPCs { get; } = new StarverNPC[Terraria.Main.maxNPCs];
		public static void DoUpDate(object args)
		{
			SpawnTimer++;
			if(SpawnTimer % 60 == 0)
			{
				UpdateCount();
				Alives = AlivePlayers();
			}
			foreach (var player in Starver.Players)
			{
				if (player is null || !player.Active || player.IsGuest)
				{
					continue;
				}
				foreach (var npc in RootNPCs)
				{
					if (Count < Alives * TShock.Config.DefaultMaximumSpawns && npc.CheckSpawn(player)) 
					{
						try
						{
							NewNPC(npc.CalcSpawnPos((Vector)player.Center), Vector.Zero, npc);
						}
						catch
						{
							
						}
					}
				}
			}
		}
		public static void OnNPCKilled(TerrariaApi.Server.NpcKilledEventArgs args)
		{
			StarverNPC npc = NPCs[args.npc.whoAmI];
			if (npc._active)
			{
				npc._active = false;
				npc.OnKilled();
				npc.KillMe();
			}
		}
		#endregion
		#region Clone
		public object Clone()
		{
			return MemberwiseClone();
		}
		#endregion
	}
}
