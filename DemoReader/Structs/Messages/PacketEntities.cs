using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DemoReader
{
	public unsafe struct Entity
	{
		public int id;
		public int serverClassID;

        public Entity(int id, in ServerClass serverClass)
        {
            this.id = id;
			this.serverClassID = serverClass.classID;
        }
    }

    public unsafe struct PacketEntities
    {
        public int maxEntries;
        public int updatedEntries;
        public bool isDelta;
        public bool updateBaseline;
        public int baseline;
        public int deltaFrom;

        public static PacketEntities Parse(SpanStream<byte> stream, Span<ServerClass> serverClasses, Span<Entity> entities, ArraySegment<byte>[] instanceBaselines, int serverClassesBits, DemoPacketHandler eventHandler)
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
						currentEntity += 1 + bitStream.ReadBitInt();

                        bool destroy = bitStream.ReadBit();
						if (destroy)
						{
							bitStream.Skip(1);

							ref Entity toDestroy = ref entities[currentEntity];
							ref ServerClass toDestroyServerClass = ref serverClasses[toDestroy.serverClassID];

							eventHandler.Destroy(ref toDestroyServerClass, ref toDestroy);

							continue;
						}

                        bool shouldCreate = bitStream.ReadBit();
                        if(shouldCreate) // If true create, if false just update
                        {
                            int serverClassID = bitStream.ReadInt(serverClassesBits);
							bitStream.Skip(10); //Entity serial (Int).

							ref ServerClass serverClass = ref serverClasses[serverClassID];
							entities[currentEntity] = new Entity(currentEntity, serverClass);

							// Apply update with instance baseline to entity
							var baselineBitStream = new SpanBitStream(instanceBaselines[serverClassID].AsSpan());
							ApplyUpdate(ref serverClass, ref entities[currentEntity], ref baselineBitStream, serverClass.properties, eventHandler);
						}

						ref Entity ent = ref entities[currentEntity];
						ref ServerClass entServerClass = ref serverClasses[ent.serverClassID];
						//Console.WriteLine($"EntityID: {ent.id}, ServerClass: {ent.serverClassID}");
						ApplyUpdate(ref entServerClass, ref ent, ref bitStream, serverClasses[ent.serverClassID].properties, eventHandler);
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

		static bool TryReadFieldIndex(ref SpanBitStream bitStream, out int ret)
		{
			if (bitStream.ReadBit())
			{
				ret = 0;
				return true;
			}

			if (bitStream.ReadBit())
			{
				ret = bitStream.ReadBits(3); // read 3 bits
			}
			else
			{
				ret = bitStream.ReadBits(7); // read 7 bits
				switch (ret & (32 | 64))
				{
					case 32:
						ret = (ret & ~96) | bitStream.ReadBits(2) << 5;
						break;
					case 64:
						ret = (ret & ~96) | bitStream.ReadBits(4) << 5;
						break;
					case 96:
						ret = (ret & ~96) | bitStream.ReadBits(7) << 5;
						break;
				}
			}

			// end marker is 4095 for cs:go
			return ret != 0xFFF;
		}

		static unsafe int ReadFieldIndexNew(ref SpanBitStream bitStream, short s)
		{
			if ((s & 2) == 2)
			{
				bitStream.Skip(-11);
				return (s & 28) >> 2; // read 3 bits
			}
			else
			{
				int ret = (s & 508) >> 2; // read 7 bits
				switch (ret & (32 | 64))
				{
					case 32:
						ret = (ret & ~96) | ((s & 1536) >> 9) << 5; // Read 2 bits
						bitStream.Skip(-5);
						break;
					case 64:
						ret = (ret & ~96) | ((s & 7680) >> 9) << 5; // Read 4 bits
						bitStream.Skip(-3);
						break;
					case 96:
						ret = (ret & ~96) | ((s & 65024) >> 9) << 5; // Read 7 bits
						break;
					default:
						bitStream.Skip(-7);
						break;
				}

				return ret;
			}
		}

		static SendProperty[] entries = new SendProperty[4096];
		static readonly int SEND_PROPERTY_SIZE = sizeof(SendProperty);

		unsafe static void ApplyUpdate(ref ServerClass serverClass, ref Entity entity, ref SpanBitStream bitStream, in Span<SendProperty> properties, DemoPacketHandler eventHandler)
		{
			// Apply
			bitStream.ReadBit(); // This bit was called new way. It affected TryReadFieldIndex
								 // Seems like it is always new way so we dont support, fix later if issue
			
			int index = -1;
			int idx = 0;

			// Use pointers to avoid bound checking
			// Use entries as buff array to get better cahe hit rate
			fixed (SendProperty* propertiesPtr = properties)
			fixed (SendProperty* entriesPtr = entries)
			//fixed (byte* lookupPtr = lookup)
			//fixed (int* maskLookupPtr = maskLookup)
			{
				//Console.WriteLine($"New: {bitStream.idx + 32}");
				//bool doPrint = bitStream.idx == 3448;
				/*
				while (TryReadFieldIndex(ref bitStream, out int ret))
				{
					index += ret + 1;
					entriesPtr[idx++] = propertiesPtr[index];
				}
				//Console.WriteLine($"Total: {index}, {idx}, {bitStream.idx}, {entriesPtr[idx - 1].type}");
				//bool doRun = true;
				*/
				int ret;
				short s;
				while (true)
				{
					int ones = bitStream.CountOnes();

					NativeMemory.Copy(propertiesPtr + index + 1, entriesPtr + idx, (nuint)(ones * SEND_PROPERTY_SIZE));
					index += ones;
					idx += ones;

					//Console.WriteLine($"Ones: {ones} - {idx} - {index}");

					bitStream.Skip(ones);
					bitStream.Read2Bytes(&s);

					if (s == -4)
						break;

					ret = ReadFieldIndexNew(ref bitStream, s);

					index += ret + 1;
					NativeMemory.Copy(propertiesPtr + index, entriesPtr + idx++, (nuint)(SEND_PROPERTY_SIZE));
				}
				/*
				*/
				//Console.WriteLine($"	Total: {index}, {idx}, {bitStream.idx + 32}, {entriesPtr[idx - 1].type}");
				//Console.WriteLine($"Total: {index}, {idx}, {entriesPtr[idx - 1].type}");

				//Now read the updated props
				for (int i = 0; i < idx; i++)
				{
					DecodeProp(ref serverClass, ref entity, ref bitStream, ref entriesPtr[i], properties, eventHandler);
				}
			}
		}

		static void DecodeProp(ref ServerClass serverClass, ref Entity entity, ref SpanBitStream bitStream, ref SendProperty property, in Span<SendProperty> properties, DemoPacketHandler eventHandler)
        {
			switch (property.type)
			{
				case SendPropertyType.Int:
				{
					var v = PropDecoder.DecodeInt(ref property, ref bitStream);
					eventHandler.Execute(ref serverClass, ref entity, ref property, v);
					break;
				}
				case SendPropertyType.Float:
				{
					var v = PropDecoder.DecodeFloat(ref property, ref bitStream);
					eventHandler.Execute(ref serverClass, ref entity, ref property, v);
					break;
				}	
				case SendPropertyType.Vector:
				{
					var v = PropDecoder.DecodeVector(ref property, ref bitStream);
					eventHandler.Execute(ref serverClass, ref entity, ref property, v);
					break;
				}
				case SendPropertyType.VectorXY:
				{
					var v = PropDecoder.DecodeVectorXY(ref property, ref bitStream);
					eventHandler.Execute(ref serverClass, ref entity, ref property, v);
					break;
				}
				case SendPropertyType.String:
				{
					var v = PropDecoder.DecodeString(ref property, ref bitStream);
					eventHandler.Execute(ref serverClass, ref entity, ref property, v);
					break;
				}
				case SendPropertyType.Array:
					var v5 = PropDecoder.DecodeArray(ref property, ref bitStream, properties);
					break;
				case SendPropertyType.Int64:
					var v6 = PropDecoder.DecodeInt64(ref property, ref bitStream);
					break;
				default:
					break;
			}
		}
	}
}