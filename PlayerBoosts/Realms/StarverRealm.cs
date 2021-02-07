using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;

namespace Starvers.PlayerBoosts.Realms
{
	public abstract class StarverRealm : IRealm
	{
		public int? TimeLeft { get; protected set; }

		public int? DefTimeLeft { get; set; }
		public virtual Vector Center { get; set; }
		public virtual Vector Velocity { get; set; }
		public bool Active { get; set; }

		public abstract bool InRange(Entity entity);
		public abstract bool Intersect(Entity entity);

		public virtual void Kill()
		{
			Active = false;
		}

		public virtual void Begin()
		{
			Active = true;
			TimeLeft = DefTimeLeft;
			Initialize();
		}

		public virtual void Update()
		{
			Center += Velocity;
			InternalUpdate();
			UpdateTimeLeft();
		}

		protected StarverRealm()
		{

		}

		protected void UpdateTimeLeft()
		{
			if (TimeLeft != null)
			{
				if (--TimeLeft == 0)
				{
					Kill();
				}
			}
		}

		protected abstract void Initialize();
		protected abstract void InternalUpdate();
	}
}
