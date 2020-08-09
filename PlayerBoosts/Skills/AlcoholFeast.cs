using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.PlayerBoosts.Skills
{
	
	using Microsoft.Xna.Framework;
	using Terraria;
	using Terraria.ID;

	public class AlcoholFeast : StarverSkill
	{
		private int[] Projs =
		{
			ProjectileID.MolotovCocktail,
			ProjectileID.Ale,
			ProjectileID.Ale,
			ProjectileID.Ale,
			ProjectileID.Ale,
			ProjectileID.Ale,
		};
		public AlcoholFeast() 
		{
			CD = 60 * 36;
			MPCost = 160;
			LevelNeed = 700;
			Author = "zhou_Qi";
			Description = @"抛射麦芽酒与鸡尾酒
""即使是怪物的体魄也无法承受如此强烈的酒精冲击""";
            Summary = "[700][击败骷髅王解锁]集中抛射大量酒精制品";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			int damage = 5 + (int)(20 * Math.Log(player.Level));
			Vector2 velocity = vel * 3.5f;
			for (int i = 0; i < 10; i++)
			{
				player.NewProj(player.Center + Rand.NextVector2(16 * 3, 16 * 3), velocity, Projs.Next(), damage, 10f);
			}
		}
		
	}
}
