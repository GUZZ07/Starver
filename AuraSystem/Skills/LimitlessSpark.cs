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
	public class LimitlessSpark : Skill
	{
		public LimitlessSpark() : base(SkillIDs.LimitlessSpark)
		{
			MP = 20;
			CD = 60 * 30;
			Author = "wither";
			Level = 500;
			Description = "耗光你所有的MP,制造咒火团";
			SetText();
		}
		public override void Release(StarverPlayer player, Vector2 vel)
		{
			int damage = (int)(10 * Math.Log10(player.Level) + Math.Min(32567, player.MP / 10)) + 200;
			int n = (int)(10 * Math.Log(player.MP) + 10);
			player.MP = 0;
			for (int i = 0; i < n; i++)
			{
				player.NewProj(player.Center,  player.FromPolar(vel.Angle() + Rand.NextDouble(-Math.PI / 6, Math.PI / 6), 29), ProjectileID.CursedFlameFriendly, damage, 20f);
			}
		}
	}
}
