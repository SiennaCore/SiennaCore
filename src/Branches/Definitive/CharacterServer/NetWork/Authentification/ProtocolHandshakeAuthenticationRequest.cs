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
using System.IO;
using System.Xml.Serialization;

using Common;
using FrameWork;

namespace CharacterServer
{
    public class ClientAuthCertificate
    {
        public string Username;
        public string Hash;
        public string Sessionkey;
    }

    [ISerializableAttribute((long)Opcodes.ProtocolHandshakeAuthenticationRequest)]
    public class ProtocolHandshakeAuthenticationRequest : ISerializablePacket
    {
        [ArrayBit(0)]
        public string Signature;

        [ArrayBit(1)]
        public string Certificate;

        [ArrayBit(2)]
        public string Unk;

        public override void OnRead(RiftClient From)
        {
            try
            {
                XmlSerializer xmls = new XmlSerializer(typeof(ClientAuthCertificate));
                ClientAuthCertificate Cert = xmls.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(Certificate))) as ClientAuthCertificate;

                if (Program.Config.UseCertificate)
                    From.Acct = Program.AcctMgr.GetAccount(Cert.Sessionkey);
                else
                    From.Acct = Program.AcctMgr.GetAccount(Cert.Username, Cert.Hash);

            }
            catch (Exception e)
            {
                From.Disconnect();
            }

            if (From.Acct == null)
            {
                Log.Error("Authentication", "Failled !");
                From.Disconnect();
            }
            else
            {
                HandshakeAuthenticationResponse Rp = new HandshakeAuthenticationResponse();
                Rp.SessionTicket = (long)BitConverter.ToUInt64(Guid.NewGuid().ToByteArray(), 0);

                From.Acct.SessionTicket = Rp.SessionTicket;
                From.Acct.Dirty = true;
                AccountMgr.AccountDB.SaveObject(From.Acct);

                From.SendSerialized(Rp);
            }
        }
    }
}
