using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Starvers.Enemies.Npcs
{
	public abstract class StarverNPC : StarverEnemy
	{
		#region Enums
		#region SpawnSpaceOptions
		/// <summary>
		/// 选择NPC出生时的空间的条件
		/// </summary>
		[Flags]
		public enum SpawnSpaceOptions
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
		/// <summary>
		/// 给npc的弹幕伤害用的
		/// </summary>
		protected double DamageIndex;
		protected Random rand;
		protected bool noTileCollide;
		protected bool noGravity;
		private Vector sSize;
		#endregion
		#region Properties
		public SpawnSpaceOptions SpaceOption
		{
			get;
			protected set;
		}
		public int Height
		{
			get => TNPC.height;
			set => TNPC.height = value;
		}
		public int Width
		{
			get => TNPC.width;
			set => TNPC.width = value;
		}
		public bool NoTileCollide
		{
			get => TNPC.noTileCollide;
			set => TNPC.noTileCollide = value;
		}
		public bool NoGravity
		{
			get => TNPC.noGravity;
			set => TNPC.noGravity = value;
		}
		public bool OverrideRawDrop
		{
			get;
			protected set;
		}
		#endregion
		#region Ctor
		public StarverNPC(Vector size)
		{
			DamageIndex = 1;
			rand = new Random();
			sSize = size;
		}
		#endregion
		#region Methods
		protected virtual StarverNPC Clone()
		{
			return (StarverNPC)MemberwiseClone();
		}
		protected virtual void Spawn(Vector pos)
		{
			// int slot = Utils.FindEmptyNPCSlot(0);
			int slot = NPC.NewNPC(0, 0, 1);
			Index = slot;
			// Main.npc[slot] = new NPC();
			// Main.npc[slot].whoAmI = Index;
			Main.npc[slot].active = true;
			Main.npc[slot].aiStyle = -1;
			Main.npc[slot].noTileCollide = noTileCollide;
			Main.npc[slot].noGravity = noGravity;
			Initialize();
			TNPC.netID = TNPC.type;
			TNPC.position = pos;
			TNPC.SendData();
		}
		public abstract void AI();
		public abstract void Initialize();
		public abstract void DropItems();
		public abstract bool CheckSpawn(StarverPlayer player);
		public virtual StarverNPC ForceSpawn(Vector pos)
		{
			var npc = Clone();
			npc.Spawn(pos);
			return npc;
		}
		public virtual bool TrySpawnNewNpc(StarverPlayer player, out StarverNPC npc)
		{
			var pos = CalcSpawnPos((Vector)player.Center);
			if (pos == null)
			{
				npc = null;
				return false;
			}
			npc = Clone();
			npc.Spawn((Vector)pos);
			return true;
		}
		public override void Kill()
		{
			DropItems();
			TNPC.life = 0;
			TNPC.checkDead();
		}
		#endregion
		#region CheckSpawnSpace
		#region CalcSpawnPos
		protected Vector? CalcSpawnPos(Vector PlayerCenter)
		{
			#region Search
			if (SpaceOption.HasFlag(SpawnSpaceOptions.InScreen))
			{
				return CalcSpawnPosInScreen(PlayerCenter);
			}
			var vectors = new List<Vector>(10);
			Vector LeftUp = new Vector(PlayerCenter.X - 16 * 50 - 16 * 10, PlayerCenter.Y - 16 * 25 - 16 * 5);
			var t = 0;
			float i, LimY, j, LimX;
			for (i = PlayerCenter.Y - 25 * 16 - 5 * 16, LimY = PlayerCenter.Y - 25 * 16; i < LimY; i += 16)
			{
				for (j = LeftUp.X, LimX = PlayerCenter.X + 50 * 16 + 10 * 16; j < LimX; j += 16)
				{
					if (CheckSpace(j, i))
					{
						vectors.Add((j, i));
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
						vectors.Add((j, i));
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
						vectors.Add((j, i));
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
						vectors.Add((j, i));
						t++;
					}
				}
			}  //搜索玩家右边
			#endregion
			#region return
			if (t > 0)
			{
				return vectors[rand.Next(t)];
			}
			if (noTileCollide)
			{
				return (Vector)(PlayerCenter + rand.NextVector2(16 * 50));
			}
			else
			{
				return null;
			}
			#endregion
		}
		protected Vector? CalcSpawnPosInScreen(Vector PlayerCenter)
		{
			#region Search
			var vectors = new List<Vector>(10);
			Vector LeftUp = new Vector(PlayerCenter.X - 16 * 50, PlayerCenter.Y - 16 * 25);
			var t = 0;
			float i, LimY, j, LimX;
			for (i = PlayerCenter.Y - 25 * 16, LimY = PlayerCenter.Y + 25 * 16; i < LimY; i += 16)
			{
				for (j = LeftUp.X - 50 * 16, LimX = PlayerCenter.X + 50 * 16; j < LimX; j += 16)
				{
					if (CheckSpace(j, i))
					{
						vectors.Add((j, i));
						t++;
					}
				}
			}  //搜索玩家屏幕内是否有空位
			#endregion
			#region return
			if (t > 0)
			{
				return vectors[rand.Next(t)];
			}
			if (noTileCollide)
			{
				return (Vector)(PlayerCenter + rand.NextVector2(16 * 50));
			}
			else
			{
				return null;
			}
			#endregion
		}
		#endregion
		#region CheckSpaceOptions
		#region Wet
		private static bool NotWet(int i, int j, int heightRequired, int widthRequired)
		{
			int StartedJ = j;
			for (; i < heightRequired; i++)
			{
				for (j = StartedJ; j < widthRequired; j++)
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
		/// <param name="heightRequired"></param>
		/// <param name="widthRequired"></param>
		/// <returns></returns>
		private bool HasGround(int i, int j, int widthRequired)
		{
			i += (int)Math.Ceiling(Height / 16.0);
			for (; j < widthRequired; j++)
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
		private bool CheckSpace(float X, float Y)
		{
			int i = (int)(Y / 16);
			int j = (int)(X / 16);
			if(i<0 || j < 0 || i >= Main.maxTilesX || j >= Main.maxTilesY)
			if (Terraria.Main.tile[j, i].wall != 0)
			{
				return false;
			}
			int heightRequired = i + (int)Math.Ceiling(sSize.Y / 16.0);
			int widthRequired = j + (int)Math.Ceiling(sSize.X / 16.0);
			#region CheckSpaceOptions
			#region Wet
			if (SpaceOption.HasFlag(SpawnSpaceOptions.NotWet) && !NotWet(i, j, heightRequired, widthRequired))
			{
				return false;
			}
			#endregion
			#region Ground
			if (SpaceOption.HasFlag(SpawnSpaceOptions.StepableGround) && !noGravity && !HasGround(i, j, widthRequired))
			{
				return false;
			}
			#endregion
			#endregion
			int StartedJ = j;
			for (; i < heightRequired; i++)
			{
				for (j = StartedJ; j < widthRequired; j++)
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
		#endregion
	}
}
