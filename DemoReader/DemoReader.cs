﻿using System.Buffers;
using System.Data;
using System.IO.MemoryMappedFiles;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace DemoReader
{
	public class DemoReader
	{
		List<SendTable> DataTables = new List<SendTable>();
		List<StringTable> StringTables = new List<StringTable>();
		ServerClass[] ServerClasses;
		List<EventDescriptor> EventDescriptors = new List<EventDescriptor>();

		ArraySegment<byte>[] instanceBaselines = new ArraySegment<byte>[300]; // TODO: Replace 300
		string[] modelPrecaches = new string[847];

		PlayerInfo[] players = new PlayerInfo[64];
		Entity[] entities = new Entity[1024];

		int ServerClassesBits;

		public DemoPacketContainer container;
		public DemoEventHandler eventHandler;
		public DemoPacketHandler packetHandler;

		public DemoReader()
		{
			container = new DemoPacketContainer();
			eventHandler = new DemoEventHandler(container, players);
			packetHandler = new DemoPacketHandler(container, eventHandler);
		}

		public void Analyze(string path)
		{
			using var file = MemoryMappedFile.CreateFromFile(path);
			using var stream = file.CreateViewStream();

			ReadHeader(stream);

			int i = 0;
			while (Read(stream))
			{
				i++;
			}

			Console.WriteLine($"Ticks: {i}");
		}

		void ReadHeader(Stream stream)
		{
			var header = stream.Read<DemoHeader>();
		}

		bool Read(Stream stream)
		{
			var command = stream.ReadByte<DemoCommand>();
			stream.SkipBytes(5);

			if (command == DemoCommand.Stop)
				return false;

			switch (command)
			{
				case DemoCommand.ConsoleCommand:
					stream.SkipBytes(stream.Read<int>());
					return true;
				case DemoCommand.UserCommand:
					stream.SkipBytes(4);
					stream.SkipBytes(stream.Read<int>());
					return true;
				case DemoCommand.DataTables:
					return HandleDataTables(stream);
				case DemoCommand.StringTables:
					Span<byte> stringTablesBuff = stackalloc byte[stream.Read<int>()];
					stream.Read(stringTablesBuff);
					return true;
				case DemoCommand.Signon:
				case DemoCommand.Packet:
					return HandlePacket(stream);
				default:
					return true;
			}
		}

		bool HandleDataTables(Stream stream)
		{
			Span<byte> buff = stackalloc byte[stream.Read<int>()];
			stream.Read(buff);

			var spanStream = new SpanStream<byte>(buff);

			DataTables = GetDataTables(ref spanStream);
			ServerClasses = GetServerClasses(ref spanStream, DataTables);
			ServerClassesBits = BitOperations.Log2(BitOperations.RoundUpToPowerOf2((uint)ServerClasses.Length));

			packetHandler.Init(ServerClasses);

			return true;
		}

		bool HandlePacket(Stream stream)
		{
			var commandInfo = stream.Read<CommandInfo>();

			stream.SkipBytes(8);

			Span<byte> buff = stackalloc byte[stream.Read<int>()];
			stream.Read(buff);

			var spanStream = new SpanStream<byte>(buff);
			while (!spanStream.IsEnd)
			{
				SVCMessages cmd = (SVCMessages)spanStream.ReadProtobufVarInt();
				int size = spanStream.ReadProtobufVarInt();
				var cmdStream = spanStream.Slice(size);

				switch (cmd)
				{
					case SVCMessages.svc_PacketEntities:
						PacketEntities.Parse(cmdStream, ServerClasses, entities, instanceBaselines, ServerClassesBits, packetHandler);
						break;
					case SVCMessages.svc_CreateStringTable:
					{
						StringTable table = StringTable.Parse(ref cmdStream);
						var len = cmdStream.ReadProtobufVarInt();
						SpanBitStream bitStream = cmdStream.SliceToBitStream(len);

						StringTables.Add(table);
						HandleCreateStringTable(ref table, ref bitStream, players, instanceBaselines, modelPrecaches);
						break;
					}
					case SVCMessages.svc_UpdateStringTable:
					{
						UpdateStringTable updateTable = UpdateStringTable.Parse(ref cmdStream);
						var len = cmdStream.ReadProtobufVarInt();
						SpanBitStream bitStream = cmdStream.SliceToBitStream(len);

						StringTable table = StringTables[(int)updateTable.tableId];
						table.numEntries = updateTable.changedEntries;
						HandleCreateStringTable(ref table, ref bitStream, players, instanceBaselines, modelPrecaches);
						break;
					}
					case SVCMessages.svc_GameEventList:
						EventDescriptors = GetEventDescriptors(cmdStream);
						break;
					case SVCMessages.svc_GameEvent:
						var e = GameEvent.Parse(cmdStream);
						var keyStream = new SpanStream<byte>(e.keys.Span);
						int eventId = e.eventId;

						eventHandler.Update(ref e, ref CollectionsMarshal.AsSpan(EventDescriptors)[eventId]);
						break;
					default:
						break;
				}
			}

			return true;
		}

		static void HandleCreateStringTable(scoped ref StringTable table, scoped ref SpanBitStream stream, PlayerInfo[] players, ArraySegment<byte>[] instanceBaselines, string[] modelPrecaches)
		{
			if (stream.ReadBit())
			{
				throw new NotImplementedException("Encoded with dictionaries, unable to decode");
			}

			Span<byte> userdata = stackalloc byte[16384];

			var queue = new CircularBuffer<IMemoryOwner<byte>>(32);
			int entryBits = BitOperations.Log2(table.maxEntries);
			for (int i = 0; i < table.numEntries; i++)
			{
				userdata.Fill(0);

				int idx = i;
				if (!stream.ReadBit())
					idx = stream.ReadInt(entryBits);

				if (stream.ReadBit())
				{
					if (stream.ReadBit())
					{
						int index = stream.ReadBits(5);
						int bytesToCopy = stream.ReadBits(5);

						var memory = MemoryPool<byte>.Shared.Rent(1024 + bytesToCopy);
						queue[index].Memory.Span.Slice(0, bytesToCopy).TryCopyTo(memory.Memory.Span);

						int pre = stream.idx;
						int l = stream.ReadUntill(0, 10, memory.Memory.Span.Slice(bytesToCopy)); // 10 might not be needed?
						memory.Memory.Span.Slice(l).Fill(0);

						queue.PushBack(memory);
					}
					else
					{
						var memory = MemoryPool<byte>.Shared.Rent(1024);

						int pre = stream.idx;
						int l = stream.ReadUntill(0, 10, memory.Memory.Span); // 10 might not be needed?
						memory.Memory.Span.Slice(l).Fill(0);

						queue.PushBack(memory);
					}
				}
				else
				{
					queue.PushBack(MemoryPool<byte>.Shared.Rent(0));
				}

				if (!stream.ReadBit())
					continue;

				uint userDataLength = table.userDataSizeBits;
				if (!table.userDataFixedSize)
					userDataLength = stream.ReadUInt(14) * 8;

				stream.ReadBytes(userDataLength, userdata);

				if (table.name == "userinfo")
				{
					SpanBitStream userDataStream = new SpanBitStream(userdata);
					PlayerInfo playerInfo = PlayerInfo.Parse(ref userDataStream);

					players[idx] = playerInfo;
				}
				else if (table.name == "instancebaseline")
				{
					int classid = int.Parse(Encoding.UTF8.GetString(queue.Back().Memory.Span)); // TODO: My intuition tells me this can be optimized, but i am sick of this code RN
					instanceBaselines[classid] = new ArraySegment<byte>(userdata.Slice(0, (int)userDataLength / 8).ToArray());
				}
				else if (table.name == "modelprecache")
				{
					modelPrecaches[idx] = Encoding.UTF8.GetString(queue.Back().Memory.Span);
				}
			}
		}

		static void HandleUpdateStringTable()
		{

		}

		static List<SendTable> GetDataTables(ref SpanStream<byte> stream)
		{
			List<SendTable> dataTables = new List<SendTable>();
			while (true)
			{
				SVCMessages msg = (SVCMessages)stream.ReadProtobufVarInt();
				int size = stream.ReadProtobufVarInt();

				var sendTable = SendTable.Parse(stream.Slice(size));

				if (sendTable.isEnd)
					break;

				dataTables.Add(sendTable);
			}

			return dataTables;
		}

		static ServerClass[] GetServerClasses(ref SpanStream<byte> stream, List<SendTable> dataTables)
		{
			short serverClassCount = stream.ReadShort();
			ServerClass[] classes = new ServerClass[serverClassCount];

			for (int i = 0; i < serverClassCount; i++)
			{
				classes[i] = ServerClass.Parse(ref stream, classes.AsSpan().Slice(0, i), dataTables);
			}

			return classes;
		}

		static List<EventDescriptor> GetEventDescriptors(SpanStream<byte> stream)
		{
			List<EventDescriptor> descriptors = new List<EventDescriptor>();
			while (!stream.IsEnd)
			{
				var desc = stream.ReadProtobufVarInt();
				var wireType = desc & 7;
				var fieldnum = desc >> 3;
				if (wireType != 2 || fieldnum != 1)
				{
					throw new InvalidDataException();
				}

				var length = stream.ReadProtobufVarInt();
				descriptors.Add(EventDescriptor.Parse(stream.Slice(length)));
			}

			return descriptors.OrderBy(x => x.eventId).ToList();
		}
	}

	public static class SpanStreamExtensions
	{
		public unsafe static ref T ReadAs<T>(this ref SpanStream<byte> stream) where T : unmanaged
		{
			return ref MemoryMarshal.AsRef<T>(stream.Read(sizeof(T)));
		}

		public static short ReadShort(this ref SpanStream<byte> stream)
		{
			return MemoryMarshal.AsRef<short>(stream.Read(2));
		}

		public static int ReadInt(this ref SpanStream<byte> stream)
		{
			return MemoryMarshal.AsRef<int>(stream.Read(4));
		}

		public static float ReadFloat(this ref SpanStream<byte> stream)
		{
			return MemoryMarshal.AsRef<float>(stream.Read(4));
		}

		public static bool ReadBool(this ref SpanStream<byte> stream)
		{
			return MemoryMarshal.AsRef<bool>(stream.Read(1));
		}

		public static string ReadCustomString(this ref SpanStream<byte> stream)
		{
			return Encoding.UTF8.GetString(stream.Read(stream.ReadInt()));
		}

		public static SpanBitStream SliceToBitStream(this ref SpanStream<byte> stream, int length)
		{
			return new SpanBitStream(stream.Read(length));
		}
	}

	public static class SpanBitStreamExtensions
	{
		public static uint ReadUBitInt(this ref SpanBitStream bs)
		{
			uint ret = bs.ReadUInt(6);
			switch (ret & (16 | 32))
			{
				case 16:
					ret = (ret & 15) | (bs.ReadUInt(4) << 4);
					break;
				case 32:
					ret = (ret & 15) | (bs.ReadUInt(8) << 4);
					break;
				case 48:
					ret = (ret & 15) | (bs.ReadUInt(32 - 4) << 4);
					break;
			}

			return ret;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ReadBitInt(this ref SpanBitStream bs)
		{
			int ret = bs.ReadBits(6);
			switch (ret & (16 | 32))
			{
				case 16:
					ret = (ret & 15) | (bs.ReadBits(4) << 4);
					break;
				case 32:
					ret = (ret & 15) | (bs.ReadBits(8) << 4);
					break;
				case 48:
					ret = (ret & 15) | (bs.ReadBits(32 - 4) << 4);
					break;
			}

			return ret;
		}

		public static string ReadCustomString(this ref SpanBitStream bs, uint length)
		{
			Span<byte> bytes = stackalloc byte[(int)BitOperations.RoundUpToPowerOf2(length)];
			bs.ReadBytes(length, bytes);

			return Encoding.UTF8.GetString(bytes);
		}

		public static string ReadCStyleString(this ref SpanBitStream bs)
		{
			Span<byte> bytes = stackalloc byte[1024];
			bs.ReadUntill(0, bytes);

			return Encoding.UTF8.GetString(bytes);
		}

		public static uint ReadUnsignedVarInt(this ref SpanBitStream bs)
		{
			uint tmpByte = 0x80;
			uint result = 0;
			for (int count = 0; (tmpByte & 0x80) != 0; count++)
			{
				if (count > 5)
				{
					throw new InvalidDataException("VarInt32 out of range");
				}

				tmpByte = bs.ReadByte();
				result |= (tmpByte & 0x7F) << (7 * count);
			}

			return result;
		}

		public static int ReadSignedVarInt(this ref SpanBitStream bs)
		{
			uint result = bs.ReadUnsignedVarInt();
			return (int)((result >> 1) ^ -(result & 1));
		}

		public static bool HasFlagFast(this SendPropertyFlags flags, SendPropertyFlags check)
		{
			return (flags & check) == check;
		}
	}
}