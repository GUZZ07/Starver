using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;


namespace Starvers.PlayerBoosts.Realms.Shapes
{
	public class Circle : IReflectiveShape
	{
		private const int Max = 60;
		private int[] Border;
		private IRealm owner;

		public Circle()
		{
			Radium = 16 * 25;
		}

		public int? DefProjTimeLeft
		{
			get;
			set;
		}
		public int? BorderProjID
		{
			get;
			set;
		}
		protected Vector Center
		{
			get => owner.Center;
			set => owner.Center = value;
		}
		public float Radium
		{
			get;
			set;
		}
		public double Rotation
		{
			get;
			set;
		}

		public virtual bool AtBorder(Entity entity)
		{
			Vector2 v = entity.Center - Center;
			return Math.Abs(v.Length() - Radium) < 16 * 1.5f;
		}
		public virtual bool InRange(Entity entity)
		{
			return IsContained(entity.Hitbox);
		}
		public virtual bool Intersect(Entity entity)
		{
			return IsCrossed(entity.Hitbox);
		}

		public virtual void Begin(IRealm owner)
		{
			Border = new int[Max];
			if (BorderProjID is not int projID)
			{
				return;
			}
			for (int i = 0; i < Border.Length; i++)
			{
				Border[i] = Utils.NewProj(Center + Vector.FromPolar(Math.PI * 2 / 60 * i, Radium), default, projID, 1, 20, Main.myPlayer);
				Main.projectile[Border[i]].aiStyle = -2;
				if (DefProjTimeLeft != 0)
				{
					Main.projectile[Border[i]].timeLeft = (int)DefProjTimeLeft;
				}
			}
		}

		public virtual void Kill()
		{
			if (BorderProjID is not int projID)
			{
				return;
			}
			for (int i = 0; i < Border.Length; i++)
			{
				var proj = Main.projectile[Border[i]];
				if (proj.active && proj.type == projID)
				{
					proj.active = false;
					proj.SendData();
				}
			}
		}

		public virtual void Update(int timer)
		{
			if (BorderProjID is not int projID)
			{
				return;
			}
			for (int i = 0; i < Border.Length; i++)
			{
				var proj = Main.projectile[Border[i]];
				if (!proj.active || proj.type != BorderProjID)
				{
					Border[i] = Utils.NewProj(Center + Vector.FromPolar(Math.PI * 2 / 60 * i, Radium), default, projID, 1, 20, Main.myPlayer);
					proj = Main.projectile[Border[i]];
					proj.aiStyle = -2;
					if (DefProjTimeLeft != null)
					{
						proj.timeLeft = (int)DefProjTimeLeft;
					}
				}
				proj.SendData();
			}
		}

		public void Reflect(Entity entity)
		{
			Vector2 Distance = entity.Center - Center;
			Distance.Normalize();
			// v减去v的径向分量的两倍
			entity.velocity -= 2 * Distance * Vector2.Dot(Distance, entity.velocity);
		}

		#region Utils
		/// <summary>
		/// 判断是否有重叠
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="circle"></param>
		/// <returns></returns>
		protected bool IsIntersect(Rectangle rect)
		{
			Vector h = (rect.Width / 2, rect.Height / 2);
			Vector v = new Vector
			{
				X = Math.Abs(Center.X - rect.Center.X),
				Y = Math.Abs(Center.Y - rect.Center.Y)
			};
			Vector u = new Vector
			{
				X = Math.Max(v.X - h.X, 0),
				Y = Math.Max(v.Y - h.Y, 0)
			};
			return u.Length <= Radium;
		}
		/// <summary>
		/// 判断rect是否在circle内
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="circle"></param>
		/// <returns></returns>
		protected bool IsContained(Rectangle rect)
		{
			Vector v = new Vector
			{
				X = Math.Abs(Center.X - rect.Center.X),
				Y = Math.Abs(Center.Y - rect.Center.Y)
			};

			return v.Length <= Radium;
		}
		/// <summary>
		/// 判断circle与rect相交但不被包含
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="circle"></param>
		/// <returns></returns>
		protected bool IsCrossed(Rectangle rect)
		{
			Vector v = new Vector
			{
				X = Math.Abs(Center.X - rect.Center.X),
				Y = Math.Abs(Center.Y - rect.Center.Y)
			};
			return v.Length > Radium;
		}

		#endregion
	}
}
