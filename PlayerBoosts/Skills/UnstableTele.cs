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
	public class UnstableTele : StarverSkill
	{
		private const int Max = 18;
		public UnstableTele()
		{
			CD = 60 * 10;
			MPCost = 20;
			LevelNeed = 5;
			Author = "zhou_Qi";
			Description = @"抛射出一颗玻璃彩球，然后进行随机传送
""尽管深受谴责，这仍是期许着不劳而获者的最爱""";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			AsyncRelease(player, vel);
		}
		private async void AsyncRelease(StarverPlayer player,Vector2 vel)
		{
			await Task.Run(() =>
			{
				try
				{
					int damage = 800;
					Vector Velocity = (Vector)vel * 3;
					IProjSet Projs = new ProjStack(Max + 5);
					int[] idxes = player.ProjCircleRet(player.Center, 16 * 3.45f, 0, ProjectileID.DemonScythe, Max, damage);
					Projs.Push(idxes, Velocity);
					int idx = player.NewProj(player.Center, Vector.Zero, ProjectileID.MonkStaffT3_AltShot, damage * 3 / 2);
					Projs.Push(idx, Velocity * 2);
					Thread.Sleep(1000);
					Projs.Launch();
					player.Center += Rand.NextVector2(16 * 100, 16 * 100);
				}
				catch
				{

				}
			});
		}
	}
}
