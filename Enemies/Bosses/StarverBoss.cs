using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using TShockAPI;

namespace Starvers.Enemies.Bosses
{
	public abstract partial class StarverBoss : StarverEnemy
	{
		public const int DefaultLevel = 2000;
		public string Name 
		{ 
			get; 
			protected set;
		}
		/// <summary>
		/// 伤害跟随Level的增长因子(从1开始)
		/// </summary>
		public virtual double DamageIndex
		{
			get => 1;
		}
		public int Level
		{
			get;
			protected set;
		}

		public override bool Active
		{
			get => 0 <= Index && Index < Main.maxNPCs && base.Active;
		}
		public Random Rand { get; }


		public int Target
		{
			get => TNPC.target;
			set => TNPC.target = value;
		}
		public StarverPlayer TargetPlayer
		{
			get => TNPC.HasPlayerTarget ? Starver.Instance.Players[Target] : null;
		}

		#region DefDatas
		protected int defLifes = 1;
		protected int defLife = 20000;
		protected int defDefense = 5000;
		protected int rawType;
		#endregion
		#region InstanceDatas
		public int Lifes
		{
			get;
			set;
		}
		public int LifesMax
		{
			get;
			set;
		}
		#endregion

		protected StarverBoss(int rawNpcType) : base(-1)
		{
			rawType = rawNpcType;
			Rand = new Random();
		}

		public virtual void Spawn(Vector2 position, int level = DefaultLevel)
		{
			int x = (int)position.X;
			int y = (int)position.Y;
			Index = NPC.NewNPC(x, y, rawType);
			Level = level;
			Lifes = defLifes;
			LifesMax = defLifes;
			SetNPCDatas();
			if (Name != null)
			{
				TSPlayer.All.SendData(PacketTypes.UpdateNPCName, Name, Index);
			}
		}
		protected virtual void ReSpawn()
		{
			int x = (int)Center.X;
			int y = (int)Center.Y;
			Index = NPC.NewNPC(x, y, rawType);
			SetNPCDatas();
			if (Name != null)
			{
				TSPlayer.All.SendData(PacketTypes.UpdateNPCName, Name, Index);
			}
		}
		#region SetNPCDatas
		protected void SetNPCDatas()
		{
			TNPC.boss = true;
			TNPC.lifeMax = defLife;
			TNPC.life = defLife;
			TNPC.defense = defDefense;
		}
		#endregion
		#region AI
		public virtual void AI()
		{
			if (!Active)
			{
				if (Lifes != 0)
				{
					ReSpawn();
				}
				else
				{
					return;
				}
			}
			RealAI();
		}
		protected abstract void RealAI();
		#endregion
		#region Defeated
		protected virtual void Defeated()
		{
			Kill();
		}
		#endregion
		#region Lifes--
		protected virtual void LifesDown()
		{
			Lifes--;
			TSPlayer.All.SendMessage($"{Name ?? TNPC.GivenOrTypeName} 还剩[c/ff0000:{Lifes}]/[c/ffff00:{LifesMax}]条命", Color.Blue);
			if (Lifes <= 0)
			{
				Kill();
			}
			else
			{
				Life = LifeMax;
			}
		}
		#endregion
		#region ReceiveDamage
		public override void ReceiveDamage(int damage)
		{
			Life -= damage;
			if (Life <= 0)
			{
				LifesDown();
			}
		}
		#endregion
		#region Kill
		public override void Kill()
		{
			DropItems();
			RewardExps();
			Active = false;
		}
		#endregion

		protected virtual void RewardExps()
		{

		}

		protected virtual void DropItems()
		{

		}

		public override void TurnToAir()
		{
			base.TurnToAir();
			Lifes = 0;
			Index = -1;
		}

		public override string ToString()
		{
			return Name ?? GetType().Name;
		}
	}
}
