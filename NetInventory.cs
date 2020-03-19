using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers
{
	public class NetInventory
	{
		public StarverPlayer Player { get; }
		public NetInventory(StarverPlayer player)
		{
			Player = player;
		}
		public NetInventorySlot this[int slot]
		{
			get
			{
				if (slot < 0 || Player.TPlayer.inventory.Length <= slot)
				{
					throw new IndexOutOfRangeException($"slot: {slot}");
				}
				return new NetInventorySlot(Player, slot);
			}
		}
	}
}
