using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sienna.Game
{
    public enum LogonOpcodes
    {
        Client_AuthCertificate = 0xC502,
        Client_Ping = 0x0402,

        Server_AuthCertificate = 0x03AF,
        Server_Unk1   = 0x0402,
        Server_ZCompressStart = 0x0119
    }
}
