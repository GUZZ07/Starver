using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;

namespace Starvers.AuraSystem.Realms
{
	using static Starvers.AuraSystem.Realms.InternalShapes;
	using Vector = TOFOUT.Terraria.Server.Vector2;
	/// <summary>
	/// 表示一个圆形领域
	/// </summary>
	public abstract class CircleRealm : StarverRealm
	{
		public float Radium { get; set; }

		public override bool InRange(Entity entity)
		{
			return IsContained(entity.Hitbox, (Center, Radium));
		}
		public override bool IsCross(Entity entity)
		{
			return IsCrossed(entity.Hitbox, (Center, Radium));
		}

		protected CircleRealm(bool killedByTimeleft) : base(killedByTimeleft)
		{

		}

		#region Utils
		/// <summary>
		/// 判断是否有重叠
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="circle"></param>
		/// <returns></returns>
		protected static bool IsOverlapping(Rectangle rect, Circle circle)
		{
			Vector h = (rect.Width / 2, rect.Height / 2);
			Vector v = new Vector
			{
				X = Math.Abs(circle.X - rect.Center.X),
				Y = Math.Abs(circle.Y - rect.Center.Y)
			};
			Vector u = new Vector
			{
				X = Math.Max(v.X - h.X, 0),
				Y = Math.Max(v.Y - h.Y, 0)
			};
			return u.Length <= circle.Radium;
		}
		/// <summary>
		/// 判断rect是否在circle内
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="circle"></param>
		/// <returns></returns>
		protected static bool IsContained(Rectangle rect, Circle circle)
		{
			Vector v = new Vector
			{
				X = Math.Abs(circle.X - rect.Center.X),
				Y = Math.Abs(circle.Y - rect.Center.Y)
			};
			return v.Length <= circle.Radium;
		}
		/// <summary>
		/// 判断circle与rect相交但不被包含
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="circle"></param>
		/// <returns></returns>
		protected static bool IsCrossed(Rectangle rect, Circle circle)
		{
			Vector v = new Vector
			{
				X = Math.Abs(circle.X - rect.Center.X),
				Y = Math.Abs(circle.Y - rect.Center.Y)
			};
			return v.Length > circle.Radium;
		}
		
		#region Wasted
		// 以下内容应该是用于矩形Realm才对

		/*
		/// <summary>
		/// 判断是否有重叠
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="circle"></param>
		/// <returns></returns>
		protected static bool IsOverlapping(Rectangle rect, Circle circle)
		{
			Vector h = (rect.Width / 2, rect.Height / 2);
			Vector v = new Vector
			{
				X = Math.Abs(circle.X - rect.Center.X),
				Y = Math.Abs(circle.Y - rect.Center.Y)
			};
			Vector u = new Vector
			{
				X = Math.Max(v.X - h.X, 0),
				Y = Math.Max(v.Y - h.Y, 0)
			};
			return u.Length <= circle.Radium;
		}
		/// <summary>
		/// 判断circle是否在rect内
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="circle"></param>
		/// <returns></returns>
		protected static bool IsContained(Rectangle rect, Circle circle)
		{
			Vector h = (rect.Width / 2, rect.Height / 2);
			Vector v = new Vector
			{
				X = Math.Abs(circle.X - rect.Center.X),
				Y = Math.Abs(circle.Y - rect.Center.Y)
			};
			Vector u = new Vector
			{
				X = Math.Max(v.X - h.X, 0),
				Y = Math.Max(v.Y - h.Y, 0)
			};

			return h.X - v.X >= circle.Radium
				&& h.Y - v.Y >= circle.Radium
				&& u.Length <= circle.Radium;
		}

		/// <summary>
		/// 判断circle与rect相交但不被包含
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="circle"></param>
		/// <returns></returns>
		protected static bool IsCrossed(Rectangle rect, Circle circle)
		{
			Vector h = (rect.Width / 2, rect.Height / 2);
			Vector v = new Vector
			{
				X = Math.Abs(circle.X - rect.Center.X),
				Y = Math.Abs(circle.Y - rect.Center.Y)
			};
			Vector u = new Vector
			{
				X = Math.Max(v.X - h.X, 0),
				Y = Math.Max(v.Y - h.Y, 0)
			};

			if (u.Length > circle.Radium)
			{
				return false;
			}

			return h.X - v.X < circle.Radium
				|| h.Y - v.Y < circle.Radium;
		}
		*/
		#endregion
		#endregion
	}
}
