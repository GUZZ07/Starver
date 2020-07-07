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
		private const float RadiumMax = 16 * 55;
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
				ProjectileID.FrostBlastFriendly
			};
		}
		//打算让UltimateBlast直接继承这个,省点事,到时候直接重写AsyncRelease
		protected UltimateSlash(int para)
		{
			CD = 60 * 70;
			MPCost = 6000;
			LevelNeed = 20000;
			Author = "zhou_Qi";
			Description = @"""我们对此一无所知""
""蕴含着最终的力量""";
		}
		#endregion
		#region Release
		public override void Release(StarverPlayer player, Vector vel)
		{
			AsyncRelease(player);
		}
		protected virtual async void AsyncRelease(StarverPlayer player)
		{
			await Task.Run(() =>
			{
				try
				{
					player.SetBuff(BuffID.ShadowDodge, 60 * 3);
					player.SetBuff(BuffID.Webbed, 60 * 3);
					unsafe
					{
						Vector* Source = stackalloc Vector[Max];
						Vector* ReverseSource = stackalloc Vector[Max];
						Vector velocity = new Vector(15, 0);
						int damage = 1080 * 2;
						damage += (int)(Math.Sqrt(player.Level) * 3);
						for (int i = 0; i < Max; i++)
						{
							Source[i] = Vector.FromPolar(Math.PI * 2 * i / Max, RadiumMax);
							ReverseSource[i] = Source[i];
							ReverseSource[i].Angle += Math.PI / 4;
							ReverseSource[i].Length = 0;
						}
						#region First stage
						while (Source[0].Length > RadiumMax / 2)
						{
							for (int i = 0; i < Max; i++)
							{
								#region NoReverse
								velocity.Angle = Source[i].Angle - Math.PI / 2 - Math.PI / 12;
								player.NewProj(player.Center + Source[i], velocity, ProjectileID.FlamingJack, damage, 20f);
								Source[i].Length -= 16 * 2 / 3;
								Source[i].Angle += Math.PI * 2 / 120;
								#endregion
								#region Reverse
								velocity.Angle = ReverseSource[i].Angle + Math.PI / 2 + Math.PI / 12;
								player.NewProj(player.Center + ReverseSource[i], velocity, FromIn.Next(), damage * 2 / 3, 20f);
								ReverseSource[i].Length += 16 * 2 / 3;
								ReverseSource[i].Angle -= Math.PI * 2 / 120;
								#endregion
							}
							Thread.Sleep(10);
						}
						#endregion
						#region Second stage(Beams)
						damage /= 2;
						damage *= 3;
						for (int i = 0; i < Beams.Length; i++)
						{
							player.ProjCircle(player.Center, 16 * 4 + 16 * 8 * i, 13 + 2 * i, Beams[i], 10 + 5 * i, (int)(damage * (1 + 0.1f * i)));
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
