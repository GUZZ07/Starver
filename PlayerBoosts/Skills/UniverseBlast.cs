using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace Starvers.PlayerBoosts.Skills
{
	public class UniverseBlast : UltimateSlash
	{
		private string msg;
		private readonly int[] Projs =
		{
			ProjectileID.NebulaBlaze1,
			ProjectileID.NebulaBlaze1,
			ProjectileID.NebulaBlaze1,
			ProjectileID.NebulaBlaze1,
			ProjectileID.NebulaBlaze1,
			ProjectileID.NebulaBlaze2,
			ProjectileID.NebulaBlaze2,
			ProjectileID.CrystalBullet,
			ProjectileID.CrystalBullet,
			ProjectileID.CrystalBullet,
			ProjectileID.CrystalBullet,
			ProjectileID.CrystalBullet,
			ProjectileID.CrystalLeafShot,
			ProjectileID.CrystalLeafShot,
			ProjectileID.CrystalLeafShot,
			ProjectileID.CrystalLeafShot,
			ProjectileID.CrystalStorm,
			ProjectileID.CrystalStorm,
			ProjectileID.CrystalStorm,
			ProjectileID.CrystalStorm,
			ProjectileID.CrystalStorm,
		};
		public UniverseBlast()
		{
			msg = @"我就是懒, 怎么着?
有本事你顺着网线来打我啊";
		}
		protected override void AsyncRelease(StarverPlayer player)
		{
			Task.Run(() =>
			{
				try
				{
					unsafe
					{
						int count = Rand.Next(6, 15);
						int damage = 1500;
						damage += (int)(5 * Math.Sqrt(player.Level));

						int* Indexes = stackalloc int[count];
						Vector* Positions = stackalloc Vector[count];
						Vector2* AlsoPositions = (Vector2*)Positions;



						for (int i = 0; i < count; i++)
						{
							AlsoPositions[i] = player.Center;
							AlsoPositions[i] += Rand.NextVector2(16 * 50, 16 * 45);
							Indexes[i] =
							player.NewProj(AlsoPositions[i], Rand.NextVector2(0.35f), ProjectileID.NebulaArcanum, damage + Rand.Next(50));
						}
						var timer = 0;

						while (timer < 4000)
						{
							for (int i = 0; i < count; i++)
							{
								#region Fix
								var proj = Main.projectile[Indexes[i]];
								if (!proj.active || proj.type != ProjectileID.NebulaArcanum)
								{
									Indexes[i] = player.NewProj(AlsoPositions[i], Rand.NextVector2(0.35f), ProjectileID.NebulaArcanum, damage + Rand.Next(50));
									proj = Main.projectile[Indexes[i]];
								}
								proj.position = AlsoPositions[i];
								proj.SendData();
								#endregion
								player.NewProj(Main.projectile[Indexes[i]].Center, Rand.NextVector2(13 + Rand.Next(6)), Projs.Next(), damage / Rand.Next(2, 4) + Rand.Next(70));
								Thread.Sleep(2);
								player.NewProj(Main.projectile[Indexes[i]].Center, Rand.NextVector2(13 + Rand.Next(6)), Projs.Next(), damage / Rand.Next(2, 4) + Rand.Next(70));
							}
							Thread.Sleep(100);
							timer += 100;
						}
						for (int i = 0; i < count; i++)
						{
							AlsoPositions[i] = player.Center;
							Positions[i] += Vector.FromPolar(Math.PI * 2 * i / count + Math.PI / 2, 16 * 60);
							player.ProjSector(AlsoPositions[i], 19, 16 * 3, Positions[i].Angle + Math.PI, Math.PI / 3, damage, ProjectileID.DD2SquireSonicBoom, 5);
						}
					}
				}
				catch
				{

				}
			});
		}
	}
}
