using System.Buffers;
using System.Numerics;

namespace DemoReader
{
    public struct Entity
    {
        public int id;
		public int serverClassID;

        public Entity(int id, in ServerClass serverClass)
        {
            this.id = id;
			this.serverClassID = serverClass.classID;
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

        public static PacketEntities Parse(SpanStream<byte> stream, Span<ServerClass> serverClasses, Span<Entity> entities, ArraySegment<byte>[] instanceBaselines, int serverClassesBits)
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
						//Console.WriteLine($"Stream pos: {bitStream.idx + 912}");
						currentEntity += 1 + (int)bitStream.ReadUBitInt();

                        bool destroy = bitStream.ReadBit();
						if (destroy)
						{
							//Console.WriteLine("Destroy");
							bitStream.ReadBit();
							continue;
						}

                        bool shouldCreate = bitStream.ReadBit();
                        if(shouldCreate) // If true create, if false just update
                        {
                            int serverClassID = bitStream.ReadInt(serverClassesBits);
                            bitStream.ReadInt(10); //Entity serial. 

                            //Console.WriteLine(serverClassID);
							//Console.WriteLine($"Create EntityID: {currentEntity}, ServerClass: {serverClassID}");
                            ref ServerClass serverClass = ref serverClasses[serverClassID];
							entities[currentEntity] = new Entity(currentEntity, serverClass);

							// Apply update with instance baseline to entity
							var baselineBitStream = new SpanBitStream(instanceBaselines[serverClassID].AsSpan());
							//Console.WriteLine($"Baseline length: {instanceBaselines[serverClassID].Count}");
							ApplyUpdate(ref entities[currentEntity], ref baselineBitStream, serverClass.properties);
                        }

						ref Entity ent = ref entities[currentEntity];
						//Console.WriteLine($"Update EntityID: {ent.id}, ServerClass: {ent.serverClassID}");
						ApplyUpdate(ref ent, ref bitStream, serverClasses[ent.serverClassID].properties);
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

        static int ReadFieldIndex(ref SpanBitStream bitStream, int lastIndex, bool bNewWay)
        {
            if (bNewWay)
            {
                if (bitStream.ReadBit())
                {
					//Console.WriteLine("		1");
					return lastIndex + 1;
                }
            }

			int ret;
			if (bNewWay && bitStream.ReadBit())
            {
                ret = (int)bitStream.ReadUInt(3); // read 3 bits
				//Console.WriteLine($"		2 - {ret}");
			}
            else
            {
				//Console.WriteLine("		3");
				ret = (int)bitStream.ReadUInt(7); // read 7 bits
                switch (ret & (32 | 64))
                {
                    case 32:
                        ret = (ret & ~96) | ((int)bitStream.ReadUInt(2) << 5);
                        break;
                    case 64:
                        ret = (ret & ~96) | ((int)bitStream.ReadUInt(4) << 5);
                        break;
                    case 96:
                        ret = (ret & ~96) | ((int)bitStream.ReadUInt(7) << 5);
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

		static void ApplyUpdate(ref Entity entity, ref SpanBitStream bitStream, in Span<SendProperty> properties)
		{
			// Apply
			bool newWay = bitStream.ReadBit();
			int index = -1;

			int idx = 0;
			SendProperty[] entries = ArrayPool<SendProperty>.Shared.Rent(4096);
			//var entries = new List<SendProperty>();

			//No read them. 
			//Console.WriteLine($"Reading field index - {bitStream.idx + 32}");
			while ((index = ReadFieldIndex(ref bitStream, index, newWay)) != -1)
			{
				//Console.WriteLine($"        {index} -  {properties[index].priority} - {properties[index].varName}");
				entries[idx++] = properties[index];
			}

			//Console.WriteLine($"	{idx}");

			//Now read the updated props
			int i = 0;
			foreach (ref readonly var prop in entries.AsSpan().Slice(0, idx))
			{
				//Console.WriteLine($"	{prop.type}");
				//Console.Write($"        {i} -  {prop.type} - {prop.varName} - {bitStream.idx + 32}");
				DecodeProp(ref entity, ref bitStream, prop, properties);
				i++;
			}

			ArrayPool<SendProperty>.Shared.Return(entries);
		}

		static int mask = 0;
		static int bits = 0;
		static int old = 0;

		static void DecodeProp(ref Entity entity, ref SpanBitStream bitStream, in SendProperty property, in Span<SendProperty> properties)
        {
			switch (property.type)
			{
				case SendPropertyType.Int:
					if ((mask & (1 << 0)) == 0)
					{
						bits = bitStream.idx;
						old = bits;
						//Console.WriteLine($"Int: {bits}");
					}

					var v = PropDecoder.DecodeInt(property, ref bitStream);

					if ((mask & (1 << 0)) == 0)
					{
						bits = bitStream.idx;
						//Console.WriteLine($"Int: {bits} - {bits - old}");

						mask |= 1 << 0;
					}
					//Console.WriteLine($" - {v}");
					break;
				case SendPropertyType.Float:
					if ((mask & (1 << 1)) == 0)
					{
						bits = bitStream.idx;
						old = bits;
						//Console.WriteLine($"Float: {bits}");
					}
					var v1 = PropDecoder.DecodeFloat(property, ref bitStream);
					if ((mask & (1 << 1)) == 0)
					{
						bits = bitStream.idx;
						//Console.WriteLine($"Float: {bits} - {bits - old}");

						mask |= 1 << 1;
					}
					//Console.WriteLine($" - {v1}");
					break;
				case SendPropertyType.Vector:
					if ((mask & (1 << 2)) == 0)
					{
						bits = bitStream.idx;
						old = bits;
						//Console.WriteLine($"Vector: {bits}");
					}
					var v2 = PropDecoder.DecodeVector(property, ref bitStream);
					if ((mask & (1 << 2)) == 0)
					{
						bits = bitStream.idx;
						//Console.WriteLine($"Vector: {bits} - {bits - old}");

						mask |= 1 << 2;
					}
					//Console.WriteLine($" - {v2}");
					break;
				case SendPropertyType.VectorXY:
					var v3 = PropDecoder.DecodeVectorXY(property, ref bitStream);
					//Console.WriteLine($" - {v3}");
					break;
				case SendPropertyType.String:
					var v4 = PropDecoder.DecodeString(property, ref bitStream);
					//Console.WriteLine($" - {v4.Length}");
					break;
				case SendPropertyType.Array:
					var v5 = PropDecoder.DecodeArray(property, ref bitStream, properties);
					//Console.WriteLine($" - {v5.Length}");
					break;
				case SendPropertyType.Int64:
					var v6 = PropDecoder.DecodeInt64(property, ref bitStream);
					//Console.WriteLine($" - {v6}");
					break;
				default:
					break;
			}
		}
	}
}