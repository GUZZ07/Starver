using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.AuraSystem.Skills
{
	using Base;
	using Microsoft.Xna.Framework;
	using Terraria.ID;
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public class StarEruption : Skill
	{
		/// <summary>
		/// 陪衬弹幕
		/// </summary>
		private readonly int[] LiningProjs =
		{
			ProjectileID.HallowStar,
			ProjectileID.FallingStar,
			ProjectileID.Starfury,
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
		};
		public StarEruption() : base(SkillIDs.StarEruption)
		{
			CD = 60 * 2 * 60;
			MP = 130;
			Level = 800;
			Author = "zhou_Qi";
			Description = @"召唤大量陨星进行攻击
""引动星辰的坠落，炽热的天堂之火以其肆虐的破坏力而深受锻造师们的喜爱""
""秘藏在浮空岛屿之上的星怒也不过是那个时代的一个小小缩影""";
			SetText();
		}
		public override void Release(StarverPlayer player, Vector2 vel)
		{
			Vector LaunchSource = (Vector)player.Center;
			LaunchSource.Y -= 30 * 16;
			Vector Unit = Vector.FromPolar(Math.PI / 4, 16 * 3);
			Unit.X *= player.TPlayer.direction;
			Vector velocity = Unit;
			velocity.Length = 19;
			int LoopTime;
			for (int i = 0; i < 50; i++)
			{
				player.NewProj(LaunchSource + Rand.NextVector2(16 * 3.5f, 0), velocity, MainProjs[Rand.Next(MainProjs.Length)], 320, 20f);
				LoopTime = Rand.Next(3, 6);
				for (int j = 0; j < LoopTime; j++)
				{
					player.NewProj(LaunchSource + Rand.NextVector2(16 * 8.5f,0), velocity * 0.95f + Rand.NextVector2(0, 10), LiningProjs[Rand.Next(LiningProjs.Length)], 270, 20f);
				}
				LaunchSource -= Unit;
			}
		}
	}
}
