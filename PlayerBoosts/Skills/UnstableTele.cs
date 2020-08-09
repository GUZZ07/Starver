using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.PlayerBoosts.Skills
{
	
	using Microsoft.Xna.Framework;
	using System.Threading;
    using Terraria;
    using Terraria.ID;
	public class UnstableTele : StarverSkill
	{
		private const int Max = 18;
		public UnstableTele()
		{
			CD = 60 * 10;
			MPCost = 100;
			LevelNeed = 350;
			Author = "zhou_Qi";
			Description = @"发射一圈弹幕，然后进行随机传送
""尽管深受谴责，这仍是期许着不劳而获者的最爱""";
			Summary = "[350][击败蠕虫/大脑解锁]生成一圈滞留弹幕并短距离随机传送";
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
					int damage = 600;
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
		public override bool CanSet(StarverPlayer player)
		{
			if (!NPC.downedBoss2)
			{
				player.SendText("该技能已被血腐的力量封印", 199, 21, 133);
				return false;
			}
			return base.CanSet(player);
		}
	}
}
