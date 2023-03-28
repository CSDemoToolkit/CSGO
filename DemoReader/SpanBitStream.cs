using System;
using System.Collections.Generic;
using System.Text;

namespace DemoReader
{
    public ref struct SpanBitStream
    {
        public Span<byte> buff;
        public int idx;

        public SpanBitStream(Span<byte> buff)
        {
            this.buff = buff;
			idx = 0;
        }

        public bool ReadBit()
        {
            return (buff[idx / 8] & (1 << idx++ % 8)) != 0;
        }

		public void Skip(int bits)
		{
			idx += bits;
		}

        public byte ReadByte()
        {
            byte b = 0;
            b |= (byte)((ReadBit() ? 1 : 0) << 0);
            b |= (byte)((ReadBit() ? 1 : 0) << 1);
            b |= (byte)((ReadBit() ? 1 : 0) << 2);
            b |= (byte)((ReadBit() ? 1 : 0) << 3);
            b |= (byte)((ReadBit() ? 1 : 0) << 4);
            b |= (byte)((ReadBit() ? 1 : 0) << 5);
            b |= (byte)((ReadBit() ? 1 : 0) << 6);
            b |= (byte)((ReadBit() ? 1 : 0) << 7);

            return b;
        }

#if NET7_0_OR_GREATER
		public void ReadBytes(int bits, scoped Span<byte> buff)
        {
            for (int i = 0; i < bits; i++)
            {
                if (!ReadBit())
                    continue;

                buff[i / 8] |= (byte)(1 << i % 8);
            }
        }

        public void ReadBytes(uint bits, scoped Span<byte> buff)
        {
            for (int i = 0; i < bits; i++)
            {
                if (!ReadBit())
                    continue;

                buff[i / 8] |= (byte)(1 << i % 8);
            }
        }
		
		public int ReadInt(int bits)
        {
            Span<byte> buff = stackalloc byte[4];
            ReadBytes(bits, buff);

            return BitConverter.ToInt32(buff);
        }

        public uint ReadUInt(int bits)
        {
            Span<byte> buff = stackalloc byte[4];
            ReadBytes(bits, buff);

            return BitConverter.ToUInt32(buff);
        }

		public long ReadLong(int bits)
		{
			Span<byte> buff = stackalloc byte[8];
			ReadBytes(bits, buff);

			return BitConverter.ToInt64(buff);
		}

		public ulong ReadULong(int bits)
		{
			Span<byte> buff = stackalloc byte[4];
			ReadBytes(bits, buff);

			return BitConverter.ToUInt64(buff);
		}

		public float ReadFloat(int bits)
		{
			Span<byte> buff = stackalloc byte[4];
			ReadBytes(bits, buff);

			return BitConverter.ToSingle(buff);
		}

		public double ReadDouble(int bits)
		{
			Span<byte> buff = stackalloc byte[8];
			ReadBytes(bits, buff);

			return BitConverter.ToDouble(buff);
		}

		public string ReadString(int length)
        {
			Span<byte> buff = stackalloc byte[length];
			ReadBytes(buff.Length * 8, buff);

            return Encoding.UTF8.GetString(buff);
		}

		public void ReadUntil(byte b, scoped Span<byte> buff)
        {
            for (int i = 0; i < buff.Length; i++)
            {
                byte c = ReadByte();

                if (c == b)
                    return;

                buff[i] = c;
            }
        }

        public int ReadUntil(byte b1, byte b2, scoped Span<byte> buff)
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
#else
		public byte[] ReadBytes(int bits)
		{
			var buff = new byte[(bits / 8) + 1];
			for (int i = 0; i < bits; i++)
			{
				if (!ReadBit())
					continue;

				buff[i / 8] |= (byte)(1 << i % 8);
			}
			return buff;
		}

		public byte[] ReadBytes(uint bits)
		{
			var buff = new byte[(bits / 8) + 1];
			for (int i = 0; i < bits; i++)
			{
				if (!ReadBit())
					continue;

				buff[i / 8] |= (byte)(1 << i % 8);
			}
			return buff;
		}

		public int ReadInt(int bits)
		{
			Span<byte> buff = stackalloc byte[4];
			buff = ReadBytes(bits);

			return BitConverter.ToInt32(buff);
		}

		public uint ReadUInt(int bits)
		{
			Span<byte> buff = stackalloc byte[4];
			buff = ReadBytes(bits);

			return BitConverter.ToUInt32(buff);
		}

		public long ReadLong(int bits)
		{
			Span<byte> buff = stackalloc byte[8];
			buff = ReadBytes(bits);

			return BitConverter.ToInt64(buff);
		}

		public ulong ReadULong(int bits)
		{
			Span<byte> buff = stackalloc byte[4];
			buff = ReadBytes(bits);

			return BitConverter.ToUInt64(buff);
		}

		public float ReadFloat(int bits)
		{
			Span<byte> buff = stackalloc byte[4];
			buff = ReadBytes(bits);

			return BitConverter.ToSingle(buff);
		}

		public double ReadDouble(int bits)
		{
			Span<byte> buff = stackalloc byte[8];
			buff = ReadBytes(bits);

			return BitConverter.ToDouble(buff);
		}

		public string ReadString(int length)
		{
			Span<byte> buff = stackalloc byte[length];
			buff = ReadBytes(buff.Length * 8);

			return Encoding.UTF8.GetString(buff);
		}

		public byte[] ReadUntil(byte b, int maxBytes)
		{
			List<byte> buff = new List<byte>();
			for (int i = 0; i < maxBytes; i++)
			{
				byte c = ReadByte();

				if (c == b)
					return buff.ToArray();

				buff[i] = c;
			}
			return new byte[0];
		}

		public (byte[], int) ReadUntil(byte b1, byte b2, int maxBytes)
		{
			List<byte> buff = new List<byte>();
			for (int i = 0; i < maxBytes; i++)
			{
				byte c = ReadByte();

				if (c == b1 || c == b2)
					return (buff.ToArray(), i);

				buff[i] = c;
			}

			return (new byte[0], maxBytes);
		}
#endif
	}
}