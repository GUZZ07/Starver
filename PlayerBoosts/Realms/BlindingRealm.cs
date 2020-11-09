using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Starvers.PlayerBoosts.Realms
{
	public class BlindingRealm<T> : SRealm<T>
		where T : IReflectiveShape, new()
	{
		private const int Proj = ProjectileID.Bat;
		private const int Blind = BuffID.Obstructed;
		private const int MaxTimeLeft = 60 * 60 * 2;

		public BlindingRealm() : this(new T())
		{
			
		}

		public BlindingRealm(T conditioner) : base(conditioner)
		{
			DefTimeLeft = MaxTimeLeft;
			conditioner.BorderProjID = Proj;
		}

		public override void Begin()
		{
			base.Begin();
		}

		protected override void InternalUpdate()
		{
			base.InternalUpdate();
			shape.Rotation += Math.PI / 180;
			if (TimeLeft % 60 == 0)
			{
				foreach (var player in Starver.Instance.Players)
				{
					if (player == null || !player.Alive)
						continue;
					if (InRange(player))
					{
						player.SetBuff(Blind, 60);
					}
				}
			}

		}
	}
}
