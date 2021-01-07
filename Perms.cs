using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Starvers
{
	public static class Perms
	{
		public const string All   = "starver.*";
		public const string Normal   = "starver.normal";
		public const string Exchange   = "starver.exchange";
		public const string ShowInfo   = "starver.showinfo";
		public const string Test   = "starver.test";
		public const string Manager   = "starver.manager";
		public static class Task
		{
			public const string All   = "starver.task.*";
			public const string Normal   = "starver.task.normal";
			public const string ListAll   = "starver.task.listall";
			public const string FFF   = "starver.task.fff";
			public const string Set   = "starver.task.set";
			public const string Reload = "starver.task.reload";
			public const string BranchT = "starver.task.brancht";
		}
		public static class Aura
		{
			public const string All   = "starver.aura.*"; 
			public const string Normal   = "starver.aura.normal";
			public const string IgnoreCD   = "starver.aura.ignorecd";
			public const string ForceUp   = "starver.aura.forceup";
			public const string SetLvl   = "starver.aura.setlvl";
		}
		public static class Boss
		{
			public const string All   = "starver.boss.*";
			public const string Spawn   = "starver.boss.spawn";
		}
		public static class VIP
		{
			public const string All = "starver.vip.*";
			public const string LessCost = "starver.vip.lesscost";
			public const string RainBowChat = "starver.vip.rainbowchat";
			public const string Invisible = "starver.vip.invisible";
		}
	}
}
