using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Starvers
{
	public static class Perms
	{
		public static readonly string Normal = "starver.normal";
		public static readonly string Admin = "starver.admin";
		public static readonly string Test = "starver.test";
		public static readonly string HotReload = "starver.hotreload";
		public static class Aura
		{
			public static readonly string Normal = "starver.aura.normal";
			public static readonly string Set = "starver.aura.set";
			public static readonly string SetOther = "starver.aura.setother";
		}
		public static class Boss
		{
			public static readonly string Spawn = "starver.boss.spawn";
		}
	}
}
