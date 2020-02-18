using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Starvers
{
	/// <summary>
	/// 16字节结构, 用于存储数据
	/// </summary>
	[type: StructLayout(LayoutKind.Explicit, Size = 16)]
	public struct Data16 : IEquatable<Data16>
	{
		#region ByteValues
		[field: FieldOffset(sizeof(byte) * 0)]
		public byte ByteValue0;
		[field: FieldOffset(sizeof(byte) * 1)]
		public byte ByteValue1;
		[field: FieldOffset(sizeof(byte) * 2)]
		public byte ByteValue2;
		[field: FieldOffset(sizeof(byte) * 3)]
		public byte ByteValue3;
		[field: FieldOffset(sizeof(byte) * 4)]
		public byte ByteValue4;
		[field: FieldOffset(sizeof(byte) * 5)]
		public byte ByteValue5;
		[field: FieldOffset(sizeof(byte) * 6)]
		public byte ByteValue6;
		[field: FieldOffset(sizeof(byte) * 7)]
		public byte ByteValue7;
		[field: FieldOffset(sizeof(byte) * 8)]
		public byte ByteValue8;
		[field: FieldOffset(sizeof(byte) * 9)]
		public byte ByteValue9;
		[field: FieldOffset(sizeof(byte) * 10)]
		public byte ByteValue10;
		[field: FieldOffset(sizeof(byte) * 11)]
		public byte ByteValue11;
		[field: FieldOffset(sizeof(byte) * 12)]
		public byte ByteValue12;
		[field: FieldOffset(sizeof(byte) * 13)]
		public byte ByteValue13;
		[field: FieldOffset(sizeof(byte) * 14)]
		public byte ByteValue14;
		[field: FieldOffset(sizeof(byte) * 15)]
		public byte ByteValue15;
		#endregion
		#region ShortValues
		[field: FieldOffset(sizeof(short) * 0)]
		public short ShortValue0;
		[field: FieldOffset(sizeof(short) * 1)]
		public short ShortValue1;
		[field: FieldOffset(sizeof(short) * 2)]
		public short ShortValue2;
		[field: FieldOffset(sizeof(short) * 3)]
		public short ShortValue3;
		[field: FieldOffset(sizeof(short) * 4)]
		public short ShortValue4;
		[field: FieldOffset(sizeof(short) * 5)]
		public short ShortValue5;
		[field: FieldOffset(sizeof(short) * 6)]
		public short ShortValue6;
		[field: FieldOffset(sizeof(short) * 7)]
		public short ShortValue7;
		#endregion
		#region IntValues
		[field: FieldOffset(sizeof(int) * 0)]
		public int IntValue0;
		[field: FieldOffset(sizeof(int) * 1)]
		public int IntValue1;
		[field: FieldOffset(sizeof(int) * 2)]
		public int IntValue2;
		[field: FieldOffset(sizeof(int) * 3)]
		public int IntValue3;
		#endregion
		#region LongValues
		[field: FieldOffset(sizeof(long) * 0)]
		public long LongValue0;
		[field: FieldOffset(sizeof(long) * 1)]
		public long LongValue1;
		#endregion
		#region FloatValues
		[field: FieldOffset(sizeof(float) * 0)]
		public float FloatValue0;
		[field: FieldOffset(sizeof(float) * 1)]
		public float FloatValue1;
		[field: FieldOffset(sizeof(float) * 2)]
		public float FloatValue2;
		[field: FieldOffset(sizeof(float) * 3)]
		public float FloatValue3;
		#endregion
		#region DoubleValues
		[field: FieldOffset(sizeof(double) * 0)]
		public double DoubleValue0;
		[field: FieldOffset(sizeof(double) * 1)]
		public double DoubleValue1;
		#endregion
		#region DecimalValues
		[field: FieldOffset(sizeof(decimal) * 0)]
		public decimal DecimalValue0;
		#endregion
		#region FixedBuffers
		[FieldOffset(0)]
		private unsafe fixed byte bytes[16];
		[FieldOffset(0)]
		private unsafe fixed short shorts[8];
		[FieldOffset(0)]
		private unsafe fixed int ints[4];
		#endregion
		#region Indexer
		public unsafe bool this[int index]
		{
			get
			{
				if (index < 0 || 16 * 8 <= index)
				{
					throw new IndexOutOfRangeException($"index: {index}");
				}
				return (bytes[index / 8] & 1 << (index % 8) )!= 0;
			}
			set
			{
				if (index < 0 || 16 * 8 <= index)
				{
					throw new IndexOutOfRangeException($"index: {index}");
				}
				if (value)
				{
					bytes[index / 8] |= (byte)(1 << (index % 8));
				}
				else
				{
					bytes[index / 8] &= (byte)~(1 << (index % 8));
				}
			}
		}
		#endregion
		#region Equals
		public static bool operator !=(Data16 left, Data16 right)
		{
			return !(left.DecimalValue0 == right.DecimalValue0);
		}
		public static bool operator ==(Data16 left, Data16 right)
		{
			return left.DecimalValue0 == right.DecimalValue0;
		}
		public override bool Equals(object obj)
		{
			if (obj is IEquatable<Data16> data)
			{
				return data.Equals(this);
			}
			return false;
		}
		public override int GetHashCode()
		{
			return DecimalValue0.GetHashCode();
		}
		public bool Equals(Data16 value)
		{
			return this == value;
		}
		#endregion
	}
}
