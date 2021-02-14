using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.PlayerBoosts.Items
{
    public class ItemBoost
    {
        public int ItemType { get; }


		/// <summary>
		/// 若为null则使用原物品的useTime
		/// </summary>
		public int? UseDelay { get; }

		internal ItemBoost(int itemID, int? useDelayOverride = null)
		{
			ItemType = itemID;
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
			return true;
		}
		public virtual void UseItem(StarverPlayer player)
		{

		}
		/// <summary>
		/// 只在controlUseItem当中触发
		/// </summary>
		/// <param name="player"></param>
		public virtual void ControlUseItem(StarverPlayer player)
		{

		}
		public virtual void UpdateInHand(StarverPlayer player)
		{

		}
	}
}
