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
			ProjectileID.MolotovCocktail,
			ProjectileID.MolotovCocktail,
			ProjectileID.MolotovCocktail,
			ProjectileID.Ale,
			ProjectileID.Ale,
			ProjectileID.Ale,
			ProjectileID.Ale,
			ProjectileID.Ale,
		};
		public AlcoholFeast() 
		{
			CD = 60 * 10;
			MPCost = 35;
			LevelNeed = 5;
			Author = "zhou_Qi";
			Description = @"抛射麦芽酒与鸡尾酒，获得醉酒与饱腹效果
""即使是怪物的体魄也无法承受如此强烈的酒精冲击""";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			int damage = 25 + (int)(80 * Math.Log(player.Level));
			Vector2 velocity = vel * 3.5f;
			for (int i = 0; i < 20; i++)
			{
				player.NewProj(player.Center + Rand.NextVector2(16 * 3, 16 * 3), velocity, Projs.Next(), damage, 10f);
			}
		}
	}
}
