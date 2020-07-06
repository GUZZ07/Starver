using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Starvers
{
	public class ProjLaunchTaskManager
	{
		private Queue<ProjLaunchTask> projLaunches;

		public ProjLaunchTaskManager()
		{
			projLaunches = new Queue<ProjLaunchTask>();
		}

		public void Update()
		{
			int count = projLaunches.Count;
			while (count-- > 0)
			{
				var task = projLaunches.Dequeue();
				if (!task.CheckLaunch())
				{
					projLaunches.Enqueue(task);
				}
			}
		}

		public void Add(in ProjLaunchTask task)
		{
			projLaunches.Enqueue(task);
		}

		public void Cancel(int projIndex, bool killProj = true)
		{
			int count = projLaunches.Count;
			while (count-- > 0)
			{
				var task = projLaunches.Dequeue();
				if (task.Index == projIndex)
				{
					task.Cancel(killProj);
				}
				else
				{
					projLaunches.Enqueue(task);
				}
			}
		}
	}
}
