using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.TaskSystem
{
	[Flags]
	public enum EffectBonus : short
	{
		/// <summary>
		/// 技能CD变为3/4
		/// </summary>
		CD34				= 0b00000000_00000001,
		/// <summary>
		/// MP消耗变为3/4
		/// </summary>
		MP34				= 0b00000000_00000010,
		/// <summary>
		/// 受伤减少5%(PVP无效)
		/// </summary>
		DamageDecrease1		= 0b00000000_00000100,
		/// <summary>
		/// 受伤减少5%(PVP无效)
		/// </summary>
		DamageDecrease2		= 0b00000000_00001000,
		/// <summary>
		/// 受伤减少5%(PVP无效)
		/// </summary>
		DamageDecrease3		= 0b00000000_00010000,
		/// <summary>
		/// 3%的概率不受伤(PVP无效)
		/// </summary>
		ProbabilisticDodge	= 0b00000000_00100000,
		/// <summary>
		/// 2%的概率2倍伤害
		/// </summary>
		DamageMultiply1		= 0b00000000_01000000,
		/// <summary>
		/// 2%的概率2倍伤害
		/// </summary>
		DamageMultiply2		= 0b00000000_10000000
	}
}
