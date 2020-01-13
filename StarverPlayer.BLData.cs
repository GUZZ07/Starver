﻿using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Starvers
{
    using TaskSystem;
	public partial class StarverPlayer
	{
		// 64 bytes
		[StructLayout(LayoutKind.Explicit)]
		public struct BLData
		{
			[FieldOffset(16 * 0)]
			private unsafe fixed byte buffer[16 * 4];
			[FieldOffset(16 * 0)]
			private Data16 data0;
			[FieldOffset(16 * 1)]
			private Data16 data1;
			[FieldOffset(16 * 2)]
			private Data16 data2;
			[FieldOffset(16 * 3)]
			private Data16 data3;

			public BLFlags AvaiableBLs
			{
				get => (BLFlags)data0.ByteValue0;
				set => data0.ByteValue0 = (byte)value;
			}
			[IndexerName("Process")]
			public unsafe byte this[BLID id]
			{
				get
				{
					if(id == BLID.None)
					{
						throw new ArgumentException("id is BLID.None", nameof(id));
					}
					if(id >= BLID.Max)
					{
						throw new ArgumentException("id >= BLID.Max", nameof(id));
					}
					return buffer[(int)id];
				}
				set
				{
					if (id == BLID.None)
					{
						throw new ArgumentException("id is BLID.None", nameof(id));
					}
					if (id >= BLID.Max)
					{
						throw new ArgumentException("id >= BLID.Max", nameof(id));
					}
					buffer[(int)id] = value;
				}
			}

			public unsafe static BLData Deserialize(byte[] binary)
			{
				if (binary.Length != 16 * 4)
				{
					throw new ArgumentException("binary.Length != 4 * 16", nameof(binary));
				}
				BLData data = default;
				for (int i = 0; i < 16 * 4; i++)
				{
					data.buffer[i] = binary[i];
				}
				return data;
			}
			public unsafe static byte[] Serialize(in BLData data)
			{
				byte[] value = new byte[16 * 4];
				for (int i = 0; i < 16 * 4; i++)
				{
					value[i] = data.buffer[i];
				}
				return value;
			}
			public unsafe static void Serialize(in BLData data, byte[] target)
			{
				if(target.Length != 16 * 4)
				{
					throw new ArgumentException("target.Length != 16 * 4", nameof(target));
				}
				byte[] value = target;
				for (int i = 0; i < 16 * 4; i++)
				{
					value[i] = data.buffer[i];
				}
			}
		}
	}
}
