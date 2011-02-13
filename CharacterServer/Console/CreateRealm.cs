using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

using Shared;

namespace CharacterServer
{
    [ConsoleHandler("realm", 3, "<id(<21),type(16-48-80),lang(0-1-3-5)>")]
    public class CreateRealm : IConsoleHandler
    {
        public bool HandleCommand(string command, List<string> args)
        {
            byte Id = byte.Parse(args[0]);
            byte Type = byte.Parse(args[1]);
            byte Lang = byte.Parse(args[2]);

            Realm Rm = Program.CharMgr.GetRealm(Id);
            if (Rm != null) // Realm already registered
            {
                Log.Error("CreateRealm", "Realm id : " + Id + " already exist");
                return false;
            }

            Rm = new Realm();
            Rm.RealmId = Id;
            Rm.Language = Lang;
            Rm.RealmType = Type;
            Rm.Online = 1;
            Rm.GenerateName(); // Generate name by RealmId;

            Program.CharMgr.AddObject(Rm);

            Log.Success("CreateRealm", "Realm '" + Rm.Name + "' Successsfully added to database.");
            return true;
        }
    }
}
