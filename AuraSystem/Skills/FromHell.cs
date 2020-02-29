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
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public class FromHell : Skill
	{
		public FromHell() : base(SkillIDs.FromHell)
		{
			Level = 90;
			MP = 30;
			CD = 60 * 25;
			Author = "zhou_Qi";
			Description = @"释放来自地狱的亡灵力量
""弥漫岩浆与热流的灰烬之地，无数的亡灵在其中挣扎翻腾，他们呼唤着同一个名字""";
			SetText();
		}
		public override void Release(StarverPlayer player, Vector2 vel)
		{
			AsyncRelease(player, (Vector)vel);
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
	}
}
