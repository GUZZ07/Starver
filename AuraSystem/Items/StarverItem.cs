using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Starvers.AuraSystem.Items
{
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public abstract class StarverItem
	{
		public int ItemType { get; }
		public int LevelNeed { get; }
		/// <summary>
		/// 若为null则使用原物品的useTime
		/// </summary>
		public int? UseDelay { get; }

		protected StarverItem(int itemid, int lvlneed, int? useDelayOverride = null)
		{
			ItemType = itemid;
			LevelNeed = lvlneed;
			UseDelay = useDelayOverride;
		}

		protected Vector ItemOffset(StarverPlayer player)
		{
			float length = player.HeldItem.height;
			Vector offset = Vector.FromPolar(player.ItemUseAngle, length);
			return offset;
		}

		public virtual bool CanUseItem(StarverPlayer player)
		{
			return player.Level >= LevelNeed;
		}
		public abstract void UseItem(StarverPlayer player);
		public virtual void ControlUseItem(StarverPlayer player)
		{

		}
		public virtual void UpdateInHand(StarverPlayer player)
		{

		}
	}
}
