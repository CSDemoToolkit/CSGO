using System;
using System.Numerics;
using System.Text;

namespace DemoReader
{
    internal static class PropDecoder
    {
        public static object DecodeProp(in SendProperty property, ref SpanBitStream bitStream, in Span<SendProperty> properties)
        {
            switch (property.type)
            {
                case SendPropertyType.Int:
                    return DecodeInt(property, ref bitStream);
                case SendPropertyType.Int64:
                    return DecodeInt64(property, ref bitStream);
                case SendPropertyType.Float:
                    return DecodeFloat(property, ref bitStream);
                case SendPropertyType.Vector:
                    return DecodeVector(property, ref bitStream);
                case SendPropertyType.Array:
                    return DecodeArray(property, ref bitStream, properties);
                case SendPropertyType.String:
                    return DecodeString(property, ref bitStream);
                case SendPropertyType.VectorXY:
                    return DecodeVectorXY(property, ref bitStream);
                default:
                    throw new NotImplementedException("Could not read property. Abort! ABORT!");
            }
        }

        public static int DecodeInt(in SendProperty prop, ref SpanBitStream bitStream)
		{
			if (prop.flags.HasFlag(SendPropertyFlags.VarInt))
			{
				if (prop.flags.HasFlag(SendPropertyFlags.Unsigned))
				{
					return (int)bitStream.ReadUnsignedVarInt();
				}
				else
				{
					return bitStream.ReadSignedVarInt();
				}
			}
			else
			{
				if (prop.flags.HasFlag(SendPropertyFlags.Unsigned))
				{
					return (int)bitStream.ReadUInt(prop.numBits);
				}
				else
				{
					return bitStream.ReadInt(prop.numBits);
				}
			}
		}

		public static long DecodeInt64(in SendProperty prop, ref SpanBitStream bitStream)
        {
            if (prop.flags.HasFlag(SendPropertyFlags.VarInt))
            {
                if (prop.flags.HasFlag(SendPropertyFlags.Unsigned))
                {
                    return bitStream.ReadUnsignedVarInt();
                }
                else
                {
                    return bitStream.ReadSignedVarInt();
                }
            }
            else
            {
                bool isNegative = false;
                uint low = 0;
                uint high = 0;

                if (prop.flags.HasFlag(SendPropertyFlags.Unsigned))
                {
                    low = bitStream.ReadUInt(32);
                    high = bitStream.ReadUInt(prop.numBits - 32);
                }
                else
                {
                    isNegative = bitStream.ReadBit();
                    low = bitStream.ReadUInt(32);
                    high = bitStream.ReadUInt(prop.numBits - 32 - 1);
                }

                long result = ((long)high << 32) | low;

                if (isNegative)
                {
                    result = -result;
                }

                return result;
            }
        }

        public static float DecodeFloat(in SendProperty prop, ref SpanBitStream bitStream)
        {
            float fVal = 0.0f;
            ulong dwInterp;

            if (TryDecodeSpecialFloat(prop, ref bitStream, out fVal))
            {
                return fVal;
            }


            //Encoding: The range between lowVal and highVal is splitted into the same steps.
            //Read an int, fit it into the range. 
            dwInterp = bitStream.ReadUInt(prop.numBits);
            fVal = (float)dwInterp / ((1 << prop.numBits) - 1);
            fVal = prop.lowValue + (prop.highValue - prop.lowValue) * fVal;

            return fVal;
        }

        public static Vector3 DecodeVector(in SendProperty prop, ref SpanBitStream bitStream)
        {
            if (prop.flags.HasFlag(SendPropertyFlags.Normal))
            {
            }

			Vector3 v = new Vector3();

            v.X = DecodeFloat(prop, ref bitStream);
            v.Y = DecodeFloat(prop, ref bitStream);

            if (!prop.flags.HasFlag(SendPropertyFlags.Normal))
            {
                v.Z = DecodeFloat(prop, ref bitStream);
            }
            else
            {
                bool isNegative = bitStream.ReadBit();

                //v0v0v1v1 in original instead of margin. 
                float absolute = v.X * v.X + v.Y * v.Y;
                if (absolute < 1.0f)
                {
                    v.Z = (float)Math.Sqrt(1 - absolute);
                }
                else
                {
                    v.Z = 0f;
                }

                if (isNegative)
                {
                    v.Z *= -1;
                }
            }

            return v;
        }

        public static object[] DecodeArray(in SendProperty prop, ref SpanBitStream bitStream, in Span<SendProperty> properties)
        {
            int numElements = prop.numElements;
            int maxElements = numElements;

            int numBits = 1;

            while ((maxElements >>= 1) != 0)
            {
                numBits++;
            }

            int nElements = (int)bitStream.ReadUInt(numBits);

            object[] result = new object[nElements];

			ref SendProperty temp = ref properties[prop.arrayElementProp];
            for (int i = 0; i < nElements; i++)
            {
                result[i] = DecodeProp(temp, ref bitStream, properties);
            }

            return result;
        }

        public static string DecodeString(in SendProperty prop, ref SpanBitStream bitStream)
        {
			Span<byte> buff = stackalloc byte[1024];
			bitStream.ReadBytes((int)bitStream.ReadUInt(9), buff);

			return Encoding.Default.GetString(buff);
        }

        public static Vector2 DecodeVectorXY(in SendProperty prop, ref SpanBitStream bitStream)
        {
			Vector2 v = new Vector2();
            v.X = DecodeFloat(prop, ref bitStream);
            v.Y = DecodeFloat(prop, ref bitStream);

            return v;
        }

        #region Float-Stuff

        private static bool TryDecodeSpecialFloat(in SendProperty prop, ref SpanBitStream bitStream, out float result)
        {
            if (prop.flags.HasFlag(SendPropertyFlags.Coord))
            {
                result = ReadBitCoord(ref bitStream);
                return true;
            }
            else if (prop.flags.HasFlag(SendPropertyFlags.CoordMp))
            {
                result = ReadBitCoordMP(ref bitStream, false, false);
                return true;
            }
            else if (prop.flags.HasFlag(SendPropertyFlags.CoordMpLowPrecision))
            {
                result = ReadBitCoordMP(ref bitStream, false, true);
                return true;
            }
            else if (prop.flags.HasFlag(SendPropertyFlags.CoordMpIntegral))
            {
                result = ReadBitCoordMP(ref bitStream, true, false);
                return true;
            }
            else if (prop.flags.HasFlag(SendPropertyFlags.NoScale))
            {
                result = bitStream.ReadFloat(32);
                return true;
            }
            else if (prop.flags.HasFlag(SendPropertyFlags.Normal))
            {
                result = ReadBitNormal(ref bitStream);
                return true;
            }
            else if (prop.flags.HasFlag(SendPropertyFlags.CellCoord))
            {
                result = ReadBitCellCoord(ref bitStream, prop.numBits, false, false);
                return true;
            }
            else if (prop.flags.HasFlag(SendPropertyFlags.CellCoordLowPrecision))
            {
                result = ReadBitCellCoord(ref bitStream, prop.numBits, true, false);
                return true;
            }
            else if (prop.flags.HasFlag(SendPropertyFlags.CellCoordIntegral))
            {
                result = ReadBitCellCoord(ref bitStream, prop.numBits, false, true);
                return true;
            }

            result = 0;

            return false;
        }

        private static readonly int COORD_FRACTIONAL_BITS = 5;
        private static readonly int COORD_DENOMINATOR = 1 << COORD_FRACTIONAL_BITS;
        private static readonly float COORD_RESOLUTION = 1.0f / COORD_DENOMINATOR;

        private static readonly int COORD_FRACTIONAL_BITS_MP_LOWPRECISION = 3;
        private static readonly float COORD_DENOMINATOR_LOWPRECISION = 1 << COORD_FRACTIONAL_BITS_MP_LOWPRECISION;
        private static readonly float COORD_RESOLUTION_LOWPRECISION = 1.0f / COORD_DENOMINATOR_LOWPRECISION;

        private static float ReadBitCoord(ref SpanBitStream bitStream)
        {
            int intVal, fractVal;
            float value = 0;

            bool isNegative = false;

            // Read the required integer and fraction flags
            intVal = (int)bitStream.ReadUInt(1);
            fractVal = (int)bitStream.ReadUInt(1);

            // If we got either parse them, otherwise it's a zero.
            if ((intVal | fractVal) != 0)
            {
                // Read the sign bit
                isNegative = bitStream.ReadBit();

                // If there's an integer, read it in
                if (intVal == 1)
                {
                    // Adjust the integers from [0..MAX_COORD_VALUE-1] to [1..MAX_COORD_VALUE]
                    intVal = (int)bitStream.ReadUInt(14) + 1; //14 --> Coord int bits
                }

                //If there's a fraction, read it in
                if (fractVal == 1)
                {
                    fractVal = (int)bitStream.ReadUInt(COORD_FRACTIONAL_BITS);
                }

                value = intVal + (float)fractVal * COORD_RESOLUTION;
            }

            if (isNegative)
            {
                value *= -1;
            }

            return value;
        }

        private static float ReadBitCoordMP(ref SpanBitStream bitStream, bool isIntegral, bool isLowPrecision)
        {
            int intval = 0, fractval = 0;
            float value = 0.0f;
            bool isNegative = false;

            bool inBounds = bitStream.ReadBit();

            if (isIntegral)
            {
                // Read the required integer and fraction flags
                intval = bitStream.ReadBit() ? 1 : 0;

                // If we got either parse them, otherwise it's a zero.
                if (intval == 1)
                {
                    // Read the sign bit
                    isNegative = bitStream.ReadBit();

                    // If there's an integer, read it in
                    // Adjust the integers from [0..MAX_COORD_VALUE-1] to [1..MAX_COORD_VALUE]
                    if (inBounds)
                    {
                        value = (float)(bitStream.ReadUInt(11) + 1);
                    }
                    else
                    {
                        value = (float)(bitStream.ReadUInt(14) + 1);
                    }
                }
            }
            else
            {
                // Read the required integer and fraction flags
                intval = bitStream.ReadBit() ? 1 : 0;

                // Read the sign bit
                isNegative = bitStream.ReadBit();

                // If we got either parse them, otherwise it's a zero.
                if (intval == 1)
                {
                    // If there's an integer, read it in
                    // Adjust the integers from [0..MAX_COORD_VALUE-1] to [1..MAX_COORD_VALUE]
                    if (inBounds)
                    {
                        value = (float)(bitStream.ReadInt(11) + 1);
                    }
                    else
                    {
                        value = (float)(bitStream.ReadInt(14) + 1);
                    }
                }

                // If there's a fraction, read it in
                fractval = (int)bitStream.ReadInt(isLowPrecision ? 3 : 5);

                // Calculate the correct floating point value
                value = intval + (float)fractval * (isLowPrecision ? COORD_RESOLUTION_LOWPRECISION : COORD_RESOLUTION);
            }

            if (isNegative)
            {
                value = -value;
            }

            return value;
        }

        private static float ReadBitCellCoord(ref SpanBitStream bitStream, int bits, bool lowPrecision, bool integral)
        {
            int intval = 0, fractval = 0;
            float value = 0.0f;

            if (integral)
            {
                value = (float)bitStream.ReadUInt(bits);
            }
            else
            {
                intval = (int)bitStream.ReadUInt(bits);
                fractval = (int)bitStream.ReadUInt(lowPrecision ? COORD_FRACTIONAL_BITS_MP_LOWPRECISION : COORD_FRACTIONAL_BITS);


                value = intval + (float)fractval * (lowPrecision ? COORD_RESOLUTION_LOWPRECISION : COORD_RESOLUTION);
            }

            return value;
        }

        private static readonly int NORMAL_FRACTIONAL_BITS = 11;
        private static readonly int NORMAL_DENOMINATOR = (1 << NORMAL_FRACTIONAL_BITS) - 1;
        private static readonly float NORMAL_RESOLUTION = 1.0f / NORMAL_DENOMINATOR;

        private static float ReadBitNormal(ref SpanBitStream bitStream)
        {
            bool isNegative = bitStream.ReadBit();

            uint fractVal = bitStream.ReadUInt(NORMAL_FRACTIONAL_BITS);

            float value = (float)fractVal * NORMAL_RESOLUTION;

            if (isNegative)
            {
                value *= -1;
            }

            return value;
        }

        #endregion
    }
}
