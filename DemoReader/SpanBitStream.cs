using System;
using System.Buffers;
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
		public void Read4Bytes(byte* buff)
		{
			int offset = idx % 8;
			int remaining = 8 - offset;

			idx += 8 * 4;

			int byteIdx = idx >> 3;

			if (offset == 0)
			{
				buff[0] = bytePtr[byteIdx - 4];
				buff[1] = bytePtr[byteIdx - 3];
				buff[2] = bytePtr[byteIdx - 2];
				buff[3] = bytePtr[byteIdx - 1];
			}

			byte first = (byte)(((1 << remaining) - 1) << offset);
			byte last = (byte)((1 << offset) - 1);

			buff[0] = (byte)(((bytePtr[byteIdx - 4] & first) >> offset) | ((bytePtr[byteIdx - 3] & last) << remaining));
			buff[1] = (byte)(((bytePtr[byteIdx - 3] & first) >> offset) | ((bytePtr[byteIdx - 2] & last) << remaining));
			buff[2] = (byte)(((bytePtr[byteIdx - 2] & first) >> offset) | ((bytePtr[byteIdx - 1] & last) << remaining));
			buff[3] = (byte)(((bytePtr[byteIdx - 1] & first) >> offset) | ((bytePtr[byteIdx] & last) << remaining));
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadBytes(int bits, byte* buff)
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
			byte* buff = stackalloc byte[4];
			ReadBytes(bits, buff);

			return *(int*)buff;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint ReadUInt(int bits)
        {
			byte* buff = stackalloc byte[4];
            ReadBytes(bits, buff);

			return *(uint*)buff;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public long ReadLong(int bits)
		{
			byte* buff = stackalloc byte[8];
			ReadBytes(bits, buff);

			return *(long*)buff;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float ReadFloat()
		{
			byte* buff = stackalloc byte[4];
			Read4Bytes(buff);

			return *(float*)buff;
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