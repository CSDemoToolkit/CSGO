namespace DemoReader
{
    public struct EventDescriptor
    {
        public int eventId;
        public string name;
        public List<EventKey> keys;

        public static EventDescriptor Parse(SpanStream<byte> stream)
        {
            var descriptor = new EventDescriptor();
            descriptor.keys = new List<EventKey>();

            while (!stream.IsEnd)
            {
                var desc = stream.ReadProtobufVarInt();
                var wireType = desc & 7;
                var fieldnum = desc >> 3;
                if (wireType == 0 && fieldnum == 1)
                {
                    descriptor.eventId = stream.ReadProtobufVarInt();
                }
                else if (wireType == 2 && fieldnum == 2)
                {
                    descriptor.name = stream.ReadProtobufString();
                }
                else if (wireType == 2 && fieldnum == 3)
                {
                    var length = stream.ReadProtobufVarInt();
                    descriptor.keys.Add(EventKey.Parse(stream.Slice(length)));
                }
                else
                {
                    throw new InvalidDataException();
                }
            }

            return descriptor;
        }
    }
}