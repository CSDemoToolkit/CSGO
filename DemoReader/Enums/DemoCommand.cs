using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoReader
{
    enum DemoCommand
    {
        Signon = 1,
        Packet,
        Synctick,
        ConsoleCommand,
        UserCommand,
        DataTables,
        Stop,
        CustomData,
        StringTables,
    };
}
