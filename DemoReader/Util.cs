namespace DemoReader.Util
{
	internal class BitOperations
	{
		public static int Log2(int v)
		{
			// Operation from https://stackoverflow.com/a/30643928
			int r = 0xFFFF - v >> 31 & 0x10;
			v >>= r;
			int shift = 0xFF - v >> 31 & 0x8;
			v >>= shift;
			r |= shift;
			shift = 0xF - v >> 31 & 0x4;
			v >>= shift;
			r |= shift;
			shift = 0x3 - v >> 31 & 0x2;
			v >>= shift;
			r |= shift;
			r |= (v >> 1);
			return r;
		}

		public static uint RoundUpToPowerOf2(uint i)
		{
			// Operation from http://www.java2s.com/example/java-utility-method/number-round/rounduptopoweroftwo-int-i-e0a29.html
			i--; // If input is a power of two, shift its high-order bit right.

			// "Smear" the high-order bit all the way to the right.
			i |= i >> 1;
			i |= i >> 2;
			i |= i >> 4;
			i |= i >> 8;
			i |= i >> 16;

			return i + 1;
		}
	}
}
