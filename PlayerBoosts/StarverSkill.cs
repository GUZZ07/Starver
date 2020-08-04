using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Starvers.PlayerBoosts.Skills;
using Terraria;

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
			Count++;
			ID = (byte)(SkillIDs)Enum.Parse(typeof(SkillIDs), GetType().Name);
			var instance = typeof(SkillInstance<>).MakeGenericType(GetType());
			instance.GetMethod(nameof(SkillInstance<StarverSkill>.Load)).Invoke(null, new[] { this });
			Rand = new Random();
		}

		public abstract void Release(StarverPlayer player, Vector direction);
		/// <summary>
		/// 为可能的动态更新bossban之类的做准备
		/// </summary>
		public virtual void UpdateState()
		{

		}
		public void Load()
		{
			bool isUltimate = GetType() == typeof(UltimateSkill);
			Name ??= GetType().Name;
			Description ??= string.Empty;

			var ultimateText = isUltimate ? "\n[c/ff0000:[终极技能][c/ff0000:]]" : string.Empty;

			Introduction ??= $@"创意来源:     {Author}
CD:           {CD / 60.0:#.##}s
最低等级限制: {LevelNeed?.ToString() ?? "无限制"}
MP消耗:       {MPCost}{ultimateText}
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

		/// <summary>
		/// 给某些特殊技能使用的
		/// </summary>
		/// <returns></returns>
		public virtual bool SpecialBindTo(StarverPlayer player)
		{
			return false;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
