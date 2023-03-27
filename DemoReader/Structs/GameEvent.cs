using System;
using System.Buffers;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace DemoReader
{
	public struct GameEvent
    {
        public string eventName;
        public int eventId;
        public Memory<byte> keys;

        IMemoryOwner<byte> owner;

        public static GameEvent Parse(SpanStream<byte> stream)
        {
            var gameEvent = new GameEvent();

            Span<byte> buff = stackalloc byte[1024 * 4]; // 4KB
            int idx = 0;

            while (!stream.IsEnd)
            {
                var desc = stream.ReadProtobufVarInt();
                var wireType = desc & 7;
                var fieldnum = desc >> 3;

                if (wireType == 2 && fieldnum == 1)
                {
                    gameEvent.eventName = stream.ReadProtobufString();
                }
                else if (wireType == 0 && fieldnum == 2)
                {
                    gameEvent.eventId = stream.ReadProtobufVarInt();
                }
                else if (wireType == 2 && fieldnum == 3)
                {
                    stream.ReadProtobufVarInt();
                    desc = stream.ReadProtobufVarInt();
                    wireType = desc & 7;
                    fieldnum = desc >> 3;
                    if (wireType != 0 || fieldnum != 1)
                    {
                        throw new InvalidDataException("Lord Gaben wasn't nice to us :/");
                    }

                    var typeMember = stream.ReadProtobufVarInt();
                    desc = stream.ReadProtobufVarInt();
                    wireType = desc & 7;
                    fieldnum = desc >> 3;

                    if (fieldnum != typeMember + 1)
                    {
                        throw new InvalidDataException("Lord Gaben wasn't nice to us :/ (srsly wtf!?)");
                    }

                    switch (typeMember)
                    {
                        case 1: // string
                            if (wireType != 2)
                            {
                                throw new InvalidDataException("proto definition differs");
                            }

                            int strLen = Encoding.UTF8.GetBytes(stream.ReadProtobufString(), buff.Slice(idx + 4));
                            Unsafe.WriteUnaligned(ref buff[idx], strLen);
                            idx += strLen + 4;
                            break;
                        case 2: // float
                            if (wireType != 5)
                            {
                                throw new InvalidDataException("proto definition differs");
                            }

                            Unsafe.WriteUnaligned(ref buff[idx], stream.ReadFloat());
                            idx += 4;
                            break;
                        case 3: // long
                        case 4: // short
                        case 5: // byte
                            if (wireType != 0)
                            {
                                throw new InvalidDataException("proto definition differs");
                            }

                            Unsafe.WriteUnaligned(ref buff[idx], stream.ReadProtobufVarInt());
                            idx += 4;
                            break;
                        case 6: // bool
                            if (wireType != 0)
                            {
                                throw new InvalidDataException("proto definition differs");
                            }

                            Unsafe.WriteUnaligned(ref buff[idx], stream.ReadProtobufVarInt() != 0);
                            idx += 1;
                            break;
                        default:
                            throw new InvalidDataException("Looks like they introduced a new type");
                    }
                }
            }

            if (idx > 0)
            {
                gameEvent.owner = MemoryPool<byte>.Shared.Rent(idx);
                gameEvent.keys = gameEvent.owner.Memory;
                buff.Slice(0, idx).CopyTo(gameEvent.owner.Memory.Span);
            }

            return gameEvent;
        }
    }
}