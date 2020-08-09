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
	public class FromHell : StarverSkill
	{
		public FromHell()
		{
			LevelNeed = 600;
			MPCost = 120;
			CD = 60 * 30;
			Author = "zhou_Qi";
			Description = @"释放来自地狱的亡灵力量
""弥漫岩浆与热流的灰烬之地，无数的亡灵在其中挣扎翻腾，他们呼唤着同一个名字""";
			Summary = "[600][击败骷髅王解锁]释放大量火焰弹幕造成群攻";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			AsyncRelease(player, vel);
		}
		private async void AsyncRelease(StarverPlayer player, Vector vector)
		{
			await Task.Run(() =>
			{
				try
				{
					int tag = 9;
					double angle;
					Vector2 pos;
					Vector2 velocity;
					while (tag >= 0)
					{
						for (int i = 0; i < 3; i++)
						{
							angle = -Math.PI / 2;
							angle += Rand.NextDouble(-Math.PI / 6, Math.PI / 6);
							player.NewProj(player.Center, Vector.FromPolar(angle, 13f), ProjectileID.HellfireArrow, 210, 20);
						}
						if (tag % 2 == 0)
						{
							for (int i = 0; i < 4; i++)
							{
								pos = Rand.NextVector2(16 * 3, 16 * 4);
								pos.X += 16 * 3 / 2;
								pos.X *= 5 / 3f;
								pos.X += 16 * 2;
								velocity = pos;
								velocity.Length(15);
								player.NewProj(player.Center + pos, velocity, ProjectileID.BallofFire, 180, 20);
							}
							for (int i = 0; i < 4; i++)
							{
								pos = Rand.NextVector2(16 * 3, 16 * 4);
								pos.X -= 16 * 3 / 2;
								pos.X *= 5 / 3f;
								pos.X -= 16 * 2;
								velocity = pos;
								velocity.Length(15);
								player.NewProj(player.Center + pos, velocity, ProjectileID.BallofFire, 180, 20);
							}
						}
						Thread.Sleep(15);
						tag--;
					}
					player.SetBuff(BuffID.Inferno, 3600);
				}
				catch
				{

				}
			});
		}
		public override bool CanSet(StarverPlayer player)
		{
			if (!NPC.downedBoss3)
			{
				player.SendText("该技能已被地牢的诅咒封印", 238, 232, 170);
				return false;
			}
			return base.CanSet(player);
		}
	}
}
