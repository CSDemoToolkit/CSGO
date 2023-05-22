using System.Runtime.CompilerServices;

namespace DemoReader
{
	public class WeaponPacketHandler
	{
		DemoPacketHandler packetHandler;

		Guid WEAPON_CLIP_ID;

		public WeaponPacketHandler(DemoPacketHandler packetHandler)
		{
			this.packetHandler = packetHandler;
		}

		public void Init(Span<ServerClass> serverClasses)
		{
			WEAPON_CLIP_ID = serverClasses.FindServerClass("CWeaponCSBase").id;
		}

		static List<int> ents = new List<int>();

		public unsafe void Execute(ref ServerClass serverClass, ref Entity entity, ref SendProperty property, int v)
		{
		}
	}
}
