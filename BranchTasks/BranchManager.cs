using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Starvers.BranchTasks
{
	public class BranchManager
	{
		#region Instances
		private class BranchInstance<T> where T : Branch
		{
			public static T Value;
		}
		#endregion
		private Branch[] branches;

		public int Count
		{
			get => branches.Length;
		}

		public Branch this[BranchID id]
		{
			get => branches[(int)id];
		}

		public BranchManager()
		{
			var types = typeof(BranchManager)
				.Assembly
				.GetTypes()
				.Where(type => type.IsSubclassOf(typeof(Branch)) && !type.IsAbstract);
			branches = new Branch[types.Count()];
			int i = 0;
			foreach (var type in types)
			{
				branches[i] = (Branch)Activator.CreateInstance(type);
				typeof(BranchInstance<>)
					.MakeGenericType(type)
					.GetField(nameof(BranchInstance<Branch>.Value))
					.SetValue(null, branches[i++]);
			}
			Array.Sort(branches, (l, r) => l.BranchID - r.BranchID);
		}

		public BranchID GetBranchID<T>() where T : Branch
		{
			return GetBranch<T>().BranchID;
		}
		public T GetBranch<T>() where T : Branch
		{
			return BranchInstance<T>.Value;
		}
	}
}
