namespace DemoReader
{
    public struct EventKey
    {
        public int type;
        public string name;

        public static EventKey Parse(SpanStream<byte> stream)
        {
            var key = new EventKey();

            while (!stream.IsEnd)
            {
                var desc = stream.ReadProtobufVarInt();
                var wireType = desc & 7;
                var fieldnum = desc >> 3;
                if (wireType == 0 && fieldnum == 1)
                {
                    key.type = stream.ReadProtobufVarInt();
                }
                else if (wireType == 2 && fieldnum == 2)
                {
                    key.name = stream.ReadProtobufString();
                }
                else
                {
                    throw new InvalidDataException();
                }
            }

            return key;
        }
    }
}