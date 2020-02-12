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
	using Terraria;
	using Terraria.ID;

	public class NatureGuard : Skill
	{
		public NatureGuard() : base(SkillIDs.NatureGuard)
		{
			MP = 10;
			CD = 60 * 22;
			Level = 50;
			Author = "zhou_Qi";
			Description = @"生成几个纯粹由自然之力构成的哨卫
""稍显愚笨，不过这也应当是一名哨兵的职责""";
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
				try
				{
					int[] arr = new int[Rand.Next(4) + 1];
					int damage = (int)(50 * Math.Log(player.Level));
					for (int i = 0; i < arr.Length; i++)
					{
						arr[i] = player.NewProj(player.Center, Rand.NextVector2(9), ProjectileID.SporeGas, damage, 20f);
					}
					while (Main.projectile[arr[0]].active && Main.projectile[arr[0]].owner == player.Index)
					{
						foreach (var idx in arr)
						{
							player.NewProj(Main.projectile[idx].Center, Rand.NextVector2(15), ProjectileID.TerrarianBeam, damage * 3 / 2, 20f);
						}
						Thread.Sleep(25);
					}
				}
				catch
				{

				}
			});
		}
	}
}
