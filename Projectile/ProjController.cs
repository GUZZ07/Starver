using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers
{
	using Proj = Terraria.Projectile;
	using Main = Terraria.Main;
	public class ProjController
	{
		public delegate void ProjAction(Proj proj, ref int timer, ref bool isEnd);

		private int id;
		private bool isEnded;
		private int timer;


		public int Index { get; }
		public ProjAction Controller { get; }

		public ProjController(int index, ProjAction action)
		{
			Index = index;
			Controller = action;
			id = Main.projectile[Index].type;
			isEnded = false;
			timer = 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>是否处于活动状态</returns>
		public bool Update()
		{
			if (!Main.projectile[Index].active || Main.projectile[Index].type != id)
			{
				return false;
			}
			Controller?.Invoke(Main.projectile[Index], ref timer, ref isEnded);
			return !isEnded;
		}

		public void KillProj()
		{
			Main.projectile[Index].active = false;
			Main.projectile[Index].netImportant = true;
			Main.projectile[Index].SendData();
		}
	}
}
