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

	public class NatureGuard : StarverSkill
	{
		public NatureGuard()
		{
			MPCost = 350;
			CD = 60 * 25;
			LevelNeed = 800;
			Author = "zhou_Qi";
			Description = @"生成几个纯粹由自然之力构成的哨卫
""稍显愚笨，不过这也应当是一名哨兵的职责""";
			Summary = "[800][击败骷髅王解锁]生成几个移动锚点发射攻击弹幕";
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
					int[] arr = new int[Rand.Next(4) + 1];
					int damage = (int)(50 * Math.Log(player.Level));
					for (int i = 0; i < arr.Length; i++)
					{
						arr[i] = player.NewProj(player.Center, Rand.NextVector2(9), ProjectileID.SporeGas, damage, 20f);
					}
					int timer=0;
					while (Main.projectile[arr[0]].active && Main.projectile[arr[0]].owner == player&&timer < 3000)
					{
						foreach (var idx in arr)
						{
							player.NewProj(Main.projectile[idx].Center, Rand.NextVector2(15), ProjectileID.TerrarianBeam, damage * 3 / 2, 20f);
						}
						timer+=25;Thread.Sleep(25);
					}
				}
				catch
				{

				}
			});
		}
	}
}
