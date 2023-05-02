using System;
using System.Data;

namespace DemoReader
{
    public struct ServerClass
    {
        public short classID;
        public int dataTableID;
        public string name;
        public string dtName;
		public SendProperty[] properties;

		public Guid id;

		public static ServerClass Parse(ref SpanStream<byte> stream, List<SendTable> dataTables)
        {
            var serverClass = new ServerClass();
            serverClass.classID = stream.ReadShort();
            serverClass.name = stream.ReadDataTableString();
            serverClass.dtName = stream.ReadDataTableString();
            serverClass.dataTableID = dataTables.FindIndex(a => a.netTableName == serverClass.dtName);
			serverClass.id = Guid.NewGuid();

			//TODO: This can probably be further made readable but i cannot be bothered now

			var excludes = new List<SendProperty>();
			GetExcludes(dataTables[serverClass.dataTableID], dataTables, excludes, true);

			var flattendProps = new List<SendProperty>();
			GatherProps(dataTables[serverClass.dataTableID], flattendProps, excludes, dataTables, Guid.Empty, "");


			var priorities = flattendProps.Select(x => x.priority).Append(64).Distinct().Order();
			int idx = 0;
			foreach (var priority in priorities)
			{
				for (int i = idx; i < flattendProps.Count; i++)
				{
					if (flattendProps[i].priority == priority || (priority == 64 && flattendProps[i].flags.HasFlag(SendPropertyFlags.ChangesOften)))
					{
						var tmp = flattendProps[i];
						flattendProps[i] = flattendProps[idx];
						flattendProps[idx] = tmp;

						idx++;
					}
				}
			}

			serverClass.properties = flattendProps.ToArray();
			return serverClass;
        }

		static IEnumerable<SendProperty> GetExcludes(in SendTable table, List<SendTable> dataTables, List<SendProperty> excludes, bool collectBaseClasses)
		{
			excludes.AddRange(table.properties.Where(x => x.flags.HasFlag(SendPropertyFlags.Exclude)));

			foreach (var prop in table.properties.Where(x => x.type == SendPropertyType.DataTable))
			{
				if (collectBaseClasses && prop.varName == "baseclass")
				{
					GetExcludes(dataTables.Find(x => x.netTableName == prop.dtName), dataTables, excludes, true);
				}
				else
				{
					GetExcludes(dataTables.Find(x => x.netTableName == prop.dtName), dataTables, excludes, false);
				}
			}

			return excludes;
		}

		static bool IsPropExcluded(SendTable table, SendProperty property, IEnumerable<SendProperty> excludes)
		{
			return excludes.Any(x => table.netTableName == x.dtName && property.varName == x.varName);
		}

		static void GatherProps(in SendTable table, List<SendProperty> flattenedProps, IEnumerable<SendProperty> excludes, List<SendTable> dataTables, Guid parent, string parentName)
		{
			List<SendProperty> tmpFlattenedProps = new List<SendProperty>();
			GatherProps_IterateProps(table, tmpFlattenedProps, flattenedProps, excludes, dataTables, parent, parentName);

			flattenedProps.AddRange(tmpFlattenedProps);
		}

		static void GatherProps_IterateProps(SendTable table, List<SendProperty> flattenedProps, List<SendProperty> aflattenedProps, IEnumerable<SendProperty> excludes, List<SendTable> dataTables, Guid parent, string parentName)
		{
			for (int i = 0; i < table.properties.Count; i++)
			{

				SendProperty property = table.properties[i];

				if (property.flags.HasFlag(SendPropertyFlags.InsideArray) || property.flags.HasFlag(SendPropertyFlags.Exclude) ||
					IsPropExcluded(table, property, excludes))
				{
					continue;
				}

				if (property.type == SendPropertyType.DataTable)
				{
					SendTable subTable = dataTables.Find(x => x.netTableName == property.dtName);

					if (property.flags.HasFlag(SendPropertyFlags.Collapsible))
					{
						//we don't prefix Collapsible stuff, since it is just derived mostly
						GatherProps_IterateProps(subTable, flattenedProps, aflattenedProps, excludes, dataTables, Guid.Empty, "");
					}
					else
					{
						//We do however prefix everything else

						GatherProps(subTable, aflattenedProps, excludes, dataTables, property.id, property.varName);
					}
				}
				else
				{
					property.parent = parent;
					property.parentName = parentName;

					if (property.type == SendPropertyType.Array)
					{
						property.arrayElementProp = new(table.properties[i - 1]);

						flattenedProps.Add(property);
					}
					else
					{
						flattenedProps.Add(property);
					}
				}
			}
		}
	}
}