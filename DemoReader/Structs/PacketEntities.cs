using System.Numerics;

namespace DemoReader
{
    public struct Entity
    {
        public int id;

        public Entity(int id, in ServerClass serverClass)
        {
            this.id = id;
        }
    }

    public struct PacketEntities
    {
        public int maxEntries;
        public int updatedEntries;
        public bool isDelta;
        public bool updateBaseline;
        public int baseline;
        public int deltaFrom;

        public static PacketEntities Parse(SpanStream<byte> stream, Span<ServerClass> serverClasses, int serverClassesBits)
        {
            PacketEntities packetEntities = new PacketEntities();

            while (!stream.IsEnd)
            {
                var desc = stream.ReadProtobufVarInt();
                var wireType = desc & 7;
                var fieldnum = desc >> 3;

                if (fieldnum == 7 && wireType == 2)
                {
                    // Entity data is special.
                    // We'll simply hope that gaben is nice and sends
                    // entity_data last, just like he should.

                    var len = stream.ReadProtobufVarInt();
                    var bitStream = stream.SliceToBitStream(len);

                    int currentEntity = -1;
                    for (int i = 0; i < packetEntities.updatedEntries; i++)
                    {
                        currentEntity += 1 + (int)bitStream.ReadUBitInt();

                        bool destroy = bitStream.ReadBit();
                        bool shouldCreate = bitStream.ReadBit();

                        if (destroy)
                            continue;

                        if(shouldCreate) // If true create, if false just update
                        {
                            int serverClassID = bitStream.ReadInt(serverClassesBits);
                            bitStream.ReadInt(10); //Entity serial. 

                            //Console.WriteLine(serverClassID);
                            ref ServerClass serverClass = ref serverClasses[serverClassID];
                            Entity entity = new Entity(currentEntity, serverClass);

                            // Apply update with instance baseline to entity
                        }
                        else
                        {
                            //Console.WriteLine($"{currentEntity} - Update");
                        }

                        // Apply
                        bool newWay = bitStream.ReadBit();
                        int index = -1;
                        var entries = new List<int>();

                        //No read them. 
                        while ((index = ReadFieldIndex(ref bitStream, index, newWay)) != -1)
                        {
                            entries.Add(index);
                        }

                        //Now read the updated props
                        foreach (var prop in entries)
                        {
                            DecodeProp(ref bitStream);
                        }
                    }

                    break;
                }

                if (wireType != 0)
                {
                    throw new InvalidDataException();
                }

                var val = stream.ReadProtobufVarInt();

                switch (fieldnum)
                {
                    case 1:
                        packetEntities.maxEntries = val;
                        break;
                    case 2:
                        packetEntities.updatedEntries = val;
                        break;
                    case 3:
                        packetEntities.isDelta = val != 0;
                        break;
                    case 4:
                        packetEntities.updateBaseline = val != 0;
                        break;
                    case 5:
                        packetEntities.baseline = val;
                        break;
                    case 6:
                        packetEntities.deltaFrom = val;
                        break;
                    default:
                        // silently drop
                        break;
                }
            }

            return packetEntities;
        }

        static int ReadFieldIndex(ref SpanBitStream stream, int lastIndex, bool bNewWay)
        {
            if (bNewWay)
            {
                if (stream.ReadBit())
                {
                    return lastIndex + 1;
                }
            }

            int ret = 0;
            if (bNewWay && stream.ReadBit())
            {
                ret = (int)stream.ReadInt(3); // read 3 bits
            }
            else
            {
                ret = (int)stream.ReadInt(7); // read 7 bits
                switch (ret & (32 | 64))
                {
                    case 32:
                        ret = (ret & ~96) | ((int)stream.ReadInt(2) << 5);
                        break;
                    case 64:
                        ret = (ret & ~96) | ((int)stream.ReadInt(4) << 5);
                        break;
                    case 96:
                        ret = (ret & ~96) | ((int)stream.ReadInt(7) << 5);
                        break;
                }
            }

            if (ret == 0xFFF)
            {
                // end marker is 4095 for cs:go
                return -1;
            }

            return lastIndex + 1 + ret;
        }

        static void DecodeProp(ref SpanBitStream stream)
        {

        }
    }
}