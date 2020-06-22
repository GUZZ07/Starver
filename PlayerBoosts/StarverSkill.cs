using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.PlayerBoosts
{
	public abstract class StarverSkill
	{
		public static byte Count { get; private set; }

		protected Random Rand { get; }

		public byte ID { get; }
		/// <summary>
		/// 技能名
		/// </summary>
		public string Name { get; protected set; }
		/// <summary>
		/// 创意来源
		/// </summary>
		public string Author { get; protected set; }
		/// <summary>
		/// 简单介绍
		/// </summary>
		public string Description { get; protected set; }
		/// <summary>
		/// 完整介绍
		/// </summary>
		public string Introduction { get; protected set; }
		/// <summary>
		/// 所需最低等级(null为无等级限制)
		/// </summary>
		public int? LevelNeed { get; protected set; }
		public int CD { get; protected set; }
		public int MPCost { get; protected set; }
		public bool ForceCD { get; protected set; }
		public bool Banned { get; set; }

		protected StarverSkill()
		{
			ID = Count++;
			var instance = typeof(SkillInstance<>).MakeGenericType(GetType());
			instance.GetMethod(nameof(SkillInstance<StarverSkill>.Load)).Invoke(null, new[] { this, (object)ID });
			Rand = new Random();
		}

		public abstract void Release(StarverPlayer player);
		/// <summary>
		/// 为可能的动态更新bossban之类的做准备
		/// </summary>
		public virtual void UpdateState()
		{

		}
		public void Load()
		{
			Name ??= GetType().Name;
			Description ??= string.Empty;
			Introduction ??= $@"创意来源:     {Author}
CD:           {CD / 60.0:#.##}s
最低等级限制: {LevelNeed?.ToString() ?? "无限制"}
MP消耗:       {MPCost}
{Description}";
		}
		public virtual bool CanSet(StarverPlayer player)
		{
			int levelNeed = LevelNeed ?? 0;
			if (player.Level < levelNeed)
			{
				player.SendErrorText("你的等级不足, 所需等级: " + levelNeed);
				return false;
			}
			return true;
		}
	}
}
