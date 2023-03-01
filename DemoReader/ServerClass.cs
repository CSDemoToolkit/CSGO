using System.Data;

namespace DemoReader
{
    public struct ServerClass
    {
        public short classID;
        public int dataTableID;
        public string name;
        public string dtName;
		public List<SendPropperty> properties;

		public static ServerClass Parse(ref SpanStream<byte> stream, List<SendTable> dataTables)
        {
            var serverClass = new ServerClass();
            serverClass.classID = stream.ReadShort();
            serverClass.name = stream.ReadDataTableString();
            serverClass.dtName = stream.ReadDataTableString();
            serverClass.dataTableID = dataTables.FindIndex(a => a.netTableName == serverClass.dtName);
			serverClass.properties = new List<SendPropperty>();

			GetProperties(dataTables[serverClass.dataTableID], dataTables, serverClass.properties);
			serverClass.properties.Sort((x, y) => x.priority.CompareTo(y.priority));

			return serverClass;
        }

		static void GetProperties(SendTable table, List<SendTable> dataTables, List<SendPropperty> properties)
		{
			foreach (var prop in table.properties)
			{
				// Ignore prop if element of array of excluded
				if (prop.flags.HasFlag(SendPropertyFlags.Exclude | SendPropertyFlags.InsideArray))
					continue;

				if (prop.dtName == "baseclass")
					continue;

				if (prop.type == SendPropertyType.DataTable)
				{
					GetProperties(dataTables.Find(x => x.netTableName == prop.dtName), dataTables, properties); // Instead of returning ref to another property return the actual property
				}

				properties.Add(prop);
			}
		}
	}
}