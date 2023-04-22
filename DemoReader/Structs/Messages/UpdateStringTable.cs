namespace DemoReader
{
	public struct UpdateStringTable
	{
		public uint tableId;
		public uint changedEntries;

		public static UpdateStringTable Parse(ref SpanStream<byte> stream)
		{
			UpdateStringTable updateStringTable = new UpdateStringTable();
			while (!stream.IsEnd)
			{
				var desc = stream.ReadProtobufVarInt();
				var wireType = desc & 7;
				var fieldnum = desc >> 3;

				if (wireType == 2 && fieldnum == 3)
				{
					// Assume data is end of update string table.
					return updateStringTable;
				}

				if (wireType != 0)
				{
					throw new InvalidDataException();
				}

				var val = stream.ReadProtobufVarUInt();

				switch (fieldnum)
				{
					case 1:
						updateStringTable.tableId = val;
						break;
					case 2:
						updateStringTable.changedEntries = val;
						break;
					default:
						// silently drop
						break;
				}
			}

			return updateStringTable;
		}
	}
}