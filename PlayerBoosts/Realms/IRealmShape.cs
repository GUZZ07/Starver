using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Starvers.PlayerBoosts.Realms
{
	public interface IRealmShape
	{
		public void Begin(IRealm owner);
		public void Kill();
		public void Update(int timer);
		public bool InRange(Entity entity);
		public bool Intersect(Entity entity);

		/// <summary>
		/// 判断entity是否在边界上
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public bool AtBorder(Entity entity);

		/// <summary>
		/// 弹幕的默认TimeLeft
		/// </summary>
		public int? DefProjTimeLeft
		{
			get;
			set;
		}
		/// <summary>
		/// 边界弹幕ID
		/// </summary>
		public int? BorderProjID
		{
			get;
			set;
		}

	}

	public interface IReflectiveShape : IRealmShape
	{
		public void Reflect(Entity entity);
	}
}
