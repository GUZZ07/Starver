using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.PlayerBoosts.Skills
{
	
	using Microsoft.Xna.Framework;
	using Terraria.ID;
	public class MiracleMana : StarverSkill
	{
		private int[] TreasureBolts =
		{
			ProjectileID.AmethystBolt,
			ProjectileID.TopazBolt,
			ProjectileID.SapphireBolt,
			ProjectileID.EmeraldBolt,
			ProjectileID.RubyBolt,
			ProjectileID.DiamondBolt,
			ProjectileID.AmberBolt
		};
		/// <summary>
		/// 30%可能性, 发射小火花
		/// </summary>
		/// <param name="player"></param>
		private void SmallFlame(StarverPlayer player, Vector2 vel)
		{
			player.NewProj(player.Center, vel, ProjectileID.BallofFire, (int)(15 * Math.Log(player.Level)), 0f);
		}
		/// <summary>
		/// 30%可能性，发射各种宝石法杖弹幕
		/// </summary>
		/// <param name="player"></param>
		/// <param name="vel"></param>
		private unsafe void TreasureShots(StarverPlayer player, Vector2 vel)
		{
			int* Bolts = stackalloc int[TreasureBolts.Length];
			#region RandSelect
			{
				bool* Selected = stackalloc bool[TreasureBolts.Length];
				for (int i = 0; i < TreasureBolts.Length; i++)
				{
					int t;
					do
					{
						t = Rand.Next(TreasureBolts.Length);
					}
					while (Selected[t]);
					Selected[t] = true;
					Bolts[i] = TreasureBolts[t];
				}
			}
			#endregion
			vel.Length(19);
			Vector relativePos = new Vector(16 * 3.75f, 0);
			int damage = 60;
			damage += (int)(2 * Math.Sqrt(player.Level));
			damage = Math.Min(300, damage);
			for (int i = 0; i < TreasureBolts.Length; i++)
			{
				relativePos.Angle = Math.PI * 2 * i / TreasureBolts.Length;
				player.NewProj(player.Center + relativePos, vel, Bolts[i], damage, 5f);
			}
		}
		/// <summary>
		/// 30%可能性, 发生高伤害星云
		/// </summary>
		/// <param name="player"></param>
		/// <param name="vel"></param>
		private void NebulaBlaze(StarverPlayer player, Vector2 vel)
		{
			vel.Length(19.75f);
			int Max = 16;
			Vector relativePos = new Vector(16 * 3.75f, 0);
			int damage = (int)(Math.Log(player.Level));
			damage *= damage;
			damage *= 10;
			damage += 175;
			for (int i = 0; i < Max; i++)
			{
				relativePos.Angle = Math.PI * 2 * i / Max;
				player.NewProj(player.Center + relativePos, vel, ProjectileID.NebulaBlaze1, damage, 5f);
			}
		}
		/// <summary>
		/// 5%可能性,高伤害蓝色星云拳
		/// </summary>
		/// <param name="player"></param>
		/// <param name="vel"></param>
		private void NebulaBlazeEx(StarverPlayer player, Vector2 vel)
		{
			vel.Length(25);
			int Max = 32;
			Vector relativePos = new Vector(16 * 19.75f, 0);
			int damage = (int)(Math.Log(player.Level));
			damage *= damage;
			damage *= 20;
			damage += 350;
			for (int i = 0; i < Max; i++)
			{
				relativePos.Angle = Math.PI * 2 * i / Max;
				player.NewProj(player.Center + relativePos, vel, ProjectileID.NebulaBlaze2, damage, 5f);
			}
		}
		/// <summary>
		/// 5%,降低血量为原来的1/4
		/// </summary>
		/// <param name="player"></param>
		/// <param name="vel"></param>
		private void Hurt(StarverPlayer player,Vector2 vel)
		{
			player.Life /= 4;
		}
		private Action<StarverPlayer, Vector2>[] RandFuns;
		public MiracleMana()
		{
			CD = 60 * 10;
			MPCost = 50;
			LevelNeed = 70;
			Author = "zhou_Qi";
			Description = @"随机发射出火花/宝石弹/星云粉拳/星云蓝拳
""风险与收益总是成正比，有时甚至会是生命的代价""";
			RandFuns = new Action<StarverPlayer, Vector2>[]
			{
				#region 30%
				SmallFlame,
				SmallFlame,
				SmallFlame,
				SmallFlame,
				SmallFlame,
				SmallFlame,
				#endregion
				#region 30%
				TreasureShots,
				TreasureShots,
				TreasureShots,
				TreasureShots,
				TreasureShots,
				TreasureShots,
				#endregion
				#region 30%
				NebulaBlaze,
				NebulaBlaze,
				NebulaBlaze,
				NebulaBlaze,
				NebulaBlaze,
				NebulaBlaze,
				#endregion
				#region 5%
				NebulaBlazeEx,
				#endregion
				#region 5%
				Hurt
				#endregion
			};
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			RandFuns.Next()(player, vel);
		}
	}
}
