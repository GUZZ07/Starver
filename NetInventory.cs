using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers
{
	public class NetInventory
	{
		private NetInventorySlot[] slots;
		public StarverPlayer Player { get; }
		public NetInventory(StarverPlayer player)
		{
			Player = player;
			slots = new NetInventorySlot[player.TPlayer.inventory.Length];
			for (int i = 0; i < slots.Length; i++)
			{
				slots[i] = new NetInventorySlot(player, i);
			}
		}
		public NetInventorySlot this[int slot] => slots[slot];
	}
}
