using System.Runtime.InteropServices;

namespace DemoReader
{
    [StructLayout(LayoutKind.Sequential)]
    struct Split
    {
        public int flags;
        public Vector3D viewOrigin;
        public Vector3D viewAngles;
        public Vector3D localViewAngles;

        public Vector3D viewOrigin2;
        public Vector3D viewAngles2;
        public Vector3D localViewAngles2;
    }
}