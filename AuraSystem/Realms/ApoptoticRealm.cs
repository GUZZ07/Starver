using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace Starvers.AuraSystem.Realms
{
	using Vector = TOFOUT.Terraria.Server.Vector2;
	/// <summary>
	/// 凋亡结界
	/// 使处于结界中的玩家不断扣血
	/// </summary>
	public class ApoptoticRealm : CircleRealm
	{
		private double angle;

		public ApoptoticRealm() : base(true)
		{
			DefaultTimeLeft = 60 * 20;
		}

		protected override void SetDefault()
		{
			Radium = 16 * 30;
		}

		protected override void InternalUpdate()
		{
			if(TimeLeft % 20 == 0)
			{
				int damage;
				foreach (var player in Starver.Players)
				{
					if (player == null || !player.Active)
						continue;
					if (InRange(player))
					{
						damage = Math.Max(1, player.Life / 60);
						player.Life -= damage;
						player.SendCombatMsg(damage.ToString(), Color.Blue);
					}
				}
			}
			{
				int idx = Utils.NewProj(Center + Vector.FromPolar(angle, Radium), new Vector2(0.001f, 0.001f), ProjectileID.VortexVortexLightning, 1, 20, Main.myPlayer);
				Main.projectile[idx].aiStyle = -2;
				Main.projectile[idx].timeLeft = 60;
				angle += MathHelper.Pi / 30;
			}
		}

	}
}
