using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.AuraSystem.Skills
{
	using Base;
	using Microsoft.Xna.Framework;
	using System.Threading;
	using Terraria.ID;

	public class FrozenCraze : Skill
	{
		private int[] Projs =
		{
			ProjectileID.IceSpike,
			ProjectileID.IceSickle,
			ProjectileID.IceBlock,
			ProjectileID.IceBlock,
			ProjectileID.IceBlock,
			ProjectileID.IceBolt,
			ProjectileID.FrostBlastFriendly,
			ProjectileID.FrostBlastFriendly,
			ProjectileID.NorthPoleSnowflake,
			ProjectileID.NorthPoleSnowflake,
			ProjectileID.NorthPoleSnowflake
		};
		public FrozenCraze() : base(SkillIDs.FrozenCraze)
		{
			CD = 60 * 30;
			Level = 50;
			MP = 60;
			Author = "zhou_Qi";
			Description = @"生成一条冰雪构成的足迹
""寒冰国度流传下来的秘法，由冰雪女王所保管""
""在冰霜之月升起的夜晚，人们曾目睹过她的姿容""";
			SetText();
		}
		public override void Release(StarverPlayer player, Vector2 vel)
		{
			AsyncRelease(player);
		}
		private async void AsyncRelease(StarverPlayer player)
		{
			await Task.Run(() =>
			{
				int damage = (int)Math.Log(player.Level);
				damage *= damage;
				damage *= 5;
				Vector2 Pos()
				{
					return player.Center + new Vector2(0, 16 * 2) + Rand.NextVector2(16 * 3.5f, 0);
				}
				int Tag = 10 * 8;
				while(Tag > 0)
				{
					player.NewProj(Pos(), Rand.NextVector2(0.5f, 0.1f) + new Vector2(0, 0.1f), Projs.Next(), damage, 20f);
					Thread.Sleep(125);
					Tag--;
				}
			});
		}
	}
}
