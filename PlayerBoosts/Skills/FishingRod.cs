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

	public class FishingRod : StarverSkill
	{
		public FishingRod()
		{
			LevelNeed = 400;
			MPCost = 25;
			CD = 60 * 3 * 60;
			Author = "zhou_Qi";
			Description = @"抛射多根随机种类的鱼竿
""Deaths会很喜欢这个技能的""
""她从未想过，她用以书写'技能'的'书页'，源自于什么""";
			Summary = "XxXxXX";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			int Count = Math.Min((int)(Math.Log(player.Level) * 2 + 1), 20);
			player.HeldItem.fishingPole = 75;
			player.TPlayer.fishingSkill = 75;
			int idx;
			for (int i = 0; i < Count; i++)
			{
				idx = player.NewProj(player.Center, Rand.NextVector2(14), ProjectileID.BobberGolden, 0, 10f);
				Main.projectile[idx].aiStyle = -1;
			}
		}
		public override bool CanSet(StarverPlayer player)
		{
			player.SendBlueText("该技能已被神秘力量封印");
			return false;
		}
	}
}
