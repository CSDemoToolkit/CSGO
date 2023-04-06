using System;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace DemoReader
{
    public unsafe ref struct SpanBitStream
    {
		public Span<byte> buff;
		public int idx = 0;

		byte* bytePtr;

		public SpanBitStream(Span<byte> buff)
		{
			this.buff = buff;
			fixed (byte* bytePtr = buff)
			{
				this.bytePtr = bytePtr;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ReadBit()
		{
			return (bytePtr[idx >> 3] & (1 << (idx++ & 7))) != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Skip(int bits)
		{
			idx += bits;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte ReadByte()
		{
			int offset = idx % 8;
			int remaining = 8 - offset;

			idx += 8;

			int byteIdx = idx >> 3;

			if (offset == 0)
				return bytePtr[byteIdx - 1];

			byte first = (byte)(((1 << remaining) - 1) << offset);
			byte last = (byte)((1 << offset) - 1);

			byte b = (byte)((bytePtr[byteIdx - 1] & first) >> offset);
			b |= (byte)((bytePtr[byteIdx] & last) << remaining);

			return b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Read2Bytes(short* buff)
		{
			int offset = idx % 8;

			idx += 8 * 2;

			int byteIdx = idx >> 3;

			int v = *(int*)&bytePtr[byteIdx - 2] >> offset;
			*buff = (short)v;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Read4Bytes(int* buff)
		{
			int offset = idx % 8;

			idx += 8 * 4;

			int byteIdx = idx >> 3;

			long v = *(long*)&bytePtr[byteIdx - 4] >> offset;
			*buff = (int)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte ReadBits(int bits)
		{
			int offset = idx % 8;
			int remaining = 8 - offset;

			idx += bits;

			int byteIdx = idx >> 3;

			if (remaining > bits)
				return (byte)((bytePtr[byteIdx] & (((1 << bits) - 1) << offset)) >> offset);
			else if (remaining == bits)
				return (byte)((bytePtr[byteIdx - 1] & (((1 << bits) - 1) << offset)) >> offset);

			byte first = (byte)(((1 << remaining) - 1) << offset);
			byte last = (byte)((1 << (bits - remaining)) - 1);

			byte b = (byte)((bytePtr[byteIdx - 1] & first) >> offset);
			b |= (byte)((bytePtr[byteIdx] & last) << remaining);

			return b;
		}

		public int CountOnes()
		{
			int offset = idx % 8;
			int byteIdx = idx >> 3;

			int c = BitOperations.TrailingZeroCount(~(*(ulong*)&bytePtr[byteIdx] | (ulong)(1 << offset) - 1)) - offset;
			if (c < 64 - offset)
				return c;

			int ones = c;
			do
			{
				byteIdx += 8;
				c = BitOperations.TrailingZeroCount(~*(ulong*)&bytePtr[byteIdx]);
				ones += c;
			} while (c == 64);

			return ones;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadBytes(int bits, scoped Span<byte> buff)
		{
			int bytes = bits / 8;
			for (int i = 0; i < bytes; i++)
			{
				buff[i] = ReadByte();
			}

			int remaining = bits - bytes * 8;
			if (remaining != 0)
				buff[bytes] = ReadBits(remaining);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadBytes(uint bits, scoped Span<byte> buff)
		{
			uint bytes = bits >> 3;
			for (int i = 0; i < bytes; i++)
			{
				buff[i] = ReadByte();
			}

			uint remaining = bits - bytes * 8;
			if (remaining != 0)
				buff[(int)bytes] = ReadBits((int)remaining);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int ReadInt(int bits)
        {
			int i;
			Read4Bytes(&i);
			Skip(-(32 - bits));
			i &= (1 << bits) - 1;

			return i;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint ReadUInt(int bits)
        {
			uint i;
			Read4Bytes((int*)&i);
			Skip(-(32 - bits));
			i &= (uint)((1 << bits) - 1);

			return i;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public long ReadLong(int bits)
		{
			long i;
			Read4Bytes((int*)&i);
			Skip(-(32 - bits));
			i &= (1 << bits) - 1;

			return i;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float ReadFloat()
		{
			float val = 0;
			Read4Bytes((int*)&val);

			return val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ReadString(int length)
        {
			Span<byte> buff = stackalloc byte[length];
			ReadBytes(buff.Length * 8, buff);

            return Encoding.UTF8.GetString(buff);
		}

		public void ReadUntill(byte b, scoped Span<byte> buff)
        {
            for (int i = 0; i < buff.Length; i++)
            {
                byte c = ReadByte();

                if (c == b)
                    return;

                buff[i] = c;
            }
        }

        public int ReadUntill(byte b1, byte b2, scoped Span<byte> buff)
        {
            for (int i = 0; i < buff.Length; i++)
            {
                byte c = ReadByte();

                if (c == b1 || c == b2)
                    return i;

                buff[i] = c;
            }

			return buff.Length;
        }
    }
}