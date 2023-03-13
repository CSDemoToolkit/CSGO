namespace DemoReader
{
    public struct SendProperty
    {
        public SendPropertyType type;
        public string varName;
        public SendPropertyFlags flags;
        public int priority;
        public string dtName;
        public int numElements;
        public float lowValue;
        public float highValue;
        public int numBits;

		public int arrayElementProp;

        public static SendProperty Parse(SpanStream<byte> stream)
        {
            var prop = new SendProperty();

            while (!stream.IsEnd)
            {
                var desc = stream.ReadProtobufVarInt();
                var wireType = desc & 7;
                var fieldnum = desc >> 3;

                if (wireType == 2)
                {
                    if (fieldnum == 2)
                    {
                        prop.varName = stream.ReadProtobufString();
					}
                    else if (fieldnum == 5)
                    {
                        prop.dtName = stream.ReadProtobufString();
                    }
                    else
                    {
                        throw new InvalidDataException("yes I know we should drop this but we" +
                                                       "probably want to know that they added a new big field");
                    }
                }
                else if (wireType == 0)
                {
                    var val = stream.ReadProtobufVarInt();

                    switch (fieldnum)
                    {
                        case 1:
                            prop.type = (SendPropertyType)val;
                            break;
                        case 3:
                            prop.flags = (SendPropertyFlags)val;
                            break;
                        case 4:
                            prop.priority = val;
                            break;
                        case 6:
                            prop.numElements = val;
                            break;
                        case 9:
                            prop.numBits = val;
                            break;
                        default:
                            // silently drop
                            break;
                    }
                }
                else if (wireType == 5)
                {
                    var val = stream.ReadFloat();

                    switch (fieldnum)
                    {
                        case 7:
                            prop.lowValue = val;
                            break;
                        case 8:
                            prop.highValue = val;
                            break;
                        default:
                            // silently drop
                            break;
                    }
                }
                else
                {
                    throw new InvalidDataException();
                }
            }

            return prop;
        }
    }
}