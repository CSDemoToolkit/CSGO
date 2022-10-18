using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace DemoReader
{
    static class SpanStreamProtobufExtensions
    {
        static int ReadProtobufVarIntStub(this ref SpanStream<byte> reader)
        {
            int idx = 0;
            
            byte b = 0x80;
            int result = 0;
            for (int count = 0; (b & 0x80) != 0; count++)
            {
                b = reader.Read();
                idx++;

                if (count < 4 || count == 4 && ((b & 0xF8) == 0 || (b & 0xF8) == 0xF8))
                {
                    result |= (b & ~0x80) << (7 * count);
                }
                else
                {
                    if (count >= 10)
                    {
                        throw new OverflowException("Nope nope nope nope! 10 bytes max!");
                    }

                    if (count == 9 ? b != 1 : (b & 0x7F) != 0x7F)
                    {
                        throw new NotSupportedException("more than 32 bits are not supported");
                    }
                }
            }

            return result;
        }

        // MSB masks (protobuf varint end signal)
        private const uint MSB_1 = 0x00000080;
        private const uint MSB_2 = 0x00008000;
        private const uint MSB_3 = 0x00800000;
        private const uint MSB_4 = 0x80000000;

        // byte masks (except MSB)
        private const uint MSK_1 = 0x0000007F;
        private const uint MSK_2 = 0x00007F00;
        private const uint MSK_3 = 0x007F0000;
        private const uint MSK_4 = 0x7F000000;

        public static int ReadProtobufVarInt(this ref SpanStream<byte> reader)
        {
            // Start by overflowingly reading 32 bits.
            // Reading beyond the buffer contents is safe in this case,
            // because the sled ensures that we stay inside of the buffer.
            uint buf = PeekInt(ref reader, 32);

            // always take the first bytes; others if necessary
            uint result = buf & MSK_1;
            if ((buf & MSB_1) != 0)
            {
                result |= (buf & MSK_2) >> 1;
                if ((buf & MSB_2) != 0)
                {
                    result |= (buf & MSK_3) >> 2;
                    if ((buf & MSB_3) != 0)
                    {
                        result |= (buf & MSK_4) >> 3;
                        if ((buf & MSB_4) != 0)
                            // dammit, it's too large (probably negative)
                            // fall back to the slow implementation, that's rare
                        {
                            return reader.ReadProtobufVarIntStub();
                        }

                        reader.Skip(4);
                    }
                    else
                    {
                        reader.Skip(3);
                    }
                }
                else
                {
                    reader.Skip(2);
                }
            }
            else
            {
                reader.Skip(1);
            }

            return unchecked((int)result);
        }

        public static uint ReadProtobufVarUInt(this ref SpanStream<byte> reader)
        {
            // Start by overflowingly reading 32 bits.
            // Reading beyond the buffer contents is safe in this case,
            // because the sled ensures that we stay inside of the buffer.
            uint buf = PeekInt(ref reader, 32);

            // always take the first bytes; others if necessary
            uint result = buf & MSK_1;
            if ((buf & MSB_1) != 0)
            {
                result |= (buf & MSK_2) >> 1;
                if ((buf & MSB_2) != 0)
                {
                    result |= (buf & MSK_3) >> 2;
                    if ((buf & MSB_3) != 0)
                    {
                        result |= (buf & MSK_4) >> 3;
                        if ((buf & MSB_4) != 0)
                        // dammit, it's too large (probably negative)
                        // fall back to the slow implementation, that's rare
                        {
                            throw new Exception("Number is too large or negative.");
                        }

                        reader.Skip(4);
                    }
                    else
                    {
                        reader.Skip(3);
                    }
                }
                else
                {
                    reader.Skip(2);
                }
            }
            else
            {
                reader.Skip(1);
            }

            return result;
        }

        public static string ReadProtobufString(this ref SpanStream<byte> reader)
        {
            return Encoding.UTF8.GetString(reader.Read(reader.ReadProtobufVarInt()));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe uint PeekInt(ref SpanStream<byte> span, int numBits)
        {
            fixed (byte* spanPtr = span)
            {
                return (uint)((*(ulong*)(spanPtr + ((0 >> 3) & ~3)) << (8 * 8 - (0 & (8 * 4 - 1)) - numBits)) >> (8 * 8 - numBits));
            }
        }

        public unsafe static string ReadDataTableString(this ref SpanStream<byte> stream)
        {
            var len = stream.Span.IndexOf((byte)0);
            return Encoding.UTF8.GetString(stream.Read(len + 1).Slice(0, len));
        }
    }
}