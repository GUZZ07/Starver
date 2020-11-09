using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Starvers.PlayerBoosts.Realms
{
	public class SRealm<T> : StarverRealm
		where T : IRealmShape
	{
		protected T shape;

		protected int timer;

		public SRealm()
		{
			shape = Activator.CreateInstance<T>();
		}

		protected SRealm(T shape)
		{
			this.shape = shape;
		}

		public override void Begin()
		{
			base.Begin();
			shape.Begin(this);
		}

        public override void Kill()
        {
            base.Kill();
			shape.Kill();
        }

        public override bool InRange(Entity entity)
		{
			return shape.InRange(entity);
		}

		public override bool Intersect(Entity entity)
		{
			return shape.Intersect(entity);
		}

		protected override void Initialize()
		{
		}

		protected override void InternalUpdate()
		{
			shape.Update(timer);
			timer++;
		}
	}
}
