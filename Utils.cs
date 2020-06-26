using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using TShockAPI;

namespace Starvers
{
	public static class Utils
	{
		private static Random rand = new Random();
		public static void SendCombatText(Vector2 pos, string text, Color color,int remoteClient = -1, int ignoreClient = -1)
		{
			NetMessage.SendData((int)PacketTypes.CreateCombatTextExtended, remoteClient, ignoreClient, NetworkText.FromLiteral(text), (int)color.PackedValue, pos.X, pos.Y, 0.0f, 0, 0, 0);
		}
		public static void SendCombatText(Vector2 randombox, Vector2 pos, string text, Color color, int remoteClient = -1, int ignoreClient = -1)
		{
			pos.X += rand.NextFloat(randombox.X);
			pos.Y += rand.NextFloat(randombox.Y);
			SendCombatText(pos, text, color, remoteClient, ignoreClient);
		}
		public static void SendCombatText(this Entity entity, string text, Color color, int remoteClient = -1, int ignoreClient = -1)
		{
			SendCombatText(entity.Size, entity.position, text, color, remoteClient, ignoreClient);
		}
		public static int NewProj(Vector2 position, Vector2 velocity, int Type, int Damage, float KnockBack = 20f, int owner = 255, float ai0 = 0, float ai1 = 0)
		{
			int index = Projectile.NewProjectile(position, velocity, Type, Damage, KnockBack, owner, ai0, ai1);
			TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", index);
			return index;
		}
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
		public static float NextFloat(this Random rand, float max)
		{
			return rand.NextFloat(0, max);
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
			return Vector.FromPolar(rand.NextAngle(), Length);
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
			return array[rand.Next(array.Length)];
		}
		public static T Next<T>(this IList<T> list)
		{
			return list[rand.Next(list.Count)];
		}
		#endregion
		#region Numbers
		/// <summary>
		/// 检查number是否属于[min, max]
		/// </summary>
		/// <param name="number"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static bool InRange(in this int number, int min, int max)
		{
			return min <= number && number <= max;
		}
		#endregion
	}
}
