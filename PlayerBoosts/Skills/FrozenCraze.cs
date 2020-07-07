using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.PlayerBoosts.Skills
{
	
	using Microsoft.Xna.Framework;
	using System.Threading;
	using Terraria.ID;

	public class FrozenCraze : StarverSkill
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
		public FrozenCraze()
		{
			CD = 60 * 30;
			LevelNeed = 50;
			MPCost = 60;
			Author = "zhou_Qi";
			Description = @"生成一条冰雪构成的足迹
""寒冰国度流传下来的秘法，由冰雪女王所保管""
""在冰霜之月升起的夜晚，人们曾目睹过她的姿容""";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			AsyncRelease(player);
		}
		private async void AsyncRelease(StarverPlayer player)
		{
			await Task.Run(() =>
			{
				try
				{
					int damage = (int)Math.Log(player.Level);
					damage *= damage;
					damage *= 5;
					Vector2 RandPos()
					{
						return player.Center + new Vector2(0, 16 * 2) + Rand.NextVector2(16 * 3.5f, 0);
					}
					Vector2 RandVel()
					{
						return Rand.NextVector2(0.5f, 0.1f) + new Vector2(0, 0.1f);
					}
					int Tag = 10 * 8;
					while (Tag > 0 && player.Alive)
					{
						player.NewProj(RandPos(), RandVel(), Projs.Next(), damage, 20f);
						Thread.Sleep(125);
						Tag--;
					}
				}
				catch
				{

				}
			});
		}
	}
}
