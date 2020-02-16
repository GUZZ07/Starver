using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.NPCSystem
{
	public abstract class MultiLifeNPC : StarverNPC
	{
		public int Lifes { get; set; }
		public int LifesMax { get; set; }
		protected MultiLifeNPC()
		{
			LifesMax = 1;
			Lifes = LifesMax;
		}
		public override void PreStrike(ref int RealDamage, float KnockBack, StarverPlayer player)
		{
			base.PreStrike(ref RealDamage, KnockBack, player);
			if(RealDamage >= Life && Lifes > 0)
			{
				Lifes--;
				RealDamage -= Life;
				Life = LifeMax;
			}
		}
	}
}
