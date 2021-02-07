using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.PlayerBoosts.Items
{
    public abstract class ItemBoost
    {
		public ItemBoostID BoostID { get; }
        public int ItemType { get; }


		/// <summary>
		/// 若为null则使用原物品的useTime
		/// </summary>
		public int? UseDelay { get; }

		protected ItemBoost(int itemID, int? useDelayOverride = null)
		{
			ItemType = itemID;
			UseDelay = useDelayOverride;
			BoostID = (ItemBoostID)Enum.Parse(typeof(ItemBoostID), GetType().Name);
		}


		protected Vector ItemOffset(StarverPlayer player)
		{
			float length = player.HeldItem.height;
			Vector offset = Vector.FromPolar(player.ItemUseAngle, length);
			return offset;
		}

		public virtual bool CanUseItem(StarverPlayer player)
		{
			return true;
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
