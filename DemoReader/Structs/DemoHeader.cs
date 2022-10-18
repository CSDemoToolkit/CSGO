using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace DemoReader
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct DemoHeader
    {
        private const int MAX_OSPATH = 260;

        fixed byte filestamp[8];
        public int protocol;
        public int networkProtocol;
        fixed byte serverName[MAX_OSPATH];
        fixed byte clientName[MAX_OSPATH];
        fixed byte mapName[MAX_OSPATH];
        fixed byte gameDirectory[MAX_OSPATH];
        public float playbackTime;
        public int playbackTicks;
        public int playbackFrames;
        public int signonLength;

        public string GetFilestamp()
        {
            return Encoding.UTF8.GetString(MemoryMarshal.CreateSpan(ref filestamp[0], 8));
        }

        public string GetServerName()
        {
            return Encoding.UTF8.GetString(MemoryMarshal.CreateSpan(ref serverName[0], MAX_OSPATH));
        }

        public string GetClientName()
        {
            return Encoding.UTF8.GetString(MemoryMarshal.CreateSpan(ref clientName[0], MAX_OSPATH));
        }

        public string GetMapName()
        {
            return Encoding.UTF8.GetString(MemoryMarshal.CreateSpan(ref mapName[0], MAX_OSPATH));
        }

        public string GetGameDirectory()
        {
            return Encoding.UTF8.GetString(MemoryMarshal.CreateSpan(ref gameDirectory[0], MAX_OSPATH));
        }
    }
}