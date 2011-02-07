using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sienna.Game
{
    public enum LogonOpcodes
    {
        Client_AuthCertificate = 0x03C5,
        Client_RequestRealmlist = 0x03C0,
        Client_RequestCharacterlist = 0x03B3,
        Client_SelectRealm = 0x039D,
        Client_RequestCharCreationInfos = 0x03C2,
        Client_CreateCharacter = 0x3D5,

        Server_AuthCertificate = 0x03AF,
        Server_Ping   = 0x0402,
        Server_SendRealmlist = 0x03C1,
        Server_AcceptRealmSelection = 0x039E,
        Server_CreateCharacterResult = 0x03D6,
        Server_ZCompressStart = 0x0119
    }
}
