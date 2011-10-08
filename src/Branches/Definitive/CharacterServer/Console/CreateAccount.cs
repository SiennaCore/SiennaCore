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
using System.Security.Cryptography;

using Common;
using FrameWork;

namespace CharacterServer
{
    [ConsoleHandler("caccount", 2, "<Username,Password>")]
    public class CreateAccount : IConsoleHandler
    {
        public bool HandleCommand(string command, List<string> args)
        {
            Account Acct = Program.AcctMgr.GetAccountByUsername(args[0]);
            if (Acct != null)
            {
                Log.Error("CreateAccount", "Username '" + args[0] + "' Already exist.");
                return false;
            }

            Log.Debug("CreateAccount", "1");

            Acct = new Account();
            Acct.Username = args[0].ToUpper();
            Acct.Sha_Password = MakePassword(args[1]);
            Acct.SessionKey = "";
            Acct.Email = "";

            Log.Debug("CreateAccount", "2");

            if (AccountMgr.AccountDB.AddObject(Acct))
                Log.Success("CreateAccount", "New Account : '" + Acct.Username + "'-'" + Acct.Sha_Password + "'");
            else
                Log.Error("CreateAccount", "Can not create account : " + Acct.Username);

            Log.Debug("CreateAccount", "3");

            return true;
        }

        static public string MakePassword(string Password)
        {
            string Sha_Password = "";

            SHA1CryptoServiceProvider Crypto = new SHA1CryptoServiceProvider();
            Crypto.Initialize();
            byte[] Result = Crypto.ComputeHash(Encoding.UTF8.GetBytes(Password), 0, Encoding.UTF8.GetByteCount(Password));
            Sha_Password = BitConverter.ToString(Result).Replace("-", string.Empty).ToLower();

            return Sha_Password;
        }


    }
}
