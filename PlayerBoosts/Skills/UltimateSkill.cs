using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Starvers.PlayerBoosts.Skills
{
	public abstract class UltimateSkill : StarverSkill
	{
		public UltimateSkill()
		{
			ForceCD = true;
		}
	}
}
