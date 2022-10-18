using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DemoReader
{
    static class StreamExtensions
    {
        public unsafe static T Read<T>(this Stream stream) where T : unmanaged
        {
            Span<byte> buff = stackalloc byte[sizeof(T)];
            stream.Read(buff);

            return MemoryMarshal.Read<T>(buff);
        }

        public unsafe static T ReadByte<T>(this Stream stream) where T : unmanaged
        {
            Span<byte> buff = stackalloc byte[1];
            stream.Read(buff);

            return Unsafe.As<byte, T>(ref buff[0]);
        }

        public static void SkipBytes(this Stream stream, int bytes)
        {
            stream.Seek(bytes, SeekOrigin.Current);
        }
    }
}