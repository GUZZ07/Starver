using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Starvers.PlayerBoosts.Skills
{
	public class ChordMana : StarverSkill
	{
		public ChordMana()
		{
			CD = 60 * 40;
			MPCost = 100;
			LevelNeed = 1000;
			Description = @"释放音符的力量
""和谐的音符给人以享受，而嘈杂的旋律则足以给人带来精神上的重创""
""她，他们，始终重复着相同的错误，在闭合的旋律中僵硬地起舞""";
			Author = "zhou_Qi";
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
					unsafe
					{
						int Count = 20;
						Vector* Block = stackalloc Vector[Count];
						Vector vel;
						int level = 0;
						int MaxLevels = 7;
						double offset = 0;
						float length = 16 * 6f;
						ProjQueue Projs = new ProjQueue((MaxLevels + 1) * Count);
						{
							while (level < MaxLevels)
							{
								for (int i = 0; i < Count; i++)
								{
									Block[i] = Vector.FromPolar(offset + Math.PI * 2 * i / Count, length);
									vel = Block[i];
									vel.Angle -= Math.PI * 2 / 4;
									vel.Length = 8 - level;
									Projs.Push(player.NewProj(player.Center + Block[i], Block[i].ToLenOf(0.4f), ProjectileID.TiedEighthNote, 420 - level * 6, 4), vel);
									if (i % 5 == 0)
									{
										Thread.Sleep(1);
									}
								}
								length += 16 * 3.5f;
								offset += Math.PI / 3 / 7;
								level++;
								Thread.Sleep(100 - 1 * Count / 5);
							}
							while (level-- >= 0)
							{
								Projs.Launch(Count);
								Thread.Sleep(200);
							}
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
