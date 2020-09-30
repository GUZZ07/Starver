using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace Starvers.PlayerBoosts.Realms
{
	using Shapes;
	/// <summary>
	/// 凋亡结界
	/// 使处于结界中的玩家不断扣血
	/// </summary>
	public class ApoptoticRealm : SRealm<IRealmShape>
	{
		public ApoptoticRealm(IRealmShape shape = null) : base(shape ?? new Circle { Radium = 16 * 30 })
		{
			DefTimeLeft = 60 * 20;
			shape.BorderProjID = ProjectileID.VortexVortexLightning;
			shape.DefProjTimeLeft = 60;
		}

		protected override void Initialize()
		{

		}

		protected override void InternalUpdate()
		{
			shape.Update(timer);
			if (timer % 20 == 0)
			{
				int damage;
				foreach (var player in Starver.Instance.Players)
				{
					if (player?.Alive != true)
					{
						continue;
					}
					if (InRange(player))
					{
						damage = Math.Max(1, player.Life / 60);
						player.Life -= damage;
						player.SendCombatText(damage.ToString(), Color.Blue);
					}
				}
			}

		}

	}
}
