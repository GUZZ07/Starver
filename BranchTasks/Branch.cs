using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Starvers.BranchTasks
{
	public abstract class Branch
	{
		public abstract BranchID BranchID { get; }
		public abstract string Name { get; }
		public abstract int Count { get; }

		protected Branch()
		{

		}

		public abstract void ShowTaskTo(StarverPlayer player, int taskID);
		public abstract bool CanStart(StarverPlayer player, int taskID);
		// 里头直接设置player.BranchTask并Task.Start()
		public abstract bool TryStartTask(StarverPlayer player, int taskID);
	}
}
