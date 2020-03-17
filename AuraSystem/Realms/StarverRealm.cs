using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;

namespace Starvers.AuraSystem.Realms
{
	public abstract class StarverRealm : IRealm
	{
		private bool KillByTimeLeft;

		public int TimeLeft { get; protected set; }

		public int DefaultTimeLeft { get; set; }
		public virtual Vector2 Center { get; set; }
		public bool Active { get; set; }

		public abstract bool InRange(Entity entity);
		public abstract bool IsCross(Entity entity);

		public virtual void Kill()
		{
			Active = false;
		}

		public virtual void Start()
		{
			Active = true;
			TimeLeft = DefaultTimeLeft;
			SetDefault();
		}

		public virtual void Update()
		{
			InternalUpdate();
			UpdateTimeLeft();
		}

		protected StarverRealm(bool killByTimeLeft)
		{
			KillByTimeLeft = killByTimeLeft;
		}

		protected void UpdateTimeLeft()
		{
			if (--TimeLeft == 0)
			{
				if (KillByTimeLeft)
					Kill();
			}
		}

		protected abstract void SetDefault();
		protected abstract void InternalUpdate();
	}
}
