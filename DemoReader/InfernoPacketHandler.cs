namespace DemoReader
{

	public class InfernoPacketHandler
	{
		const int MAX_EDICT_BITS = 11;
		const int INDEX_MASK = (1 << MAX_EDICT_BITS) - 1;

		DemoPacketHandler eventHandler;

		Guid INFERNO_ID;
		Guid OWNER_ENTITY_ID;

		public InfernoPacketHandler(DemoPacketHandler eventHandler)
		{
			this.eventHandler = eventHandler;
		}

		public void Init(Span<ServerClass> serverClasses)
		{
			INFERNO_ID = serverClasses.FindServerClass("CInferno").id;
			OWNER_ENTITY_ID = serverClasses.FindProperty("CInferno", "m_hOwnerEntity").id;
		}

		public void Execute(ref ServerClass serverClass, ref Entity entity, ref SendProperty property, int v)
		{
			if (serverClass.id == INFERNO_ID)
			{
				if (property.id == OWNER_ENTITY_ID)
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
			if (serverClass.id == INFERNO_ID)
			{
				eventHandler.infernos.Remove(entity.id);
			}
		}
	}
}
