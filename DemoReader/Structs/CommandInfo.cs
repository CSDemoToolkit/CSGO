using System.Runtime.InteropServices;

namespace DemoReader
{
    [StructLayout(LayoutKind.Sequential)]
    struct CommandInfo
    {
        public Split split1;
        public Split split2;
    }
}