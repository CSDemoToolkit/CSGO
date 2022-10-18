using System.Runtime.InteropServices;

namespace DemoReader
{
    [StructLayout(LayoutKind.Sequential)]
    struct Vector3D
    {
        public float x;
        public float y;
        public float z;

        public override string ToString()
        {
            return $"X: {x}, Y: {y}, Z: {z}";
        }
    }
}