using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.TaskSystem
{
	/// <summary>
	/// 标识单一支线
	/// </summary>
	public enum BLID : byte
	{
		None = 0,
		TestLine1 = 1,
		YrtAEvah = 2,
		StoryToContinue = 3,
		/// <summary>
		/// 用于标识支线最大数目(开放边界)
		/// </summary>
		Max = 4
	}
}
