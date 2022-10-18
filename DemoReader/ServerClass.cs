namespace DemoReader
{
    public struct ServerClass
    {
        public short classID;
        public int dataTableID;
        public string name;
        public string dtName;

        public static ServerClass Parse(ref SpanStream<byte> stream, List<SendTable> dataTables)
        {
            var serverClass = new ServerClass();
            serverClass.classID = stream.ReadShort();
            serverClass.name = stream.ReadDataTableString();
            serverClass.dtName = stream.ReadDataTableString();
            serverClass.dataTableID = dataTables.FindIndex(a => a.netTableName == serverClass.dtName);

            return serverClass;
        }
    }
}