using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.TaskSystem.Branches
{
	public partial class StoryToContinue : BranchLine
	{
		private Task[] tasks;

		public override int Count => 8;

		public StoryToContinue() : base(BLID.StoryToContinue)
		{
			tasks = new Task[Count];
			for(int i = 0; i < Count; i++)
			{
				tasks[i] = new Task(i);
				tasks[i].SetDefault();
			}
		}

		public override (bool success, string msg) TryStartTask(StarverPlayer player, int index)
		{
			var result = tasks[index].CanStartTask(player);
			if (result.success)
			{
				var task = new Task(index, player);
				task.SetDefault();
				task.Start();
				player.BranchTask = task;
			}
			return result;
		}

		public override BranchTask this[int index]
		{
			get
			{
				return tasks[index];
			}
		}
	}
}
