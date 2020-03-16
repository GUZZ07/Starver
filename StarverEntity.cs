using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Terraria;
using TShockAPI;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using BigInt = System.Numerics.BigInteger;

namespace Starvers
{
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public abstract partial class StarverEntity
	{
		#region Properties
		public static Projectile[] Projs => Main.projectile;
		public static Random Rand => Starver.Rand;
		public int Index { get; protected set; } = -1;
		public int Target
		{
			get => RealNPC.target;
			set => RealNPC.target = value;
		}
		public int Interval { get; protected set; }
		public float[] Rawai => RealNPC.ai;
		public DateTime LastUpdate { get; protected set; } = DateTime.Now;
		public StarverPlayer TargetPlayer => Starver.Players[Target];
		public NPC RealNPC => Main.npc[Index];
		public Vector2 Center
		{
			get => RealNPC.Center;
			set => RealNPC.Center = value;
		}
		public ref Vector2 Position => ref RealNPC.position;
		public ref Vector2 Velocity => ref RealNPC.velocity;
		public bool OverrideDead { get; protected set; }
		public bool Active => _active && Index != -1 && RealNPC.active;
		public virtual float DamageIndex { get; set; } = 1;
		public float DamagedIndex { get; protected set; } = 1;
		public int RealLife
		{
			get => RealNPC.realLife;
			set => RealNPC.realLife = value;
		}
		public bool DontTakeDamage
		{
			get => RealNPC.dontTakeDamage;
			set => RealNPC.dontTakeDamage = value;
		}
		public bool IgnoreDistance { get; set; } 
		public int Life
		{
			get => RealNPC.life;
			set => RealNPC.life = value;
		}
		public int LifeMax
		{
			get => RealNPC.lifeMax;
			set => RealNPC.lifeMax = value;
		}
		public int Defense
		{
			get => RealNPC.defense;
			set => RealNPC.defense = value;
		}
		#endregion
		#region Fields
		internal bool _active;
		protected DropItem[] Drops;
		protected Vector Vel;
		protected uint Timer;
		protected const int None = -1;
		protected Vector FakeVelocity;
		internal int Level;
		internal BigInt ExpGive = 1;
		internal int Client = -1;
		public const float PI = (float)Math.PI;
		public const int CriticalLevel = 2000;
		public const int BaseLevel = 2000;
		#endregion
		#region Ctor
		protected StarverEntity()
		{

		}
		public StarverEntity(int idx)
		{
			Index = idx;
			ExpGive = Main.npc[Index].lifeMax * StarverConfig.Config.TaskNow;
		}
		#endregion
		#region Utils
		#region FromPolar
		public static Vector FromPolar(double rad, float length)
		{
			return Vector.FromPolar(rad, length);
		}
		#endregion
		#region NewProj
		public static int NewProj(Vector2 position, Vector2 velocity, int Type, int Damage, float KnockBack = 3f, float ai0 = 0, float ai1 = 0)
		{
			return Utils.NewProj(position, velocity, Type, Damage, KnockBack, Main.myPlayer, ai0, ai1);
		}
		public static int NewProj(Vector2 position, Vector2 velocity, int Type, int Damage, int owner, float KnockBack, float ai0 = 0, float ai1 = 0)
		{
			return Utils.NewProj(position, velocity, Type, Damage, KnockBack, owner, ai0, ai1);
		}
		#endregion
		#region Proj
		public int Proj(Vector2 position, Vector2 velocity, int Type, int Damage, float KnockBack = 10f, float ai0 = 0, float ai1 = 0)
		{
			int idx = NewProj(position, velocity, Type, (int)(DamageIndex * Damage + Rand.Next(-10, 10)), KnockBack, ai0, ai1);
			return idx;
		}
		#endregion
		#region NewProjCircle
		/// <summary>
		/// 弹幕圆
		/// </summary>
		/// <param name="Center"></param>
		/// <param name="r"></param>
		/// <param name="speed">速率</param>
		/// <param name="Type"></param>
		/// <param name="number">弹幕总数</param>
		/// <param name="Damage">伤害</param>
		/// <param name="direction">0:不动 1:向内 2:向外</param>
		public static void NewProjCircle(Vector2 Center, int r, float speed, int Type, int number, int Damage, byte direction = 1, float ai0 = 0, float ai1 = 0)
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
			for (int i = 0; i < number; i++)
			{
				NewProj(Center + FromPolar(averagerad * i, r), FromPolar(averagerad * i, -speed), Type, Damage, 4f, ai0, ai1);
			}
		}
		/// <summary>
		/// 多向发射
		/// </summary>
		/// <param name="speed">速率</param>
		/// <param name="rotation">旋转角</param>
		/// <param name="type">类型</param>
		/// <param name="number">数量</param>
		/// <param name="damage">伤害</param>
		/// <param name="ai0"></param>
		/// <param name="ai1"></param>
		public void LaunchProjs(float speed, double rotation, int type, int number, int damage, float ai0 = 0, float ai1 = 0)
		{
			double averagerad = Math.PI * 2 / number;
			for (int i = 0; i < number; i++)
			{
				NewProj(Center, FromPolar(rotation + averagerad * i, -speed), type, damage, 4f, ai0, ai1);
			}
		}
		/// <summary>
		/// 弹幕圆
		/// </summary>
		/// <param name="Center"></param>
		/// <param name="r"></param>
		/// <param name="speed">速率</param>
		/// <param name="Type"></param>
		/// <param name="number">弹幕总数</param>
		/// <param name="Damage">伤害</param>
		public static void NewProjCircle(int owner, Vector2 Center, int r, float speed, int Type, int number, int Damage, float ai0 = 0, float ai1 = 0)
		{
			double averagerad = Math.PI * 2 / number;
			for (int i = 0; i < number; i++)
			{
				NewProj(Center + FromPolar(averagerad * i, r), FromPolar(averagerad * i, -speed), Type, Damage, owner, 4f, ai0, ai1);
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
		/// <param name="Damage">伤害(已被加成)</param>
		public void ProjCircle(Vector2 Center, float r, float speed, int Type, int number, int Damage, float ai0 = 0, float ai1 = 0)
		{
			double averagerad = Math.PI * 2 / number;
			for (int i = 0; i < number; i++)
			{
				Proj(Center + FromPolar(averagerad * i, r), FromPolar(averagerad * i, -speed), Type, Damage, 4f, ai0, ai1);
			}
		}
		/// <summary>
		/// 弹幕圆(返回所有索引)
		/// </summary>
		/// <param name="Center"></param>
		/// <param name="r"></param>
		/// <param name="Vel">速率</param>
		/// <param name="Type"></param>
		/// <param name="number">弹幕总数</param>
		/// <param name="Damage">伤害(已被加成)</param>
		public int[] ProjCircleWithReturn(Vector2 Center, float r, float Vel, int Type, int number, int Damage, float ai0 = 0, float ai1 = 0)
		{
			int[] Indexes = new int[number];
			double averagerad = Math.PI * 2 / number;
			for (int i = 0; i < number; i++)
			{
				Indexes[i] = Proj(Center + FromPolar(averagerad * i, r), FromPolar(averagerad * i, -Vel), Type, Damage, 4f, ai0, ai1);
			}
			return Indexes;
		}
		/// <summary>
		/// 弹幕圆(返回所有索引)
		/// </summary>
		/// <param name="Center"></param>
		/// <param name="r"></param>
		/// <param name="Vel">速度</param>
		/// <param name="Type"></param>
		/// <param name="number">弹幕总数</param>
		/// <param name="Damage">伤害(已被加成)</param>
		public int[] ProjCircleWithReturn(Vector2 Center, float r, Vector2 Vel, int Type, int number, int Damage, float ai0 = 0, float ai1 = 0)
		{
			int[] Indexes = new int[number];
			double averagerad = Math.PI * 2 / number;
			for (int i = 0; i < number; i++)
			{
				Indexes[i] = Proj(Center + FromPolar(averagerad * i, r), Vel, Type, Damage, 4f, ai0, ai1);
			}
			return Indexes;
		}
		#endregion
		#region NewProjSector
		/// <summary>
		/// 扇形弹幕
		/// </summary>
		/// <param name="Center">圆心</param>
		/// <param name="speed">速率</param>
		/// <param name="r">半径</param>
		/// <param name="interrad">中心半径的方向</param>
		/// <param name="rad">张角</param>
		/// <param name="Damage">伤害</param>
		/// <param name="Type"></param>
		/// <param name="num">数量</param>
		/// <param name="direction">0:不动 1:向内 2:向外</param>
		/// <param name="ai0"></param>
		/// <param name="ai1"></param>
		public static void NewProjSector(Vector2 Center, float speed, float r, double interrad, double rad, int Damage, int Type, int num, byte direction = 2, float ai0 = 0, float ai1 = 0)
		{
			if (num == 1)
			{
				NewProj(Center, FromPolar(interrad, speed * -direction), Type, Damage);
			}
			double start = interrad - rad / 2;
			double average = rad / (num - 1);
			switch (direction)
			{
				case 0:
					speed *= 0;
					break;
				case 1:
					speed *= 1;
					break;
				case 2:
					speed *= -1;
					break;
			}
			for (int i = 0; i < num; i++)
			{
				NewProj(Center + FromPolar(start + i * average, r), FromPolar(start + i * average, speed), Type, Damage, 4f, ai0, ai1);
			}
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
		/// <param name="Type"></param>
		/// <param name="num">数量</param>
		/// <param name="ai0"></param>
		/// <param name="ai1"></param>
		public void ProjSector(Vector2 Center, float speed, float r, double interrad, double rad, int Damage, int Type, int num, float ai0 = 0, float ai1 = 0)
		{
			if (num == 1)
			{
				Proj(Center, FromPolar(interrad, speed), Type, Damage);
				return;
			}
			double start = interrad - rad / 2;
			double average = rad / (num - 1);
			for (int i = 0; i < num; i++)
			{
				Proj(Center + FromPolar(start + i * average, r), FromPolar(start + i * average, speed), Type, Damage, 4f, ai0, ai1);
			}
		}
		#endregion
		#region NewProjLine
		public static void NewProjLine(Vector2 Begin, Vector2 End, Vector2 Vel, int num, int Damage, int type, float ai0 = 0, float ai1 = 0)
		{
			Vector2 average = Begin - End;
			average /= num;
			for (int i = 0; i < num; i++)
			{
				NewProj(Begin + average * i, Vel, type, Damage, 3f, ai0, ai1);
			}
		}
		#endregion
		#region ProjLine
		public void ProjLine(Vector2 Begin, Vector2 End, Vector2 Vel, int num, int Damage, int type, float ai0 = 0, float ai1 = 0)
		{
			Vector2 average = End - Begin;
			average /= num;
			for (int i = 0; i <= num; i++)
			{
				Proj(Begin + average * i, Vel, type, Damage, 3f, ai0, ai1);
			}
		}
		#endregion
		#region SendCombatText
		public void SendCombatMsg(string msg, Color color)
		{
			RealNPC.SendCombatMsg(msg, color);
		}
		#endregion
		#endregion
		#region Methods
		#region AI
		public delegate void AIDelegate();
		public virtual void AI(object args = null)
		{
			if (!Active)
			{
				return;
			}
			if (RealNPC.aiStyle == None)
			{
				if (Target < 0 || Target > Starver.Players.Length || TargetPlayer == null || !TargetPlayer.Active)
				{
					TargetClosest();
					if (Target == None || Vector2.Distance(TargetPlayer.Center, Center) > 16 * 400)
					{
						KillMe();
					}
				}

			}
			else
			{
				++Timer;
				Velocity = FakeVelocity;
				SendData();
			}
		}
		#endregion
		#region StrikeMe
		public virtual void StrikeMe(int Damage, float knockback, StarverPlayer player)
		{
			RealNPC.playerInteraction[player.Index] = true;
			int realdamage = (int)(Damage * (Rand.NextDouble() - 0.5) / 4 - RealNPC.defense * 0.5);
			RealNPC.life = Math.Max(RealNPC.life - realdamage, 0);
			SendCombatMsg(realdamage.ToString(), Color.Yellow);
			if (RealNPC.life < 1)
			{
				RealNPC.checkDead();
				OnKilled();
				KillMe();
			}
			else
			{
				Velocity.LengthAdd(knockback * (1f - RealNPC.knockBackResist));
				SendData();
			}
		}
		public virtual void PreStrike(ref int RealDamage, float KnockBack, StarverPlayer player)
		{

		}
		public virtual void OnStrike(int RealDamage, float KnockBack, StarverPlayer player)
		{

		}
		#endregion
		#region OnDead
		public virtual void OnKilled()
		{

		}
		#endregion
		#region OnSpawn
		public virtual void OnSpawn()
		{

		}
		#endregion
		#region TargetClosest
		public void TargetClosest()
		{
			Target = -1;
			for (int i = 0; i < Starver.Players.Length; i++)
			{
				if (Starver.Players[i] == null || !Starver.Players[i].Active)
				{
					continue;
				}
				else if (Target == -1)
				{
					Target = i;
				}
				else if (Vector2.Distance(Center, Starver.Players[i].Center) < Vector2.Distance(Center, Starver.Players[Target].Center))
				{
					Target = i;
				}
			}
		}
		#endregion
		#region AlivePlayers
		public static int AlivePlayers()
		{
			int total = 0;
			for (int i = 0; i < Starver.Players.Length; i++)
			{
				if (Starver.Players[i] == null || !Starver.Players[i].Active)
				{
					continue;
				}
				total++;
			}
			return total;
		}
		#endregion
		#region Killme
		public virtual void KillMe()
		{
			try
			{
				RealNPC.active = false;
				_active = false;
				SendData();
			}
			catch (Exception e)
			{
#if DEBUG
				StarverPlayer.All.SendMessage(e.ToString(), Color.Red);
				TSPlayer.Server.SendErrorMessage(e.ToString());
#endif
			}
		}
		#endregion
		#region NewNPC
		protected int NewNPC(Vector pos, Vector vel, int type, float ai0 = 0, float ai1 = 0, float ai2 = 0, float ai3 = 0)
		{
			int idx = NPC.NewNPC((int)pos.X, (int)pos.Y, type, 0, ai0, ai1, ai2, ai3);
			Main.npc[idx].velocity = vel;
			SendData(idx);
			return idx;
		}
		protected int NewNPC(Vector pos, Vector vel, int type, int lifeMax, int defense, int start = 0)
		{
			int idx = NPC.NewNPC((int)pos.X, (int)pos.Y, type, start);
			Main.npc[idx].velocity = vel;
			Main.npc[idx].lifeMax = lifeMax;
			Main.npc[idx].life = lifeMax - 1;
			Main.npc[idx].defense = defense;
			SendData(idx);
			return idx;
		}
		protected static int NewNPCStatic(Vector pos, Vector vel, int type, int lifeMax, int defense)
		{
			int idx = NPC.NewNPC((int)pos.X, (int)pos.Y, type);
			Main.npc[idx].velocity = vel;
			Main.npc[idx].lifeMax = lifeMax;
			Main.npc[idx].life = lifeMax - 1;
			Main.npc[idx].defense = defense;
			NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, null, idx);
			return idx;
		}
		#endregion
		#region CheckTime
		protected bool CheckTime()
		{
			if ((DateTime.Now - LastUpdate).TotalSeconds > Interval)
			{
				LastUpdate = DateTime.Now;
				return true;
			}
			return false;
		}
		#endregion
		#region SendData
		public void SendData()
		{
			NetMessage.SendData((int)PacketTypes.NpcUpdate, Client, -1, null, Index);
		}
		public void SendData(int Index)
		{
			NetMessage.SendData((int)PacketTypes.NpcUpdate, Client, -1, null, Index);
		}
		#endregion
		#endregion
	}
}
