namespace DemoReader
{
    public struct SendTable
    {
        public List<SendPropperty> properties;
        public string netTableName;
        public bool isEnd;
        public bool needsDecoder;

        public static SendTable Parse(SpanStream<byte> stream)
        {
            var sendTable = new SendTable();
            sendTable.properties = new List<SendPropperty>();

            while (!stream.IsEnd)
            {
                var desc = stream.ReadProtobufVarInt();
                var wireType = desc & 7;
                var fieldnum = desc >> 3;

                if (wireType == 2)
                {
                    if (fieldnum == 2)
                    {
                        sendTable.netTableName = stream.ReadProtobufString();
                    }
                    else if (fieldnum == 4)
                    {
                        int size = stream.ReadProtobufVarInt();
                        sendTable.properties.Add(SendPropperty.Parse(stream.Slice(size)));
                    }
                    else
                    {
                        throw new InvalidDataException("yes I know we should drop this but we probably want to know that they added a new big field");
                    }
                }
                else if (wireType == 0)
                {
                    var val = stream.ReadProtobufVarInt() != 0;

                    switch (fieldnum)
                    {
                        case 1:
                            sendTable.isEnd = val;
                            break;
                        case 3:
                            sendTable.needsDecoder = val;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    throw new InvalidDataException();
                }
            }

            return sendTable;
        }
    }
}