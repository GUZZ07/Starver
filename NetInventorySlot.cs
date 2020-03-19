using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Starvers
{
	using Item = Terraria.Item;
	public struct NetInventorySlot
	{
		private readonly int? owner;

		public int? Slot { get; }

		public StarverPlayer Owner => Starver.Players[(int)owner];
		public Item Item => Owner.TPlayer.inventory[(int)Slot];
		public int MaxStack => Item.maxStack;

		public int Type
		{
			get => Item.type;
			set => SetItemType(value);
		}
		public int Stack
		{
			get => Item.stack;
			set => SetItemStack(value);
		}
		public byte Prefix
		{
			get => Item.prefix;
			set => SetItemPrefix(value);
		}

		public NetInventorySlot(int who, int slot)
		{
			owner = who;
			Slot = slot;
		}

		public void ToAir()
		{
			Type = 0;
		}
		public void SetDefaults(int type)
		{
			Item.SetDefaults(type);
			SendData();
		}

		#region Privates
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SetItemPrefix(byte prefix)
		{
			Item.prefix = prefix;
			SendData();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SetItemStack(int stack)
		{
			Item.stack = stack;
			SendData();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SetItemType(int type)
		{
			Item.type = type;
			SendData();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SendData()
		{
			Owner.SendData(PacketTypes.PlayerSlot, "", Owner, (int)Slot);
		}
		#endregion
	}
}
