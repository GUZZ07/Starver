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
	public class UltimateSlash : UltimateSkill
	{
		#region Fields
		private const int Max = 4;
		private const float RadiumMax = 16 * 45;
		protected int[] FromOut;
		protected int[] FromIn;
		private int[] Beams =
		{
			ProjectileID.LightBeam,
			ProjectileID.NightBeam,
			ProjectileID.TerraBeam,
			ProjectileID.InfluxWaver
		};
		#endregion
		#region Ctor
		public UltimateSlash() : this(default)
		{
			FromOut = new int[]
			{
				ProjectileID.FlamingJack,
				ProjectileID.FlamingJack,
				ProjectileID.FlamingJack,
				ProjectileID.Hellwing,
				ProjectileID.Hellwing
			};
			FromIn = new int[]
			{
				ProjectileID.EnchantedBeam,
				ProjectileID.SwordBeam,
			};
		}
		protected UltimateSlash(int para)
		{
			Author = "zhou_Qi";
			Summary = "[20000][最终技能]释放极限的斩击";
		}
		#endregion
		#region Release
		protected override void InternalRelease(StarverPlayer player, Vector vel)
		{
			AsyncRelease(player);
		}
		protected virtual void AsyncRelease(StarverPlayer player)
		{
			Task.Run(() =>
			{
				try
				{
					player.SetBuff(BuffID.ShadowDodge, 60 * 3);
					player.SetBuff(BuffID.Webbed, 60 * 3);
					unsafe
					{
						Vector* Source = stackalloc Vector[Max];
						Vector* ReversedSource = stackalloc Vector[Max];
						Vector velocity = new Vector(15, 0);
						int damage = 1080 * 2;
						damage += (int)(Math.Sqrt(player.Level) * 3);
						for (int i = 0; i < Max; i++)
						{
							Source[i] = Vector.FromPolar(Math.PI * 2 * i / Max, RadiumMax);
							ReversedSource[i] = Source[i];
							ReversedSource[i].Angle += Math.PI / 2;
						}
						#region First stage
						var rotation = 0.0;
						while (rotation < 2 * Math.PI)
						{
							for (int i = 0; i < Max; i++)
							{
								#region NoReverse
								velocity.Angle = Source[i].Angle - Math.PI / 2 - Math.PI / 12;
								player.NewProj(player.Center + Source[i], velocity, FromIn[0], damage, 20f);
								// Source[i].Length -= 16 * 2 / 3;
								Source[i].Angle += Math.PI / 2 / 30;
								#endregion
								#region Reverse
								velocity.Angle = ReversedSource[i].Angle + Math.PI / 2 + Math.PI / 12;
								player.NewProj(player.Center + ReversedSource[i], velocity, FromIn[1], damage, 20f);
								// ReversedSource[i].Length += 16 * 2 / 3;
								ReversedSource[i].Angle -= Math.PI / 2 / 30;
								#endregion
							}
							rotation += Math.PI / 2 / 30;
							Thread.Sleep(2000 / 30);
						}
						#endregion
						#region Second stage(Beams)
						damage /= 2;
						damage *= 3;
						for (int i = 0; i < Beams.Length; i++)
						{
							player.ProjCircle(player.Center, 16 * 4 + 16 * 8 * i, -13 - 2 * i, Beams[i], 10 + 5 * i, (int)(damage * (1 + 0.1f * i)));
							Thread.Sleep(175 + 25 * i);
						}
						#endregion
					}
				}
				catch
				{

				}
			});
		}
		#endregion
	}
}
