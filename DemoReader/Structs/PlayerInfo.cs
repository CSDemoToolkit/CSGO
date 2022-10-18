using System.Buffers.Binary;

namespace DemoReader
{
	public struct PlayerInfo
    {
        public long version;
        public long xuiID;
        public string name;
        public int userID;
		public string guid;
		public int friendsID;
		public string friendsName;
		public bool isFakePlayer;
		public bool isHLTV;

		public int customFiles0;
		public int customFiles1;
		public int customFiles2;
		public int customFiles3;

        private byte filesDownloaded;

        public static PlayerInfo Parse(ref SpanBitStream stream)
        {
            return new PlayerInfo()
            {
				version = BinaryPrimitives.ReverseEndianness(stream.ReadLong(64)),
			    xuiID = BinaryPrimitives.ReverseEndianness(stream.ReadLong(64)),
			    name = stream.ReadString(128),
			    userID = BinaryPrimitives.ReverseEndianness(stream.ReadInt(32)),
			    guid = stream.ReadString(33),
			    friendsID = BinaryPrimitives.ReverseEndianness(stream.ReadInt(32)),
			    friendsName = stream.ReadString(128),

			    isFakePlayer = stream.ReadBit(),
			    isHLTV = stream.ReadBit(),

			    customFiles0 = stream.ReadInt(32),
			    customFiles1 = stream.ReadInt(32),
			    customFiles2 = stream.ReadInt(32),
			    customFiles3 = stream.ReadInt(32),

			    filesDownloaded = stream.ReadByte(),
		    };
        }
	}
}