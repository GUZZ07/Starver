using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Starvers
{
	public class ProjManager
	{
		private Queue<ProjLauncher> projLaunches;
		private Queue<ProjController> projControllers;

		public ProjManager()
		{
			projLaunches = new Queue<ProjLauncher>();
			projControllers = new Queue<ProjController>();
		}

		public void Update()
		{
			#region Launchers
			int count = projLaunches.Count;
			while (count-- > 0)
			{
				var item = projLaunches.Dequeue();
				if (!item.CheckLaunch())
				{
					projLaunches.Enqueue(item);
				}
			}
			#endregion
			#region Controllers
			count = projControllers.Count;
			while (count-- > 0)
			{
				var item = projControllers.Dequeue();
				if (item.Update())
				{
					projControllers.Enqueue(item);
				}
			}
			#endregion
		}

		public void Add(ProjLauncher launcher)
		{
			projLaunches.Enqueue(launcher);
		}
		public void Add(ProjController controller)
		{
			projControllers.Enqueue(controller);
		}

		public void CancelLaunch(int projIndex, bool killProj = true)
		{
			int count = projLaunches.Count;
			while (count-- > 0)
			{
				var launcher = projLaunches.Dequeue();
				if (launcher.Index == projIndex)
				{
					launcher.Cancel(killProj);
				}
				else
				{
					projLaunches.Enqueue(launcher);
				}
			}
		}
		public void CancelControl(int projIndex, bool killProj = true)
		{
			int count = projControllers.Count;
			while (count-- > 0)
			{
				var controller = projControllers.Dequeue();
				if (controller.Index == projIndex)
				{
					if(killProj)
					{
						controller.KillProj();
					}
				}
				else
				{
					projControllers.Enqueue(controller);
				}
			}
		}
	}
}
