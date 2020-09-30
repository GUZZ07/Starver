using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Starvers.PlayerBoosts.Realms
{
	public class SRealm<T> : StarverRealm
		where T : IRealmShape, new()
	{
		private T shape;

		protected int timer;

		public SRealm()
		{
			shape = new T();
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
			shape.Begin(this);
		}

		protected override void InternalUpdate()
		{
			shape.Update(timer);
			timer++;
		}
	}
}
