using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Starvers.AuraSystem.Skills.Base;
using Terraria.ID;

namespace Starvers.AuraSystem.Skills
{
	public class PosionFog:Skill
	{
		public PosionFog():base(SkillIDs.PosionFog)
		{
			MP = 190;
			Level = 400;
			CD = 60 * 15;
			Author = "Deaths";
			Description = "制造一片毒雾";
			SetText();
		}
		public override void Release(StarverPlayer player, Vector2 vel)
		{
			for (int i = 0; i < 8; i++)
			{
				player.ProjCircle(player.Center, 32 + 34 * i, 1, ProjectileID.ToxicCloud, 10 + 5 * i,103);
			}
		}
	}
}


