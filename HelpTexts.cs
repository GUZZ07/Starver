using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers
{
	public class HelpTexts
	{
		public static string Task { get; } = @"用法:
    check:  检查任务
    list:   列出任务
    branch: 支线任务
    bonus:  显示特殊属性";
		public static string Aura { get; } = @"用法:
    up:    升级
    toexp: 兑换经验
    set <slot> <skill> :设置技能
    list:  查看技能列表
    buy <slot> 获取技能槽位对应的武器
    help <skillName/SkillID> 查看技能说明";
		public static string LevelSystem { get; } = @"用法:
    up:    升级
    toexp: 兑换经验";
		public static string Weapon { get; } =
			"用法错误\n" +
			"正确用法:\n" +
			"  <Career> <Name> : Career 武器类型;Name 武器名\n" +
			"武器类型为:\n" +
			"  Melee\n  Ranged\n  Magic\n  Minion\n +" +
			" <Career> : 查看该类型武器";
	}
}
