using System;
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
			internal unsafe fixed byte buffer[Size];
			[FieldOffset(16 * 0)]
			private BLFlags avaiables;
			[FieldOffset(EffectsOffSet)]
			private EffectBonus bonus;

			public const int Size = 16 * 4;

			private const int EffectsOffSet = sizeof(BLFlags) + sizeof(BLFlags) * 8 * sizeof(byte);

			public BLFlags AvaiableBLs
			{
				get => avaiables;
				set => avaiables = value;
			}

			public EffectBonus Bonus
			{
				get => bonus;
				set => bonus = value;
			}

			[IndexerName("Process")]
			public unsafe byte this[BLID id]
			{
				get
				{
					if (id == BLID.None)
					{
						throw new ArgumentException("id is BLID.None", nameof(id));
					}
					if (id >= BLID.Max)
					{
						throw new ArgumentException("id >= BLID.Max", nameof(id));
					}
					return buffer[(int)id + sizeof(BLFlags)];
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
					buffer[(int)id + sizeof(int)] = value;
				}
			}

			public unsafe static BLData Deserialize(byte[] binary)
			{
				if (binary.Length != Size)
				{
					throw new ArgumentException("binary.Length != 16 * 4", nameof(binary));
				}
				BLData data = default;
				for (int i = 0; i < Size; i++)
				{
					data.buffer[i] = binary[i];
				}
				return data;
			}
			public unsafe static byte[] Serialize(in BLData data)
			{
				byte[] value = new byte[Size];
				for (int i = 0; i < Size; i++)
				{
					value[i] = data.buffer[i];
				}
				return value;
			}
			public unsafe static void Serialize(in BLData data, byte[] target)
			{
				if (target.Length != Size)
				{
					throw new ArgumentException("target.Length != 16 * 4", nameof(target));
				}
				byte[] value = target;
				for (int i = 0; i < Size; i++)
				{
					value[i] = data.buffer[i];
				}
			}
		}
	}
}
