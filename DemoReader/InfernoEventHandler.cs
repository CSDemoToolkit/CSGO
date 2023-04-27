namespace DemoReader
{
	public class InfernoEventHandler
	{
		const int MAX_EDICT_BITS = 11;
		const int INDEX_MASK = (1 << MAX_EDICT_BITS) - 1;

		DemoEventHandler eventHandler;

		public InfernoEventHandler(DemoEventHandler eventHandler)
		{
			this.eventHandler = eventHandler;
		}

		public void Execute(ref ServerClass serverClass, ref Entity entity, in SendProperty property, int v)
		{
			if (serverClass.name == "CInferno")
			{
				if (property.varName == "m_hOwnerEntity")
				{
					int playerID = v & INDEX_MASK;
					if (!eventHandler.infernos.TryAdd(entity.id, new Inferno { Owner = playerID }))
					{
						eventHandler.infernos[entity.id] = new Inferno { Owner = playerID };
					}
				}
			}
		}

		public void Destroy(ref ServerClass serverClass, ref Entity entity)
		{

			if (serverClass.name == "CInferno")
			{
				eventHandler.infernos.Remove(entity.id);
			}
		}
	}
}
