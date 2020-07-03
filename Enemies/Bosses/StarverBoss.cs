using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Starvers.Enemies.Bosses
{
	public abstract class StarverBoss : StarverEnemy
	{
		public const int DefaultLevel = 2000;
		/// <summary>
		/// 伤害跟随Level的增长因子(从1开始)
		/// </summary>
		public virtual double DamageIndex => throw new NotImplementedException();
		public int Level
		{
			get;
			protected set;
		}
		protected int rawType;
		protected StarverBoss(int rawNpcType) : base(-1)
		{
			rawType = rawNpcType;
		}
		protected virtual void Spawn(Vector2 position, int level = DefaultLevel)
		{
			int x = (int)position.X;
			int y = (int)position.Y;
			Index = NPC.NewNPC(x, y, rawType);
			Level = level;
		}
	}
}
