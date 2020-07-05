using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using Terraria.Localization;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;

namespace Starvers
{
	public interface IProjSet
	{
		/// <summary>
		/// 将弹幕以及状态压入
		/// </summary>
		/// <param name="idx">弹幕索引</param>
		/// <param name="velocity">速度</param>
		/// <returns>代表是否可以继续添加(是否已满)</returns>
		public bool Push(int idx, Vector velocity);
		public bool Push(IEnumerable<int> idxes, Vector velocity);
		public unsafe bool Push(int* ptr, int count, Vector vel);
		/// <summary>
		/// 发射若干个弹幕
		/// </summary>
		/// <param name="HowMany">发射多少个</param>
		/// <returns>是否可以继续发射(是否为空)</returns>
		public bool Launch(int HowMany);
		/// <summary>
		/// 发射若干个弹幕
		/// </summary>
		/// <param name="HowMany">发射多少个</param>
		/// <param name="Velocity">速度</param>
		/// <returns>是否可以继续发射(是否为空)</returns>
		public bool Launch(int HowMany, Vector Velocity);
		/// <summary>
		/// 发射所有剩余弹幕
		/// </summary>
		public void Launch();
		/// <summary>
		/// 以该速度发射所有弹幕
		/// </summary>
		/// <param name="Velocity"></param>
		public void Launch(Vector Velocity);
		/// <summary>
		/// 指定发射终点发射所有弹幕
		/// </summary>
		/// <param name="Pos"></param>
		public void LaunchTo(Vector Pos, float vel);
		/// <summary>
		/// 发射到指定位置
		/// </summary>
		/// <param name="HowMany"></param>
		/// <param name="Pos"></param>
		/// <param name="vel">速率</param>
		public bool LaunchTo(int HowMany, Vector Pos, float vel);
		/// <summary>
		/// 重置状态
		/// </summary>
		public void Reset();
	}
}
