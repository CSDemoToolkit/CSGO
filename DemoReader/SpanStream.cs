using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace DemoReader
{
    public ref struct SpanStream<T> where T : unmanaged
    {
        public int Current => idx;

        public ReadOnlySpan<T> Span => buff.Slice(idx);

        public int Length => buff.Length;

        public bool IsEnd => idx >= buff.Length;

        Span<T> buff;
        int idx = 0;

        public SpanStream(Span<T> buff)
        {
            this.buff = buff;
        }

        public T Read()
        {
            return buff[idx++]; 
        }

        public Span<T> Read(int length)
        {
            idx += length;
            return buff.Slice(idx - length, length);
        }

        public void Skip(int length)
        {
            idx += length;
        }

        public SpanStream<T> Slice(int length)
        {
            idx += length;
            return new SpanStream<T>(buff.Slice(idx - length, length));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ref T GetPinnableReference()
        {
            return ref buff.Slice(idx).GetPinnableReference();
        }
    }
}