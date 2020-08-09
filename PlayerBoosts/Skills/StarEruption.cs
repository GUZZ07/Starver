using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.PlayerBoosts.Skills
{
	
	using Microsoft.Xna.Framework;
	using Terraria.ID;
	public class StarEruption : StarverSkill
	{
		/// <summary>
		/// 陪衬弹幕
		/// </summary>
		private readonly int[] LiningProjs =
		{
			ProjectileID.HallowStar,
			ProjectileID.Starfury,
			ProjectileID.StarVeilStar
		};
		/// <summary>
		/// 主弹幕
		/// </summary>
		private readonly int[] MainProjs =
		{
			ProjectileID.StarWrath,
			ProjectileID.Meteor1,
			ProjectileID.Meteor2,
			ProjectileID.Meteor3,
			ProjectileID.FallingStar,
			ProjectileID.SuperStar
		};
		public StarEruption()
		{
			CD = 60 * 70;
			MPCost = 500;
			LevelNeed = 3000;
			Author = "zhou_Qi";
			Description = @"召唤大量陨星进行攻击
""引动星辰的坠落，炽热的天堂之火以其肆虐的破坏力而深受锻造师们的喜爱""
""秘藏在浮空岛屿之上的星怒也不过是那个时代的一个小小缩影""";
			Summary = "[3000][击败世纪之花解锁]引动星辰的力量发动攻击";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			var LaunchSource = player.Center;
			LaunchSource.Y -= 30 * 16;
			var Unit = Vector.FromPolar(Math.PI / 4, 16 * 3);
			Unit.X *= player.TPlayer.direction;
			var velocity = Unit;
			velocity.Length = 19;
			int LoopTime;
			for (int i = 0; i < 50; i++)
			{
				player.NewProj(LaunchSource + Rand.NextVector2(16 * 3.5f, 0), velocity, MainProjs[Rand.Next(MainProjs.Length)], 560, 20f);
				LoopTime = Rand.Next(3, 6);
				for (int j = 0; j < LoopTime; j++)
				{
					player.NewProj(LaunchSource + Rand.NextVector2(16 * 8.5f,0), velocity * 0.95f + Rand.NextVector2(0, 10), LiningProjs[Rand.Next(LiningProjs.Length)], 420, 20f);
				}
				LaunchSource -= Unit;
			}
		}
	}
}
