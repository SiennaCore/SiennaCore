/*
 * Copyright (C) 2011 APS
 *	http://AllPrivateServer.com
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{
    [Serializable]
    [ISerializableAttribute((long)Opcodes.ProtocolHandshakeClientKey)]
    public class HandshakeClientKey : ISerializablePacket
    {
        [ArrayBitAttribute(0)]
        public byte[] ClientKey;

        #region 1.2 Client

        //[ArrayBit(1)]
        //public byte[] Unknown;

        //[ArrayBit(2)]
        //public byte[] Unknown1;

        //[Encoded7BitAttribute(3)]
        //public long Version;

        #endregion

        public override void OnRead(RiftClient From)
        {
            Log.Dump("ClientKey", ClientKey, 0, ClientKey.Length);

            From.InitCrypto(ClientKey);

            HandshakeCompression Cp = new HandshakeCompression();
            Cp.Enabled = true;
            From.SendSerialized(Cp);
            From.EnableSendCompress();


            #region 1.1 Client

            HandshakeServerKey ServerKey = new HandshakeServerKey();
            ServerKey.Nid = 420;
            ServerKey.ServerKey = From.LocalPublicKey;
            From.SendSerialized(ServerKey);

            #endregion

            #region 1.2 Client

            //HandshakeServerKey ServerKey = new HandshakeServerKey();
            //ServerKey.Nid = 2980549511;
            //ServerKey.Unk = 3061945505;
            //ServerKey.ServerKey = From.LocalPublicKey;
            //From.SendSerialized(ServerKey);

            #endregion
        }
    }
}
