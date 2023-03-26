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

		public static ServerClass Parse(ref SpanStream<byte> stream, List<SendTable> dataTables)
        {
            var serverClass = new ServerClass();
            serverClass.classID = stream.ReadShort();
            serverClass.name = stream.ReadDataTableString();
            serverClass.dtName = stream.ReadDataTableString();
            serverClass.dataTableID = dataTables.FindIndex(a => a.netTableName == serverClass.dtName);

			/*
			//TODO: This can probably be further made readable but i cannot be bothered now
			List<SendProperty> pri = new List<SendProperty>();
			List<SendProperty> sec = new List<SendProperty>();

			GetProperties(dataTables[serverClass.dataTableID], dataTables, pri, sec, false);

			pri.AddRange(sec);
			*/

			/*
			var priorities = pri.Select(x => x.priority).Append(64).Distinct().Order();
			int idx = 0;
			foreach (var priority in priorities)
			{
				for (int i = idx; i < pri.Count; i++)
				{
					if (pri[i].priority == priority || (priority == 64 && pri[i].flags.HasFlag(SendPropertyFlags.ChangesOften)))
					{
						var tmp = pri[i];
						pri[i] = pri[idx];
						pri[idx] = tmp;

						idx++;
					}
				}
			}
			*/

			/*
			List<int> priorities = new List<int>();
			priorities.Add(64);
			priorities.AddRange(pri.Select(a => a.priority).Distinct());
			priorities.Sort();

			int start = 0;
			for (int priorityIndex = 0; priorityIndex < priorities.Count; priorityIndex++)
			{
				int priority = priorities[priorityIndex];

				while (true)
				{
					int currentProp = start;

					while (currentProp < pri.Count)
					{
						SendProperty prop = pri[currentProp];

						if (prop.priority == priority || (priority == 64 && prop.flags.HasFlag(SendPropertyFlags.ChangesOften)))
						{
							if (start != currentProp)
							{
								SendProperty temp = pri[start];
								pri[start] = pri[currentProp];
								pri[currentProp] = temp;
							}

							start++;
							break;
						}

						currentProp++;
					}

					if (currentProp == pri.Count)
					{
						break;
					}
				}
			}
			*/

			//serverClass.properties = pri.ToArray();

			var excludes = new List<SendProperty>();
			GetExcludes(dataTables[serverClass.dataTableID], dataTables, excludes, true);

			var flattendProps = new List<SendProperty>();
			GatherProps(dataTables[serverClass.dataTableID], flattendProps, excludes, dataTables);

			/*
			*/

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

			/*
			GetProperties(dataTables[serverClass.dataTableID], dataTables, pri, sec, false);

			//serverClass.properties.AddRange(pri);
			//serverClass.properties.AddRange(sec);
			//serverClass.properties.Sort((x, y) => x.priority.CompareTo(y.priority));

			pri.AddRange(sec);
			serverClass.properties.AddRange(pri.OrderBy(x => x.priority));
			*/

			return serverClass;
        }

		static void GetProperties(in SendTable table, List<SendTable> dataTables, List<SendProperty> pri, List<SendProperty> sec, bool isPri, int indent = 0)
		{
			var excludes = new List<SendProperty>();
			GetExcludes(table, dataTables, excludes, true);

			for (int i = 0; i < table.properties.Count; i++)
			{
				var prop = table.properties[i];

				// Ignore prop if element of array of excluded
				if (prop.flags.HasFlag(SendPropertyFlags.Exclude | SendPropertyFlags.InsideArray) || IsPropExcluded(table, prop, excludes))
					continue;

				switch (prop.type)
				{
					case SendPropertyType.DataTable:
						GetProperties(dataTables.Find(x => x.netTableName == prop.dtName), dataTables, pri, sec, !prop.flags.HasFlag(SendPropertyFlags.Collapsible), indent + 1); // Instead of returning ref to another property return the actual property
						break;
					default:
						if (prop.type == SendPropertyType.Array)
							prop.arrayElementProp = new(table.properties[i - 1]);

						if (isPri)
						{
							pri.Add(prop);
						}
						else
						{
							sec.Add(prop);
						}
						break;
				}
			}
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

		static void GatherProps(in SendTable table, List<SendProperty> flattenedProps, IEnumerable<SendProperty> excludes, List<SendTable> dataTables)
		{
			List<SendProperty> tmpFlattenedProps = new List<SendProperty>();
			GatherProps_IterateProps(table, tmpFlattenedProps, flattenedProps, excludes, dataTables);

			flattenedProps.AddRange(tmpFlattenedProps);
		}

		static void GatherProps_IterateProps(SendTable table, List<SendProperty> flattenedProps, List<SendProperty> aflattenedProps, IEnumerable<SendProperty> excludes, List<SendTable> dataTables)
		{
			//Console.WriteLine(table.Name);
			for (int i = 0; i < table.properties.Count; i++)
			{

				SendProperty property = table.properties[i];
				//Console.WriteLine($"    Type: {property.Type}:{property.Name} - {property.Flags.HasFlag(SendPropertyFlags.Exclude)} - {property.DataTableName}");

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
						GatherProps_IterateProps(subTable, flattenedProps, aflattenedProps, excludes, dataTables);
					}
					else
					{
						//We do however prefix everything else

						GatherProps(subTable, aflattenedProps, excludes, dataTables);
					}
				}
				else
				{
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