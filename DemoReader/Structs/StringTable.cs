using System.IO;

namespace DemoReader
{
	public struct StringTable
    {
        public string name;
        public uint maxEntries;
        public uint numEntries;
        public bool userDataFixedSize;
        public uint userDataSize;
        public uint userDataSizeBits;
        public uint flags;

        public static StringTable Parse(ref SpanStream<byte> stream)
        {
            StringTable table = new StringTable();
            while (!stream.IsEnd)
            {
                var desc = stream.ReadProtobufVarInt();
                var wireType = desc & 7;
                var fieldnum = desc >> 3;

                if (wireType == 2)
                {
                    if (fieldnum == 1)
                    {
                        table.name = stream.ReadProtobufString();
                        continue;
                    }
                    else if (fieldnum == 8)
                    {
                        // Assume data is end of string table.
                        return table;
                    }
                }

                if (wireType != 0)
                {
                    throw new InvalidDataException();
                }

                var val = stream.ReadProtobufVarUInt();

                switch (fieldnum)
                {
                    case 2:
                        table.maxEntries = val;
                        break;
                    case 3:
                        table.numEntries = val;
                        break;
                    case 4:
                        table.userDataFixedSize = val != 0;
                        break;
                    case 5:
                        table.userDataSize = val;
                        break;
                    case 6:
                        table.userDataSizeBits = val;
                        break;
                    case 7:
                        table.flags = val;
                        break;
                    default:
                        // silently drop
                        break;
                }
            }

            return table;
        }
    }
}