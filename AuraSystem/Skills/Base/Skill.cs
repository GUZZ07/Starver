using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;

namespace Starvers.AuraSystem.Skills.Base
{
	public abstract class Skill
	{
		private int cd;
		public const int MaxSlots = 5;
		public static Random Rand => Starver.Rand;
		public string Name { get; private set; }
		public int Index { get; private set; }
		public int CD
		{
			get => cd;
			protected set => cd = value;
		}
		public int MP { get; protected set; }
		public int Level { get; protected set; }
		public bool ForceCD { get; protected set; }
		public bool BossBan { get; protected set; }
		public string Author { get; protected set; } = string.Empty;
		public string Description { get; protected set; } = string.Empty;
		public string Text { get; private set; } = string.Empty;
		public abstract void Release(StarverPlayer player, Vector2 vel);
		public int TaskNeed = 0;
		public virtual bool CanSet(StarverPlayer player)
		{
			bool condition = TaskNeed <= StarverConfig.Config.TaskNow;
			if(!condition)
			{
				player.SendInfoMessage($"当前任务不足, 该技能将在任务{TaskNeed}完成后解锁");
				return condition;
			}
			condition = Level <= player.Level;
			if(!condition)
			{
				player.SendInfoMessage($"当前等级不足, 该技能将在{Level}级解锁");
			}
			return condition;
		}
		protected Skill()
		{
			Name = GetType().Name;
			Index = (int)(SkillIDs)Enum.Parse(typeof(SkillIDs), Name);
		}
		protected Skill(SkillIDs idx) : this()
		{
			Index = (int)idx;
		}
		protected void SetText()
		{
			StringBuilder sb = new StringBuilder(30);
			sb.AppendLine($"创意来源: {Author}");
			sb.AppendLine($"CD: {CD / 60}s");
			sb.AppendLine($"所需等级: {Level}");
			sb.AppendLine($"MP: {MP}");
			sb.Append($"{Description}");
			Text = sb.ToString();
		}
	}
}
