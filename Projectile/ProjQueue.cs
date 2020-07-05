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
	/// <summary>
	/// 事先将弹幕和速度存入,稍后发射
	/// </summary>
	public class ProjQueue : Queue<Starvers.ProjLaunchTask>, IProjSet
	{
		#region Fields
		private int Size;
		#endregion
		#region Ctor
		public ProjQueue(int size = 30) : base(size)
		{
			Size = size;
		}
		#endregion
		#region Push
		public bool Push(int idx, Vector velocity)
		{
			Enqueue(new Starvers.ProjLaunchTask(idx, velocity, 0));
			return Count < Size;
		}
		public bool Push(IEnumerable<int> idxes, Vector vel)
		{
			foreach (var idx in idxes)
			{
				if (!Push(idx, vel))
				{
					return false;
				}
			}
			return Count < Size;
		}
		public unsafe bool Push(int* ptr, int count, Vector vel)
		{
			int* end = ptr + count;
			while (ptr != end)
			{
				if (!Push(*ptr++, vel))
				{
					return false;
				}
			}
			return Count < Size;
		}
		#endregion
		#region Launch
		public void Launch()
		{
			while (Count > 0)
			{
				Dequeue().Launch();
			}
		}
		public void Launch(Vector Velocity)
		{
			while (Count > 0)
			{
				Dequeue().Launch(Velocity);
			}
		}
		public bool Launch(int HowMany, Vector Vel)
		{
			while (HowMany-- > 0 && Count > 0)
			{
				Launch(Vel);
			}
			return Count > 0;
		}
		public bool Launch(int HowMany)
		{
			while (HowMany-- > 0 && Count > 0)
			{
				Launch();
			}
			return Count > 0;
		}
		public bool LaunchTo(int HowMany, Vector Pos, float speed)
		{
			while (HowMany-- > 0 && Count > 0)
			{
				Dequeue().LaunchTo(Pos, speed);
			}
			return Count > 0;
		}
		public void LaunchTo(Vector Pos, float speed)
		{
			LaunchTo(Count, Pos, speed);
		}
		#endregion
		#region Reset
		public void Reset()
		{
			Clear();
		}
		#endregion
	}
}
