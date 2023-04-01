﻿using System;
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
			if (prop.flags.HasFlagFast(SendPropertyFlags.VarInt))
			{
				if (prop.flags.HasFlagFast(SendPropertyFlags.Unsigned))
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
				if (prop.flags.HasFlagFast(SendPropertyFlags.Unsigned))
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
            if (prop.flags.HasFlagFast(SendPropertyFlags.VarInt))
            {
                if (prop.flags.HasFlagFast(SendPropertyFlags.Unsigned))
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

                if (prop.flags.HasFlagFast(SendPropertyFlags.Unsigned))
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
            if (TryDecodeSpecialFloat(prop, ref bitStream, out float fVal))
            {
                return fVal;
            }


			//Encoding: The range between lowVal and highVal is splitted into the same steps.
			//Read an int, fit it into the range. 
			ulong dwInterp = bitStream.ReadUInt(prop.numBits);
            fVal = (float)dwInterp / ((1 << prop.numBits) - 1);
            fVal = prop.lowValue + (prop.highValue - prop.lowValue) * fVal;

            return fVal;
        }

        public static Vector3 DecodeVector(in SendProperty prop, ref SpanBitStream bitStream)
        {
			Vector3 v = new Vector3();

            v.X = DecodeFloat(prop, ref bitStream);
            v.Y = DecodeFloat(prop, ref bitStream);

            if (!prop.flags.HasFlagFast(SendPropertyFlags.Normal))
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

            int nElements = bitStream.ReadInt(numBits);
            object[] result = new object[nElements];

			SendProperty temp = prop.arrayElementProp.ToSendProperty();
            for (int i = 0; i < nElements; i++)
            {
                result[i] = DecodeProp(temp, ref bitStream, properties);
			}

            return result;
        }

        public static string DecodeString(in SendProperty prop, ref SpanBitStream bitStream)
        {
			int bytes = (int)bitStream.ReadUInt(9);

			Span<byte> buff = stackalloc byte[bytes];
			bitStream.ReadBytes(bytes * 8, buff);

			//Console.WriteLine($"\n			{bytes} - {buff.Length} - {Encoding.Default.GetString(buff)}");
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
			if (prop.flags.HasFlagFast(SendPropertyFlags.NoScale))
			{
				result = bitStream.ReadFloat();
				return true;
			}
			else if (prop.flags.HasFlagFast(SendPropertyFlags.CellCoord))
			{
				result = ReadBitCellCoordHighPrecision(ref bitStream, prop.numBits);
				return true;
			}
			else if (prop.flags.HasFlagFast(SendPropertyFlags.CellCoordLowPrecision))
			{
				result = ReadBitCellCoordLowPrecision(ref bitStream, prop.numBits);
				return true;
			}
			else if (prop.flags.HasFlagFast(SendPropertyFlags.Coord))
            {
                result = ReadBitCoord(ref bitStream);
                return true;
            }
            else if (prop.flags.HasFlagFast(SendPropertyFlags.CoordMp))
            {
                result = ReadBitCoordMP(ref bitStream, false, false);
                return true;
            }
            else if (prop.flags.HasFlagFast(SendPropertyFlags.CoordMpLowPrecision))
            {
                result = ReadBitCoordMP(ref bitStream, false, true);
                return true;
            }
            else if (prop.flags.HasFlagFast(SendPropertyFlags.CoordMpIntegral))
            {
                result = ReadBitCoordMP(ref bitStream, true, false);
                return true;
            }
            else if (prop.flags.HasFlagFast(SendPropertyFlags.Normal))
            {
                result = ReadBitNormal(ref bitStream);
                return true;
            }
            else if (prop.flags.HasFlagFast(SendPropertyFlags.CellCoordIntegral))
            {
                result = bitStream.ReadUInt(prop.numBits);
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
			// Read the required integer and fraction flags
			bool intVal = bitStream.ReadBit();
			bool fractVal = bitStream.ReadBit();

			// If we got either parse them, otherwise it's a zero.
			if (!intVal && !fractVal)
				return 0;

			// Read the sign bit
			bool isNegative = bitStream.ReadBit();
			float value = 0;

			// If there's an integer, read it in
			// Adjust the integers from [0..MAX_COORD_VALUE-1] to [1..MAX_COORD_VALUE]
			if (intVal)
				value = bitStream.ReadInt(14) + 1; //14 --> Coord int bits

			//If there's a fraction, read it in
			if (fractVal)
				value += bitStream.ReadInt(COORD_FRACTIONAL_BITS) * COORD_RESOLUTION;

			if (isNegative)
			{
				value *= -1;
			}

            return value;
        }

        private unsafe static float ReadBitCoordMP(ref SpanBitStream bitStream, bool isIntegral, bool isLowPrecision)
        {
			bool intval;
			int fractval = 0;
            float value = 0.0f;

            bool inBounds = bitStream.ReadBit();

            if (isIntegral)
            {
                // Read the required integer and fraction flags
                intval = bitStream.ReadBit();

                // If we got either parse them, otherwise it's a zero.
                if (intval)
                {
					// Read the sign bit
					bool isNegative = bitStream.ReadBit();

                    // If there's an integer, read it in
                    // Adjust the integers from [0..MAX_COORD_VALUE-1] to [1..MAX_COORD_VALUE]
                    if (inBounds)
                    {
                        value = bitStream.ReadUInt(11) + 1;
                    }
                    else
                    {
                        value = bitStream.ReadUInt(14) + 1;
                    }

					if (isNegative)
					{
						value = -value;
					}
				}
            }
            else
            {
                // Read the required integer and fraction flags
                intval = bitStream.ReadBit();

				// Read the sign bit
				bool isNegative = bitStream.ReadBit();

                // If we got either parse them, otherwise it's a zero.
                if (intval)
                {
                    // If there's an integer, read it in
                    // Adjust the integers from [0..MAX_COORD_VALUE-1] to [1..MAX_COORD_VALUE]
                    if (inBounds)
                    {
                        value = bitStream.ReadInt(11) + 1;
                    }
                    else
                    {
                        value = bitStream.ReadInt(14) + 1;
                    }
                }

                // If there's a fraction, read it in
                fractval = bitStream.ReadInt(isLowPrecision ? 3 : 5);

                // Calculate the correct floating point value
                value = *((byte*)(&intval)) + fractval * (isLowPrecision ? COORD_RESOLUTION_LOWPRECISION : COORD_RESOLUTION);

				if (isNegative)
				{
					value = -value;
				}
			}

            return value;
        }

        private static float ReadBitCellCoord(ref SpanBitStream bitStream, int bits, bool lowPrecision)
        {
			int intval = bitStream.ReadInt(bits);
			int fractval = bitStream.ReadInt(lowPrecision ? COORD_FRACTIONAL_BITS_MP_LOWPRECISION : COORD_FRACTIONAL_BITS);
			return intval + fractval * (lowPrecision ? COORD_RESOLUTION_LOWPRECISION : COORD_RESOLUTION);
        }

		private static float ReadBitCellCoordLowPrecision(ref SpanBitStream bitStream, int bits)
		{
			return bitStream.ReadInt(bits) + bitStream.ReadInt(COORD_FRACTIONAL_BITS_MP_LOWPRECISION) * COORD_RESOLUTION_LOWPRECISION;
		}

		private static float ReadBitCellCoordHighPrecision(ref SpanBitStream bitStream, int bits)
		{
			return bitStream.ReadInt(bits) + bitStream.ReadInt(COORD_FRACTIONAL_BITS) * COORD_RESOLUTION;
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
