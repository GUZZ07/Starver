using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Starvers
{
	/***************************************************
	 * 这个东西的存在主要是为了简化向量极坐标的操作    *
	 * 它可以隐式转化为Xna的Vector2	                   *
	 * 但是Vector2转化为它需要显示转化                 *
	 * 主要是因为如果不这么做，                        *
	 * 调用重载的算术运算符就会产生二义性              *
	 * 这个类型中的所有角度均采用弧度制                *
	 ***************************************************/
	/// <summary>
	/// <para> 用于简化向量部分操作</para>
	/// <para> 可以隐式转化为XNA的Vector2</para>
	/// <para> 可隐式由(X, Y)或显式由XNA得Vector2转化而来</para>
	/// </summary>
	public struct Vector : IEquatable<Vector>
	{
		#region Fields
		/// <summary>
		/// X坐标不解释
		/// </summary>
		public float X;
		/// <summary>
		/// Y坐标不解释
		/// </summary>
		public float Y;
		#endregion
		#region Properties
		private float PolarRadius
		{
			get => Length;
			set => Length = value;
		}
		/// <summary>
		/// 获取或设置该向量的极径
		/// </summary>
		public float Length
		{
			get
			{
				return (float)Math.Sqrt(LengthSquared);
			}
			set
			{
				double d = Angle;
				X = (float)(value * Math.Cos(d));
				Y = (float)(value * Math.Sin(d));
			}
		}
		/// <summary>
		/// 返回模长平方
		/// </summary>
		public float LengthSquared
		{
			get
			{
				return X * X + Y * Y;
			}
		}
		/// <summary>
		/// 获取或设置该向量的角度
		/// </summary>
		// 使用时请确保X, Y均不为0
		public double Angle
		{
			get
			{
				return Math.Atan2(Y, X);
			}
			set
			{
				float len = Length;
				X = (float)(len * Math.Cos(value));
				Y = (float)(len * Math.Sin(value));
			}
		}
		#endregion
		#region ctors
		public Vector(float x, float y)
		{
			X = x;
			Y = y;
		}
		/// <summary>
		/// 初始化为 (xy, xy)
		/// </summary>
		/// <param name="xy">X分量兼Y分量</param>
		public Vector(float xy)
		{
			X = Y = xy;
		}
		/// <summary>
		/// 通过极坐标获取向量
		/// </summary>
		/// <param name="angle">极角</param>
		/// <param name="length">极径</param>
		/// <returns></returns>
		public static Vector FromPolar(double angle, float length)
		{
			return new Vector { X = (float)(Math.Cos(angle) * length), Y = (float)(Math.Sin(angle) * length) };
		}
		/// <summary>
		/// 通过极坐标获取向量(和另一个没区别)
		/// </summary>
		/// <param name="angle">极角</param>
		/// <param name="length">极径</param>
		public static Vector NewByPolar(double angle, float length)
		{
			return new Vector { X = (float)(Math.Cos(angle) * length), Y = (float)(Math.Sin(angle) * length) };
		}
		#endregion
		#region Operators
		/// <summary>
		/// 获取反方向向量
		/// </summary>
		public static Vector operator -(Vector me)
		{
			me.X = -me.X;
			me.Y = -me.Y;
			return me;
		}
		/// <summary>
		/// 向量减法, 不需要多说
		/// </summary>
		public static Vector operator -(Vector left, Vector right)
		{
			return (left.X - right.X, left.Y - right.Y);
		}
		/// <summary>
		/// 加法不解释
		/// </summary>
		public static Vector operator +(Vector left, Vector right)
		{
			return (left.X + right.X, left.Y + right.Y);
		}
		/// <summary>
		/// 返回(left.X * right.X, left.Y * right.Y)
		/// </summary>
		public static Vector operator *(Vector left, Vector right)
		{
			return (left.X * right.X, left.Y + right.Y);
		}

		public static Vector operator *(Vector vector, float scale)
		{
			vector.X *= scale;
			vector.Y *= scale;
			return vector;
		}
		public static Vector operator *(float scale, Vector vector)
		{
			vector.X *= scale;
			vector.Y *= scale;
			return vector;
		}
		/// <summary>
		/// 判断不等(实际上是判断距离是否不小于0.001)
		/// </summary>
		public static bool operator !=(Vector left, Vector right)
		{
			return !(left == right);
		}
		/// <summary>
		/// 判断是否相等(实际上是判断距离是否小于0.001)
		/// </summary>
		public static bool operator ==(Vector left, Vector right)
		{
			return DistanceSquare(left, right) < 0.001f * 0.001f;
		}
		public static Vector operator /(Vector value1, float divider)
		{
			float num = 1f / divider;
			Vector result = default;
			result.X = value1.X * num;
			result.Y = value1.Y * num;
			return result;
		}
		/// <summary>
		/// 可以正常使用
		/// </summary>
		public unsafe static implicit operator Microsoft.Xna.Framework.Vector2(Vector value)
		{
			return *(Microsoft.Xna.Framework.Vector2*)&value;
		}
		/// <summary>
		/// 可以正常使用
		/// </summary>
		public unsafe static explicit operator Vector(Microsoft.Xna.Framework.Vector2 value)
		{
			return *(Vector*)&value;
		}
		public unsafe static implicit operator Vector((float,float)value)
		{
			return *(Vector*)&value;
		}
		public unsafe static implicit operator Vector((double, double) value)
		{
			return ((float)value.Item1, (float)value.Item2);
		}
		#endregion
		#region OperatorMethod
		public static float Dot(Vector left,Vector right)
		{
			return left.X * right.X + left.Y + right.Y;
		}
		public static float CrossModule(Vector left, Vector right)
		{
			return left.X * right.Y - left.Y * right.X;
		}
		#endregion
		#region Utils
		/// <summary>
		/// 求外积的模长
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public float Cross(Vector value)
		{
			return X * value.Y - Y * value.X;
		}
		/// <summary>
		/// 获取对称向量
		/// </summary>
		/// <param name="Center">对称中心</param>
		/// <returns></returns>
		public Vector Symmetry(Vector Center)
		{
			return Center * 2f - this;
		}
		
		public Vector ToLenOf(float len)
		{
			Vector result = this;
			result.PolarRadius = len;
			return result;
		}
		public Vector ToAngleOf(double angle)
		{
			Vector result = this;
			result.Angle = angle;
			return result;
		}
		private Vector ToVertical()
		{
			return (-Y, X);
		}
		public Vector ToVertical(float len)
		{
			return FromPolar(Angle + Math.PI / 2, len);
		}

		public Vector AddVertical(float len)
		{
			return this + ToVertical(len);
		}
		/// <summary>
		/// 获取法向量(-Y, X), 实际上只是角度加上了 PI / 2
		/// </summary>
		public Vector Vertical()
		{
			return new Vector(-Y, X);
		}
		/// <summary>
		/// 获取偏转一定角度的向量
		/// </summary>
		/// <param name="rad">偏转角</param>
		public Vector Deflect(double rad)
		{
			Vector result = this;
			result.Angle += rad;
			return result;
		}
		/// <summary>
		/// 转化为Main.tile中的坐标
		/// </summary>
		/// <returns></returns>
		public Point ToTileCoordinate()
		{
			int TileX = (int)Math.Ceiling(X / 16);
			int TileY = (int)Math.Ceiling(Y / 16);
			return new Point(TileX, TileY);
		}
		/// <summary>
		/// 变为单位向量(其实几乎没用到过)
		/// </summary>
		public void Normalize()
		{
			float num = X * X + Y * Y;
			float num2 = 1f / (float)Math.Sqrt((double)num);
			X *= num2;
			Y *= num2;
		}
		/// <summary>
		/// 转化为(X, Y)的字符串表示形式
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("({0},{1})", X, Y);
		}
		#endregion
		#region statics
		/// <summary>
		/// 零向量
		/// </summary>
		public static Vector Zero
		{
			get
			{
				return default;
			}
		}
		/// <summary>
		/// 求距离
		/// </summary>
		public static float Distance(Vector left, Vector right)
		{
			float X = left.X - right.X;
			float Y = left.Y - right.Y;
			return (float)Math.Sqrt(X * X + Y * Y);
		}
		/// <summary>
		/// 求距离平方
		/// </summary>
		public static float DistanceSquare(Vector left, Vector right)
		{
			float X = left.X - right.X;
			float Y = left.Y - right.Y;
			return X * X + Y * Y;
		}
		#endregion
		#region else
		public bool Equals(Vector value)
		{
			return this == value;
		}
		public override bool Equals(object obj)
		{
			if (obj is Vector)
			{
				return ((IEquatable<Vector>)obj).Equals(this);
			}
			return false;
		}
		/// <summary>
		/// 容不容易冲突还不知道
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return X.GetHashCode() * Y.GetHashCode();
		}
		#endregion
	}
}
