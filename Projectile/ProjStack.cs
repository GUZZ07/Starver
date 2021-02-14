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
	public class ProjStack : Stack<Starvers.ProjLauncher>, IProjSet
	{
		#region Fields
		private int Size;
		#endregion
		#region Ctor
		public ProjStack(int Size = 30) : base(Size)
		{
			this.Size = Size;
		}
		#endregion
		#region Push
		public bool Push(int idx, Vector Velocity)
		{
			Push(new ProjLauncher(idx, Velocity, 1));
			return Count < Size;
		}
		public bool Push(IEnumerable<int> idxes, Vector Velocity)
		{
			foreach (int idx in idxes)
			{
				if (!Push(idx, Velocity))
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
		public bool Launch(int HowMany)
		{
			Starvers.ProjLauncher pair;
			for (int i = 0; i < HowMany && base.Count > 0; i++)
			{
				pair = Pop();
				pair.Launch();
			}
			return base.Count > 0;
		}
		public bool Launch(int HowMany, Vector vel)
		{
			Starvers.ProjLauncher pair;
			for (int i = 0; i < HowMany && base.Count > 0; i++)
			{
				pair = Pop();
				pair.Launch(vel);
			}
			return Count > 0;
		}
		public bool LaunchTo(int HowMany, Vector Where, float speed)
		{
			Starvers.ProjLauncher pair;
			for (int i = 0; i < HowMany && base.Count > 0; i++)
			{
				pair = Pop();
				pair.LaunchTo(Where, speed);
			}
			return Count > 0;
		}
		public void Launch()
		{
			while (Count > 0)
			{
				Pop().Launch();
			}
		}
		public void Launch(Vector vel)
		{
			while (Count > 0)
			{
				Pop().Launch(vel);
			}
		}
		public void LaunchTo(Vector Where, float speed)
		{
			while (Count > 0)
			{
				Pop().LaunchTo(Where, speed);
			}
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
